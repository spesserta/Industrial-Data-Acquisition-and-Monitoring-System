using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indus.Communication;
using Indus.Communication.Modbus;
using Indus.Industrial.BLL;
using Indus.Industrial.Model;

namespace Indus.Industrial.Base
{
    /// <summary>
    /// 全局监控器，负责管理串口通信、数据采集、设备状态轮询等核心监控逻辑
    /// </summary>
    public class GlobalMonitor
    {

        public static List<StorageModel> StorageList { get; set; }  //存储区配置列表，描述各从站的寄存器区域信息


        public static List<DeviceModel> DeviceList { get; set; }  //设备列表，包含所有监控设备及其对应的监控变量


        public static SerialInfo SerialInfo { get; set; }  //串口通信配置信息


        static bool isRunning = true;   //自旋锁标志位


        static Task mainTask = null;   //主监控任务的引用，用于等待任务结束


        static RTU rtuInstance = null;   //Modbus RTU 通信单例实例


        public static void Start(Action sucessAction, Action<string> faultAction)  //启动全局监控服务。初始化配置、建立串口连接并开始轮询各存储区的数据。
        {
            //在后台线程中执行初始化及轮询操作
            mainTask = Task.Run(async () =>
             {
                 IndustrialBLL bll = new IndustrialBLL();

                 //获取串口配置信息
                 var si = bll.InitSerialInfo();
                 if (si.State)
                     SerialInfo = si.Data;
                 else
                 {
                     faultAction(si.Message); return;
                 }

                 //获取存储区信息
                 var sa = bll.InitStorageArea();
                 if (sa.State)
                     StorageList = sa.Data;
                 else
                 {
                     faultAction(sa.Message); return;
                 }

                 //获取设备变量集合及警戒值
                 var dr = bll.InitDevices();
                 if (dr.State)
                     DeviceList = dr.Data;
                 else
                 {
                     faultAction(dr.Message); return;
                 }

                 //单例模式初始化串口通信实例
                 rtuInstance = RTU.GetInstance(SerialInfo);
                 //注册数据接收回调，用于解析从站返回的报文数据
                 rtuInstance.ResponseData = new Action<int, List<byte>>(ParsingData);

                 //尝试建立串口连接
                 if (rtuInstance.Connection())
                 {
                     //通知外部初始化成功
                     sucessAction();

                     int startAddr = 0;
                     //持续轮询各存储区的数据
                     while (isRunning)
                     {
                         foreach (var item in StorageList)
                         {
                             //若存储区长度超过 100，则分多次读取（Modbus 协议单次读取上限）
                             if (item.Length > 100)
                             {
                                 startAddr = item.StartAddress;
                                 int readCount = item.Length / 100;
                                 for (int i = 0; i < readCount; i++)
                                 {
                                     //每次读取 100 个寄存器
                                     int readLen = i == readCount ? item.Length - 100 * i : 100;
                                     await rtuInstance.Send(item.SlaveAddress, (byte)int.Parse(item.FuncCode), startAddr + 100 * i, readLen);
                                 }
                             }
                             //处理剩余不足 100 的部分
                             if (item.Length % 100 > 0)
                             {
                                 await rtuInstance.Send(item.SlaveAddress, (byte)int.Parse(item.FuncCode), startAddr + 100 * (item.Length / 100), item.Length % 100);
                             }
                         }
                     }
                 }
                 else
                 {
                     //串口连接失败，通过回调通知调用方
                     faultAction("程序无法启动，串口连接初始化失败！请检查设备是否连接正常。");
                 }
             });
        }

        /// <summary>
        /// 解析从 Modbus 从站返回的报文数据，更新对应监控变量的当前值
        /// </summary>

        private static void ParsingData(int start_addr, List<byte> byteList)
        {
            if (byteList != null && byteList.Count > 0)
            {
                // 查找设备监控点位中与当前返回报文相关的监控数据列表
                // 匹配条件：从站地址 + 功能码 + 起始地址 组合成的存储区标识
                var mvl = (from q in DeviceList
                           from m in q.MonitorValueList
                           where m.StorageAreaId == (byteList[0].ToString() + byteList[1].ToString("00") + start_addr.ToString())
                           select m
                         ).ToList();

                int startByte;
                byte[] res = null;
                foreach (var item in mvl)
                {
                    // 根据数据类型采用不同的解析方式
                    switch (item.DataType)
                    {
                        case "Float":
                            // 计算当前变量在返回字节数组中的起始位置（地址 * 2 + 3 偏移）
                            startByte = item.StartAddress * 2 + 3;
                            if (startByte < start_addr + byteList.Count)
                            {
                                // 取出 4 个字节并转换为浮点数
                                res = new byte[4] { byteList[startByte], byteList[startByte + 1], byteList[startByte + 2], byteList[startByte + 3] };
                                // 通过扩展方法将字节数组转换为浮点数值
                                item.CurrentValue = Convert.ToDouble(res.ByteArrsyToFloat());
                            }
                            break;
                        case "Bool":
                            // Bool 类型暂未实装
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 释放全局监控资源。停止轮询循环、关闭串口通信并等待任务结束。
        /// </summary>
        public static void Dispose()
        {
            // 停止轮询循环
            isRunning = false;

            // 释放串口通信实例
            if (rtuInstance != null)
                rtuInstance.Dispose();

            // 等待后台监控任务完全退出
            if (mainTask != null)
                mainTask.Wait();
        }
    }
}

