using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indus.Industrial.Base
{
    /// <summary>
    /// 扩展方法静态类，为内置类型提供额外的转换功能
    /// </summary>
    public static class ExtendClass
    {

        public static float ByteArrsyToFloat(this byte[] value)
        {
            float fValue = 0f;
            // 将 4 个字节按大端序组合成一个 32 位无符号整数
            // 组合方式: byte[2]*256 + byte[3] + 65536*(byte[0]*256 + byte[1])

            uint nRest = ((uint)value[2]) * 256
                + ((uint)value[3]) +
                65536 * (((uint)value[0]) * 256 + ((uint)value[1]));
            unsafe
            {
                // 使用不安全代码将 uint 的内存地址直接 reinterpret 为 float
                float* ptemp;
                ptemp = (float*)(&nRest);
                fValue = *ptemp;
            }
            return fValue;
        }
    }
}

