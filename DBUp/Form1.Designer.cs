namespace DBUp
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtpassWord = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtuserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServerAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comBoxDBList = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCurrState = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.comBoxDBList2 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtServerAddress2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2_2 = new System.Windows.Forms.Button();
            this.checkBox3_2 = new System.Windows.Forms.CheckBox();
            this.button1_2 = new System.Windows.Forms.Button();
            this.txtpassWord2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtuserName2 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.cbox_view = new System.Windows.Forms.CheckBox();
            this.cbox_proc = new System.Windows.Forms.CheckBox();
            this.cbox_u = new System.Windows.Forms.CheckBox();
            this.cbox_tr = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.button4 = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label10 = new System.Windows.Forms.Label();
            this.CheckSTime = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.checkBox3);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.txtpassWord);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtuserName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 81);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(449, 172);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "登录到当前数据库";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(234, 132);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "确 定";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(315, 139);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(84, 16);
            this.checkBox3.TabIndex = 5;
            this.checkBox3.Text = "所有数据库";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(69, 132);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 20;
            this.button1.TabStop = false;
            this.button1.Text = "测试连接";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // txtpassWord
            // 
            this.txtpassWord.Location = new System.Drawing.Point(118, 87);
            this.txtpassWord.Name = "txtpassWord";
            this.txtpassWord.PasswordChar = '*';
            this.txtpassWord.Size = new System.Drawing.Size(274, 21);
            this.txtpassWord.TabIndex = 4;
            this.txtpassWord.Tag = "Dcs$(~)20160308OAiTour!@0%(_";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "密码";
            // 
            // txtuserName
            // 
            this.txtuserName.Location = new System.Drawing.Point(118, 37);
            this.txtuserName.Name = "txtuserName";
            this.txtuserName.Size = new System.Drawing.Size(274, 21);
            this.txtuserName.TabIndex = 3;
            this.txtuserName.Tag = "DCSV1.0SVN";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "用户名";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(45, 12);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(111, 23);
            this.button5.TabIndex = 20;
            this.button5.Text = "开启本地服务";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "当前服务器地址";
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.Location = new System.Drawing.Point(103, 42);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.Size = new System.Drawing.Size(358, 21);
            this.txtServerAddress.TabIndex = 1;
            this.txtServerAddress.Tag = "211.149.202.115";
            this.txtServerAddress.Text = ".\\SQLEXPRESS";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(53, 274);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "当前数据库";
            // 
            // comBoxDBList
            // 
            this.comBoxDBList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comBoxDBList.FormattingEnabled = true;
            this.comBoxDBList.Location = new System.Drawing.Point(141, 268);
            this.comBoxDBList.Name = "comBoxDBList";
            this.comBoxDBList.Size = new System.Drawing.Size(274, 20);
            this.comBoxDBList.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1, 330);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "当前状态：";
            // 
            // lblCurrState
            // 
            this.lblCurrState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCurrState.AutoSize = true;
            this.lblCurrState.Location = new System.Drawing.Point(68, 330);
            this.lblCurrState.Name = "lblCurrState";
            this.lblCurrState.Size = new System.Drawing.Size(41, 12);
            this.lblCurrState.TabIndex = 15;
            this.lblCurrState.Text = "label9";
            // 
            // comBoxDBList2
            // 
            this.comBoxDBList2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comBoxDBList2.FormattingEnabled = true;
            this.comBoxDBList2.Location = new System.Drawing.Point(652, 268);
            this.comBoxDBList2.Name = "comBoxDBList2";
            this.comBoxDBList2.Size = new System.Drawing.Size(274, 20);
            this.comBoxDBList2.TabIndex = 24;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(564, 274);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 25;
            this.label5.Text = "目标数据库";
            // 
            // txtServerAddress2
            // 
            this.txtServerAddress2.Location = new System.Drawing.Point(614, 42);
            this.txtServerAddress2.Name = "txtServerAddress2";
            this.txtServerAddress2.Size = new System.Drawing.Size(358, 21);
            this.txtServerAddress2.TabIndex = 21;
            this.txtServerAddress2.Tag = "211.149.202.115";
            this.txtServerAddress2.Text = ".\\SQLEXPRESS";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(523, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 22;
            this.label6.Text = "目标服务器地址";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2_2);
            this.groupBox2.Controls.Add(this.checkBox3_2);
            this.groupBox2.Controls.Add(this.button1_2);
            this.groupBox2.Controls.Add(this.txtpassWord2);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtuserName2);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(523, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(449, 172);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "登录到目标数据库";
            // 
            // button2_2
            // 
            this.button2_2.Location = new System.Drawing.Point(234, 132);
            this.button2_2.Name = "button2_2";
            this.button2_2.Size = new System.Drawing.Size(75, 23);
            this.button2_2.TabIndex = 6;
            this.button2_2.Text = "确 定";
            this.button2_2.UseVisualStyleBackColor = true;
            this.button2_2.Click += new System.EventHandler(this.button2_2_Click);
            // 
            // checkBox3_2
            // 
            this.checkBox3_2.AutoSize = true;
            this.checkBox3_2.Location = new System.Drawing.Point(315, 139);
            this.checkBox3_2.Name = "checkBox3_2";
            this.checkBox3_2.Size = new System.Drawing.Size(84, 16);
            this.checkBox3_2.TabIndex = 5;
            this.checkBox3_2.Text = "所有数据库";
            this.checkBox3_2.UseVisualStyleBackColor = true;
            // 
            // button1_2
            // 
            this.button1_2.Location = new System.Drawing.Point(69, 132);
            this.button1_2.Name = "button1_2";
            this.button1_2.Size = new System.Drawing.Size(75, 23);
            this.button1_2.TabIndex = 20;
            this.button1_2.TabStop = false;
            this.button1_2.Text = "测试连接";
            this.button1_2.UseVisualStyleBackColor = true;
            this.button1_2.Visible = false;
            // 
            // txtpassWord2
            // 
            this.txtpassWord2.Location = new System.Drawing.Point(118, 87);
            this.txtpassWord2.Name = "txtpassWord2";
            this.txtpassWord2.PasswordChar = '*';
            this.txtpassWord2.Size = new System.Drawing.Size(274, 21);
            this.txtpassWord2.TabIndex = 4;
            this.txtpassWord2.Tag = "Dcs$(~)20160308OAiTour!@0%(_";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(41, 94);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "密码";
            // 
            // txtuserName2
            // 
            this.txtuserName2.Location = new System.Drawing.Point(118, 37);
            this.txtuserName2.Name = "txtuserName2";
            this.txtuserName2.Size = new System.Drawing.Size(274, 21);
            this.txtuserName2.TabIndex = 3;
            this.txtuserName2.Tag = "DCSV1.0SVN";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(41, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "用户名";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(872, 303);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 23);
            this.button3.TabIndex = 20;
            this.button3.Text = "开  始";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // cbox_view
            // 
            this.cbox_view.AutoSize = true;
            this.cbox_view.Checked = true;
            this.cbox_view.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbox_view.Location = new System.Drawing.Point(747, 310);
            this.cbox_view.Name = "cbox_view";
            this.cbox_view.Size = new System.Drawing.Size(48, 16);
            this.cbox_view.TabIndex = 26;
            this.cbox_view.Text = "视图";
            this.cbox_view.UseVisualStyleBackColor = true;
            // 
            // cbox_proc
            // 
            this.cbox_proc.AutoSize = true;
            this.cbox_proc.Checked = true;
            this.cbox_proc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbox_proc.Location = new System.Drawing.Point(673, 310);
            this.cbox_proc.Name = "cbox_proc";
            this.cbox_proc.Size = new System.Drawing.Size(72, 16);
            this.cbox_proc.TabIndex = 26;
            this.cbox_proc.Text = "存储过程";
            this.cbox_proc.UseVisualStyleBackColor = true;
            // 
            // cbox_u
            // 
            this.cbox_u.AutoSize = true;
            this.cbox_u.Checked = true;
            this.cbox_u.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbox_u.Location = new System.Drawing.Point(609, 310);
            this.cbox_u.Name = "cbox_u";
            this.cbox_u.Size = new System.Drawing.Size(60, 16);
            this.cbox_u.TabIndex = 26;
            this.cbox_u.Text = "用户表";
            this.cbox_u.UseVisualStyleBackColor = true;
            // 
            // cbox_tr
            // 
            this.cbox_tr.AutoSize = true;
            this.cbox_tr.Checked = true;
            this.cbox_tr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbox_tr.Location = new System.Drawing.Point(797, 310);
            this.cbox_tr.Name = "cbox_tr";
            this.cbox_tr.Size = new System.Drawing.Size(60, 16);
            this.cbox_tr.TabIndex = 26;
            this.cbox_tr.Text = "触发器";
            this.cbox_tr.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(2, 357);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(383, 12);
            this.linkLabel1.TabIndex = 28;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "注：谨慎使用该工具生成的SQL，造成的后果本软件以及开发者概不负责";
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.Location = new System.Drawing.Point(471, 125);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(40, 86);
            this.button4.TabIndex = 20;
            this.button4.TabStop = false;
            this.button4.Text = "<=>";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(161, 300);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(155, 21);
            this.dateTimePicker1.TabIndex = 29;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(61, 308);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 30;
            this.label10.Text = "更新时间";
            // 
            // CheckSTime
            // 
            this.CheckSTime.AutoSize = true;
            this.CheckSTime.Location = new System.Drawing.Point(139, 304);
            this.CheckSTime.Name = "CheckSTime";
            this.CheckSTime.Size = new System.Drawing.Size(15, 14);
            this.CheckSTime.TabIndex = 31;
            this.CheckSTime.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(325, 307);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 32;
            this.label11.Text = "至今";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(991, 373);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.CheckSTime);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.cbox_u);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.cbox_proc);
            this.Controls.Add(this.cbox_tr);
            this.Controls.Add(this.cbox_view);
            this.Controls.Add(this.comBoxDBList2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtServerAddress2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.lblCurrState);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.comBoxDBList);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtServerAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据库升级助手(简易版) - 作者：谭盼(1301236335)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServerAddress;
        private System.Windows.Forms.TextBox txtpassWord;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtuserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.ComboBox comBoxDBList;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblCurrState;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.ComboBox comBoxDBList2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtServerAddress2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button2_2;
        private System.Windows.Forms.CheckBox checkBox3_2;
        private System.Windows.Forms.Button button1_2;
        private System.Windows.Forms.TextBox txtpassWord2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtuserName2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox cbox_view;
        private System.Windows.Forms.CheckBox cbox_proc;
        private System.Windows.Forms.CheckBox cbox_u;
        private System.Windows.Forms.CheckBox cbox_tr;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox CheckSTime;
        private System.Windows.Forms.Label label11;
    }
}

