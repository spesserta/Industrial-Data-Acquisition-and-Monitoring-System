using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.Communication;
using Zhaoxi.Industrial.DAL;
using Zhaoxi.Industrial.Model;
using System.Data;
using System.Windows;

namespace Zhaoxi.Industrial.BLL
{
    public class IndustrialBLL
    {
        DataAccess da = new DataAccess();
        // 获取串口信息
        public DataResult<SerialInfo> InitSerialInfo()
        {
            DataResult<SerialInfo> result = new DataResult<SerialInfo>();

            try
            {
                SerialInfo si = new SerialInfo();
                si.PortName = ConfigurationManager.AppSettings["port"].ToString();
                si.BaudRate = int.Parse(ConfigurationManager.AppSettings["baud"].ToString());
                si.DataBit = int.Parse(ConfigurationManager.AppSettings["data_bit"].ToString());
                si.Parity = (Parity)Enum.Parse(typeof(Parity), ConfigurationManager.AppSettings["parity"].ToString(), true);
                si.StopBits = (StopBits)Enum.Parse(typeof(StopBits), ConfigurationManager.AppSettings["stop_bit"].ToString(), true);

                result.State = true;
                result.Data = si;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
            ;
        }

        public DataResult<List<StorageModel>> InitStorageArea()
        {
            DataResult<List<StorageModel>> result = new DataResult<List<StorageModel>>();

            try
            {
                var sa = da.GetStorageArea();

                result.State = true;
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
                result.Message = ex.Message;
            }

            return result;
        }

        public DataResult<List<DeviceModel>> InitDevices()
        {
            DataResult<List<DeviceModel>> result = new DataResult<List<DeviceModel>>();

            try
            {
                var devices = da.GetDevices();
                var monitorValues = da.GetMonitorValues();

                List<DeviceModel> deviceList = new List<DeviceModel>();
                foreach (var dr in devices.AsEnumerable())
                {
                    DeviceModel dModel = new DeviceModel();
                    deviceList.Add(dModel);

                    dModel.DeviceID = dr.Field<string>("d_id");
                    dModel.DeviceName = dr.Field<string>("d_name");
                    dModel.IsRunning = true;
                    dModel.IsWarning = false;

                    foreach (var mv in monitorValues.AsEnumerable().Where(m => m.Field<string>("d_id") == dModel.DeviceID))
                    {
                        MonitorValueModel mvm = new MonitorValueModel();
                        dModel.MonitorValueList.Add(mvm);

                        mvm.ValueId = mv.Field<string>("value_id");
                        mvm.ValueName = mv.Field<string>("value_name");
                        mvm.StorageAreaId = mv.Field<string>("s_area_id");
                        mvm.StartAddress = mv.Field<Int32>("address");
                        mvm.DataType = mv.Field<string>("data_type");
                        mvm.IsAlarm = mv.Field<Int32>("is_alarm") == 1;
                        mvm.ValueDesc = mv.Field<string>("description");
                        mvm.Unit = mv.Field<string>("unit");

                        // 警戒值
                        var column = mv.Field<string>("alarm_lolo");
                        mvm.LoLoAlarm = column == null ? 0.0 : double.Parse(column);
                        column = mv.Field<string>("alarm_low");
                        mvm.LowAlarm = column == null ? 0.0 : double.Parse(column);
                        column = mv.Field<string>("alarm_high");
                        mvm.HighAlarm = column == null ? 0.0 : double.Parse(column);
                        column = mv.Field<string>("alarm_hihi");
                        mvm.HiHiAlarm = column == null ? 0.0 : double.Parse(column);

                        mvm.ValueStateChanged = (state, msg, value_id) =>
                        {
                            try
                            {
                                Application.Current?.Dispatcher.Invoke(() =>
                                {
                                    var index = dModel.WarningMessageList.ToList().FindIndex(w => w.ValueId == value_id);
                                    if (index > -1)
                                        dModel.WarningMessageList.RemoveAt(index);

                                    if (state != Base.MonitorValueState.OK)
                                    {
                                        dModel.IsWarning = true;
                                        dModel.WarningMessageList.Add(new WarningMessageModel { ValueId = value_id, Message = msg });
                                    }
                                });

                                var ss = dModel.WarningMessageList.Count > 0;
                                if (dModel.IsWarning != ss)
                                {
                                    dModel.IsWarning = ss;
                                }
                            }
                            catch { }
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
    }
}
