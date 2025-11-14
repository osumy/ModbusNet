using Modbus.Models;
using Modbus.Models.Enums;
using Modbus.Utils;
using System.IO.Ports;

namespace Modbus.Forms
{
    public partial class SettingsForm : Form
    {
        public ConnectionSettings Settings { get; private set; }

        private string[] baudRates = { "9600 Baud Rate", "19200 Baud Rate", "38400 Baud Rate" };

        public SettingsForm(ConnectionSettings currentSettings)
        {
            InitializeComponent();

            CancelButton = cancelBtn;
            AcceptButton = okBtn;

            Settings = currentSettings;

            fillForm();
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
            comboConnectionType.DataSource = EnumExtensions.ToList<ConnectionType>();
            comboConnectionType.SelectedValue = Settings.connectionType;


            comboPortName.DataSource = EnumExtensions.ToList<PortName>();
            comboPortName.SelectedValue = Settings.portName;

            comboBaudRate.Items.Clear();
            comboBaudRate.Items.AddRange(baudRates);
            string desiredText = BaudRateIntToString(Settings.baudRate);
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
                textBoxCustomBaud.Text = Settings.baudRate > 0 ? Settings.baudRate.ToString() : "";
            }

            comboParity.DataSource = Enum.GetValues(typeof(ParityType))
                .Cast<Parity>()
                .Select(e => new { Value = e, Text = e.ToString() + " Parity" })
                .ToList();

            comboParity.SelectedValue = Settings.parity;


            comboDataBits.DataSource = EnumExtensions.ToList<DataBits>();
            comboDataBits.SelectedValue = Settings.dataBits;


            comboStopBits.DataSource = Enum.GetValues(typeof(StopBitsType))
                .Cast<StopBits>()
                .Select(e => new { Value = e, Text = e.ToString() + " Stop Bit" })
                .ToList(); comboStopBits.DisplayMember = "Text";

            comboStopBits.SelectedValue = Settings.stopBits;

            respTBox.Text = Settings.responseTimeout.ToString();

            delayTBox.Text = Settings.delay.ToString();

            radioButtonRTU.Checked = Settings.isRTU;

            radioButtonASCII.Checked = !Settings.isRTU;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            Settings.connectionType = (ConnectionType)comboConnectionType.SelectedValue;
            Settings.portName = (PortName)comboPortName.SelectedValue;
            Settings.parity = (Parity)comboParity.SelectedValue;
            Settings.dataBits = (DataBits)comboDataBits.SelectedValue;
            Settings.stopBits = (StopBits)comboStopBits.SelectedValue;
            Settings.responseTimeout = Int32.Parse(respTBox.Text);
            Settings.delay = Int32.Parse(delayTBox.Text);
            Settings.isRTU = radioButtonRTU.Checked;

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
            Settings.baudRate = baudValue;


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
    }
}
