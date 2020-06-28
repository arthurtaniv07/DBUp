using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    /// <summary>
    /// 函数 存储过程 信息
    /// </summary>
    public class Function : DBInfo
    {
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FunctionEnum Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Definer { get; set; }
        /// <summary>
        /// 创建时间 Proc无此项
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// 修改时间 Proc无此项
        /// </summary>
        public DateTime Modified { get; set; }
        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

        public string ClientCharSet { get; set; }
        public string CharSet { get; set; }
        public FunctionInfo Info { get; set; }

    }


    public class FunctionInfo
    {
        public string Name { get; set; }
        public string SQLModel { get; set; }
        public string CreateSQL { get; set; }

        public string ClientCharSet { get; set; }
        public string CharSet { get; set; }
    }

}
