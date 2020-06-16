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

        string oldConnString = "";
        string newConnString = "";
        DBConnection oldConn = new DBConnection();
        DBConnection newConn = new DBConnection();


        string oldDbName = "";
        string newDbName = "";
        string oldServerName = "";
        string newServerName = "";
        public MainForm()
        {
            InitializeComponent();
        }
        public void SetPathSetting(PathSetting cs, string targ) {

            cs.Tables = targ + pathCs.Tables;
            cs.Funcs = targ + pathCs.Funcs;
            cs.Procs = targ + pathCs.Procs;
            cs.Trigs = targ + pathCs.Trigs;
            cs.Views = targ + pathCs.Views;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            //加载数据库链接
            #region 加载数据库链接
            var allConnection = ConfigurationManager.ConnectionStrings;
            int inx = -1;
            int oldInx = -1;
            int newInx = -1;
            foreach (ConnectionStringSettings item in allConnection)
            {
                inx++;
                this.ddlOldDb.Items.Add(item.Name);
                this.ddlNewDb.Items.Add(item.Name);
                if ("old".Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                {
                    oldInx = inx;
                    this.ddlOldDb.SelectedIndex =inx;
                }
                else if("new".Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                {
                    newInx = inx;
                    this.ddlNewDb.SelectedIndex = inx;
                }
            }

            if (oldInx < 0 || newInx < 0)
            {
                if (this.ddlOldDb.Items.Count > 0) this.ddlOldDb.SelectedIndex = 0;
                if (this.ddlOldDb.Items.Count > 0) this.ddlNewDb.SelectedIndex = 0;
            }

            #endregion

            SetPathSetting(oldpathCs, "old");
            SetPathSetting(newpathCs, "new");
            SetPathSetting(diffpathCs, "diff");

            //newConn.ProviderName = ConfigurationManager.ConnectionStrings["new"].ProviderName;
            //oldConn.ProviderName = ConfigurationManager.ConnectionStrings["old"].ProviderName;

            config.NewPathSetting = newpathCs;
            config.OldPathSetting = oldpathCs;
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




            totalTime = new Timer();
            totalTime.Interval = 500;
            totalTime.Tick += TotalTime_Tick;

        }

        private void TotalTime_Tick(object sender, EventArgs e)
        {
            SetTotalTime();
        }

        System.Threading.Thread t = null;
        private void btnCompare_Click(object sender, EventArgs e)
        {


            if (this.ddlOldDb.SelectedItem == null || this.ddlNewDb.SelectedItem == null)
            {
                MessageBox.Show("请选择数据库", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (this.ddlOldDb.SelectedItem.ToString() == this.ddlNewDb.SelectedItem.ToString())
            {
                MessageBox.Show("新旧数据库不能重复", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            newConn.ProviderName = ConfigurationManager.ConnectionStrings[this.ddlOldDb.SelectedItem.ToString()].ProviderName;
            oldConn.ProviderName = ConfigurationManager.ConnectionStrings[this.ddlNewDb.SelectedItem.ToString()].ProviderName;

            oldConnString = ConfigurationManager.ConnectionStrings[this.ddlOldDb.SelectedItem.ToString()].ConnectionString;
            newConnString = ConfigurationManager.ConnectionStrings[this.ddlNewDb.SelectedItem.ToString()].ConnectionString;
            //从配置文件加载Setting
            if (File.Exists(Environment.CurrentDirectory + "/Setting.txt"))
            {
                try
                {
                    cs = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Environment.CurrentDirectory + "/Setting.txt"));
                }
                catch (Exception)
                {
                    MessageBox.Show("配置文件(Setting.txt)获取出错，请联系管理员", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

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
        MySqlOptionHelper helper = null;

        Timer totalTime = null;
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
        DateTime startTime = default(DateTime);

        public void SetTotalTime()
        {
            //lblTotalTime.Text = lblTotalTime.Tag + Tools.GetTimeSpan(DateTime.Now - startTime);
        }
        
        private void Start()
        {
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

                    //St.StStack stExceptionStack = new St.StStack(0);
                    //string content = stExceptionStack.ToString();
                    string content = e.ToString();
                    //StMailHelper.CheckSend("1578403183@qq.com", "DBUp_MySql", "程序出错", content.Replace("\r\n", "<br />"));
                    content = content.Insert(0, string.Format("---------------{0}---------------", DateTime.Now.ToString()));
                    string resultStr = Environment.CurrentDirectory + string.Format("/result/{0}-Error.txt", DateTime.Now.ToString("yyyyMMdd"));

                    File.AppendAllText(resultStr, content);
                    MessageBox.Show(e.ToString());
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.ToString());
                }
            }
        }


        private void SetStatus(bool isStart)
        {
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

        private void StartCompare()
        {

            cs.CheckCommon = this.cheComm.Checked;
            cs.OutputComment = xhkOutComment.Checked;
            cs.OutputDeleteSql = chkOutDeleteSql.Checked;
            cs.OutputDeleteSqlIsCommon = chkOutDeleteSqlIsCommon.Checked;


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




            using (helper = new MySqlOptionHelper(oldConnString))
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


                oldDbName = helper.DbName;
                oldServerName = helper.Server;
                oldConn.ConnectionString = string.Format("Server={0};Database={1};Port={2}", helper.Server, helper.DbName, helper.Port);
            }
            using (helper = new MySqlOptionHelper(newConnString))
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

                newDbName = helper.DbName;
                newServerName = helper.Server;
                newConn.ConnectionString = string.Format("Server={0};Database={1};Port={2}", helper.Server, helper.DbName, helper.Port);
            }
            AppendOutputText("\n", OutputType.None);
            if (oldConn.ConnectionString == newConn.ConnectionString)
            {
                AppendOutputText("新旧数据库数据源一致 无需比较", OutputType.Error);
                MessageBox.Show("结束对比：新旧数据库数据源一致 无需比较", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetStatus(false);
                return;
            }
            //int tempInx = 0;
            ////从网卡层面判断是否联网
            //if (!Win32API.InternetGetConnectedState(ref tempInx, 0))
            //{
            //    AppendOutputText("请检查你的网络状态", OutputType.Error);
            //    MessageBox.Show("结束对比：请检查你的网络状态", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    SetStatus(false);
            //    return;
            //}

            string resultStr = Environment.CurrentDirectory + string.Format("/result/{0}-{1}-{2}/", DateTime.Now.ToString("yyyyMMddHHmmss"), oldDbName, newDbName);
            if (!Directory.Exists(resultStr))
            {
                Directory.CreateDirectory(resultStr);
            }

            File.AppendAllText(resultStr + oldpathCs.Path, JsonConvert.SerializeObject(config));


            //这里比较数据库的sqlmode
            new CompareAndShowResultHelperBase().ShowDbDiff(oldConnString, newConnString);


            //var c = new CompareAndShowResultHelperFactory();

            if (cs.IsSearTable)
            {
                tempBool = true;
                Dictionary<string, TableInfo> oldTabs = new Dictionary<string, TableInfo>();
                Dictionary<string, TableInfo> newTabs = new Dictionary<string, TableInfo>();
                TableCompareAndShowResultHelper viewHelper = new TableCompareAndShowResultHelper();
                viewHelper.OutputText = AppendOutputText;
                viewHelper.ReplaceLastLineText = ReplaceLastLineText;
                if (cs.IsFileDataPath_Table == false)
                {
                    AppendOutputText("从数据库中获取表结构\n", OutputType.Comment);
                    if (viewHelper.GetInfoByDb(oldConnString, out oldTabs))
                        File.AppendAllText(resultStr + oldpathCs.Tables, JsonConvert.SerializeObject(oldTabs.Values));
                    else
                        tempBool = false;

                    if (viewHelper.GetInfoByDb(newConnString, out newTabs))
                        File.AppendAllText(resultStr + newpathCs.Tables, JsonConvert.SerializeObject(newTabs.Values));
                    else
                        tempBool = false;
                }
                else
                {

                    AppendOutputText("从文件中获取表结构\n", OutputType.Comment);
                    tempStt = viewHelper.GetInfoByFile(cs.FileDataPath, oldpathCs.Tables, out oldTabs);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + oldpathCs.Tables, JsonConvert.SerializeObject(oldTabs.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }

                    tempStt = viewHelper.GetInfoByFile(cs.FileDataPath, newpathCs.Tables, out newTabs);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + newpathCs.Tables, JsonConvert.SerializeObject(newTabs.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }


                if (cs.IsDiff)
                {
                    viewHelper.CompareAndShow(oldTabs, newTabs, cs, out string errorString, newConnString);

                    if (string.IsNullOrEmpty(errorString) && tempBool)
                    {
                        AppendOutputText("对比完毕\n\n", OutputType.Comment);
                        //compIsError = true;
                        File.AppendAllText(resultStr + diffpathCs.Tables, RtxResult.Text);
                    }
                    else
                    {
                        //string tips = string.Concat("对比中发现以下问题，请修正后重新进行比较：\n\n", errorString);
                        //MessageBox.Show(tips, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    tempStr = RtxResult.Text;
                }
            }


            if (cs.IsSearView)
            {

                tempBool = true;
                Dictionary<string, ViewInfo> views = new Dictionary<string, ViewInfo>();
                Dictionary<string, ViewInfo> newViews = new Dictionary<string, ViewInfo>();
                ViewCompareAndShowResultHelper viewHelper = new ViewCompareAndShowResultHelper();
                viewHelper.OutputText = AppendOutputText;
                viewHelper.ReplaceLastLineText = ReplaceLastLineText;
                if (cs.IsFileDataPath_View == false)
                {
                    AppendOutputText("从数据库中获取视图\n", OutputType.Comment);
                    if (viewHelper.GetInfoByDb(oldConnString, out views))
                        File.AppendAllText(resultStr + oldpathCs.Views, JsonConvert.SerializeObject(views.Values));
                    else
                        tempBool = false;

                    if (viewHelper.GetInfoByDb(newConnString, out newViews))
                        File.AppendAllText(resultStr + newpathCs.Views, JsonConvert.SerializeObject(newViews.Values));
                    else
                        tempBool = false;

                }
                else
                {


                    AppendOutputText("从文件中获取视图\n", OutputType.Comment);
                    tempStt = viewHelper.GetInfoByFile(cs.FileDataPath, oldpathCs.Views, out views);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + oldpathCs.Views, JsonConvert.SerializeObject(views.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }

                    tempStt = viewHelper.GetInfoByFile(cs.FileDataPath, newpathCs.Views, out newViews);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + newpathCs.Views, JsonConvert.SerializeObject(newViews.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }

                if (cs.IsDiff)
                {

                    viewHelper.CompareAndShow(views, newViews, cs, out string errorString);

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

                    tempStr = RtxResult.Text;
                }
            }

            if (cs.IsSearTri)
            {
                tempBool = true;
                Dictionary<string, Trigger> tris = new Dictionary<string, Trigger>();
                Dictionary<string, Trigger> newTris = new Dictionary<string, Trigger>();
                TrigCompareAndShowResultHelper trigHelper = new TrigCompareAndShowResultHelper();
                trigHelper.OutputText = AppendOutputText;
                trigHelper.ReplaceLastLineText = ReplaceLastLineText;
                if (cs.IsFileDataPath_Trig == false)
                {
                    AppendOutputText("从数据库中获取触发器\n", OutputType.Comment);
                    if (trigHelper.GetInfoByDb(oldConnString, out tris))
                        File.AppendAllText(resultStr + oldpathCs.Trigs, JsonConvert.SerializeObject(tris.Values));
                    else
                        tempBool = false;

                    if (trigHelper.GetInfoByDb(newConnString, out newTris))
                        File.AppendAllText(resultStr + newpathCs.Trigs, JsonConvert.SerializeObject(newTris.Values));
                    else
                        tempBool = false;
                }
                else
                {

                    AppendOutputText("从文件中获取触发器\n", OutputType.Comment);
                    tempStt = trigHelper.GetInfoByFile(cs.FileDataPath, oldpathCs.Trigs, out tris);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + oldpathCs.Trigs, JsonConvert.SerializeObject(tris.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }

                    tempStt = trigHelper.GetInfoByFile(cs.FileDataPath, newpathCs.Trigs, out newTris);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + newpathCs.Trigs, JsonConvert.SerializeObject(newTris.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }

                if (cs.IsDiff)
                {

                    trigHelper.CompareAndShow(tris, newTris, cs, out string errorString);

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

                    tempStr = RtxResult.Text;
                }
            }



            if (cs.IsSearProc)
            {
                tempBool = true;
                Dictionary<string, Function> procs = new Dictionary<string, Function>();
                Dictionary<string, Function> newProcs = new Dictionary<string, Function>();
                ProcCompareAndShowResultHelper funcHelper = new ProcCompareAndShowResultHelper();
                funcHelper.OutputText = AppendOutputText;
                funcHelper.ReplaceLastLineText = ReplaceLastLineText;

                if (cs.IsFileDataPath_Proc == false)
                {
                    AppendOutputText("从数据库中获取存储过程\n", OutputType.Comment);
                    if (funcHelper.GetInfoByDb(oldConnString, out procs))
                        File.AppendAllText(resultStr + oldpathCs.Procs, JsonConvert.SerializeObject(procs.Values));
                    else
                        tempBool = false;

                    if (funcHelper.GetInfoByDb(newConnString, out newProcs))
                        File.AppendAllText(resultStr + newpathCs.Procs, JsonConvert.SerializeObject(newProcs.Values));
                    else
                        tempBool = false;


                }
                else
                {
                    AppendOutputText("从文件中获取存储过程\n", OutputType.Comment);
                    tempStt = funcHelper.GetInfoByFile(cs.FileDataPath, oldpathCs.Procs, out procs);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + oldpathCs.Procs, JsonConvert.SerializeObject(procs.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }

                    tempStt = funcHelper.GetInfoByFile(cs.FileDataPath, newpathCs.Procs, out newProcs);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + newpathCs.Procs, JsonConvert.SerializeObject(newProcs.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }

                if (cs.IsDiff)
                {
                    funcHelper.CompareAndShow(procs, newProcs, cs, out string errorString);
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

                    tempStr = RtxResult.Text;
                }
            }



            if (cs.IsSearFunc)
            {
                tempBool = true;
                Dictionary<string, Function> funcs = new Dictionary<string, Function>();
                Dictionary<string, Function> newFuncs = new Dictionary<string, Function>();
                FuncCompareAndShowResultHelper funcHelper = new FuncCompareAndShowResultHelper();
                funcHelper.OutputText = AppendOutputText;
                funcHelper.ReplaceLastLineText = ReplaceLastLineText;
                if (cs.IsFileDataPath_Func == false)
                {
                    AppendOutputText("从数据库中获取函数\n", OutputType.Comment);
                    if (funcHelper.GetInfoByDb(oldConnString, out funcs))
                        File.AppendAllText(resultStr + oldpathCs.Funcs, JsonConvert.SerializeObject(funcs.Values));
                    else
                        tempBool = false;

                    if (funcHelper.GetInfoByDb(newConnString, out newFuncs))
                        File.AppendAllText(resultStr + newpathCs.Funcs, JsonConvert.SerializeObject(newFuncs.Values));
                    else
                        tempBool = false;
                }
                else
                {
                    AppendOutputText("从文件中获取函数\n", OutputType.Comment);
                    tempStt = funcHelper.GetInfoByFile(cs.FileDataPath, oldpathCs.Funcs, out funcs);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + oldpathCs.Funcs, JsonConvert.SerializeObject(funcs.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                    tempStt = funcHelper.GetInfoByFile(cs.FileDataPath, newpathCs.Funcs, out newFuncs);
                    if (string.IsNullOrWhiteSpace(tempStt))
                        File.AppendAllText(resultStr + newpathCs.Funcs, JsonConvert.SerializeObject(newFuncs.Values));
                    else
                    {
                        tempBool = false;
                        AppendOutputText(tempStt + "\r\n", OutputType.Error);
                    }
                }

                if (cs.IsDiff)
                {

                    funcHelper.CompareAndShow(funcs, newFuncs, cs, out string errorString);

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


        //定义更新输出的委托
        public delegate bool OutputTextHander(string text, OutputType type);
        OutputTextHander appendOutputText;//追加
        OutputTextHander replaceOutputText;//替换
        public delegate bool ClearTextHander();
        ClearTextHander clearOutputText;//清空

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

        public bool AppendOutputText(string text, OutputType type)
        {
            Color color = Color.Black;
            if (type == OutputType.Comment)
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
                this.Invoke(d, new object[] { text, type });
                Application.DoEvents();
            }
            else
            {
                RtxResult.SelectionColor = color;
                //_lastTextColor = color;

                if (type != OutputType.Sql && type != OutputType.None)
                    RtxResult.AppendText("-- ");

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
        delegate bool SetTextCallback(string content, OutputType type = OutputType.Comment);
        /// <summary>
        /// 替换最后一行的文本
        /// </summary>
        /// <param name="content"></param>
        public bool ReplaceLastLineText(string content, OutputType type = OutputType.Comment)
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
                    this.Invoke(d, new object[] { content, type });
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
                    RtxResult.ScrollToCaret();
                    RtxResult.Refresh();
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
                return false;
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
    }

}
