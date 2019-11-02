using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public interface CompareAndShowResultHelper<T> where T : DBInfo
    {
        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="rel"></param>
        /// <returns></returns>
        bool GetInfoByDb(string connStr, out Dictionary<string, T> rel);
        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="rel"></param>
        /// <returns></returns>
        string GetInfoByFile(string dirName, string fileName, out Dictionary<string, T> list);
        /// <summary>
        /// 比较并输出信息到界面中
        /// </summary>
        /// <param name="oldItems"></param>
        /// <param name="newItems"></param>
        /// <param name="setting"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        bool CompareAndShow(Dictionary<string, T> oldItems, Dictionary<string, T> newItems, Setting setting, out string errorString);
    }
    public delegate bool OutPutTextHander(string str, OutputType type = OutputType.Comment);
    public delegate string GetResultTextHander();
    public class CompareAndShowResultHelperBase
    {
        /// <summary>
        /// 转换数字为指定精度的小数(四舍五入)
        /// </summary>
        /// <param name="_input">要转换的小数</param>
        /// <param name="fractionDigits">保留的精度</param>
        /// <returns>转换后的结果</returns>
        public static double ToPrecision(double _input, int fractionDigits)
        {
            return Math.Round(_input, fractionDigits, MidpointRounding.AwayFromZero);
        }
        public OutPutTextHander OutputText { get; set; }
        public OutPutTextHander DeleteLastLintText { get; set; }
        public OutPutTextHander ReplaceLastLineText { get; set; }

        List<Tuple<string, OutputType>> ts = new List<Tuple<string, OutputType>>();
        public void AppendLine(string str, OutputType type)
        {
            ts.Add(new Tuple<string, OutputType>(str, type));
        }

        public void AppendLineToCtrl(int minCount = 3)
        {
            if (ts.Count < minCount)
            {
                ts.Clear();
                return;
            }
            foreach (var item in ts)
            {
                OutputText(item.Item1, item.Item2);
            }
            ts.Clear();
        }


        public MySqlOptionHelper Helper { get; set; }

        DateTime startTime=default(DateTime);
        public void SetLen(string currTabName, int tabCount, int i)
        {
            if (i == 0 || i == 1)
            {
                startTime = DateTime.Now;
            }
            if (tabCount == i || true)
            {
                ReplaceLastLineText(string.Format("当前进度：{0}% {1}/{2} ，耗时：" + Tools.GetTimeSpan(DateTime.Now - startTime), ToPrecision(i * 100.0 / tabCount, 2), i, tabCount, currTabName), OutputType.Comment);
            }
            else
                ReplaceLastLineText(string.Format("当前进度：{0}% {1}/{2} ({3}) ", ToPrecision(i * 100.0 / tabCount, 2), i, tabCount, currTabName), OutputType.Comment);
        }


        public Dictionary<string, T> GetInfo<T>(string dirName, string fileName) where T : DBInfo
        {
            Dictionary<string, T> rel = new Dictionary<string, T>();
            string resultStr = ReadFileString(dirName, fileName);
            List<T> newtabRel = JsonConvert.DeserializeObject<List<T>>(resultStr);
            foreach (var tab in newtabRel)
            {
                rel.Add(tab.Name, tab);
            }
            return rel;
        }

        /// <summary>
        /// 将List中的所有数据用指定分隔符连接为一个新字符串
        /// </summary>
        public string CombineString(IList<string> list, string separateString)
        {
            if (list == null || list.Count < 1)
                return null;
            else
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < list.Count; ++i)
                    builder.Append(list[i]).Append(separateString);

                string result = builder.ToString();
                // 去掉最后多加的一次分隔符
                if (separateString != null)
                    return result.Substring(0, result.Length - separateString.Length);
                else
                    return result;
            }
        }

        public string ReadFileString(string dirName, string fileName, string currDir = null)
        {
            if (currDir == null)
            {
                currDir = Environment.CurrentDirectory;
            }
            string resultStr = currDir + string.Format("/result/{0}/{1}", dirName, fileName);
            if (!File.Exists(resultStr))
                throw new Exception(string.Format("文件不存在（{0}/{1}）", dirName, fileName));
            return File.ReadAllText(resultStr);
        }

        
    }
    public class TableCompareAndShowResultHelper : CompareAndShowResultHelperBase, CompareAndShowResultHelper<TableInfo>
    {

        public bool GetInfoByDb(string connStr, out Dictionary<string, TableInfo> rel)
        {
            rel = new Dictionary<string, TableInfo>();
            using (Helper = new MySqlOptionHelper(connStr))
            {
                if (Helper.Open())
                {
                    OutputText?.Invoke("开始获取旧数据库表结构(" + Helper.DbName + ")\n", OutputType.Comment);
                    Helper.Set_DbHander(SetLen);
                    if (Helper.GetTables(out List<string> tempList, out string errorMsg))
                    {
                        OutputText?.Invoke("  获取到 " + tempList.Count + " 个表\n", OutputType.Comment);
                        OutputText?.Invoke("...", OutputType.Comment);
                        foreach (string tabName in tempList)
                        {
                            rel.Add(tabName, Helper.GetTableInfo(tabName));
                        }
                        OutputText?.Invoke("\n", OutputType.None);
                        OutputText?.Invoke("\n", OutputType.Comment);
                        return true;
                    }
                    else
                    {
                        OutputText?.Invoke("获取表信息失败：" + errorMsg, OutputType.Comment);
                        return false;
                    }
                }
                else
                {
                    OutputText("打开数据库失败(" + Helper.DbName + ")", OutputType.Comment);
                    return false;
                }
            }
        }

        public string GetInfoByFile(string dirName, string fileName, out Dictionary<string, TableInfo> list)
        {
            list = new Dictionary<string, TableInfo>();
            try
            {
                list = GetInfo<TableInfo>(dirName, fileName);
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public bool CompareAndShow(Dictionary<string, TableInfo> oldItems, Dictionary<string, TableInfo> newItems, Setting setting, out string errorString)
        {
            StringBuilder errorStringBuilder = new StringBuilder();

            // 找出新版本中删除的表
            List<string> dropTableNames = new List<string>();
            foreach (string tableName in oldItems.Keys)
            {
                if (!newItems.Keys.Contains(tableName))
                    dropTableNames.Add(tableName);
            }
            if (dropTableNames.Count > 0)
            {
                OutputText("==============================================\n", OutputType.Comment);
                OutputText(string.Format("新版本数据库中删除以下表格：{0}\n", CombineString(dropTableNames, ",")), OutputType.Comment);
                //foreach (string tableName in dropTableNames)
                //{
                //    str.Add(new Tuple<string, OutputType>(string.Format("生成删除{0}表的SQL\n", tableName), OutputType.Comment);
                //    if (AppValues.AllTableCompareRule.ContainsKey(tableName) && AppValues.AllTableCompareRule[tableName].CompareWay == TableCompareWays.Ignore)
                //    {
                //        str.Add(new Tuple<string, OutputType>("该表格配置为忽略比较，故不进行删除\n", OutputType.Warning);
                //        continue;
                //    }
                //    string dropTableSql = _GetDropTableSql(AppValues.OldSchemaName, tableName);
                //    str.Add(new Tuple<string, OutputType>(dropTableSql, OutputType.Sql);
                //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                //}
            }
            // 找出新版本中新增的表
            List<string> addTableNames = new List<string>();
            foreach (string tableName in newItems.Keys)
            {
                if (!oldItems.Keys.Contains(tableName))
                    addTableNames.Add(tableName);
            }
            if (addTableNames.Count > 0)
            {
                OutputText("==============================================\n", OutputType.Comment);
                OutputText(string.Format("新版本数据库中新增以下表格：{0}\n", CombineString(addTableNames, ",")), OutputType.Comment);
                //foreach (string tableName in addTableNames)
                //{
                //    str.Add(new Tuple<string, OutputType>(string.Format("生成创建{0}表及填充数据的SQL\n", tableName), OutputType.Comment);
                //    if (AppValues.AllTableCompareRule.ContainsKey(tableName) && AppValues.AllTableCompareRule[tableName].CompareWay == TableCompareWays.Ignore)
                //    {
                //        str.Add(new Tuple<string, OutputType>("该表格配置为忽略比较，故不进行新建\n", OutputType.Warning);
                //        continue;
                //    }
                //    // 通过MySQL提供的功能得到建表SQL
                //    string createTableSql = _GetCreateTableSql(AppValues.NewSchemaName, tableName, AppValues.OldSchemaName, AppValues.NewConn);
                //    str.Add(new Tuple<string, OutputType>(createTableSql, OutputType.Sql);
                //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                //    // 得到填充数据的SQL
                //    DataTable data = _SelectData(AppValues.NewSchemaName, tableName, "*", AppValues.NewConn);
                //    string fillDataSql = _GetFillDataSql(AppValues.NewTableInfo[tableName], data, AppValues.OldSchemaName);
                //    if (!string.IsNullOrEmpty(fillDataSql))
                //    {
                //        str.Add(new Tuple<string, OutputType>(fillDataSql, OutputType.Sql);
                //        str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                //    }
                //}
            }
            // 对两版本中均存在的表格进行对比
            foreach (string tableName in newItems.Keys)
            {
                if (oldItems.Keys.Contains(tableName))
                {
                    AppendLine("----------------------------------------------\n", OutputType.Comment);
                    AppendLine(string.Format("表：{0}\n", tableName), OutputType.Comment);
                    TableInfo newTableInfo = newItems[tableName];
                    TableInfo oldTableInfo = oldItems[tableName];

                    //TableCompareRule compareRule = null;
                    //if (AppValues.AllTableCompareRule.ContainsKey(tableName))
                    //{
                    //    compareRule = AppValues.AllTableCompareRule[tableName];
                    //    using (StringReader reader = new StringReader(compareRule.GetCompareRuleComment()))
                    //    {
                    //        string line = null;
                    //        while ((line = reader.ReadLine()) != null)
                    //        {
                    //            str.Add(new Tuple<string, OutputType>(line, OutputType.Comment);
                    //            str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    compareRule = new TableCompareRule();
                    //    compareRule.CompareWay = TableCompareWays.ColumnInfoAndData;
                    //    str.Add(new Tuple<string, OutputType>("未设置对该表格进行对比的方式，将默认对比表结构及数据\n", OutputType.Warning);
                    //}

                    // 进行表结构比较
                    const string SPLIT_STRING = ",\n";
                    bool isPrimaryKeySame = true;
                    //if (compareRule.CompareWay != TableCompareWays.Ignore)
                    {
                        //str.Add(new Tuple<string, OutputType>("开始进行结构对比\n", OutputType.Comment);
                        //// 修改表结构的SQL开头
                        //string alterTableSqlPrefix = string.Format(_ALTER_TABLE_SQL, _CombineDatabaseTableFullName(AppValues.OldSchemaName, tableName));
                        string alterTableSqlPrefix = "";
                        //// 标识是否输出过修改表结构的SQL开头
                        //bool hasOutputPrefix = false;
                        bool hasOutputPrefix = true;
                        // 标识是否输出过该对比部分中的第一条SQL，非第一条输出前先加逗号并换行
                        bool hasOutputPartFirstSql = false;

                        // 找出删除列
                        List<string> dropColumnNames = new List<string>();
                        foreach (string columnName in oldTableInfo.AllColumnInfo.Keys)
                        {
                            if (!newTableInfo.AllColumnInfo.ContainsKey(columnName))
                                dropColumnNames.Add(columnName);
                        }
                        if (dropColumnNames.Count > 0)
                        {
                            //if (hasOutputPrefix == false)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(alterTableSqlPrefix, OutputType.Sql);
                            //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                            //    hasOutputPrefix = true;
                            //}
                            //// 如果之前对比出差异并进行了输出，需要先为上一条语句添加逗号结尾
                            //if (hasOutputPartFirstSql == true)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                            //    hasOutputPartFirstSql = false;
                            //}
                            AppendLine(string.Format("新版本中删除以下列：{0}\n", CombineString(dropColumnNames, ",")), OutputType.Comment);
                            //foreach (string columnName in dropColumnNames)
                            //{
                            //    if (hasOutputPartFirstSql == false)
                            //        hasOutputPartFirstSql = true;
                            //    else
                            //        str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);

                            //    string dropColumnSql = string.Format(_DROP_COLUMN_SQL, columnName);
                            //    str.Add(new Tuple<string, OutputType>(dropColumnSql, OutputType.Sql);
                            //}
                        }

                        // 找出新增列
                        List<string> addColumnNames = new List<string>();
                        foreach (string columnName in newTableInfo.AllColumnInfo.Keys)
                        {
                            if (!oldTableInfo.AllColumnInfo.ContainsKey(columnName))
                                addColumnNames.Add(columnName);
                        }
                        string addColumnNameString = CombineString(addColumnNames, ",");
                        if (addColumnNames.Count > 0)
                        {
                            //if (hasOutputPrefix == false)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(alterTableSqlPrefix, OutputType.Sql);
                            //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                            //    hasOutputPrefix = true;
                            //}
                            //if (hasOutputPartFirstSql == true)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                            //    hasOutputPartFirstSql = false;
                            //}
                            AppendLine(string.Format("新版本中新增以下列：{0}\n", CombineString(addColumnNames, ",")), OutputType.Comment);
                            //foreach (string columnName in addColumnNames)
                            //{
                            //    if (hasOutputPartFirstSql == false)
                            //        hasOutputPartFirstSql = true;
                            //    else
                            //        str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);

                            //    // 根据新增列属性生成添加新列的SQL
                            //    ColumnInfo columnInfo = newTableInfo.AllColumnInfo[columnName];
                            //    string notEmptyString = columnInfo.IsNotEmpty == true ? "NOT NULL" : "NULL";
                            //    // 注意如果列设为NOT NULL，就不允许设置默认值为NULL
                            //    string defaultValue = columnInfo.DefaultValue.Equals("NULL") ? string.Empty : string.Concat(" DEFAULT ", columnInfo.DefaultValue);
                            //    string addColumnSql = string.Format(_ADD_COLUMN_SQL, columnName, columnInfo.DataType, notEmptyString, defaultValue, columnInfo.Comment);
                            //    str.Add(new Tuple<string, OutputType>(addColumnSql, OutputType.Sql);
                            //}
                        }

                        // 在改变列属性前需先同步索引设置，因为自增属性仅可用于设置了索引的列
                        // 找出主键修改
                        isPrimaryKeySame = true;
                        if (newTableInfo.PrimaryKeyColumnNames.Count != oldTableInfo.PrimaryKeyColumnNames.Count)
                            isPrimaryKeySame = false;
                        else
                        {
                            foreach (string primaryKey in newTableInfo.PrimaryKeyColumnNames)
                            {
                                if (!oldTableInfo.PrimaryKeyColumnNames.Contains(primaryKey))
                                {
                                    isPrimaryKeySame = false;
                                    break;
                                }
                            }
                        }
                        if (isPrimaryKeySame == false)
                        {
                            //if (hasOutputPrefix == false)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(alterTableSqlPrefix, OutputType.Sql);
                            //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                            //    hasOutputPrefix = true;
                            //}
                            //if (hasOutputPartFirstSql == true)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                            //    hasOutputPartFirstSql = false;
                            //}
                            string newPrimaryKeyString = newTableInfo.PrimaryKeyColumnNames.Count > 0 ? CombineString(newTableInfo.PrimaryKeyColumnNames, ",") : "无";
                            string oldPrimaryKeyString = oldTableInfo.PrimaryKeyColumnNames.Count > 0 ? CombineString(oldTableInfo.PrimaryKeyColumnNames, ",") : "无";
                            AppendLine(string.Format("  主键：{0} => {1}\n", newPrimaryKeyString, oldPrimaryKeyString), OutputType.Comment);
                            //// 先删除原有的主键设置
                            //str.Add(new Tuple<string, OutputType>(_DROP_PRIMARY_KEY_SQL, OutputType.Sql);
                            //str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                            //// 再重新设置
                            //List<string> primaryKeyDefine = new List<string>();
                            //foreach (string primaryKey in newTableInfo.PrimaryKeyColumnNames)
                            //    primaryKeyDefine.Add(string.Format("`{0}`", primaryKey));

                            //string addPrimaryKeySql = string.Format(_ADD_PRIMARY_KEY_SQL, CombineString(primaryKeyDefine, ","));
                            //str.Add(new Tuple<string, OutputType>(addPrimaryKeySql, OutputType.Sql);
                            //hasOutputPartFirstSql = true;
                        }

                        // 找出唯一索引修改
                        // 找出新版本中删除的索引
                        List<string> dropIndexNames = new List<string>();
                        foreach (string name in oldTableInfo.IndexInfo.Keys)
                        {
                            if (!newTableInfo.IndexInfo.ContainsKey(name))
                                dropIndexNames.Add(name);
                        }
                        if (dropIndexNames.Count > 0)
                        {
                            //if (hasOutputPrefix == false)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(alterTableSqlPrefix, OutputType.Sql);
                            //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                            //    hasOutputPrefix = true;
                            //}
                            //if (hasOutputPartFirstSql == true)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                            //    hasOutputPartFirstSql = false;
                            //}
                            AppendLine(string.Format("  新版本中删除以下索引：{0}\n", CombineString(dropIndexNames, ",")), OutputType.Comment);
                            if (CombineString(dropIndexNames, ",") == "index_Type")
                            {
                                int a = 0;
                            }
                            //foreach (string name in dropIndexNames)
                            //{
                            //    if (hasOutputPartFirstSql == false)
                            //        hasOutputPartFirstSql = true;
                            //    else
                            //        str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);

                            //    //string dropIndexSql = string.Format(_DROP_INDEX_SQL, name);
                            //    //str.Add(new Tuple<string, OutputType>(dropIndexSql, OutputType.Sql);
                            //}
                        }
                        // 找出新版本中新增索引
                        List<string> addIndexNames = new List<string>();
                        foreach (string name in newTableInfo.IndexInfo.Keys)
                        {
                            if (!oldTableInfo.IndexInfo.ContainsKey(name))
                                addIndexNames.Add(name);
                        }
                        if (addIndexNames.Count > 0)
                        {
                            //if (hasOutputPrefix == false)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(alterTableSqlPrefix, OutputType.Sql);
                            //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                            //    hasOutputPrefix = true;
                            //}
                            //if (hasOutputPartFirstSql == true)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                            //    hasOutputPartFirstSql = false;
                            //}
                            AppendLine(string.Format("  新版本中新增以下索引：{0}\n", CombineString(addIndexNames, ",")), OutputType.Comment);
                            //foreach (string name in addIndexNames)
                            //{
                            //    if (hasOutputPartFirstSql == false)
                            //        hasOutputPartFirstSql = true;
                            //    else
                            //        str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);

                            //    // 根据新增索引属性生成添加新索引的SQL
                            //    // 注意列名后必须声明排序方式，MySQL只支持索引的升序排列
                            //    List<string> columnDefine = new List<string>();
                            //    foreach (string columnName in newTableInfo.IndexInfo[name])
                            //        columnDefine.Add(string.Format("`{0}` ASC", columnName));

                            //    //string addIndexSql = string.Format(_ADD_UNIQUE_INDEX_SQL, name, CombineString(columnDefine, ","));
                            //    //str.Add(new Tuple<string, OutputType>(addIndexSql, OutputType.Sql);
                            //}
                        }
                        // 找出同名索引的变动
                        foreach (var pair in newTableInfo.IndexInfo)
                        {
                            string name = pair.Key;
                            if (oldTableInfo.IndexInfo.ContainsKey(name))
                            {
                                List<string> newIndexColumnInfo = pair.Value;
                                List<string> oldIndexColumnInfo = oldTableInfo.IndexInfo[name];
                                bool isIndexColumnSame = true;
                                if (newIndexColumnInfo.Count != oldIndexColumnInfo.Count)
                                    isIndexColumnSame = false;
                                else
                                {
                                    int count = newIndexColumnInfo.Count;
                                    for (int i = 0; i < count; ++i)
                                    {
                                        if (!newIndexColumnInfo[i].Equals(oldIndexColumnInfo[i]))
                                        {
                                            isIndexColumnSame = false;
                                            break;
                                        }
                                    }
                                }

                                if (isIndexColumnSame == false)
                                {
                                    //if (hasOutputPrefix == false)
                                    //{
                                    //    str.Add(new Tuple<string, OutputType>(alterTableSqlPrefix, OutputType.Sql);
                                    //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                                    //    hasOutputPrefix = true;
                                    //}
                                    //if (hasOutputPartFirstSql == true)
                                    //{
                                    //    str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                                    //    hasOutputPartFirstSql = false;
                                    //}
                                    AppendLine(string.Format("  新版本中名为{0}的索引，涉及的列名为{1}，而旧版本中为{2}\n", name, CombineString(newIndexColumnInfo, ","), CombineString(oldIndexColumnInfo, ",")), OutputType.Comment);
                                    //// 先删除
                                    //string dropIndexSql = string.Format(_DROP_INDEX_SQL, name);
                                    //str.Add(new Tuple<string, OutputType>(dropIndexSql, OutputType.Sql);
                                    //str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                                    //// 再重新创建
                                    //List<string> columnDefine = new List<string>();
                                    //foreach (string columnName in newIndexColumnInfo)
                                    //    columnDefine.Add(string.Format("`{0}` ASC", columnName));

                                    //string addIndexSql = string.Format(_ADD_UNIQUE_INDEX_SQL, name, CombineString(columnDefine, ","));
                                    //str.Add(new Tuple<string, OutputType>(addIndexSql, OutputType.Sql);
                                    //hasOutputPartFirstSql = true;
                                }
                            }
                        }

                        // 找出列属性修改
                        foreach (string columnName in newTableInfo.AllColumnInfo.Keys)
                        {
                            if (oldTableInfo.AllColumnInfo.ContainsKey(columnName))
                            {
                                ColumnInfo newColumnInfo = newTableInfo.AllColumnInfo[columnName];
                                ColumnInfo oldColumnInfo = oldTableInfo.AllColumnInfo[columnName];
                                // 比较各个属性
                                bool isDataTypeSame = newColumnInfo.DataType.Equals(oldColumnInfo.DataType);
                                bool isCommentSame = newColumnInfo.Comment.Equals(oldColumnInfo.Comment);
                                if (!setting.CheckCommon) isCommentSame = true;
                                bool isNotEmptySame = newColumnInfo.IsNotEmpty == oldColumnInfo.IsNotEmpty;
                                bool isAutoIncrementSame = newColumnInfo.IsAutoIncrement == oldColumnInfo.IsAutoIncrement;
                                bool isDefaultValueSame = newColumnInfo.DefaultValue.Equals(oldColumnInfo.DefaultValue);
                                if (isDataTypeSame == false || isCommentSame == false || isNotEmptySame == false || isAutoIncrementSame == false || isDefaultValueSame == false)
                                {
                                    //if (hasOutputPrefix == false)
                                    //{
                                    //    str.Add(new Tuple<string, OutputType>(alterTableSqlPrefix, OutputType.Sql);
                                    //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                                    //    hasOutputPrefix = true;
                                    //}
                                    //if (hasOutputPartFirstSql == true)
                                    //{
                                    //    str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                                    //    hasOutputPartFirstSql = false;
                                    //}
                                    AppendLine(string.Format("  列：{0}\n", columnName), OutputType.Comment);
                                    if (isDataTypeSame == false)
                                        AppendLine(string.Format("    属性：数据类型{0} => {1}\n", newColumnInfo.DataType, oldColumnInfo.DataType), OutputType.Comment);
                                    if (isCommentSame == false)
                                        AppendLine(string.Format("    属性：列注释\"{0}\" => \"{1}\"\n", newColumnInfo.Comment, oldColumnInfo.Comment), OutputType.Comment);
                                    if (isNotEmptySame == false)
                                        AppendLine(string.Format("    属性：（为空）{0} => {1}\n", newColumnInfo.IsNotEmpty == true ? "不允许" : "允许", oldColumnInfo.IsNotEmpty == true ? "不允许" : "允许"), OutputType.Comment);
                                    if (isAutoIncrementSame == false)
                                        AppendLine(string.Format("    属性：列设{0}  =>  {1}\n", newColumnInfo.IsAutoIncrement == true ? "自增" : "不自增", oldColumnInfo.IsAutoIncrement == true ? "自增" : "不自增"), OutputType.Comment);
                                    if (isDefaultValueSame == false)
                                        AppendLine(string.Format("    属性：默认值{0}  =>  {1}\n", newColumnInfo.DefaultValue, oldColumnInfo.DefaultValue), OutputType.Comment);

                                    //// 根据新的列属性进行修改
                                    //string notEmptyString = newColumnInfo.IsNotEmpty == true ? "NOT NULL" : "NULL";
                                    //string defaultValue = newColumnInfo.DefaultValue.Equals("NULL") ? string.Empty : string.Concat(" DEFAULT ", newColumnInfo.DefaultValue);
                                    //string changeColumnSql = string.Format(_CHANGE_COLUMN_SQL, columnName, newColumnInfo.DataType, notEmptyString, defaultValue, newColumnInfo.Comment);
                                    //str.Add(new Tuple<string, OutputType>(changeColumnSql, OutputType.Sql);
                                    //hasOutputPartFirstSql = true;
                                }
                            }
                        }

                        // 对比表校对集
                        if (!newTableInfo.Collation.Equals(oldTableInfo.Collation))
                        {
                            //if (hasOutputPrefix == false)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(alterTableSqlPrefix, OutputType.Sql);
                            //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                            //    hasOutputPrefix = true;
                            //}
                            //if (hasOutputPartFirstSql == true)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                            //    hasOutputPartFirstSql = false;
                            //}
                            AppendLine(string.Format("  校对集：\"{0}\" => \"{1}\"\n", newTableInfo.Collation, oldTableInfo.Collation), OutputType.Comment);
                            //string alterTableComment = string.Format(_ALTER_TABLE_COLLATION_SQL, newTableInfo.Collation);
                            //str.Add(new Tuple<string, OutputType>(alterTableComment, OutputType.Sql);
                            //hasOutputPartFirstSql = true;
                        }

                        // 对比表注释
                        if (setting.CheckCommon && !newTableInfo.Comment.Equals(oldTableInfo.Comment))
                        {
                            //if (hasOutputPrefix == false)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(alterTableSqlPrefix, OutputType.Sql);
                            //    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                            //    hasOutputPrefix = true;
                            //}
                            //if (hasOutputPartFirstSql == true)
                            //{
                            //    str.Add(new Tuple<string, OutputType>(SPLIT_STRING, OutputType.Sql);
                            //    hasOutputPartFirstSql = false;
                            //}
                            AppendLine(string.Format("  注释：\"{0}\" => \"{1}\"\n", newTableInfo.Comment, oldTableInfo.Comment), OutputType.Comment);
                            //string alterTableComment = string.Format(_ALTER_TABLE_COMMENT_SQL, newTableInfo.Comment);
                            //str.Add(new Tuple<string, OutputType>(alterTableComment, OutputType.Sql);
                            //hasOutputPartFirstSql = true;
                        }

                        //// 最后添加分号结束
                        //if (hasOutputPartFirstSql == true)
                        //{
                        //    str.Add(new Tuple<string, OutputType>(";\n", OutputType.Sql);
                        //    hasOutputPrefix = false;
                        //    hasOutputPartFirstSql = false;
                        //}

                        //// 进行表数据比较
                        //if (compareRule.CompareWay == TableCompareWays.ColumnInfoAndData)
                        //{
                        //    str.Add(new Tuple<string, OutputType>("开始进行数据对比\n", OutputType.Comment);

                        //    // 检查表格是否设置了主键，本工具生成的同步数据的SQL需要通过主键确定数据行
                        //    if (newTableInfo.PrimaryKeyColumnNames.Count == 0)
                        //    {
                        //        string tips = string.Format("错误：表格\"{0}\"未设置主键，本工具无法通过主键生成定位并更新数据的SQL，请设置主键后重试\n本次操作被迫中止\n", tableName);
                        //        str.Add(new Tuple<string, OutputType>(tips, OutputType.Error);
                        //        errorStringBuilder.Append(tips);
                        //        errorString = errorStringBuilder.ToString();
                        //        return;
                        //    }
                        //    // 检查用户设置的对比配置，不允许将主键列设为数据比较时忽略的列
                        //    if (compareRule.CompareIgnoreColumn.Count > 0)
                        //    {
                        //        foreach (string primaryKeyColumnName in newTableInfo.PrimaryKeyColumnNames)
                        //        {
                        //            if (compareRule.CompareIgnoreColumn.Contains(primaryKeyColumnName))
                        //            {
                        //                string tips = string.Format("\n错误：对比数据时不允许将表格主键列设为忽略，而您的配置声明对表格\"{0}\"的主键列\"{1}\"进行忽略，请修正配置后重试\n本次操作被迫中止\n", tableName, primaryKeyColumnName);
                        //                str.Add(new Tuple<string, OutputType>(tips, OutputType.Error);
                        //                errorStringBuilder.Append(tips);
                        //                errorString = errorStringBuilder.ToString();
                        //                return;
                        //            }
                        //        }
                        //    }
                        //    // 如果新旧版本中的主键设置不同，无法进行数据对比
                        //    if (isPrimaryKeySame == false)
                        //    {
                        //        string tips = string.Format("新旧两版本表格\"{0}\"的主键设置不同，本工具目前无法在此情况下进行数据比较并生成同步SQL，请先通过执行上面生成的同步数据库表结构SQL，使得旧版表格和新版为相同的主键设置后，再次运行本工具进行数据比较及同步\n", tableName);
                        //        str.Add(new Tuple<string, OutputType>(tips, OutputType.Error);
                        //        errorStringBuilder.Append(tips);
                        //        continue;
                        //    }

                        //    DataTable newData = _SelectData(AppValues.NewSchemaName, tableName, "*", AppValues.NewConn);
                        //    DataTable oldData = _SelectData(AppValues.OldSchemaName, tableName, "*", AppValues.OldConn);
                        //    Dictionary<string, int> newDataInfo = _GetDataInfoByPrimaryKey(newData, newTableInfo.PrimaryKeyColumnNames);
                        //    Dictionary<string, int> oldDataInfo = _GetDataInfoByPrimaryKey(oldData, newTableInfo.PrimaryKeyColumnNames);

                        //    // 找出删除的数据
                        //    foreach (var pair in oldDataInfo)
                        //    {
                        //        string primaryKeyValueString = pair.Key;
                        //        int index = pair.Value;
                        //        if (!newDataInfo.ContainsKey(primaryKeyValueString))
                        //        {
                        //            DataRow dataRow = oldData.Rows[index];
                        //            string primaryKeyColumnNameAndValueString = _GetColumnNameAndValueString(dataRow, newTableInfo.PrimaryKeyColumnNames, " AND ");
                        //            str.Add(new Tuple<string, OutputType>(string.Concat("新版本中删除了主键列为以下值的一行：", primaryKeyColumnNameAndValueString, "\n"), OutputType.Comment);
                        //            // 判断该行数据是否被设为忽略
                        //            //if (_IsIgnoreData(compareRule.CompareIgnoreData, dataRow) == true)
                        //            //    str.Add(new Tuple<string, OutputType>("该行符合配置的需忽略的数据行，故不进行删除\n", OutputType.Warning);
                        //            //else
                        //            //{
                        //            string dropDataSql = string.Format(_DROP_DATA_SQL, _CombineDatabaseTableFullName(AppValues.OldSchemaName, tableName), _GetColumnNameAndValueString(dataRow, newTableInfo.PrimaryKeyColumnNames, " AND "));
                        //            str.Add(new Tuple<string, OutputType>(dropDataSql, OutputType.Sql);
                        //            str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                        //            //}
                        //        }
                        //    }

                        //    // 找出需要对比数据的列（以新版表中所有列为基准，排除用户设置的忽略列以及旧版表中不存在的列，因为主键列的值在找新旧两表对应行时已经对比，也无需比较）
                        //    List<string> compareColumnNames = new List<string>();
                        //    foreach (string columnName in newTableInfo.AllColumnInfo.Keys)
                        //    {
                        //        if (oldTableInfo.AllColumnInfo.ContainsKey(columnName) && !newTableInfo.PrimaryKeyColumnNames.Contains(columnName) && !compareRule.CompareIgnoreColumn.Contains(columnName))
                        //            compareColumnNames.Add(columnName);
                        //    }

                        //    // 生成新增数据时所有列名组成的定义字符串
                        //    List<string> columnDefine = new List<string>();
                        //    foreach (string columnName in newTableInfo.AllColumnInfo.Keys)
                        //        columnDefine.Add(string.Format("`{0}`", columnName));

                        //    string columnDefineString = CombineString(columnDefine, ", ");

                        //    foreach (var pair in newDataInfo)
                        //    {
                        //        string primaryKeyValueString = pair.Key;
                        //        int newTableIndex = pair.Value;
                        //        // 新增数据
                        //        if (!oldDataInfo.ContainsKey(primaryKeyValueString))
                        //        {
                        //            DataRow dataRow = newData.Rows[newTableIndex];
                        //            string primaryKeyColumnNameAndValueString = _GetColumnNameAndValueString(dataRow, newTableInfo.PrimaryKeyColumnNames, " AND ");
                        //            str.Add(new Tuple<string, OutputType>(string.Concat("新版本中新增主键列为以下值的一行：", primaryKeyColumnNameAndValueString, "\n"), OutputType.Comment);
                        //            //// 判断该行数据是否被设为忽略
                        //            //if (_IsIgnoreData(compareRule.CompareIgnoreData, dataRow) == true)
                        //            //    str.Add(new Tuple<string, OutputType>("该行符合配置的需忽略的数据行，故不进行新增\n", OutputType.Warning);
                        //            //else
                        //            //{
                        //            List<string> values = new List<string>();
                        //            foreach (string columnName in newTableInfo.AllColumnInfo.Keys)
                        //            {
                        //                object value = dataRow[columnName];
                        //                values.Add(_GetDatabaseValueString(value));
                        //            }

                        //            string valueString = string.Format("({0})", CombineString(values, ","));
                        //            string insertDataSql = string.Format(_INSERT_DATA_SQL, _CombineDatabaseTableFullName(AppValues.OldSchemaName, tableName), columnDefineString, valueString);
                        //            str.Add(new Tuple<string, OutputType>(insertDataSql, OutputType.Sql);
                        //            str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                        //            //}
                        //        }
                        //        // 判断未被忽略的列中的数据是否相同
                        //        else
                        //        {
                        //            int oldTableIndex = oldDataInfo[primaryKeyValueString];
                        //            DataRow newDataRow = newData.Rows[newTableIndex];
                        //            DataRow oldDataRow = oldData.Rows[oldTableIndex];

                        //            List<string> dataDiffColumnNames = new List<string>();
                        //            foreach (string columnName in compareColumnNames)
                        //            {
                        //                string newDataValue = _GetDatabaseValueString(newDataRow[columnName]);
                        //                string oldDataValue = _GetDatabaseValueString(oldDataRow[columnName]);
                        //                if (!newDataValue.Equals(oldDataValue))
                        //                    dataDiffColumnNames.Add(columnName);
                        //            }
                        //            string primaryKeyColumnNameAndValueString = _GetColumnNameAndValueString(newDataRow, newTableInfo.PrimaryKeyColumnNames, " AND ");
                        //            if (dataDiffColumnNames.Count > 0)
                        //            {
                        //                string newColumnNameAndValueString = _GetColumnNameAndValueString(newDataRow, dataDiffColumnNames, ", ");
                        //                string oldColumnNameAndValueString = _GetColumnNameAndValueString(oldDataRow, dataDiffColumnNames, ", ");
                        //                str.Add(new Tuple<string, OutputType>(string.Format("主键为{0}的行中，新版本中以下数据为{1}，而旧版本中为{2}\n", primaryKeyColumnNameAndValueString, newColumnNameAndValueString, oldColumnNameAndValueString), OutputType.Comment);
                        //                // 判断该行数据是否被设为忽略
                        //                if (_IsIgnoreData(compareRule.CompareIgnoreData, newDataRow) == true)
                        //                    str.Add(new Tuple<string, OutputType>("该行符合配置的需忽略的数据行，故不进行修改\n", OutputType.Warning);
                        //                else
                        //                {
                        //                    List<string> values = new List<string>();
                        //                    foreach (string columnName in newTableInfo.AllColumnInfo.Keys)
                        //                    {
                        //                        object value = newDataRow[columnName];
                        //                        values.Add(_GetDatabaseValueString(value));
                        //                    }
                        //                    string valueString = string.Format("({0})", CombineString(values, ","));
                        //                    string updateDataSql = string.Format(_UPDATE_DATA_SQL, _CombineDatabaseTableFullName(AppValues.OldSchemaName, tableName), newColumnNameAndValueString, primaryKeyColumnNameAndValueString);
                        //                    str.Add(new Tuple<string, OutputType>(updateDataSql, OutputType.Sql);
                        //                    str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                        //                }
                        //            }

                        //            // 新版表格中新增列的值需要同步到旧表，无视用户是否设置为忽略列
                        //            if (addColumnNames.Count > 0)
                        //            {
                        //                string addColumnNameAndValueString = _GetColumnNameAndValueString(newDataRow, addColumnNames, ", ");
                        //                str.Add(new Tuple<string, OutputType>(string.Format("为新版本中新增的{0}列填充数据\n", addColumnNameString), OutputType.Comment);
                        //                string updateDataSql = string.Format(_UPDATE_DATA_SQL, _CombineDatabaseTableFullName(AppValues.OldSchemaName, tableName), addColumnNameAndValueString, primaryKeyColumnNameAndValueString);
                        //                str.Add(new Tuple<string, OutputType>(updateDataSql, OutputType.Sql);
                        //                str.Add(new Tuple<string, OutputType>("\n", OutputType.None);
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    AppendLineToCtrl();
                    //if (DeleteLastLintText("表："))
                    //    DeleteLastLintText("----------------------------------------------");
                }
            }

            errorString = errorStringBuilder.ToString();
            return string.IsNullOrWhiteSpace(errorString);
        }
    }
    public class ViewCompareAndShowResultHelper : CompareAndShowResultHelperBase, CompareAndShowResultHelper<ViewInfo>
    {

        public bool GetInfoByDb(string connStr, out Dictionary<string, ViewInfo> rel)
        {
            rel = new Dictionary<string, ViewInfo>();
            using (Helper = new MySqlOptionHelper(connStr))
            {
                if (Helper.Open())
                {
                    OutputText("开始获取数数据库视图结构(" + Helper.DbName + ")\n", OutputType.Comment);
                    Helper.Set_DbHander(SetLen);
                    if (Helper.GetViews(out List<string> tempTrig, out string errorMsg))
                    {
                        OutputText("  获取到 " + tempTrig.Count + " 个视图\n", OutputType.Comment);
                        foreach (var item in tempTrig)
                        {
                            rel.Add(item, Helper.GetViewInfo(item));
                        }
                        OutputText("\n", OutputType.None);
                        OutputText("\n", OutputType.Comment);
                        return true;
                    }
                    else
                    {
                        OutputText("获取视图信息失败：" + errorMsg, OutputType.Comment);
                        return false;
                    }

                }
                else
                {
                    OutputText("打开数据库失败(" + Helper.DbName + ")", OutputType.Comment);
                    return false;
                }
            }
        }

        public string GetInfoByFile(string dirName, string fileName, out Dictionary<string, ViewInfo> list)
        {
            list = new Dictionary<string, ViewInfo>();
            try
            {
                list = GetInfo<ViewInfo>(dirName, fileName);
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public bool CompareAndShow(Dictionary<string, ViewInfo> oldItems, Dictionary<string, ViewInfo> newItems, Setting setting, out string errorString)
        {
            StringBuilder errorStringBuilder = new StringBuilder();

            // 找出新版本中删除的表
            List<string> dropTableNames = new List<string>();
            foreach (string tableName in oldItems.Keys)
            {
                if (!newItems.Keys.Contains(tableName))
                    dropTableNames.Add(tableName);
            }
            //OutputText("\n", OutputType.None);
            //OutputText("\n", OutputType.None);
            if (dropTableNames.Count > 0)
            {
                OutputText("==============================================\n", OutputType.Comment);
                OutputText(string.Format("新版本数据库中删除以下视图：{0}\n", CombineString(dropTableNames, ",")), OutputType.Comment);
            }
            // 找出新版本中新增的表
            List<string> addTableNames = new List<string>();
            foreach (string tableName in newItems.Keys)
            {
                if (!oldItems.Keys.Contains(tableName))
                    addTableNames.Add(tableName);
            }
            if (addTableNames.Count > 0)
            {
                OutputText("==============================================\n", OutputType.Comment);
                OutputText(string.Format("新版本数据库中新增以下视图：{0}\n", CombineString(addTableNames, ",")), OutputType.Comment);
            }
            // 对两版本中均存在的表格进行对比
            foreach (string tableName in newItems.Keys)
            {
                if (oldItems.Keys.Contains(tableName))
                {
                    AppendLine("----------------------------------------------\n", OutputType.Comment);
                    AppendLine(string.Format("视图：{0}\n", tableName), OutputType.Comment);
                    ViewInfo newTableInfo = newItems[tableName];
                    ViewInfo oldTableInfo = oldItems[tableName];


                    // 对比表校对集
                    if (!newTableInfo.CharSet.Equals(oldTableInfo.CharSet))
                    {
                        AppendLine(string.Format("  校对集：\"{0}\" => \"{1}\"\n", newTableInfo.CharSet, oldTableInfo.CharSet), OutputType.Comment);
                    }
                    if (!newTableInfo.ClientCharSet.Equals(oldTableInfo.ClientCharSet))
                    {
                        AppendLine(string.Format("  校对集（Client）：\"{0}\" => \"{1}\"\n", newTableInfo.ClientCharSet, oldTableInfo.ClientCharSet), OutputType.Comment);
                    }
                    //避免DEFINER 和注释产生影响
                    string oldSql = oldTableInfo.CreateSQL.Substring(oldTableInfo.CreateSQL.IndexOf("`" + oldTableInfo.Name + "`"));
                    string newSql = oldTableInfo.CreateSQL.Substring(oldTableInfo.CreateSQL.IndexOf("`" + newTableInfo.Name + "`"));
                    if (!oldSql.Equals(newSql))
                    {
                        AppendLine("  内容有变化\n", OutputType.Comment);
                    }

                    AppendLineToCtrl();
                }
            }

            errorString = errorStringBuilder.ToString();
            return string.IsNullOrWhiteSpace(errorString);
        }
    }
    public class TrigCompareAndShowResultHelper : CompareAndShowResultHelperBase, CompareAndShowResultHelper<Trigger>
    {
        public bool GetInfoByDb(string connStr, out Dictionary<string, Trigger> rel)
        {
            rel = new Dictionary<string, Trigger>();
            using (Helper = new MySqlOptionHelper(connStr))
            {
                if (Helper.Open())
                {
                    OutputText("开始获取数数据库触发器结构(" + Helper.DbName + ")\n", OutputType.Comment);
                    Helper.Set_DbHander(SetLen);
                    if (Helper.GetTris(out List<Trigger> tempTrig, out string errorMsg))
                    {
                        OutputText("  获取到 " + tempTrig.Count + " 个触发器\n", OutputType.Comment);
                        foreach (var item in tempTrig)
                        {
                            rel.Add(item.Name, item);
                        }
                        OutputText("\n", OutputType.None);
                        OutputText("\n", OutputType.Comment);
                        return true;
                    }
                    else
                    {
                        OutputText("获取触发器信息失败：" + errorMsg, OutputType.Comment);
                        return false;
                    }

                }
                else
                {
                    OutputText("打开数据库失败(" + Helper.DbName + ")", OutputType.Comment);
                    return false;
                }
            }
        }

        public string GetInfoByFile(string dirName, string fileName, out Dictionary<string, Trigger> list)
        {
            list = new Dictionary<string, Trigger>();
            try
            {
                list = GetInfo<Trigger>(dirName, fileName);
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public bool CompareAndShow(Dictionary<string, Trigger> oldItems, Dictionary<string, Trigger> newItems, Setting setting, out string errorString)
        {
            StringBuilder errorStringBuilder = new StringBuilder();

            // 找出新版本中删除的表
            List<string> dropTableNames = new List<string>();
            foreach (string tableName in oldItems.Keys)
            {
                if (!newItems.Keys.Contains(tableName))
                    dropTableNames.Add(tableName);
            }
            //OutputText("\n", OutputType.None);
            //OutputText("\n", OutputType.None);
            if (dropTableNames.Count > 0)
            {
                OutputText("==============================================\n", OutputType.Comment);
                OutputText(string.Format("新版本数据库中删除以下触发器：{0}\n", CombineString(dropTableNames, ",")), OutputType.Comment);
            }
            // 找出新版本中新增的表
            List<string> addTableNames = new List<string>();
            foreach (string tableName in newItems.Keys)
            {
                if (!oldItems.Keys.Contains(tableName))
                    addTableNames.Add(tableName);
            }
            if (addTableNames.Count > 0)
            {
                OutputText("==============================================\n", OutputType.Comment);
                OutputText(string.Format("新版本数据库中新增以下触发器：{0}\n", CombineString(addTableNames, ",")), OutputType.Comment);
            }
            // 对两版本中均存在的表格进行对比
            foreach (string tableName in newItems.Keys)
            {
                if (oldItems.Keys.Contains(tableName))
                {
                    AppendLine("----------------------------------------------\n", OutputType.Comment);
                    AppendLine(string.Format("触发器：{0}\n", tableName), OutputType.Comment);
                    Trigger newTableInfo = newItems[tableName];
                    Trigger oldTableInfo = oldItems[tableName];

                    // 对比表校对集
                    if (!newTableInfo.CharSet.Equals(oldTableInfo.CharSet))
                    {
                        AppendLine(string.Format("  校对集：\"{0}\" => \"{1}\"\n", newTableInfo.CharSet, oldTableInfo.CharSet), OutputType.Comment);
                    }
                    if (!newTableInfo.ClientCharSet.Equals(oldTableInfo.ClientCharSet))
                    {
                        AppendLine(string.Format("  校对集（Client）：\"{0}\" => \"{1}\"\n", newTableInfo.ClientCharSet, oldTableInfo.ClientCharSet), OutputType.Comment);
                    }
                    if (!newTableInfo.SQLMode.Equals(oldTableInfo.SQLMode))
                    {
                        AppendLine(string.Format("  SQLModel：\"{0}\" => \"{1}\"\n", newTableInfo.SQLMode, oldTableInfo.SQLMode), OutputType.Comment);
                    }
                    //避免DEFINER 和注释产生影响
                    string oldSql = oldTableInfo.Statement;
                    string newSql = oldTableInfo.Statement;
                    if (!oldSql.Equals(newSql))
                    {
                        AppendLine("  触发器内容有变化\n", OutputType.Comment);
                    }
                    AppendLineToCtrl();
                }
            }

            errorString = errorStringBuilder.ToString();
            return string.IsNullOrWhiteSpace(errorString);
        }

    }
    public class ProcCompareAndShowResultHelper : FuncCompareAndShowResultHelper
    {

        public ProcCompareAndShowResultHelper()
        {
            base.isFun = true;
        }
    }
    public class FuncCompareAndShowResultHelper : CompareAndShowResultHelperBase, CompareAndShowResultHelper<Function>
    {

        protected bool isFun = true;

        public bool GetInfoByDb(string connStr,out Dictionary<string, Function> rel)
        {
            List<Function> tempList;
            rel = new Dictionary<string, Function>();
            string errorMsg;
            using (Helper=new MySqlOptionHelper(connStr))
            {
                Helper.Set_DbHander(SetLen);
                if (Helper.Open())
                {
                    OutputText("开始获取旧数据库函数结构(" + Helper.DbName + ")\n", OutputType.Comment);
                    bool tempBool = true;
                    string spName2 = "";
                    if (isFun)
                    {
                        spName2 = "函数";
                        tempBool = Helper.GetFuncs(out tempList, out errorMsg);
                    }
                    else
                    {
                        spName2 = "存储过程";
                        tempBool = Helper.GetProcs(out tempList, out errorMsg);
                    }
                    if (tempBool)
                    {
                        OutputText("  获取到 " + tempList.Count + " 个"+ spName2 + "\n", OutputType.Comment);
                        OutputText("...", OutputType.Comment);
                        foreach (var item in tempList)
                        {
                            if(isFun)
                                item.Info = Helper.GetFuncInfo(item.Name);
                            else
                                item.Info = Helper.GetProcInfo(item.Name);
                            rel.Add(item.Name, item);
                        }
                        OutputText("\n", OutputType.None);
                        OutputText("\n", OutputType.Comment);
                        return true;
                    }
                    else
                    {
                        OutputText("获取" + spName2 + "信息失败：" + errorMsg, OutputType.Comment);
                        return false;
                    }

                }
                else
                {
                    OutputText("打开数据库失败(" + Helper.DbName + ")", OutputType.Comment);
                    return false;
                }
            }
        }
        public string GetInfoByFile(string dirName,string fileName,out Dictionary<string,Function> list)
        {
            list = new Dictionary<string, Function>();
            try
            {
                list = GetInfo<Function>(dirName, fileName);
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        public bool CompareAndShow(Dictionary<string, Function> oldItems, Dictionary<string, Function> newItems, Setting setting, out string errorString)
        {
            StringBuilder errorStringBuilder = new StringBuilder();
            string temp = isFun ? "函数" : "存储过程";

            // 找出新版本中删除的表
            List<string> dropTableNames = new List<string>();
            foreach (string tableName in oldItems.Keys)
            {
                if (!newItems.Keys.Contains(tableName))
                    dropTableNames.Add(tableName);
            }
            //OutputText("\n", OutputType.None);
            //OutputText("\n", OutputType.None);
            if (dropTableNames.Count > 0)
            {
                OutputText("==============================================\n", OutputType.Comment);
                OutputText(string.Format("新版本数据库中删除以下" + temp + "：{0}\n", CombineString(dropTableNames, ",")), OutputType.Comment);
            }
            // 找出新版本中新增的表
            List<string> addTableNames = new List<string>();
            foreach (string tableName in newItems.Keys)
            {
                if (!oldItems.Keys.Contains(tableName))
                    addTableNames.Add(tableName);
            }
            if (addTableNames.Count > 0)
            {
                OutputText("==============================================\n", OutputType.Comment);
                OutputText(string.Format("新版本数据库中新增以下" + temp + "：{0}\n", CombineString(addTableNames, ",")), OutputType.Comment);
            }
            // 对两版本中均存在的表格进行对比
            foreach (string tableName in newItems.Keys)
            {
                if (oldItems.Keys.Contains(tableName))
                {
                    AppendLine("----------------------------------------------\n", OutputType.Comment);
                    AppendLine(string.Format(temp + "：{0}\n", tableName), OutputType.Comment);
                    Function newTableInfo = newItems[tableName];
                    Function oldTableInfo = oldItems[tableName];

                    if (setting.CheckCommon && !string.Equals(newTableInfo.Comment, oldTableInfo.Comment))
                    {
                        AppendLine(string.Format("  注释：\"{0}\" => \"{1}\"\n", newTableInfo.Comment, oldTableInfo.Comment), OutputType.Comment);
                    }
                    // 对比表校对集
                    if (!newTableInfo.CharSet.Equals(oldTableInfo.CharSet))
                    {
                        AppendLine(string.Format("  校对集：\"{0}\" => \"{1}\"\n", newTableInfo.CharSet, oldTableInfo.CharSet), OutputType.Comment);
                    }
                    if (!newTableInfo.ClientCharSet.Equals(oldTableInfo.ClientCharSet))
                    {
                        AppendLine(string.Format("  校对集（Client）：\"{0}\" => \"{1}\"\n", newTableInfo.ClientCharSet, oldTableInfo.ClientCharSet), OutputType.Comment);
                    }
                    if (!newTableInfo.Info.SQLModel.Equals(oldTableInfo.Info.SQLModel))
                    {
                        AppendLine(string.Format("  SQLModel：\"{0}\" => \"{1}\"\n", newTableInfo.Info.SQLModel, oldTableInfo.Info.SQLModel), OutputType.Comment);
                    }
                    //避免DEFINER 和注释产生影响
                    string oldSql = oldTableInfo.Info.CreateSQL.Substring(oldTableInfo.Info.CreateSQL.IndexOf("`" + oldTableInfo.Name + "`")).Replace("COMMENT '" + oldTableInfo.Comment + "'", "COMMENT '" + newTableInfo.Comment + "'");
                    string newSql = oldTableInfo.Info.CreateSQL.Substring(oldTableInfo.Info.CreateSQL.IndexOf("`" + newTableInfo.Name + "`"));
                    if (!oldSql.Equals(newSql))
                    {
                        AppendLine("  " + temp + "内容有变化\n", OutputType.Comment);
                    }
                    AppendLineToCtrl();
                }
            }

            errorString = errorStringBuilder.ToString();
            return string.IsNullOrWhiteSpace(errorString);
        }

    }




    public class CompareAndShowResultHelperFactory
    {
        /// <summary>
        /// 获取帮助类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public dynamic GetHelper(DBObjType type)
        {
            dynamic rel;
            switch (type)
            {
                case DBObjType.Table:
                    rel = new TableCompareAndShowResultHelper();
                    break;
                case DBObjType.View:
                    rel = new ViewCompareAndShowResultHelper();
                    break;
                case DBObjType.Trig:
                    rel = new TrigCompareAndShowResultHelper();
                    break;
                case DBObjType.Proc:
                    rel = new ProcCompareAndShowResultHelper();
                    break;
                case DBObjType.Func:
                    rel = new FuncCompareAndShowResultHelper();
                    break;
                default:
                    throw new Exception("未知的（DBObjType）");
            }
            return rel;
        }


        public string getName(DBObjType type)
        {
            switch (type)
            {
                case DBObjType.Table:
                    return "表结构";
                case DBObjType.View:
                    return "视图";
                    break;
                case DBObjType.Trig:
                    return "触发器";
                    break;
                case DBObjType.Proc:
                    return "存储过程";
                    break;
                case DBObjType.Func:
                    return "函数";
                    break;
                default:
                    throw new Exception("未知的（DBObjType）");
            }
        }

        public string GetFileName(DBObjType type, PathSetting cs)
        {
            switch (type)
            {
                case DBObjType.Table:
                    return cs.Tables;
                case DBObjType.View:
                    return cs.Views;
                    break;
                case DBObjType.Trig:
                    return cs.Trigs;
                    break;
                case DBObjType.Proc:
                    return cs.Procs;
                    break;
                case DBObjType.Func:
                    return cs.Funcs;
                    break;
                default:
                    throw new Exception("未知的（DBObjType）");
            }
        }

    }
}
