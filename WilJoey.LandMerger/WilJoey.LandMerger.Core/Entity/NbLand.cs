using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Topology;
using Newtonsoft.Json;

namespace WilJoey.LandMerger.Core.Entity
{
    public class NbLand : ICloneable
    {
        /// <summary>
        /// 地政事務所代碼
        /// </summary>
        public string Office { get; set; }

        /// <summary>
        /// 所屬地段
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// 地號，NBB原始唯一地號
        /// </summary>
        public string LandNo { get; set; }

        /// <summary>
        /// 地號，固定八碼
        /// </summary>
        public string LandNo8 { get; set; }

        /// <summary>
        /// 延伸地號，一碼
        /// </summary>
        public string MapNo { get; set; }

        /// <summary>
        /// 完整八碼地段代碼
        /// </summary>
        public string Boundary { get; set; }

        /// <summary>
        /// 邊界線陣列
        /// </summary>
        //public List<NbbLine> Lines { get; set; }

        /// <summary>
        /// 地籍節點陣列
        /// </summary>
        public List<Coordinate> Points { get; set; }

        /// <summary>
        /// 地籍 Geometry 資訊
        /// </summary>
        [JsonIgnore]
        public IPolygon Geometry { get; set; }

        /// <summary>
        /// "m" OR 四碼延伸地號
        /// </summary>
        ///
        public string Note { get; set; }

        /// <summary>
        /// 建立目前執行個體複本的新物件。
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new NbLand()
            {
                Office = this.Office,
                Section = this.Section,
                LandNo8 = this.LandNo8,
                LandNo = this.LandNo + "x",
                Note = this.Note,
                MapNo = "x",
                Geometry = null,
                Boundary = string.Empty,
                //Lines = new List<NbbLine>(),
                Points = new List<Coordinate>()
            };
        }
    }
}
