using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DBUp_Mysql
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        //  DBUp_Mysql.exe -out "E:\a c"
        //  DBUp_Mysql.exe -out E:\~c
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var param = Tools.GetParam(args);
            Tools.CurrentDirectory = Tools.GetParamString(ref param, "out");


            Application.Run(new MainForm());
        }
    }
}
