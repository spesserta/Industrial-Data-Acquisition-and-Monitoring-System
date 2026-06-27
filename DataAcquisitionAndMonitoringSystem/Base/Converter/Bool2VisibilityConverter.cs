using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Indus.Industrial.Base.Converter
{
    /// <summary>
    /// Bool值到 Visibility 的转换器。当绑定值为 true 时返回 Visible，否则返回 Collapsed。
    /// </summary>
    public class Bool2VisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)   //bool转是否可见
        {
            // 尝试将值解析为bool，成功后如果为true则显示，否则隐藏
            if (value != null && bool.TryParse(value.ToString(), out bool result) && result)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// 反向转换（未使用，返回 null）
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

