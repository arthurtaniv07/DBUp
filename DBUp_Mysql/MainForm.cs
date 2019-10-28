using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBUp_Mysql
{
   
    public partial class MainForm : Form
    {
        public class Setting
        {
            /// <summary>
            /// 比较注释
            /// </summary>
            public bool CheckCommon { get; set; }
        }
        string oldConn = System.Configuration.ConfigurationManager.ConnectionStrings["old"].ConnectionString;
        string newConn = System.Configuration.ConfigurationManager.ConnectionStrings["new"].ConnectionString;
        public MainForm()
        {
            InitializeComponent();
        }
        private void btnCompare_Click(object sender, EventArgs e)
        {
            Start();
        }
        MySqlOptionHelper oldhelper = null;
        MySqlOptionHelper helper = null;
        private void Start()
        {
            RtxResult.Clear();
            Dictionary<string, TableInfo> oldTabs = new Dictionary<string, TableInfo>();
            List<string> tabNames = new List<string>();
            string errorMsg = "";
            using ( oldhelper = new MySqlOptionHelper(oldConn))
            {
                if (oldhelper.Open())
                {
                    if (oldhelper.GetTables(out tabNames, out errorMsg))
                    {
                        foreach (string tabName in tabNames)
                        {
                            oldTabs.Add(tabName, oldhelper.GetTableInfo(tabName));
                        }
                    }
                    else
                    {
                        AppendOutputText("获取表信息失败", OutputType.Comment);
                        return;
                    }
                }
            }

            Dictionary<string, TableInfo> newTabs = new Dictionary<string, TableInfo>();
            tabNames = new List<string>();
            errorMsg = "";
            using (MySqlOptionHelper helper = new MySqlOptionHelper(newConn))
            {
                if (helper.Open())
                {
                    if (helper.GetTables(out tabNames, out errorMsg))
                    {
                        foreach (string tabName in tabNames)
                        {
                            newTabs.Add(tabName, helper.GetTableInfo(tabName));
                        }
                    }
                    else
                    {
                        AppendOutputText("获取表信息失败", OutputType.Comment);
                        return;
                    }
                }
            }
            Setting cs = new Setting();
            cs.CheckCommon = this.cheComm.Checked;
            CompareAndShowResult(oldTabs, newTabs,cs, out string errorString);

            if (!string.IsNullOrEmpty(errorString))
            {
                string tips = string.Concat("对比中发现以下问题，请修正后重新进行比较：\n\n", errorString);
                MessageBox.Show(tips, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                AppendOutputText("\n", OutputType.Comment);
                AppendOutputText("对比完毕", OutputType.Comment);
            }
        }

        /// <summary>
        /// 对新旧两版本数据库进行对比，并展示结果以及生成的SQL
        /// </summary>
        public void CompareAndShowResult(Dictionary<string, TableInfo> oldTabs, Dictionary<string, TableInfo> newTabs, Setting cs, out string errorString)
        {
            StringBuilder errorStringBuilder = new StringBuilder();

            // 找出新版本中删除的表
            List<string> dropTableNames = new List<string>();
            foreach (string tableName in oldTabs.Keys)
            {
                if (!newTabs.Keys.Contains(tableName))
                    dropTableNames.Add(tableName);
            }
            if (dropTableNames.Count > 0)
            {
                AppendOutputText("==============================================\n", OutputType.Comment);
                AppendOutputText(string.Format("新版本数据库中删除以下表格：{0}\n", CombineString(dropTableNames, ",")), OutputType.Comment);
                //foreach (string tableName in dropTableNames)
                //{
                //    AppendOutputText(string.Format("生成删除{0}表的SQL\n", tableName), OutputType.Comment);
                //    if (AppValues.AllTableCompareRule.ContainsKey(tableName) && AppValues.AllTableCompareRule[tableName].CompareWay == TableCompareWays.Ignore)
                //    {
                //        AppendOutputText("该表格配置为忽略比较，故不进行删除\n", OutputType.Warning);
                //        continue;
                //    }
                //    string dropTableSql = _GetDropTableSql(AppValues.OldSchemaName, tableName);
                //    AppendOutputText(dropTableSql, OutputType.Sql);
                //    AppendOutputText("\n", OutputType.None);
                //}
                AppendOutputText("\n", OutputType.None);
                AppendOutputText("\n", OutputType.None);
            }
            // 找出新版本中新增的表
            List<string> addTableNames = new List<string>();
            foreach (string tableName in newTabs.Keys)
            {
                if (!oldTabs.Keys.Contains(tableName))
                    addTableNames.Add(tableName);
            }
            if (addTableNames.Count > 0)
            {
                AppendOutputText("==============================================\n", OutputType.Comment);
                AppendOutputText(string.Format("新版本数据库中新增以下表格：{0}\n", CombineString(addTableNames, ",")), OutputType.Comment);
                //foreach (string tableName in addTableNames)
                //{
                //    AppendOutputText(string.Format("生成创建{0}表及填充数据的SQL\n", tableName), OutputType.Comment);
                //    if (AppValues.AllTableCompareRule.ContainsKey(tableName) && AppValues.AllTableCompareRule[tableName].CompareWay == TableCompareWays.Ignore)
                //    {
                //        AppendOutputText("该表格配置为忽略比较，故不进行新建\n", OutputType.Warning);
                //        continue;
                //    }
                //    // 通过MySQL提供的功能得到建表SQL
                //    string createTableSql = _GetCreateTableSql(AppValues.NewSchemaName, tableName, AppValues.OldSchemaName, AppValues.NewConn);
                //    AppendOutputText(createTableSql, OutputType.Sql);
                //    AppendOutputText("\n", OutputType.None);
                //    // 得到填充数据的SQL
                //    DataTable data = _SelectData(AppValues.NewSchemaName, tableName, "*", AppValues.NewConn);
                //    string fillDataSql = _GetFillDataSql(AppValues.NewTableInfo[tableName], data, AppValues.OldSchemaName);
                //    if (!string.IsNullOrEmpty(fillDataSql))
                //    {
                //        AppendOutputText(fillDataSql, OutputType.Sql);
                //        AppendOutputText("\n", OutputType.None);
                //    }
                //}
                AppendOutputText("\n", OutputType.None);
                AppendOutputText("\n", OutputType.None);
            }
            // 对两版本中均存在的表格进行对比
            foreach (string tableName in newTabs.Keys)
            {
                if (oldTabs.Keys.Contains(tableName))
                {

                    AppendOutputText("----------------------------------------------\n", OutputType.Comment);
                    AppendOutputText(string.Format("表：{0}\n", tableName), OutputType.Comment);
                    TableInfo newTableInfo = newTabs[tableName];
                    TableInfo oldTableInfo = oldTabs[tableName];

                    //TableCompareRule compareRule = null;
                    //if (AppValues.AllTableCompareRule.ContainsKey(tableName))
                    //{
                    //    compareRule = AppValues.AllTableCompareRule[tableName];
                    //    using (StringReader reader = new StringReader(compareRule.GetCompareRuleComment()))
                    //    {
                    //        string line = null;
                    //        while ((line = reader.ReadLine()) != null)
                    //        {
                    //            AppendOutputText(line, OutputType.Comment);
                    //            AppendOutputText("\n", OutputType.None);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    compareRule = new TableCompareRule();
                    //    compareRule.CompareWay = TableCompareWays.ColumnInfoAndData;
                    //    AppendOutputText("未设置对该表格进行对比的方式，将默认对比表结构及数据\n", OutputType.Warning);
                    //}

                    // 进行表结构比较
                    const string SPLIT_STRING = ",\n";
                    bool isPrimaryKeySame = true;
                    //if (compareRule.CompareWay != TableCompareWays.Ignore)
                    {
                        //AppendOutputText("开始进行结构对比\n", OutputType.Comment);
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
                            if (hasOutputPrefix == false)
                            {
                                AppendOutputText(alterTableSqlPrefix, OutputType.Sql);
                                AppendOutputText("\n", OutputType.None);
                                hasOutputPrefix = true;
                            }
                            // 如果之前对比出差异并进行了输出，需要先为上一条语句添加逗号结尾
                            if (hasOutputPartFirstSql == true)
                            {
                                AppendOutputText(SPLIT_STRING, OutputType.Sql);
                                hasOutputPartFirstSql = false;
                            }
                            AppendOutputText(string.Format("新版本中删除以下列：{0}\n", CombineString(dropColumnNames, ",")), OutputType.Comment);
                            //foreach (string columnName in dropColumnNames)
                            //{
                            //    if (hasOutputPartFirstSql == false)
                            //        hasOutputPartFirstSql = true;
                            //    else
                            //        AppendOutputText(SPLIT_STRING, OutputType.Sql);

                            //    string dropColumnSql = string.Format(_DROP_COLUMN_SQL, columnName);
                            //    AppendOutputText(dropColumnSql, OutputType.Sql);
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
                            if (hasOutputPrefix == false)
                            {
                                AppendOutputText(alterTableSqlPrefix, OutputType.Sql);
                                AppendOutputText("\n", OutputType.None);
                                hasOutputPrefix = true;
                            }
                            if (hasOutputPartFirstSql == true)
                            {
                                AppendOutputText(SPLIT_STRING, OutputType.Sql);
                                hasOutputPartFirstSql = false;
                            }
                            AppendOutputText(string.Format("新版本中新增以下列：{0}\n", CombineString(addColumnNames, ",")), OutputType.Comment);
                            //foreach (string columnName in addColumnNames)
                            //{
                            //    if (hasOutputPartFirstSql == false)
                            //        hasOutputPartFirstSql = true;
                            //    else
                            //        AppendOutputText(SPLIT_STRING, OutputType.Sql);

                            //    // 根据新增列属性生成添加新列的SQL
                            //    ColumnInfo columnInfo = newTableInfo.AllColumnInfo[columnName];
                            //    string notEmptyString = columnInfo.IsNotEmpty == true ? "NOT NULL" : "NULL";
                            //    // 注意如果列设为NOT NULL，就不允许设置默认值为NULL
                            //    string defaultValue = columnInfo.DefaultValue.Equals("NULL") ? string.Empty : string.Concat(" DEFAULT ", columnInfo.DefaultValue);
                            //    string addColumnSql = string.Format(_ADD_COLUMN_SQL, columnName, columnInfo.DataType, notEmptyString, defaultValue, columnInfo.Comment);
                            //    AppendOutputText(addColumnSql, OutputType.Sql);
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
                            if (hasOutputPrefix == false)
                            {
                                AppendOutputText(alterTableSqlPrefix, OutputType.Sql);
                                AppendOutputText("\n", OutputType.None);
                                hasOutputPrefix = true;
                            }
                            if (hasOutputPartFirstSql == true)
                            {
                                AppendOutputText(SPLIT_STRING, OutputType.Sql);
                                hasOutputPartFirstSql = false;
                            }
                            string newPrimaryKeyString = newTableInfo.PrimaryKeyColumnNames.Count > 0 ? CombineString(newTableInfo.PrimaryKeyColumnNames, ",") : "无";
                            string oldPrimaryKeyString = oldTableInfo.PrimaryKeyColumnNames.Count > 0 ? CombineString(oldTableInfo.PrimaryKeyColumnNames, ",") : "无";
                            AppendOutputText(string.Format("  新版本中主键为：{0}，而旧版本中为：{1}\n", newPrimaryKeyString, oldPrimaryKeyString), OutputType.Comment);
                            //// 先删除原有的主键设置
                            //AppendOutputText(_DROP_PRIMARY_KEY_SQL, OutputType.Sql);
                            //AppendOutputText(SPLIT_STRING, OutputType.Sql);
                            //// 再重新设置
                            //List<string> primaryKeyDefine = new List<string>();
                            //foreach (string primaryKey in newTableInfo.PrimaryKeyColumnNames)
                            //    primaryKeyDefine.Add(string.Format("`{0}`", primaryKey));

                            //string addPrimaryKeySql = string.Format(_ADD_PRIMARY_KEY_SQL, CombineString(primaryKeyDefine, ","));
                            //AppendOutputText(addPrimaryKeySql, OutputType.Sql);
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
                            if (hasOutputPrefix == false)
                            {
                                AppendOutputText(alterTableSqlPrefix, OutputType.Sql);
                                AppendOutputText("\n", OutputType.None);
                                hasOutputPrefix = true;
                            }
                            if (hasOutputPartFirstSql == true)
                            {
                                AppendOutputText(SPLIT_STRING, OutputType.Sql);
                                hasOutputPartFirstSql = false;
                            }
                            AppendOutputText(string.Format("  新版本中删除以下索引：{0}\n", CombineString(dropIndexNames, ",")), OutputType.Comment);
                            foreach (string name in dropIndexNames)
                            {
                                if (hasOutputPartFirstSql == false)
                                    hasOutputPartFirstSql = true;
                                else
                                    AppendOutputText(SPLIT_STRING, OutputType.Sql);

                                //string dropIndexSql = string.Format(_DROP_INDEX_SQL, name);
                                //AppendOutputText(dropIndexSql, OutputType.Sql);
                            }
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
                            if (hasOutputPrefix == false)
                            {
                                AppendOutputText(alterTableSqlPrefix, OutputType.Sql);
                                AppendOutputText("\n", OutputType.None);
                                hasOutputPrefix = true;
                            }
                            if (hasOutputPartFirstSql == true)
                            {
                                AppendOutputText(SPLIT_STRING, OutputType.Sql);
                                hasOutputPartFirstSql = false;
                            }
                            AppendOutputText(string.Format("新版本中新增以下索引：{0}\n", CombineString(addIndexNames, ",")), OutputType.Comment);
                            foreach (string name in addIndexNames)
                            {
                                if (hasOutputPartFirstSql == false)
                                    hasOutputPartFirstSql = true;
                                else
                                    AppendOutputText(SPLIT_STRING, OutputType.Sql);

                                // 根据新增索引属性生成添加新索引的SQL
                                // 注意列名后必须声明排序方式，MySQL只支持索引的升序排列
                                List<string> columnDefine = new List<string>();
                                foreach (string columnName in newTableInfo.IndexInfo[name])
                                    columnDefine.Add(string.Format("`{0}` ASC", columnName));

                                //string addIndexSql = string.Format(_ADD_UNIQUE_INDEX_SQL, name, CombineString(columnDefine, ","));
                                //AppendOutputText(addIndexSql, OutputType.Sql);
                            }
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
                                    if (hasOutputPrefix == false)
                                    {
                                        AppendOutputText(alterTableSqlPrefix, OutputType.Sql);
                                        AppendOutputText("\n", OutputType.None);
                                        hasOutputPrefix = true;
                                    }
                                    if (hasOutputPartFirstSql == true)
                                    {
                                        AppendOutputText(SPLIT_STRING, OutputType.Sql);
                                        hasOutputPartFirstSql = false;
                                    }
                                    AppendOutputText(string.Format("  新版本中名为{0}的索引，涉及的列名为{1}，而旧版本中为{2}\n", name, CombineString(newIndexColumnInfo, ","), CombineString(oldIndexColumnInfo, ",")), OutputType.Comment);
                                    //// 先删除
                                    //string dropIndexSql = string.Format(_DROP_INDEX_SQL, name);
                                    //AppendOutputText(dropIndexSql, OutputType.Sql);
                                    //AppendOutputText(SPLIT_STRING, OutputType.Sql);
                                    //// 再重新创建
                                    //List<string> columnDefine = new List<string>();
                                    //foreach (string columnName in newIndexColumnInfo)
                                    //    columnDefine.Add(string.Format("`{0}` ASC", columnName));

                                    //string addIndexSql = string.Format(_ADD_UNIQUE_INDEX_SQL, name, CombineString(columnDefine, ","));
                                    //AppendOutputText(addIndexSql, OutputType.Sql);
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
                                if (!cs.CheckCommon) isCommentSame = true;
                                bool isNotEmptySame = newColumnInfo.IsNotEmpty == oldColumnInfo.IsNotEmpty;
                                bool isAutoIncrementSame = newColumnInfo.IsAutoIncrement == oldColumnInfo.IsAutoIncrement;
                                bool isDefaultValueSame = newColumnInfo.DefaultValue.Equals(oldColumnInfo.DefaultValue);
                                if (isDataTypeSame == false || isCommentSame == false || isNotEmptySame == false || isAutoIncrementSame == false || isDefaultValueSame == false)
                                {
                                    if (hasOutputPrefix == false)
                                    {
                                        AppendOutputText(alterTableSqlPrefix, OutputType.Sql);
                                        AppendOutputText("\n", OutputType.None);
                                        hasOutputPrefix = true;
                                    }
                                    if (hasOutputPartFirstSql == true)
                                    {
                                        AppendOutputText(SPLIT_STRING, OutputType.Sql);
                                        hasOutputPartFirstSql = false;
                                    }
                                    AppendOutputText(string.Format("  列：{0}\n", columnName), OutputType.Comment);
                                    if (isDataTypeSame == false)
                                        AppendOutputText(string.Format("    属性：数据类型{0} => {1}\n", newColumnInfo.DataType, oldColumnInfo.DataType), OutputType.Comment);
                                    if (isCommentSame == false)
                                        AppendOutputText(string.Format("    属性：列注释\"{0}\" => \"{1}\"\n", newColumnInfo.Comment, oldColumnInfo.Comment), OutputType.Comment);
                                    if (isNotEmptySame == false)
                                        AppendOutputText(string.Format("    属性：{0} => {1}\n", newColumnInfo.IsNotEmpty == true ? "不允许" : "允许", oldColumnInfo.IsNotEmpty == true ? "不允许" : "允许"), OutputType.Comment);
                                    if (isAutoIncrementSame == false)
                                        AppendOutputText(string.Format("    属性：列设{0}  =>  {1}\n", newColumnInfo.IsAutoIncrement == true ? "自增" : "不自增", oldColumnInfo.IsAutoIncrement == true ? "自增" : "不自增"), OutputType.Comment);
                                    if (isDefaultValueSame == false)
                                        AppendOutputText(string.Format("    属性：默认值{0}  =>  {1}\n", newColumnInfo.DefaultValue, oldColumnInfo.DefaultValue), OutputType.Comment);

                                    //// 根据新的列属性进行修改
                                    //string notEmptyString = newColumnInfo.IsNotEmpty == true ? "NOT NULL" : "NULL";
                                    //string defaultValue = newColumnInfo.DefaultValue.Equals("NULL") ? string.Empty : string.Concat(" DEFAULT ", newColumnInfo.DefaultValue);
                                    //string changeColumnSql = string.Format(_CHANGE_COLUMN_SQL, columnName, newColumnInfo.DataType, notEmptyString, defaultValue, newColumnInfo.Comment);
                                    //AppendOutputText(changeColumnSql, OutputType.Sql);
                                    //hasOutputPartFirstSql = true;
                                }
                            }
                        }

                        // 对比表校对集
                        if (!newTableInfo.Collation.Equals(oldTableInfo.Collation))
                        {
                            if (hasOutputPrefix == false)
                            {
                                AppendOutputText(alterTableSqlPrefix, OutputType.Sql);
                                AppendOutputText("\n", OutputType.None);
                                hasOutputPrefix = true;
                            }
                            if (hasOutputPartFirstSql == true)
                            {
                                AppendOutputText(SPLIT_STRING, OutputType.Sql);
                                hasOutputPartFirstSql = false;
                            }
                            AppendOutputText(string.Format("  校对集：\"{0}\" => \"{1}\"\n", newTableInfo.Collation, oldTableInfo.Collation), OutputType.Comment);
                            //string alterTableComment = string.Format(_ALTER_TABLE_COLLATION_SQL, newTableInfo.Collation);
                            //AppendOutputText(alterTableComment, OutputType.Sql);
                            //hasOutputPartFirstSql = true;
                        }

                        // 对比表注释
                        if (cs.CheckCommon && !newTableInfo.Comment.Equals(oldTableInfo.Comment))
                        {
                            if (hasOutputPrefix == false)
                            {
                                AppendOutputText(alterTableSqlPrefix, OutputType.Sql);
                                AppendOutputText("\n", OutputType.None);
                                hasOutputPrefix = true;
                            }
                            if (hasOutputPartFirstSql == true)
                            {
                                AppendOutputText(SPLIT_STRING, OutputType.Sql);
                                hasOutputPartFirstSql = false;
                            }
                            AppendOutputText(string.Format("  注释：\"{0}\" => \"{1}\"\n", newTableInfo.Comment, oldTableInfo.Comment), OutputType.Comment);
                            //string alterTableComment = string.Format(_ALTER_TABLE_COMMENT_SQL, newTableInfo.Comment);
                            //AppendOutputText(alterTableComment, OutputType.Sql);
                            //hasOutputPartFirstSql = true;
                        }

                        // 最后添加分号结束
                        if (hasOutputPartFirstSql == true)
                        {
                            AppendOutputText(";\n", OutputType.Sql);
                            hasOutputPrefix = false;
                            hasOutputPartFirstSql = false;
                        }

                        //// 进行表数据比较
                        //if (compareRule.CompareWay == TableCompareWays.ColumnInfoAndData)
                        //{
                        //    AppendOutputText("开始进行数据对比\n", OutputType.Comment);

                        //    // 检查表格是否设置了主键，本工具生成的同步数据的SQL需要通过主键确定数据行
                        //    if (newTableInfo.PrimaryKeyColumnNames.Count == 0)
                        //    {
                        //        string tips = string.Format("错误：表格\"{0}\"未设置主键，本工具无法通过主键生成定位并更新数据的SQL，请设置主键后重试\n本次操作被迫中止\n", tableName);
                        //        AppendOutputText(tips, OutputType.Error);
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
                        //                AppendOutputText(tips, OutputType.Error);
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
                        //        AppendOutputText(tips, OutputType.Error);
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
                        //            AppendOutputText(string.Concat("新版本中删除了主键列为以下值的一行：", primaryKeyColumnNameAndValueString, "\n"), OutputType.Comment);
                        //            // 判断该行数据是否被设为忽略
                        //            //if (_IsIgnoreData(compareRule.CompareIgnoreData, dataRow) == true)
                        //            //    AppendOutputText("该行符合配置的需忽略的数据行，故不进行删除\n", OutputType.Warning);
                        //            //else
                        //            //{
                        //            string dropDataSql = string.Format(_DROP_DATA_SQL, _CombineDatabaseTableFullName(AppValues.OldSchemaName, tableName), _GetColumnNameAndValueString(dataRow, newTableInfo.PrimaryKeyColumnNames, " AND "));
                        //            AppendOutputText(dropDataSql, OutputType.Sql);
                        //            AppendOutputText("\n", OutputType.None);
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
                        //            AppendOutputText(string.Concat("新版本中新增主键列为以下值的一行：", primaryKeyColumnNameAndValueString, "\n"), OutputType.Comment);
                        //            //// 判断该行数据是否被设为忽略
                        //            //if (_IsIgnoreData(compareRule.CompareIgnoreData, dataRow) == true)
                        //            //    AppendOutputText("该行符合配置的需忽略的数据行，故不进行新增\n", OutputType.Warning);
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
                        //            AppendOutputText(insertDataSql, OutputType.Sql);
                        //            AppendOutputText("\n", OutputType.None);
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
                        //                AppendOutputText(string.Format("主键为{0}的行中，新版本中以下数据为{1}，而旧版本中为{2}\n", primaryKeyColumnNameAndValueString, newColumnNameAndValueString, oldColumnNameAndValueString), OutputType.Comment);
                        //                // 判断该行数据是否被设为忽略
                        //                if (_IsIgnoreData(compareRule.CompareIgnoreData, newDataRow) == true)
                        //                    AppendOutputText("该行符合配置的需忽略的数据行，故不进行修改\n", OutputType.Warning);
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
                        //                    AppendOutputText(updateDataSql, OutputType.Sql);
                        //                    AppendOutputText("\n", OutputType.None);
                        //                }
                        //            }

                        //            // 新版表格中新增列的值需要同步到旧表，无视用户是否设置为忽略列
                        //            if (addColumnNames.Count > 0)
                        //            {
                        //                string addColumnNameAndValueString = _GetColumnNameAndValueString(newDataRow, addColumnNames, ", ");
                        //                AppendOutputText(string.Format("为新版本中新增的{0}列填充数据\n", addColumnNameString), OutputType.Comment);
                        //                string updateDataSql = string.Format(_UPDATE_DATA_SQL, _CombineDatabaseTableFullName(AppValues.OldSchemaName, tableName), addColumnNameAndValueString, primaryKeyColumnNameAndValueString);
                        //                AppendOutputText(updateDataSql, OutputType.Sql);
                        //                AppendOutputText("\n", OutputType.None);
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
            }

            errorString = errorStringBuilder.ToString();
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
        public void AppendOutputText(string text, OutputType type)
        {
            Color color= Color.Black;
            if (type == OutputType.Comment)
                color = Color.DarkGray;
            else if (type == OutputType.Warning)
                color = Color.Orange;
            else if (type == OutputType.Error)
                color = Color.Red;
            else if (type == OutputType.Sql)
                color = Color.Black;
            //else
            //    color = _lastTextColor;

            RtxResult.SelectionColor = color;
            //_lastTextColor = color;

            if (type != OutputType.Sql && type != OutputType.None)
                RtxResult.AppendText("-- ");

            RtxResult.AppendText(text);
            RtxResult.Focus();
        }
        public enum OutputType
        {
            None,
            Comment,
            Warning,
            Error,
            Sql,
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (oldhelper != null)
            {
                oldhelper.Close();
            }
            if (helper != null)
            {
                helper.Close();
            }
        }
    }

}
