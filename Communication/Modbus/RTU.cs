using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indus.Communication.Modbus
{
    /// <summary>
    /// RTU通信协议实现类，提供串口级别的数据收发与CRC校验功能。
    /// 采用单例模式确保全局只有一个串口实例，避免端口冲突。
    /// </summary>
    public class RTU
    {

        public Action<int, List<byte>> ResponseData;

        //单例实例
        private static RTU _instance;
        //串口配置信息
        private static SerialInfo _serialInfo;
        SerialPort _serialPort;

        //自旋锁标志
        bool _isBusing = false;


        int _currentSlave;  
        int _funcCode;    
        int _wordLen;       
        int _startAddr;   


        //构造器，实例化的时候初始化串口
        private RTU(SerialInfo serialInfo)
        {
            _serialPort = new SerialPort();    
            _serialInfo = serialInfo;           
        }

        /// <summary>
        /// 获取 RTU 单例实例。线程安全，第一次调用时使用传入的serialInfo初始化。
        /// </summary>
        public static RTU GetInstance(SerialInfo serialInfo)
        {
            lock ("rtu")                        //锁住rtu以保证线程安全
            {
                if (_instance == null)          
                    _instance = new RTU(serialInfo);
                return _instance;
            }
        }


        /// <summary>
        /// 打开串口连接，并根据配置信息设置串口参数。
        /// </summary>
        public bool Connection()   //打开串口
        {
            try
            {
                if (_serialPort.IsOpen)       // 如果串口已打开则先关闭
                    _serialPort.Close();

                //从配置对象中读取串口参数并赋值
                _serialPort.PortName = _serialInfo.PortName;          
                _serialPort.BaudRate = _serialInfo.BaudRate;           
                _serialPort.DataBits = _serialInfo.DataBit;            
                _serialPort.Parity = _serialInfo.Parity;                
                _serialPort.StopBits = _serialInfo.StopBits;           

                _serialPort.ReceivedBytesThreshold = 1;                 // 接收到1字节即触发DataReceived事件
                _serialPort.DataReceived += _serialPort_DataReceived;   // 注册数据接收事件处理

                _serialPort.Open();      // 打开串口
            }
            catch
            {
                return false;                
            }

            return true;                    
        }


        public void Dispose()  // 关闭串口
        {
            if (_serialPort.IsOpen)           
            {
                _serialPort.Close();          
                _serialPort.Dispose();         
                _serialPort = null;           
            }
        }



        /// <summary>
        /// RTU 接收数据帧：从串口缓冲区逐字节读取，接收完整后校验从站地址和功能码。
        /// </summary>
        int _receiveByteCount = 0;              //已接收字节计数
        byte[] _byteBuffer = new byte[512];     //接收缓冲区最大512字节
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e) //接收数据帧
        {
            //从那个串口中读取数据，一个字节一个字节的读取
            byte _receiveBytes;
            while (_serialPort.BytesToRead > 0)  //一直到读完为止 
            {
                _receiveBytes = (byte)_serialPort.ReadByte();    //读一个字节
                _byteBuffer[_receiveByteCount] = _receiveBytes;     //存入缓冲区
                _receiveByteCount++;                         
                if (_receiveByteCount >= 512)               //缓冲区满了就提交+清空
                {
                    _receiveByteCount = 0;                    
                    _serialPort.DiscardInBuffer();            
                    return;                                
                }
            }
            //校验数据帧：从站地址与功能码匹配，且长度满足最少需求
            if (_byteBuffer[0] == (byte)_currentSlave && _byteBuffer[1] == _funcCode && _receiveByteCount >= _wordLen + 5)
            {
                
                ResponseData?.Invoke(_startAddr, new List<byte>(SubByteArray(_byteBuffer, 0, _wordLen + 3)));
                _serialPort.DiscardInBuffer(); 
            }
        }

        /// <summary>
        /// 从字节数组中截取指定起始位置和长度的子数组。
        /// </summary>

        private byte[] SubByteArray(byte[] byteArr, int start, int len) 
        {
            byte[] Res = new byte[len];     // 创建目标数组
            if (byteArr != null && byteArr.Length > len)  // 确保源数组不为空且足够长
            {
                for (int i = 0; i < len; i++)
                {
                    Res[i] = byteArr[i + start];        // 逐个复制字节
                }
            }
            return Res;
        }



        /// <summary>
        /// 发送RTU请求数据帧
        /// </summary>

        public async Task<bool> Send(int slaveAddr, byte funcCode, int startAddr, int len)  // 发送数据帧
        {
            _currentSlave = slaveAddr;        
            _funcCode = funcCode;              
            _startAddr = startAddr;           

            if (funcCode == 0x01)      //读线圈状态
                _wordLen = len / 8 + ((len % 8 > 0) ? 1 : 0);   // 线圈按位计算字节数
            if (funcCode == 0x03)         //读保持寄存器
                _wordLen = len * 2;                          //每个寄存器占2字节


            // 拼接 8 字节的请求数据帧：[从站地址] [功能码] [起始地址高位] [起始地址低位] [长度高位] [长度低位] [CRC低位] [CRC高位]
            List<byte> sendBuffer = new List<byte>();
            sendBuffer.Add((byte)slaveAddr);                    //从站地址
            sendBuffer.Add(funcCode);                           //功能码
            sendBuffer.Add((byte)(startAddr / 256));           //起始地址高字节
            sendBuffer.Add((byte)(startAddr % 256));            //起始地址低字节
            sendBuffer.Add((byte)(len / 256));                   //长度高字节
            sendBuffer.Add((byte)(len % 256));                  //长度低字节
            byte[] crc = Crc16(sendBuffer.ToArray(), 6);        //对前 6 字节计算 CRC 校验码
            sendBuffer.AddRange(crc);                           //追加 CRC 码到帧尾

            
            try
            {
                while (_isBusing) { }        

                _isBusing = true;               //串口
                _serialPort.Write(sendBuffer.ToArray(), 0, 8);  // 写入8字节请求帧
                _isBusing = false;            

                await Task.Delay(1000);         // 等待1秒，给下位机足够时间返回响应帧
            }
            catch
            {
                return false;                  
            }
            _receiveByteCount = 0;          // 重置接收字节计数，准备接收响应
            return true;                    
        }

        
        /// <summary>
        /// CRC-16 循环冗余校验，使用 Modbus 标准多项式（0x8005）。
        /// </summary>
        #region  CRC校验
        // CRC 高字节查找表
        private static readonly byte[] aucCRCHi = {
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40
         };
        // CRC 低字节查找表
        private static readonly byte[] aucCRCLo = {
             0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7,
             0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E,
             0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9,
             0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,
             0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
             0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,
             0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D,
             0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,
             0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF,
             0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
             0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1,
             0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,
             0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB,
             0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA,
             0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
             0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,
             0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97,
             0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E,
             0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89,
             0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
             0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83,
             0x41, 0x81, 0x80, 0x40
         };

        /// <summary>
        /// 使用查表法计算 CRC-16 校验码，遵循 Modbus 协议标准。
        /// </summary>
        /// <param name="pucFrame">待校验的数据帧</param>
        /// <param name="usLen">数据帧长度</param>
        /// <returns>2 字节 CRC 码（低字节在前，高字节在后）</returns>
        private byte[] Crc16(byte[] pucFrame, int usLen)
        {
            int i = 0;                          // 数据帧索引
            byte crcHi = 0xFF;                  // CRC 高字节初始值
            byte crcLo = 0xFF;                  // CRC 低字节初始值
            UInt16 iIndex = 0x0000;             // 查表索引

            while (usLen-- > 0)                 // 逐字节处理
            {
                iIndex = (UInt16)(crcLo ^ pucFrame[i++]);   // 低字节异或当前数据字节，得到查表索引
                crcLo = (byte)(crcHi ^ aucCRCHi[iIndex]);   // 新低字节 = 原高字节异或高字节表值
                crcHi = aucCRCLo[iIndex];                   // 新高字节 = 低字节表值
            }

            return new byte[] { crcLo, crcHi }; // 返回 2 字节 CRC 码
        }


        #endregion
    }
}

