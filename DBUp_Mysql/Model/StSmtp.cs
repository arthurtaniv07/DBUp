using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{


    public partial class StSmtp
    {
        #region Model
        private string _smtpserver;
        private string _smtpusername;
        private string _smtpaccountname;
        private string _smtppassword;
        private bool _smtpenablessl = false;
        private uint _smtpport = 25;


        /// <summary>
        /// smtp地址
        /// </summary>
        public string SmtpServer
        {
            set
            {
                _smtpserver = (value == null || value.Length <= 100 ? value : value.Substring(0, 100));
            }
            get { return _smtpserver; }
        }
        /// <summary>
        /// 用戶名
        /// </summary>
        public string SmtpUserName
        {
            set
            {
                _smtpusername = (value == null || value.Length <= 50 ? value : value.Substring(0, 50));
            }
            get { return _smtpusername; }
        }

        /// <summary>
        /// 發送帳戶名
        /// </summary>
        public string SmtpAccountName
        {
            set
            {
                _smtpaccountname = (value == null || value.Length <= 50 ? value : value.Substring(0, 50));
            }
            get { return _smtpaccountname; }
        }

        /// <summary>
        /// 密碼
        /// </summary>
        public string SmtpPassword
        {
            set
            {
                _smtppassword = (value == null || value.Length <= 50 ? value : value.Substring(0, 50));
            }
            get { return _smtppassword; }
        }
        /// <summary>
        /// 設置ssl
        /// </summary>
        public bool SmtpEnableSsl
        {
            set { _smtpenablessl = value; }
            get { return _smtpenablessl; }
        }
        /// <summary>
        /// 設置port
        /// </summary>
        public uint SmtpPort
        {
            set { _smtpport = value; }
            get { return _smtpport; }
        }
        #endregion Model
    }
}
