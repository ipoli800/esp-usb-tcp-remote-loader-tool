using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;

namespace WindowsFormsApplication1
{
    public class Ethernet : IProtocol
    {
        TcpListener tcpListener;
        Thread listenThread;
        Thread CliListenThread;
        volatile bool IsClientConnected = false;
        volatile bool IsListening = false;
        volatile bool DoDisconnect = false;
        NetworkStream clientStream;
        const int MaxRxLen = 5 * 1024;
        ConnStatusE LastStatus = ConnStatusE.OffLine;
        TcpClient tcpclnt = new TcpClient();
        Stream stm;

        public int ServerPort = 5000;
  //      public event CliReadDataHandler CliOnReadData = delegate { };
        public event ReadDataHandler OnReadData = delegate { };
        public event ReadDataHandler OnCliReadData = delegate { };
        public event ConnStatusHandler OnConnChange = delegate { };
        public event ConnStatusHandler OnCliConnChange = delegate { };

        public bool Connect()
        {
            if (IsListening)
            {
                return true;
            }

            tcpListener = new TcpListener(IPAddress.Any, ServerPort);

            try
            {
                tcpListener.Start();
            }

            catch (Exception)
            {
                MessageBox.Show("Port " + ServerPort + " is already used by an another application!\nPlease select an another port and then connect again.", "Server port error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            DoDisconnect = false;
            IsListening = true;

            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
            Thread.Sleep(10);

            LastStatus = ConnStatusE.Connecting;
            OnConnChange(LastStatus, "Server Listening...");

            return true;
        }
    
    public bool Disconnect()
    {
        if (false == IsListening)
        {
            return true;
        }

        LastStatus = ConnStatusE.OffLine;
        OnConnChange(LastStatus, "Disconnected");
        DoDisconnect = true;
        Thread.Sleep(30);


        return true;
    }

    public string GetConnectionDescription()
    {
        string Desc;

        Desc = "Server port:" + ServerPort.ToString();

        return Desc;
    }

    public ConnStatusE GetConnStatus()
    {
        return LastStatus;
    }

    public int Write(byte[] txData)
    {
        if (IsClientConnected)
        {

            try
            {
                clientStream.Write(txData, 0, txData.Length);
            }

            catch (Exception)
            {
                IsClientConnected = false;
                return 0;
            }

            return txData.Length;
        }

        return 0;
    }

    private void ListenForClients()
    {
        byte[] message = new byte[MaxRxLen];
        string ClientStr;

        while (false == DoDisconnect)
        {
            if (false == tcpListener.Pending())
            {
                Thread.Sleep(20);
                continue;
            }

            //blocks until a client has connected to the server
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            IPEndPoint ipend = (IPEndPoint)tcpClient.Client.RemoteEndPoint;

            ClientStr = ipend.Address.ToString();
            ClientStr += ":" + ipend.Port.ToString();

            clientStream = tcpClient.GetStream();
            clientStream.ReadTimeout = 50;
            clientStream.WriteTimeout = 50;

            IsClientConnected = true;
            LastStatus = ConnStatusE.OnLine;
            OnConnChange(LastStatus, "Connected to " + ClientStr);

            try
            {
                clientStream.BeginRead(message, 0, MaxRxLen, new AsyncCallback(processInput), message);
            }

            catch
            {

            }

            while (false == DoDisconnect && true == IsClientConnected)
            {
                Thread.Sleep(20);
            }

            IsClientConnected = false;
            tcpClient.Close();

            if (false == DoDisconnect)
            {
                LastStatus = ConnStatusE.Connecting;
                OnConnChange(LastStatus, "Listening...");
            }
        }

        tcpListener.Stop();
        IsListening = false;
    }

    protected void processInput(IAsyncResult r)
    {
        int bytesRead;
        try
        {
            bytesRead = clientStream.EndRead(r);
        }
        catch
        {
            // notify someone of exception
            // notify someone that you will stop reading
            IsClientConnected = false;
            return;
        }
        if (bytesRead == 0)
        { // Closed
          // notify someone that you will stop reading
            IsClientConnected = false;
            return;
        }

        byte[] rxBuf = (byte[])r.AsyncState;
        byte[] rxData = new byte[bytesRead];

        Buffer.BlockCopy(rxBuf, 0, rxData, 0, bytesRead);

        OnReadData(rxData);

        byte[] message = new byte[MaxRxLen];

        try
        {
            clientStream.BeginRead(message, 0, MaxRxLen, new AsyncCallback(processInput), message);
        }

        catch
        {

        }
    }

    public void ClientConnect(string address, int port)
    {
        try
        {
            tcpclnt.Connect(address, port);
            stm = tcpclnt.GetStream();
                OnCliConnChange(LastStatus, "Client Connected...");
            }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
            CliListenThread = new Thread(new ThreadStart(ClientReaddStr));
            CliListenThread.Start();
            Thread.Sleep(10);

            //LastStatus = ConnStatusE.Connecting;
            OnCliConnChange(LastStatus, "Client Listening...");
        }
    public void ClientSendStr(string text)
    {
        try
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(text);
            Console.WriteLine("Transmitting.....");
                OnCliConnChange(LastStatus, "Client transmitting...");
                stm.Write(ba, 0, ba.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }
    private void ClientReaddStr()
        {
            while (true)
            {
                try
                {
                    string ttt = "";
                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);
                    for (int i = 0; i < k; i++)
                        ttt += Convert.ToChar(bb[i]);
                    OnCliReadData(bb);
                    Thread.Sleep(20);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error..... " + e.StackTrace);
                }
            }
    }
    public void ClientClose()
    {
        try
        {
            tcpclnt.Close();
            OnCliConnChange(LastStatus, "Client disconnected...");
            }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }
}
}
