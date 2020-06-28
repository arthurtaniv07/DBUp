using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    /// <summary>
    /// 一张数据库表格的信息
    /// </summary>
    public class TableInfo : DBInfo
    {
        public string Name { get; set; }
        // Schema名
        public string SchemaName { get; set; }
        // 注释
        public string Comment { get; set; }
        // 校对集
        public string Collation { get; set; }

        /// <summary>
        /// 创建表的SQL
        /// </summary>
        public string CreateSql { get; set; }

        // 所有列信息（key：列名， value：列信息）
        public SortedDictionary<string, ColumnInfo> AllColumnInfo { get; set; }
        /// <summary>
        ///  表名
        /// </summary>
        public List<string> TableNames { get; set; }
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
            AllColumnInfo = new SortedDictionary<string, ColumnInfo>();
            PrimaryKeyColumnNames = new List<string>();
            IndexInfo = new Dictionary<string, TableIndex>();
            TableNames = new List<string>();
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
   

    /// <summary>
    /// 表索引
    /// </summary>
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
        /// <summary>
        /// 是否是虚拟列 5.7及以上版本支持
        /// </summary>
        public VirtualInfo Virtual { get; set; }
    }
    /// <summary>
    /// 虚拟列信息
    /// </summary>
    public class VirtualInfo
    {
        public VirtualType VirtualType { get; set; }
        /// <summary>
        /// 表达式
        /// </summary>
        public string Val { get; set; }
        /// <summary>
        /// 永远生成
        /// </summary>
        public bool IsForever { get; set; }
        /// <summary>
        /// 二进制
        /// </summary>
        public bool IsBinary { get; set; }
    }


    /// <summary>
    /// desc [tableName] 信息
    /// </summary>
    public class TableDescInfo
    {
        public string Field { get; set; }
        public string Type { get; set; }
        public string Null { get; set; }
        public string Key { get; set; }
        public string Default { get; set; }
        public string Extra { get; set; }
    }
    




}
