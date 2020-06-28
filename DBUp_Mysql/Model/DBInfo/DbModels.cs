using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    /// <summary>
    /// 
    /// </summary>
    public class DbModels
    {
        public DBSetting DbModel { get; set; }
        /// <summary>
        /// 表
        /// </summary>
        public Dictionary<string, TableInfo> Tables { get; set; }
        /// <summary>
        /// 视图
        /// </summary>
        public Dictionary<string, ViewInfo> Views { get; set; }
        /// <summary>
        /// 触发器
        /// </summary>
        public Dictionary<string, Trigger> Triggers { get; set; }
        /// <summary>
        /// 函数
        /// </summary>
        public Dictionary<string, Function> Functions { get; set; }
        /// <summary>
        /// 存储过程
        /// </summary>
        public Dictionary<string, Function> Procs { get; set; }
    }
}
