using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indus.Industrial.Base
{
    /// <summary>
    /// 日志类型枚举，定义系统运行时日志的严重级别
    /// </summary>
    public enum LogType
    {
        
        Info = 0,   //普通日志 
       
        Warn = 1,  //警告日志
       
        Fault = 2  //严重警告
    }
}

