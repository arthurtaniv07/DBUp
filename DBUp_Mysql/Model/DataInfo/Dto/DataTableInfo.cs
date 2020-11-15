using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql.Model
{
    /// <summary>
    /// 數據表詳情
    /// </summary>
    public class DataTableInfo
    {
        public string TableName { get; set; }
        public List<string> Unique { get; set; }
        public List<string> Columns { get; set; }
        public List<DefaultColumn> DefaultColumns { get; set; }
    }
    public class DefaultColumn
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
