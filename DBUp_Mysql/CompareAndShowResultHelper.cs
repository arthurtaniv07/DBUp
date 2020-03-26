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
        bool CompareAndShow(Dictionary<string, T> oldItems, Dictionary<string, T> newItems, Setting setting, out string errorString, string newConnStr = null);
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

        public MySqlOptionHelper dHelper = new MySqlOptionHelper();

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
        public string JoinString(IList<string> list, string separateString)
        {
            if (list == null || list.Count < 1)
                return null;
            else
            {
                //StringBuilder builder = new StringBuilder();
                //for (int i = 0; i < list.Count; ++i)
                //    builder.Append(list[i]).Append(separateString);

                //string result = builder.ToString();
                //// 去掉最后多加的一次分隔符
                //if (separateString != null)
                //    return result.Substring(0, result.Length - separateString.Length);
                //else
                //    return result;
                return string.Join(separateString, list);
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

        public bool ShowDbDiff(string oldConnStr, string newConnStr)
        {
            DbModel oldInfo=null;
            DbModel newInfo = null;
            using (Helper = new MySqlOptionHelper(oldConnStr))
            {
                oldInfo = Helper.GetDbInfo();
            }
            using (Helper = new MySqlOptionHelper(newConnStr))
            {
                newInfo = Helper.GetDbInfo();
            }

            if (oldInfo?.SqlMode != newInfo?.SqlMode)
            {
                OutputText?.Invoke("-- 修改mysql sql_mode\n", OutputType.Error);
                OutputText?.Invoke(string.Format("-- set @@global.sql_mode='{0}';\n", newInfo.SqlMode), OutputType.Error);
            }
            return true;
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
                    OutputText?.Invoke("开始获取数据库表结构(" + Helper.Server + (Helper.Port == "-1" ? "" : ":" + Helper.Port) + "  " + Helper.DbName + ")\n", OutputType.Comment);
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
        public bool CompareAndShow(Dictionary<string, TableInfo> oldItems, Dictionary<string, TableInfo> newItems, Setting setting, out string errorString,string newConnStr)
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
                OutputText(string.Format("新版本数据库中删除以下表格：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment);
                foreach (string tableName in dropTableNames)
                {
                    OutputText(string.Format("生成删除{0}表的SQL\n", tableName), OutputType.Comment);
                    string dropTableSql = dHelper.GetDropTableSql( tableName);
                    OutputText(dropTableSql, OutputType.Sql);
                    OutputText("\n", OutputType.None);
                }
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
                OutputText(string.Format("新版本数据库中新增以下表格：{0}\n", JoinString(addTableNames, ",")), OutputType.Comment);
                using (Helper = new MySqlOptionHelper(newConnStr))
                {
                    foreach (string tableName in addTableNames)
                    {
                        OutputText(string.Format("生成创建{0}表的SQL\n", tableName), OutputType.Comment);
                        // 通过MySQL提供的功能得到建表SQL
                        var tabInfo = newItems[tableName];
                        string createTableSql = Helper.GetCreateTableSql(tableName);
                        OutputText(createTableSql, OutputType.Sql);
                        OutputText("\n", OutputType.None);
                    }
                }
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

                    // 进行表结构比较

                    // 找出删除列
                    List<string> dropColumnNames = new List<string>();
                    foreach (string columnName in oldTableInfo.AllColumnInfo.Keys)
                    {
                        if (!newTableInfo.AllColumnInfo.ContainsKey(columnName))
                            dropColumnNames.Add(columnName);
                    }
                    if (dropColumnNames.Count > 0)
                    {
                        AppendLine(string.Format("  新版本中删除以下列：{0}\n", JoinString(dropColumnNames, ",")), OutputType.Comment);
                        foreach (string columnName in dropColumnNames)
                        {
                            string dropColumnSql = dHelper.GetDropTableColumnSql(tableName, columnName);
                            AppendLine(dropColumnSql, OutputType.Sql);
                        }
                    }

                    // 找出新增列
                    List<string> addColumnNames = new List<string>();
                    foreach (string columnName in newTableInfo.AllColumnInfo.Keys)
                    {
                        if (!oldTableInfo.AllColumnInfo.ContainsKey(columnName))
                            addColumnNames.Add(columnName);
                    }
                    if (addColumnNames.Count > 0)
                    {
                        AppendLine(string.Format("  新版本中新增以下列：{0}\n", JoinString(addColumnNames, ",")), OutputType.Comment);
                        foreach (string columnName in addColumnNames)
                        {
                            string addColumnSql = dHelper.GetAddTableColumnSql(tableName, newTableInfo.AllColumnInfo[columnName]);
                            AppendLine(addColumnSql, OutputType.Sql);
                        }
                        //新增列后再修改顺序
                        foreach (var item in newTableInfo.AllColumnInfo)
                        {

                        }
                    }

                    // 在改变列属性前需先同步索引设置，因为自增属性仅可用于设置了索引的列
                    // 找出主键修改
                    bool isPrimaryKeySame = newTableInfo.PrimaryKeyColumnNames.Count == oldTableInfo.PrimaryKeyColumnNames.Count && newTableInfo.PrimaryKeyColumnNames.Any(i => oldTableInfo.PrimaryKeyColumnNames.Contains(i));
                    isPrimaryKeySame &= newTableInfo.PrimaryKeyColumnNames.Count > 0 || oldTableInfo.PrimaryKeyColumnNames.Count > 0;

                    if (isPrimaryKeySame == false)
                    {
                        string newPrimaryKeyString = newTableInfo.PrimaryKeyColumnNames.Count > 0 ? JoinString(newTableInfo.PrimaryKeyColumnNames, ",") : "无";
                        string oldPrimaryKeyString = oldTableInfo.PrimaryKeyColumnNames.Count > 0 ? JoinString(oldTableInfo.PrimaryKeyColumnNames, ",") : "无";
                        AppendLine(string.Format("  主键：{0} => {1}\n", oldPrimaryKeyString, newPrimaryKeyString), OutputType.Comment);
                        // 先删除原有的主键设置
                        string dropPrimaryKeySql = dHelper.GetDropPrimarySql(tableName);
                        AppendLine(dropPrimaryKeySql, OutputType.Sql);
                        // 再重新设置
                        string addPrimaryKeySql = dHelper.GetAddPrimarySql(tableName, newTableInfo.PrimaryKeyColumnNames);
                        AppendLine(addPrimaryKeySql, OutputType.Sql);
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
                        AppendLine(string.Format("  新版本中删除以下索引：{0}\n", JoinString(dropIndexNames, ",")), OutputType.Comment);
                        foreach (string name in dropIndexNames)
                        {
                            string dropIndexSql = dHelper.GetDropIndexSql(tableName, name);
                            AppendLine(dropIndexSql, OutputType.Sql);
                        }
                    }
                    // 找出新版本中新增索引
                    List<TableIndex> addIndexNames = new List<TableIndex>();
                    foreach (string name in newTableInfo.IndexInfo.Keys)
                    {
                        if (!oldTableInfo.IndexInfo.ContainsKey(name))
                            addIndexNames.Add(newTableInfo.IndexInfo[name]);
                    }
                    if (addIndexNames.Count > 0)
                    {
                        AppendLine(string.Format("  新版本中新增以下索引：{0}\n", JoinString(addIndexNames.Select(i => i.Name).ToList(), ",")), OutputType.Comment);
                        foreach (TableIndex tabInx in addIndexNames)
                        {
                            string addIndexSql = dHelper.GetAddIndexSql(tableName, tabInx);
                            AppendLine(addIndexSql, OutputType.Sql);
                        }
                    }
                    // 找出同名索引的变动
                    foreach (var pair in newTableInfo.IndexInfo)
                    {
                        string name = pair.Key;
                        if (oldTableInfo.IndexInfo.ContainsKey(name))
                        {
                            TableIndex newIndex = pair.Value;
                            TableIndex oldIndex = oldTableInfo.IndexInfo[name];

                            bool isIndexColumnSame = newIndex.Columns.Count == oldIndex.Columns.Count && newIndex.Columns.Any(i => oldIndex.Columns.Contains(i));
                            bool isIndexComment = (newIndex.Common ?? "") == (oldIndex.Common ?? "");

                            if (isIndexColumnSame == false || newIndex.IndexFunc != oldIndex.IndexFunc ||
                                newIndex.IndexType != oldIndex.IndexType || isIndexComment == false)
                            {
                                AppendLine(string.Format("  名为{0}的索引：\n", name), OutputType.Comment);
                                if (isIndexColumnSame == false)
                                    AppendLine(string.Format("    涉及列名：{0} => {1}\n", JoinString(oldIndex.Columns, ","), JoinString(newIndex.Columns, ",")), OutputType.Comment);
                                if (newIndex.IndexFunc != oldIndex.IndexFunc)
                                    AppendLine(string.Format("    索引方法：{0} => {1}\n", oldIndex.IndexFunc + "", newIndex.IndexFunc + ""), OutputType.Comment);
                                if (newIndex.IndexType != oldIndex.IndexType)
                                    AppendLine(string.Format("    索引类型：{0} => {1}\n", oldIndex.IndexType + "", newIndex.IndexType + ""), OutputType.Comment);
                                if (setting.CheckCommon && isIndexComment == false)
                                    AppendLine(string.Format("    注释：{0} => {1}\n", oldIndex.Common, newIndex.Common), OutputType.Comment);
                                // 先删除
                                string dropIndexSql = dHelper.GetDropIndexSql(tableName, name);
                                AppendLine(dropIndexSql, OutputType.Sql);
                                // 再重新创建
                                string addIndexSql = dHelper.GetAddIndexSql(tableName, newIndex);
                                AppendLine(addIndexSql, OutputType.Sql);
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
                                AppendLine(string.Format("  列：{0}\n", columnName), OutputType.Comment);
                                if (isDataTypeSame == false)
                                    AppendLine(string.Format("    属性：数据类型{0} => {1}\n", oldColumnInfo.DataType, newColumnInfo.DataType), OutputType.Comment);
                                if (isCommentSame == false)
                                    AppendLine(string.Format("    属性：列注释\"{0}\" => \"{1}\"\n", oldColumnInfo.Comment, newColumnInfo.Comment), OutputType.Comment);
                                if (isNotEmptySame == false)
                                    AppendLine(string.Format("    属性：（为空）{0} => {1}\n", oldColumnInfo.IsNotEmpty == true ? "不允许" : "允许", newColumnInfo.IsNotEmpty == true ? "不允许" : "允许"), OutputType.Comment);
                                if (isAutoIncrementSame == false)
                                    AppendLine(string.Format("    属性：列设{0}  =>  {1}\n", oldColumnInfo.IsAutoIncrement == true ? "自增" : "不自增", newColumnInfo.IsAutoIncrement == true ? "自增" : "不自增"), OutputType.Comment);
                                if (isDefaultValueSame == false)
                                    AppendLine(string.Format("    属性：默认值{0}  =>  {1}\n", oldColumnInfo.DefaultValue, newColumnInfo.DefaultValue), OutputType.Comment);

                                // 根据新的列属性进行修改
                                string changeColumnSql = dHelper.GetChangeTableColumnSql(tableName, newColumnInfo);
                                AppendLine(changeColumnSql, OutputType.Sql);
                            }
                        }
                    }

                    // 对比表校对集
                    if (!newTableInfo.Collation.Equals(oldTableInfo.Collation))
                    {
                        AppendLine(string.Format("  校对集：\"{0}\" => \"{1}\"\n", oldTableInfo.Collation, newTableInfo.Collation), OutputType.Comment);
                        string alterTableComment = dHelper.GetChangeCollateSql(tableName, oldTableInfo.Collation);
                        AppendLine(alterTableComment, OutputType.Sql);
                    }

                    // 对比表注释
                    if (setting.CheckCommon && !newTableInfo.Comment.Equals(oldTableInfo.Comment))
                    {
                        AppendLine(string.Format("  注释：\"{0}\" => \"{1}\"\n", oldTableInfo.Comment, newTableInfo.Comment), OutputType.Comment);
                        string alterTableComment = dHelper.GetChangeCommentSql(tableName, newTableInfo.Comment);
                        AppendLine(alterTableComment, OutputType.Sql);
                    }


                    // 对比表选项
                    string alterTableOption;

                    if (oldTableInfo.Option.Auto_Increment != newTableInfo.Option.Auto_Increment)
                    {
                        AppendLine(string.Format("  自动增加：{0} => {1}\n", oldTableInfo.Option.Auto_Increment, newTableInfo.Option.Auto_Increment), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.Auto_Increment), oldTableInfo.Option.Auto_Increment);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.Avg_Row_Length != newTableInfo.Option.Avg_Row_Length)
                    {
                        AppendLine(string.Format("  平均记录长度：{0} => {1}\n", oldTableInfo.Option.Avg_Row_Length, newTableInfo.Option.Avg_Row_Length), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.Avg_Row_Length), oldTableInfo.Option.Avg_Row_Length);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.Checksum != newTableInfo.Option.Checksum)
                    {
                        AppendLine(string.Format("  检查记录和：{0} => {1}\n", oldTableInfo.Option.Checksum, newTableInfo.Option.Checksum), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.Checksum), oldTableInfo.Option.Checksum);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.COMPRESSION != newTableInfo.Option.COMPRESSION)
                    {
                        AppendLine(string.Format("  压缩方式：{0} => {1}\n", oldTableInfo.Option.COMPRESSION, newTableInfo.Option.COMPRESSION), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.COMPRESSION), oldTableInfo.Option.COMPRESSION);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.ENCRYPTION != newTableInfo.Option.ENCRYPTION)
                    {
                        AppendLine(string.Format("  加密：{0} => {1}\n", oldTableInfo.Option.ENCRYPTION, newTableInfo.Option.ENCRYPTION), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.ENCRYPTION), oldTableInfo.Option.ENCRYPTION);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.Engine != newTableInfo.Option.Engine)
                    {
                        AppendLine(string.Format("  引擎：{0} => {1}\n", oldTableInfo.Option.Engine, newTableInfo.Option.Engine), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.Engine), oldTableInfo.Option.Engine);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.Max_Rows != newTableInfo.Option.Max_Rows)
                    {
                        AppendLine(string.Format("  最大记录行数：{0} => {1}\n", oldTableInfo.Option.Max_Rows, newTableInfo.Option.Max_Rows), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.Max_Rows), oldTableInfo.Option.Max_Rows);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.Min_Rows != newTableInfo.Option.Min_Rows)
                    {
                        AppendLine(string.Format("  最小记录行数：{0} => {1}\n", oldTableInfo.Option.Min_Rows, newTableInfo.Option.Min_Rows), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.Min_Rows), oldTableInfo.Option.Min_Rows);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.RowFormat != newTableInfo.Option.RowFormat)
                    {
                        AppendLine(string.Format("  记录格式：{0} => {1}\n", oldTableInfo.Option.RowFormat, newTableInfo.Option.RowFormat), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.RowFormat), oldTableInfo.Option.RowFormat);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.STATS_AUTO_RECALC != newTableInfo.Option.STATS_AUTO_RECALC)
                    {
                        AppendLine(string.Format("  累计数据自动重计：{0} => {1}\n", oldTableInfo.Option.STATS_AUTO_RECALC, newTableInfo.Option.STATS_AUTO_RECALC), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.STATS_AUTO_RECALC), oldTableInfo.Option.STATS_AUTO_RECALC);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.STATS_PERSISTENT != newTableInfo.Option.STATS_PERSISTENT)
                    {
                        AppendLine(string.Format("  统计数据持久：{0} => {1}\n", oldTableInfo.Option.STATS_PERSISTENT, newTableInfo.Option.STATS_PERSISTENT), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.STATS_PERSISTENT), oldTableInfo.Option.STATS_PERSISTENT);
                        AppendLine(alterTableOption, OutputType.Sql);
                    }

                    if (oldTableInfo.Option.TABLESPACE != newTableInfo.Option.TABLESPACE)
                    {
                        AppendLine(string.Format("  表空间：{0} => {1}\n", oldTableInfo.Option.TABLESPACE, newTableInfo.Option.TABLESPACE), OutputType.Comment);
                        alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(oldTableInfo.Option.TABLESPACE), oldTableInfo.Option.TABLESPACE);
                        AppendLine(alterTableOption, OutputType.Sql);
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
                    OutputText("开始获取数据库视图结构(" + Helper.Server + (Helper.Port == "-1" ? "" : ":" + Helper.Port) + "  " + Helper.DbName + ")\n", OutputType.Comment);
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
        public bool CompareAndShow(Dictionary<string, ViewInfo> oldItems, Dictionary<string, ViewInfo> newItems, Setting setting, out string errorString, string newConnStr = null)
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
                OutputText(string.Format("新版本数据库中删除以下视图：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment);
                foreach (var viewName in dropTableNames)
                {
                    string dropViewSql = dHelper.GetDropViewSql(viewName);
                    OutputText(dropViewSql, OutputType.Sql);
                }
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
                OutputText(string.Format("新版本数据库中新增以下视图：{0}\n", JoinString(addTableNames, ",")), OutputType.Comment);
                foreach (var viewName in addTableNames)
                {
                    OutputText(string.Format("生成创建{0}视图的SQL\n", viewName), OutputType.Comment);
                    string addViewSql = dHelper.GetAddViewSql(newItems[viewName].CreateSQL);
                    OutputText(addViewSql, OutputType.Sql);
                    OutputText("\n", OutputType.None);
                }
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
                        AppendLine(string.Format("  校对集：\"{0}\" => \"{1}\"\n", oldTableInfo.CharSet, newTableInfo.CharSet), OutputType.Comment);
                    }
                    if (!newTableInfo.ClientCharSet.Equals(oldTableInfo.ClientCharSet))
                    {
                        AppendLine(string.Format("  校对集（Client）：\"{0}\" => \"{1}\"\n", oldTableInfo.ClientCharSet, newTableInfo.ClientCharSet), OutputType.Comment);
                    }
                    //避免DEFINER 和注释产生影响
                    string oldSql = oldTableInfo.CreateSQL;//.Substring(oldTableInfo.CreateSQL.IndexOf("`" + oldTableInfo.Name + "`"));
                    string newSql = newTableInfo.CreateSQL;//.Substring(oldTableInfo.CreateSQL.IndexOf("`" + newTableInfo.Name + "`"));
                    if (!oldSql.Equals(newSql))
                    {
                        AppendLine("  内容有变化\n", OutputType.Comment);
                        string dropViewSql = dHelper.GetDropViewSql(tableName);
                        OutputText(dropViewSql, OutputType.Sql);
                        OutputText(string.Format("生成创建{0}视图的SQL\n", tableName), OutputType.Comment);
                        string addViewSql = dHelper.GetAddViewSql(newItems[tableName].CreateSQL);
                        OutputText(addViewSql, OutputType.Sql);
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
                    OutputText("开始获取数据库触发器结构(" + Helper.Server + (Helper.Port == "-1" ? "" : ":" + Helper.Port) + "  " + Helper.DbName + ")\n", OutputType.Comment);
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
        public bool CompareAndShow(Dictionary<string, Trigger> oldItems, Dictionary<string, Trigger> newItems, Setting setting, out string errorString, string newConnStr = null)
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
                OutputText(string.Format("新版本数据库中删除以下触发器：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment);
                foreach (var viewName in dropTableNames)
                {
                    string dropViewSql = dHelper.GetDropTrisSql(viewName);
                    OutputText(dropViewSql, OutputType.Sql);
                }
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
                OutputText(string.Format("新版本数据库中新增以下触发器：{0}\n", JoinString(addTableNames, ",")), OutputType.Comment);
                foreach (var viewName in addTableNames)
                {
                    OutputText(string.Format("生成创建{0}触发器的SQL\n", viewName), OutputType.Comment);
                    string addViewSql = dHelper.GetAddTrisSql(newItems[viewName]);
                    OutputText(addViewSql, OutputType.Sql);
                    OutputText("\n", OutputType.None);
                }
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
                        AppendLine(string.Format("  校对集：\"{0}\" => \"{1}\"\n", oldTableInfo.CharSet, newTableInfo.CharSet), OutputType.Comment);
                    }
                    if (!newTableInfo.ClientCharSet.Equals(oldTableInfo.ClientCharSet))
                    {
                        AppendLine(string.Format("  校对集（Client）：\"{0}\" => \"{1}\"\n", oldTableInfo.ClientCharSet, newTableInfo.ClientCharSet), OutputType.Comment);
                    }
                    //if (!newTableInfo.SQLMode.Equals(oldTableInfo.SQLMode))
                    //{
                    //    AppendLine(string.Format("  SQLModel：\"{0}\" => \"{1}\"\n", oldTableInfo.SQLMode, newTableInfo.SQLMode), OutputType.Comment);
                    //}
                    //避免DEFINER 和注释产生影响
                    string oldSql = oldTableInfo.Statement;
                    string newSql = newTableInfo.Statement;
                    if (!oldSql.Equals(newSql))
                    {
                        AppendLine("  触发器内容有变化\n", OutputType.Comment);
                        string dropViewSql = dHelper.GetDropTrisSql(tableName);
                        OutputText(dropViewSql, OutputType.Sql);
                        OutputText(string.Format("生成创建{0}触发器的SQL\n", tableName), OutputType.Comment);
                        string addViewSql = dHelper.GetAddTrisSql(newItems[tableName]);
                        OutputText(addViewSql, OutputType.Sql);
                        OutputText("\n", OutputType.None);
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
            base.isFun = false;
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
                    OutputText("开始获取数据库函数结构(" + Helper.Server + (Helper.Port == "-1" ? "" : ":" + Helper.Port) + "  " + Helper.DbName + ")\n", OutputType.Comment);
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


        public bool CompareAndShow(Dictionary<string, Function> oldItems, Dictionary<string, Function> newItems, Setting setting, out string errorString, string newConnStr = null)
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
                OutputText(string.Format("新版本数据库中删除以下" + temp + "：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment);
                if (isFun)
                {
                    foreach (var viewName in dropTableNames)
                    {
                        string dropViewSql = dHelper.GetDropFuncSql(viewName);
                        OutputText(dropViewSql, OutputType.Sql);
                    }
                }
                else
                {
                    foreach (var viewName in dropTableNames)
                    {
                        string dropViewSql = dHelper.GetDropProcsSql(viewName);
                        OutputText(dropViewSql, OutputType.Sql);
                    }
                }
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
                OutputText(string.Format("新版本数据库中新增以下" + temp + "：{0}\n", JoinString(addTableNames, ",")), OutputType.Comment);
                if (isFun)
                {
                    foreach (var viewName in addTableNames)
                    {
                        OutputText(string.Format("生成创建{0}{1}的SQL\n", viewName, temp), OutputType.Comment);
                        string addViewSql = dHelper.GetAddFuncSql(newItems[viewName]);
                        OutputText(addViewSql, OutputType.Sql);
                        OutputText("\n", OutputType.None);
                    }
                }
                else
                {
                    foreach (var viewName in addTableNames)
                    {
                        OutputText(string.Format("生成创建{0}{1}的SQL\n", viewName, temp), OutputType.Comment);
                        string addViewSql = dHelper.GetAddProcsSql(newItems[viewName]);
                        OutputText(addViewSql, OutputType.Sql);
                        OutputText("\n", OutputType.None);
                    }
                }
            }
            // 对两版本中均存在的表格进行对比
            foreach (string tableName in newItems.Keys)
            {
                Function newTableInfo = newItems[tableName];
                if (oldItems.Keys.Contains(tableName))
                {
                    AppendLine("----------------------------------------------\n", OutputType.Comment);
                    AppendLine(string.Format(temp + "：{0}\n", tableName), OutputType.Comment);
                    Function oldTableInfo = oldItems[tableName];

                    if (setting.CheckCommon && !string.Equals(newTableInfo.Comment, oldTableInfo.Comment))
                    {
                        AppendLine(string.Format("  注释：\"{0}\" => \"{1}\"\n", oldTableInfo.Comment, newTableInfo.Comment), OutputType.Comment);
                    }
                    // 对比表校对集
                    if (!newTableInfo.CharSet.Equals(oldTableInfo.CharSet))
                    {
                        AppendLine(string.Format("  校对集：\"{0}\" => \"{1}\"\n", oldTableInfo.CharSet, newTableInfo.CharSet), OutputType.Comment);
                    }
                    if (!newTableInfo.ClientCharSet.Equals(oldTableInfo.ClientCharSet))
                    {
                        AppendLine(string.Format("  校对集（Client）：\"{0}\" => \"{1}\"\n", oldTableInfo.ClientCharSet, newTableInfo.ClientCharSet), OutputType.Comment);
                    }
                    //if (!newTableInfo.Info.SQLModel.Equals(oldTableInfo.Info.SQLModel))
                    //{
                    //    AppendLine(string.Format("  SQLModel：\"{0}\" => \"{1}\"\n", oldTableInfo.Info.SQLModel, newTableInfo.Info.SQLModel), OutputType.Comment);
                    //}
                    //避免DEFINER 和注释产生影响
                    var flagTypeStr = isFun ? "FUNCTION" : "PROCEDURE";
                    string oldSql = oldTableInfo.Info.CreateSQL;
                    if (oldSql.IndexOf("CREATE DEFINER=") > -1 && oldSql.IndexOf(flagTypeStr) > 0)
                    {
                        oldSql = "CREATE " + oldSql.Substring(oldSql.IndexOf(flagTypeStr));
                    }
                    string newSql = newTableInfo.Info.CreateSQL;
                    if (newSql.IndexOf("CREATE DEFINER=") > -1 && newSql.IndexOf(flagTypeStr) > 0)
                    {
                        newSql = "CREATE " + newSql.Substring(newSql.IndexOf(flagTypeStr));
                    }
                    if (!oldSql.Equals(newSql))
                    {
                        AppendLine("  " + temp + "内容有变化\n", OutputType.Comment);
                        //删除后创建
                        if (isFun)
                        {
                            string dropViewSql = dHelper.GetDropFuncSql(tableName);
                            OutputText(dropViewSql, OutputType.Sql);
                            OutputText(string.Format("生成创建{0}{1}的SQL\n", tableName, temp), OutputType.Comment);
                            string addViewSql = dHelper.GetAddFuncSql(newItems[tableName]);
                            OutputText(addViewSql, OutputType.Sql);
                            OutputText("\n", OutputType.None);
                        }
                        else
                        {
                            string dropViewSql = dHelper.GetDropProcsSql(tableName);
                            OutputText(dropViewSql, OutputType.Sql);
                            OutputText(string.Format("生成创建{0}{1}的SQL\n", tableName, temp), OutputType.Comment);
                            string addViewSql = dHelper.GetAddProcsSql(newItems[tableName]);
                            OutputText(addViewSql, OutputType.Sql);
                            OutputText("\n", OutputType.None);
                        }
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
                case DBObjType.Trig:
                    return "触发器";
                case DBObjType.Proc:
                    return "存储过程";
                case DBObjType.Func:
                    return "函数";
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
                case DBObjType.Trig:
                    return cs.Trigs;
                case DBObjType.Proc:
                    return cs.Procs;
                case DBObjType.Func:
                    return cs.Funcs;
                default:
                    throw new Exception("未知的（DBObjType）");
            }
        }

    }
}
