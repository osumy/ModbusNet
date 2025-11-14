namespace Modbus.Forms
{
    partial class MainForm
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
            buttonConnectionSettings = new Button();
            textBoxSlaveId = new TextBox();
            connectBtn = new Button();
            groupBox1 = new GroupBox();
            labelConnectionStatus = new Label();
            groupBoxHoldingRegisters = new GroupBox();
            richTextBoxResult = new RichTextBox();
            textBoxValue = new TextBox();
            textBoxRegisterNum = new TextBox();
            textBoxStartAddress = new TextBox();
            buttonWrite = new Button();
            buttonRead = new Button();
            groupBox1.SuspendLayout();
            groupBoxHoldingRegisters.SuspendLayout();
            SuspendLayout();
            // 
            // buttonConnectionSettings
            // 
            buttonConnectionSettings.Location = new Point(12, 12);
            buttonConnectionSettings.Name = "buttonConnectionSettings";
            buttonConnectionSettings.Size = new Size(154, 52);
            buttonConnectionSettings.TabIndex = 0;
            buttonConnectionSettings.Text = "Connection Settings";
            buttonConnectionSettings.UseVisualStyleBackColor = true;
            buttonConnectionSettings.Click += buttonConnectionSettings_Click;
            // 
            // textBoxSlaveId
            // 
            textBoxSlaveId.Location = new Point(12, 22);
            textBoxSlaveId.Name = "textBoxSlaveId";
            textBoxSlaveId.Size = new Size(60, 23);
            textBoxSlaveId.TabIndex = 1;
            textBoxSlaveId.Text = "1";
            // 
            // connectBtn
            // 
            connectBtn.Location = new Point(78, 21);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new Size(60, 24);
            connectBtn.TabIndex = 2;
            connectBtn.Text = "Connect";
            connectBtn.UseVisualStyleBackColor = true;
            connectBtn.Click += connectBtn_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(labelConnectionStatus);
            groupBox1.Controls.Add(textBoxSlaveId);
            groupBox1.Controls.Add(connectBtn);
            groupBox1.Location = new Point(12, 93);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(154, 74);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Slave ID";
            // 
            // labelConnectionStatus
            // 
            labelConnectionStatus.AutoSize = true;
            labelConnectionStatus.Font = new Font("Segoe UI", 8F);
            labelConnectionStatus.Location = new Point(10, 52);
            labelConnectionStatus.Name = "labelConnectionStatus";
            labelConnectionStatus.Size = new Size(88, 13);
            labelConnectionStatus.TabIndex = 3;
            labelConnectionStatus.Text = "Not Connected.";
            // 
            // groupBoxHoldingRegisters
            // 
            groupBoxHoldingRegisters.Controls.Add(richTextBoxResult);
            groupBoxHoldingRegisters.Controls.Add(textBoxValue);
            groupBoxHoldingRegisters.Controls.Add(textBoxRegisterNum);
            groupBoxHoldingRegisters.Controls.Add(textBoxStartAddress);
            groupBoxHoldingRegisters.Controls.Add(buttonWrite);
            groupBoxHoldingRegisters.Controls.Add(buttonRead);
            groupBoxHoldingRegisters.Location = new Point(12, 184);
            groupBoxHoldingRegisters.Name = "groupBoxHoldingRegisters";
            groupBoxHoldingRegisters.Size = new Size(307, 222);
            groupBoxHoldingRegisters.TabIndex = 4;
            groupBoxHoldingRegisters.TabStop = false;
            groupBoxHoldingRegisters.Text = "Holding Registers";
            // 
            // richTextBoxResult
            // 
            richTextBoxResult.Location = new Point(6, 82);
            richTextBoxResult.Name = "richTextBoxResult";
            richTextBoxResult.Size = new Size(291, 140);
            richTextBoxResult.TabIndex = 4;
            richTextBoxResult.Text = "";
            // 
            // textBoxValue
            // 
            textBoxValue.Location = new Point(6, 51);
            textBoxValue.Name = "textBoxValue";
            textBoxValue.Size = new Size(100, 23);
            textBoxValue.TabIndex = 3;
            // 
            // textBoxRegisterNum
            // 
            textBoxRegisterNum.Location = new Point(116, 22);
            textBoxRegisterNum.Name = "textBoxRegisterNum";
            textBoxRegisterNum.Size = new Size(100, 23);
            textBoxRegisterNum.TabIndex = 2;
            // 
            // textBoxStartAddress
            // 
            textBoxStartAddress.Location = new Point(6, 22);
            textBoxStartAddress.Name = "textBoxStartAddress";
            textBoxStartAddress.Size = new Size(100, 23);
            textBoxStartAddress.TabIndex = 1;
            // 
            // buttonWrite
            // 
            buttonWrite.Location = new Point(222, 51);
            buttonWrite.Name = "buttonWrite";
            buttonWrite.Size = new Size(75, 23);
            buttonWrite.TabIndex = 0;
            buttonWrite.Text = "Write";
            buttonWrite.UseVisualStyleBackColor = true;
            buttonWrite.Click += buttonWrite_Click;
            // 
            // buttonRead
            // 
            buttonRead.Location = new Point(222, 21);
            buttonRead.Name = "buttonRead";
            buttonRead.Size = new Size(75, 23);
            buttonRead.TabIndex = 0;
            buttonRead.Text = "Read";
            buttonRead.UseVisualStyleBackColor = true;
            buttonRead.Click += buttonRead_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBoxHoldingRegisters);
            Controls.Add(groupBox1);
            Controls.Add(buttonConnectionSettings);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainForm";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBoxHoldingRegisters.ResumeLayout(false);
            groupBoxHoldingRegisters.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button buttonConnectionSettings;
        private TextBox textBoxSlaveId;
        private Button connectBtn;
        private GroupBox groupBox1;
        private Label labelConnectionStatus;
        private GroupBox groupBoxHoldingRegisters;
        private Button buttonRead;
        private TextBox textBoxStartAddress;
        private TextBox textBoxValue;
        private TextBox textBoxRegisterNum;
        private Button buttonWrite;
        private RichTextBox richTextBoxResult;
    }
}