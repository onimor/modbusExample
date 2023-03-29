namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly ModbusService _modbusService;
        private bool IsRun { get; set; }
        public Form1()
        {
            InitializeComponent();
            _modbusService = new ModbusService();
            dataGridView1.Columns.Add("Value", "Value");
            IsRun = false;
            button1.Enabled = !IsRun;
            button2.Enabled = IsRun;
            button3.Enabled = IsRun;
            button4.Enabled = IsRun;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            _modbusService.StartModbusTcpSlave();
            await _modbusService.InitMaster();
            IsRun = true;
            button1.Enabled = !IsRun;
            button2.Enabled = IsRun;
            button3.Enabled = IsRun;
            button4.Enabled = IsRun;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _modbusService.StopTCPSlave();
            IsRun = false;
            button1.Enabled = !IsRun;
            button2.Enabled = IsRun;
            button3.Enabled = IsRun;
            button4.Enabled = IsRun;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await _modbusService.SendDataToRegisters(textBox1.Text);
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            var readData = await _modbusService.ReadRegisters();
            dataGridView1.Rows.Clear();
            for (int i = 0; i < readData.Length; i++)
            {
                dataGridView1.Rows.Add(new object[] { readData[i] });
            }
        }
    }
}