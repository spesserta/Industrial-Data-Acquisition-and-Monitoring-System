using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indus.Industrial.Model
{

    public class StorageModel
    {
        //存储区编号
        public string id { get; set; }

        //从站地址（Modbus 设备地址）
        public int SlaveAddress { get; set; }

        //功能码，用于指定读取操作类型（如 03: 读保持寄存器）
        public string FuncCode { get; set; }

        //存储区的起始寄存器地址
        public int StartAddress { get; set; }

        //需要读取的寄存器数量
        public int Length { get; set; }
    }
}

