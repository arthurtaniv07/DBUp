using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace DBUp_Mysql
{
    public class StMailHelper
    {

        /// <summary>
        /// SMTP主機
        /// </summary>
        static readonly string smtpServer = ConfigHelper.GetConfigString("SmtpServer");

        /// <summary>
        /// 發件人
        /// </summary>
        static readonly string from = ConfigHelper.GetConfigString("EmailFrom");

        /// <summary>
        /// 發件人郵箱密碼
        /// </summary>
        static readonly string emailPwd = ConfigHelper.GetConfigString("EmailPwd");

        /// <summary>
        /// SMTP主機端口
        /// </summary>
        static readonly uint smtpPort = ConfigHelper.GetConfigUInt("SmtpPort");

        /// <summary>
        /// 發件人郵箱密碼
        /// </summary>
        static readonly bool SmtpEnableSSL = ConfigHelper.GetConfigBool("SmtpEnableSSL");

        /// <summary>
        /// 發件人郵箱密碼
        /// </summary>
        static readonly string SmtpAccountName = ConfigHelper.GetConfigString("EmailAccountName");

        /// <summary>
        /// 轉發電郵（如果這個存在則電郵都會發送到該賬戶）
        /// </summary>
        static readonly string ForwardEmail = ConfigHelper.GetConfigString("ForwardEmail");
        public static bool CheckSend(string toEmail, string fromName, string subject, string body, int sendLevel = 0)
        {

            toEmail = ForwardEmail ?? toEmail;
            var newSmtpModel = new StSmtp()
            {
                SmtpServer = smtpServer,
                SmtpUserName = from,
                SmtpAccountName = SmtpAccountName,
                SmtpPassword = emailPwd,
                SmtpEnableSsl = SmtpEnableSSL,
                SmtpPort = smtpPort == 0 ? 25 : smtpPort,
                Mode = "Default"
            };
            try
            {
                Send(toEmail.Split(';'), subject, body, newSmtpModel.SmtpUserName, newSmtpModel);
            }
            catch (Exception ex)
            {
            }
            return true;
            

        }


        /// <summary>
        ///  发邮件
        /// </summary>
        /// <param name="toArray">收件人</param>
        /// <param name="subject">主题</param>
        /// <param name="body">正文</param>
        /// <param name="fromName">发件人名稱(顯示)</param>
        /// <param name="smtpModel">Able_Smtp对象</param>
        /// <param name="isWriteLog">是否寫日誌</param>
        /// <param name="addFile">是否添加附件</param>
        /// <param name="fileUrl">添加附件的絕對路徑集合</param>
        public static void Send(string[] toArray, string subject, string body, string fromName, StSmtp smtpModel)
        {
            if (smtpModel.SmtpPort == 465)
            {
                System.Web.Mail.MailMessage mmsg = new System.Web.Mail.MailMessage();
                //验证  
                mmsg.Subject = subject;// "zhuti1";//邮件主题

                mmsg.BodyFormat = System.Web.Mail.MailFormat.Html;
                mmsg.Body = body;// "wqerwerwerwer";//邮件正文
                mmsg.BodyEncoding = Encoding.UTF8;//正文编码

                mmsg.Priority = System.Web.Mail.MailPriority.High;//优先级

                mmsg.From = smtpModel.SmtpUserName;//发件者邮箱地址
                mmsg.To = string.Join(";", toArray);


                //抄送
                //foreach (var cc in mailCC)
                //{
                //    mailCCString.Append(cc + ";");
                //}
                //mmsg.Cc = mailCCString.ToString();


                mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                //登陆名  
                mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", smtpModel.SmtpUserName);
                //登陆密码  
                mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", smtpModel.SmtpPassword);

                mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", 465);//端口 
                mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", smtpModel.SmtpEnableSsl.ToString().ToLower());
                System.Web.Mail.SmtpMail.SmtpServer = smtpModel.SmtpServer;    //企业账号用smtp.exmail.qq.com 

                System.Web.Mail.SmtpMail.Send(mmsg);
                return;
            }

            MailAddress mailAddress = new MailAddress(smtpModel.SmtpUserName, fromName);
            MailMessage m = new MailMessage();
            m.From = mailAddress;
            foreach (string t in toArray)
            {
                m.To.Add(t);//收件人处会显示所有人的邮箱
            }

            m.Subject = subject;
            m.Body = body;
            m.IsBodyHtml = true;
            m.Priority = MailPriority.Normal;
            m.BodyEncoding = Encoding.GetEncoding("utf-8");

            SmtpClient smtp = new SmtpClient();
            smtp.Host = smtpModel.SmtpServer;
            if (!string.IsNullOrEmpty(smtpModel.SmtpPassword))
            {
                string userName = smtpModel.SmtpUserName;
                if (!string.IsNullOrEmpty(smtpModel.SmtpAccountName))
                {
                    userName = smtpModel.SmtpAccountName;
                }
                smtp.Credentials = new System.Net.NetworkCredential(userName, smtpModel.SmtpPassword);
            }

            smtp.EnableSsl = smtpModel.SmtpEnableSsl;
            smtp.Port = (int)smtpModel.SmtpPort;
            smtp.Send(m);
        }
    } 
    
    public partial class StSmtp
    {
        #region Model
        private string _smtpserver;
        private string _smtpusername;
        private string _smtpaccountname;
        private string _smtppassword;
        private bool _smtpenablessl = false;
        private uint _smtpport = 25;
        private string _mode;
        

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
        /// <summary>
        /// 选择的模式，可以时google 和yahoo，default
        /// </summary>
        public string Mode
        {
            set
            {
                _mode = (value == null || value.Length <= 100 ? value : value.Substring(0, 100));

            }
            get { return _mode; }
        }
        #endregion Model
    }
}
