using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DBUp_Mysql
{
    public class TabContentManage
    {
        /// <summary>
        /// 创建窗体并将该窗体停靠在指定选项卡中
        /// </summary>
        /// <param name="form">窗体的完全限定名</param>
        /// <param name="sender">选项卡</param>
        public static void GenerateForm(string form, object sender)
        {
            Form fm = (Form)Assembly.GetExecutingAssembly().CreateInstance(form);
            fm.FormBorderStyle = FormBorderStyle.None;
            fm.TopLevel = false;
            fm.Parent = ((TabControl)sender).SelectedTab;
            fm.ControlBox = false;
            fm.Dock = DockStyle.Fill;
            fm.Show();

        }


        
    }

    public class TabContent
    {
        public TabContent(TabControl tabControl) : this(tabControl, true)
        {
        }
        public TabContent(TabControl tabControl, bool isClearTable)
        {
            if(isClearTable)
            tabControl.TabPages.Clear();
            _currTab = tabControl;
            _tabPages = new List<PageContent>();
        }

        /// <summary>
        /// 当前容器
        /// </summary>
        private TabControl _currTab;


        /// <summary>
        /// 总个数
        /// </summary>
        public int Count { get { return _tabPages.Count; } }

        private List<PageContent> _tabPages;
        /// <summary>
        /// 获取当前容器的所有页
        /// </summary>
        public List<PageContent> TabPages
        {
            get
            {
                return _tabPages;
            }
        }

        /// <summary>
        /// 创建一个选项卡
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fullName"></param>
        public void Add(string name, string fullName)
        {
            TabPage tb = new TabPage();
            tb.Location = new System.Drawing.Point(4, 22);
            tb.Text = string.Format(" {0} ", name);
            tb.Padding = new System.Windows.Forms.Padding(3);
            tb.Size = new System.Drawing.Size(876, 100);
            tb.TabIndex = 1;
            tb.Name = "tabPage" + _currTab.TabCount;
            tb.UseVisualStyleBackColor = true;

            PageContent page = new PageContent();
            _currTab.TabPages.Add(tb);
            page.TabIndex = _currTab.TabCount - 1;
            page.Dock = _currTab;
            page.FullName = fullName;
            page.IsLoad = false;
            page.Dock.TabPages[page.TabIndex].Select();
            _tabPages.Add(page);
            GenerateForm(page);
        }


        private void GenerateForm(PageContent form)
        {
            if (form.IsLoad == true)
                return;

            var currPage = form.Dock.TabPages[form.TabIndex];
            currPage.Name = form.Name;
            Form fm = (Form)Assembly.GetExecutingAssembly().CreateInstance(form.FullName);
            fm.FormBorderStyle = FormBorderStyle.None;
            fm.TopLevel = false;
            fm.Parent = currPage;
            fm.ControlBox = false;
            fm.Dock = DockStyle.Fill;
            fm.Show();

        }


        public void Close(int tabInx)
        {
            _currTab.TabPages.RemoveAt(tabInx);
            _tabPages.RemoveAt(tabInx);
        }
    }
    
}
