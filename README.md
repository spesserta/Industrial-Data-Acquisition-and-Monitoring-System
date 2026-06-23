# 工业数据采集与监控系统

### 一、项目简介

* 开发语言：C# <br>
* UI界面：WPF  <br>
* 架构：SCADA 
* 通信协议：ModbusRTU
* 数据库：SQL Server 2026<br>
* 数据库工具：SQL Server Management Studio 2022<br>
* 开发软件：Visual Studio <br>

项目文件描述：
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

### 二、运行展示

启动界面：

<img width="1298" height="751" alt="image" src="https://github.com/user-attachments/assets/60c21a00-c7dd-4ff0-a751-0d23c3f5424f" />

该项目采用串口通信，在启动项目后需要使用VSPD新建COM1和COM2这一对串口，然后该程序模拟上位机采用COM1串口，Modbus Slave软件采用COM2串口，在打开Slave软件后点击设置，将从站ID设置位1（1号冷却塔），存储区设置位03保持寄存器、首地址为0、长度为36

<img width="358" height="333" alt="image" src="https://github.com/user-attachments/assets/2706a4c1-80fb-48b2-9340-b0957be69bfd" />

设置完后点击连接，串口号选择COM2，当连接成功后界面更新为如下效果：

<img width="1299" height="746" alt="image" src="https://github.com/user-attachments/assets/945d740e-4ff5-4294-a66a-1c6fe60e2769" />

现在往保持寄存器里面填数据模拟PLC数据的变化，将数据格式设置为Float型大端AB CD，往里面填数值如下：


<img width="604" height="521" alt="image" src="https://github.com/user-attachments/assets/af5c973e-de7a-4c52-a8fb-be30a64e5d36" />

此时该程序会将PCL设置好的数值实时更新到UI界面中，当数值过高或者过低的时候，下方将出现报警信息，例如下图中的“冷却塔1#的入口压力极高。当前值：100”

<img width="1296" height="745" alt="image" src="https://github.com/user-attachments/assets/82af6d22-0abb-41f5-b479-769c434edac6" />



### 三、数据库设计


数据库包括devices设备表（表示冷却塔设备的信息）、monitor_values监控信息表（表示各个冷却塔设备的监控信息）、storage_area存储区表（表示各个从站保持寄存器的配置值）














