using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indus.Industrial.Base;

namespace Indus.Industrial.Model
{
    /// <summary>
    /// 日志数据模型，记录系统运行过程中的各类日志信息
    /// </summary>
    public class LogModel
    {
        /// <summary>
        /// 行号，用于日志列表的顺序编号
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// 产生日志的设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 日志的简要描述信息
        /// </summary>
        public string LogInfo { get; set; }

        /// <summary>
        /// 日志的详细消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 日志类型（Info / Warn / Fault）
        /// </summary>
        public LogType LogType { get; set; }
    }
}

