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
        public void TwoPieces_兩個多邊形合併_沒有相交()
        {
            //00480000, 04220000
            var lands = _lands.Where(x => x.LandNo8 == "04220000").Select(x=>new PolyLand
            {
                Code = x.Boundary,
                Points = x.Points
            }).ToList();
            var merger = new Merger(_boundaries);
            Polygon polygon = merger.TwoPieces(lands.First(), lands.Last());

            var pg1 = new Polygon(lands.First().Points);
            Assert.True(polygon.Contains(pg1));
            var pg2 = new Polygon(lands.Last().Points);
            Assert.True(polygon.Contains(pg2));
        }
        
        [Test]
        [TestCase("00010003", 4547.418451)]
        [TestCase("03760002", 8260.303000)]
        [TestCase("01310002", 3398.079426)]
        public void ThreePieces_三個多邊形合併_沒有相交(string landno, double area)
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
            
            foreach (var land in lands)
            {
                var pg = new Polygon(land.Points);
                Assert.True(polygon.Contains(pg));
            }
        }
       
        
        [Test]
        public void ThreePieces_四個多邊形合併()
        {
            var lands = _lands.Where(x => x.LandNo8 == "01310002").Select(x => new PolyLand
            {
                Code = x.Boundary,
                Points = x.Points
            }).ToList();
            var merger = new Merger(_boundaries);
            var polygon = merger.ThreePieces(lands);
            Assert.NotNull(polygon);
            Assert.AreEqual(3398.079426, polygon.Area, 0.000001);
            foreach (var land in lands)
            {
                var pg = new Polygon(land.Points);
                Assert.True(polygon.Contains(pg));
            }
        }
        [Test]
        public void ThreePieces_五個多邊形合併()
        {
            //04050000, 04060000, 01440007
            var lands = _lands.Where(x => x.LandNo8 == "01440007").Select(x => new PolyLand
            {
                Code = x.Boundary,
                Points = x.Points
            }).ToList();
            var merger = new Merger(_boundaries);
            var polygon = merger.ThreePieces(lands);
            foreach (var land in lands)
            {
                var pg = new Polygon(land.Points);
                Assert.True(polygon.Contains(pg));
            }
        }
        [Test]
        public void ThreePieces_MoreThenFive()
        {
            //04020000, 13940000
            var lands = _lands.Where(x => x.LandNo8 == "04020000").Select(x => new PolyLand
            {
                Code = x.Boundary,
                Points = x.Points
            }).ToList();
            var merger = new Merger(_boundaries);
            var polygon = merger.ThreePieces(lands);
            foreach (var land in lands)
            {
                var pg = new Polygon(land.Points);
                Assert.True(polygon.Contains(pg));
            }
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

        //{"Code":"03020015","Points":[{"M":"NaN","X":275503.477994109,"Y":2757594.7833203459,"Z":"NaN","NumOrdinates":2},{"M":"NaN","X":275466.72964286612,"Y":2757591.8214018894,"Z":"NaN","NumOrdinates":2},{"M":"NaN","X":275429.94121402991,"Y":2757588.9532408221,"Z":"NaN","NumOrdinates":2},{"M":"NaN","X":275429.93469449208,"Y":2757545.8932697494,"Z":"NaN","NumOrdinates":2},{"M":"NaN","X":275500.75199733494,"Y":2757545.8642775714,"Z":"NaN","NumOrdinates":2},{"M":"NaN","X":275503.22490233264,"Y":2757555.2275553606,"Z":"NaN","NumOrdinates":2},{"M":"NaN","X":275508.72536459984,"Y":2757576.5936708814,"Z":"NaN","NumOrdinates":2},{"M":"NaN","X":275508.03962494305,"Y":2757584.0420535933,"Z":"NaN","NumOrdinates":2},{"M":"NaN","X":275507.418591571,"Y":2757591.4964217525,"Z":"NaN","NumOrdinates":2},{"M":"NaN","X":275503.477994109,"Y":2757594.7833203459,"Z":"NaN","NumOrdinates":2}]}
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
    }
}
