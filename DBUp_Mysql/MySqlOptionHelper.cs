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
        private string _port = null;
             
        public string Port
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_port))
                    return _port;
                try
                {
                    if(Conn.State == ConnectionState.Closed)
                        Conn.Open();
                    if (Conn.State == ConnectionState.Open)
                    {
                        // 查询端口
                        MySqlCommand cmd = new MySqlCommand("show global variables like 'port';", Conn);
                        DataTable dtColumnInfo = _ExecuteSqlCommand(cmd);
                        if (dtColumnInfo != null && dtColumnInfo.Rows.Count > 0)
                            _port = dtColumnInfo.Rows[0][1] + "";
                        else
                            _port = "0";
                    }
                }
                catch (Exception)
                {
                    _port = "-1";
                }
                finally
                {
                    if (Conn.State == ConnectionState.Open)
                        Conn.Close();
                }
                return _port;

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
        public MySqlOptionHelper()
        {
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

        private const string _SELECT_DBMODE_SQL = "select @@global.sql_mode;";
        public DbModel GetDbInfo()
        {
            DbModel rel = new DbModel();

            MySqlCommand cmd = new MySqlCommand(_SELECT_DBMODE_SQL, Conn);
            DataTable dt = _ExecuteSqlCommand(cmd);
            rel.SqlMode = dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : string.Empty;
            return rel;
        }

        #region 表
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
                //在调用之前已经打开连接，不知道为什么这里的数据库连接状态是关闭的要再次打开
                if (Conn.State == ConnectionState.Closed)
                    Conn.Open();
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
            tableInfo.Name = tableName;
            // 表注释（注意转义注释中的换行）
            tableInfo.Comment = _GetTableProperty(tableName, "TABLE_COMMENT").Replace(System.Environment.NewLine, "\\n").Replace("\n", "\\n");
            // 表校对集
            tableInfo.Collation = _GetTableProperty(tableName, "TABLE_COLLATION");
            // 索引设置
            tableInfo.IndexInfo = _GetIndexInfo(tableName);

            //选项
            tableInfo.Option = new TableOption();
            DataTable dt = _GetTableProperty(tableName);
            if (dt?.Rows.Count > 0)
            {
                DataRow firRow = dt.Rows[0];
                int temp_int;
                string temp;

                temp = firRow["AUTO_INCREMENT"] + "";
                if (int.TryParse(temp, out temp_int))
                    tableInfo.Option.Auto_Increment = temp_int;

                temp = firRow["AVG_ROW_LENGTH"] + "";
                if (int.TryParse(temp, out temp_int))
                    tableInfo.Option.Avg_Row_Length = temp_int;

                temp = firRow["CHECKSUM"] + "";
                if (int.TryParse(temp, out temp_int))
                    tableInfo.Option.Checksum = (TableChecksum)Enum.ToObject(typeof(TableChecksum), temp_int);

                temp = firRow["TABLE_COLLATION"] + "";
                tableInfo.Option.Collate = temp;

                temp = firRow["ENGINE"] + "";
                if (!string.IsNullOrWhiteSpace(temp))
                    tableInfo.Option.Engine = (TableEngine)Enum.Parse(typeof(TableEngine), temp, true);

                temp = firRow["Create_Options"] + "";

                if (!string.IsNullOrWhiteSpace(temp))
                {
                    //min_rows=3 max_rows=2 avg_row_length=1 stats_persistent=0 stats_auto_recalc=1 COMPRESSION="ZLIB" ENCRYPTION = 'Y'
                    /* min_rows 最小行
                     * max_rows 最大行
                     * stats_persistent 统计数据持久 null 0 1 default
                     * stats_auto_recalc 统计数据自动重计 null 0 1 default
                     * COMPRESSION 压缩方式  LZ4 NONE ZLIB
                     * ENCRYPTION 加密
                     * 
                     * ALTER TABLE `test_up`.`test` COMPRESSION = 'LZ4', ENCRYPTION = 'Y', STATS_PERSISTENT = DEFAULT;
                     */
                    foreach (var item in temp.Split(' '))
                    {
                        if (!item.Contains("="))
                            continue;
                        string[] itemArr = item.Split('=').Select(i => i.Trim()).ToArray();
                        temp = itemArr[1];
                        switch (itemArr[0].ToLower())
                        {
                            case "min_rows":
                                if (int.TryParse(temp, out temp_int))
                                    tableInfo.Option.Min_Rows = temp_int;
                                break;
                            case "max_rows":
                                if (int.TryParse(temp, out temp_int))
                                    tableInfo.Option.Max_Rows = temp_int;
                                break;
                            case "stats_persistent":
                                tableInfo.Option.STATS_PERSISTENT = temp;
                                break;
                            case "stats_auto_recalc":
                                tableInfo.Option.STATS_AUTO_RECALC = temp;
                                break;
                            case "compression":
                                if(!string.IsNullOrWhiteSpace(temp))
                                    tableInfo.Option.COMPRESSION = temp;
                                break;
                            case "encryption":
                                if(!string.IsNullOrWhiteSpace(temp))
                                    tableInfo.Option.ENCRYPTION = temp;
                                break;
                            case "tablespace":
                                if(!string.IsNullOrWhiteSpace(temp))
                                    tableInfo.Option.TABLESPACE = temp;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

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
        /// <summary>
        /// 获取某张表的某个属性
        /// </summary>
        private DataTable _GetTableProperty(string tableName)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_TABLE_INFO_SQL, "*", _SchemaName, tableName), Conn);
            DataTable dt = _ExecuteSqlCommand(cmd);
            return dt;
        }
        private const string _SHOW_INDEX_SQL = "SHOW INDEX FROM {0} WHERE Key_name != 'PRIMARY';";
        /// <summary>
        /// 获取某表格的索引设置
        /// </summary>
        private Dictionary<string, TableIndex> _GetIndexInfo(string tableName)
        {
            Dictionary<string, TableIndex> indexInfo = new Dictionary<string, TableIndex>();

            // MySQL的SHOW INDEX语句中无法使用ORDER BY，而List中没有前面的元素就无法在后面指定下标处插入数据，故用下面的数据结构进行整理，其中内层Dictionary的key为序号，value为列名
            Dictionary<string, Dictionary<int, string>> tempIndexInfo = new Dictionary<string, Dictionary<int, string>>();

            MySqlCommand cmd = new MySqlCommand(string.Format(_SHOW_INDEX_SQL, _SchemaTabName(tableName)), Conn);
            DataTable dt = _ExecuteSqlCommand(cmd);
            TableIndex temp;
            int count = dt.Rows.Count;
            for (int i = 0; i < count; ++i)
            {
                string name = dt.Rows[i]["Key_name"].ToString();
                string columnName = dt.Rows[i]["Column_name"].ToString();
                int Non_unique = int.Parse(dt.Rows[i]["Non_unique"].ToString());
                int seq = int.Parse(dt.Rows[i]["Seq_in_index"].ToString());
                string Collation = dt.Rows[i]["Collation"].ToString();
                string Index_type = dt.Rows[i]["Index_type"].ToString();
                if (!indexInfo.ContainsKey(name))
                {
                    indexInfo.Add(name, new TableIndex() { Columns = new List<string>(), Name = name });
                }
                temp = indexInfo[name];
                temp.Columns.Add(columnName);
                //temp.IndexType = (IndexType)Enum.Parse(typeof(IndexType), Index_type, true);
                if (Index_type == "FULLTEXT")
                {
                    temp.IndexType = IndexType.Fulltext;
                    temp.IndexFunc = null;
                }
                else
                {
                    temp.IndexFunc = (IndexFunction)Enum.Parse(typeof(IndexFunction), Index_type, true);
                }
                if (Non_unique == 0)
                {
                    temp.IndexType = IndexType.Unique;
                }
            }

            

            return indexInfo;
        }

        private const string _SELECT_COLUMN_INFO_SQL = "SELECT * FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' ORDER BY ORDINAL_POSITION ASC;";
        /// <summary>
        /// 获取所有列信息
        /// </summary>
        private DataTable _GetAllColumnInfo(string tableName)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_COLUMN_INFO_SQL, _SchemaName, tableName), Conn);
            return _ExecuteSqlCommand(cmd);
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
                //这里为什么bool类型的默认值不进判断 待查
                if ((bool)value == true)
                    return "\"1\"";
                else
                    return "\"0\"";
            }
            // MySQL中string类型的空字符串，若用单引号包裹则认为是NULL，用双引号包裹才认为是空字符串。还要注意转义数据中的引号
            else
            {
                if ((value + "").ToLower().StartsWith("b'"))
                {
                    return value + "";
                }
                if ((value + "").ToUpper().Equals("CURRENT_TIMESTAMP"))
                {
                    return value + "";
                }
                return string.Concat("\"", value.ToString().Replace("\"", "\\\""), "\"");
            }
        }


        private const string _SHOW_CREATE_TABLE_SQL = "SHOW CREATE TABLE {0}";
        /// <summary>
        /// 获取某张表的建表SQL
        /// </summary>
        public string GetCreateTableSql(string tableName)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SHOW_CREATE_TABLE_SQL, _SchemaTabName(tableName)), Conn);
            DataTable dt = _ExecuteSqlCommand(cmd);
            string createTableSql = dt.Rows[0]["Create Table"].ToString();
            // MySQL提供功能返回的建表SQL不含Schema，这里自己加上
            int firstBracketIndex = createTableSql.IndexOf("(");
            return string.Format(_CREATE_TABLE_SQL, _SchemaTabName(tableName), createTableSql.Substring(firstBracketIndex));
        }

        private const string _CREATE_TABLE_SQL = "CREATE TABLE {0} {1};\n";


        private const string _DROP_TABLE_SQL = "DROP TABLE {0};\n";
        /// <summary>
        /// 获取删除某张表的SQL
        /// </summary>
        public string GetDropTableSql(string tableName)
        {
            return string.Format(_DROP_TABLE_SQL, _SchemaTabName(tableName));
        }
        private const string _ALTER_TABLE_SQL = "ALTER TABLE {0} ";
        private const string _DROP_COLUMN_SQL = "DROP COLUMN `{0}`;\n";
        /// <summary>
        /// 获取删除列的SQL
        /// </summary>
        public string GetDropTableColumnSql(string tableName,string columnName)
        {
            return string.Concat(string.Format(_ALTER_TABLE_SQL, _SchemaTabName(tableName)), string.Format(_DROP_COLUMN_SQL, columnName));
        }
        private const string _ADD_COLUMN_SQL = "ADD COLUMN `{0}` {1} {2}{3} COMMENT '{4}';\n";
        /// <summary>
        /// 获取添加列的SQL
        /// </summary>
        public string GetAddTableColumnSql(string tableName, ColumnInfo columnInfo)
        {
            string notEmptyString = columnInfo.IsNotEmpty == true ? "NOT NULL" : "NULL";
            // 注意如果列设为NOT NULL，就不允许设置默认值为NULL
            string defaultValue = columnInfo.DefaultValue.Equals("NULL") ? string.Empty : string.Concat(" DEFAULT ", columnInfo.DefaultValue);
            return string.Concat(string.Format(_ALTER_TABLE_SQL, _SchemaTabName(tableName)), string.Format(_ADD_COLUMN_SQL, columnInfo.ColumnName, columnInfo.DataType, notEmptyString, defaultValue, columnInfo.Comment));
        }
        private const string _CHANGE_COLUMN_SQL = "CHANGE COLUMN `{0}` `{0}` {1} {2}{3} COMMENT '{4}';\n";
        /// <summary>
        /// 获取修改列的SQL
        /// </summary>
        public string GetChangeTableColumnSql(string tableName, ColumnInfo columnInfo)
        {
            string notEmptyString = columnInfo.IsNotEmpty == true ? "NOT NULL" : "NULL";
            // 注意如果列设为NOT NULL，就不允许设置默认值为NULL
            string defaultValue = columnInfo.DefaultValue.Equals("NULL") ? string.Empty : string.Concat(" DEFAULT ", columnInfo.DefaultValue);
            return string.Concat(string.Format(_ALTER_TABLE_SQL, _SchemaTabName(tableName)), string.Format(_CHANGE_COLUMN_SQL, columnInfo.ColumnName, columnInfo.DataType, notEmptyString, defaultValue, columnInfo.Comment));
        }

        private const string _DROP_PRIMARY_KEY_SQL = "DROP PRIMARY KEY ;\n";
        /// <summary>
        /// 获取删除主键的SQL
        /// </summary>
        public string GetDropPrimarySql(string tableName)
        {
            return string.Concat(string.Format(_ALTER_TABLE_SQL, _SchemaTabName(tableName)), _DROP_PRIMARY_KEY_SQL);
        }

        private const string _ADD_PRIMARY_KEY_SQL = "ADD PRIMARY KEY ({0});\n";
        /// <summary>
        /// 获取添加主键的SQL
        /// </summary>
        public string GetAddPrimarySql(string tableName, List<string> primaryKeyDefine)
        {
            return string.Concat(string.Format(_ALTER_TABLE_SQL, _SchemaTabName(tableName)), string.Format(_ADD_PRIMARY_KEY_SQL, "`" + string.Join("`,`", primaryKeyDefine) + "`"));
        }

        private const string _DROP_INDEX_SQL = "DROP INDEX {0};\n";
        /// <summary>
        /// 获取删除索引的SQL
        /// </summary>
        public string GetDropIndexSql(string tableName,params string[] indexDefine)
        {
            return string.Concat(string.Format(_ALTER_TABLE_SQL, _SchemaTabName(tableName)), string.Format(_DROP_INDEX_SQL, "`" + string.Join("`,`", indexDefine) + "`"));
        }
        private const string _ADD_INDEX_SQL = "ADD{0} INDEX `{1}`({2}) {3} COMMENT '{4}';\n";
        /// <summary>
        /// 获取添加索引的SQL
        /// </summary>
        public string GetAddIndexSql(string tableName, TableIndex tableIndex)
        {
            List<string> columnDefine = new List<string>();
            // 根据新增索引属性生成添加新索引的SQL
            // 注意列名后必须声明排序方式，MySQL只支持索引的升序排列
            foreach (string columnName in tableIndex.Columns)
                columnDefine.Add(string.Format("`{0}` ASC", columnName));
            string indexType = "";
            if (tableIndex.IndexType != IndexType.Normal)
                indexType = " " + tableIndex.IndexType.ToString().ToUpper();
            string indexFun = "";
            if (tableIndex.IndexFunc.HasValue)
                indexFun = "USING " + tableIndex.IndexFunc.ToString().ToUpper();
            return string.Concat(string.Format(_ALTER_TABLE_SQL, _SchemaTabName(tableName)),
                string.Format(_ADD_INDEX_SQL, indexType, tableIndex.Name, string.Join(",", columnDefine), indexFun, tableIndex.Common));
        }

        private const string _ALTER_TABLE_COLLATION_SQL = "COLLATE = {0};\n";
        /// <summary>
        /// 获取修改表校对集的SQL
        /// </summary>
        public string GetChangeCollateSql(string tableName, string collation)
        {
            return string.Concat(string.Format(_ALTER_TABLE_SQL, _SchemaTabName(tableName)), string.Format(_ALTER_TABLE_COLLATION_SQL, collation));
        }
        private const string _ALTER_TABLE_COMMENT_SQL = "COMMENT = '{0}';\n";
        /// <summary>
        /// 获取修改表注释
        /// </summary>
        public string GetChangeCommentSql(string tableName, string comment)
        {
            return string.Concat(string.Format(_ALTER_TABLE_SQL, _SchemaTabName(tableName)), string.Format(_ALTER_TABLE_COMMENT_SQL, comment));
        }
        /// <summary>
        /// 获取修改表选项
        /// </summary>
        public string GetChangeOptionSql(string tableName, string fieldName, dynamic value)
        {
            return "";
            string str;
            string splitStr = "";
            switch (fieldName.ToLower())
            {
                case "tablespace":
                    splitStr = "`";
                    break;
                case "encryption":
                case "compression":
                    splitStr = "'";
                    break;
                default:
                    break;
            }

            str = fieldName.ToUpper() + "=" + splitStr + value + splitStr;
            return string.Concat(string.Format(_ALTER_TABLE_SQL, _SchemaTabName(tableName)), str);
        }
        #endregion
        private const string _DROP_DATA_SQL = "DELETE FROM {0} WHERE {1};\n";
        private const string _UPDATE_DATA_SQL = "UPDATE {0} SET {1} WHERE {2};\n";

        #region 视图


        /// <summary>
        /// 获取所有视图
        /// </summary>
        /// <param name="existViewNames"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        public bool GetViews(out List<string> existViewNames, out string errorString)
        {
            TabsCount = 0;
            TabsInx = 0;
            existViewNames = new List<string>();
            try
            {
                //在调用之前已经打开连接，不知道为什么这里的数据库连接状态是关闭的要再次打开
                if (Conn.State == ConnectionState.Closed)
                    Conn.Open();
                if (Conn.State == ConnectionState.Open)
                {
                    // 获取已存在的数据表名
                    DataTable schemaInfo = _GetViews();
                    foreach (DataRow info in schemaInfo.Rows)
                        existViewNames.Add(info.ItemArray[2].ToString());

                    TabsCount = existViewNames.Count;
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
        public ViewInfo GetViewInfo(string viewName)
        {
            TabsInx++;
            ViewInfo viewInfo = null;
            // 列信息
            DataTable dtColumnInfo = _GetAllViewInfo(viewName);
            if (dtColumnInfo != null && dtColumnInfo.Rows.Count > 0)
            {
                viewInfo = new ViewInfo();
                var row = dtColumnInfo.Rows[0];
                viewInfo.Name = row[0] + "";
                viewInfo.CreateSQL = row[1] + "";
                viewInfo.ClientCharSet = row[2] + "";
                viewInfo.CharSet = row[3] + "";
            }
            GetDBTableInfohander?.Invoke(viewName, TabsCount, TabsInx);
            return viewInfo;
        }

        private const string _SELECT_VIEW_INFO_SQL = "SHOW CREATE VIEW `{0}`.`{1}`;";
        /// <summary>
        /// 获取视图详细信息
        /// </summary>
        private DataTable _GetAllViewInfo(string tableName)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_VIEW_INFO_SQL, _SchemaName, tableName), Conn);
            return _ExecuteSqlCommand(cmd);
        }


        private const string _SELECT_VIEWS_SQL = "SELECT * from information_schema.`TABLES` where table_schema = '{0}' and (TABLE_TYPE = 'VIEW'  )";
        /// <summary>
        /// 查询所有视图名称
        /// </summary>
        /// <returns></returns>
        private DataTable _GetViews()
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_VIEWS_SQL, _SchemaName), Conn);
            return _ExecuteSqlCommand(cmd);
        }

        private const string _DROP_VIEW_SQL = "DROP VIEW `{0}`;\n";
        public string GetDropViewSql(string viewName)
        {
            return string.Format(_DROP_VIEW_SQL, viewName);
        }
        private const string _ADD_VIEW_SQL = "{0}\n";
        public string GetAddViewSql(string sql)
        {
            return string.Format(_ADD_VIEW_SQL, sql);
        }
        #endregion


        #region 函数

        public bool GetFuncs(out List<Function> list, out string errorString)
        {
            TabsCount = 0;
            TabsInx = 0;
            list = new List<Function>();
            try
            {
                //在调用之前已经打开连接，不知道为什么这里的数据库连接状态是关闭的要再次打开
                if (Conn.State == ConnectionState.Closed)
                    Conn.Open();
                if (Conn.State == ConnectionState.Open)
                {
                    // 获取已存在的数据表名
                    DataTable schemaInfo = _GetFuncs();
                    Function f;
                    foreach (DataRow row in schemaInfo.Rows)
                    {
                        f = new Function();
                        f.Name = row[1] + "";
                        f.Definer = row[3] + "";
                        f.Type = FunctionEnum.FUNCTION;
                        f.Modified = Convert.ToDateTime(row[4] + "");
                        f.Created = Convert.ToDateTime(row[4] + "");
                        f.Comment = row[7] + "";
                        f.ClientCharSet = row[8] + "";
                        f.CharSet = row[9] + "";
                        list.Add(f);
                    }

                    TabsCount = list.Count;
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
        public FunctionInfo GetFuncInfo(string viewName)
        {
            TabsInx++;
            FunctionInfo viewInfo = null;
            // 列信息
            DataTable dtColumnInfo = _GetAllFuncInfo(viewName);
            if (dtColumnInfo != null && dtColumnInfo.Rows.Count > 0)
            {
                viewInfo = new FunctionInfo();
                var row = dtColumnInfo.Rows[0];
                viewInfo.Name = row[0] + "";
                viewInfo.SQLModel = row[1] + "";
                viewInfo.CreateSQL = row[2] + "";
                viewInfo.ClientCharSet = row[3] + "";
                viewInfo.CharSet = row[4] + "";
            }
            GetDBTableInfohander?.Invoke(viewName, TabsCount, TabsInx);
            return viewInfo;
        }


        private const string _SELECT_Func_INFO_SQL = "SHOW CREATE FUNCTION `{0}`.`{1}`;";
        /// <summary>
        /// 获取视图详细信息
        /// </summary>
        private DataTable _GetAllFuncInfo(string tableName)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_Func_INFO_SQL, _SchemaName, tableName), Conn);
            return _ExecuteSqlCommand(cmd);
        }

        private const string _SELECT_FUNC_SQL = "SHOW FUNCTION STATUS WHERE `Db`='{0}';";
        /// <summary>
        /// 查询所有视图名称
        /// </summary>
        /// <returns></returns>
        private DataTable _GetFuncs()
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_FUNC_SQL, _SchemaName), Conn);
            return _ExecuteSqlCommand(cmd);
        }


        private const string _DROP_FUNC_SQL = "DROP FUNCTION `{0}`;\r\n";
        public string GetDropFuncSql(string viewName)
        {
            return string.Format(_DROP_FUNC_SQL, viewName);
        }
        private const string _ADD_FUNC_SQL = "DELIMITER $$\r\n{0}$$\r\nDELIMITER ;\r\n";
        public string GetAddFuncSql(Function model)
        {
            string sql = model.Info.CreateSQL;
            if (sql.IndexOf("CREATE DEFINER=") > -1 && sql.IndexOf("FUNCTION") > 0)
            {
                sql = "CREATE " + sql.Substring(sql.IndexOf("FUNCTION"));
            }
            return string.Format(_ADD_FUNC_SQL, sql);
        }

        #endregion

        #region 存储过程


        /// <summary>
        /// 查询所有存储过程名称
        /// </summary>
        /// <returns></returns>
        public bool GetProcs(out List<Function> list, out string errorString)
        {
            TabsCount = 0;
            TabsInx = 0;
            list = new List<Function>();
            try
            {
                //在调用之前已经打开连接，不知道为什么这里的数据库连接状态是关闭的要再次打开
                if (Conn.State == ConnectionState.Closed)
                    Conn.Open();
                if (Conn.State == ConnectionState.Open)
                {
                    // 获取已存在的数据表名
                    DataTable schemaInfo = _GetProcs();
                    Function f;
                    foreach (DataRow row in schemaInfo.Rows)
                    {
                        f = new Function();
                        f.Name = row[1] + "";
                        f.Definer = row[3] + "";
                        f.Type = FunctionEnum.PROCEDURE;
                        f.Modified = Convert.ToDateTime(row[4] + "");
                        f.Created = Convert.ToDateTime(row[4] + "");
                        f.Comment = row[7] + "";
                        f.ClientCharSet = row[8] + "";
                        f.CharSet = row[9] + "";
                        list.Add(f);
                    }

                    TabsCount = list.Count;
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
        /// 将某存储过程的属性作为FunctionInfo类返回
        /// </summary>
        public FunctionInfo GetProcInfo(string viewName)
        {
            TabsInx++;
            FunctionInfo viewInfo = null;
            // 列信息
            DataTable dtColumnInfo = _GetAllProcInfo(viewName);
            if (dtColumnInfo != null && dtColumnInfo.Rows.Count > 0)
            {
                viewInfo = new FunctionInfo();
                var row = dtColumnInfo.Rows[0];
                viewInfo.Name = row[0] + "";
                viewInfo.SQLModel = row[1] + "";
                viewInfo.CreateSQL = row[2] + "";
                viewInfo.ClientCharSet = row[3] + "";
                viewInfo.CharSet = row[4] + "";
            }
            GetDBTableInfohander?.Invoke(viewName, TabsCount, TabsInx);
            return viewInfo;
        }


        private const string _SELECT_Proc_INFO_SQL = "SHOW CREATE PROCEDURE `{0}`.`{1}`;";
        /// <summary>
        /// 获取存储过程详细信息
        /// </summary>
        private DataTable _GetAllProcInfo(string tableName)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_Proc_INFO_SQL, _SchemaName, tableName), Conn);
            return _ExecuteSqlCommand(cmd);
        }

        private const string _SELECT_PROC_SQL = "SHOW PROCEDURE STATUS WHERE `Db`='{0}';";
        /// <summary>
        /// 查询所有存储过程名称
        /// </summary>
        /// <returns></returns>
        private DataTable _GetProcs()
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_PROC_SQL, _SchemaName), Conn);
            return _ExecuteSqlCommand(cmd);
        }



        private const string _DROP_PROCS_SQL = "DROP PROCEDURE `{0}`;\n";
        public string GetDropProcsSql(string viewName)
        {
            return string.Format(_DROP_PROCS_SQL, viewName);
        }
        private const string _ADD_PROCS_SQL = "DELIMITER $$\r\n{0}$$\r\nDELIMITER ;\r\n";
        public string GetAddProcsSql(Function model)
        {
            string sql = model.Info.CreateSQL;
            if (sql.IndexOf("CREATE DEFINER=") > -1 && sql.IndexOf("PROCEDURE") > 0)
            {
                sql = "CREATE " + sql.Substring(sql.IndexOf("PROCEDURE"));
            }
            return string.Format(_ADD_PROCS_SQL, sql);
        }

        #endregion

        #region 触发器


        /// <summary>
        /// 查询所触发器名称
        /// </summary>
        /// <returns></returns>
        public bool GetTris(out List<Trigger> list, out string errorString)
        {
            TabsCount = 0;
            TabsInx = 0;
            list = new List<Trigger>();
            try
            {
                //在调用之前已经打开连接，不知道为什么这里的数据库连接状态是关闭的要再次打开
                if (Conn.State == ConnectionState.Closed)
                    Conn.Open();
                if (Conn.State == ConnectionState.Open)
                {
                    // 获取已存在的数据表名
                    DataTable schemaInfo = _GetTris();
                    Trigger t;
                    foreach (DataRow info in schemaInfo.Rows)
                    {
                        t = new Trigger();
                        t.Name = info[0] + "";
                        t.Event = (TriggerEvent)Enum.Parse(typeof(TriggerEvent), info[1] + "", true);
                        t.TableName = info[2] + "";
                        t.Statement = info[3] + "";
                        t.Time =  (TeiggerTime)Enum.Parse(typeof(TeiggerTime), info[4] + "", true);
                        t.Created = Convert.ToDateTime(info[5] + "");
                        t.SQLMode = info[6] + "";
                        t.Definer = info[7] + "";
                        t.ClientCharSet = info[8] + "";
                        t.CharSet = info[9] + "";
                        list.Add(t);
                    }

                    TabsCount = list.Count;
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


        private const string _SELECT_TRI_SQL = "SHOW TRIGGERS FROM `{0}`;";
        /// <summary>
        /// 查询所触发器名称
        /// </summary>
        /// <returns></returns>
        private DataTable _GetTris()
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(_SELECT_TRI_SQL, _SchemaName), Conn);
            return _ExecuteSqlCommand(cmd);
        }



        private const string _DROP_TRIS_SQL = "DROP TRIGGER `{0}`;\n";
        public string GetDropTrisSql(string viewName)
        {
            return string.Format(_DROP_TRIS_SQL, viewName);
        }
        private const string _ADD_TRIS_SQL = @"CREATE TRIGGER {0} 
    {1} {2} on {3} 
    FOR EACH ROW 
{4}\n";
        public string GetAddTrisSql(Trigger model)
        {
            return string.Format(_ADD_TRIS_SQL, model.Name, model.Time.ToString(), model.Event.ToString(), model.TableName, model.Statement);
        }
        #endregion


    }


}
