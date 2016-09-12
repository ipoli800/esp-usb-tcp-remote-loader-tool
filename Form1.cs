using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1

{
    
    public partial class Form1 : Form
    {
        string prejem = " ";
        string sendtoesp = "";

        Color barva = Color.Black;
        Font pisava = new Font("Courier New", 10);
        //string prejemstar = " ";
        TcpClient tcpclnt = new TcpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            else
            {
                try
                {
                    serialPort1.BaudRate = 115200;
                    serialPort1.PortName = comboBox1.Text;
                    if (chkbkbaudchange.Checked)
                    {
                        serialPort1.BaudRate = 74880;
                    }
                    serialPort1.Open();
                    if (chkbkreset.Checked)
                    {
                        serialPort1.RtsEnable = true;
                        Thread.Sleep(100);
                        serialPort1.RtsEnable = false;
                        Thread.Sleep(300);
                       
                    }
                    serialPort1.BaudRate = 115200;
                    
                }
                catch (Exception) { }
            }
            if (serialPort1.IsOpen)
            {
                button1.Text = "Close";
            }

            else
                button1.Text = "Open";
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string RxString = serialPort1.ReadExisting();

            //UpdateGui(RxString);
                 prejem += RxString ;

            barva = Color.Blue;

            //richTextBox1.AppendText(serialPort1.ReadExisting(), Color.Blue);
        }


        private void UpdateGui(string data)
        {

            richTextBox1.Text += data;
            richTextBox1.ScrollToCaret();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (prejem != "")
            {
                richTextBox1.AppendText(prejem, barva, pisava);
                richTextBox1.ScrollToCaret();
                prejem = "";
            }
            if (sendtoesp != "")
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Write(sendtoesp);
                }
                if (tcpclnt.Connected)
                {
                    Stream stm = tcpclnt.GetStream();
                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(sendtoesp);
                    stm.Write(ba, 0, ba.Length);
                }
                sendtoesp = "";
            }
            if (tcpclnt.Connected)
                toolStripStatusLabel1.Text = "TCP Connected";
            if (serialPort1.IsOpen)
                toolStripStatusLabel1.Text = serialPort1.PortName + " Connected";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sendtoesp += "for k,v in pairs(file.list()) do print(k..\"(\"..v..\" bytes)\") end\r\n";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tcpclnt.Connected)
            {
                tcpclnt.Close();
            }
            else
            {
                try
                {
                    String str;
                    barva = Color.Red;
                    prejem += System.Environment.NewLine + "Opening remote address: " + richTextBox2.Text + " port: " + richTextBox3.Text;
                    tcpclnt.Connect(richTextBox2.Text, 80); // use the ipaddress as in the server program

                    prejem += System.Environment.NewLine + "Connected.";
                    Stream stm = tcpclnt.GetStream();
                    ASCIIEncoding asen = new ASCIIEncoding();
                    prejem += System.Environment.NewLine + "Trying to establish telnet communication... ";
                    byte[] ba = asen.GetBytes("telnet");
                    stm.Write(ba, 0, ba.Length);

                    prejem += System.Environment.NewLine + "Established. ";
                    barva = Color.Blue;
                    //            do
                    //             {
                    //                str = Console.ReadLine();
                    //                Stream stm = tcpclnt.GetStream();
                    //                ASCIIEncoding asen = new ASCIIEncoding();
                    //                byte[] ba = asen.GetBytes(str);
                    //                Console.WriteLine("Transmitting.....");
                    //                stm.Write(ba, 0, ba.Length);

                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);
                    prejem += System.Environment.NewLine;
                    for (int i = 0; i < k; i++)
                    {
                        prejem += Convert.ToChar(bb[i]);
                    }

                    //            while (str != "exit");

                }
                catch (Exception ee)
                {
                    prejem += System.Environment.NewLine + "Error..... " + ee.StackTrace;
                }
            }

            if (tcpclnt.Connected)
            {
                button3.Text = "Close";
            }

            else
                button3.Text = "Open";

        }

        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            string str1 = e.KeyCode.ToString();
            if (str1 == "Return")
                str1 = System.Environment.NewLine;

            sendtoesp += str1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            sendtoesp = richTextBox4.Text;
            sendtoesp += System.Environment.NewLine;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DppTCPClient esptcp = new DppTCPClient();
            esptcp.Connect("10.22.0.80",80);
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes("telnet");
            esptcp.WriteBytes(ba,ba.Length);
        }
    }
    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        public static void AppendText(this RichTextBox box, string text, Color color, Font font)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.SelectionFont = font;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;

        }
    }
    public class ESPComm
    {
        public IPAddress IPAddressa { get; set; } = IPAddress.Parse("10.22.0.80");
        public Int32 TCPPort { get; set; } = 80;
        private TcpClient esptcp = new TcpClient();

        public void TCPOpen(IPAddress addr, Int32 prt)
        {
            esptcp.Connect(addr, prt);
            Stream stm = esptcp.GetStream();
        }
        public void TCPSendString(string str1)
        {
            Stream stm = esptcp.GetStream();
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(str1);
            stm.Write(ba, 0, ba.Length);
        }
        public string TCPReadString()
        {
            Stream stm = esptcp.GetStream();
            string pr="";
            byte[] bb = new byte[100];
            int k = stm.Read(bb, 0, 100);
            for (int i = 0; i < k; i++)
            {
                pr += Convert.ToChar(bb[i]);
            }
            return pr;
        }
    }
    public class DppTCPClient
    {
        public TcpClient m_client = new TcpClient();

        public void Connect(string address, int port)
        {
            if (m_client.Connected)
                throw new Exception("Connect: Already connected");

            m_client.Connect(address, port);
        }

        public void WriteBytes(byte[] data, int length)
        {
            if (data.Length != length)
                throw new Exception("WriteBytes: Length should be same");

            // Get access to network stream
            Stream stm = m_client.GetStream();
            stm.Write(data, 0, data.Length);

        }

        public void ReadAllBytes(byte[] buffer, int length)
        {
            if (buffer.Length != length)
                throw new Exception("ReadAllBytes: Length should be same");

            Stream stm = m_client.GetStream();

            // Start reading
            int offset = 0;
            int remaining = length;
            while (remaining > 0)
            {
                int read = stm.Read(buffer, offset, remaining);
                if (read <= 0)
                    throw new EndOfStreamException
                        (String.Format("ReadAllBytes: End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;
            }

        }

        public void CloseDppClient()
        {
            if (m_client.Connected)
            {
                m_client.GetStream().Close();
                m_client.Close();
            }
        }
    }
}
