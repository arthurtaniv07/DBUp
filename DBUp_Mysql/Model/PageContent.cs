using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBUp_Mysql
{
    public class PageContent
    {
        /// <summary>
        /// 窗体的完全限定名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 停靠的容器
        /// </summary>
        public TabControl Dock { get; set; }
        /// <summary>
        /// 当前窗体在容器中停靠的索引
        /// </summary>
        public int TabIndex { get; set; }
        /// <summary>
        /// 选项卡名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否已经加载过
        /// </summary>
        public bool IsLoad { get; set; }
    }
}
