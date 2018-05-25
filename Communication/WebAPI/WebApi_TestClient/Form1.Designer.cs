namespace WebApi_TestClient
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTest2 = new System.Windows.Forms.Button();
            this.btnTest1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnGetGroupsByIp = new System.Windows.Forms.Button();
            this.txtGetDeviceByIp = new System.Windows.Forms.Button();
            this.txtGroupsIp = new System.Windows.Forms.TextBox();
            this.txtDeviceIp = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnListar = new System.Windows.Forms.Button();
            this.txtListarDevice = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtListarUser = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnListPending = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtListPendingGroups = new System.Windows.Forms.TextBox();
            this.txtListPendingDevice = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtListPendingUser = new System.Windows.Forms.TextBox();
            this.btnListActiveRules = new System.Windows.Forms.Button();
            this.txtActiveRules = new System.Windows.Forms.TextBox();
            this.btnMarkActionStateAs = new System.Windows.Forms.Button();
            this.txtMarkActionState = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMarkActionDevice = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMarkActionUser = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnGetGroupNames = new System.Windows.Forms.Button();
            this.txtGetGroupNames = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstTraceLog = new System.Windows.Forms.ListBox();
            this.btnGetTest = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnTest2);
            this.groupBox1.Controls.Add(this.btnTest1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(122, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pruebas básicas";
            // 
            // btnTest2
            // 
            this.btnTest2.Location = new System.Drawing.Point(6, 48);
            this.btnTest2.Name = "btnTest2";
            this.btnTest2.Size = new System.Drawing.Size(110, 23);
            this.btnTest2.TabIndex = 1;
            this.btnTest2.Text = "Prueba 2";
            this.btnTest2.UseVisualStyleBackColor = true;
            this.btnTest2.Click += new System.EventHandler(this.btnTest2_Click);
            // 
            // btnTest1
            // 
            this.btnTest1.Location = new System.Drawing.Point(6, 19);
            this.btnTest1.Name = "btnTest1";
            this.btnTest1.Size = new System.Drawing.Size(110, 23);
            this.btnTest1.TabIndex = 0;
            this.btnTest1.Text = "Prueba 1";
            this.btnTest1.UseVisualStyleBackColor = true;
            this.btnTest1.Click += new System.EventHandler(this.btnTest1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnGetGroupsByIp);
            this.groupBox2.Controls.Add(this.txtGetDeviceByIp);
            this.groupBox2.Controls.Add(this.txtGroupsIp);
            this.groupBox2.Controls.Add(this.txtDeviceIp);
            this.groupBox2.Location = new System.Drawing.Point(13, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(241, 76);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Device Client";
            // 
            // btnGetGroupsByIp
            // 
            this.btnGetGroupsByIp.Location = new System.Drawing.Point(112, 44);
            this.btnGetGroupsByIp.Name = "btnGetGroupsByIp";
            this.btnGetGroupsByIp.Size = new System.Drawing.Size(123, 23);
            this.btnGetGroupsByIp.TabIndex = 3;
            this.btnGetGroupsByIp.Text = "GetGroupsByIp";
            this.btnGetGroupsByIp.UseVisualStyleBackColor = true;
            this.btnGetGroupsByIp.Click += new System.EventHandler(this.btnGetGroupsByIp_Click);
            // 
            // txtGetDeviceByIp
            // 
            this.txtGetDeviceByIp.Location = new System.Drawing.Point(112, 19);
            this.txtGetDeviceByIp.Name = "txtGetDeviceByIp";
            this.txtGetDeviceByIp.Size = new System.Drawing.Size(123, 23);
            this.txtGetDeviceByIp.TabIndex = 2;
            this.txtGetDeviceByIp.Text = "GetDeviceByIp";
            this.txtGetDeviceByIp.UseVisualStyleBackColor = true;
            this.txtGetDeviceByIp.Click += new System.EventHandler(this.txtGetDeviceByIp_Click);
            // 
            // txtGroupsIp
            // 
            this.txtGroupsIp.Location = new System.Drawing.Point(7, 46);
            this.txtGroupsIp.Name = "txtGroupsIp";
            this.txtGroupsIp.Size = new System.Drawing.Size(100, 20);
            this.txtGroupsIp.TabIndex = 1;
            this.txtGroupsIp.Text = "127.0.0.1";
            // 
            // txtDeviceIp
            // 
            this.txtDeviceIp.Location = new System.Drawing.Point(6, 19);
            this.txtDeviceIp.Name = "txtDeviceIp";
            this.txtDeviceIp.Size = new System.Drawing.Size(100, 20);
            this.txtDeviceIp.TabIndex = 0;
            this.txtDeviceIp.Text = "127.0.0.1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnListar);
            this.groupBox3.Controls.Add(this.txtListarDevice);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.txtListarUser);
            this.groupBox3.Location = new System.Drawing.Point(12, 256);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(242, 100);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Historic Client";
            // 
            // btnListar
            // 
            this.btnListar.Location = new System.Drawing.Point(114, 68);
            this.btnListar.Name = "btnListar";
            this.btnListar.Size = new System.Drawing.Size(122, 23);
            this.btnListar.TabIndex = 4;
            this.btnListar.Text = "Listar";
            this.btnListar.UseVisualStyleBackColor = true;
            this.btnListar.Click += new System.EventHandler(this.btnListar_Click);
            // 
            // txtListarDevice
            // 
            this.txtListarDevice.Location = new System.Drawing.Point(8, 71);
            this.txtListarDevice.Name = "txtListarDevice";
            this.txtListarDevice.Size = new System.Drawing.Size(100, 20);
            this.txtListarDevice.TabIndex = 3;
            this.txtListarDevice.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Device:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "User:";
            // 
            // txtListarUser
            // 
            this.txtListarUser.Location = new System.Drawing.Point(6, 32);
            this.txtListarUser.Name = "txtListarUser";
            this.txtListarUser.Size = new System.Drawing.Size(100, 20);
            this.txtListarUser.TabIndex = 0;
            this.txtListarUser.Text = "5";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnListPending);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.txtListPendingGroups);
            this.groupBox4.Controls.Add(this.txtListPendingDevice);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.txtListPendingUser);
            this.groupBox4.Controls.Add(this.btnListActiveRules);
            this.groupBox4.Controls.Add(this.txtActiveRules);
            this.groupBox4.Controls.Add(this.btnMarkActionStateAs);
            this.groupBox4.Controls.Add(this.txtMarkActionState);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.txtMarkActionDevice);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.txtMarkActionUser);
            this.groupBox4.Location = new System.Drawing.Point(13, 362);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(241, 390);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "RuleMotor Client";
            // 
            // btnListPending
            // 
            this.btnListPending.Location = new System.Drawing.Point(114, 256);
            this.btnListPending.Name = "btnListPending";
            this.btnListPending.Size = new System.Drawing.Size(122, 23);
            this.btnListPending.TabIndex = 19;
            this.btnListPending.Text = "ListPending";
            this.btnListPending.UseVisualStyleBackColor = true;
            this.btnListPending.Click += new System.EventHandler(this.btnListPending_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 242);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Groups:";
            // 
            // txtListPendingGroups
            // 
            this.txtListPendingGroups.Location = new System.Drawing.Point(8, 258);
            this.txtListPendingGroups.Name = "txtListPendingGroups";
            this.txtListPendingGroups.Size = new System.Drawing.Size(100, 20);
            this.txtListPendingGroups.TabIndex = 17;
            this.txtListPendingGroups.Text = "1;2;3";
            // 
            // txtListPendingDevice
            // 
            this.txtListPendingDevice.Location = new System.Drawing.Point(8, 219);
            this.txtListPendingDevice.Name = "txtListPendingDevice";
            this.txtListPendingDevice.Size = new System.Drawing.Size(100, 20);
            this.txtListPendingDevice.TabIndex = 16;
            this.txtListPendingDevice.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 203);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Device:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 164);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "User:";
            // 
            // txtListPendingUser
            // 
            this.txtListPendingUser.Location = new System.Drawing.Point(6, 180);
            this.txtListPendingUser.Name = "txtListPendingUser";
            this.txtListPendingUser.Size = new System.Drawing.Size(100, 20);
            this.txtListPendingUser.TabIndex = 13;
            this.txtListPendingUser.Text = "5";
            // 
            // btnListActiveRules
            // 
            this.btnListActiveRules.Location = new System.Drawing.Point(112, 139);
            this.btnListActiveRules.Name = "btnListActiveRules";
            this.btnListActiveRules.Size = new System.Drawing.Size(122, 23);
            this.btnListActiveRules.TabIndex = 12;
            this.btnListActiveRules.Text = "ListActiveRules";
            this.btnListActiveRules.UseVisualStyleBackColor = true;
            this.btnListActiveRules.Click += new System.EventHandler(this.btnListActiveRules_Click);
            // 
            // txtActiveRules
            // 
            this.txtActiveRules.Location = new System.Drawing.Point(6, 141);
            this.txtActiveRules.Name = "txtActiveRules";
            this.txtActiveRules.Size = new System.Drawing.Size(100, 20);
            this.txtActiveRules.TabIndex = 11;
            this.txtActiveRules.Text = "16;18;50;51;52";
            // 
            // btnMarkActionStateAs
            // 
            this.btnMarkActionStateAs.Location = new System.Drawing.Point(112, 110);
            this.btnMarkActionStateAs.Name = "btnMarkActionStateAs";
            this.btnMarkActionStateAs.Size = new System.Drawing.Size(122, 23);
            this.btnMarkActionStateAs.TabIndex = 5;
            this.btnMarkActionStateAs.Text = "MarkActionStateAs";
            this.btnMarkActionStateAs.UseVisualStyleBackColor = true;
            this.btnMarkActionStateAs.Click += new System.EventHandler(this.btnMarkActionStateAs_Click);
            // 
            // txtMarkActionState
            // 
            this.txtMarkActionState.Location = new System.Drawing.Point(7, 110);
            this.txtMarkActionState.Name = "txtMarkActionState";
            this.txtMarkActionState.Size = new System.Drawing.Size(100, 20);
            this.txtMarkActionState.TabIndex = 10;
            this.txtMarkActionState.Text = "5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 94);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "State:";
            // 
            // txtMarkActionDevice
            // 
            this.txtMarkActionDevice.Location = new System.Drawing.Point(8, 71);
            this.txtMarkActionDevice.Name = "txtMarkActionDevice";
            this.txtMarkActionDevice.Size = new System.Drawing.Size(100, 20);
            this.txtMarkActionDevice.TabIndex = 8;
            this.txtMarkActionDevice.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "User:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Device:";
            // 
            // txtMarkActionUser
            // 
            this.txtMarkActionUser.Location = new System.Drawing.Point(6, 32);
            this.txtMarkActionUser.Name = "txtMarkActionUser";
            this.txtMarkActionUser.Size = new System.Drawing.Size(100, 20);
            this.txtMarkActionUser.TabIndex = 5;
            this.txtMarkActionUser.Text = "5";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnGetGroupNames);
            this.groupBox5.Controls.Add(this.txtGetGroupNames);
            this.groupBox5.Location = new System.Drawing.Point(13, 201);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(241, 49);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Group Client";
            // 
            // btnGetGroupNames
            // 
            this.btnGetGroupNames.Location = new System.Drawing.Point(112, 17);
            this.btnGetGroupNames.Name = "btnGetGroupNames";
            this.btnGetGroupNames.Size = new System.Drawing.Size(123, 23);
            this.btnGetGroupNames.TabIndex = 4;
            this.btnGetGroupNames.Text = "GetGroupNames";
            this.btnGetGroupNames.UseVisualStyleBackColor = true;
            this.btnGetGroupNames.Click += new System.EventHandler(this.btnGetGroupNames_Click);
            // 
            // txtGetGroupNames
            // 
            this.txtGetGroupNames.Location = new System.Drawing.Point(7, 19);
            this.txtGetGroupNames.Name = "txtGetGroupNames";
            this.txtGetGroupNames.Size = new System.Drawing.Size(100, 20);
            this.txtGetGroupNames.TabIndex = 0;
            this.txtGetGroupNames.Text = "1;2;3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(288, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Respuesta:";
            // 
            // lstTraceLog
            // 
            this.lstTraceLog.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstTraceLog.FormattingEnabled = true;
            this.lstTraceLog.Location = new System.Drawing.Point(291, 24);
            this.lstTraceLog.Margin = new System.Windows.Forms.Padding(2);
            this.lstTraceLog.Name = "lstTraceLog";
            this.lstTraceLog.ScrollAlwaysVisible = true;
            this.lstTraceLog.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstTraceLog.Size = new System.Drawing.Size(505, 173);
            this.lstTraceLog.TabIndex = 5;
            this.lstTraceLog.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstTraceLog_DrawItem);
            // 
            // btnGetTest
            // 
            this.btnGetTest.BackColor = System.Drawing.Color.Red;
            this.btnGetTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetTest.ForeColor = System.Drawing.Color.White;
            this.btnGetTest.Location = new System.Drawing.Point(310, 228);
            this.btnGetTest.Name = "btnGetTest";
            this.btnGetTest.Size = new System.Drawing.Size(133, 57);
            this.btnGetTest.TabIndex = 6;
            this.btnGetTest.Text = "GetTest";
            this.btnGetTest.UseVisualStyleBackColor = false;
            this.btnGetTest.Click += new System.EventHandler(this.btnGetTest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 764);
            this.Controls.Add(this.btnGetTest);
            this.Controls.Add(this.lstTraceLog);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "WebApi Test Client";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnTest2;
        private System.Windows.Forms.Button btnTest1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnGetGroupsByIp;
        private System.Windows.Forms.Button txtGetDeviceByIp;
        private System.Windows.Forms.TextBox txtGroupsIp;
        private System.Windows.Forms.TextBox txtDeviceIp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstTraceLog;
        private System.Windows.Forms.TextBox txtGetGroupNames;
        private System.Windows.Forms.Button btnGetGroupNames;
        private System.Windows.Forms.TextBox txtListarDevice;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtListarUser;
        private System.Windows.Forms.Button btnListar;
        private System.Windows.Forms.Button btnMarkActionStateAs;
        private System.Windows.Forms.TextBox txtMarkActionState;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMarkActionDevice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMarkActionUser;
        private System.Windows.Forms.Button btnListActiveRules;
        private System.Windows.Forms.TextBox txtActiveRules;
        private System.Windows.Forms.Button btnListPending;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtListPendingGroups;
        private System.Windows.Forms.TextBox txtListPendingDevice;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtListPendingUser;
        private System.Windows.Forms.Button btnGetTest;
    }
}

