using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using dbu.DAL;
using System.IO;
using dbu.Model;
using System.Net;
using System.Net.Sockets;

namespace DBUp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }




        private void Form1_Load(object sender, EventArgs e)
        {

            this.dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            this.dateTimePicker1.MaxDate = DateTime.Now;

            //MessageBox.Show( "\\  s/".Replace("*",""));
            this.lblCurrState.Text = "";
            //this.txtServerAddress.Text = "(local)";
            this.txtServerAddress.Text = AppConfigHelper.GetValue("formAddr", "(local)");
            this.txtuserName.Text = AppConfigHelper.GetValue("formName", "sa");
            this.txtpassWord.Text = AppConfigHelper.GetValue("formPass", "123456");
            this.txtServerAddress2.Text = AppConfigHelper.GetValue("toAddr", "(local)");
            this.txtuserName2.Text = AppConfigHelper.GetValue("toName", "sa");
            this.txtpassWord2.Text = AppConfigHelper.GetValue("toPass", "123456");





        }

        bool isLineSuccess = false;
        string serverAddress = "";
        string passWord = "";
        string name = "";


        private void ChangeCurrState(string msg)
        {
            this.lblCurrState.Text = msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isTest">是否是测试连接 为false则获取数据库名称列表</param>
        /// <returns></returns>
        List<string> LineToServer(bool isTest, bool isShouSuccessMsg = true)
        {
            isLineSuccess = false;

            if (string.IsNullOrWhiteSpace(serverAddress = this.txtServerAddress.Text) || string.IsNullOrWhiteSpace(name = this.txtuserName.Text) || string.IsNullOrWhiteSpace(passWord = this.txtpassWord.Text))
            {
                MessageBox.Show("请将连接信息填写完整");
                return null;
            }
            if (isTest)
            {
                if (DBHelper.TextLineToServer(serverAddress, name, passWord))
                {
                    if (isShouSuccessMsg)
                        MessageBox.Show("测试连接成功");
                    return new List<string>();
                }
                else
                {
                    if (isShouSuccessMsg)
                        MessageBox.Show("测试连接失败");
                    else
                        MessageBox.Show("连接失败");
                    return null;
                }
            }

            //不是测试连接  获取数据库
            return DBHelper.getAlldbName(serverAddress, name, passWord, !this.checkBox3.Checked);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.comBoxDBList.DataSource = null;

            if (LineToServer(true, false) == null)
            {
                ChangeCurrState("服务器连接失败");
                return;
            }

            ChangeCurrState("开始加载数据...");
            isLineSuccess = false;
            List<string> dbList = null;
            dbList = LineToServer(false);

            //连接数据库成功  ，将数据库名称列表添加到下拉框
            this.comBoxDBList.DataSource = dbList;
            ChangeCurrState("数据库列表加载成功");
        }



        bool isLineSuccess2 = false;
        string serverAddress2 = "";
        string passWord2 = "";
        string name2 = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isTest">是否是测试连接 为false则获取数据库名称列表</param>
        /// <returns></returns>
        List<string> LineToServer2(bool isTest, bool isShouSuccessMsg = true)
        {
            isLineSuccess2 = false;

            if (string.IsNullOrWhiteSpace(serverAddress2 = this.txtServerAddress2.Text) || string.IsNullOrWhiteSpace(name2 = this.txtuserName2.Text) || string.IsNullOrWhiteSpace(passWord2 = this.txtpassWord2.Text))
            {
                MessageBox.Show("请将连接信息填写完整");
                return null;
            }
            if (isTest)
            {
                if (DBHelper.TextLineToServer(serverAddress2, name2, passWord2))
                {
                    if (isShouSuccessMsg)
                        MessageBox.Show("测试连接成功");
                    return new List<string>();
                }
                else
                {
                    if (isShouSuccessMsg)
                        MessageBox.Show("测试连接失败");
                    else
                        MessageBox.Show("连接失败");
                    return null;
                }
            }

            //不是测试连接  获取数据库
            return DBHelper.getAlldbName(serverAddress2, name2, passWord2, !this.checkBox3_2.Checked);
        }

        private void button2_2_Click(object sender, EventArgs e)
        {

            this.comBoxDBList2.DataSource = null;

            if (LineToServer2(true, false) == null)
            {
                ChangeCurrState("服务器连接失败");
                return;
            }

            ChangeCurrState("开始加载数据...");
            isLineSuccess2 = false;
            List<string> dbList = null;
            dbList = LineToServer2(false);

            //连接数据库成功  ，将数据库名称列表添加到下拉框
            this.comBoxDBList2.DataSource = dbList;
            ChangeCurrState("数据库列表加载成功");
        }






        
        ///<summary>
        /// 获取标准北京时间2
        ///</summary>
        ///<returns></returns>
        public static DateTime GetBeijingTime(string url="")
        {
            //t0 = new Date().getTime();
            //nyear = 2011;
            //nmonth = 7;
            //nday = 5;
            //nwday = 2;
            //nhrs = 17;
            //nmin = 12;
            //nsec = 12;
            DateTime dt;
            WebRequest wrt =null;
            WebResponse wrp =null;
            try
            {
                wrt = WebRequest.Create("http://www.beijing-time.org/");
                wrp = wrt.GetResponse();
                string html =string.Empty;
                using (Stream stream = wrp.GetResponseStream())
                {
                    using (StreamReader sr =new StreamReader(stream, Encoding.UTF8))
                    {
                        html = sr.ReadToEnd();
                    }
                }

                string[] tempArray = html.Split(':');
                for (int i =0; i < tempArray.Length; i++)
                {
                    tempArray[i] = tempArray[i].Replace("\r\n", "");
                }

                string year = tempArray[1].Substring(tempArray[1].IndexOf("nyear=") +6);
                string month = tempArray[2].Substring(tempArray[2].IndexOf("nmonth=") +7);
                string day = tempArray[3].Substring(tempArray[3].IndexOf("nday=") +5);
                string hour = tempArray[5].Substring(tempArray[5].IndexOf("nhrs=") +5);
                string minite = tempArray[6].Substring(tempArray[6].IndexOf("nmin=") +5);
                string second = tempArray[7].Substring(tempArray[7].IndexOf("nsec=") +5);
                dt = DateTime.Parse(year +"-"+ month +"-"+ day +""+ hour +":"+ minite +":"+ second);
            }
            catch (System.Net.WebException)
            {
                return DateTime.MinValue;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
            finally
            {
                if (wrp !=null)
                    wrp.Close();
                if (wrt !=null)
                    wrt.Abort();
            }
            return dt;
        }





        Dictionary<string, List<StObjField>> fss = null;
        Dictionary<string, List<StObjField>> fss2 = null;

        /// <summary>
        /// 程序是否在调试
        /// </summary>
        private bool IsDebug = AppConfigHelper.GetBooleanValue("IsDebug");


        private void button3_Click(object sender, EventArgs e)
        {
            if (IsDebug)
            {
                ShengCheng();
            }
            else
            {
                System.Threading.Thread t = new System.Threading.Thread(ShengCheng);
                t.IsBackground = true;
                t.Start();
            }
        }

        string dbName = "";
        string dbName2 = "";

        static List<StObject> StObjects_1 = null;


        /// <summary>
        /// 转换数字为指定精度的小数(四舍五入)
        /// </summary>
        /// <param name="_input">要转换的小数</param>
        /// <param name="fractionDigits">保留的精度</param>
        /// <returns>转换后的结果</returns>
        public static double ToPrecision(double _input, int fractionDigits)
        {
            return Math.Round(_input, fractionDigits, MidpointRounding.AwayFromZero);
        }


        string currDBName = "";
        int tabCount = 0;
        public void SetLen(int i)
        {

            this.lblCurrState.Text = this.lblCurrState.Tag + string.Format("({0}) {1}%", currDBName, ToPrecision(i * 100.0 / tabCount, 2));
        }

        public void reSetCurrLevel(string dbName, int tabCount)
        {
            this.currDBName = dbName;
            this.tabCount = tabCount;
        }

        int temp_i = 0;
        string temp_str = string.Empty;
        public void ShengCheng()
        {
            DateTime? sTime = null;
            DBHelper.Set_GetDBTableInfohander(SetLen);
            #region 验证输入


            if (this.CheckSTime.Checked)
            {
                sTime = this.dateTimePicker1.Value;
            }


            ChangeCurrState("正在验证输入...");
            //点击生成
            if (this.comBoxDBList.DataSource == null || String.IsNullOrWhiteSpace(this.comBoxDBList.Text))
            {
                ChangeCurrState("您还没有选择数据库");
                MessageBox.Show("您还没有选择数据库");
                return;
            }
            string[] ss = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            #endregion

            List<string> types = new List<string>() { "U", "V" };
            //if (this.checkBox1.Checked)
            //{
            //    types.Add("U");
            //}
            //if (this.checkBox2.Checked)
            //{
            //    types.Add("V");
            //}

            string types_str = "'" + string.Join("','", types) + "'";
            string[] conf_s = new string[] { };

            dbName = this.comBoxDBList.Text;
            dbName2 = this.comBoxDBList2.Text;

            ChangeCurrState("正在获取数据库字段信息...");

            string linkStr=DBHelper.GetLineStr(serverAddress, dbName, name, passWord);
            string linkStr2 = DBHelper.GetLineStr(serverAddress2, dbName2, name2, passWord2);

            if (linkStr == linkStr2)
            {
                MessageBox.Show("请选择两个不同的数据库");
                return;
            }
            //连接字符串
            string connStr2 = DBHelper.GetLineStr(this.txtServerAddress2.Text, this.comBoxDBList2.Text, this.txtuserName2.Text, this.txtpassWord2.Text);
            string connStr1 = DBHelper.GetLineStr(this.txtServerAddress.Text, this.comBoxDBList.Text, this.txtuserName.Text, this.txtpassWord.Text); 


            int c = DBHelper.QueryDataTable(linkStr, "select name from sysobjects where xtype in (" + types_str + ")").Rows.Count;
            reSetCurrLevel(dbName, c);
            //对于原表全库对比  只是不做删除的对比
            //fss = DBHelper.GetAllTableFieldInfo(serverAddress, dbName, name, passWord, types, sTime, "_", conf_s);
            fss = DBHelper.GetAllTableFieldInfo(serverAddress, dbName, name, passWord, types, null, "_", conf_s);

            int c2 = DBHelper.QueryDataTable(linkStr2, "select name from sysobjects where xtype in (" + types_str + ")").Rows.Count;
            reSetCurrLevel(dbName2, c2);
            fss2 = DBHelper.GetAllTableFieldInfo(serverAddress2, dbName2, name2, passWord2, types, sTime, "_", conf_s);//目标数据库


            //排序引用视图
            List<string> views = new List<string>();
            DBHelper.GetAllViewSortInfo(connStr2, ref views);
            fss2 = Util.DictionarySort(fss2, views);

            //排序引用表
            List<string> tabSort = DBHelper.GetAllTableSortInfo(serverAddress2, dbName2, name2, passWord2);
            fss2 = Util.DictionarySort(fss2, tabSort);


            DBHelper.Clear_GetDBTableInfohander();

            StObjects_1 = DBHelper.GetAllTableObjectsInfo2(serverAddress2, dbName2, name2, passWord2);




            ChangeCurrState("字段信息获取成功...");


            bool isadd = false;
            bool isdel = false;
            bool isupd = false;
            StringBuilder add = new StringBuilder();
            StringBuilder del = new StringBuilder();
            StringBuilder upd = new StringBuilder();


            bool isaddtab = false;
            bool isdeltab = false;
            StringBuilder addtab = new StringBuilder();
            StringBuilder deltab = new StringBuilder();

            StringBuilder addview = new StringBuilder();
            StringBuilder delview = new StringBuilder();
            StringBuilder updview = new StringBuilder();

            StringBuilder addproc = new StringBuilder();
            StringBuilder delproc = new StringBuilder();
            StringBuilder updproc = new StringBuilder();

            StringBuilder addtr = new StringBuilder();
            StringBuilder deltr = new StringBuilder();
            StringBuilder updtr = new StringBuilder();

            StringBuilder refView = new StringBuilder();
            StringBuilder refView2 = new StringBuilder();

            bool temp_bool = false;
            #region 获取存储过程和触发器
            Dictionary<string, StObject> objs = new Dictionary<string, StObject>(StringComparer.OrdinalIgnoreCase);
            DataTable dt1 = DBHelper.QueryDataTable(connStr1, "select id [ObjID],xtype [type],name [ObjName] from sysobjects where xtype in ('TR','P')");
            foreach (DataRow item in dt1.Rows)
            {
                StObjectType t = default(StObjectType);
                Enum.TryParse<StObjectType>(item["Type"] + "", false, out t);
                objs.Add(item["ObjName"] + "", new StObject() { ObjID = Convert.ToInt32(item["ObjID"]), ObjName = item["ObjName"] + "", Type = t });
            }
            Dictionary<string, StObject> objs2 = new Dictionary<string, StObject>(StringComparer.OrdinalIgnoreCase);
            DataTable dt2 = DBHelper.QueryDataTable(connStr2, "select id [ObjID],xtype [type],name [ObjName] from sysobjects where xtype in ('TR','P')");
            foreach (DataRow item in dt2.Rows)
            {
                StObjectType t = default(StObjectType);
                Enum.TryParse<StObjectType>(item["Type"] + "", false, out t);
                objs2.Add(item["ObjName"] + "", new StObject() { ObjID = Convert.ToInt32(item["ObjID"]), ObjName = item["ObjName"] + "", Type = t });
            }

            foreach (string objName in objs2.Keys)
            {
                if (!objs.ContainsKey(objName)) {
                    switch (objs2[objName].Type)
                    {
                        case StObjectType.U:
                            break;
                        case StObjectType.V:
                            break;
                        case StObjectType.PK:
                            break;
                        case StObjectType.C:
                            break;
                        case StObjectType.D:
                            break;
                        case StObjectType.P:

                            DataTable dt = DBHelper.QueryDataTable(connStr2, "sp_helptext '" + objName + "'");
                            
                            foreach (DataRow item in dt.Rows)
                            {
                                if (string.IsNullOrWhiteSpace(item[0] + "")) { continue; }
                                addproc.Append(item[0] + "");
                            }
                            addproc.AppendLine();
                            addproc.AppendLine("go");
                            addproc.AppendLine();
                            addproc.AppendLine();
                            continue;
                            break;
                        case StObjectType.TR:
                            DataTable dttr = DBHelper.QueryDataTable(connStr2, "sp_helptext '" + objName + "'");
                            
                            foreach (DataRow item in dttr.Rows)
                            {
                                if (string.IsNullOrWhiteSpace(item[0] + "")) { continue; }
                                addtr.Append(item[0] + "");
                            }
                            addtr.AppendLine();
                            addtr.AppendLine("go");
                            addtr.AppendLine();
                            addtr.AppendLine();
                            continue;
                            break;
                        case StObjectType.F:
                            break;
                        case StObjectType.TT:
                            break;
                        case StObjectType.IF:
                            break;
                        case StObjectType.UQ:
                            break;
                        case StObjectType.L:
                            break;
                        case StObjectType.X:
                            break;
                        case StObjectType.TF:
                            break;
                        case StObjectType.S:
                            break;
                        case StObjectType.RF:
                            break;
                        case StObjectType.FN:
                            break;
                        default:
                            break;
                    }
                }
            }

            StringBuilder temp = new StringBuilder();
            foreach (string tabName in objs.Keys)
            {

                var ty = objs[tabName].Type;
                if (!objs2.ContainsKey(tabName))
                {
                    //应该删除
                    switch (ty)
                    {
                        case StObjectType.U:
                            break;
                        case StObjectType.V:
                            break;
                        case StObjectType.PK:
                            break;
                        case StObjectType.C:
                            break;
                        case StObjectType.D:
                            break;
                        case StObjectType.P:
                            delproc.AppendFormat("drop proc [{0}]\r\n", tabName);
                            addproc.AppendLine();
                            delproc.AppendLine("go");
                            break;
                        case StObjectType.TR:
                            deltr.AppendFormat("drop trigger [{0}]\r\n", tabName);
                            addproc.AppendLine();
                            deltr.AppendLine("go");
                            break;
                        case StObjectType.F:
                            break;
                        case StObjectType.TT:
                            break;
                        case StObjectType.IF:
                            break;
                        case StObjectType.UQ:
                            break;
                        case StObjectType.L:
                            break;
                        case StObjectType.X:
                            break;
                        case StObjectType.TF:
                            break;
                        case StObjectType.S:
                            break;
                        case StObjectType.RF:
                            break;
                        case StObjectType.FN:
                            break;
                        default:
                            break;
                    }
                }


                if (objs2.ContainsKey(tabName))
                {
                    //  更新   这里先删除再添加
                    //仅限于视图 存储过程 触发器等依附于表之上 重新建立不影响数据业务的
                    bool jin = false;
                    if (ty == StObjectType.P || ty == StObjectType.TR)
                    {

                        switch (ty)
                        {
                            //case StObjectType.U:
                            //    break;
                            case StObjectType.V:
                                break;
                            case StObjectType.PK:
                                break;
                            case StObjectType.C:
                                break;
                            case StObjectType.D:
                                break;
                            case StObjectType.P:
                                DataTable dtp = DBHelper.QueryDataTable(connStr2, "sp_helptext '" + tabName + "'");
                                DataRowCollection dtp2 = DBHelper.QueryDataTable(connStr1, "sp_helptext '" + tabName + "'").Rows;
                                temp_i = -1;
                                jin = dtp2.Count != dtp.Rows.Count;
                                temp = new StringBuilder();
                                foreach (DataRow item in dtp.Rows)
                                {
                                    temp_i ++;
                                    temp_str = item[0] + "";
                                    if (temp_i == 0 && temp_str.Trim().ToLower().IndexOf("create") == 0)
                                    {
                                        temp_str = temp_str.Trim();
                                        temp_str = "alter" + temp_str.Substring("create".Length) + "\r\n";
                                    }
                                    temp.Append(temp_str);
                                    if (!jin && dtp2[temp_i][0] + "" != item[0] + "")
                                    {
                                        jin = true;
                                    }

                                }
                                if (jin)
                                {
                                    updproc.Append(temp.ToString());
                                    addproc.AppendLine();
                                    updproc.AppendLine("go");
                                    updproc.AppendLine();
                                    updproc.AppendLine();
                                }
                                break;
                            case StObjectType.TR:
                                //updtr.AppendFormat("drop trigger [{0}]\r\n", tabName);
                                //updtr.AppendLine("go");
                                //updtr.AppendLine();
                                DataTable dttr = DBHelper.QueryDataTable(connStr2, "sp_helptext '" + tabName + "'");
                                DataRowCollection dttr2 = DBHelper.QueryDataTable(connStr1, "sp_helptext '" + tabName + "'").Rows;
                                temp_i = -1;
                                jin = dttr.Rows.Count!= dttr2.Count;
                                temp = new StringBuilder();
                                foreach (DataRow item in dttr.Rows)
                                {

                                    temp_i ++;

                                    temp_str = item[0] + "";
                                    if (temp_i == 0 && temp_str.Trim().ToLower().IndexOf("create") == 0)
                                    {
                                        temp_str = temp_str.Trim();
                                        temp_str = "alter" + temp_str.Substring("create".Length) + "\r\n";
                                    }
                                    temp.Append(temp_str);

                                    if (!jin && dttr2[temp_i][0] + "" != item[0] + "")
                                    {
                                        jin = true;
                                    }

                                }
                                if (jin)
                                {
                                    updtr.Append(temp.ToString());
                                    updtr.AppendLine("go");
                                    updtr.AppendLine();
                                    updtr.AppendLine();
                                }
                                break;
                            case StObjectType.F:
                                break;
                            case StObjectType.TT:
                                break;
                            case StObjectType.IF:
                                break;
                            case StObjectType.UQ:
                                break;
                            case StObjectType.L:
                                break;
                            case StObjectType.X:
                                break;
                            case StObjectType.TF:
                                break;
                            case StObjectType.S:
                                break;
                            case StObjectType.RF:
                                break;
                            case StObjectType.FN:
                                break;
                            default:
                                break;
                        }

                        continue;

                    }
                }
            }
            #endregion


            #region 添加

            foreach (string tabName in fss2.Keys)
            {

                if (!fss.ContainsKey(tabName))
                {
                    //应该加
                    #region MyRegion

                    isdeltab = true;
                    switch (fss2[tabName][0].ObjType)
                    {
                        case StObjectType.P:
                            break;
                        case StObjectType.TR:

                            break;
                        case StObjectType.U:

                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("CREATE TABLE [dbo].[{0}](\r\n", tabName);
                            int temp_i = -1;
                            foreach (StObjField item in fss2[tabName])
                            {
                                temp_i++;
                                sb.AppendLine("\t" + (temp_i > 0 ? "," : "") + DB_GetFieldStr(item));
                            }
                            string temp_str = DB_GetPrimaryKeyFields(fss2[tabName]);
                            if (!string.IsNullOrWhiteSpace(temp_str))
                            {
                                sb.AppendLine("\t," + temp_str);
                            }
                            sb.AppendLine(")");
                            sb.AppendLine("go");

                            foreach (StObjField item in fss2[tabName])
                            {
                                sb.AppendLine(DB_GetFOREIGNKEYSQL(tabName, item, fss2));
                            }
                            sb.AppendLine(DB_GetCHECKSQL(tabName));


                            foreach (StObjField item in fss2[tabName])
                            {
                                sb.AppendLine(DB_GetDefaultSQL(item));
                            }


                            sb.AppendLine(DB_GetObjDescribeSQL(fss2[tabName][0]));
                            foreach (StObjField item in fss2[tabName])
                            {
                                sb.AppendLine(DB_GetDescribeSQL(item));
                            }
                            addtab.AppendLine(sb.ToString());
                            break;
                        case StObjectType.V:
                            string connStringv = DBHelper.GetLineStr(this.txtServerAddress2.Text, this.comBoxDBList2.Text, this.txtuserName2.Text, this.txtpassWord2.Text);
                            DataTable dtv = DBHelper.QueryDataTable(connStringv, "sp_helptext '" + tabName + "'");
                            foreach (DataRow item in dtv.Rows)
                            {
                                if (string.IsNullOrWhiteSpace(item[0] + "")) { continue; }
                                addview.Append(item[0] + "");
                            }
                            addview.AppendLine();
                            addview.AppendLine("go");

                            break;
                        //case StObjectType.PK:
                        //    break;
                        //case StObjectType.C:
                        //    break;
                        //case StObjectType.D:
                        //    break;
                        //case StObjectType.F:
                        //    break;
                        //case StObjectType.TT:
                        //    break;
                        //case StObjectType.IF:
                        //    break;
                        //case StObjectType.UQ:
                        //    break;
                        //case StObjectType.L:
                        //    break;
                        //case StObjectType.X:
                        //    break;
                        //case StObjectType.TF:
                        //    break;
                        //case StObjectType.S:
                        //    break;
                        //case StObjectType.RF:
                        //    break;
                        //case StObjectType.FN:
                        //    break;
                        default:
                            break;
                    }
                    continue;
                    #endregion
                }
            }

            #endregion

            #region 更新和删除

            foreach (string tabName in fss.Keys)
            {
                StObjectType tabType = fss[tabName][0].ObjType;

                if (fss2.ContainsKey(tabName))
                {
                    //  更新   这里先删除再添加
                    #region 更新对象

                    //仅限于视图 存储过程 触发器等依附于表之上 重新建立不影响数据业务的
                    var ty=fss[tabName][0].ObjType;
                    bool jin = false;
                    if (ty == StObjectType.V || ty == StObjectType.TR)
                    {

                        switch (ty)
                        {
                            //case StObjectType.U:
                            //    break;
                            case StObjectType.V:
                                //updview.AppendFormat("drop view [{0}]\r\n", tabName);
                                //updview.AppendLine("go");
                                //updview.AppendLine();
                                DataTable dt = DBHelper.QueryDataTable(connStr2, "sp_helptext '" + tabName + "'");
                                DataRowCollection dt_dr2 = DBHelper.QueryDataTable(connStr1, "sp_helptext '" + tabName + "'").Rows;


                                jin = dt.Rows.Count != dt_dr2.Count;
                                temp_i = -1;
                                temp = new StringBuilder();
                                foreach (DataRow item in dt.Rows)
                                {

                                    temp_i ++;

                                    temp_str = item[0] + "";
                                    if (temp_i == 0 && temp_str.Trim().ToLower().IndexOf("create") == 0)
                                    {
                                        temp_str = temp_str.Trim();
                                        temp_str = "alter" + temp_str.Substring("create".Length) + "\r\n";
                                    }
                                    temp.Append(temp_str);

                                    if (!jin && dt_dr2[temp_i][0] + "" != item[0] + "")
                                    {
                                        jin = true;
                                    }
                                }
                                if (jin)
                                {
                                    updview.Append(temp.ToString());
                                    updview.AppendLine();
                                    updview.AppendLine("go");
                                    updview.AppendLine();
                                    updview.AppendLine();
                                }
                                break;
                            case StObjectType.PK:
                                break;
                            case StObjectType.C:
                                break;
                            case StObjectType.D:
                                break;
                            case StObjectType.P:
                                break;
                            case StObjectType.TR:
                                break;
                            case StObjectType.F:
                                break;
                            case StObjectType.TT:
                                break;
                            case StObjectType.IF:
                                break;
                            case StObjectType.UQ:
                                break;
                            case StObjectType.L:
                                break;
                            case StObjectType.X:
                                break;
                            case StObjectType.TF:
                                break;
                            case StObjectType.S:
                                break;
                            case StObjectType.RF:
                                break;
                            case StObjectType.FN:
                                break;
                            default:
                                break;
                        }
                        continue;

                    }
                    #endregion
                }


                if (!fss2.ContainsKey(tabName))
                {
                    #region 删除对象
                    isaddtab = true;
                    switch (tabType)
                    {
                        case StObjectType.U:
                            deltab.AppendFormat("drop table [{0}]\r\n", tabName);
                            deltab.AppendLine("go");
                            deltab.AppendLine();
                            break;
                        case StObjectType.V:
                            delview.AppendFormat("drop view [{0}]\r\n", tabName);
                            delview.AppendLine("go");
                            delview.AppendLine();
                            break;
                        case StObjectType.PK:
                            break;
                        case StObjectType.C:
                            break;
                        case StObjectType.D:
                            break;
                        case StObjectType.P:
                            delproc.AppendFormat("drop proc [{0}]\r\n", tabName);
                            delproc.AppendLine("go");
                            delproc.AppendLine();
                            break;
                        case StObjectType.TR:
                            deltr.AppendFormat("drop trigger [{0}]\r\n", tabName);
                            deltr.AppendLine("go");
                            deltr.AppendLine();
                            break;
                        case StObjectType.F:
                            break;
                        case StObjectType.TT:
                            break;
                        case StObjectType.IF:
                            break;
                        case StObjectType.UQ:
                            break;
                        case StObjectType.L:
                            break;
                        case StObjectType.X:
                            break;
                        case StObjectType.TF:
                            break;
                        case StObjectType.S:
                            break;
                        case StObjectType.RF:
                            break;
                        case StObjectType.FN:
                            break;
                        default:
                            break;
                    }
                    continue;


                    #endregion
                }


                List<StObjField> f1 = fss[tabName];

                List<StObjField> f2 = fss2[tabName];
                List<string> fs1 = f1.Select(i => i.ColName).ToList();
                List<string> fs2 = f2.Select(i => i.ColName).ToList();


                List<string> rel = new List<string>();

                if (tabType == StObjectType.U)
                {
                    //删除字段
                    #region MyRegion
                    
                    temp = new StringBuilder();
                    //目标(f2)比初始(f1)少的   应该删除
                    foreach (StObjField field1 in f1)
                    {
                        if (!fs2.Contains(field1.ColName,StringComparer.OrdinalIgnoreCase) && field1.ObjType == StObjectType.U)
                        {
                            temp.AppendLine("alter table [" + field1.ObjName + "] drop column [" + field1.ColName + "]");
                            temp.AppendLine("go");
                        }
                    }
                    if (temp.Length > 0)
                    {
                        isdel = true;
                        del.AppendLine("-- [" + tabName + "]删除的字段");
                        del.AppendLine(temp.ToString());
                        del.AppendLine();
                        del.AppendLine();
                    }
                    #endregion
                }


                if (tabType == StObjectType.U)
                {
                    //增加字段
                    #region MyRegion
                    
                    temp = new StringBuilder();
                    //目标(f2)比初始(f1)多的   应该加
                    foreach (StObjField field2 in f2)
                    {
                        if (!fs1.Contains(field2.ColName,StringComparer.OrdinalIgnoreCase))
                        {
                            if (field2.ObjType == StObjectType.U)
                            {
                                temp.AppendLine("alter table [" + field2.ObjName + "] add " + DB_GetFieldStr(field2));
                                temp.AppendLine("go");
                                //添加字段的默认值
                                temp.AppendLine(DB_GetDefaultSQL(field2));
                                //添加说明
                                temp.AppendLine(DB_GetDescribeSQL(field2));
                                

                            }
                        }
                    }
                    if (temp.Length > 0)
                    {
                        isadd = true;
                        add.AppendLine("-- [" + tabName + "]增加的字段");
                        add.AppendLine(temp.ToString());
                        add.AppendLine();
                        add.AppendLine();
                    }
                    #endregion
                }



                if (tabType == StObjectType.U)
                {
                    //更新字段
                    #region MyRegion

                    temp = new StringBuilder();
                    //
                    foreach (StObjField field1 in f1)
                    {
                        foreach (StObjField field2 in f2)
                        {
                            if (string.Equals( field1.ColName , field2.ColName,StringComparison.OrdinalIgnoreCase))
                            {
                                if (DB_GetFieldStr(field1) != DB_GetFieldStr(field2))
                                {
                                    temp.AppendLine("-- 字段[" + field2.ColName + "]的类型改为【" + DB_GetFieldStr(field2) + "】");
                                    temp.AppendLine(string.Format("alter table [{0}] alter column {1}", field1.ObjName, DB_GetFieldStr(field2)));
                                    temp.AppendLine("go");
                                }

                                if (field1.Describe != field2.Describe)
                                {
                                    if (field1.Describe != null && field2.Describe != null)
                                    {
                                        //是更改
                                        temp.AppendLine("-- 字段[" + field2.ColName + "]的说明改为【" + field2.Describe + "】");
                                        temp.AppendFormat("exec sp_updateextendedproperty 'MS_Description','{0}','SCHEMA',dbo,'table','{1}','column','{2}'\r\ngo\r\n", field2.Describe.Replace("'", "''"), field2.ObjName, field2.ColName);
                                    }
                                    else
                                    {
                                        if (field1.Describe == null)
                                        {
                                            //是添加说明
                                            temp.AppendLine(string.Format(@"exec sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{2}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{0}', @level2type=N'COLUMN',@level2name=N'{1}'
go", field1.ObjName, field1.ColName, field2.Describe.Replace("'","''")));
                                        }
                                        else if (field2.Describe == null)
                                        {
                                            //是删除描述
                                            temp.AppendFormat("exec sp_dropextendedproperty 'MS_Description','user',dbo,'table','{0}','column',{1} \r\ngo\r\n", field1.ObjName, field1.ColName);
                                        }
                                    }
                                }
                                if (field1.DefaultValue != field2.DefaultValue)
                                {
                                    if (field1.DefaultValue != null && field2.DefaultValue != null)
                                    {
                                        //是更改
                                        temp.AppendLine("-- 字段[" + field2.ColName + "]的默认改为【" + field2.DefaultValue + "】");
                                        //先查询约束名  删除后再添加
                                        string connString = DBHelper.GetLineStr(this.txtServerAddress.Text, this.comBoxDBList.Text, this.txtuserName.Text, this.txtpassWord.Text);
                                        DataTable dt = DBHelper.QueryDataTable(connString, "select name from sysobjects where id='" + field1.DefaultID + "'");
                                        if (dt.Rows.Count == 1 && !(dt.Rows[0][0] is DBNull))
                                        {
                                            string n = dt.Rows[0][0] + "";
                                            temp.AppendFormat("alter table [{0}] drop constraint [{1}] \r\ngo\r\n", field1.ObjName, n);
                                            temp.AppendFormat("alter table [{0}] add default{1} for [{2}] \r\ngo\r\n", field1.ObjName, field2.DefaultValue, field1.ColName);
                                        }
                                    }
                                    else
                                    {
                                        if (field1.DefaultValue == null)
                                        {
                                            //是添加默认值
                                            temp.AppendFormat("alter table [{0}] add default{1} for [{2}] \r\ngo\r\n", field1.ObjName, field2.DefaultValue, field1.ColName);
                                        }
                                        else if (field2.DefaultValue == null)
                                        {
                                            //EXEC   sp_dropextendedproperty   'MS_Description','user',dbo,'table','表','column',a1  



                                            //需要先查询原约束名
                                            string connString = DBHelper.GetLineStr(this.txtServerAddress.Text, this.comBoxDBList.Text, this.txtuserName.Text, this.txtpassWord.Text);
                                            DataTable dt = DBHelper.QueryDataTable(connString, "select name from sysobjects where id='" + field1.DefaultID + "'");
                                            if (dt.Rows.Count == 1 && !(dt.Rows[0][0] is DBNull))
                                            {
                                                string n = dt.Rows[0][0] + "";
                                                temp.AppendFormat("alter table [{0}] drop constraint [{1}]\r\ngo\r\n", field1.ObjName, n);
                                            }

                                        }
                                    }
                                }
                                if (field1.IsCheck != field2.IsCheck || field1.IDENTITY_INCREASE != field2.IDENTITY_INCREASE || field1.IDENTITY_SEED != field2.IDENTITY_SEED
                                    || field1.IsIdentity != field2.IsIdentity || field1.IsPrimaryKey != field2.IsPrimaryKey
                                    || field1.IsUnique != field2.IsUnique || field1.CheckText != field2.CheckText)
                                {
                                    temp.AppendLine("-- [" + field2.ColName + "]请仔细核对该字段的其他信息");
                                }

                                break;
                            }
                        }
                    }

                    if (temp.Length > 0)
                    {
                        isupd = true;
                        upd.AppendLine("-- [" + tabName + "]修改的字段");
                        upd.AppendLine(temp.ToString());
                        upd.AppendLine();
                        upd.AppendLine();
                    }
                    #endregion
                }


            }

            #endregion



            //需要刷新的视图
            #region 需要刷新的视图

            //sp_depends 'BMC_SOrder' 
            DataTable dtrefView = null;
                //如果表只更改了数据类型或者是否可为空就会报错  应该加
            temp_str = @"select distinct OBJECT_NAME(d.depid) parentName,par.type parentType, o.name 'name',o.type
from sysdepends d,sys.objects o,sys.objects par
where o.object_id = d.id and par.object_id = d.depid and deptype < 2 and par.type ='U' and o.type ='V' and o.modify_date<par.modify_date";
            dtrefView = DBHelper.QueryDataTable(connStr1, temp_str);

            foreach (DataRow row in dtrefView.Rows)
            {
                refView.AppendFormat("sp_refreshview '{0}'", row["name"]);
                refView.AppendLine();
                refView.AppendLine("go");
            }
            dtrefView = DBHelper.QueryDataTable(connStr2, temp_str);

            foreach (DataRow row in dtrefView.Rows)
            {
                refView2.AppendFormat("sp_refreshview '{0}'", row["name"]);
                refView2.AppendLine();
                refView2.AppendLine("go");
            }
            temp_str = "";

            #endregion



            ChangeCurrState("成功");
            string filePath = string.Format(@"result/'{0}' 到 '{1}'", dbName, dbName2);
            string filePath_temp = string.Format(@"result/'{0}' 到 '{1}'", dbName, dbName2);
            int iii = 0;
            while (true)
            {
                if (iii > 0)
                {
                    filePath = filePath_temp + "_" + iii;
                }
                iii++;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                    break;
                }
            }
            string rrr = "";


            //当有时间限制时 无法比较删除信息
            if (cbox_tr.Checked && sTime.HasValue == false)
            {
                File.AppendAllText(filePath + "/0.0 删除触发器.txt", deltr.ToString());
                rrr += deltr.ToString();
            }
            if (cbox_view.Checked && sTime.HasValue == false)
            {
                File.AppendAllText(filePath + "/0.1 删除视图.txt", delview.ToString());
                rrr += delview.ToString();
            }
            if (cbox_proc.Checked && sTime.HasValue == false)
            {
                File.AppendAllText(filePath + "/0.2 删除存储过程.txt", delproc.ToString());
                rrr += delproc.ToString();
            }

            if (cbox_u.Checked)
            {
                File.AppendAllText(filePath + "/0.3 添加表.txt", addtab.ToString());
                rrr += addtab.ToString();
            }

            if (cbox_u.Checked)
            {
                File.AppendAllText(filePath + "/0.4 删除字段.txt", del.ToString());
                rrr += del.ToString();
            }
            if (cbox_u.Checked)
            {
                File.AppendAllText(filePath + "/1.0 更新字段.txt", upd.ToString());
                rrr += upd.ToString();
            }

            if (cbox_u.Checked)
            {
                File.AppendAllText(filePath + "/1.1 添加字段.txt", add.ToString());
                rrr += add.ToString();
            }
            if (cbox_view.Checked)
            {
                File.AppendAllText(filePath + "/1.2 添加视图.txt", addview.ToString());
                rrr += addview.ToString();
            }
            if (cbox_proc.Checked)
            {
                File.AppendAllText(filePath + "/1.3 添加存储过程.txt", addproc.ToString());
                rrr += addproc.ToString();
            }

            if (cbox_tr.Checked)
            {
                File.AppendAllText(filePath + "/1.4 添加触发器.txt", addtr.ToString());
                rrr += addtr.ToString();
            }

            if (cbox_proc.Checked)
            {
                File.AppendAllText(filePath + "/2.0 更新存储过程.txt", updproc.ToString());
                rrr += updproc.ToString();
            }
            if (cbox_tr.Checked)
            {
                File.AppendAllText(filePath + "/2.1 更新触发器.txt", updtr.ToString());
                rrr += updtr.ToString();
            }


            if (cbox_u.Checked && sTime.HasValue == false)
            {
                File.AppendAllText(filePath + "/3 删除表.txt", deltab.ToString());
                rrr += deltab.ToString();
            }

            File.AppendAllText(filePath + "/7.1 更新(当前数据库)相关视图-未纳入all.txt", refView.ToString());
            File.AppendAllText(filePath + "/7.2 更新(目标数据库)相关视图.txt", refView2.ToString());
            rrr += refView2.ToString();

            File.AppendAllText(filePath + "/9 all.txt", rrr);
        }

        /// <summary>
        /// 获取说明SQL
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string DB_GetCHECKSQL(string tabName)
        {
            string rel = "";
            List<StObject> ches = StObjects_1.Where(i => i.ObjName == tabName).ToList();
            if (ches.Count == 0) { return ""; }
            foreach (StObject item in ches)
            {
                rel += string.Format(@"
alter table [{0}] add constraint [{1}] check{2}
go
", tabName, item.Name, item.CreateText);
            }
            return rel.Trim('\r', '\n');
        }

        /// <summary>
        /// 生成外键SQL  未完成
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string DB_GetFOREIGNKEYSQL(string tabName,StObjField f,Dictionary<string, List<StObjField>> fss)
        {
            if (f == null) { return ""; }
            if (string.IsNullOrWhiteSpace(f.PrientTableName)) { return ""; }
            string rel = "";

            if (!fss.ContainsKey(f.PrientTableName)) { return string.Format("--表【{0}】没在生成的所有表中", f.PrientTableName); }
            //if (fss == null) { return ""; }
            StObjField f_pk = fss[f.PrientTableName].Where(i => f.PrientTableID == i.ObjID && f.ParentColID == i.ColID).FirstOrDefault();
            //if (f_pk == null) { return ""; }



            //            rel += string.Format(@"ALTER TABLE [dbo].[{0}]  WITH CHECK ADD  CONSTRAINT [FK_{0}_{2}] FOREIGN KEY([{1}])
            //REFERENCES [dbo].[{2}] ([ID])
            //GO
            //ALTER TABLE [dbo].[{0}] CHECK CONSTRAINT [FK_{0}_{2}]
            //GO", tabName, f.ColName, f.PrientTableName, f_pk.ColName);
            rel += string.Format(@"ALTER TABLE [dbo].[{0}]  WITH CHECK ADD  CONSTRAINT [FK_{0}_{2}] FOREIGN KEY([{1}])
REFERENCES [dbo].[{2}] ([{3}])
GO", tabName, f.ColName, f.PrientTableName, f_pk.ColName);
            return rel == "" ? "" : "\r\n" + rel + "\r\n";
        }
        /// <summary>
        /// 获取默认值SQL
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string DB_GetDefaultSQL(StObjField f)
        {
            //string tabName = args[0] + "";
            string rel = "";
            //StObjField f = args[1] as StObjField;
            if (f == null) { return ""; }
            if (f.DefaultValue == null) { return ""; }

            rel += string.Format(@"ALTER TABLE [dbo].[{0}] ADD  CONSTRAINT [DF_{0}_{1}]  DEFAULT {2} FOR [{1}]
GO", f.ObjName, f.ColName, f.DefaultValue);
            return rel == "" ? "" :  rel ;
        }
        /// <summary>
        /// 获取表说明SQL
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string DB_GetObjDescribeSQL(StObjField f)
        {
            string rel = "";
            //StObjField f = args[0] as StObjField;
            if (f == null) { return ""; }
            if (f.ObjDescribe == null) { return ""; }

            rel += string.Format(@"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{1}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{0}'
GO", f.ObjName, f.ObjDescribe.Replace("'","''"));
            return rel == "" ? "" :  rel ;
        }
        /// <summary>
        /// 获取说明SQL
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string DB_GetDescribeSQL(StObjField f)
        {
            string rel = "";
            //StObjField f = args[0] as StObjField;
            if (f == null) { return ""; }
            if (f.Describe == null) { return ""; }

            rel += string.Format(@"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{2}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{0}', @level2type=N'COLUMN',@level2name=N'{1}'
GO", f.ObjName, f.ColName, f.Describe.Replace("'", "''"));
            return rel == "" ? "" :  rel ;
        }
//        /// <summary>
//        /// 获取说明SQL
//        /// </summary>
//        /// <param name="args"></param>
//        /// <returns></returns>
//        private static string DB_GetCHECKSQL(object[] args)
//        {
//            string tabName = args[0] + "";
//            string rel = "";
//            List<StObject> ches = StObjects_1.Where(i => i.ObjName == tabName).ToList();
//            if (ches.Count == 0) { return ""; }
//            foreach (StObject item in ches)
//            {
//                rel += string.Format(@"
//alter table [{0}] add constraint [{1}] check{2}
//go
//", tabName, item.Name, item.CreateText);
//            }
//            return rel.Trim('\r', '\n');
//        }
        /// <summary>
        /// 获取主键列
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string DB_GetPrimaryKeyFields(List<StObjField> f)
        {
            //List<StObjField> f = args[0] as List<StObjField>;
            if (f == null) { return ""; }
            var primarys = f.Where(i => i.IsPrimaryKey).Select(i => i.ColName);
            if (primarys.Count() == 0) { return ""; }
            return string.Format("primary key({0})", string.Join(",", primarys));
        }
        private static string DB_GetFieldStr(StObjField f,bool isOlayType=false)
        {
            //[{$field.ColName}] [{$field.ColTypeName}]{$iif(field.IsIdentity," IDENTITY(1,1) ","")} {$iif(field.IsNull,"NULL","NOT NULL")}（）
            //StObjField f = args[0] as StObjField;
            if (f == null) { return ""; }
            StringBuilder rel = new StringBuilder();
            rel.AppendFormat("[{0}]", f.ColName);
            if (f.IsComputed)
            {
                rel.AppendFormat(" as {0}", f.ComputedText);
                return rel.ToString();
            }
            rel.AppendFormat(" [{0}]", f.ColTypeName);
            switch (f.ColTypeName.ToLower())
            {
                case "varchar":
                case "nvarchar":
                case "char":
                case "nchar":
                //case "time":
                case "varbinary":
                //case "datetime2":
                case "binary":
                case "datetimeoffset":
                    rel.AppendFormat("({0})", f.OldLength < 0 ? "MAX" : "" + f.OldLength);
                    break;
                case "numeric":
                case "decimal":
                    rel.AppendFormat("({0},{1})", f.OldLength, f.Scale);
                    break;
                default:
                    break;
            }
            if (!isOlayType)
            {
                if (f.IsIdentity)
                    rel.AppendFormat(" IDENTITY({0},{1})", f.IDENTITY_SEED, f.IDENTITY_INCREASE);
                if (f.IsUnique)
                {
                    rel.Append(" UNIQUE");
                }
                rel.Append(f.IsNull ? " NULL" : " NOT NULL");
            }
            return rel.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string temp = "";
            bool temp_bool = false;

            temp = this.txtuserName2.Text;
            this.txtuserName2.Text = this.txtuserName.Text;
            this.txtuserName.Text = temp;

            temp = this.txtServerAddress2.Text;
            this.txtServerAddress2.Text = this.txtServerAddress.Text;
            this.txtServerAddress.Text = temp;

            temp = this.txtpassWord2.Text;
            this.txtpassWord2.Text = this.txtpassWord.Text;
            this.txtpassWord.Text = temp;

            temp_bool = this.checkBox3_2.Checked;
            this.checkBox3_2.Checked = this.checkBox3.Checked;
            this.checkBox3.Checked = temp_bool;




            dbName = this.comBoxDBList.Text;
            dbName2 = this.comBoxDBList2.Text;


            object temp_obj;

            temp_obj = this.comBoxDBList.DataSource;
            this.comBoxDBList.DataSource = null;
            this.comBoxDBList.DataSource = this.comBoxDBList2.DataSource;
            this.comBoxDBList2.DataSource = null;
            this.comBoxDBList2.DataSource = temp_obj;

            try
            {
                this.comBoxDBList.Text = dbName2;
                this.comBoxDBList2.Text = dbName;
            }
            catch (Exception)
            {
                
            }
        }






    }
}




