using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Topology;

namespace WilJoey.LandMerger.Core
{
    public class Merger
    {
        public Polygon TwoPieces(Polygon polygon1, Polygon polygon2, LineString line1, LineString line2)
        {
            if (polygon1.Intersects(polygon2) || polygon1.Touches(polygon2))
            {
                return polygon1.Union(polygon2) as Polygon;
            }
            else
            {
                var pg = GetConvexHull(line1, line2);
                return pg.Union(polygon1).Union(polygon2) as Polygon;
            }
        }

        public Polygon GetConvexHull(LineString line1, LineString line2)
        {
            var coords1 = new List<Coordinate>
            {
                line1.StartPoint.Coordinate,
                line1.EndPoint.Coordinate,
                line2.StartPoint.Coordinate,
                line2.EndPoint.Coordinate,
                line1.StartPoint.Coordinate
            };
            var pg1 = new Polygon(coords1);

            var coords2 = new List<Coordinate>
            {
                line1.StartPoint.Coordinate,
                line1.EndPoint.Coordinate,
                line2.EndPoint.Coordinate,
                line2.StartPoint.Coordinate,
                line1.StartPoint.Coordinate
            };
            var pg2 = new Polygon(coords2);
            return pg1.Area > pg2.Area ? pg1 : pg2;
            //return line1.Union(line2).ConvexHull() as Polygon;
        }
    }
}
