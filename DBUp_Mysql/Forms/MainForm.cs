using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUp_Mysql.Forms;

namespace DBUp_Mysql
{

    public partial class MainForm : Form
    {
        Config config = new Config();

        Setting cs = new Setting();
        PathSetting pathCs = new PathSetting();
        PathSetting oldpathCs = new PathSetting();
        PathSetting newpathCs = new PathSetting();
        PathSetting diffpathCs = new PathSetting();
        
        DBConnection oldConn = new DBConnection();
        DBConnection newConn = new DBConnection();

        
        Dictionary<string, DBDataSource> sourceList = new Dictionary<string, DBDataSource>();
        DBDataSource oldDataSource = null;
        DBDataSource newDataSource = null;
        public Dictionary<string, DBDataSource> ReloadDataSource()
        {
            var rel = new Dictionary<string, DBDataSource>();
            rel.Add("【空】", new DBDataSource() { Key = "【空】", Type = DBDataSourceType.Empty, Value = "" });


            var dbSource = ConfigHelper.GetConnections();
            var fileSourcce = Tools.GetConnections();

            foreach (var item in dbSource)
            {
                rel.Add(item.Key, item.Value);
            }
            foreach (var item in fileSourcce)
            {
                rel.Add(item.Key, item.Value);
            }
            return rel;
        }
        public MainForm()
        {
            InitializeComponent();
        }
        public void SetPathSetting(PathSetting cs, string targ)
        {

            cs.Tables = targ + pathCs.Tables;
            cs.Funcs = targ + pathCs.Funcs;
            cs.Procs = targ + pathCs.Procs;
            cs.Trigs = targ + pathCs.Trigs;
            cs.Views = targ + pathCs.Views;
            cs.DBSetting = targ + pathCs.DBSetting;
        }

        public void f_reloadDataSource()
        {
            #region 加载数据源

            int inx = -1;
            int oldInx = -1;
            int newInx = -1;
            sourceList = ReloadDataSource();
            //this.ddlOldDb.DataSource = sourceList.Values;
            foreach (var item in sourceList.Values)
            {
                inx++;
                this.ddlOldDb.Items.Add(item.Key);
                this.ddlNewDb.Items.Add(item.Key);
                if ("old".Equals(item.Key, StringComparison.OrdinalIgnoreCase))
                {
                    oldInx = inx;
                    this.ddlOldDb.SelectedIndex = inx;
                }
                else if ("new".Equals(item.Key, StringComparison.OrdinalIgnoreCase))
                {
                    newInx = inx;
                    this.ddlNewDb.SelectedIndex = inx;
                }
            }

            if (oldInx < 0 || newInx < 0)
            {
                if (this.ddlOldDb.Items.Count > 0) this.ddlOldDb.SelectedIndex = 0;
                if (this.ddlNewDb.Items.Count > 0) this.ddlNewDb.SelectedIndex = 0;
            }
            #endregion
        }
        private void MainForm_Load(object sender, EventArgs e)
        {

            //{
            //    var temp_list = new List<string>();
            //    for (int i = 0; i < 10000; i++)
            //    {
            //        temp_list.Add("test " + i);
            //    }

            //    var temp_asyncHelper = new AsyncHelper<string, string>(50, temp_list);

            //    temp_asyncHelper.Action = delegate (string name)
            //    {
            //        if (temp_asyncHelper.Get().Count % 100 == 0)
            //            System.Threading.Thread.Sleep(100);
            //        return "";
            //    };

            //    //temp_asyncHelper.SuccessCountHander=delegate(int c)

            //    temp_asyncHelper.Start(true);

            //    //while (true)
            //    //{
            //    //    if (temp_asyncHelper.IsAllAbort())
            //    //    {
            //    //        break;
            //    //    }
            //    //    System.Threading.Thread.Sleep(100);
            //    //}

            //    var aaa = temp_asyncHelper.Get();

            //    MessageBox.Show($"Result = {temp_list.Count == aaa.Count}    temp_list.Count={temp_list.Count} , aaa.Count={aaa.Count}");

            //    return;
            //}

            lblTotalTime.Visible = false;
            //加载数据源
            f_reloadDataSource();


            SetPathSetting(oldpathCs, "old");
            SetPathSetting(newpathCs, "new");
            SetPathSetting(diffpathCs, "diff");

            //newConn.ProviderName = ConfigurationManager.ConnectionStrings["new"].ProviderName;
            //oldConn.ProviderName = ConfigurationManager.ConnectionStrings["old"].ProviderName;

            config.NewPathSetting = newpathCs;
            config.OldPathSetting = oldpathCs;
            config.DiffPathSetting = diffpathCs;
            config.NewConnection = newConn;
            config.OldConnection = oldConn;
            config.Setting = cs;


            JsonSerializerSettings setting = new JsonSerializerSettings();
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                ////日期类型默认格式化处理
                //setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss zzzz";

                //缩进
                setting.Formatting = Formatting.Indented;

                //空值处理
                //setting.NullValueHandling = NullValueHandling.Ignore;

                //setting.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

                return setting;
            });


            //从配置文件加载Setting
            if (File.Exists(Tools.CurrentDirectory + "/Setting.txt"))
            {
                try
                {
                    cs = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Tools.CurrentDirectory + "/Setting.txt"));
                }
                catch (Exception)
                {
                    MessageBox.Show("配置文件(Setting.txt)获取出错，请联系管理员", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            this.Text = string.Format(this.Tag + "", config.Ver);
            this.cheComm.Checked = cs.CheckCommon;
            this.xhkOutComment.Checked = cs.OutputComment;
            this.chkOutDeleteSql.Checked = cs.OutputDeleteSql;
            this.chkOutDeleteSqlIsCommon.Checked = cs.OutputDeleteSqlIsCommon;

            this.chkDiffFunc.Checked = cs.IsSearFunc;
            this.chkDiffTable.Checked = cs.IsSearTable;
            this.chkDiffProc.Checked = cs.IsSearProc;
            this.chkDiffTrigger.Checked = cs.IsSearTri;
            this.chkDiffView.Checked = cs.IsSearView;
            this.textBox2.Text = Math.Max(cs.TaskCount, 1).ToString();

            totalTime = new Timer();
            totalTime.Interval = 500;
            totalTime.Tick += TotalTime_Tick;


            //this.btnExchangeDataSource.
        }

        private void TotalTime_Tick(object sender, EventArgs e)
        {
            SetTotalTime();
        }

        System.Threading.Thread t = null;
        private void btnCompare_Click(object sender, EventArgs e)
        {
            
            //Dictionary<string, string> aaa = new Dictionary<string, string>();
            //aaa.Add("stu", "1");
            //aaa.Add("Id", "1");
            //aaa.Add("Age", "1");
            //foreach (var item in aaa)
            //{
            //    MessageBox.Show(item.Key);
            //}
            //return;

            if (this.ddlOldDb.SelectedItem == null || this.ddlNewDb.SelectedItem == null)
            {
                MessageBox.Show("请选择数据库", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (this.ddlOldDb.SelectedItem.ToString() == this.ddlNewDb.SelectedItem.ToString())
            {

                MessageBox.Show("新旧数据库不能重复", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            oldDataSource = sourceList.Values.FirstOrDefault(i => i.Key == this.ddlOldDb.SelectedItem.ToString());
            newDataSource = sourceList.Values.FirstOrDefault(i => i.Key == this.ddlNewDb.SelectedItem.ToString());


            newConn.ProviderName = newDataSource.ProviderName;
            oldConn.ProviderName = oldDataSource.ProviderName;


            if (cs.IsDebug)
            {
                Start();
            }
            else
            {
                StartTh();

                //使用Task替代线程 防止界面假死
                //Task.Factory.StartNew(() =>
                //{
                //    Start();
                //});
            }
        }
        DBStructureHelper helper = null;

        Timer totalTime = null;
        DateTime startTime = default(DateTime);

        public void SetTotalTime(params object[] param)
        {
            string str = lblTotalTime.Tag + Tools.GetTimeSpan(DateTime.Now - startTime);
            if (this.InvokeRequired)
            {
                ConCallback d = new ConCallback(SetTotalTime);
                this.Invoke(d, new object[] { null });
                Application.DoEvents();
                return;
            }
            lblTotalTime.Visible = true;
            lblTotalTime.Text = str;

        }
        
        
        private void Start()
        {
            //cs.IsDebug = true;
            if (cs.IsDebug)
            {

                StartCompare();
                return;
            }
            try
            {
                StartCompare();
            }
            catch (Exception e)
            {
                try
                {
                    SetStatus(false);
                    //St.StStack stExceptionStack = new St.StStack(0);
                    //string content = stExceptionStack.ToString();
                    string content = e.ToString();
                    //StMailHelper.CheckSend("1578403183@qq.com", "DBUp_MySql", "程序出错", content.Replace("\r\n", "<br />"));
                    content = content.Insert(0, string.Format("---------------{0}---------------", DateTime.Now.ToString()));
                    string resultDir = Tools.CurrentDirectory + "/result";
                    if (!Directory.Exists(resultDir))
                        Directory.CreateDirectory(resultDir);
                    string resultStr = resultDir + string.Format("/{0}-Error.txt", DateTime.Now.ToString("yyyyMMdd"));

                    File.AppendAllText(resultStr, content);
                    MessageBox.Show(e.ToString());
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.ToString());
                }
            }
        }


        string resultStr = "";
        private void StartCompare()
        {

            config.Setting = cs;


            if (oldDataSource == null || newDataSource == null)
            {
                SetStatus(false);
                AppendOutputText("无数据源", OutputType.Error);
                MessageBox.Show("无数据源", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DbModels oldDbModels = GetEmptyDbModel(oldDataSource.Type);
            DbModels newDbModels = GetEmptyDbModel(newDataSource.Type);


            cs.CheckCommon = this.cheComm.Checked;
            cs.OutputComment = xhkOutComment.Checked;
            cs.OutputDeleteSql = chkOutDeleteSql.Checked;
            cs.OutputDeleteSqlIsCommon = chkOutDeleteSqlIsCommon.Checked;
            cs.IsSearFunc = this.chkDiffFunc.Checked;
            cs.IsSearTable = this.chkDiffTable.Checked;
            cs.IsSearProc = this.chkDiffProc.Checked;
            cs.IsSearTri = this.chkDiffTrigger.Checked;
            cs.IsSearView = this.chkDiffView.Checked;

            //线程个数
            try
            {
                cs.TaskCount = Convert.ToInt32(this.textBox2.Text);
            }
            catch (Exception)
            {
                cs.TaskCount = 1;
            }
            if (cs.TaskCount < 1)
                cs.TaskCount = 1;
            if (cs.TaskCount > 20)
                cs.TaskCount = 20;


            //处理数据源字符串
            config.OldConnection.ConnectionString = Tools.GetShowConnectionString(oldDataSource);
            config.NewConnection.ConnectionString = Tools.GetShowConnectionString(newDataSource);
            config.OldConnection.ProviderName = oldDataSource.ProviderName;
            config.NewConnection.ProviderName = newDataSource.ProviderName;

            startTime = DateTime.Now;
            SetTotalTime();

            ClearOutputText();
            //throw new Exception("testing");
            List<string> tempList = new List<string>();
            List<Function> tempFunList = new List<Function>();
            string tempStt = "";
            string tempStr = "";
            bool tempBool = false;

            SetStatus(true);

            AppendOutputText("我们正在准备一些事情，请耐心等待\n", OutputType.Comment);



            //获取数据库信息
            tempBool = true;
            DataBaseCompareAndShowResultHelper dbHelper = new DataBaseCompareAndShowResultHelper();
            dbHelper.OutputText = AppendOutputText;
            dbHelper.ReplaceLastLineText = ReplaceLastLineText;


            #region 获取数据源

            if (oldDataSource.Type == DBDataSourceType.DataSourceFile)
            {
                ////测试连接到数据库，以免报错
                //AppendOutputText("正在检查连接到新数据库的状态，请耐心等待\n", OutputType.Comment);
                //if (!helper.TestLine(10000))
                //{
                //    AppendOutputText("新数据库连接失败\n", OutputType.Comment);
                //    MessageBox.Show("新数据库连接失败", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    SetStatus(false);
                //    return;
                //}
                //else
                //{
                //    AppendOutputText("新数据库连接成功\n", OutputType.Comment);
                //}


                AppendOutputText("从文件中获取数据库信息\n", OutputType.Comment);
                tempStt = dbHelper.GetInfoByFile(oldDataSource.Value, oldpathCs.DBSetting, ref oldDbModels);
                if (!string.IsNullOrWhiteSpace(tempStt))
                {
                    tempBool = false;
                    AppendOutputText(tempStt + "\r\n", OutputType.Error);
                }
            }
            else if (oldDataSource.Type == DBDataSourceType.MySql)
            {
                AppendOutputText("从数据库中获取数据库信息\n", OutputType.Comment);
                if (!dbHelper.GetInfoByDb(oldDataSource.Value, ref oldDbModels))
                    tempBool = false;
            }
            if (newDataSource.Type == DBDataSourceType.DataSourceFile)
            {
                ////测试连接到数据库，以免报错
                //AppendOutputText("正在检查连接到旧数据库的状态，请耐心等待\n", OutputType.Comment);
                //if (!helper.TestLine(10000))
                //{
                //    AppendOutputText("旧数据库连接失败\n", OutputType.Comment);
                //    MessageBox.Show("旧数据库连接失败", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    SetStatus(false);
                //    return;
                //}
                //else
                //{
                //    AppendOutputText("旧数据库连接成功\n", OutputType.Comment);
                //}


                AppendOutputText("从文件中获取数据库信息\n", OutputType.Comment);
                tempStt = dbHelper.GetInfoByFile(newDataSource.Value, newpathCs.DBSetting, ref newDbModels);
                if (!string.IsNullOrWhiteSpace(tempStt))
                {
                    tempBool = false;
                    AppendOutputText(tempStt + "\r\n", OutputType.Error);
                }
            }
            else if (newDataSource.Type == DBDataSourceType.MySql)
            {
                AppendOutputText("从数据库中获取数据库信息\n", OutputType.Comment);
                if (!dbHelper.GetInfoByDb(newDataSource.Value, ref newDbModels))
                    tempBool = false;
            }
            #endregion

            if (tempBool)
            {
                resultStr = Tools.GetDirFullPath(string.Format("/result/{0}-{1}-{2}/", DateTime.Now.ToString("yyyyMMddHHmmss"), oldDbModels.DbModel.DbName, newDbModels.DbModel.DbName));
                cs.OutputPath = resultStr;

                File.AppendAllText(resultStr + oldpathCs.DBSetting, JsonConvert.SerializeObject(oldDbModels.DbModel));
                File.AppendAllText(resultStr + newpathCs.DBSetting, JsonConvert.SerializeObject(newDbModels.DbModel));
            }
            else
            {
                throw new Exception("获取数据库信息失败");
            }
            

            ////从网卡层面判断是否联网
            //if (!Win32API.InternetGetConnectedState(ref int tempInx, 0))
            //{
            //    AppendOutputText("请检查你的网络状态", OutputType.Error);
            //    MessageBox.Show("结束对比：请检查你的网络状态", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    SetStatus(false);
            //    return;
            //}


            AppendOutputText("\n", OutputType.None);
            File.AppendAllText(resultStr + oldpathCs.Path, JsonConvert.SerializeObject(config));

            
            if (cs.IsDiff)
            {
                dbHelper.CompareAndShow(ref oldDbModels, ref newDbModels, cs, out string errorString);

                if (string.IsNullOrEmpty(errorString) && tempBool)
                {
                    AppendOutputText("对比完毕\n\n", OutputType.Comment);
                    //compIsError = true;
                }
                else
                {
                    //string tips = string.Concat("对比中发现以下问题，请修正后重新进行比较：\n\n", errorString);
                    //MessageBox.Show(tips, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                File.AppendAllText(resultStr + diffpathCs.DBSetting, string.IsNullOrWhiteSpace(tempStr) ? RtxResult.Text : RtxResult.Text.Replace(tempStr, ""));

            }
            tempStr = RtxResult.Text;


            if (cs.IsSearTable)
            {
                tempBool = true;
                TableCompareAndShowResultHelper viewHelper = new TableCompareAndShowResultHelper(cs);
                viewHelper.OutputText = AppendOutputText;
                viewHelper.ReplaceLastLineText = ReplaceLastLineText;



                #region 获取数据源

                if (oldDataSource.Type == DBDataSourceType.DataSourceFile)
                {
                    AppendOutputText("从文件中获取表结构\n", OutputType.Comment);
                    tempStt = viewHelper.GetInfoByFile(oldDataSource.Value, oldpathCs.Tables, ref oldDbModels);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + oldpathCs.Tables, JsonConvert.SerializeObject(oldDbModels.Tables.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }
                else if (oldDataSource.Type == DBDataSourceType.MySql)
                {
                    AppendOutputText("从数据库中获取表结构\n", OutputType.Comment);
                    if (viewHelper.GetInfoByDb(oldDataSource.Value, ref oldDbModels))
                        File.AppendAllText(resultStr + oldpathCs.Tables, JsonConvert.SerializeObject(oldDbModels.Tables.Values));
                    else
                        tempBool = false;
                }
                if (newDataSource.Type == DBDataSourceType.DataSourceFile)
                {
                    AppendOutputText("从文件中获取表结构\n", OutputType.Comment);
                    tempStt = viewHelper.GetInfoByFile(newDataSource.Value, newpathCs.Tables, ref newDbModels);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + newpathCs.Tables, JsonConvert.SerializeObject(newDbModels.Tables.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }
                else if (newDataSource.Type == DBDataSourceType.MySql)
                {
                    AppendOutputText("从数据库中获取表结构\n", OutputType.Comment);
                    if (viewHelper.GetInfoByDb(newDataSource.Value, ref newDbModels))
                        File.AppendAllText(resultStr + newpathCs.Tables, JsonConvert.SerializeObject(newDbModels.Tables.Values));
                    else
                        tempBool = false;
                }

                #endregion


                if (cs.IsDiff)
                {
                    viewHelper.CompareAndShow(ref oldDbModels, ref newDbModels, cs, out string errorString);

                    if (string.IsNullOrEmpty(errorString) && tempBool)
                    {
                        AppendOutputText("对比完毕\n\n", OutputType.Comment);
                        //compIsError = true;
                    }
                    else
                    {
                        //string tips = string.Concat("对比中发现以下问题，请修正后重新进行比较：\n\n", errorString);
                        //MessageBox.Show(tips, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    File.AppendAllText(resultStr + diffpathCs.Tables, string.IsNullOrWhiteSpace(tempStr) ? RtxResult.Text : RtxResult.Text.Replace(tempStr, ""));

                }
                tempStr = RtxResult.Text;
            }


            if (cs.IsSearView)
            {

                tempBool = true;
                ViewCompareAndShowResultHelper viewHelper = new ViewCompareAndShowResultHelper();
                viewHelper.OutputText = AppendOutputText;
                viewHelper.ReplaceLastLineText = ReplaceLastLineText;

                #region 获取数据源

                if (oldDataSource.Type == DBDataSourceType.DataSourceFile)
                {
                    AppendOutputText("从文件中获取视图\n", OutputType.Comment);
                    tempStt = viewHelper.GetInfoByFile(oldDataSource.Value, oldpathCs.Views, ref oldDbModels);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + oldpathCs.Views, JsonConvert.SerializeObject(oldDbModels.Views.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }
                else if(oldDataSource.Type == DBDataSourceType.MySql)
                {
                    AppendOutputText("从数据库中获取视图\n", OutputType.Comment);
                    if (viewHelper.GetInfoByDb(oldDataSource.Value, ref oldDbModels))
                        File.AppendAllText(resultStr + oldpathCs.Views, JsonConvert.SerializeObject(oldDbModels.Views.Values));
                    else
                        tempBool = false;
                }

                if (newDataSource.Type == DBDataSourceType.DataSourceFile)
                {
                    AppendOutputText("从文件中获取视图\n", OutputType.Comment);
                    tempStt = viewHelper.GetInfoByFile(newDataSource.Value, newpathCs.Views, ref newDbModels);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + newpathCs.Views, JsonConvert.SerializeObject(newDbModels.Views.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }
                else if(newDataSource.Type == DBDataSourceType.MySql)
                {
                    AppendOutputText("从数据库中获取视图\n", OutputType.Comment);
                    if (viewHelper.GetInfoByDb(newDataSource.Value, ref newDbModels))
                        File.AppendAllText(resultStr + newpathCs.Views, JsonConvert.SerializeObject(newDbModels.Views.Values));
                    else
                        tempBool = false;
                }
                
                #endregion

                if (cs.IsDiff)
                {

                    viewHelper.CompareAndShow(ref oldDbModels, ref newDbModels, cs, out string errorString);

                    if (string.IsNullOrEmpty(errorString) && tempBool)
                    {
                        AppendOutputText("对比完毕\n\n", OutputType.Comment);
                    }
                    else
                    {
                        //string tips = string.Concat("对比中发现以下问题，请修正后重新进行比较：\n\n", errorString);
                        //MessageBox.Show(tips, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    File.AppendAllText(resultStr + diffpathCs.Views, string.IsNullOrWhiteSpace(tempStr) ? RtxResult.Text : RtxResult.Text.Replace(tempStr, ""));

                }
                tempStr = RtxResult.Text;
            }

            if (cs.IsSearTri)
            {
                tempBool = true;
                TrigCompareAndShowResultHelper trigHelper = new TrigCompareAndShowResultHelper();
                trigHelper.OutputText = AppendOutputText;
                trigHelper.ReplaceLastLineText = ReplaceLastLineText;


                #region 获取数据源


                if (oldDataSource.Type == DBDataSourceType.DataSourceFile)
                {
                    AppendOutputText("从文件中获取触发器\n", OutputType.Comment);
                    tempStt = trigHelper.GetInfoByFile(oldDataSource.Value, oldpathCs.Trigs, ref oldDbModels);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + oldpathCs.Trigs, JsonConvert.SerializeObject(oldDbModels.Triggers.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }
                else if(oldDataSource.Type == DBDataSourceType.MySql)
                {
                    AppendOutputText("从数据库中获取触发器\n", OutputType.Comment);
                    if (trigHelper.GetInfoByDb(oldDataSource.Value, ref oldDbModels))
                        File.AppendAllText(resultStr + oldpathCs.Trigs, JsonConvert.SerializeObject(oldDbModels.Triggers.Values));
                    else
                        tempBool = false;
                }

                if (newDataSource.Type == DBDataSourceType.DataSourceFile)
                {
                    AppendOutputText("从文件中获取触发器\n", OutputType.Comment);
                    tempStt = trigHelper.GetInfoByFile(newDataSource.Value, newpathCs.Trigs, ref newDbModels);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + newpathCs.Trigs, JsonConvert.SerializeObject(newDbModels.Triggers.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }
                else if(newDataSource.Type == DBDataSourceType.MySql)
                {
                    AppendOutputText("从数据库中获取触发器\n", OutputType.Comment);
                    if (trigHelper.GetInfoByDb(newDataSource.Value, ref newDbModels))
                        File.AppendAllText(resultStr + newpathCs.Trigs, JsonConvert.SerializeObject(newDbModels.Triggers.Values));
                    else
                        tempBool = false;
                }
                
                #endregion



                if (cs.IsDiff)
                {

                    trigHelper.CompareAndShow(ref oldDbModels, ref newDbModels, cs, out string errorString);

                    if (string.IsNullOrEmpty(errorString) && tempBool)
                    {
                        AppendOutputText("对比完毕\n\n", OutputType.Comment);
                        File.AppendAllText(resultStr + diffpathCs.Trigs, string.IsNullOrWhiteSpace(tempStr) ? RtxResult.Text : RtxResult.Text.Replace(tempStr, ""));
                    }
                    else
                    {
                        //string tips = string.Concat("对比中发现以下问题，请修正后重新进行比较：\n\n", errorString);
                        //MessageBox.Show(tips, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                tempStr = RtxResult.Text;
            }



            if (cs.IsSearProc)
            {
                tempBool = true;
                ProcCompareAndShowResultHelper funcHelper = new ProcCompareAndShowResultHelper();
                funcHelper.OutputText = AppendOutputText;
                funcHelper.ReplaceLastLineText = ReplaceLastLineText;


                #region 获取数据源

                if (oldDataSource.Type == DBDataSourceType.DataSourceFile)
                {
                    AppendOutputText("从文件中获取存储过程\n", OutputType.Comment);
                    tempStt = funcHelper.GetInfoByFile(oldDataSource.Value, oldpathCs.Procs, ref oldDbModels);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + oldpathCs.Procs, JsonConvert.SerializeObject(oldDbModels.Procs.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }
                else if(oldDataSource.Type == DBDataSourceType.MySql)
                {
                    AppendOutputText("从数据库中获取存储过程\n", OutputType.Comment);
                    if (funcHelper.GetInfoByDb(oldDataSource.Value, ref oldDbModels))
                        File.AppendAllText(resultStr + oldpathCs.Procs, JsonConvert.SerializeObject(oldDbModels.Procs.Values));
                    else
                        tempBool = false;
                }
                if (newDataSource.Type == DBDataSourceType.DataSourceFile)
                {
                    AppendOutputText("从文件中获取存储过程\n", OutputType.Comment);
                    tempStt = funcHelper.GetInfoByFile(newDataSource.Value, newpathCs.Procs, ref newDbModels);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + newpathCs.Procs, JsonConvert.SerializeObject(newDbModels.Procs.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }
                else if(newDataSource.Type == DBDataSourceType.MySql)
                {
                    AppendOutputText("从数据库中获取存储过程\n", OutputType.Comment);
                    if (funcHelper.GetInfoByDb(newDataSource.Value, ref newDbModels))
                        File.AppendAllText(resultStr + newpathCs.Procs, JsonConvert.SerializeObject(newDbModels.Procs.Values));
                    else
                        tempBool = false;
                }

                #endregion

                if (cs.IsDiff)
                {
                    funcHelper.CompareAndShow(ref oldDbModels, ref newDbModels, cs, out string errorString);
                    //CompareAndShowResult(procs, newProcs, cs, out string errorString);

                    if (string.IsNullOrEmpty(errorString) && tempBool)
                    {
                        AppendOutputText("对比完毕\n\n", OutputType.Comment);
                        File.AppendAllText(resultStr + diffpathCs.Procs, string.IsNullOrWhiteSpace(tempStr) ? RtxResult.Text : RtxResult.Text.Replace(tempStr, ""));
                    }
                    else
                    {
                        //string tips = string.Concat("对比中发现以下问题，请修正后重新进行比较：\n\n", errorString);
                        //MessageBox.Show(tips, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                tempStr = RtxResult.Text;
            }



            if (cs.IsSearFunc)
            {
                tempBool = true;
                FuncCompareAndShowResultHelper funcHelper = new FuncCompareAndShowResultHelper();
                funcHelper.OutputText = AppendOutputText;
                funcHelper.ReplaceLastLineText = ReplaceLastLineText;


                #region 获取数据源

                if (oldDataSource.Type == DBDataSourceType.DataSourceFile)
                {
                    AppendOutputText("从文件中获取函数\n", OutputType.Comment);
                    tempStt = funcHelper.GetInfoByFile(oldDataSource.Value, oldpathCs.Funcs, ref oldDbModels);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + oldpathCs.Funcs, JsonConvert.SerializeObject(oldDbModels.Functions.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }
                else if(oldDataSource.Type == DBDataSourceType.MySql)
                {
                    AppendOutputText("从数据库中获取函数\n", OutputType.Comment);
                    if (funcHelper.GetInfoByDb(oldDataSource.Value, ref oldDbModels))
                        File.AppendAllText(resultStr + oldpathCs.Funcs, JsonConvert.SerializeObject(oldDbModels.Functions.Values));
                    else
                        tempBool = false;
                }

                if (newDataSource.Type == DBDataSourceType.DataSourceFile)
                {
                    AppendOutputText("从文件中获取函数\n", OutputType.Comment);
                    tempStt = funcHelper.GetInfoByFile(newDataSource.Value, newpathCs.Funcs, ref newDbModels);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + newpathCs.Funcs, JsonConvert.SerializeObject(newDbModels.Functions.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }
                else if(newDataSource.Type == DBDataSourceType.MySql)
                {
                    AppendOutputText("从数据库中获取函数\n", OutputType.Comment);
                    if (funcHelper.GetInfoByDb(newDataSource.Value, ref newDbModels))
                        File.AppendAllText(resultStr + newpathCs.Funcs, JsonConvert.SerializeObject(newDbModels.Functions.Values));
                    else
                        tempBool = false;
                }

                #endregion


                if (cs.IsDiff)
                {

                    funcHelper.CompareAndShow(ref oldDbModels, ref newDbModels, cs, out string errorString);

                    if (string.IsNullOrEmpty(errorString) && tempBool)
                    {
                        AppendOutputText("对比完毕\n\n", OutputType.Comment);
                        File.AppendAllText(resultStr + diffpathCs.Funcs, string.IsNullOrWhiteSpace(tempStr) ? RtxResult.Text : RtxResult.Text.Replace(tempStr, ""));
                    }
                    else
                    {
                        //string tips = string.Concat("对比中发现以下问题，请修正后重新进行比较：\n\n", errorString);
                        //MessageBox.Show(tips, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                tempStr = RtxResult.Text;
            }

            //bool compIsError = false;

            AppendOutputText("\n", OutputType.Comment);
            AppendOutputText("执行完毕\n", OutputType.Comment);

            SetStatus(false);
            //try
            //{
            //    AbortTh();
            //}
            //catch (Exception)
            //{
            //}
        }

        public delegate void SetStatusCallback(bool isStart);
        public delegate void ConCallback(params object[] param);
        private void SetStatus(bool isStart)
        {
            if (this.InvokeRequired)
            {
                SetStatusCallback d = new SetStatusCallback(SetStatus);
                this.Invoke(d, new object[] { isStart });
                Application.DoEvents();
                return;
            }
            if (isStart)
            {
                this.btnCompare.Tag = "1";
                this.btnCompare.Enabled = false;
                this.RtxResult.Enabled = false;
                this.ddlOldDb.Enabled = false;
                this.ddlNewDb.Enabled = false;
                totalTime.Stop();
                totalTime.Start();
            }
            else
            {
                this.btnCompare.Tag = "0";
                this.btnCompare.Enabled = true;
                this.RtxResult.Enabled = true;
                this.ddlOldDb.Enabled = true;
                this.ddlNewDb.Enabled = true;
                totalTime.Stop();
            }
        }


        private DbModels GetEmptyDbModel(DBDataSourceType sourceType)
        {
            DbModels rel = new DbModels();
            if (sourceType == DBDataSourceType.Empty)
            {
                rel.DbModel = new DBSetting() { DbName = "NULL", SchemaName = "NULL" };
                rel.Functions = new Dictionary<string, Function>();
                rel.Procs = new Dictionary<string, Function>();
                rel.Tables = new Dictionary<string, TableInfo>();
                rel.Triggers = new Dictionary<string, Trigger>();
                rel.Views = new Dictionary<string, ViewInfo>();
            }
            return rel;
        }

        
        
        //定义更新输出的委托
        public delegate bool OutputTextHander(string text, OutputType type);
        public delegate bool ClearTextHander();

        public bool ClearOutputText()
        {

            if (RtxResult.InvokeRequired)
            {
                //// 解决窗体关闭时出现“访问已释放句柄”异常
                //while (RtxResult.IsHandleCreated == false)
                //{
                //    if (RtxResult.Disposing || RtxResult.IsDisposed) return false;
                //}

                //BeginInvoke(new Action(() =>
                //{
                //    RtxResult.SelectionColor = color;
                //    //_lastTextColor = color;

                //    if (type != OutputType.Sql && type != OutputType.None)
                //        RtxResult.AppendText("-- ");

                //    RtxResult.AppendText(text);
                //    RtxResult.Focus();
                //    RtxResult.ScrollToCaret();
                //    RtxResult.Refresh();
                //}));
                ClearTextHander d = new ClearTextHander(ClearOutputText);
                this.Invoke(d, new object[] { });
                Application.DoEvents();
            }
            else
            {
                RtxResult.Clear();
                RtxResult.Refresh();
            }
            //RtxResult.SelectionColor = color;
            ////_lastTextColor = color;

            //if (type != OutputType.Sql && type != OutputType.None)
            //    RtxResult.AppendText("-- ");

            //RtxResult.AppendText(text);
            //RtxResult.Focus();
            //RtxResult.ScrollToCaret();
            return true;
        }

        //解决在获取进度的时候跳动的问题
        bool isFirstReplace = true;
        public bool AppendOutputText(string text, OutputType type, SqlType sqlType = 0)
        {
            isFirstReplace = true;
            Color color = Color.Black;
            if (type == OutputType.Comment || type == OutputType.Loading)
                color = Color.DarkGray;
            else if (type == OutputType.Warning)
                color = Color.Orange;
            else if (type == OutputType.Error)
                color = Color.Red;
            else if (type == OutputType.Sql)
                color = Color.Black;
            //else
            //    color = _lastTextColor;


            if (RtxResult.InvokeRequired)
            {
                //// 解决窗体关闭时出现“访问已释放句柄”异常
                //while (RtxResult.IsHandleCreated == false)
                //{
                //    if (RtxResult.Disposing || RtxResult.IsDisposed) return false;
                //}

                //BeginInvoke(new Action(() =>
                //{
                //    RtxResult.SelectionColor = color;
                //    //_lastTextColor = color;

                //    if (type != OutputType.Sql && type != OutputType.None)
                //        RtxResult.AppendText("-- ");

                //    RtxResult.AppendText(text);
                //    RtxResult.Focus();
                //    RtxResult.ScrollToCaret();
                //    RtxResult.Refresh();
                //}));
                SetTextCallback d = new SetTextCallback(AppendOutputText);
                this.Invoke(d, new object[] { text, type, sqlType });
                Application.DoEvents();
            }
            else
            {
                RtxResult.SelectionColor = color;
                //_lastTextColor = color;
                if (type != OutputType.Sql && type != OutputType.None)
                    text.Insert(0, "-- ");

                //同种类型输出到文件
                if (sqlType != SqlType.Common && sqlType != 0)
                    File.AppendAllText(resultStr + "diff_" + sqlType + ".txt", text + StOs.NewLine);


                RtxResult.AppendText(text);
                RtxResult.Focus();
                RtxResult.ScrollToCaret();
                RtxResult.Refresh();
            }
            //RtxResult.SelectionColor = color;
            ////_lastTextColor = color;

            //if (type != OutputType.Sql && type != OutputType.None)
            //    RtxResult.AppendText("-- ");

            //RtxResult.AppendText(text);
            //RtxResult.Focus();
            //RtxResult.ScrollToCaret();
            return true;
        }
        // 定义委托类型
        delegate bool SetTextCallback(string content, OutputType type = OutputType.Comment, SqlType sqlType = 0);
        /// <summary>
        /// 替换最后一行的文本
        /// </summary>
        /// <param name="content"></param>
        public bool ReplaceLastLineText(string content, OutputType type = OutputType.Comment, SqlType sqlType = 0)
        {
            try
            {

                if (RtxResult.InvokeRequired)
                {
                    //// 解决窗体关闭时出现“访问已释放句柄”异常
                    //while (RtxResult.IsHandleCreated == false)
                    //{
                    //    if (RtxResult.Disposing || RtxResult.IsDisposed) return false;
                    //}
                    //BeginInvoke(new Action(() =>
                    //{
                    //    ////RtxResult.Text = content;
                    //    //System.Threading.Thread.Sleep(10);
                    //    //File.AppendAllText(Environment.CurrentDirectory + "/1.txt", "ReplaceLastLineText Start" + "\r\n");
                    //    if (type != OutputType.Sql && type != OutputType.None)
                    //        content = content.Insert(0, "-- ");
                    //    RtxResult.Select(RtxResult.Text.LastIndexOf("\n") + 1, RtxResult.Text.Length - RtxResult.Text.LastIndexOf("\n"));
                    //    RtxResult.SelectedText = content;
                    //    RtxResult.Focus();
                    //    //System.Threading.Thread.Sleep(10);
                    //    //File.AppendAllText(Environment.CurrentDirectory + "/1.txt", "ReplaceLastLineText END" + "\r\n");
                    //    RtxResult.ScrollToCaret();
                    //    RtxResult.Refresh();
                    //}));
                    SetTextCallback d = new SetTextCallback(ReplaceLastLineText);
                    this.Invoke(d, new object[] { content, type, sqlType });
                    Application.DoEvents();
                }
                else
                {
                    ////RtxResult.Text = content;
                    //System.Threading.Thread.Sleep(10);
                    //File.AppendAllText(Environment.CurrentDirectory + "/1.txt", "ReplaceLastLineText Start" + "\r\n");
                    if (type != OutputType.Sql && type != OutputType.None)
                        content = content.Insert(0, "-- ");
                    RtxResult.Select(RtxResult.Text.LastIndexOf("\n") + 1, RtxResult.Text.Length - RtxResult.Text.LastIndexOf("\n"));
                    RtxResult.SelectedText = content;
                    RtxResult.Focus();
                    //System.Threading.Thread.Sleep(10);
                    //File.AppendAllText(Environment.CurrentDirectory + "/1.txt", "ReplaceLastLineText END" + "\r\n");
                    RtxResult.Refresh();
                    if (isFirstReplace)
                        RtxResult.ScrollToCaret();
                    isFirstReplace = false;
                }
                ////System.Threading.Thread.Sleep(10);
                ////File.AppendAllText(Environment.CurrentDirectory + "/1.txt", "ReplaceLastLineText Start" + "\r\n");
                //if (type != OutputType.Sql && type != OutputType.None)
                //    content = content.Insert(0, "-- ");
                //RtxResult.Select(RtxResult.Text.LastIndexOf("\n") + 1, RtxResult.Text.Length - RtxResult.Text.LastIndexOf("\n"));
                //RtxResult.SelectedText = content;
                //RtxResult.Focus();
                ////System.Threading.Thread.Sleep(10);
                ////File.AppendAllText(Environment.CurrentDirectory + "/1.txt", "ReplaceLastLineText END" + "\r\n");
                //RtxResult.ScrollToCaret();
                return true;
                //return false;
            }
            catch (Exception e)
            {

                //File.AppendAllText(Environment.CurrentDirectory + "/1.txt", "2/" + e.Message);
                return false;
            }
        }



        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            helper?.Close();
            totalTime?.Stop();
            AbortTh();


        }

        public void StartTh()
        {
            t = new System.Threading.Thread(Start);
            t.IsBackground = true;     
            t.Start();
            this.btnTh.Visible = true;
            this.btnTh.Text = "暂停";
        }
        public void AbortTh()
        {
            this.btnTh.Visible = false;
            if (t?.ThreadState != System.Threading.ThreadState.Stopped)
                t?.Abort();
        }
        public void SuspendTh()
        {
            t.Suspend();//挂起线程
            this.btnTh.Text = "继续";
        }
        public void ResumeTh()
        {
            t.Resume();//恢复挂起的线程
            this.btnTh.Text = "暂停";
        }

        private void btnTh_Click(object sender, EventArgs e)
        {
            if (t.ThreadState == System.Threading.ThreadState.Running)
            {
                SuspendTh();
            }
            else if (t.ThreadState == System.Threading.ThreadState.Suspended)
            {
                ResumeTh();
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 彻底的退出
            Environment.Exit(0);
        }

        private void chkOutDeleteSql_CheckedChanged(object sender, EventArgs e)
        {
            var val = this.chkOutDeleteSql.Checked;
            this.chkOutDeleteSqlIsCommon.Visible = val;
        }


        //选择配置文件
        private void btn_openDataSetting_Click(object sender, EventArgs e)
        {
            MessageBox.Show("【数据对比】功能暂未开放 敬请期待！！！ ^_^");
            return;
            OpenFileDialog openfile = new OpenFileDialog();
            //openfile.InitialDirectory = @"";
            openfile.Title = "请选择数据对比配置文件";
            //过滤文件类型
            openfile.Filter = "Json配置文件|*.Json|文本文件|*.txt";
            if (DialogResult.OK == openfile.ShowDialog())
            {
                //将选择的文件的全路径赋值给文本框
                textBox1.Text = openfile.FileName;
            }
        }

        private void btnExchangeDataSource_Click(object sender, EventArgs e)
        {
            //交换数据源

            if (this.ddlOldDb.SelectedItem == null || this.ddlNewDb.SelectedItem == null)
            {
                MessageBox.Show("请选择数据库", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (this.ddlOldDb.SelectedItem.ToString() == this.ddlNewDb.SelectedItem.ToString())
            {

                MessageBox.Show("新旧数据库不能重复", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var oldSourceName = this.ddlOldDb.SelectedItem.ToString();
            var newSourceName = this.ddlNewDb.SelectedItem.ToString();

            this.ddlOldDb.SelectedItem = newSourceName;
            this.ddlNewDb.SelectedItem = oldSourceName;

        }

        Form tip_btnExchangeDataSource = null;
        private void btnExchangeDataSource_MouseHover(object sender, EventArgs e)
        {
            if (cs.IsTipMessage)
                tip_btnExchangeDataSource = FrmAnchorTips.ShowTips(this.btnExchangeDataSource, "交换数据源", AnchorTipsLocation.TOP);
        }

        private void btnExchangeDataSource_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void btnExchangeDataSource_MouseLeave(object sender, EventArgs e)
        {
            if (tip_btnExchangeDataSource != null)
            {
                tip_btnExchangeDataSource.Close();
                this.Activate();
            }
        }

        //protected override void DefWndProc(ref Message m)
        //{
        //    if (m.Msg == 0x60)
        //    {
        //        IntPtr pData = new IntPtr();
        //        int iDataLen = 64;
        //        byte[] byData = new byte[128];
        //        System.Runtime.InteropServices.Marshal.Copy(pData, byData, 0, iDataLen);
        //        string strData = System.Text.Encoding.ASCII.GetString(byData);

        //        MessageBox.Show("DefWndProc-Test888888-" + strData);
        //        return;
        //    }
        //    base.DefWndProc(ref m);
        //}
    }

}
