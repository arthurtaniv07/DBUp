using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public delegate bool OutPutTextHander(string str, OutputType type = OutputType.Comment, SqlType sqlType = 0);
    public delegate string GetResultTextHander();
    public abstract class CompareAndShowResultHelperBase : ICompareAndShowResult
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

        List<Tuple<string, OutputType, SqlType>> ts = new List<Tuple<string, OutputType, SqlType>>();
        public void AppendLine(string str, OutputType type, SqlType sqlType)
        {
            ts.Add(new Tuple<string, OutputType, SqlType>(str, type, sqlType));
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
                OutputText(item.Item1, item.Item2, item.Item3);
            }
            ts.Clear();
        }


        public DBStructureHelper Helper { get; set; }

        public DBStructureHelper dHelper = new DBStructureHelper();

        DateTime startTime = default(DateTime);
        public void SetLen(string currTabName, int tabCount, int i)
        {
            if (i == 0 || i == 1)
            {
                startTime = DateTime.Now;
            }
            if (tabCount == i || true)
            {
                ReplaceLastLineText(string.Format("当前进度：{0}% {1}/{2} ，耗时：" + Tools.GetTimeSpan(DateTime.Now - startTime), ToPrecision(i * 100.0 / tabCount, 2), i, tabCount, currTabName), OutputType.Loading);
            }
            else
                ReplaceLastLineText(string.Format("当前进度：{0}% {1}/{2} ({3}) ", ToPrecision(i * 100.0 / tabCount, 2), i, tabCount, currTabName), OutputType.Loading);
        }



        /// <summary>
        /// 将List中的所有数据用指定分隔符连接为一个新字符串
        /// </summary>
        public string JoinString(IList<string> list, string separateString)
        {
            if (list == null || list.Count < 1)
                return null;
            return string.Join(separateString, list);

        }
        

        public void Output(string text, OutputType outputType, Setting setting, SqlType sqlType, bool appendLing = true)
        {
            if (OutputText == null) return;
            //注释
            if (setting.OutputComment == false && sqlType == SqlType.Common) return;
            //删除
            if (sqlType == SqlType.Delete)
            {
                if (setting.OutputDeleteSql == false) return;
                if (setting.OutputDeleteSqlIsCommon) outputType = OutputType.Comment;
            }
            if (appendLing)
            {
                AppendLine(text, outputType, sqlType);
                return;
            }
            OutputText(text, outputType, sqlType);
        }

        public abstract bool GetInfoByDb(string connStr, ref DbModels rel);
        public abstract string GetInfoByFile(string dirName, string fileName, ref DbModels list);
        public abstract bool CompareAndShow(ref DbModels oldItems, ref DbModels newItems, Setting setting, out string errorString);
    }
    
}
