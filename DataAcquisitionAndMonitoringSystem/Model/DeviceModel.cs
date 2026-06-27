using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indus.Industrial.Base;

namespace Indus.Industrial.Model
{
    /// <summary>
    /// 设备数据模型，继承自属性通知基类，表示一个被监控的工业设备。
    /// 包含设备基本信息、运行状态、报警状态及其关联的监控变量和报警消息。
    /// </summary>
    public class DeviceModel : NotifyPropertyBase
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        // 设备运行状态后端字段
        private bool _isRunning;

        /// <summary>
        /// 设备是否正在运行
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                // 通知 UI 属性已变更
                this.RaisePropertyChanged();
            }
        }

        // 设备报警状态后端字段，默认为 false
        private bool _isWarning = false;

        /// <summary>
        /// 设备是否处于报警状态
        /// </summary>
        public bool IsWarning
        {
            get { return _isWarning; }
            set
            {
                _isWarning = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 该设备关联的监控变量集合，用于展示实时数据
        /// </summary>
        public ObservableCollection<MonitorValueModel> MonitorValueList { get; set; } = new ObservableCollection<MonitorValueModel>();

        /// <summary>
        /// 该设备产生的报警消息集合
        /// </summary>
        public ObservableCollection<WarningMessageModel> WarningMessageList { get; set; } = new ObservableCollection<WarningMessageModel>();
    }
}

