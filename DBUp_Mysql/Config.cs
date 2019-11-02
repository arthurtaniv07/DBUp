using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class Config
    {
        public string Ver { get; set; } = "V1.0";
        public DBConnection OldConnection { get; set; }
        public DBConnection NewConnection { get; set; }
        public PathSetting OldPathSetting { get; set; }
        public PathSetting NewPathSetting { get; set; }
        public PathSetting DiffPathSetting { get; set; }
        public Setting Setting { get; set; }
    }
}
