using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Modbus.Data;
using Modbus.Device;
using Modbus.Utility;

namespace NModbus_example
{
    public partial class Form2 : Form
    {
        public TcpClient masterTcpClient;
        public ModbusIpMaster master;

        private Thread thr_tmrobot_position;
        private Thread thr_tmrobot_io;
        private Thread thr_tmrobot_registers;

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ip_address = textBox1.Text;
            int port = Convert.ToInt16(textBox2.Text);

            try
            {
                // create the master
                masterTcpClient = new TcpClient(ip_address, port);
                master = ModbusIpMaster.CreateIp(masterTcpClient);
                master.Transport.ReadTimeout = 1500;
                button2.Enabled = true;

                thr_tmrobot_position = new Thread(LoopExecute_RobotPose);
                thr_tmrobot_io = new Thread(LoopExecute_RobotIO);
                thr_tmrobot_registers = new Thread(LoopExecute_RobotRegisters);
                thr_tmrobot_position.Start();
                thr_tmrobot_io.Start();
                thr_tmrobot_registers.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connect error, Please check IP Address.");
            }
        }

        public void LoopExecute_RobotPose()
        {
            while (true)
            {
                Thread.Sleep(100);
                this.Invoke((Action)Robot_Position);
            }
        }

        public void LoopExecute_RobotIO()
        {
            while (true)
            {
                Thread.Sleep(100);
                this.Invoke((Action)Robot_IO);
            }
        }

        public void LoopExecute_RobotRegisters()
        {
            while (true)
            {
                Thread.Sleep(100);
                this.Invoke((Action)Robot_Registers);
            }
        }

        public void Robot_Position()
        {
            ushort numInputs = 48;
            ushort startAddress = 7001;

            // read five register values
            ushort[] inputs = master.ReadInputRegisters(startAddress, numInputs);
            float[] result = ModbusUtility.ConvertUshortArrayToFloatArray(inputs);

            //Techman Robot world position
            label11.Text = result[0].ToString("F3");
            label12.Text = result[1].ToString("F3");
            label13.Text = result[2].ToString("F3");
            label14.Text = result[3].ToString("F3");
            label15.Text = result[4].ToString("F3");
            label16.Text = result[5].ToString("F3");
            //Techman Robot Joint position
            label22.Text = result[6].ToString("F3");
            label21.Text = result[7].ToString("F3");
            label20.Text = result[8].ToString("F3");
            label19.Text = result[9].ToString("F3");
            label18.Text = result[10].ToString("F3");
            label17.Text = result[11].ToString("F3");
        }

        public void Robot_IO()
        {
            // Write IO
            master.WriteSingleCoil(801, checkBox1.Checked);
            master.WriteSingleCoil(1, checkBox5.Checked);

            // Read IO
            ushort numInputs_I = 1;
            ushort numInputs_O = 1;

            ushort startAddress_I = 801;
            ushort startAddress_O = 1;

            bool[] input_I = master.ReadCoils(startAddress_I, numInputs_I);
            bool[] input_O = master.ReadCoils(startAddress_O, numInputs_O);

            label35.Text = input_I[0].ToString();
            label36.Text = input_O[0].ToString();
        }

        public void Robot_Registers()
        {
            ushort numInputs = 1;
            ushort startAddress = 9000;

            // read five register values
            ushort[] inputs = master.ReadHoldingRegisters(startAddress, numInputs);
            label32.Text = ((Int16)inputs[0]).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // clean up
            masterTcpClient.Close();
            thr_tmrobot_position.Abort();
            thr_tmrobot_io.Abort();
            thr_tmrobot_registers.Abort();
            button2.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
