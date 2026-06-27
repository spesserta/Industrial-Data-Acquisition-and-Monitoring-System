using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Indus.Industrial.Base
{
    /// <summary>
    /// 属性变更通知基类，实现 INotifyPropertyChanged 接口，
    /// 为 WPF 数据绑定提供属性变更通知支持
    /// </summary>
    public class NotifyPropertyBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 当属性值发生变化时触发的事件，通知 UI 更新绑定
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 触发属性变更通知
        /// </summary>
        /// <param name="propName">发生变更的属性名称，默认由编译器自动填充</param>
        public void RaisePropertyChanged([CallerMemberName] string propName = "")
        {
            // 使用空条件运算符触发事件，若无可跳过
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /// <summary>
        /// 设置属性值并自动触发变更通知（若新值与旧值不同）
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="field">属性对应的后端字段引用</param>
        /// <param name="value">要设置的新值</param>
        /// <param name="propName">属性名称，默认由编译器自动填充</param>
        public void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            // 比较新旧值是否相等，若相等则不做任何操作
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            // 更新字段值并触发通知
            field = value;
            RaisePropertyChanged(propName);
        }
    }
}

