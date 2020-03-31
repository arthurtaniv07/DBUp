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
            this.SuspendLayout();
            // 
            // btnCompare
            // 
            this.btnCompare.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompare.Location = new System.Drawing.Point(10, 44);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(580, 35);
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
            this.RtxResult.Location = new System.Drawing.Point(10, 87);
            this.RtxResult.Margin = new System.Windows.Forms.Padding(2);
            this.RtxResult.Name = "RtxResult";
            this.RtxResult.Size = new System.Drawing.Size(582, 272);
            this.RtxResult.TabIndex = 9;
            this.RtxResult.Text = "";
            // 
            // cheComm
            // 
            this.cheComm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cheComm.AutoSize = true;
            this.cheComm.Location = new System.Drawing.Point(438, 19);
            this.cheComm.Margin = new System.Windows.Forms.Padding(2);
            this.cheComm.Name = "cheComm";
            this.cheComm.Size = new System.Drawing.Size(48, 16);
            this.cheComm.TabIndex = 10;
            this.cheComm.Text = "注释";
            this.cheComm.UseVisualStyleBackColor = true;
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(12, 364);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(65, 12);
            this.lblTotalTime.TabIndex = 11;
            this.lblTotalTime.Tag = "总共耗时：";
            this.lblTotalTime.Text = "总共耗时：";
            this.lblTotalTime.Visible = false;
            // 
            // btnTh
            // 
            this.btnTh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTh.Location = new System.Drawing.Point(517, 10);
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
            this.linkLabel1.Location = new System.Drawing.Point(209, 364);
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
            this.ddlOldDb.Size = new System.Drawing.Size(174, 20);
            this.ddlOldDb.TabIndex = 30;
            // 
            // ddlNewDb
            // 
            this.ddlNewDb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlNewDb.FormattingEnabled = true;
            this.ddlNewDb.Location = new System.Drawing.Point(194, 15);
            this.ddlNewDb.Name = "ddlNewDb";
            this.ddlNewDb.Size = new System.Drawing.Size(174, 20);
            this.ddlNewDb.TabIndex = 30;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 381);
            this.Controls.Add(this.ddlNewDb);
            this.Controls.Add(this.ddlOldDb);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.lblTotalTime);
            this.Controls.Add(this.cheComm);
            this.Controls.Add(this.RtxResult);
            this.Controls.Add(this.btnTh);
            this.Controls.Add(this.btnCompare);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(599, 399);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
    }
}

