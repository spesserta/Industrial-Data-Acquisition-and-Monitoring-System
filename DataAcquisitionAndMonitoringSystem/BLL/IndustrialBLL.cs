using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indus.Communication;
using Indus.Industrial.DAL;
using Indus.Industrial.Model;
using System.Data;
using System.Windows;
using Indus.Industrial.Base;

namespace Indus.Industrial.BLL
{
    /// <summary>
    /// 工业数据采集与监控系统的业务逻辑层，负责处理串口通信、设备初始化、报警阈值管理等核心业务
    /// </summary>
    public class IndustrialBLL
    {
        //数据访问层实例，用于与数据库交互
        DataAccess da = new DataAccess();

        /// <summary>
        /// 从配置文件读取串口参数并初始化串口信息
        /// </summary>
        /// <returns>包含串口信息（端口号、波特率、数据位、校验位、停止位）的结果对象</returns>
        public DataResult<SerialInfo> InitSerialInfo()
        {
            DataResult<SerialInfo> result = new DataResult<SerialInfo>();

            try
            {
                SerialInfo si = new SerialInfo();
                // 从 App.config 中读取串口配置参数
                si.PortName = ConfigurationManager.AppSettings["port"].ToString();
                si.BaudRate = int.Parse(ConfigurationManager.AppSettings["baud"].ToString());
                si.DataBit = int.Parse(ConfigurationManager.AppSettings["data_bit"].ToString());
                // 将字符串形式的校验位枚举转换为 Parity 枚举值
                si.Parity = (Parity)Enum.Parse(typeof(Parity), ConfigurationManager.AppSettings["parity"].ToString(), true);
                // 将字符串形式的停止位枚举转换为 StopBits 枚举值
                si.StopBits = (StopBits)Enum.Parse(typeof(StopBits), ConfigurationManager.AppSettings["stop_bit"].ToString(), true);

                result.State = true;
                result.Data = si;
            }
            catch (Exception ex)
            {
                // 捕获异常并将错误信息写入结果对象
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 从数据库初始化所有存储区域信息
        /// </summary>
        public DataResult<List<StorageModel>> InitStorageArea()
        {
            DataResult<List<StorageModel>> result = new DataResult<List<StorageModel>>();

            try
            {
                // 调用数据访问层获取存储区域原始数据表
                var sa = da.GetStorageArea();

                result.State = true;
                // 将 DataTable 转换为 List<StorageModel>，并映射字段
                result.Data = (from q in sa.AsEnumerable()
                               select new StorageModel
                               {
                                   id = q.Field<string>("id"),
                                   SlaveAddress = q.Field<Int32>("slave_add"),
                                   FuncCode = q.Field<string>("func_code"),
                                   StartAddress = int.Parse(q.Field<string>("start_reg")),
                                   Length = int.Parse(q.Field<string>("length"))
                               }).ToList();
            }
            catch (Exception ex)
            {
                // 捕获异常并将错误信息写入结果对象
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 从数据库初始化所有设备及其关联的监控点位信息
        /// </summary>
        public DataResult<List<DeviceModel>> InitDevices()
        {
            DataResult<List<DeviceModel>> result = new DataResult<List<DeviceModel>>();

            try
            {
                // 获取设备表和监控点位表数据
                var devices = da.GetDevices();
                var monitorValues = da.GetMonitorValues();

                List<DeviceModel> deviceList = new List<DeviceModel>();
                // 遍历设备数据行，构建设备模型
                foreach (var dr in devices.AsEnumerable())
                {
                    DeviceModel dModel = new DeviceModel();
                    deviceList.Add(dModel);

                    // 填充设备基本属性
                    dModel.DeviceID = dr.Field<string>("d_id");
                    dModel.DeviceName = dr.Field<string>("d_name");
                    dModel.IsRunning = true;       // 设备默认为运行状态
                    dModel.IsWarning = false;      // 设备默认为无报警状态

                    // 遍历监控点位，为当前设备添加点位信息
                    foreach (var mv in monitorValues.AsEnumerable().Where(m => m.Field<string>("d_id") == dModel.DeviceID))
                    {
                        MonitorValueModel mvm = new MonitorValueModel();
                        dModel.MonitorValueList.Add(mvm);

                        // 填充监控点位基本属性
                        mvm.ValueId = mv.Field<string>("value_id");
                        mvm.ValueName = mv.Field<string>("value_name");
                        mvm.StorageAreaId = mv.Field<string>("s_area_id");
                        mvm.StartAddress = mv.Field<Int32>("address");
                        mvm.DataType = mv.Field<string>("data_type");
                        mvm.IsAlarm = mv.Field<Int32>("is_alarm") == 1;   // 是否启用报警
                        mvm.ValueDesc = mv.Field<string>("description");
                        mvm.Unit = mv.Field<string>("unit");

                        // 从数据库读取四级报警阈值，空值则默认为 0.0
                        var column = mv.Field<string>("alarm_lolo");
                        mvm.LoLoAlarm = column == null ? 0.0 : double.Parse(column);
                        column = mv.Field<string>("alarm_low");
                        mvm.LowAlarm = column == null ? 0.0 : double.Parse(column);
                        column = mv.Field<string>("alarm_high");
                        mvm.HighAlarm = column == null ? 0.0 : double.Parse(column);
                        column = mv.Field<string>("alarm_hihi");
                        mvm.HiHiAlarm = column == null ? 0.0 : double.Parse(column);

                        // 注册监控点位状态变化回调：当数值状态变化时触发
                        mvm.ValueStateChanged = (state, msg, value_id) =>
                        {
                            try
                            {
                                // 通过 UI 线程调度器更新报警消息列表
                                Application.Current?.Dispatcher.Invoke(() =>
                                {
                                    // 查找已有报警记录，若存在则移除
                                    var index = dModel.WarningMessageList.ToList().FindIndex(w => w.ValueId == value_id);
                                    if (index > -1)
                                        dModel.WarningMessageList.RemoveAt(index);

                                    // 如果当前状态不是正常，则添加报警消息
                                    if (state != Base.MonitorValueState.OK)
                                    {
                                        dModel.IsWarning = true;
                                        dModel.WarningMessageList.Add(new WarningMessageModel { ValueId = value_id, Message = msg });
                                    }
                                });

                                // 同步更新设备的报警状态标识
                                var ss = dModel.WarningMessageList.Count > 0;
                                if (dModel.IsWarning != ss)
                                {
                                    dModel.IsWarning = ss;
                                }
                            }
                            catch { } // 忽略回调中的异常，防止影响主流程
                        };
                    }
                }

                result.State = true;
                result.Data = deviceList;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 更新监控点位报警阈值，并同步到全局内存缓存 GlobalMonitor.DeviceList 中
        /// </summary>
        /// <param name="valueId">监控点位 ID</param>
        /// <param name="lolo">低低报阈值</param>
        /// <param name="low">低报阈值</param>
        /// <param name="high">高报阈值</param>
        /// <param name="hihi">高高报阈值</param>
        /// <returns>操作结果，包含是否成功及消息提示</returns>
        public DataResult UpdateAlarmThreshold(string valueId, double lolo, double low, double high, double hihi)
        {
            DataResult result = new DataResult();
            try
            {
                //更新数据库中的报警阈值
                result.State = da.UpdateMonitorValueAlarm(valueId, lolo, low, high, hihi);
                if (!result.State)
                {
                    result.Message = "数据库更新失败，未找到对应的监控点位";
                    return result;
                }

                //同步更新 GlobalMonitor 内存中对应点位的阈值，保证界面实时刷新
                foreach (var device in GlobalMonitor.DeviceList)
                {
                    var mv = device.MonitorValueList.FirstOrDefault(m => m.ValueId == valueId);
                    if (mv != null)
                    {
                        mv.LoLoAlarm = lolo;
                        mv.LowAlarm = low;
                        mv.HighAlarm = high;
                        mv.HiHiAlarm = hihi;
                        break;  //找到目标点位后即退出循环
                    }
                }

                result.State = true;
                result.Message = "更新成功";
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}

