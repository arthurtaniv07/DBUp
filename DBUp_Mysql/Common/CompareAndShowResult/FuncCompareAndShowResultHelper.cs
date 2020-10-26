using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class FuncCompareAndShowResultHelper : CompareAndShowResultHelperBase, ICompareAndShowResult
    {

        protected bool isFun = true;

        public override bool GetInfoByDb(string connStr, ref DbModels rel)
        {
            List<Function> tempList;
            string errorMsg;
            using (Helper=new DBStructureHelper(connStr))
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
                        if (isFun)
                        {
                            rel.Functions = new Dictionary<string, Function>();
                            foreach (var item in tempList)
                            {
                                item.Info = Helper.GetFuncInfo(item.Name);
                                rel.Functions.Add(item.Name, item);
                            }
                        }
                        else
                        {
                            rel.Procs = new Dictionary<string, Function>();
                            foreach (var item in tempList)
                            {
                                item.Info = Helper.GetProcInfo(item.Name);
                                rel.Procs.Add(item.Name, item);
                            }
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

        public override string GetInfoByFile(string dirName,string fileName, ref DbModels list)
        {
            try
            {
                if (isFun)
                {
                    list.Functions = new Dictionary<string, Function>();
                    foreach (var item in Tools.GetInfo<Function>(dirName, fileName))
                    {
                        list.Functions.Add(item.Name, item);
                    }
                }
                else
                {
                    list.Procs = new Dictionary<string, Function>();
                    foreach (var item in Tools.GetInfo<Function>(dirName, fileName))
                    {
                        list.Procs.Add(item.Name, item);
                    }
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

            Dictionary<string, Function> oldItems = isFun? oldModel.Functions : oldModel.Procs;
            Dictionary<string, Function> newItems = isFun ? newModel.Functions : newModel.Procs;

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
                //OutputText("==============================================\n", OutputType.Comment);
                Output("==============================================\n", OutputType.Comment, setting, SqlType.Common);
                //OutputText(string.Format("新版本数据库中删除以下" + temp + "：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment);
                Output(string.Format("新版本数据库中删除以下" + temp + "：{0}\n", JoinString(dropTableNames, ",")), OutputType.Comment, setting, SqlType.Common);
                if (isFun)
                {
                    foreach (var viewName in dropTableNames)
                    {
                        string dropViewSql = dHelper.GetDropFuncSql(viewName);
                        //OutputText(dropViewSql, OutputType.Sql);
                        Output(dropViewSql, OutputType.Sql, setting, SqlType.Delete);
                    }
                }
                else
                {
                    foreach (var viewName in dropTableNames)
                    {
                        string dropViewSql = dHelper.GetDropProcsSql(viewName);
                        //OutputText(dropViewSql, OutputType.Sql);
                        Output(dropViewSql, OutputType.Sql, setting, SqlType.Delete);
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
                //OutputText("==============================================\n", OutputType.Comment);
                Output("==============================================\n", OutputType.Comment, setting, SqlType.Common);
                //OutputText(string.Format("新版本数据库中新增以下" + temp + "：{0}\n", JoinString(addTableNames, ",")), OutputType.Comment);
                Output(string.Format("新版本数据库中新增以下" + temp + "：{0}\n", JoinString(addTableNames, ",")), OutputType.Comment, setting, SqlType.Common);
                if (isFun)
                {
                    foreach (var viewName in addTableNames)
                    {
                        //OutputText(string.Format("生成创建{0}{1}的SQL\n", viewName, temp), OutputType.Comment);
                        Output(string.Format("生成创建{0}{1}的SQL\n", viewName, temp), OutputType.Comment, setting, SqlType.Common);
                        string addViewSql = dHelper.GetAddFuncSql(newItems[viewName]);
                        AppendLine(addViewSql, OutputType.Sql, SqlType.Create);
                        //OutputText("\n", OutputType.None);
                        Output("\n", OutputType.None, setting, SqlType.Common);
                    }
                }
                else
                {
                    foreach (var viewName in addTableNames)
                    {
                        //OutputText(string.Format("生成创建{0}{1}的SQL\n", viewName, temp), OutputType.Comment);
                        Output(string.Format("生成创建{0}{1}的SQL\n", viewName, temp), OutputType.Comment, setting, SqlType.Common);
                        string addViewSql = dHelper.GetAddProcsSql(newItems[viewName]);
                        AppendLine(addViewSql, OutputType.Sql, SqlType.Create);
                        //OutputText("\n", OutputType.None);
                        Output("\n", OutputType.None, setting, SqlType.Common);
                    }
                }
            }
            // 对两版本中均存在的表格进行对比
            foreach (string tableName in newItems.Keys)
            {
                Function newTableInfo = newItems[tableName];
                if (oldItems.Keys.Contains(tableName))
                {
                    //AppendLine("----------------------------------------------\n", OutputType.Comment);
                    Output("----------------------------------------------\n", OutputType.Comment, setting, SqlType.Common);
                    //AppendLine(string.Format(temp + "：{0}\n", tableName), OutputType.Comment);
                    Output(string.Format(temp + "：{0}\n", tableName), OutputType.Comment, setting, SqlType.Common);
                    Function oldTableInfo = oldItems[tableName];

                    if (setting.CheckCommon && !string.Equals(newTableInfo.Comment, oldTableInfo.Comment))
                    {
                        //AppendLine(string.Format("  注释：\"{0}\" => \"{1}\"\n", oldTableInfo.Comment, newTableInfo.Comment), OutputType.Comment);
                        Output(string.Format("  注释：\"{0}\" => \"{1}\"\n", oldTableInfo.Comment, newTableInfo.Comment), OutputType.Comment, setting, SqlType.Common);
                    }
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
                    //if (!newTableInfo.Info.SQLModel.Equals(oldTableInfo.Info.SQLModel))
                    //{
                    //    AppendLine(string.Format("  SQLModel：\"{0}\" => \"{1}\"\n", oldTableInfo.Info.SQLModel, newTableInfo.Info.SQLModel), OutputType.Comment);
                    //}
                    //避免DEFINER 和注释产生影响
                    var flagTypeStr = isFun ? "FUNCTION" : "PROCEDURE";

                    string oldSql = oldTableInfo.Info.CreateSQL;
                    string newSql = newTableInfo.Info.CreateSQL;

                    if (oldSql.IndexOf("CREATE DEFINER=") > -1 && oldSql.IndexOf(flagTypeStr) > 0)
                        oldSql = "CREATE " + oldSql.Substring(oldSql.IndexOf(flagTypeStr));
                    if (newSql.IndexOf("CREATE DEFINER=") > -1 && newSql.IndexOf(flagTypeStr) > 0)
                        newSql = "CREATE " + newSql.Substring(newSql.IndexOf(flagTypeStr));

                    //忽略 CHARSET
                    int temp_int = oldSql.IndexOf(" CHARSET ");
                    if (temp_int > 0 && oldSql.IndexOf("begin") > temp_int)
                        oldSql = oldSql.Substring(0, temp_int) + "\r\n" + oldSql.Substring(oldSql.IndexOf("begin"));
                    temp_int = newSql.IndexOf(" CHARSET ");
                    if (temp_int > 0 && newSql.IndexOf("begin") > temp_int)
                        newSql = newSql.Substring(0, temp_int) + "\r\n" + newSql.Substring(newSql.IndexOf("begin"));

                    //忽略换行
                    oldSql = oldSql.Replace("\r\n", "").Replace("\n", "");
                    newSql = newSql.Replace("\r\n", "").Replace("\n", "");

                    //if (tableName == "fun_getMemberPropertyValue") {
                    //    int ooo = 0;
                    //}
                    if (!oldSql.Equals(newSql))
                    {
                        //AppendLine("  " + temp + "内容有变化\n", OutputType.Comment);
                        Output("  " + temp + "内容有变化\n", OutputType.Comment, setting, SqlType.Common);
                        //int inxItem = -1;
                        //foreach (char item in oldSql)
                        //{
                        //    inxItem++;
                        //    if (newSql.Length > inxItem)
                        //    {
                        //        if (newSql[inxItem] != oldSql[inxItem])
                        //        {
                        //            bool sqlNoEq = false;
                        //        }
                        //    }
                        //}
                        //删除后创建
                        if (isFun)
                        {
                            string dropViewSql = dHelper.GetDropFuncSql(tableName);
                            AppendLine(dropViewSql, OutputType.Sql, SqlType.Alter);
                            //OutputText(string.Format("生成创建{0}{1}的SQL\n", tableName, temp), OutputType.Comment);
                            Output(string.Format("生成创建{0}{1}的SQL\n", tableName, temp), OutputType.Comment, setting, SqlType.Common);
                            string addViewSql = dHelper.GetAddFuncSql(newItems[tableName]);
                            AppendLine(addViewSql, OutputType.Sql, SqlType.Alter);
                            //OutputText("\n", OutputType.None);
                            Output("\n", OutputType.None, setting, SqlType.Common);
                        }
                        else
                        {
                            string dropViewSql = dHelper.GetDropProcsSql(tableName);
                            AppendLine(dropViewSql, OutputType.Sql, SqlType.Alter);
                            //OutputText(string.Format("生成创建{0}{1}的SQL\n", tableName, temp), OutputType.Comment);
                            Output(string.Format("生成创建{0}{1}的SQL\n", tableName, temp), OutputType.Comment, setting, SqlType.Common);
                            string addViewSql = dHelper.GetAddProcsSql(newItems[tableName]);
                            AppendLine(addViewSql, OutputType.Sql, SqlType.Alter);
                            //OutputText("\n", OutputType.None);
                            Output("\n", OutputType.None, setting, SqlType.Common);
                        }
                    }
                }
            }

            AppendLineToCtrl(setting.OutputComment ? 3 : 1);
            errorString = errorStringBuilder.ToString();
            return string.IsNullOrWhiteSpace(errorString);
        }

    }

    
    
}
