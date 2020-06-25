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
    }
}

