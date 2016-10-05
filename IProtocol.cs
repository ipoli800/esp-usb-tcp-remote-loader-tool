using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public enum ConnStatusE { OffLine, OnLine, Connecting };
    public delegate void ReadDataHandler(byte[] rxData);
    public delegate void ConnStatusHandler(ConnStatusE Status, string Description);
    
    public interface IProtocol
    {
        event ReadDataHandler OnReadData;
        event ConnStatusHandler OnConnChange;

        bool Connect();
        bool Disconnect();
        int Write(byte[] txData);
        
        ConnStatusE GetConnStatus();
        string GetConnectionDescription();
    }

}
