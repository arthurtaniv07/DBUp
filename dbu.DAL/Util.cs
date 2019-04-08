using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbu.DAL
{
    public class Util
    {

        public static string[] Sort(Dictionary<string, List<string>> dic, ref List<string> rellist)
        {
            //排除自身循环引用
            List<string> cf = new List<string>();
            foreach (string str in dic.Keys)
            {
                foreach (string item in dic[str])
                {
                    //这里应该删除item项
                    if (item == str)
                    {
                        cf.Add(item);
                    }
                }
            }
            if (cf.Count > 0)
            {
                foreach (var item in cf)
                {
                    dic[item].Remove(item);
                }
            }

            //检测循环引用
            foreach (string str in dic.Keys)
            {
                foreach (string item in dic[str])
                {
                    if (item == str)
                    {
                        return new string[] { str };
                    }
                    if (dic.ContainsKey(item))
                    {
                        foreach (string item1 in dic[item])
                        {
                            if (item1 == str)
                            {
                                return new string[] { str, item1 };
                            }
                        }
                    }
                }
            }

            Dictionary<string, int> rel = new Dictionary<string, int>();
            foreach (string key in dic.Keys)
            {
                if (!rel.ContainsKey(key))
                {
                    rel.Add(key, 0);
                }
                foreach (string item in dic[key])
                {
                    if (!rel.ContainsKey(item))
                    {
                        rel.Add(item, 0);
                    }
                }
            }
            string[] erro = sort(dic, rel);
            if (erro.Length == 0)
            {
                rellist = rel.OrderBy(i => i.Value).Select(i => i.Key).ToList();
            }

            return erro;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="rel"></param>
        /// <param name="ct">冲突的</param>
        private static string[] sort(Dictionary<string, List<string>> dic, Dictionary<string, int> all)
        {
            foreach (string str in dic.Keys)
            {
                //Console.WriteLine("key=" + str);
                foreach (string item in dic[str])
                {
                    //Console.WriteLine("key=" + str + ",item=" + item);
                    jia(dic, all, item);
                    //foreach (var item1 in all.Keys)
                    //{
                    //    Console.WriteLine(item1 + "\t" + all[item1]);
                    //}
                }
                //Console.WriteLine();
            }
            return new string[0];
        }
        /// <summary>
        /// 将指定key所在树的深度＋1
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="all"></param>
        /// <param name="key"></param>
        private static void jia(Dictionary<string, List<string>> dic, Dictionary<string, int> all, string key)
        {
            all[key]++;
            if (dic.ContainsKey(key))
            {
                foreach (var item in dic[key])
                {
                    jia(dic, all, item);
                }
            }
        }

    }
}
