using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class ViewCompareAndShowResultHelper : CompareAndShowResultHelperBase, ICompareAndShowResult
    {

        public override bool GetInfoByDb(string connStr, ref DbModels rel)
        {
            rel.Views = new Dictionary<string, ViewInfo>();
            using (Helper = new DBStructureHelper(connStr))
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
                            rel.Views.Add(item, Helper.GetViewInfo(item));
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

        public override string GetInfoByFile(string dirName, string fileName, ref DbModels list)
        {
            try
            {
                list.Views = new Dictionary<string, ViewInfo>();
                foreach (var item in Tools.GetInfo<ViewInfo>(dirName, fileName))
                {
                    list.Views.Add(item.Name, item);
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

            Dictionary<string, ViewInfo> oldItems = oldModel.Views;
            Dictionary<string, ViewInfo> newItems = newModel.Views;



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
                //OutputText("==============================================\n", OutputType.Comment);
                Output("==============================================\n", OutputType.Comment, setting, SqlType.Common);
                //OutputText(string.Format("新版本数据库中删除以下视图：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment);
                Output(string.Format("新版本数据库中删除以下视图：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment, setting, SqlType.Common);
                foreach (var viewName in dropTableNames)
                {
                    string dropViewSql = dHelper.GetDropViewSql(viewName);
                    //OutputText(dropViewSql, OutputType.Sql);
                    Output(dropViewSql, OutputType.Sql, setting, SqlType.Delete);
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
                //OutputText("==============================================\n", OutputType.Comment);
                Output("==============================================\n", OutputType.Comment, setting, SqlType.Common);
                //OutputText(string.Format("新版本数据库中新增以下视图：{0}\n", JoinString(addTableNames, ",")), OutputType.Comment);
                Output(string.Format("新版本数据库中新增以下视图：{0}\n", JoinString(addTableNames, ",")), OutputType.Comment, setting, SqlType.Common);
                foreach (var viewName in addTableNames)
                {
                    //OutputText(string.Format("生成创建{0}视图的SQL\n", viewName), OutputType.Comment);
                    Output(string.Format("生成创建{0}视图的SQL\n", viewName), OutputType.Comment, setting, SqlType.Common);
                    string addViewSql = dHelper.GetAddViewSql(newItems[viewName].CreateSQL);
                    AppendLine(addViewSql, OutputType.Sql);
                    //OutputText("\n", OutputType.None);
                    Output("\n", OutputType.None, setting, SqlType.Common);
                }
            }
            // 对两版本中均存在的表格进行对比
            foreach (string tableName in newItems.Keys)
            {
                if (oldItems.Keys.Contains(tableName))
                {
                    //AppendLine("----------------------------------------------\n", OutputType.Comment);
                    Output("----------------------------------------------\n", OutputType.Comment, setting, SqlType.Common);
                    //AppendLine(string.Format("视图：{0}\n", tableName), OutputType.Comment);
                    Output(string.Format("视图：{0}\n", tableName), OutputType.Comment, setting, SqlType.Common);
                    ViewInfo newTableInfo = newItems[tableName];
                    ViewInfo oldTableInfo = oldItems[tableName];


                    // 对比表校对集
                    if (!newTableInfo.CharSet.Equals(oldTableInfo.CharSet))
                    {
                        //AppendLine(string.Format("  校对集：\"{0}\" => \"{1}\"\n", oldTableInfo.CharSet, newTableInfo.CharSet), OutputType.Comment);
                        Output(string.Format("  校对集：\"{0}\" => \"{1}\"\n", oldTableInfo.CharSet, newTableInfo.CharSet), OutputType.Comment, setting, SqlType.Common);
                    }
                    if (!newTableInfo.ClientCharSet.Equals(oldTableInfo.ClientCharSet))
                    {
                        //AppendLine(string.Format("  校对集（Client）：\"{0}\" => \"{1}\"\n", oldTableInfo.ClientCharSet, newTableInfo.ClientCharSet), OutputType.Comment);
                        Output(string.Format("  校对集（Client）：\"{0}\" => \"{1}\"\n", oldTableInfo.ClientCharSet, newTableInfo.ClientCharSet), OutputType.Comment, setting, SqlType.Common);
                    }
                    //避免DEFINER 和注释产生影响
                    string oldSql = oldTableInfo.CreateSQL;//.Substring(oldTableInfo.CreateSQL.IndexOf("`" + oldTableInfo.Name + "`"));
                    string newSql = newTableInfo.CreateSQL;//.Substring(oldTableInfo.CreateSQL.IndexOf("`" + newTableInfo.Name + "`"));


                    string flagTypeStr = "VIEW";

                    //忽略DEFINER 以及 SQL SECURITY
                    if (oldSql.IndexOf("CREATE ") > -1 && oldSql.IndexOf(" DEFINER=") > -1 && oldSql.IndexOf(flagTypeStr) > 0)
                        oldSql = "CREATE " + oldSql.Substring(oldSql.IndexOf(flagTypeStr));
                    if (newSql.IndexOf("CREATE ") > -1 && newSql.IndexOf(" DEFINER=") > -1 && newSql.IndexOf(flagTypeStr) > 0)
                        newSql = "CREATE " + newSql.Substring(newSql.IndexOf(flagTypeStr));

                    if (!oldSql.Equals(newSql))
                    {
                        //AppendLine("  内容有变化\n", OutputType.Comment);
                        Output("  内容有变化\n", OutputType.Comment, setting, SqlType.Common);
                        //OutputText(string.Format("生成创建{0}视图的SQL\n", tableName), OutputType.Comment);
                        Output(string.Format("生成修改{0}视图的SQL\n", tableName), OutputType.Comment, setting, SqlType.Common);
                        string addViewSql = dHelper.GetEditViewSql(newItems[tableName].CreateSQL);
                        AppendLine(addViewSql, OutputType.Sql);
                    }

                }
            }

            AppendLineToCtrl(setting.OutputComment ? 3 : 1);
            errorString = errorStringBuilder.ToString();
            return string.IsNullOrWhiteSpace(errorString);
        }
    }
   
}
