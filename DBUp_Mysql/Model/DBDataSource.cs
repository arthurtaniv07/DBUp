using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class DBDataSource
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string ProviderName { get; set; }
        public DBDataSourceType Type { get; set; }
    }

}