using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Topology;
using Newtonsoft.Json;

namespace WilJoey.LandMerger.Core.Entity
{
    /// <summary>
    /// 圖幅框資訊
    /// </summary>
    public class MapBox
    {
        /// <summary>
        /// 圖幅編號
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 節點清單
        /// </summary>
        public List<Coordinate> Points { get; set; }

        /// <summary>
        /// 圖幅框
        /// </summary>
        [JsonIgnore]
        public LineString Extents { get; set; }

        /// <summary>
        /// 圖幅框
        /// </summary>
        public Polygon Box { get; set; }

        /// <summary>
        /// 鄰接的圖幅框編號
        /// </summary>
        public List<string> Neighbors { get; set; }
    }
}
