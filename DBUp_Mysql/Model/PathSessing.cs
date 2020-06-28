using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class PathSetting
    {
        public string Path { get; set; } = "pathSetting.txt";
        public string Setting { get; set; } = "Setting.txt";
        public string Tables { get; set; } = "Tables.txt";
        public string Views { get; set; } = "Views.txt";
        public string Procs { get; set; } = "Procs.txt";
        public string Funcs { get; set; } = "Funcs.txt";
        public string Trigs { get; set; } = "Trigs.txt";
        public string DBSetting { get; set; } = "DB.txt";

    }
}
