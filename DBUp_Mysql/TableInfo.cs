using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public abstract class DBInfo
    {
        public string Name { get; set; }
    }
    /// <summary>
    /// 一张数据库表格的信息
    /// </summary>
    public class TableInfo : DBInfo
    {
        // Schema名
        public string SchemaName { get; set; }
        // 注释
        public string Comment { get; set; }
        // 校对集
        public string Collation { get; set; }
        // 所有列信息（key：列名， value：列信息）
        public Dictionary<string, ColumnInfo> AllColumnInfo { get; set; }
        // 主键列的列名
        public List<string> PrimaryKeyColumnNames { get; set; }
        // 索引设置（key：索引名， value：按顺序排列的列名）
        public Dictionary<string, TableIndex> IndexInfo { get; set; }
        /// <summary>
        /// 表选项
        /// </summary>
        public TableOption Option { get; set; }
        public TableInfo()
        {
            AllColumnInfo = new Dictionary<string, ColumnInfo>();
            PrimaryKeyColumnNames = new List<string>();
            IndexInfo = new Dictionary<string, TableIndex>();
        }
    }

    /// <summary>
    /// 选项（表）
    /// </summary>
    public class TableOption
    {
        /// <summary>
        /// 默认字符集
        /// </summary>
        public string Collate { get; set; }
        /// <summary>
        /// 引擎
        /// </summary>
        public TableEngine Engine { get; set; }
        /// <summary>
        /// 记录格式
        /// </summary>
        public TableRowFormat RowFormat { get; set; }
        /// <summary>
        /// 检查记录和
        /// </summary>
        public TableChecksum Checksum { get; set; } = TableChecksum.UnCheck;
        /// <summary>
        /// 自动增加
        /// </summary>
        public int? Auto_Increment { get; set; }
        /// <summary>
        /// 平均记录长度
        /// </summary>
        public int? Avg_Row_Length { get; set; }
        /// <summary>
        /// 最大记录行数
        /// </summary>
        public int? Max_Rows { get; set; }
        /// <summary>
        /// 最小记录行数
        /// </summary>
        public int? Min_Rows { get; set; }
        /// <summary>
        /// 统计数据持久 null 0 1 default
        /// </summary>
        public string STATS_PERSISTENT { get; set; }
        /// <summary>
        /// 统计数据自动重计 null 0 1 default
        /// </summary>
        public string STATS_AUTO_RECALC { get; set; }
        /// <summary>
        /// 压缩方式  LZ4 NONE ZLIB
        /// </summary>
        public string COMPRESSION { get; set; }
        /// <summary>
        /// 加密
        /// </summary>
        public string ENCRYPTION { get; set; }
        /// <summary>
        /// 表空间
        /// </summary>
        public string TABLESPACE { get; set; }

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

    public class TableIndex
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 列名称
        /// </summary>
        public List<string> Columns { get; set; }
        /// <summary>
        /// 索引类型
        /// </summary>
        public IndexType IndexType { get; set; } = IndexType.Normal;
        /// <summary>
        /// 索引函数
        /// </summary>
        public IndexFunction? IndexFunc { get; set; }
        /// <summary>
        /// 注释
        /// </summary>
        public string Common { get; set; }

    }

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
    /// 数据库表格中一列的信息
    /// </summary>
    public class ColumnInfo
    {
        // 表名
        public string TableName { get; set; }
        // 列名
        public string ColumnName { get; set; }
        // 数据类型（包含长度）
        public string DataType { get; set; }
        // 注释
        public string Comment { get; set; }
        // 是否是主键
        public bool IsPrimaryKey { get; set; }
        // 是否唯一
        public bool IsUnique { get; set; }
        // 是否为索引列但允许重复值
        public bool IsMultiple { get; set; }
        // 是否非空
        public bool IsNotEmpty { get; set; }
        // 是否自增
        public bool IsAutoIncrement { get; set; }
        // 默认值
        public string DefaultValue { get; set; }
    }

    /// <summary>
    /// 视图信息
    /// </summary>
    public class ViewInfo : DBInfo
    {
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
    /// <summary>
    /// 函数 存储过程 信息
    /// </summary>
    public class Function : DBInfo
    {
        /// <summary>
        /// 类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FunctionEnum Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Definer { get; set; }
        /// <summary>
        /// 创建时间 Proc无此项
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// 修改时间 Proc无此项
        /// </summary>
        public DateTime Modified { get; set; }
        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

        public string ClientCharSet { get; set; }
        public string CharSet { get; set; }
        public FunctionInfo Info { get; set; }

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

    public class FunctionInfo
    {
        public string Name { get; set; }
        public string SQLModel { get; set; }
        public string CreateSQL { get; set; }

        public string ClientCharSet { get; set; }
        public string CharSet { get; set; }
    }
    /// <summary>
    /// 触发器
    /// </summary>
    public class Trigger : DBInfo
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public TriggerEvent Event { get; set; }
        public string TableName { get; set; }
        public string Statement { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TeiggerTime Time { get; set; }
        public DateTime Created { get; set; }
        public string SQLMode { get; set; }
        public string Definer { get; set; }
        public string ClientCharSet { get; set; }
        public string CharSet { get; set; }
    }

    public enum TriggerEvent
    {
        Delete,
        Insert,
        Update
    }
    public enum TeiggerTime
    {
        After,
        Before
    }





    public enum OutputType
    {
        None,
        Comment,
        Warning,
        Error,
        Sql,
    }
    /// <summary>
    /// 数据库对象类型
    /// </summary>
    public enum DBObjType
    {
        Table,
        View,
        Trig,
        Proc,
        Func
    }


    /// <summary>
    /// 数据库全局信息
    /// </summary>
    public class DbModel
    {
        /// <summary>
        /// SqlMode
        /// </summary>
        public string SqlMode { get; set; }
    }
}
