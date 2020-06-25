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
        /// 输出注释
        /// </summary>
        public bool OutputComment { get; set; } = true;
        /// <summary>
        /// 输出删除的sql
        /// </summary>
        public bool OutputDeleteSql { get; set; } = true;
        /// <summary>
        /// 是否将删除语句输出为注释 （不受<see cref="OutputComment"/>影响）
        /// </summary>
        public bool OutputDeleteSqlIsCommon { get; set; } = false;
    }
}
