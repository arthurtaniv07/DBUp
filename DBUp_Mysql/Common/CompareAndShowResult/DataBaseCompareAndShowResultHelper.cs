using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class DataBaseCompareAndShowResultHelper : CompareAndShowResultHelperBase, ICompareAndShowResult
    {
        public override bool CompareAndShow(ref DbModels oldModel, ref DbModels newModel, Setting setting, out string errorString)
        {
            //不对比新增或者删除
            errorString = "";
            //只对比第一个
            DBSetting oldSetting = oldModel.DbModel;
            DBSetting newSetting = newModel.DbModel;


            Output("----------------------------------------------\n", OutputType.Comment, setting, SqlType.Common);
            Output(string.Format("数据库：{0} =》 {1}\n", oldSetting.DbName, newSetting.DbName), OutputType.Comment, setting, SqlType.Common);
            if (oldSetting.SqlMode != newSetting.SqlMode)
            {
                Output(string.Format("  SqlModel：{0} => {1}\n", oldSetting.SqlMode, newSetting.SqlMode), OutputType.Comment, setting, SqlType.Common);
            }
            if (oldSetting.Version != newSetting.Version)
            {
                Output(string.Format("  Version：{0} => {1} （{2}）\n", oldSetting.Version, newSetting.Version, "数据库版本变更，如对业务有影响请手动在服务器安装配置数据库"), OutputType.Comment, setting, SqlType.Common);
            }
            if (oldSetting.TransactionIsolationType != newSetting.TransactionIsolationType)
            {
                Output(string.Format("  事务隔离级别：{0} => {1} （{2}）\n", oldSetting.TransactionIsolationType, newSetting.TransactionIsolationType, "隔离级别变化，如需永久更改请到mysql配置文件更改为" + newSetting.TransactionIsolationType), OutputType.Comment, setting, SqlType.ChangeSetting);
                string changeTransactionIsolationType = DBSqlHelper.GetChangeTranIsolationLevelSql(DbSettingSessionType.GLOBAL, DBTrancstionConst.GetDBChangeTrancstionConst(newSetting.TransactionIsolationType));
                Output(changeTransactionIsolationType, OutputType.Sql, setting, SqlType.Alter);

            }
            AppendLineToCtrl(setting.OutputComment ? 3 : 1);
            return true;
        }

        public override bool GetInfoByDb(string connStr, ref DbModels rel)
        {
            using (Helper = new DBStructureHelper(connStr))
            {
                if (Helper.Open())
                {
                    OutputText?.Invoke("开始获取数据库信息(" + Helper.Server + (Helper.Port == "-1" ? "" : ":" + Helper.Port) + "  " + Helper.DbName + ")\n", OutputType.Comment);
                    Helper.Set_DbHander(SetLen);
                    if (Helper.GetTables(out List<string> tempList, out string errorMsg))
                    {
                        rel.DbModel = Helper.GetDbInfo();
                        OutputText?.Invoke("\n", OutputType.None);
                        OutputText?.Invoke("\n", OutputType.Comment);
                        return true;
                    }
                    else
                    {
                        OutputText?.Invoke("获取数据库信息失败：" + errorMsg, OutputType.Comment);
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
                list.DbModel = Tools.GetModel<DBSetting>(dirName, fileName);
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }

}
