using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indus.Industrial.Model
{
    /// 报警消息数据模型，记录设备监控变量触发的报警信息
    public class WarningMessageModel
    {
        //触发报警的监控变量编号
        public string ValueId { get; set; }

        //报警消息详细内容（包含报警类型和当前值等）
        public string Message { get; set; }
    }
}

