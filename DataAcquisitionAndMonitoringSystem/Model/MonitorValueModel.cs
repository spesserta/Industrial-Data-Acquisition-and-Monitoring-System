using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indus.Industrial.Base;

namespace Indus.Industrial.Model
{
    /// <summary>
    /// 监控变量数据模型，继承自属性通知基类。
    /// 表示一个具体的设备监控点位，包含其地址、数据类型、报警阈值、当前值和历史数据图表。
    /// </summary>
    public class MonitorValueModel : NotifyPropertyBase
    {
        //监控值状态变更事件，当当前值超出报警限值时触发，传递状态、消息和变量 ID
        public Action<MonitorValueState, string, string> ValueStateChanged;

        //监控变量编号
        public string ValueId { get; set; }

        //监控变量名称（如温度、压力等）
        public string ValueName { get; set; }

        //所属存储区标识（由从站地址 + 功能码 + 起始地址拼接而成）
        public string StorageAreaId { get; set; }

        //在存储区中的起始地址偏移
        public int StartAddress { get; set; }

        //数据类型（如 "Float"、"Bool" 等）
        public string DataType { get; set; }

        //是否启用报警检测
        public bool IsAlarm { get; set; }

        //极低报警阈值，低于此值触发 LoLo 报警
        public double LoLoAlarm { get; set; }

        //低报警阈值，低于此值触发 Low 报警
        public double LowAlarm { get; set; }

        //高报警阈值，高于此值触发 High 报警
        public double HighAlarm { get; set; }

        //极高报警阈值，高于此值触发 HiHi 报警
        public double HiHiAlarm { get; set; }

        //工程单位（如 ℃、MPa、L/min 等）
        public string Unit { get; set; }

        // 当前值后端字段
        private double _currentValue;

        public double CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;

                // 如果启用了报警检测，则根据阈值判断当前状态
                if (IsAlarm)
                {
                    string msg = ValueDesc;
                    MonitorValueState state = MonitorValueState.OK;

                    // 按优先级依次判断：极低 -> 过低 -> 极高 -> 过高
                    if (value < LoLoAlarm)
                    { msg += "极低"; state = MonitorValueState.LoLo; }
                    else if (value < LowAlarm)
                    { msg += "过低"; state = MonitorValueState.Low; }
                    else if (value > HiHiAlarm)
                    { msg += "极高"; state = MonitorValueState.HiHi; }
                    else if (value > HighAlarm)
                    { msg += "过高"; state = MonitorValueState.High; }

                    // 触发状态变更事件，传递报警消息和变量 ID
                    ValueStateChanged(state, msg + "。当前值：" + value.ToString(), ValueId);
                }

                // 将当前值添加到历史数据图表中（用于实时曲线绘制）
                Values.Add(new ObservableValue(value));
                // 最多保留 60 个数据点，超出时移除最早的数据
                if (Values.Count > 60) Values.RemoveAt(0);

                // 通知 UI 属性已变更
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 变量描述信息，用于报警消息的拼接
        /// </summary>
        public string ValueDesc { get; set; }

        /// <summary>
        /// 用于 LiveCharts 图表绑定的历史数值集合，支持实时曲线展示
        /// </summary>
        public ChartValues<ObservableValue> Values { get; set; } = new ChartValues<ObservableValue>();
    }
}

