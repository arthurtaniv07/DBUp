using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class Setting
    {
        /// <summary>
        /// 是否调试
        /// </summary>
        public bool IsDebug { get; set; } = true;
        /// <summary>
        /// 比较注释
        /// </summary>
        public bool CheckCommon { get; set; } = false;

        public bool IsSearTable { get; set; } = true;
        public bool IsSearView { get; set; } = true;
        public bool IsSearProc { get; set; } = true;
        public bool IsSearFunc { get; set; } = true;
        public bool IsSearTri { get; set; } = true;
        public bool IsDiff { get; set; } = true;
        /// <summary>
        /// 如果有值 则从这个路径获取数据
        /// </summary>
        public string FileDataPath { get; set; }

        public bool IsFileDataPath_Table { get; set; } = false;
        public bool IsFileDataPath_View { get; set; } = false;
        public bool IsFileDataPath_Trig { get; set; } = false;
        public bool IsFileDataPath_Func { get; set; } = false;
        public bool IsFileDataPath_Proc { get; set; } = false;
    }
}
