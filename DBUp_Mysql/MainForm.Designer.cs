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
            this.cheComm.AutoSize = true;
            this.cheComm.Location = new System.Drawing.Point(22, 17);
            this.cheComm.Margin = new System.Windows.Forms.Padding(2);
            this.cheComm.Name = "cheComm";
            this.cheComm.Size = new System.Drawing.Size(48, 16);
            this.cheComm.TabIndex = 10;
            this.cheComm.Text = "注释";
            this.cheComm.UseVisualStyleBackColor = true;
            // 
            // lblTotalTime
            // 
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
            this.btnTh.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTh.Location = new System.Drawing.Point(517, 10);
            this.btnTh.Name = "btnTh";
            this.btnTh.Size = new System.Drawing.Size(71, 28);
            this.btnTh.TabIndex = 8;
            this.btnTh.Text = "暂停";
            this.btnTh.UseVisualStyleBackColor = true;
            this.btnTh.Visible = false;
            this.btnTh.Click += new System.EventHandler(this.btnTh_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 381);
            this.Controls.Add(this.lblTotalTime);
            this.Controls.Add(this.cheComm);
            this.Controls.Add(this.RtxResult);
            this.Controls.Add(this.btnTh);
            this.Controls.Add(this.btnCompare);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
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
    }
}

