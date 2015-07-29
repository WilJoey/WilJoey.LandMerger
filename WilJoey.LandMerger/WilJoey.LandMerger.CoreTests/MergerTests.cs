using System.Collections.Generic;
using System.Linq;
using DotSpatial.Topology;
using NUnit.Framework;
using WilJoey.LandMerger.Core;

namespace WilJoey.LandMerger.CoreTests
{
    [TestFixture]
    public class MergerTests
    {
        [Test]
        public void TwoPieces_兩個多邊形有交集()
        {
            var coords1 = new List<Coordinate>
            {
                new Coordinate(1,1),
                new Coordinate(2.11,1),
                new Coordinate(2.1,2),
                new Coordinate(1.9,2),
                new Coordinate(1,1)
            };
            var p1 = new Polygon(coords1);
            var coords2 = new List<Coordinate>
            {
                new Coordinate(2,1),
                new Coordinate(3.1,1),
                new Coordinate(3.1,2),
                new Coordinate(2.1,2),
                new Coordinate(2, 1)
            };
            var p2 = new Polygon(coords2);
            var merger = new Merger();
            var result = merger.TwoPieces(p1, p2, null, null);

            Assert.NotNull(result);
            Assert.AreEqual(1.6500, result.Area, 0.00000001);
        }

        [Test]
        public void TwoPiecees_兩個多邊形僅Touch()
        {
            var coords1 = new List<Coordinate>
            {
                new Coordinate(1, 1),
                new Coordinate(2, 1),
                new Coordinate(2, 2),
                new Coordinate(1, 2),
                new Coordinate(1, 1)
            };
            var p1 = new Polygon(coords1);
            var coords2 = new List<Coordinate>
            {
                new Coordinate(2, 1),
                new Coordinate(3, 1),
                new Coordinate(3, 2),
                new Coordinate(2, 2),
                new Coordinate(2, 1)
            };
            var p2 = new Polygon(coords2);

            var merger = new Merger();
            var result = merger.TwoPieces(p1, p2, null, null);

            Assert.NotNull(result);
            Assert.AreEqual(2.0, result.Area, 0.00000001);
        }

        [Test]
        public void TwoPieces_兩個多邊形沒有相交_須提供各自的邊界線()
        {
            
        }

        [Test]
        public void ConvexHull_由兩條線段產生多邊形()
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

            var merger = new Merger();
            var result = merger.GetConvexHull(line1, line2);

            Assert.NotNull(result);
            Assert.AreEqual(1.0, result.Area, 0.00000001);

        }

        //private List<Coordinate> GetCoordinateList(double[][] coors)
        //{
        //    //return coors.Select(x => new Coordinate()
        //    //{
        //    //    x.First()
        //    //}).ToList();
        //    //foreach (var coor in coors)
        //    //{
                
        //    //}
        //}
    }
}