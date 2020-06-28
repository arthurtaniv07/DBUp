using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class TrigCompareAndShowResultHelper : CompareAndShowResultHelperBase, ICompareAndShowResult
    {
        public override bool GetInfoByDb(string connStr, ref DbModels rel)
        {
            rel.Triggers = new Dictionary<string, Trigger>();
            using (Helper = new DBStructureHelper(connStr))
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
                            rel.Triggers.Add(item.Name, item);
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

        public override string GetInfoByFile(string dirName, string fileName, ref DbModels list)
        {
            try
            {
                list.Triggers = new Dictionary<string, Trigger>();
                foreach (var item in Tools.GetInfo<Trigger>(dirName, fileName))
                {
                    list.Triggers.Add(item.Name, item);
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

            Dictionary<string, Trigger> oldItems = oldModel.Triggers;
            Dictionary<string, Trigger> newItems = newModel.Triggers;


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
                //OutputText(string.Format("新版本数据库中删除以下触发器：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment);
                Output(string.Format("新版本数据库中删除以下触发器：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment, setting, SqlType.Common);
                foreach (var viewName in dropTableNames)
                {
                    string dropViewSql = dHelper.GetDropTrisSql(viewName);
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
                //OutputText(string.Format("新版本数据库中新增以下触发器：{0}\n", JoinString(addTableNames, ",")), OutputType.Comment);
                Output(string.Format("新版本数据库中新增以下触发器：{0}\n", JoinString(addTableNames, ",")), OutputType.Comment, setting, SqlType.Common);
                foreach (var viewName in addTableNames)
                {
                    //OutputText(string.Format("生成创建{0}触发器的SQL\n", viewName), OutputType.Comment);
                    Output(string.Format("生成创建{0}触发器的SQL\n", viewName), OutputType.Comment, setting, SqlType.Common);
                    string addViewSql = dHelper.GetAddTrisSql(newItems[viewName]);
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
                    //AppendLine(string.Format("触发器：{0}\n", tableName), OutputType.Comment);
                    Output(string.Format("触发器：{0}\n", tableName), OutputType.Comment, setting, SqlType.Common);
                    Trigger newTableInfo = newItems[tableName];
                    Trigger oldTableInfo = oldItems[tableName];

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
                    //if (!newTableInfo.SQLMode.Equals(oldTableInfo.SQLMode))
                    //{
                    //    AppendLine(string.Format("  SQLModel：\"{0}\" => \"{1}\"\n", oldTableInfo.SQLMode, newTableInfo.SQLMode), OutputType.Comment);
                    //}
                    //避免DEFINER 和注释产生影响
                    string oldSql = oldTableInfo.Statement;
                    string newSql = newTableInfo.Statement;
                    if (!oldSql.Equals(newSql))
                    {
                        //AppendLine("  触发器内容有变化\n", OutputType.Comment);
                        Output("  触发器内容有变化\n", OutputType.Comment, setting, SqlType.Common);
                        string dropViewSql = dHelper.GetDropTrisSql(tableName);
                        AppendLine(dropViewSql, OutputType.Sql);
                        //OutputText(string.Format("生成创建{0}触发器的SQL\n", tableName), OutputType.Comment);
                        Output(string.Format("生成创建{0}触发器的SQL\n", tableName), OutputType.Comment, setting, SqlType.Common);
                        string addViewSql = dHelper.GetAddTrisSql(newItems[tableName]);
                        AppendLine(addViewSql, OutputType.Sql);
                        //OutputText("\n", OutputType.None);
                        Output("\n", OutputType.None, setting, SqlType.Common);
                    }
                }
            }

            AppendLineToCtrl(setting.OutputComment ? 3 : 1);
            errorString = errorStringBuilder.ToString();
            return string.IsNullOrWhiteSpace(errorString);
        }

    }
   
}
