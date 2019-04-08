using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp
{
    public class Util
    {
        public static void DictionarySort(Dictionary<string, int> dic)
        {
            if (dic.Count > 0)
            {
                List<KeyValuePair<string, int>> lst = new List<KeyValuePair<string, int>>(dic);
                lst.Sort(delegate(KeyValuePair<string, int> s1, KeyValuePair<string, int> s2)
                {
                    return s2.Value.CompareTo(s1.Value);
                });
            }
        }
        public static Dictionary<string, T> DictionarySort<T>(Dictionary<string, T> dic, List<string> sort)
        {
            Dictionary<string, T> dicSorted = new Dictionary<string, T>();
            foreach (string item in sort)
            {
                dicSorted.Add(item, dic[item]);
            }
            foreach (string item in dic.Keys)
            {
                if (!sort.Contains(item))
                {
                    dicSorted.Add(item, dic[item]);
                }
            }
            return dicSorted;
        }

        public static List<string> DictionarySort(Dictionary<string, List<string>> dic)
        {
            List<string> left = dic.Keys.ToList();
            List<string> right = new List<string>();
            foreach (List<string> item in dic.Values)
            {
                foreach (string r in item)
                {
                    if (!right.Contains(r))
                    {
                        right.Add(r);
                    }
                }
            }

            return null;
        }

    }
}
