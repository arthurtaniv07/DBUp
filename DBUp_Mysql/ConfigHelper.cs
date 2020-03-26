using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class ConfigHelper
    {

        /// <summary>
        /// 得到AppSettings中的配置字符串信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigString(string key)
        {
            try
            {
                var objModel = ConfigurationManager.AppSettings[key];
                if (objModel != null)
                {
                    return objModel.ToString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 得到AppSettings中的配置Bool信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetConfigBool(string key)
        {
            bool result = false;
            string cfgVal = GetConfigString(key);
            if (!string.IsNullOrEmpty(cfgVal))
            {
                try
                {
                    result = bool.Parse(cfgVal);
                }
                catch (FormatException)
                {
                    result = false;
                    // Ignore format exceptions.
                }
            }
            return result;
        }
        /// <summary>
        /// 得到AppSettings中的配置uint信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static uint GetConfigUInt(string key)
        {
            uint result = 0;
            string cfgVal = GetConfigString(key);
            if (!string.IsNullOrEmpty(cfgVal))
            {
                try
                {
                    result = uint.Parse(cfgVal);
                }
                catch (FormatException)
                {
                    // Ignore format exceptions.
                }
            }

            return result;
        }
    }
}
