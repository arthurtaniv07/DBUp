using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class Tools
    {
        public static string GetTimeSpan(TimeSpan ts)
        {
            List<int> rel = new List<int>();
            if (ts.Hours > 0)
            {
                rel.Add(ts.Hours);
            }

            if (ts.Minutes > 0)
                rel.Add(ts.Minutes);
            else if (rel.Count > 0)
                rel.Add(0);
            rel.Add(ts.Seconds);
            return string.Join(":", rel.Select(i => i.ToString().PadLeft(2, '0')));
        }

        /// <summary>
        /// 获取相对程序的绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isCreate"></param>
        /// <returns></returns>
        public static string GetDirFullPath(string path, bool isCreate = true)
        {

            if (!path.Contains(":"))
            {
                if (!path.StartsWith("/"))
                    path = "/" + path;
            }

            if (!path.EndsWith("/"))
                path += "/";

            string rel = path.Contains(":") ? path : Environment.CurrentDirectory + path;

            if (isCreate && !Directory.Exists(rel))
            {
                Directory.CreateDirectory(rel);
            }
            return rel;
        }
        public static List<T> GetInfo<T>(string dirName, string fileName) where T :class, new()
        {
            List<T>  rel = new List<T>();
            try
            {
                string resultStr = Tools.ReadFileString(dirName, fileName);
                rel = JsonConvert.DeserializeObject<List<T>>(resultStr);
            }
            catch (Exception)
            {
                //文件读取失败时 返回一个空的集合
            }
            return rel;
        }
        public static T GetModel<T>(string dirName, string fileName) where T:class,new()
        {
            T rel = new T();
            try
            {
                string resultStr = Tools.ReadFileString(dirName, fileName);
                return JsonConvert.DeserializeObject<T>(resultStr);

            }
            catch (Exception)
            {
                //文件读取失败时 返回一个空的集合
            }
            return rel;
        }
        public static string ReadFileString(string dirName, string fileName, string currDir = null)
        {
            string resultStr = "";
            if (dirName.Contains(":"))
            {
                resultStr = dirName.TrimEnd('/').TrimEnd('\\') + "/" + fileName;
            }
            else
            {
                if (currDir == null)
                {
                    currDir = Environment.CurrentDirectory;
                }
                resultStr = currDir + string.Format("/DataSourceFile/{0}/{1}", dirName.StartsWith("/") ? dirName.Substring(1) : dirName, fileName);
            }
            if (!File.Exists(resultStr))
                throw new Exception(string.Format("文件不存在（{0}）", resultStr));
            return File.ReadAllText(resultStr);
        }
        /// <summary>
        /// 获取指定目录的下的所有文件夹
        /// </summary>
        /// <param name="dirFullPath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDirNameList(string dirFullPath)
        {
            dirFullPath = Tools.GetDirFullPath(dirFullPath);
            var aaa = new DirectoryInfo(dirFullPath);
            Dictionary<string, string> rel = new Dictionary<string, string>();
            foreach (var item in aaa.GetDirectories())
            {
                rel.Add(item.Name, item.FullName);
            }
            return rel;
        }

        /// <summary>
        /// 从DataSourceFile文件获取数据源信息
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, DBDataSource> GetConnections()
        {
            Dictionary<string, DBDataSource> rel = new Dictionary<string, DBDataSource>();

            var dirNames = GetDirNameList("/DataSourceFile");
            if (dirNames.Count > 0)
            {
                foreach (var item in dirNames)
                {
                    rel.Add(item.Key, new DBDataSource()
                    {
                        Key = "/" + item.Key,
                        Type = DBDataSourceType.DataSourceFile,
                        Value = item.Value,
                        ProviderName = ""
                    });
                }
            }
            return rel;
        }


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
        /// <summary>
        /// 将List中的所有数据用指定分隔符连接为一个新字符串
        /// </summary>
        public static string JoinString(IList<string> list, string separateString)
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
    }
}

