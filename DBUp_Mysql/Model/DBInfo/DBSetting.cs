using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    /// <summary>
    /// 数据库全局信息
    /// </summary>
    public class DBSetting : DBInfo
    {
        public string Name { get; set; }
        /// <summary>
        /// 远程主机名
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 逻辑库名称 可理解为数据库名称
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// SqlMode
        /// </summary>
        public string SqlMode { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        ///  事务隔离级别  <see cref="DBTrancstionConst"/>
        /// </summary>
        public string TransactionIsolationType { get; set; }
    }
}
