namespace Modbus.Forms
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            comboConnectionType = new ComboBox();
            groupBox1 = new GroupBox();
            okBtn = new Button();
            cancelBtn = new Button();
            groupBox2 = new GroupBox();
            textBoxCustomBaud = new TextBox();
            checkBoxCustomBaud = new CheckBox();
            comboStopBits = new ComboBox();
            comboParity = new ComboBox();
            comboDataBits = new ComboBox();
            comboBaudRate = new ComboBox();
            comboPortName = new ComboBox();
            groupBox3 = new GroupBox();
            radioButtonASCII = new RadioButton();
            radioButtonRTU = new RadioButton();
            groupBox4 = new GroupBox();
            label1 = new Label();
            respTBox = new TextBox();
            groupBox5 = new GroupBox();
            label2 = new Label();
            delayTBox = new TextBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // comboConnectionType
            // 
            comboConnectionType.DisplayMember = "Text";
            comboConnectionType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboConnectionType.FormattingEnabled = true;
            comboConnectionType.Location = new Point(12, 22);
            comboConnectionType.Name = "comboConnectionType";
            comboConnectionType.Size = new Size(281, 23);
            comboConnectionType.TabIndex = 1;
            comboConnectionType.ValueMember = "Value";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboConnectionType);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(312, 57);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Connection";
            // 
            // okBtn
            // 
            okBtn.Location = new Point(387, 17);
            okBtn.Name = "okBtn";
            okBtn.Size = new Size(75, 23);
            okBtn.TabIndex = 1;
            okBtn.Text = "OK";
            okBtn.UseVisualStyleBackColor = true;
            okBtn.Click += okBtn_Click;
            // 
            // cancelBtn
            // 
            cancelBtn.Location = new Point(387, 46);
            cancelBtn.Name = "cancelBtn";
            cancelBtn.Size = new Size(75, 23);
            cancelBtn.TabIndex = 4;
            cancelBtn.Text = "Cancel";
            cancelBtn.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(textBoxCustomBaud);
            groupBox2.Controls.Add(checkBoxCustomBaud);
            groupBox2.Controls.Add(comboStopBits);
            groupBox2.Controls.Add(comboParity);
            groupBox2.Controls.Add(comboDataBits);
            groupBox2.Controls.Add(comboBaudRate);
            groupBox2.Controls.Add(comboPortName);
            groupBox2.Location = new Point(12, 91);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(312, 172);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Serial Settings";
            // 
            // textBoxCustomBaud
            // 
            textBoxCustomBaud.Location = new Point(167, 80);
            textBoxCustomBaud.Name = "textBoxCustomBaud";
            textBoxCustomBaud.Size = new Size(100, 23);
            textBoxCustomBaud.TabIndex = 8;
            // 
            // checkBoxCustomBaud
            // 
            checkBoxCustomBaud.AutoSize = true;
            checkBoxCustomBaud.Location = new Point(157, 55);
            checkBoxCustomBaud.Name = "checkBoxCustomBaud";
            checkBoxCustomBaud.Size = new Size(124, 19);
            checkBoxCustomBaud.TabIndex = 7;
            checkBoxCustomBaud.Text = "Custom Baud Rate";
            checkBoxCustomBaud.UseVisualStyleBackColor = true;
            checkBoxCustomBaud.CheckedChanged += checkBoxCustomBaud_CheckedChanged;
            // 
            // comboStopBits
            // 
            comboStopBits.DisplayMember = "Text";
            comboStopBits.DropDownStyle = ComboBoxStyle.DropDownList;
            comboStopBits.FormattingEnabled = true;
            comboStopBits.Location = new Point(12, 138);
            comboStopBits.Name = "comboStopBits";
            comboStopBits.Size = new Size(117, 23);
            comboStopBits.TabIndex = 6;
            comboStopBits.ValueMember = "Value";
            // 
            // comboParity
            // 
            comboParity.DisplayMember = "Text";
            comboParity.DropDownStyle = ComboBoxStyle.DropDownList;
            comboParity.FormattingEnabled = true;
            comboParity.Location = new Point(12, 109);
            comboParity.Name = "comboParity";
            comboParity.Size = new Size(117, 23);
            comboParity.TabIndex = 5;
            comboParity.ValueMember = "Value";
            // 
            // comboDataBits
            // 
            comboDataBits.DisplayMember = "Text";
            comboDataBits.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDataBits.FormattingEnabled = true;
            comboDataBits.Location = new Point(12, 80);
            comboDataBits.Name = "comboDataBits";
            comboDataBits.Size = new Size(117, 23);
            comboDataBits.TabIndex = 4;
            comboDataBits.ValueMember = "Value";
            // 
            // comboBaudRate
            // 
            comboBaudRate.DisplayMember = "Text";
            comboBaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBaudRate.FormattingEnabled = true;
            comboBaudRate.Location = new Point(12, 51);
            comboBaudRate.Name = "comboBaudRate";
            comboBaudRate.Size = new Size(117, 23);
            comboBaudRate.TabIndex = 3;
            comboBaudRate.ValueMember = "Value";
            // 
            // comboPortName
            // 
            comboPortName.DisplayMember = "Text";
            comboPortName.DropDownStyle = ComboBoxStyle.DropDownList;
            comboPortName.FormattingEnabled = true;
            comboPortName.Location = new Point(12, 22);
            comboPortName.Name = "comboPortName";
            comboPortName.Size = new Size(281, 23);
            comboPortName.TabIndex = 2;
            comboPortName.ValueMember = "Value";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(radioButtonASCII);
            groupBox3.Controls.Add(radioButtonRTU);
            groupBox3.Location = new Point(330, 91);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(132, 50);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            groupBox3.Text = "Mode";
            // 
            // radioButtonASCII
            // 
            radioButtonASCII.AutoSize = true;
            radioButtonASCII.Location = new Point(64, 22);
            radioButtonASCII.Name = "radioButtonASCII";
            radioButtonASCII.Size = new Size(53, 19);
            radioButtonASCII.TabIndex = 8;
            radioButtonASCII.Text = "ASCII";
            radioButtonASCII.UseVisualStyleBackColor = true;
            // 
            // radioButtonRTU
            // 
            radioButtonRTU.AutoSize = true;
            radioButtonRTU.Checked = true;
            radioButtonRTU.Location = new Point(6, 22);
            radioButtonRTU.Name = "radioButtonRTU";
            radioButtonRTU.Size = new Size(45, 19);
            radioButtonRTU.TabIndex = 7;
            radioButtonRTU.TabStop = true;
            radioButtonRTU.Text = "RTU";
            radioButtonRTU.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(label1);
            groupBox4.Controls.Add(respTBox);
            groupBox4.Location = new Point(330, 152);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(132, 50);
            groupBox4.TabIndex = 7;
            groupBox4.TabStop = false;
            groupBox4.Text = "Response Timeout";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8F);
            label1.Location = new Point(99, 22);
            label1.Name = "label1";
            label1.Size = new Size(27, 13);
            label1.TabIndex = 8;
            label1.Text = "(ms)";
            // 
            // respTBox
            // 
            respTBox.Location = new Point(10, 18);
            respTBox.Name = "respTBox";
            respTBox.Size = new Size(85, 23);
            respTBox.TabIndex = 9;
            respTBox.Text = "2000";
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(label2);
            groupBox5.Controls.Add(delayTBox);
            groupBox5.Location = new Point(330, 213);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(132, 50);
            groupBox5.TabIndex = 10;
            groupBox5.TabStop = false;
            groupBox5.Text = "Delay Between Polls";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8F);
            label2.Location = new Point(99, 22);
            label2.Name = "label2";
            label2.Size = new Size(27, 13);
            label2.TabIndex = 8;
            label2.Text = "(ms)";
            // 
            // delayTBox
            // 
            delayTBox.Location = new Point(10, 18);
            delayTBox.Name = "delayTBox";
            delayTBox.Size = new Size(85, 23);
            delayTBox.TabIndex = 9;
            delayTBox.Text = "20";
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(474, 305);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(cancelBtn);
            Controls.Add(okBtn);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Connection Setup";
            TopMost = true;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ComboBox comboConnectionType;
        private GroupBox groupBox1;
        private Button okBtn;
        private Button cancelBtn;
        private GroupBox groupBox2;
        private ComboBox comboBaudRate;
        private ComboBox comboPortName;
        private TextBox textBoxCustomBaud;
        private CheckBox checkBoxCustomBaud;
        private ComboBox comboStopBits;
        private ComboBox comboParity;
        private ComboBox comboDataBits;
        private GroupBox groupBox3;
        private RadioButton radioButtonASCII;
        private RadioButton radioButtonRTU;
        private GroupBox groupBox4;
        private TextBox respTBox;
        private Label label1;
        private GroupBox groupBox5;
        private Label label2;
        private TextBox delayTBox;
    }
}