using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Topology;
using Newtonsoft.Json;

namespace WilJoey.LandMerger.Core.Entity
{
    /// <summary>
    /// 個圖幅框資訊
    /// </summary>
    public class Boundary
    {
        /// <summary>
        /// 地政事務所代碼
        /// </summary>
        public string Office { get; set; }

        /// <summary>
        /// 完整地段代碼，"02551001"
        /// </summary>
        public string FullCode { get; set; }

        /// <summary>
        /// 段代碼 "0255" 前四碼
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// 圖幅段代碼，不同的圖幅段會有不同的六參數，"02551001"中的第五碼
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 圖幅段內的分圖代碼，"02551001"中的末三碼"001"
        /// </summary>
        public string Map { get; set; }

        /// <summary>
        /// 圖幅框邊線
        /// </summary>
        public List<Coordinate> BorderLine { get; set; }

        /// <summary>
        /// 圖幅框
        /// </summary>
        [JsonIgnore]
        public LineString Extents { get; set; }

        /// <summary>
        /// 回傳 JSON 物件字串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{{Office:'{0}',FullCode:'{1}',Section:'{2}',Region:'{3}',Map:'{4}'}}",
                Office,
                FullCode,
                Section,
                Region,
                Map);
        }
    }
}
