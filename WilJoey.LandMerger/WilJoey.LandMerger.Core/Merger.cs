using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Topology;
using WilJoey.LandMerger.Core.Entity;

namespace WilJoey.LandMerger.Core
{
    public class Merger
    {
        private readonly double _tolerance = 0.3;
        public List<MapBox> MapBoxs { get; set; }

        public Merger(List<Boundary> boundaries)
        {
            MapBoxs = SetupMapBox(boundaries);
        }

        private List<MapBox> SetupMapBox(List<Boundary> boundaries)
        {
            var mapBoxs = boundaries.Select(x => new MapBox
            {
                Code = x.FullCode,
                Points = x.BorderLine,
                Extents = x.Extents,
                Box = new Polygon(x.BorderLine)
            }).ToList();

            var avgX = mapBoxs.Average(x => (x.Box.Envelope.Right() - x.Box.Envelope.Left()) / 2);
            var avgY = mapBoxs.Average(x => (x.Box.Envelope.Top() - x.Box.Envelope.Bottom()) / 2);
            //對角線長度
            var diagonal = Math.Sqrt(avgX * avgX + avgY * avgY) * 0.9;
            avgX *= 1.1;
            avgY *= 1.1;

            foreach (var mapBox in mapBoxs)
            {
                mapBox.Neighbors = mapBoxs.Where(x =>
                    Math.Abs(x.Box.Envelope.Right() - mapBox.Box.Centroid.X) <= avgX || Math.Abs(x.Box.Envelope.Left() - mapBox.Box.Centroid.X) <= avgX
                    && Math.Abs(x.Box.Envelope.Top() - mapBox.Box.Centroid.X) <= avgY || Math.Abs(x.Box.Envelope.Bottom() - mapBox.Box.Centroid.X) <= avgY
                    && x.Box.Envelope.Center().Distance(mapBox.Box.Envelope.Center()) <= diagonal
                ).Select(x => x.Code).ToList();
                //移掉自己
                mapBox.Neighbors.Remove(mapBox.Code);
            }
            return mapBoxs;
        }

        /// <summary>
        /// 兩塊地籍合併
        /// </summary>
        /// <param name="polyLand1">第一塊地籍</param>
        /// <param name="polyLand2">第二塊地籍</param>
        /// <returns>合併後的多邊形</returns>
        public Polygon TwoPieces(PolyLand polyLand1, PolyLand polyLand2)
        {
            var polygon1 = new Polygon(polyLand1.Points);
            var polygon2 = new Polygon(polyLand2.Points);
            if (polygon1.Intersects(polygon2) || polygon1.Touches(polygon2))
            {
                return polygon1.Union(polygon2) as Polygon;
            }
            else
            {
                SetupBorders(polyLand1);
                SetupBorders(polyLand2);
                var list = new List<Polygon>();
                foreach (var border1 in polyLand1.Borders)
                {
                    foreach (var border2 in polyLand2.Borders)
                    {
                        var pg = GetConvexHull(border1, border2);
                        if (pg.Centroid.Distance(border1) <= _tolerance
                            && pg.Centroid.Distance(border2) <= _tolerance
                            )
                        {
                            pg = pg.Union(polygon1).Union(polygon2) as Polygon;
                            list.Add(pg);
                        }
                    }
                }
                switch (list.Count)
                {
                    case 0:
                        return null;
                    case 1:
                        return list.First();
                    default:
                        var result = list.First();
                        list.Remove(result);
                        foreach (var item in list)
                        {
                            result = result.Union(item) as Polygon;
                        }
                        return result;
                }
            }
        }

        /// <summary>
        /// 三塊地籍合併
        /// </summary>
        /// <param name="lands">三塊地籍清單</param>
        /// <returns>合併後的多邊形</returns>
        public Polygon ThreePieces(List<PolyLand> lands)
        {
            var polys = new List<Polygon>();
            while (lands.Count > 0)
            {
                var land = lands.First();
                lands.Remove(land);
                var codes = MapBoxs.First(x => x.Code == land.Code).Neighbors;
                var boxs = MapBoxs.Where(x => codes.Contains(x.Code));
                foreach (var box in boxs)
                {
                    var neighbors = lands.Where(x => x.Code == box.Code);
                    foreach (var neighbor in neighbors)
                    {
                        //System.Diagnostics.Debug.WriteLine();
                        var merged = TwoPieces(land, neighbor);
                        if (merged != null)
                        {
                            polys.Add(merged);
                        }
                    }
                }
            }
            if (polys.Count == 0)
            {
                return null;
            }
            else
            {
                var result = polys.First();
                polys.Remove(result);
                result = polys.Aggregate(result, (current, poly) => current.Union(poly) as Polygon);
                return result;
            }
        }

        /// <summary>
        /// 兩個未相交多邊形合併時，依照邊界線先產出中間空白的多邊形
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public Polygon GetConvexHull(LineString line1, LineString line2)
        {
            return line1.Union(line2).ConvexHull() as Polygon;
        }

        /// <summary>
        /// 取得多邊形所有的圖幅框邊界線
        /// </summary>
        /// <param name="polyLand">要判斷的多邊形</param>
        public void SetupBorders(PolyLand polyLand)
        {
            polyLand.Borders = new List<LineString>();
            var boundary = MapBoxs.First(x => x.Code == polyLand.Code);
            LineString prev = null;
            for (var i = 1; i < polyLand.Points.Count; i++)
            {
                var ls = GetLineString(polyLand.Points[i - 1], polyLand.Points[i]);
                if (IsLineTouchesBoundary(ls, boundary.Extents))
                {
                    if (prev != null)
                    {
                        //prev = prev.Union(ls) as LineString;
                        prev.Coordinates.Add(polyLand.Points[i]);
                    }
                    else
                    {
                        polyLand.Borders.Add(ls);
                        prev = ls;
                    }
                }
                else
                {
                    prev = null;
                }
            }
        }

        /// <summary>
        /// 檢驗線段是否與圖幅框接觸，起點、終點、中間點都要比對
        /// </summary>
        /// <param name="line">要被檢驗的線段</param>
        /// <param name="extent">圖幅框</param>
        /// <returns></returns>
        public bool IsLineTouchesBoundary(ILineString line, LineString extent)
        {
            return line.Centroid.Distance(extent) < _tolerance &&
                   line.StartPoint.Distance(extent) < _tolerance &&
                   line.EndPoint.Distance(extent) < _tolerance;
        }

        /// <summary>
        /// 傳入兩點，產生LineString物件
        /// </summary>
        /// <param name="start">起點</param>
        /// <param name="end">終點</param>
        /// <returns></returns>
        public static LineString GetLineString(Coordinate start, Coordinate end)
        {
            return new LineString(new List<Coordinate>
            {
                start, end
            });
        }
    }
}