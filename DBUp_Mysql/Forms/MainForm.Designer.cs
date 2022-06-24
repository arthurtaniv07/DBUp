namespace DBUp_Mysql
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCompare = new System.Windows.Forms.Button();
            this.RtxResult = new System.Windows.Forms.RichTextBox();
            this.cheComm = new System.Windows.Forms.CheckBox();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.btnTh = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.ddlOldDb = new System.Windows.Forms.ComboBox();
            this.ddlNewDb = new System.Windows.Forms.ComboBox();
            this.xhkOutComment = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkOutDeleteSql = new System.Windows.Forms.CheckBox();
            this.chkOutDeleteSqlIsCommon = new System.Windows.Forms.CheckBox();
            this.chkDiffTable = new System.Windows.Forms.CheckBox();
            this.chkDiffView = new System.Windows.Forms.CheckBox();
            this.chkDiffTrigger = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkDiffFunc = new System.Windows.Forms.CheckBox();
            this.chkDiffProc = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btn_openDataSetting = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.btnExchangeDataSource = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCompare
            // 
            this.btnCompare.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompare.Location = new System.Drawing.Point(10, 144);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(714, 35);
            this.btnCompare.TabIndex = 8;
            this.btnCompare.Text = "对比";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // RtxResult
            // 
            this.RtxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RtxResult.Location = new System.Drawing.Point(10, 184);
            this.RtxResult.Margin = new System.Windows.Forms.Padding(2);
            this.RtxResult.Name = "RtxResult";
            this.RtxResult.Size = new System.Drawing.Size(714, 306);
            this.RtxResult.TabIndex = 9;
            this.RtxResult.Text = "";
            // 
            // cheComm
            // 
            this.cheComm.AutoSize = true;
            this.cheComm.Location = new System.Drawing.Point(485, 49);
            this.cheComm.Margin = new System.Windows.Forms.Padding(2);
            this.cheComm.Name = "cheComm";
            this.cheComm.Size = new System.Drawing.Size(72, 16);
            this.cheComm.TabIndex = 10;
            this.cheComm.Text = "比较注释";
            this.cheComm.UseVisualStyleBackColor = true;
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(12, 495);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(65, 12);
            this.lblTotalTime.TabIndex = 11;
            this.lblTotalTime.Tag = "总共耗时：";
            this.lblTotalTime.Text = "总共耗时：";
            // 
            // btnTh
            // 
            this.btnTh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTh.Location = new System.Drawing.Point(649, 15);
            this.btnTh.Name = "btnTh";
            this.btnTh.Size = new System.Drawing.Size(71, 28);
            this.btnTh.TabIndex = 8;
            this.btnTh.Text = "暂停";
            this.btnTh.UseVisualStyleBackColor = true;
            this.btnTh.Visible = false;
            this.btnTh.Click += new System.EventHandler(this.btnTh_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(341, 495);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(383, 12);
            this.linkLabel1.TabIndex = 29;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "注：谨慎使用该工具生成的SQL，造成的后果本软件以及开发者概不负责";
            // 
            // ddlOldDb
            // 
            this.ddlOldDb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlOldDb.FormattingEnabled = true;
            this.ddlOldDb.Location = new System.Drawing.Point(12, 15);
            this.ddlOldDb.Name = "ddlOldDb";
            this.ddlOldDb.Size = new System.Drawing.Size(279, 20);
            this.ddlOldDb.TabIndex = 30;
            // 
            // ddlNewDb
            // 
            this.ddlNewDb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlNewDb.FormattingEnabled = true;
            this.ddlNewDb.Location = new System.Drawing.Point(339, 15);
            this.ddlNewDb.Name = "ddlNewDb";
            this.ddlNewDb.Size = new System.Drawing.Size(279, 20);
            this.ddlNewDb.TabIndex = 30;
            // 
            // xhkOutComment
            // 
            this.xhkOutComment.AutoSize = true;
            this.xhkOutComment.Location = new System.Drawing.Point(82, 98);
            this.xhkOutComment.Margin = new System.Windows.Forms.Padding(2);
            this.xhkOutComment.Name = "xhkOutComment";
            this.xhkOutComment.Size = new System.Drawing.Size(72, 16);
            this.xhkOutComment.TabIndex = 10;
            this.xhkOutComment.Text = "输出注释";
            this.xhkOutComment.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 11;
            this.label1.Tag = "配置：";
            this.label1.Text = "输出配置：";
            // 
            // chkOutDeleteSql
            // 
            this.chkOutDeleteSql.AutoSize = true;
            this.chkOutDeleteSql.Checked = true;
            this.chkOutDeleteSql.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOutDeleteSql.Location = new System.Drawing.Point(169, 98);
            this.chkOutDeleteSql.Margin = new System.Windows.Forms.Padding(2);
            this.chkOutDeleteSql.Name = "chkOutDeleteSql";
            this.chkOutDeleteSql.Size = new System.Drawing.Size(96, 16);
            this.chkOutDeleteSql.TabIndex = 10;
            this.chkOutDeleteSql.Text = "输出删除语句";
            this.chkOutDeleteSql.UseVisualStyleBackColor = true;
            this.chkOutDeleteSql.CheckedChanged += new System.EventHandler(this.chkOutDeleteSql_CheckedChanged);
            // 
            // chkOutDeleteSqlIsCommon
            // 
            this.chkOutDeleteSqlIsCommon.AutoSize = true;
            this.chkOutDeleteSqlIsCommon.Checked = true;
            this.chkOutDeleteSqlIsCommon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOutDeleteSqlIsCommon.Location = new System.Drawing.Point(282, 98);
            this.chkOutDeleteSqlIsCommon.Margin = new System.Windows.Forms.Padding(2);
            this.chkOutDeleteSqlIsCommon.Name = "chkOutDeleteSqlIsCommon";
            this.chkOutDeleteSqlIsCommon.Size = new System.Drawing.Size(132, 16);
            this.chkOutDeleteSqlIsCommon.TabIndex = 10;
            this.chkOutDeleteSqlIsCommon.Text = "删除语句以注释输出";
            this.chkOutDeleteSqlIsCommon.UseVisualStyleBackColor = true;
            // 
            // chkDiffTable
            // 
            this.chkDiffTable.AutoSize = true;
            this.chkDiffTable.Checked = true;
            this.chkDiffTable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDiffTable.Location = new System.Drawing.Point(150, 49);
            this.chkDiffTable.Margin = new System.Windows.Forms.Padding(2);
            this.chkDiffTable.Name = "chkDiffTable";
            this.chkDiffTable.Size = new System.Drawing.Size(36, 16);
            this.chkDiffTable.TabIndex = 10;
            this.chkDiffTable.Text = "表";
            this.chkDiffTable.UseVisualStyleBackColor = true;
            // 
            // chkDiffView
            // 
            this.chkDiffView.AutoSize = true;
            this.chkDiffView.Checked = true;
            this.chkDiffView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDiffView.Location = new System.Drawing.Point(207, 49);
            this.chkDiffView.Margin = new System.Windows.Forms.Padding(2);
            this.chkDiffView.Name = "chkDiffView";
            this.chkDiffView.Size = new System.Drawing.Size(48, 16);
            this.chkDiffView.TabIndex = 10;
            this.chkDiffView.Text = "视图";
            this.chkDiffView.UseVisualStyleBackColor = true;
            this.chkDiffView.CheckedChanged += new System.EventHandler(this.chkOutDeleteSql_CheckedChanged);
            // 
            // chkDiffTrigger
            // 
            this.chkDiffTrigger.AutoSize = true;
            this.chkDiffTrigger.Checked = true;
            this.chkDiffTrigger.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDiffTrigger.Location = new System.Drawing.Point(273, 49);
            this.chkDiffTrigger.Margin = new System.Windows.Forms.Padding(2);
            this.chkDiffTrigger.Name = "chkDiffTrigger";
            this.chkDiffTrigger.Size = new System.Drawing.Size(60, 16);
            this.chkDiffTrigger.TabIndex = 10;
            this.chkDiffTrigger.Text = "触发器";
            this.chkDiffTrigger.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(80, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 11;
            this.label2.Tag = "配置：";
            this.label2.Text = "比较对象：";
            // 
            // chkDiffFunc
            // 
            this.chkDiffFunc.AutoSize = true;
            this.chkDiffFunc.Checked = true;
            this.chkDiffFunc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDiffFunc.Location = new System.Drawing.Point(339, 49);
            this.chkDiffFunc.Margin = new System.Windows.Forms.Padding(2);
            this.chkDiffFunc.Name = "chkDiffFunc";
            this.chkDiffFunc.Size = new System.Drawing.Size(48, 16);
            this.chkDiffFunc.TabIndex = 31;
            this.chkDiffFunc.Text = "函数";
            this.chkDiffFunc.UseVisualStyleBackColor = true;
            // 
            // chkDiffProc
            // 
            this.chkDiffProc.AutoSize = true;
            this.chkDiffProc.Checked = true;
            this.chkDiffProc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDiffProc.Location = new System.Drawing.Point(400, 49);
            this.chkDiffProc.Margin = new System.Windows.Forms.Padding(2);
            this.chkDiffProc.Name = "chkDiffProc";
            this.chkDiffProc.Size = new System.Drawing.Size(72, 16);
            this.chkDiffProc.TabIndex = 31;
            this.chkDiffProc.Text = "存储过程";
            this.chkDiffProc.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(14, 49);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 16);
            this.checkBox1.TabIndex = 32;
            this.checkBox1.Text = "结构";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(14, 76);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(48, 16);
            this.checkBox2.TabIndex = 32;
            this.checkBox2.Text = "数据";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(82, 71);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(390, 21);
            this.textBox1.TabIndex = 33;
            // 
            // btn_openDataSetting
            // 
            this.btn_openDataSetting.Location = new System.Drawing.Point(471, 70);
            this.btn_openDataSetting.Name = "btn_openDataSetting";
            this.btn_openDataSetting.Size = new System.Drawing.Size(30, 23);
            this.btn_openDataSetting.TabIndex = 34;
            this.btn_openDataSetting.Text = "...";
            this.btn_openDataSetting.UseVisualStyleBackColor = true;
            this.btn_openDataSetting.Click += new System.EventHandler(this.btn_openDataSetting_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 35;
            this.label3.Tag = "配置：";
            this.label3.Text = "线程个数：";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(82, 118);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(63, 21);
            this.textBox2.TabIndex = 36;
            this.textBox2.Text = "1";
            // 
            // btnExchangeDataSource
            // 
            this.btnExchangeDataSource.Location = new System.Drawing.Point(296, 13);
            this.btnExchangeDataSource.Name = "btnExchangeDataSource";
            this.btnExchangeDataSource.Size = new System.Drawing.Size(36, 23);
            this.btnExchangeDataSource.TabIndex = 37;
            this.btnExchangeDataSource.Text = "<=>";
            this.btnExchangeDataSource.UseVisualStyleBackColor = true;
            this.btnExchangeDataSource.Click += new System.EventHandler(this.btnExchangeDataSource_Click);
            this.btnExchangeDataSource.MouseLeave += new System.EventHandler(this.btnExchangeDataSource_MouseLeave);
            this.btnExchangeDataSource.MouseHover += new System.EventHandler(this.btnExchangeDataSource_MouseHover);
            this.btnExchangeDataSource.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnExchangeDataSource_MouseMove);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 512);
            this.Controls.Add(this.btnExchangeDataSource);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_openDataSetting);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.chkDiffProc);
            this.Controls.Add(this.chkDiffFunc);
            this.Controls.Add(this.ddlNewDb);
            this.Controls.Add(this.ddlOldDb);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkDiffTrigger);
            this.Controls.Add(this.lblTotalTime);
            this.Controls.Add(this.chkDiffView);
            this.Controls.Add(this.chkOutDeleteSqlIsCommon);
            this.Controls.Add(this.chkDiffTable);
            this.Controls.Add(this.chkOutDeleteSql);
            this.Controls.Add(this.xhkOutComment);
            this.Controls.Add(this.cheComm);
            this.Controls.Add(this.RtxResult);
            this.Controls.Add(this.btnTh);
            this.Controls.Add(this.btnCompare);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(599, 399);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "数据库升级助手(MySQL 简易版) {0}       By谭盼(13012363357)";
            this.Text = "数据库升级助手(MySQL 简易版) By谭盼(13012363357)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.RichTextBox RtxResult;
        private System.Windows.Forms.CheckBox cheComm;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.Button btnTh;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ComboBox ddlOldDb;
        private System.Windows.Forms.ComboBox ddlNewDb;
        private System.Windows.Forms.CheckBox xhkOutComment;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkOutDeleteSql;
        private System.Windows.Forms.CheckBox chkOutDeleteSqlIsCommon;
        private System.Windows.Forms.CheckBox chkDiffTable;
        private System.Windows.Forms.CheckBox chkDiffView;
        private System.Windows.Forms.CheckBox chkDiffTrigger;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkDiffFunc;
        private System.Windows.Forms.CheckBox chkDiffProc;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btn_openDataSetting;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button btnExchangeDataSource;
    }
}

