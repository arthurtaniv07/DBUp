using DBUp_Mysql.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    
    public class TableCompareAndShowResultHelper : CompareAndShowResultHelperBase, ICompareAndShowResult
    {
        private List<DataTableInfo> DataDiffSetting = null;


        public TableCompareAndShowResultHelper() { }


        public TableCompareAndShowResultHelper(List<DataTableInfo> dataDiffSetting)
        {
            DataDiffSetting = dataDiffSetting;
        }

        public override bool GetInfoByDb(string connStr, ref DbModels rel)
        {
            rel.Tables = new Dictionary<string, TableInfo>();
            using (Helper = new DBStructureHelper(connStr))
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
                            rel.Tables.Add(tabName, Helper.GetTableInfo(tabName));
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

        public override string GetInfoByFile(string dirName, string fileName, ref DbModels list)
        {
            try
            {
                list.Tables = new Dictionary<string, TableInfo>();
                foreach (var item in Tools.GetInfo<TableInfo>(dirName, fileName))
                {
                    list.Tables.Add(item.Name, item);
                }
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public override bool CompareAndShow(ref DbModels oldModel, ref DbModels newModel, Setting setting, out string errorString)
        {

            Dictionary<string, TableInfo> oldItems = oldModel.Tables;
            Dictionary<string, TableInfo> newItems = newModel.Tables;

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
                Output("==============================================\n", OutputType.Comment, setting, SqlType.Common);
                Output(string.Format("新版本数据库中删除以下表格：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment, setting, SqlType.Common);
                foreach (string tableName in dropTableNames)
                {
                    Output(string.Format("生成删除{0}表的SQL\n", tableName), OutputType.Comment, setting, SqlType.Common);
                    string dropTableSql = dHelper.GetDropTableSql( tableName);
                    Output(dropTableSql, OutputType.Sql, setting, SqlType.Delete);
                    Output("\n", OutputType.None, setting, SqlType.Common);
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
                Output("==============================================\n", OutputType.Comment, setting, SqlType.Common);

                foreach (string tableName in addTableNames)
                {
                    //OutputText(string.Format("生成创建{0}表的SQL\n", tableName), OutputType.Comment);
                    Output(string.Format("生成创建{0}表的SQL\n", tableName), OutputType.Comment, setting, SqlType.Common);
                    // 通过MySQL提供的功能得到建表SQL
                    var tabInfo = newItems[tableName];
                    string createTableSql = tabInfo.CreateSql;
                    AppendLine(createTableSql, OutputType.Sql, SqlType.Create);
                    Output("\n", OutputType.None, setting, SqlType.Common);
                }

            }
            // 对两版本中均存在的表格进行对比
            foreach (string tableName in newItems.Keys)
            {
                if (oldItems.Keys.Contains(tableName))
                {
                    Output("----------------------------------------------\n", OutputType.Comment, setting, SqlType.Common);
                    Output(string.Format("表：{0}\n", tableName), OutputType.Comment, setting, SqlType.Common);
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
                        Output(string.Format("  新版本中删除以下列：{0}\n", JoinString(dropColumnNames, ",")), OutputType.Comment, setting, SqlType.Common);
                        foreach (string columnName in dropColumnNames)
                        {
                            string dropColumnSql = dHelper.GetDropTableColumnSql(tableName, columnName);
                            Output(dropColumnSql, OutputType.Sql, setting, SqlType.Delete);
                        }
                    }



                    // 找出新增列
                    List<string> addColumnNames = new List<string>();
                    foreach (string columnName in newTableInfo.AllColumnInfo.Keys)
                    {
                        if (!oldTableInfo.AllColumnInfo.ContainsKey(columnName))
                            addColumnNames.Add(columnName);
                    }
                    List<string> oldCols = oldTableInfo.TableNames;
                    List<string> newCols = newTableInfo.TableNames;
                    foreach (var item in dropColumnNames)
                        if (oldCols.Contains(item))
                            oldCols.Remove(item);
                    foreach (var item in addColumnNames)
                        if (!oldCols.Contains(item))
                            oldCols.Add(item);
                    FieldSortedOption<string> fieldSortedOption = new FieldSortedOption<string>();
                    fieldSortedOption.NewList = newCols;
                    fieldSortedOption.OldList = oldCols;

                    var sortingOption = new SortingOption<string>();
                    sortingOption.GetSortedOption(ref fieldSortedOption);
                    List<string> sortFielded = new List<string>();
                    SortedOption<string> tempSort;
                    if (addColumnNames.Count > 0)
                    {
                        Output(string.Format("  新版本中新增以下列：{0}\n", JoinString(addColumnNames, ",")), OutputType.Comment, setting, SqlType.Common);
                        foreach (string columnName in addColumnNames)
                        {
                            bool isChecked = sortingOption.IsChecked(columnName, out tempSort);
                            string offset = null;
                            string fieldName = null;
                            if (isChecked)
                            {
                                sortFielded.Add(columnName);
                                offset = tempSort.OptionType.ToString();
                                fieldName = tempSort.NewValue;
                            }
                            string addColumnSql = dHelper.GetAddTableColumnSql(tableName, newTableInfo.AllColumnInfo[columnName], offset, fieldName);
                            AppendLine(addColumnSql, OutputType.Sql, SqlType.Create);
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
                            if (!setting.CheckCommon)
                                isCommentSame = true;
                            bool isNotEmptySame = newColumnInfo.IsNotEmpty == oldColumnInfo.IsNotEmpty;
                            bool isAutoIncrementSame = newColumnInfo.IsAutoIncrement == oldColumnInfo.IsAutoIncrement;
                            bool isDefaultValueSame = newColumnInfo.DefaultValue.Equals(oldColumnInfo.DefaultValue);
                            if (isDataTypeSame == false || isCommentSame == false || isNotEmptySame == false || isAutoIncrementSame == false || isDefaultValueSame == false)
                            {
                                Output(string.Format("  列：{0}\n", columnName), OutputType.Comment, setting, SqlType.Common);
                                if (isDataTypeSame == false)
                                    Output(string.Format("    属性：数据类型{0} => {1}\n", oldColumnInfo.DataType, newColumnInfo.DataType), OutputType.Comment, setting, SqlType.Common);
                                if (isCommentSame == false)
                                    Output(string.Format("    属性：列注释\"{0}\" => \"{1}\"\n", oldColumnInfo.Comment, newColumnInfo.Comment), OutputType.Comment, setting, SqlType.Common);
                                if (isNotEmptySame == false)
                                    Output(string.Format("    属性：（为空）{0} => {1}\n", oldColumnInfo.IsNotEmpty == true ? "不允许" : "允许", newColumnInfo.IsNotEmpty == true ? "不允许" : "允许"), OutputType.Comment, setting, SqlType.Common);
                                if (isAutoIncrementSame == false)
                                    Output(string.Format("    属性：列设{0}  =>  {1}\n", oldColumnInfo.IsAutoIncrement == true ? "自增" : "不自增", newColumnInfo.IsAutoIncrement == true ? "自增" : "不自增"), OutputType.Comment, setting, SqlType.Common);
                                if (isDefaultValueSame == false)
                                    Output(string.Format("    属性：默认值{0}  =>  {1}\n", oldColumnInfo.DefaultValue, newColumnInfo.DefaultValue), OutputType.Comment, setting, SqlType.Common);

                                bool isChecked = sortingOption.IsChecked(columnName, out tempSort);
                                string offset = null;
                                string fieldName = null;
                                if (isChecked)
                                {
                                    sortFielded.Add(columnName);
                                    offset = tempSort.OptionType.ToString();
                                    fieldName = tempSort.NewValue;
                                }
                                // 根据新的列属性进行修改
                                string changeColumnSql = dHelper.GetChangeTableColumnSql(tableName, newColumnInfo, offset, fieldName);
                                AppendLine(changeColumnSql, OutputType.Sql, SqlType.Alter);
                            }
                        }
                    }
                    if ( fieldSortedOption.Checked)
                    {
                        var sss = fieldSortedOption.Options.Where(i => !sortFielded.Contains(i.OptionValue) && i.OptionType != SortedOptionType.NONE);
                        if (sss.Any())
                        {
                            Output("  新版本数据库字段顺序改变\n", OutputType.Comment, setting, SqlType.Common);
                        }
                        foreach (var item in sss)
                        {
                            string sortFieldSql = dHelper.GetModifySort(tableName, newTableInfo.AllColumnInfo[item.OptionValue], item.OptionType.ToString(), item.NewValue);
                            AppendLine(sortFieldSql, OutputType.Sql, SqlType.Alter);
                        }
                    }

                    //新增列后再修改顺序



                    // 在改变列属性前需先同步索引设置，因为自增属性仅可用于设置了索引的列
                    // 找出主键修改
                    bool isPrimaryKeySame = newTableInfo.PrimaryKeyColumnNames.Count == oldTableInfo.PrimaryKeyColumnNames.Count && newTableInfo.PrimaryKeyColumnNames.Any(i => oldTableInfo.PrimaryKeyColumnNames.Contains(i));
                    //isPrimaryKeySame &= newTableInfo.PrimaryKeyColumnNames.Count > 0 || oldTableInfo.PrimaryKeyColumnNames.Count > 0;

                    if (isPrimaryKeySame == false && (newTableInfo.PrimaryKeyColumnNames.Count > 0 || oldTableInfo.PrimaryKeyColumnNames.Count > 0))
                    {
                        string newPrimaryKeyString = newTableInfo.PrimaryKeyColumnNames.Count > 0 ? JoinString(newTableInfo.PrimaryKeyColumnNames, ",") : "无";
                        string oldPrimaryKeyString = oldTableInfo.PrimaryKeyColumnNames.Count > 0 ? JoinString(oldTableInfo.PrimaryKeyColumnNames, ",") : "无";
                        Output(string.Format("  主键：{0} => {1}\n", oldPrimaryKeyString, newPrimaryKeyString), OutputType.Comment, setting, SqlType.Common);
                        // 先删除原有的主键设置
                        string dropPrimaryKeySql = dHelper.GetDropPrimarySql(tableName);
                        Output(dropPrimaryKeySql, OutputType.Sql, setting, SqlType.Delete);

                        if (newTableInfo.PrimaryKeyColumnNames.Any())
                        {
                            // 再重新设置
                            string addPrimaryKeySql = dHelper.GetAddPrimarySql(tableName, newTableInfo.PrimaryKeyColumnNames);
                            AppendLine(addPrimaryKeySql, OutputType.Sql, SqlType.Alter);
                        }
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
                        Output(string.Format("  新版本中删除以下索引：{0}\n", JoinString(dropIndexNames, ",")), OutputType.Comment, setting, SqlType.Common);
                        foreach (string name in dropIndexNames)
                        {
                            string dropIndexSql = dHelper.GetDropIndexSql(tableName, name);
                            Output(dropIndexSql, OutputType.Sql, setting, SqlType.Delete);
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
                        Output(string.Format("  新版本中新增以下索引：{0}\n", JoinString(addIndexNames.Select(i => i.Name).ToList(), ",")), OutputType.Comment, setting, SqlType.Common);
                        foreach (TableIndex tabInx in addIndexNames)
                        {
                            string addIndexSql = dHelper.GetAddIndexSql(tableName, tabInx);
                            AppendLine(addIndexSql, OutputType.Sql, SqlType.Create);
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
                                Output(string.Format("  名为{0}的索引：\n", name), OutputType.Comment, setting, SqlType.Common);
                                if (isIndexColumnSame == false)
                                    Output(string.Format("    涉及列名：{0} => {1}\n", JoinString(oldIndex.Columns, ","), JoinString(newIndex.Columns, ",")), OutputType.Comment, setting, SqlType.Common);
                                if (newIndex.IndexFunc != oldIndex.IndexFunc)
                                    Output(string.Format("    索引方法：{0} => {1}\n", oldIndex.IndexFunc + "", newIndex.IndexFunc + ""), OutputType.Comment, setting, SqlType.Common);
                                if (newIndex.IndexType != oldIndex.IndexType)
                                    Output(string.Format("    索引类型：{0} => {1}\n", oldIndex.IndexType + "", newIndex.IndexType + ""), OutputType.Comment, setting, SqlType.Common);
                                if (setting.CheckCommon && isIndexComment == false)
                                    Output(string.Format("    注释：{0} => {1}\n", oldIndex.Common, newIndex.Common), OutputType.Comment, setting, SqlType.Common);
                                // 先删除
                                string dropIndexSql = dHelper.GetDropIndexSql(tableName, name);
                                AppendLine(dropIndexSql, OutputType.Sql, SqlType.Alter);
                                // 再重新创建
                                string addIndexSql = dHelper.GetAddIndexSql(tableName, newIndex);
                                AppendLine(addIndexSql, OutputType.Sql, SqlType.Alter);
                            }
                        }
                    }


                    // 对比表校对集
                    if (!newTableInfo.Collation.Equals(oldTableInfo.Collation))
                    {
                        Output(string.Format("  校对集：\"{0}\" => \"{1}\"\n", oldTableInfo.Collation, newTableInfo.Collation), OutputType.Comment, setting, SqlType.Common);
                        string alterTableComment = dHelper.GetChangeCollateSql(tableName, newTableInfo.Collation);
                        AppendLine(alterTableComment, OutputType.Sql, SqlType.Alter);
                    }

                    // 对比表注释
                    if (setting.CheckCommon && !newTableInfo.Comment.Equals(oldTableInfo.Comment))
                    {
                        Output(string.Format("  注释：\"{0}\" => \"{1}\"\n", oldTableInfo.Comment, newTableInfo.Comment), OutputType.Comment, setting, SqlType.Common);
                        string alterTableComment = dHelper.GetChangeCommentSql(tableName, newTableInfo.Comment);
                        AppendLine(alterTableComment, OutputType.Sql, SqlType.Alter);
                    }


                    // 对比表选项
                    string alterTableOption;

                    bool isDiffTableOption = false;
                    if (isDiffTableOption)
                    {
                        if (oldTableInfo.Option.Auto_Increment != newTableInfo.Option.Auto_Increment)
                        {
                            Output(string.Format("  自动增加：{0} => {1}\n", oldTableInfo.Option.Auto_Increment, newTableInfo.Option.Auto_Increment), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.Auto_Increment), newTableInfo.Option.Auto_Increment);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.Avg_Row_Length != newTableInfo.Option.Avg_Row_Length)
                        {
                            Output(string.Format("  平均记录长度：{0} => {1}\n", oldTableInfo.Option.Avg_Row_Length, newTableInfo.Option.Avg_Row_Length), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.Avg_Row_Length), newTableInfo.Option.Avg_Row_Length);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.Checksum != newTableInfo.Option.Checksum)
                        {
                            Output(string.Format("  检查记录和：{0} => {1}\n", oldTableInfo.Option.Checksum, newTableInfo.Option.Checksum), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.Checksum), newTableInfo.Option.Checksum);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.COMPRESSION != newTableInfo.Option.COMPRESSION)
                        {
                            Output(string.Format("  压缩方式：{0} => {1}\n", oldTableInfo.Option.COMPRESSION, newTableInfo.Option.COMPRESSION), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.COMPRESSION), newTableInfo.Option.COMPRESSION);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.ENCRYPTION != newTableInfo.Option.ENCRYPTION)
                        {
                            Output(string.Format("  加密：{0} => {1}\n", oldTableInfo.Option.ENCRYPTION, newTableInfo.Option.ENCRYPTION), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.ENCRYPTION), newTableInfo.Option.ENCRYPTION);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.Engine != newTableInfo.Option.Engine)
                        {
                            Output(string.Format("  引擎：{0} => {1}\n", oldTableInfo.Option.Engine, newTableInfo.Option.Engine), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.Engine), newTableInfo.Option.Engine);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.Max_Rows != newTableInfo.Option.Max_Rows)
                        {
                            Output(string.Format("  最大记录行数：{0} => {1}\n", oldTableInfo.Option.Max_Rows, newTableInfo.Option.Max_Rows), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.Max_Rows), newTableInfo.Option.Max_Rows);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.Min_Rows != newTableInfo.Option.Min_Rows)
                        {
                            Output(string.Format("  最小记录行数：{0} => {1}\n", oldTableInfo.Option.Min_Rows, newTableInfo.Option.Min_Rows), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.Min_Rows), newTableInfo.Option.Min_Rows);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.RowFormat != newTableInfo.Option.RowFormat)
                        {
                            Output(string.Format("  记录格式：{0} => {1}\n", oldTableInfo.Option.RowFormat, newTableInfo.Option.RowFormat), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.RowFormat), newTableInfo.Option.RowFormat);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.STATS_AUTO_RECALC != newTableInfo.Option.STATS_AUTO_RECALC)
                        {
                            Output(string.Format("  累计数据自动重计：{0} => {1}\n", oldTableInfo.Option.STATS_AUTO_RECALC, newTableInfo.Option.STATS_AUTO_RECALC), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.STATS_AUTO_RECALC), newTableInfo.Option.STATS_AUTO_RECALC);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.STATS_PERSISTENT != newTableInfo.Option.STATS_PERSISTENT)
                        {
                            Output(string.Format("  统计数据持久：{0} => {1}\n", oldTableInfo.Option.STATS_PERSISTENT, newTableInfo.Option.STATS_PERSISTENT), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.STATS_PERSISTENT), newTableInfo.Option.STATS_PERSISTENT);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                        if (oldTableInfo.Option.TABLESPACE != newTableInfo.Option.TABLESPACE)
                        {
                            Output(string.Format("  表空间：{0} => {1}\n", oldTableInfo.Option.TABLESPACE, newTableInfo.Option.TABLESPACE), OutputType.Comment, setting, SqlType.Common);
                            alterTableOption = dHelper.GetChangeOptionSql(tableName, nameof(newTableInfo.Option.TABLESPACE), newTableInfo.Option.TABLESPACE);
                            AppendLine(alterTableOption, OutputType.Sql, SqlType.Alter);
                        }

                    }



                    //if (DeleteLastLintText("表："))
                    //    DeleteLastLintText("----------------------------------------------");
                }
            }
            AppendLineToCtrl(setting.OutputComment ? 3 : 1);

            errorString = errorStringBuilder.ToString();
            return string.IsNullOrWhiteSpace(errorString);
        }
    }
    
}
