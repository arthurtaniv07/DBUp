using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    /// <summary>
    /// 视图信息
    /// </summary>
    public class ViewInfo : DBInfo
    {
        public string Name { get; set; }
        /// <summary>
        /// 创建语句
        /// </summary>
        public string CreateSQL { get; set; }
        /// <summary>
        /// Charset字符集（Client）
        /// </summary>
        public string ClientCharSet { get; set; }
        /// <summary>
        /// Charset字符集
        /// </summary>
        public string CharSet { get; set; }
    }
}
