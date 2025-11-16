using ModbusNet;
using ModbusUI.Services;

namespace Modbus.Forms
{
    public partial class MainForm : Form
    {
        private ModbusSettings _settings;
        private ModbusService _modbusService;
        private byte _slaveId;

        public MainForm()
        {
            InitializeComponent();

            _settings = ModbusSettings.Default;
            _modbusService = new ModbusService();

            // Initialize your ComboBox items
            comboBoxFC.Items.AddRange(new object[] {
                new { Text = "Read Coils (0x01)", Value = 1 },
                new { Text = "Read Discrete Inputs (0x02)", Value = 2 },
                new { Text = "Read Holding Registers (0x03)", Value = 3 },
                new { Text = "Read Input Registers (0x04)", Value = 4 },
                new { Text = "Write Single Coil (0x05)", Value = 5 },
                new { Text = "Write Single Register (0x06)", Value = 6 },
                new { Text = "Write Multiple Coils (0x0F)", Value = 15 },
                new { Text = "Write Multiple Registers (0x10)", Value = 16 }
            });

            // Set display member and value member
            comboBoxFC.DisplayMember = "Text";
            comboBoxFC.ValueMember = "Value";

            // Set default selection
            comboBoxFC.SelectedIndex = 2;
        }

        private void buttonConnectionSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(_settings);
            settingsForm.ShowDialog();

            if (settingsForm.DialogResult == DialogResult.OK)
            {
                _settings = settingsForm.Settings;
            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            int id = 0;
            string txt = textBoxSlaveId.Text?.Trim();
            if (!int.TryParse(txt, out id) || id <= 0)
            {
                MessageBox.Show("Please enter a valid numeric slave id.", "Invalid Slave ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                bool res = _modbusService.Connect(_settings);
                if (!res)
                {
                    MessageBox.Show("Connection Failed!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    labelConnectionStatus.Text = "Not Connected.";
                    labelConnectionStatus.ForeColor = SystemColors.ControlText;
                    textBoxSlaveId.Enabled = true;
                }
                else
                {
                    labelConnectionStatus.Text = "Connected!";
                    labelConnectionStatus.ForeColor = Color.Green;
                    textBoxSlaveId.Enabled = false;
                    _slaveId = Byte.Parse(txt);
                }
            }
        }


        private void comboBoxFC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxFC.SelectedItem == null) return;

            // Get the selected function code
            var selectedItem = comboBoxFC.SelectedItem;
            var functionCode = (int)selectedItem.GetType().GetProperty("Value").GetValue(selectedItem);

            UpdateUIForFunctionCode(functionCode);
        }

        private void UpdateUIForFunctionCode(int functionCode)
        {
            // Hide all optional controls first
            HideAllOptionalControls();

            switch (functionCode)
            {
                case 1: // Read Coils
                case 2: // Read Discrete Inputs
                    ShowReadDiscreteControls();
                    break;

                case 3: // Read Holding Registers
                case 4: // Read Input Registers
                    ShowReadRegisterControls();
                    break;

                case 5: // Write Single Coil
                    ShowWriteSingleCoilControls();
                    break;

                case 6: // Write Single Register
                    ShowWriteSingleRegisterControls();
                    break;

                case 15: // Write Multiple Coils
                    ShowWriteMultipleCoilsControls();
                    break;

                case 16: // Write Multiple Registers
                    ShowWriteMultipleRegistersControls();
                    break;
            }

            UpdateButtonText(functionCode);
        }

        private void HideAllOptionalControls()
        {
            // Hide controls that are not common to all functions
            labelCount.Visible = false;
            numericUpDownCount.Visible = false;
            checkBoxCoilValue.Visible = false;
            textBoxCount.Visible = false;

            dataGridViewValues.Visible = false;
            //labelData.Visible = false;
            //textBoxData.Visible = false;
        }

        private void ShowReadDiscreteControls()
        {
            // For reading coils/discrete inputs
            labelStartAddress.Text = "Start Address:";
            labelCount.Text = "Quantity:";
            labelCount.Visible = true;
            numericUpDownCount.Visible = true;
            numericUpDownCount.Minimum = 1;
            numericUpDownCount.Maximum = 2000; // Modbus limit for coils
        }

        private void ShowReadRegisterControls()
        {
            // For reading holding/input registers
            labelStartAddress.Text = "Start Address:";
            labelCount.Text = "Quantity:";
            labelCount.Visible = true;
            numericUpDownCount.Visible = true;
            numericUpDownCount.Minimum = 1;
            numericUpDownCount.Maximum = 125; // Modbus limit for registers
        }

        private void ShowWriteSingleCoilControls()
        {
            labelStartAddress.Text = "Coil Address:";
            labelCount.Text = "Coil Value:";
            labelCount.Visible = true;

            // Use checkbox for coil value instead of textbox
            checkBoxCoilValue.Visible = true;
        }

        private void ShowWriteSingleRegisterControls()
        {
            labelStartAddress.Text = "Address:";
            labelCount.Text = "Value:";
            labelCount.Visible = true;
            textBoxCount.Visible = true;
            textBoxCount.Text = "0";
        }

        private void ShowWriteMultipleCoilsControls()
        {
            labelStartAddress.Text = "Start Address:";
            labelCount.Text = "Quantity:";
            labelCount.Visible = true;
            numericUpDownCount.Visible = true;

            // Show data grid for multiple coil values
            dataGridViewValues.Visible = true;
            SetupCoilsDataGrid();
        }

        private void ShowWriteMultipleRegistersControls()
        {
            labelStartAddress.Text = "Start Address:";
            labelCount.Text = "Quantity:";
            labelCount.Visible = true;
            numericUpDownCount.Visible = true;

            // Show data grid or text box for multiple register values
            dataGridViewValues.Visible = true;
            SetupRegistersDataGrid();
        }

        private void UpdateButtonText(int functionCode)
        {
            string action = functionCode <= 4 ? "Read" : "Write";
            buttonExecute.Text = $"{action} Data";
        }

        private void SetupCoilsDataGrid()
        {
            dataGridViewValues.Rows.Clear();
            dataGridViewValues.Columns.Clear();

            dataGridViewValues.Columns.Add("Address", "Address");
            dataGridViewValues.Columns.Add("Value", "Value (True/False)");

            // Add rows based on quantity
            int quantity = (int)numericUpDownCount.Value;
            for (int i = 0; i < quantity; i++)
            {
                dataGridViewValues.Rows.Add(i, "False");
            }
        }

        private void SetupRegistersDataGrid()
        {
            dataGridViewValues.Rows.Clear();
            dataGridViewValues.Columns.Clear();

            dataGridViewValues.Columns.Add("Address", "Address");
            dataGridViewValues.Columns.Add("Value", "Value (0-65535)");

            // Add rows based on quantity
            int quantity = (int)numericUpDownCount.Value;
            for (int i = 0; i < quantity; i++)
            {
                dataGridViewValues.Rows.Add(i, "0");
            }
        }

        private void buttonExecute_Click(object sender, EventArgs e)
        {
            if (comboBoxFC.SelectedItem == null) return;

            var selectedItem = comboBoxFC.SelectedItem;
            var functionCode = (int)selectedItem.GetType().GetProperty("Value").GetValue(selectedItem);

            try
            {
                switch (functionCode)
                {
                    //case 1:
                    //    ReadCoils();
                    //    break;
                    //case 2:
                    //    ReadDiscreteInputs();
                    //    break;
                    //case 3:
                    //    ReadHoldingRegisters();
                    //    break;
                    //case 4:
                    //    ReadInputRegisters();
                    //    break;
                    //case 5:
                    //    WriteSingleCoil();
                    //    break;
                    //case 6:
                    //    WriteSingleRegister();
                    //    break;
                    //case 15:
                    //    WriteMultipleCoils();
                    //    break;
                    //case 16:
                    //    WriteMultipleRegisters();
                    //    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Modbus Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
