using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;

namespace DBUp
{
    /// <summary>
    /// 配置文件操作类
    /// </summary>
    public class AppConfigHelper
    {
        #region 获取值
        /// <summary>  
        /// 根据Key取Value值    不存在时返回默认字符串
        /// </summary>  
        /// <param name="key"></param>  
        /// <param name="defaultValue">默认字符串</param>  
        /// <param name="isTrim">是否去掉两边的空格</param>  
        public static string GetValue(string key, string defaultValue = "", bool isTrim = true)
        {
            if (ConfigurationManager.AppSettings[key] == null) { return defaultValue; }
            if (isTrim)
                return ConfigurationManager.AppSettings[key].ToString().Trim();
            else
                return ConfigurationManager.AppSettings[key].ToString();
        }

        /// <summary>  
        /// 根据Key取Boolean值    不存在时返回默认值
        /// </summary>  
        /// <param name="key"></param>  
        /// <param name="isTrim">不存在时的默认值</param>  
        public static bool GetBooleanValue(string key, bool defaultValue = false)
        {
            if (ConfigurationManager.AppSettings[key] == null) { return defaultValue; }

            //获取值
            string v = ConfigurationManager.AppSettings[key].ToString().Trim().ToLower();

            if (v == "true" || v == "1" || v == "yes") { return true; }
            return false;
        }
        /// <summary>  
        /// 根据Key取Double值    不存在时返回默认值
        /// </summary>  
        /// <param name="key"></param>  
        /// <param name="isTrim">不存在时的默认值</param>  
        public static double GetDoubleValue(string key, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[key])) { return defaultValue; }
            //获取值
            string v = ConfigurationManager.AppSettings[key].ToString().Trim().ToLower();
            double.TryParse(v, out defaultValue);
            return defaultValue;
        }
        /// <summary>  
        /// 根据Key取Int值    不存在时返回默认值
        /// </summary>  
        /// <param name="key"></param>  
        /// <param name="isTrim">不存在时的默认值</param>  
        public static int GetIntValue(string key, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[key])) { return defaultValue; }
            //获取值
            string v = ConfigurationManager.AppSettings[key].ToString().Trim().ToLower();
            int.TryParse(v, out defaultValue);
            return defaultValue;
        }

        #endregion

        /// <summary>  
        /// 根据Key修改Value    不存在时自动添加
        /// </summary>  
        /// <param name="key">要修改的Key</param>  
        /// <param name="value">要修改为的值</param>  
        public static void SetValue(string key, string value)
        {
            if (ConfigurationManager.AppSettings[key] == null)
            {
                //不存在
                ConfigurationManager.AppSettings.Add(key, value);
                return;
            }
            ConfigurationManager.AppSettings.Set(key, value);
        }

        /// <summary>  
        /// 添加新的Key ，Value键值对  
        /// </summary>  
        /// <param name="key">Key</param>  
        /// <param name="value">Value</param>  
        public static void Add(string key, string value)
        {
            ConfigurationManager.AppSettings.Add(key, value);
        }

        /// <summary>  
        /// 根据Key删除项   
        /// </summary>  
        /// <param name="key">Key</param>  
        public static void Remove(string key)
        {
            if (ConfigurationManager.AppSettings[key] != null)
                ConfigurationManager.AppSettings.Remove(key);
        }





        #region 以下是扩展方法



        /// <summary>
        /// 是否开启了后台审核验证  - IsAuditValidate
        /// </summary>
        /// <returns></returns>
        public static bool Ex_IsAuditValidate()
        {
            return GetBooleanValue("IsAuditValidate", true);
        }

        /// <summary>
        /// 在添加或修改团数据时 是否验证机票(剩余数量)  - AddOrUpdateGroupData_ValidateFielTicket
        /// </summary>
        /// <returns></returns>
        public static bool Ex_AddOrUpdateGroupData_ValidateFielTicket()
        {
            return GetBooleanValue("AddOrUpdateGroupData_ValidateFielTicket", true);
        }
        /// <summary>
        /// 是否开启第一级审核的部门权限  - IsOpen_FirstAudit_Dep
        /// </summary>
        /// <returns></returns>
        public static bool Ex_IsOpen_FirstAudit_Dep()
        {
            return GetBooleanValue("IsOpen_FirstAudit_Dep", true);
        }
        #endregion
    }
    

     
}