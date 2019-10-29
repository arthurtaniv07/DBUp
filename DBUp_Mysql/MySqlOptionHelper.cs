using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public delegate void DBHander(string currDBName, int tabCount, int i);
    public class MySqlOptionHelper : IDisposable
    {


        public static DBHander GetDBTableInfohander;

        private int TabsCount = 0;
        private int TabsInx = 0;
        public void Set_DbHander(DBHander hander)
        {
            GetDBTableInfohander = hander;
        }


        public string DbName
        {
            get
            {
                return Conn.Database;
            }
        }
        public string Server
        {
            get
            {
                return Conn.DataSource;
            }
        }

        #region 链接对象处理

        private MySqlConnection Conn;

        private string _SchemaTabName(string tabName)
        {
            return string.Format("`{0}`", tabName);
        }
        private string _SchemaName
        {
            get
            {
                return Conn.Database;
            }
        }
        public MySqlOptionHelper(string connStr)
        {
            Conn = new MySqlConnection(connStr);
        }
        public bool Open()
        {
            if (Conn.State != ConnectionState.Open)
                Conn.Open();
            return Conn.State == ConnectionState.Open;
        }
        public bool Close()
        {
            if (Conn.State == ConnectionState.Open)
                Conn.Close();
            return Conn.State == ConnectionState.Closed;
        }
        public void Dispose()
        {
            if (Conn != null && Conn.State == ConnectionState.Open)
                Conn.Close();
        }
        #endregion

        /// <summary>
        /// 执行指定SQL语句，返回执行结果
        /// </summary>
        private static DataTable _ExecuteSqlCommand(MySqlCommand cmd)
        {
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        /// <summary>
        /// 获取所有表
        /// </summary>
        /// <param name="existTableNames"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        public bool GetTables(out List<string> existTableNames, out string errorString)
        {
            TabsCount = 0;
            TabsInx = 0;
            existTableNames = new List<string>();
            try
            {
                if (Conn.State == ConnectionState.Open)
                {
                    // 获取已存在的数据表名
                    DataTable schemaInfo = Conn.GetSchema(System.Data.SqlClient.SqlClientMetaDataCollectionNames.Tables);
                    foreach (DataRow info in schemaInfo.Rows)
                        existTableNames.Add(info.ItemArray[2].ToString());

                    TabsCount = existTableNames.Count;
                    errorString = null;
                    return true;
                }
                else
                {
                    errorString = "未知错误";
                    return true;
                }
            }
            catch (Exception exception)
            {
                TabsCount = 0;
                errorString = exception.Message;
                return false;
            }
        }


        /// <summary>
        /// 将某张表格的属性作为TableInfo类返回
        /// </summary>
        public TableInfo GetTableInfo(string tableName)
        {
            TabsInx++;
            TableInfo tableInfo = new TableInfo();
            // Schema名
            tableInfo.SchemaName = _SchemaName;
            // 表名
            tableInfo.TableName = tableName;
            // 表注释（注意转义注释中的换行）
            tableInfo.Comment = _GetTableProperty(tableName, "TABLE_COMMENT").Replace(System.Environment.NewLine, "\\n").Replace("\n", "\\n");
            // 表校对集
            tableInfo.Collation = _GetTableProperty(tableName, "TABLE_COLLATION");
            // 索引设置
            tableInfo.IndexInfo = _GetIndexInfo(tableName);
            // 列信息
            DataTable dtColumnInfo = _GetAllColumnInfo(tableName);
            if (dtColumnInfo != null)
            {
                int columnCount = dtColumnInfo.Rows.Count;
                for (int i = 0; i < columnCount; ++i)
                {
                    ColumnInfo columnInfo = new ColumnInfo();
                    // 表名
                    columnInfo.TableName = tableName;
                    // 列名
                    columnInfo.ColumnName = dtColumnInfo.Rows[i]["COLUMN_NAME"].ToString();
                    // 注释（注意转义注释中的换行）
                    columnInfo.Comment = dtColumnInfo.Rows[i]["COLUMN_COMMENT"].ToString().Replace(System.Environment.NewLine, "\\n").Replace("\n", "\\n");
                    // 数据类型（包含长度）
                    columnInfo.DataType = dtColumnInfo.Rows[i]["COLUMN_TYPE"].ToString();
                    // 属性
                    string columnKey = dtColumnInfo.Rows[i]["COLUMN_KEY"].ToString();
                    if (!string.IsNullOrEmpty(columnKey))
                    {
                        if (columnKey.IndexOf("PRI", StringComparison.CurrentCultureIgnoreCase) != -1)
                        {
                            columnInfo.IsPrimaryKey = true;
                            tableInfo.PrimaryKeyColumnNames.Add(columnInfo.ColumnName);
                        }
                        if (columnKey.IndexOf("UNI", StringComparison.CurrentCultureIgnoreCase) != -1)
                            columnInfo.IsUnique = true;
                        if (columnKey.IndexOf("MUL", StringComparison.CurrentCultureIgnoreCase) != -1)
                            columnInfo.IsMultiple = true;
                    }
                    // 额外属性
                    string extra = dtColumnInfo.Rows[i]["EXTRA"].ToString();
                    if (!string.IsNullOrEmpty(extra))
                    {
                        if (columnKey.IndexOf("auto_increment", StringComparison.CurrentCultureIgnoreCase) != -1)
                            columnInfo.IsAutoIncrement = true;
                    }
                    // 是否非空
                    columnInfo.IsNotEmpty = dtColumnInfo.Rows[i]["IS_NULLABLE"].ToString().Equals("NO", StringComparison.CurrentCultureIgnoreCase);
                    // 默认值
                    object defaultValue = dtColumnInfo.Rows[i]["COLUMN_DEFAULT"];
                    string defaultValueString = _GetDatabaseValueString(defaultValue);
                    columnInfo.DefaultValue = defaultValueString;

                    tableInfo.AllColumnInfo.Add(columnInfo.ColumnName, columnInfo);
                }
            }

            GetDBTableInfohander(tableName, TabsCount, TabsInx);
            return tableInfo;
        }

        private const string _SELECT_COLUMN_INFO_SQL = "SELECT * FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' ORDER BY ORDINAL_POSITION ASC;";
        /// <summary>
        /// 返回某张表格所有列属性
        /// </summary>
        private DataTable _GetAllColumnInfo(string tableName)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_COLUMN_INFO_SQL, _SchemaName, tableName), Conn);
            return _ExecuteSqlCommand(cmd);
        }

        private const string _SHOW_INDEX_SQL = "SHOW INDEX FROM {0} WHERE Key_name != 'PRIMARY' AND Index_type = 'BTREE';";
        /// <summary>
        /// 获取某表格的索引设置
        /// </summary>
        private Dictionary<string, List<string>> _GetIndexInfo(string tableName)
        {
            Dictionary<string, List<string>> indexInfo = new Dictionary<string, List<string>>();

            // MySQL的SHOW INDEX语句中无法使用ORDER BY，而List中没有前面的元素就无法在后面指定下标处插入数据，故用下面的数据结构进行整理，其中内层Dictionary的key为序号，value为列名
            Dictionary<string, Dictionary<int, string>> tempIndexInfo = new Dictionary<string, Dictionary<int, string>>();

            MySqlCommand cmd = new MySqlCommand(string.Format(_SHOW_INDEX_SQL, _SchemaTabName(tableName)), Conn);
            DataTable dt = _ExecuteSqlCommand(cmd);

            int count = dt.Rows.Count;
            for (int i = 0; i < count; ++i)
            {
                string name = dt.Rows[i]["Key_name"].ToString();
                string columnName = dt.Rows[i]["Column_name"].ToString();
                int seq = int.Parse(dt.Rows[i]["Seq_in_index"].ToString());
                if (!tempIndexInfo.ContainsKey(name))
                    tempIndexInfo.Add(name, new Dictionary<int, string>());

                Dictionary<int, string> tempColumnNames = tempIndexInfo[name];
                tempColumnNames.Add(seq, columnName);
            }

            // 转为Dictionary<string, List<string>>数据结构
            foreach (var pair in tempIndexInfo)
            {
                string name = pair.Key;
                indexInfo.Add(name, new List<string>());
                List<string> columnNames = indexInfo[name];
                int columnCount = pair.Value.Count;
                for (int seq = 1; seq <= columnCount; ++seq)
                    columnNames.Add(pair.Value[seq]);
            }

            return indexInfo;
        }
        /// <summary>
        /// 获取数据库中一个数据在SQL语句中的表示形式
        /// </summary>
        private static string _GetDatabaseValueString(object value)
        {
            if (value.GetType() == typeof(System.DBNull))
                return "NULL";
            else if (value.GetType() == typeof(System.Boolean))
            {
                if ((bool)value == true)
                    return "\"1\"";
                else
                    return "\"0\"";
            }
            // MySQL中string类型的空字符串，若用单引号包裹则认为是NULL，用双引号包裹才认为是空字符串。还要注意转义数据中的引号
            else
                return string.Concat("\"", value.ToString().Replace("\"", "\\\""), "\"");
        }
        private const string _SELECT_TABLE_INFO_SQL = "SELECT {0} FROM information_schema.TABLES WHERE TABLE_SCHEMA = '{1}' AND TABLE_NAME = '{2}';";
        /// <summary>
        /// 获取某张表的某个属性
        /// </summary>
        private string _GetTableProperty(string tableName, string propertyName)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_TABLE_INFO_SQL, propertyName, _SchemaName, tableName), Conn);
            DataTable dt = _ExecuteSqlCommand(cmd);
            return dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : string.Empty;
        }

        private const string _SHOW_CREATE_TABLE_SQL = "SHOW CREATE TABLE {0}";
        /// <summary>
        /// 获取某张表的建表SQL
        /// </summary>
        private string _GetCreateTableSql(string tableName, string targetSchemaName)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SHOW_CREATE_TABLE_SQL, _SchemaTabName(tableName)), Conn);
            DataTable dt = _ExecuteSqlCommand(cmd);
            string createTableSql = dt.Rows[0]["Create Table"].ToString();
            // MySQL提供功能返回的建表SQL不含Schema，这里自己加上
            int firstBracketIndex = createTableSql.IndexOf("(");
            return string.Format("CREATE TABLE {0} {1};", _SchemaTabName(tableName), createTableSql.Substring(firstBracketIndex));
        }

    }


}
