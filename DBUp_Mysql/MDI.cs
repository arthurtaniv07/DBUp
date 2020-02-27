using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBUp_Mysql
{

    public partial class MDI : Form
    {

        public MDI()
        {
            InitializeComponent();
        }
        TabContent tabContent = null;
        private void MDI_Load(object sender, EventArgs e)
        {
            tabContent = new TabContent(tabControl1);
            tabContent.Add("数据库结构升级助手", "DBUp_Mysql.MainForm");
            tabContent.Add("代码发布工具", "DBUp_Mysql.ReleaseCode");

            //tabContent.Close(1);
        }
    }

}