# 工业数据采集与监控系统

### 一、项目简介

* 开发语言：C# <br>
* UI界面：WPF  <br>
* 架构：SCADA 
* 通信协议：ModbusRTU
* 数据库：SQL Server 2026<br>
* 数据库工具：SQL Server Management Studio 2022<br>
* 开发软件：Visual Studio <br>



### 二、运行展示

启动界面：

<img width="1301" height="749" alt="image" src="https://github.com/user-attachments/assets/a48b8c50-e37c-479b-be0d-2495b02f6dae" />


该项目采用串口通信，在启动项目后需要使用VSPD新建COM1和COM2这一对串口，然后该程序模拟上位机采用COM1串口，Modbus Slave软件采用COM2串口，在打开Slave软件后点击设置，将从站ID设置位1（1号冷却塔），存储区设置位03保持寄存器、首地址为0、长度为36

<img width="358" height="333" alt="image" src="https://github.com/user-attachments/assets/2706a4c1-80fb-48b2-9340-b0957be69bfd" />

设置完后点击连接，串口号选择COM2，当连接成功后界面更新为如下效果：

<img width="1298" height="748" alt="image" src="https://github.com/user-attachments/assets/5e66b4fe-a426-42f5-9090-de4cd3613a45" />


现在往保持寄存器里面填数据模拟PLC数据的变化，将数据格式设置为Float型大端AB CD，往里面填数值如下：


<img width="604" height="521" alt="image" src="https://github.com/user-attachments/assets/af5c973e-de7a-4c52-a8fb-be30a64e5d36" />

此时该程序会将PCL设置好的数值实时更新到UI界面中，当数值过高或者过低的时候，下方将出现报警信息，例如下图中的“冷却塔1#的入口压力极高。当前值：100”

<img width="1295" height="745" alt="image" src="https://github.com/user-attachments/assets/44ef1814-d0b0-42f9-a997-dbb97026ef05" />




### 三、数据库设计


数据库包括devices设备表（表示冷却塔设备的信息）、monitor_values监控信息表（表示各个冷却塔设备的监控信息）、storage_area存储区表（表示各个从站保持寄存器的配置值）
