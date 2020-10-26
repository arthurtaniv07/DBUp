using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    class Enums
    {
    }
    #region TableOption Enums
    /// <summary>
    /// 引擎
    /// </summary>
    public enum TableEngine
    {
        Performance_Schema,
        CSV,
        Blackhole,
        InnoDB,
        Memory,
        Archive,
        MyISAM
    }
    /// <summary>
    /// 记录格式
    /// </summary>
    public enum TableRowFormat
    {
        DEFAULT,
        FIXED,
        DYNAMIC,
        COMPRESSED,
        REDUNDANT,
        COMPACT
    }
    /// <summary>
    /// 检查记录和
    /// </summary>
    public enum TableChecksum
    {
        UnCheck = 0,
        Checked = 1
    }
    ///// <summary>
    ///// 统计数据持久 null 0 1 default
    ///// </summary>
    //public enum TableStatsPersistent
    //{
    //    No = 0,
    //    Yes = 1,
    //    Default
    //}
    ///// <summary>
    ///// 压缩方式  LZ4 NONE ZLIB
    ///// </summary>
    //public enum TableCompression
    //{
    //    LZ4,
    //    NONE,
    //    ZLIB
    //}

    #endregion

    #region TableIndex Enums

    /// <summary>
    /// 索引类型
    /// </summary>
    public enum IndexType
    {
        Primary,
        Normal,
        Unique,
        Fulltext,
        Spatial
    }

    /// <summary>
    /// 索引函数
    /// </summary>
    public enum IndexFunction
    {
        BTREE,
        HASH
    }
    #endregion



    /// <summary>
    /// 虚拟类型
    /// </summary>
    public enum VirtualType
    {
        NULL,
        STORED,
        VIRTUAL
    }


    /// <summary>
    /// 数据库设置会话类型
    /// </summary>
    public enum DbSettingSessionType
    {
        /// <summary>
        /// 当前会话
        /// </summary>
        SESSION,
        /// <summary>
        /// 当前或之后的所有会话
        /// </summary>
        GLOBAL
    }

    public enum FunctionEnum
    {
        /// <summary>
        /// 存储过程
        /// </summary>
        PROCEDURE,
        /// <summary>
        /// 函数
        /// </summary>
        FUNCTION
    }

    #region 触发器

    /// <summary>
    /// 触发器类型
    /// </summary>
    public enum TriggerEvent
    {
        /// <summary>
        /// 删除时
        /// </summary>
        Delete,
        /// <summary>
        /// 新增时
        /// </summary>
        Insert,
        /// <summary>
        /// 更新时
        /// </summary>
        Update
    }
    /// <summary>
    /// 触发时间
    /// </summary>
    public enum TeiggerTime
    {
        /// <summary>
        /// 之后
        /// </summary>
        After,
        /// <summary>
        /// 之前
        /// </summary>
        Before
    }
    #endregion



    public enum OutputType
    {
        None = 0,
        Comment = 1,
        Warning = 2,
        Error = 3,
        Sql = 4,
        Loading = 5,
    }
    /// <summary>
    /// 数据库对象类型
    /// </summary>
    public enum DBObjType
    {
        Table = 1,
        View = 2,
        Trig = 3,
        Proc = 4,
        Func = 5,
        DBSetting = 6
    }
    public enum SortedOptionType
    {
        NONE = 0,
        /// <summary>
        /// 第一
        /// </summary>
        FIRST = 1,
        /// <summary>
        /// 在之后
        /// </summary>
        AFTER = 2

    }
    /// <summary>
    /// 参与比较的数据源类型
    /// </summary>
    public enum DBDataSourceType
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 空
        /// </summary>
        Empty = 1,
        /// <summary>
        /// mysql数据库
        /// </summary>
        MySql = 2,
        /// <summary>
        /// 文件
        /// </summary>
        DataSourceFile = 3
    }

    public enum SqlType
    {
        Common = 1,
        Delete = 2,
        Alter = 4,
        Create = 8,
        ChangeSetting = 16
    }

    class Structs
    {
    }

    /// <summary>
    /// desc table 常量
    /// </summary>
    public struct TableDescInfoConst
    {
        public const string VIRTUAL = "VIRTUAL GENERATED";
        public const string AUTOINCREMENT = "auto_increment";
        public const string YES = "YES";
        public const string NO = "NO";
    }
    /// <summary>
    /// 事务隔离级别
    /// </summary>
    public struct DBTrancstionConst
    {
        /// <summary>
        /// Read UnCommitted 读未提交
        /// </summary>
        public const string Read_Uncommitted = "READ-UNCOMMITTED";
        /// <summary>
        /// Read Committed 读已提交
        /// </summary>
        public const string Read_Committed = "READ-COMMITTED";
        /// <summary>
        /// 可重复读（Repeatable-Read)
        /// </summary>
        public const string Repeatable_Read = "REPEATABLE-READ";
        /// <summary>
        /// 序列化（ serializable）
        /// </summary>
        public const string Serializable = "SERIALIZABLE";

        public static string GetDBChangeTrancstionConst(string DBTrancstionConstStr)
        {
            string rel = "";
            switch (DBTrancstionConstStr)
            {
                case Read_Uncommitted:
                    rel = DBChangeTrancstionConst.Read_Uncommitted;
                    break;
                case Read_Committed:
                    rel = DBChangeTrancstionConst.Read_Committed;
                    break;
                case Repeatable_Read:
                    rel = DBChangeTrancstionConst.Repeatable_Read;
                    break;
                case Serializable:
                    rel = DBChangeTrancstionConst.Serializable;
                    break;
                default:
                    break;
            }
            return rel;
        }
    }
    /// <summary>
    /// 事务隔离级别
    /// </summary>
    public struct DBChangeTrancstionConst
    {
        /// <summary>
        /// Read UnCommitted 读未提交
        /// </summary>
        public const string Read_Uncommitted = "read uncommitted";
        /// <summary>
        /// Read Committed 读已提交
        /// </summary>
        public const string Read_Committed = "read committed";
        /// <summary>
        /// 可重复读（Repeatable-Read)
        /// </summary>
        public const string Repeatable_Read = "repeatable read";
        /// <summary>
        /// 序列化（ serializable）
        /// </summary>
        public const string Serializable = "serializable";
    }
}
