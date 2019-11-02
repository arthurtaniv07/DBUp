using System;
using System.Collections.Generic;
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
    }
}
