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
            //var lands = _lands.GroupBy(x => x.LandNo8).Where(x => x.Count() == 3);
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
        public void ThreePieces_三個多邊形合併_沒有相交()
        {
            //00010003, 03760002
            var lands = _lands.Where(x => x.LandNo8 == "00010003").Select(x => new PolyLand
            {
                Code = x.Boundary,
                Points = x.Points
            }).ToList();
            var merger = new Merger(_boundaries);
            Polygon polygon = merger.ThreePieces(lands);
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
    }
}
