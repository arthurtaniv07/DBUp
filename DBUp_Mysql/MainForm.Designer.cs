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
            this.SuspendLayout();
            // 
            // btnCompare
            // 
            this.btnCompare.Location = new System.Drawing.Point(13, 46);
            this.btnCompare.Margin = new System.Windows.Forms.Padding(4);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(774, 44);
            this.btnCompare.TabIndex = 8;
            this.btnCompare.Text = "对比";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // RtxResult
            // 
            this.RtxResult.Location = new System.Drawing.Point(13, 97);
            this.RtxResult.Name = "RtxResult";
            this.RtxResult.Size = new System.Drawing.Size(774, 341);
            this.RtxResult.TabIndex = 9;
            this.RtxResult.Text = "";
            // 
            // cheComm
            // 
            this.cheComm.AutoSize = true;
            this.cheComm.Location = new System.Drawing.Point(29, 13);
            this.cheComm.Name = "cheComm";
            this.cheComm.Size = new System.Drawing.Size(59, 19);
            this.cheComm.TabIndex = 10;
            this.cheComm.Text = "注释";
            this.cheComm.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cheComm);
            this.Controls.Add(this.RtxResult);
            this.Controls.Add(this.btnCompare);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.RichTextBox RtxResult;
        private System.Windows.Forms.CheckBox cheComm;
    }
}

