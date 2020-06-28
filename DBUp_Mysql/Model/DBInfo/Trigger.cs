using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    /// <summary>
    /// 触发器
    /// </summary>
    public class Trigger : DBInfo
    {
        public string Name { get; set; }
        /// <summary>
        /// 触发器类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public TriggerEvent Event { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Statement { get; set; }
        /// <summary>
        /// 触发事件
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public TeiggerTime Time { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SQLMode { get; set; }
        public string Definer { get; set; }
        public string ClientCharSet { get; set; }
        public string CharSet { get; set; }
    }
}
