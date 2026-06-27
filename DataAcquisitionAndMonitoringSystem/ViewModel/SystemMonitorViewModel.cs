using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indus.Industrial.Base;
using Indus.Industrial.Model;

namespace Indus.Industrial.ViewModel
{
    /// <summary>
    /// 系统监控视图模型，管理设备运行状态、日志信息列表以及设备详细面板的显示切换。
    /// 包含测试数据用于验证数据模板的展示效果，并提供设备组件点击命令以展开详情。
    /// </summary>
    public class SystemMonitorViewModel : NotifyPropertyBase
    {
        /// <summary>
        /// 运行日志列表，绑定到界面日志表格控件，记录设备启动、告警等信息。
        /// </summary>
        public ObservableCollection<LogModel> LogList { get; set; } = new ObservableCollection<LogModel>();

        private DeviceModel _currentDevice;
        /// <summary>
        /// 当前选中的设备对象，点击设备组件时赋值，用于显示设备详情面板。
        /// </summary>
        public DeviceModel CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; this.RaisePropertyChanged(); }
        }

        private bool _isShowDetail = false;
        /// <summary>
        /// 是否显示设备详情面板，点击设备组件时设为 true，隐藏时为 false。
        /// </summary>
        public bool IsShowDetail
        {
            get { return _isShowDetail; }
            set { _isShowDetail = value; this.RaisePropertyChanged(); }
        }

        /// <summary>
        /// 测试设备对象，用于在设计时验证数据模板的绑定效果。
        /// </summary>
        public DeviceModel TestDevice { get; set; }

        /// <summary>
        /// 设备组件点击命令，点击流程图中的设备时执行，用于显示设备详细面板。
        /// </summary>
        public CommandBase ComponentCommand { get; set; }

        /// <summary>
        /// 构造函数：初始化日志信息、测试数据以及设备组件点击命令。
        /// </summary>
        public SystemMonitorViewModel()
        {
            // 初始化默认日志列表
            InitLogInfo();

            #region 测试数据，用于验证数据模板的界面展示效果
            // 创建测试设备对象，填充模拟的监控点位数据
            TestDevice = new DeviceModel();
            TestDevice.DeviceName = "冷却塔 1#";
            TestDevice.IsRunning = true;
            TestDevice.IsWarning = true;
            TestDevice.MonitorValueList.Add(new MonitorValueModel
            {
                ValueId = "1",
                ValueName = "液位",
                Unit = "m",
                CurrentValue = 45,
                Values = new LiveCharts.ChartValues<LiveCharts.Defaults.ObservableValue> { new LiveCharts.Defaults.ObservableValue(0), new LiveCharts.Defaults.ObservableValue(0) }
            }); ;
            TestDevice.MonitorValueList.Add(new MonitorValueModel
            {
                ValueId = "1",
                ValueName = "入口压力",
                Unit = "Mpa",
                CurrentValue = 34,
                Values = new LiveCharts.ChartValues<LiveCharts.Defaults.ObservableValue> { new LiveCharts.Defaults.ObservableValue(0), new LiveCharts.Defaults.ObservableValue(0) }
            });
            TestDevice.MonitorValueList.Add(new MonitorValueModel
            {
                ValueId = "1",
                ValueName = "入口温度",
                Unit = "℃",
                CurrentValue = 34,
                Values = new LiveCharts.ChartValues<LiveCharts.Defaults.ObservableValue> { new LiveCharts.Defaults.ObservableValue(0), new LiveCharts.Defaults.ObservableValue(0) }
            });
            TestDevice.MonitorValueList.Add(new MonitorValueModel
            {
                ValueId = "1",
                ValueName = "出口压力",
                Unit = "Mpa",
                CurrentValue = 34,
                Values = new LiveCharts.ChartValues<LiveCharts.Defaults.ObservableValue> { new LiveCharts.Defaults.ObservableValue(0), new LiveCharts.Defaults.ObservableValue(0) }
            });
            TestDevice.MonitorValueList.Add(new MonitorValueModel
            {
                ValueId = "1",
                ValueName = "出口温度",
                Unit = "℃",
                CurrentValue = 34,
                Values = new LiveCharts.ChartValues<LiveCharts.Defaults.ObservableValue> { new LiveCharts.Defaults.ObservableValue(0), new LiveCharts.Defaults.ObservableValue(0) }
            });
            TestDevice.MonitorValueList.Add(new MonitorValueModel
            {
                ValueId = "1",
                ValueName = "补水压力",
                Unit = "Mpa",
                CurrentValue = 34,
                Values = new LiveCharts.ChartValues<LiveCharts.Defaults.ObservableValue> { new LiveCharts.Defaults.ObservableValue(0), new LiveCharts.Defaults.ObservableValue(0) }
            });

            // 添加测试告警消息
            TestDevice.WarningMessageList.Add(new WarningMessageModel { Message = "冷却塔1#液位极低，当前值：0" });
            TestDevice.WarningMessageList.Add(new WarningMessageModel { Message = "冷却塔1#入口压力极低，当前值：0" });
            TestDevice.WarningMessageList.Add(new WarningMessageModel { Message = "冷却塔1#入口温度极低，当前值：0" });
            #endregion

            // 初始化设备组件点击命令
            this.ComponentCommand = new CommandBase(new Action<object>(DoTowerCommand));
        }

        /// <summary>
        /// 初始化系统运行日志列表，填充一些模拟的启动和告警日志。
        /// </summary>
        void InitLogInfo()
        {
            this.LogList.Add(new LogModel { RowNumber = 1, DeviceName = "冷却塔 1#", LogInfo = "已启动", LogType = Base.LogType.Info });
            this.LogList.Add(new LogModel { RowNumber = 2, DeviceName = "冷却塔 2#", LogInfo = "已启动", LogType = Base.LogType.Info });
            this.LogList.Add(new LogModel { RowNumber = 3, DeviceName = "冷却塔 3#", LogInfo = "液位极低", LogType = Base.LogType.Warn });
            this.LogList.Add(new LogModel { RowNumber = 4, DeviceName = "循环水泵 1#", LogInfo = "频率过大", LogType = Base.LogType.Warn });
            this.LogList.Add(new LogModel { RowNumber = 5, DeviceName = "循环水泵 2#", LogInfo = "已启动", LogType = Base.LogType.Info });
            this.LogList.Add(new LogModel { RowNumber = 6, DeviceName = "循环水泵 3#", LogInfo = "已启动", LogType = Base.LogType.Info });
        }

        /// <summary>
        /// 设备组件点击命令执行方法：将点击的设备设为当前设备，并显示详情面板。
        /// </summary>
        /// <param name="param">命令参数，应为 DeviceModel 类型</param>
        private void DoTowerCommand(object param)
        {
            // 将点击的设备赋值给 CurrentDevice，用于详情面板绑定
            CurrentDevice = param as DeviceModel;
            // 显示设备详情面板
            this.IsShowDetail = true;
        }
    }
}

