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
            textBoxCount = new TextBox();
            textBoxStartAddress = new TextBox();
            groupBox2 = new GroupBox();
            dataGridViewValues = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            buttonExecute = new Button();
            checkBoxCoilValue = new CheckBox();
            labelCount = new Label();
            labelStartAddress = new Label();
            numericUpDownCount = new NumericUpDown();
            comboBoxFC = new ComboBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewValues).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCount).BeginInit();
            SuspendLayout();
            // 
            // buttonConnectionSettings
            // 
            buttonConnectionSettings.Location = new Point(12, 13);
            buttonConnectionSettings.Margin = new Padding(3, 4, 3, 4);
            buttonConnectionSettings.Name = "buttonConnectionSettings";
            buttonConnectionSettings.Size = new Size(178, 69);
            buttonConnectionSettings.TabIndex = 0;
            buttonConnectionSettings.Text = "Connection Settings";
            buttonConnectionSettings.UseVisualStyleBackColor = true;
            buttonConnectionSettings.Click += buttonConnectionSettings_Click;
            // 
            // textBoxSlaveId
            // 
            textBoxSlaveId.Location = new Point(14, 29);
            textBoxSlaveId.Margin = new Padding(3, 4, 3, 4);
            textBoxSlaveId.Name = "textBoxSlaveId";
            textBoxSlaveId.Size = new Size(52, 27);
            textBoxSlaveId.TabIndex = 1;
            textBoxSlaveId.Text = "1";
            // 
            // connectBtn
            // 
            connectBtn.Location = new Point(84, 28);
            connectBtn.Margin = new Padding(3, 4, 3, 4);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new Size(86, 28);
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
            groupBox1.Location = new Point(14, 100);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(176, 99);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Slave ID";
            // 
            // labelConnectionStatus
            // 
            labelConnectionStatus.AutoSize = true;
            labelConnectionStatus.Font = new Font("Segoe UI", 8F);
            labelConnectionStatus.Location = new Point(11, 69);
            labelConnectionStatus.Name = "labelConnectionStatus";
            labelConnectionStatus.Size = new Size(105, 19);
            labelConnectionStatus.TabIndex = 3;
            labelConnectionStatus.Text = "Not Connected.";
            // 
            // textBoxCount
            // 
            textBoxCount.Location = new Point(587, 26);
            textBoxCount.Margin = new Padding(3, 4, 3, 4);
            textBoxCount.Name = "textBoxCount";
            textBoxCount.Size = new Size(128, 27);
            textBoxCount.TabIndex = 3;
            textBoxCount.Visible = false;
            // 
            // textBoxStartAddress
            // 
            textBoxStartAddress.Location = new Point(379, 26);
            textBoxStartAddress.Margin = new Padding(3, 4, 3, 4);
            textBoxStartAddress.Name = "textBoxStartAddress";
            textBoxStartAddress.Size = new Size(128, 27);
            textBoxStartAddress.TabIndex = 1;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(dataGridViewValues);
            groupBox2.Controls.Add(buttonExecute);
            groupBox2.Controls.Add(checkBoxCoilValue);
            groupBox2.Controls.Add(textBoxCount);
            groupBox2.Controls.Add(labelCount);
            groupBox2.Controls.Add(labelStartAddress);
            groupBox2.Controls.Add(numericUpDownCount);
            groupBox2.Controls.Add(textBoxStartAddress);
            groupBox2.Controls.Add(comboBoxFC);
            groupBox2.Location = new Point(14, 206);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(888, 382);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Function";
            // 
            // dataGridViewValues
            // 
            dataGridViewValues.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewValues.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2 });
            dataGridViewValues.Location = new Point(6, 60);
            dataGridViewValues.Name = "dataGridViewValues";
            dataGridViewValues.RowHeadersWidth = 51;
            dataGridViewValues.Size = new Size(876, 316);
            dataGridViewValues.TabIndex = 6;
            // 
            // Column1
            // 
            Column1.HeaderText = "Column1";
            Column1.MinimumWidth = 6;
            Column1.Name = "Column1";
            Column1.Width = 125;
            // 
            // Column2
            // 
            Column2.HeaderText = "Column2";
            Column2.MinimumWidth = 6;
            Column2.Name = "Column2";
            Column2.Width = 125;
            // 
            // buttonExecute
            // 
            buttonExecute.Location = new Point(775, 24);
            buttonExecute.Name = "buttonExecute";
            buttonExecute.Size = new Size(107, 29);
            buttonExecute.TabIndex = 5;
            buttonExecute.Text = "123";
            buttonExecute.UseVisualStyleBackColor = true;
            buttonExecute.Click += buttonExecute_Click;
            // 
            // checkBoxCoilValue
            // 
            checkBoxCoilValue.AutoSize = true;
            checkBoxCoilValue.Location = new Point(645, 31);
            checkBoxCoilValue.Name = "checkBoxCoilValue";
            checkBoxCoilValue.Size = new Size(18, 17);
            checkBoxCoilValue.TabIndex = 4;
            checkBoxCoilValue.UseVisualStyleBackColor = true;
            checkBoxCoilValue.Visible = false;
            // 
            // labelCount
            // 
            labelCount.AutoSize = true;
            labelCount.Location = new Point(513, 28);
            labelCount.Name = "labelCount";
            labelCount.Size = new Size(68, 20);
            labelCount.TabIndex = 3;
            labelCount.Text = "Quantity:";
            // 
            // labelStartAddress
            // 
            labelStartAddress.AutoSize = true;
            labelStartAddress.Location = new Point(273, 29);
            labelStartAddress.Name = "labelStartAddress";
            labelStartAddress.Size = new Size(100, 20);
            labelStartAddress.TabIndex = 2;
            labelStartAddress.Text = "Start Address:";
            // 
            // numericUpDownCount
            // 
            numericUpDownCount.Location = new Point(601, 27);
            numericUpDownCount.Name = "numericUpDownCount";
            numericUpDownCount.Size = new Size(114, 27);
            numericUpDownCount.TabIndex = 1;
            // 
            // comboBoxFC
            // 
            comboBoxFC.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFC.FormattingEnabled = true;
            comboBoxFC.Location = new Point(6, 26);
            comboBoxFC.Name = "comboBoxFC";
            comboBoxFC.Size = new Size(249, 28);
            comboBoxFC.TabIndex = 0;
            comboBoxFC.SelectedIndexChanged += comboBoxFC_SelectedIndexChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(buttonConnectionSettings);
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainForm";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewValues).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCount).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button buttonConnectionSettings;
        private TextBox textBoxSlaveId;
        private Button connectBtn;
        private GroupBox groupBox1;
        private Label labelConnectionStatus;
        private TextBox textBoxStartAddress;
        private TextBox textBoxCount;
        private GroupBox groupBox2;
        private ComboBox comboBoxFC;
        private DataGridView dataGridViewValues;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private NumericUpDown numericUpDownCount;
        private Label labelStartAddress;
        private Label labelCount;
        private CheckBox checkBoxCoilValue;
        private Button buttonExecute;
    }
}