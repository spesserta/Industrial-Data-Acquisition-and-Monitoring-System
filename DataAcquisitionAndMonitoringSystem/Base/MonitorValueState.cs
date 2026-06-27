using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indus.Industrial.Base
{
    /// <summary>
    /// 监控值状态枚举，表示当前设备监控值的告警级别
    /// </summary>
    public enum MonitorValueState
    {
        //正常范围
        OK = 0,
        //极低报警
        LoLo = 1,
        //过低报警
        Low = 2,
        //过高报警
        High = 3,
        //极高报警
        HiHi = 4
    }
}

