using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotSpatial.Topology;
using Newtonsoft.Json;
using NUnit.Framework;
using WilJoey.LandMerger.Core;
using WilJoey.LandMerger.Core.Entity;

namespace WilJoey.LandMerger.CoreTests
{
    /// <summary>
    /// Summary description for GeometryFileTest
    /// </summary>
    [TestFixture]
    public class GeometryFileTest
    {
        private List<NbLand> _lands;
        private List<Boundary> _boundaries;

        #region Setup

        
        [SetUp]
        public void Setup()
        {
            _lands = SetupLands();
            _boundaries = SetupBoundary();
        }

        public static List<NbLand> SetupLands()
        {
            using (var sr = new StreamReader("Data/Samples.json"))
            {
                var json = sr.ReadToEnd();

                var lands = JsonConvert.DeserializeObject<List<NbLand>>(json);
                foreach (var land in lands)
                {
                    land.Geometry = new Polygon(land.Points);
                }
                return lands;
            }
        }
        public static List<Boundary> SetupBoundary()
        {
            //0302
            using (var sr = new StreamReader("Data/Boundary.json"))
            {
                var json = sr.ReadToEnd();
                var boundaries = JsonConvert.DeserializeObject<List<Boundary>>(json);
                boundaries.ForEach(x => x.Extents = new LineString(x.BorderLine));
                return boundaries;
            }
        }
        #endregion

        [Test]
        public void TestBoundariesHaveData()
        {
            Assert.AreEqual(33,_boundaries.Count());
            Assert.AreEqual(45, _lands.Count());
            var filter = _lands.GroupBy(x => x.LandNo8);
            Assert.AreEqual(10, filter.Count());
            var merger = new Merger(_boundaries);
        }
    }
}
