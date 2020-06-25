using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DBUp_Mysql
{
    public class Win32API
    {
        /// <summary>
        /// 获取网络状态
        /// </summary>
        /// <param name="dwFlag">驱动类型</param>
        /// <param name="dwReserved">0</param>
        /// <returns>是否连接网络</returns>
        [DllImport("winInet.dll")]
        public static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);
    }
}
