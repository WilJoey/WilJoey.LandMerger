using System.Collections.Generic;
using DotSpatial.Topology;
using Newtonsoft.Json;

namespace WilJoey.LandMerger.Core.Entity
{
    /// <summary>
    /// 多個地籍合併時使用的地籍物件
    /// </summary>
    public class PolyLand
    {
        /// <summary>
        /// 所屬圖幅編號
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 形成多邊形的所有點序
        /// </summary>
        public List<Coordinate> Points { get; set; }


        /// <summary>
        /// 此多邊形的邊界線，由圖幅BOUNDARY判斷出來
        /// </summary>
        [JsonIgnore]
        public List<LineString> Borders { get; set; }
    }
}
