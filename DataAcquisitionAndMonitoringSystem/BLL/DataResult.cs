using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indus.Industrial.BLL
{
    /// <summary>
    /// 泛型数据结果类，封装业务逻辑层的操作返回结果
    /// </summary>
    public class DataResult<T>
    {
        //操作是否成功
        public bool State { get; set; } = false;


        //操作返回的消息（如错误信息或成功提示）
        public string Message { get; set; }

        //操作返回的数据
        public T Data { get; set; }
    }


    //非泛型数据结果类，默认返回 string 类型数据

    public class DataResult : DataResult<string> { }
}

