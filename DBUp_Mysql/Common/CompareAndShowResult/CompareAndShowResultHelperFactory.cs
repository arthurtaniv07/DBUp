using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class CompareAndShowResultHelperFactory
    {
        /// <summary>
        /// 获取帮助类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CompareAndShowResultHelperBase GetHelper(DBObjType type)
        {
            CompareAndShowResultHelperBase rel;
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
                case DBObjType.DBSetting:
                    rel = new DataBaseCompareAndShowResultHelper();
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
                case DBObjType.DBSetting:
                    return "数据库";
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
                case DBObjType.DBSetting:
                    return cs.DBSetting;
                default:
                    throw new Exception("未知的（DBObjType）");
            }
        }

    }
}
