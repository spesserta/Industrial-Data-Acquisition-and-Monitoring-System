using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indus.Communication
{
    /// <summary>
    /// 串口通信参数配置类，封装 Modbus RTU 通信所需的串口参数。
    /// </summary>
    public class SerialInfo
    {

        public string PortName { get; set; } = "COM1";


        public int BaudRate { get; set; } = 9600;


        public int DataBit { get; set; } = 8;


        public Parity Parity { get; set; } = Parity.None;


        public StopBits StopBits { get; set; } = StopBits.One;
    }
}

