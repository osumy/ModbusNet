using ModbusNet;
using ModbusNet.Enum;
using ModbusUI.Enums;
using ModbusUI.Utils;
using System.IO.Ports;

namespace Modbus.Forms
{
    public partial class SettingsForm : Form
    {
        public ModbusSettings Settings { get; private set; }

        private string[] baudRates = { "9600 Baud Rate", "19200 Baud Rate", "38400 Baud Rate" };

        public SettingsForm(ModbusSettings currentSettings)
        {
            InitializeComponent();

            CancelButton = cancelBtn;
            AcceptButton = okBtn;

            Settings = currentSettings;

            fillForm();
            enableGroups();
        }

        /// <summary>
        /// Converts a baud rate string like "9600 Baud Rate" to an integer (e.g., 9600).
        /// </summary>
        private int BaudRateStringToInt(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            // Extract only digits from the string
            string digits = new string(text.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out int baud) ? baud : 0;
        }

        /// <summary>
        /// Converts an integer baud rate (e.g., 9600) to a display string like "9600 Baud Rate".
        /// </summary>
        private string BaudRateIntToString(int baud)
        {
            return $"{baud} Baud Rate";
        }

        private void fillForm()
        {
            List<string> cnn = new List<string> { "Serial Port", "TCP/IP" };
            comboConnectionType.Items.AddRange(cnn.ToArray());
            comboConnectionType.SelectedItem = (Settings.ConnectionType.ToString() == "ASCII") ||
                                                (Settings.ConnectionType.ToString() == "RTU") ? "Serial Port" : "TCP/IP";


            comboPortName.DataSource = EnumExtensions.ToList<PortName>();
            comboPortName.SelectedValue = Settings.PortName;

            comboBaudRate.Items.Clear();
            comboBaudRate.Items.AddRange(baudRates);
            string desiredText = BaudRateIntToString(Settings.BaudRate);
            if (comboBaudRate.Items.Contains(desiredText))
            {
                checkBoxCustomBaud.Checked = false;
                comboBaudRate.Enabled = true;
                textBoxCustomBaud.Enabled = false;
                comboBaudRate.SelectedItem = desiredText;
            }
            else
            {
                checkBoxCustomBaud.Checked = true;
                comboBaudRate.Enabled = false;
                textBoxCustomBaud.Enabled = true;
                textBoxCustomBaud.Text = Settings.BaudRate > 0 ? Settings.BaudRate.ToString() : "";
            }

            comboParity.DataSource = Enum.GetValues(typeof(ParityType))
                .Cast<Parity>()
                .Select(e => new { Value = e, Text = e.ToString() + " Parity" })
                .ToList();

            comboParity.SelectedValue = Settings.Parity;


            comboDataBits.DataSource = EnumExtensions.ToList<DataBits>();
            comboDataBits.SelectedValue = Settings.DataBits;


            comboStopBits.DataSource = Enum.GetValues(typeof(StopBitsType))
                .Cast<StopBits>()
                .Select(e => new { Value = e, Text = e.ToString() + " Stop Bit" })
                .ToList(); comboStopBits.DisplayMember = "Text";

            comboStopBits.SelectedValue = Settings.StopBits;

            respTBox.Text = Settings.Timeout.ToString();

            delayTBox.Text = Settings.RetryDelayMs.ToString();

            radioButtonRTU.Checked = Settings.ConnectionType.ToString() == "RTU";

            radioButtonASCII.Checked = Settings.ConnectionType.ToString() == "ASCII";
        }

        private void enableGroups()
        {
            bool isSerial = comboConnectionType.SelectedItem.ToString() == "Serial Port";
            groupBox6.Enabled = !isSerial;
            groupBox2.Enabled = isSerial;
            groupBox3.Enabled = isSerial;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            Settings.ConnectionType = comboConnectionType.SelectedItem.ToString() switch
            {
                "Serial Port" when radioButtonASCII.Checked => ConnectionType.ASCII,
                "Serial Port" when radioButtonRTU.Checked => ConnectionType.RTU,
                "TCP/IP" => ConnectionType.TCP,
                _ => Settings.ConnectionType
            };
            Settings.PortName = (PortName)comboPortName.SelectedValue;
            Settings.Parity = (Parity)comboParity.SelectedValue;
            Settings.DataBits = (DataBits)comboDataBits.SelectedValue;
            Settings.StopBits = (StopBits)comboStopBits.SelectedValue;
            Settings.Timeout = Int32.Parse(respTBox.Text);
            Settings.RetryDelayMs = Int32.Parse(delayTBox.Text);

            int baudValue = 0;
            if (checkBoxCustomBaud.Checked)
            {
                string txt = textBoxCustomBaud.Text?.Trim();
                if (!int.TryParse(txt, out baudValue) || baudValue <= 0)
                {
                    MessageBox.Show("Please enter a valid numeric baud rate.", "Invalid Baud Rate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                if (comboBaudRate.SelectedItem != null)
                    baudValue = BaudRateStringToInt(comboBaudRate.SelectedItem.ToString());
                else
                {
                    if (comboBaudRate.Items.Count > 0)
                        baudValue = BaudRateStringToInt(comboBaudRate.Items[0].ToString());
                }
            }
            Settings.BaudRate = baudValue;


            DialogResult = DialogResult.OK;
            Close();
        }

        private void checkBoxCustomBaud_CheckedChanged(object sender, EventArgs e)
        {
            bool custom = checkBoxCustomBaud.Checked;
            comboBaudRate.Enabled = !custom;
            textBoxCustomBaud.Enabled = custom;

            if (custom)
            {
                if (comboBaudRate.SelectedItem != null)
                    textBoxCustomBaud.Text = BaudRateStringToInt(comboBaudRate.SelectedItem.ToString()).ToString();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(textBoxCustomBaud.Text))
                {
                    if (int.TryParse(textBoxCustomBaud.Text, out int v))
                    {
                        string textForm = BaudRateIntToString(v);
                        if (comboBaudRate.Items.Contains(textForm))
                            comboBaudRate.SelectedItem = textForm;
                    }
                }
            }
        }

        private void comboConnectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            enableGroups();
        }
    }
}
