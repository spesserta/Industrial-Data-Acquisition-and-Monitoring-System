## 工业数据采集与监控系统

### 项目架构图：
```
DataAcquisitionAndMonitoringSystem/ 
├── DataAcquisitionAndMonitoringSystem.sln  # 解决方案
├── Communication/                           # 类库：串口通信层
│   ├── Modbus/RTU.cs                        # Modbus RTU协议实现
│   └── SerialInfo.cs                        # 串口配置模型
├── Controls/                                # 类库：自定义控件
│   ├── CircularProgressBar.xaml(.cs)        # 环形进度条控件
│   └── Components/<br>
│       ├── ComponentBase.cs                 # 设备组件基类
│       ├── CoolingTower.xaml(.cs)           # 冷却塔图形组件
│       ├── CoolingPump.xaml(.cs)            # 冷却水泵图形组件
│       └── Pipeline.xaml(.cs)               # 管道流动画组件
└── DataAcquisitionAndMonitoringSystem/      # WPF 主项目
    ├── App.xaml(.cs)                        # 应用入口，启动全局监控
    ├── MainWindow.xaml(.cs)                 # 主窗口+导航栏
    ├── App.config                           # 数据库连接+串口配置
    ├── /Base/                               # 基础设施
    │   ├── CommandBase.cs                   # ICommand实现
    │   ├── NotifyPropertyBase.cs            # INotifyPropertyChanged实现
    │   ├── GlobalMonitor.cs                 # 全局监控核心循环
    │   ├── LogType.cs                       # 日志枚举
    │   ├── MonitorValueState.cs             # 警戒状态枚举
    │   ├── ExtendClass.cs                   # 字节转浮点扩展方法
    │   └── Converter/Bool2Visibility.cs     # 布尔→可见性转换器
    ├── /Model/                              # 数据模型
    │   ├── DeviceModel.cs                   # 设备模型
    │   ├── MonitorValueModel.cs             # 监控点位模型
    │   ├── StorageModel.cs                  # 存储区模型
    │   ├── LogModel.cs                      # 日志模型
    │   └── WarningMessageModel.cs           # 报警消息模型
    ├── /DAL/                                # 数据访问层
    │   └── DataAccess.cs                    # SQL Server查询
    ├── /BLL/                                # 业务逻辑层
    │   ├── IndustrialBLL.cs                 # 初始化业务逻辑
    │   └── DataResult.cs                    # 通用结果包装
    ├── /ViewModel/                          # 视图模型
    │   ├── MainViewModel.cs                 # 主窗口VM+导航
    │   └── SystemMonitorViewModel.cs        # 系统监控页VM
    └── /View/                               # 视图
        ├── SystemMonitor.xaml(.cs)           # 系统监控主界面
        └── ReportManagement.xaml(.cs)        # 报表页（预留）
```
