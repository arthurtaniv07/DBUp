using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Runtime.InteropServices;

namespace DBUp
{
    public class DateHelper
    {
        /// <summary>  
        /// 获取国家授时中心网提供的时间。（授时中心连接经常出状况，暂时用百度网代替）  
        /// 通过分析网页报头，查找Date对应的值，获得GMT格式的时间。可通过GMT2Local(string gmt)方法转化为本地时间格式。  
        /// 用法 DateTime netTime = GetNetTime.GMT2Local(GetNetTime.GetNetDate());  
        /// </summary>  
        /// <returns></returns>  
        public static string GetNetDate()
        {
            try
            {
                //WebRequest request = WebRequest.Create("http://www.time.ac.cn");//国家授时中心经常连接不上  
                WebRequest request = WebRequest.Create("http://www.baidu.com");
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                WebHeaderCollection myWebHeaderCollection = response.Headers;
                for (int i = 0; i < myWebHeaderCollection.Count; i++)
                {
                    string header = myWebHeaderCollection.GetKey(i);
                    string[] values = myWebHeaderCollection.GetValues(header);
                    if (header.Length <= 0 || header == null)
                    {
                        return null;
                    }
                    else if (header == "Date")
                    {
                        return values[0];
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [DllImport("kernel32.dll")]
         private static extern bool SetLocalTime(ref Systemtime time);
 
         [StructLayout(LayoutKind.Sequential)]
         public struct Systemtime
         {
             public short year;
             public short month;
             public short dayOfWeek;
             public short day;
             public short hour;
             public short minute;
             public short second;
             public short milliseconds;
         }
         public static Systemtime GetSystemtime(DateTime dt)
         {
             Systemtime st;

             st.year = (short)dt.Year;
             st.month = (short)dt.Month;
             st.dayOfWeek = (short)dt.DayOfWeek;
             st.day = (short)dt.Day;
             st.hour = (short)dt.Hour;
             st.minute = (short)dt.Minute;
             st.second = (short)dt.Second;
             st.milliseconds = (short)dt.Millisecond;

             return st;
         }


        /// <summary>
        /// 将本地时间转换为GMT时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToGMT(DateTime dt)
        {
            return dt.ToUniversalTime().ToString("r");
        }

        /// <summary>    
        /// GMT时间转成本地时间   
        /// DateTime dt1 = GMT2Local("Thu, 29 Sep 2014 07:04:39 GMT");  
        /// 转换后的dt1为：2014-9-29 15:04:39  
        /// DateTime dt2 = GMT2Local("Thu, 29 Sep 2014 15:04:39 GMT+0800");  
        /// 转换后的dt2为：2014-9-29 15:04:39  
        /// </summary>    
        /// <param name="gmt">字符串形式的GMT时间</param>    
        /// <returns></returns>    
        public static DateTime GMT2Local(string gmt)
        {
            
            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmt.IndexOf("+0") != -1)
                {
                    gmt = gmt.Replace("GMT", "");
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";
                }
                if (gmt.ToUpper().IndexOf("GMT") != -1)
                {
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmt);
                }
            }
            catch
            {
            }
            return dt;
        }  
    }
}
