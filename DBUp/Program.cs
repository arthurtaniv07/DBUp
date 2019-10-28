﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.IO;

namespace DBUp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {


                //设置应用程序处理异常方式：ThreadException处理
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
  

                DateTime netTime = DateHelper.GMT2Local(DateHelper.GetNetDate());


                if (netTime< new DateTime(2018, 8, 21))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                else
                {
                    MessageBox.Show("未授权");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("未接入网络 或联系开发者");
            }
        }


        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            MessageBox.Show(str, "系统错误(UI)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //LogManager.WriteLog(str);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            MessageBox.Show(str, "系统错误(not UI)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //LogManager.WriteLog(str);
        }

        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);

                sb.AppendLine("【异常方法】：" + ex.TargetSite);

            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("***************************************************************");


            if (!Directory.Exists("ErrorLog"))
            {
                Directory.CreateDirectory("ErrorLog");
            }
            //记录到文件
            File.AppendAllText("ErrorLog/" + DateTime.Now.ToString("MMdd") + ".txt", sb.ToString());


            return sb.ToString();

        }
    }
}
