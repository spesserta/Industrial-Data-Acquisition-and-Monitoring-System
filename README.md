DataAcquisitionAndMonitoringSystem/  <br>
├── DataAcquisitionAndMonitoringSystem.sln  # 解决方案<br>
├── Communication/                           # 类库：串口通信层<br>
│   ├── Modbus/RTU.cs                        # Modbus RTU 协议实现<br>
│   └── SerialInfo.cs                        # 串口配置模型<br>
├── Controls/                                # 类库：自定义控件<br>
│   ├── CircularProgressBar.xaml(.cs)        # 环形进度条控件<br>
│   └── Components/<br>
│       ├── ComponentBase.cs                 # 设备组件基类<br>
│       ├── CoolingTower.xaml(.cs)           # 冷却塔图形组件<br>
│       ├── CoolingPump.xaml(.cs)            # 冷却水泵图形组件<br>
│       └── Pipeline.xaml(.cs)               # 管道流动画组件<br>
└── DataAcquisitionAndMonitoringSystem/      # WPF 主项目 (WinExe)<br>
    ├── App.xaml(.cs)                        # 应用入口，启动全局监控<br>
    ├── MainWindow.xaml(.cs)                 # 主窗口 + 导航栏<br>
    ├── App.config                           # 数据库连接 + 串口配置<br>
    ├── /Base/                               # 基础设施<br>
    │   ├── CommandBase.cs                   # ICommand 实现<br>
    │   ├── NotifyPropertyBase.cs            # INotifyPropertyChanged 实现<br>
    │   ├── GlobalMonitor.cs                 # 全局监控核心循环<br>
    │   ├── LogType.cs                       # 日志枚举<br>
    │   ├── MonitorValueState.cs             # 警戒状态枚举<br>
    │   ├── ExtendClass.cs                   # 字节转浮点扩展方法<br>
    │   └── Converter/Bool2Visibility.cs     # 布尔→可见性转换器<br>
    ├── /Model/                              # 数据模型<br>
    │   ├── DeviceModel.cs                   # 设备模型<br>
    │   ├── MonitorValueModel.cs             # 监控点位模型<br>
    │   ├── StorageModel.cs                  # 存储区模型<br>
    │   ├── LogModel.cs                      # 日志模型<br>
    │   └── WarningMessageModel.cs           # 报警消息模型<br>
    ├── /DAL/                                # 数据访问层<br>
    │   └── DataAccess.cs                    # SQL Server 查询<br>
    ├── /BLL/                                # 业务逻辑层<br>
    │   ├── IndustrialBLL.cs                 # 初始化业务逻辑<br>
    │   └── DataResult.cs                    # 通用结果包装<br>
    ├── /ViewModel/                          # 视图模型<br>
    │   ├── MainViewModel.cs                 # 主窗口 VM + 导航<br>
    │   └── SystemMonitorViewModel.cs        # 系统监控页 VM<br>
    └── /View/                               # 视图<br>
        ├── SystemMonitor.xaml(.cs)           # 系统监控主界面<br>
        └── ReportManagement.xaml(.cs)        # 报表页（预留）<br>
