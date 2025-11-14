using Modbus.Models;
using Modbus.Services;
using static System.Net.Mime.MediaTypeNames;

namespace Modbus.Forms
{
    public partial class MainForm : Form
    {
        private ConnectionSettings _settings;
        private ModbusService _modbusService;
        private byte _slaveId;

        public MainForm()
        {
            InitializeComponent();

            _settings = new ConnectionSettings();
            _modbusService = new ModbusService();
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

        private void buttonRead_Click(object sender, EventArgs e)
        {
            var regs = _modbusService.ReadHoldingRegisters(
                _slaveId,
                UInt16.Parse(textBoxStartAddress.Text),
                UInt16.Parse(textBoxRegisterNum.Text));

            string txt = "";
            foreach (var reg in regs)
            {
                txt += reg.ToString() + "\n";
            }
            richTextBoxResult.Text = txt;
        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            ushort value = UInt16.Parse(textBoxValue.Text);

            _modbusService.WriteSingleRegister(
                _slaveId,
                UInt16.Parse(textBoxStartAddress.Text),
                value);

            richTextBoxResult.Text = "Done!";

        }
    }
}
