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
        /// <summary>
        /// 是否获取表
        /// </summary>
        public bool IsSearTable { get; set; } = true;
        /// <summary>
        /// 是否获取视图
        /// </summary>
        public bool IsSearView { get; set; } = true;
        /// <summary>
        /// 是否获取存储过程
        /// </summary>
        public bool IsSearProc { get; set; } = true;
        /// <summary>
        /// 是否获取函数
        /// </summary>
        public bool IsSearFunc { get; set; } = true;
        /// <summary>
        /// 是否获取触发器
        /// </summary>
        public bool IsSearTri { get; set; } = true;
        /// <summary>
        /// 是否获取数据库
        /// </summary>
        public bool IsSearDB { get; set; } = true;

        /// <summary>
        /// 是否执行比较
        /// </summary>
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
