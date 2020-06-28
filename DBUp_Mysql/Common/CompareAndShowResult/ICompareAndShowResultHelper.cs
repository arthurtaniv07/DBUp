using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public interface ICompareAndShowResult
    {
        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="rel"></param>
        /// <returns></returns>
        bool GetInfoByDb(string connStr, ref DbModels rel);

        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="rel"></param>
        /// <returns></returns>
        string GetInfoByFile(string dirName, string fileName, ref DbModels list);

        /// <summary>
        /// 比较并输出信息到界面中
        /// </summary>
        /// <param name="oldModel"></param>
        /// <param name="newModel"></param>
        /// <param name="setting"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        bool CompareAndShow(ref DbModels oldModel, ref DbModels newModel, Setting setting, out string errorString);
    }
}
