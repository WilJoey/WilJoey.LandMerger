using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Topology;
using NUnit.Framework;
using WilJoey.LandMerger.Core;
using WilJoey.LandMerger.Core.Entity;

namespace WilJoey.LandMerger.CoreTests
{
    [TestFixture]
    public class MergerTest
    {
        private List<NbLand> _lands;
        private List<Boundary> _boundaries;

        [SetUp]
        public void Setup()
        {
            _lands = GeometryFileTest.SetupLands();
            _boundaries = GeometryFileTest.SetupBoundary();
            //2013
            //var lands = _lands.GroupBy(x => x.LandNo8).Where(x => x.Count() == 5);
            //foreach (var land in lands)
            //{
            //    System.Diagnostics.Debug.WriteLine(land.Key);
            //}
        }

        [Test]
        [TestCase("04220000", 1946.649559)]
        [TestCase("00480000", 3892.695382)]
        public void TwoPieces_兩個多邊形合併_沒有相交(string landno, double area)
        {
            var lands = _lands.Where(x => x.LandNo8 == landno).Select(x => new PolyLand
            {
                Code = x.Boundary,
                Points = x.Points
            }).ToList();
            var merger = new Merger(_boundaries);
            var polygon = merger.TwoPieces(lands.First(), lands.Last());
            Assert.NotNull(polygon);
            Assert.AreEqual(area, polygon.Area, 0.000001);
            
            var pg1 = new Polygon(lands.First().Points);
            Assert.True(polygon.Contains(pg1));
            var pg2 = new Polygon(lands.Last().Points);
            Assert.True(polygon.Contains(pg2));
        }
        
        [Test]
        [TestCase("00010003", 4547.418451)]
        [TestCase("03760002", 8260.303000)]
        [TestCase("01310002", 3398.079426)] //4
        [TestCase("01440007", 10273.585447)] //5
        [TestCase("04060000", 20657.584259)] //5
        [TestCase("04020000", 58791.182574)] //6 more
        [TestCase("13940000", 56432.201192)] //6 more
        [TestCase("04050000", 20126.188562)] //6 more * 尖角
        public void ThreePieces_三個以上多邊形合併(string landno, double area)
        {
            TestThreePieces(landno, area);
        }

        private void TestThreePieces(string landno, double area)
        {
            var lands = _lands.Where(x => x.LandNo8 == landno).Select(x => new PolyLand
            {
                Code = x.Boundary,
                Points = x.Points
            }).ToList();
            var merger = new Merger(_boundaries);
            var polygon = merger.ThreePieces(lands);
            Assert.NotNull(polygon);
            Assert.AreEqual(area, polygon.Area, 0.000001);

            //foreach (var land in lands)
            //{
            //    var pg = new Polygon(land.Points);
            //    Assert.True(polygon.Contains(pg));
            //}
        }

        [Test]
        public void ThreePieces_多邊形單獨測試()
        {
            TestThreePieces("04060000", 20657.584259);

        }

        [Test]
        public void SetupBorders_()
        {
            var lands = _lands.Where(x => x.LandNo8 == "01310002").Select(x => new PolyLand
            {
                Code = x.Boundary,
                Points = x.Points
            }).ToList();
            var merger = new Merger(_boundaries);
            var polyLand = lands.First();
            merger.SetupBorders(polyLand);
            Assert.AreEqual(2, polyLand.Borders.Count);
        }


        [Test]
        public void GetConvexHull_由兩條邊界線產生一個多邊形()
        {
            var coords1 = new List<Coordinate>
            {
                new Coordinate(1, 1),
                new Coordinate(1, 2)
            };
            var line1 = new LineString(coords1);
            var coords2 = new List<Coordinate>
            {
                new Coordinate(2, 1),
                new Coordinate(2, 2)
            };
            var line2 = new LineString(coords2);

            var merger = new Merger(_boundaries);
            var result = merger.GetConvexHull(line1, line2);

            Assert.NotNull(result);
            Assert.AreEqual(1.0, result.Area, 0.00000001);
        }

        [Test]
        public void SetupBorders_當邊界線位於四個角落_應該切為兩段()
        {
            var polyland = new PolyLand()
            {
                Code = "03020015",
                Points = new List<Coordinate>()
                {
                    new Coordinate(275503.47799410898,2757594.7833203459),
                    new Coordinate(275466.72964286612,2757591.8214018894),
                    new Coordinate(275429.94121402991,2757588.9532408221),
                    new Coordinate(275429.93469449208,2757545.8932697494),
                    new Coordinate(275500.75199733494,2757545.8642775714),
                    new Coordinate(275503.22490233264,2757555.2275553606),
                    new Coordinate(275508.72536459984,2757576.5936708814),
                    new Coordinate(275508.03962494305,2757584.0420535933),
                    new Coordinate(275507.41859157098,2757591.4964217525),
                    new Coordinate(275503.47799410898,2757594.7833203459)
                }
            };
            
            var merger = new Merger(_boundaries);
            merger.SetupBorders(polyland);
            Assert.AreEqual(2, polyland.Borders.Count);
        }

        [Test]
        public void SetupBorders_Test2()
        {
            var lands = _lands.Where(x => x.LandNo8 == "13940000").Select(x => new PolyLand
            {
                Code = x.Boundary,
                Points = x.Points
            }).ToList();
            var polyland = lands.First(x => x.Code == "03020028");
            var merger = new Merger(_boundaries);
            merger.SetupBorders(polyland);
            Assert.AreEqual(2, polyland.Borders.Count);
        }
    }
}
