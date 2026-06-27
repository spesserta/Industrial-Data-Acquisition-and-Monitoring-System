using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Indus.Controls
{
    /// <summary>
    /// 圆形进度条控件，通过 Arc 路径弧线动画展示百分比进度。
    /// 支持自定义背景色和前景色，适用于仪表盘、加载指示等场景。
    /// </summary>
    public partial class CircularProgressBar : UserControl
    {
        /// <summary>
        /// 当前进度值（0~100），超过 100 时会自动取模。
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        /// <summary>
        /// Value 依赖属性，值变化时触发 OnPropertyChanged 回调以更新 UI。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(default(double), new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// 进度条的背景圆弧颜色。默认浅灰色。
        /// </summary>
        public Brush BackColor
        {
            get { return (Brush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }
        /// <summary>
        /// BackColor 依赖属性。
        /// </summary>
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register("BackColor", typeof(Brush), typeof(CircularProgressBar), new PropertyMetadata(Brushes.LightGray));

        /// <summary>
        /// 进度条的前景弧线颜色（即已到达部分的颜色）。默认橙色。
        /// </summary>
        public Brush ForeColor
        {
            get { return (Brush)GetValue(ForeColorProperty); }
            set { SetValue(ForeColorProperty, value); }
        }
        /// <summary>
        /// ForeColor 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(Brush), typeof(CircularProgressBar), new PropertyMetadata(Brushes.Orange));


        /// <summary>
        /// 构造函数，初始化 XAML 组件并注册尺寸变化事件。
        /// </summary>
        public CircularProgressBar()
        {
            InitializeComponent();                  // 加载 XAML 定义的 UI 元素

            this.SizeChanged += CircularProgressBar_SizeChanged;    // 监听控件尺寸变化以重绘
        }

        /// <summary>
        /// 控件尺寸变化时重新计算并更新圆弧路径。
        /// </summary>
        private void CircularProgressBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateValue();                     // 尺寸变了需要重新绘制弧线
        }

        /// <summary>
        /// 依赖属性值变化时的静态回调，转发给实例方法 UpdateValue。
        /// </summary>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CircularProgressBar).UpdateValue();   // 强制更新圆弧路径
        }

        /// <summary>
        /// 根据当前进度值计算并设置 Arc 路径的几何数据，
        /// 通过 SVG 路径语法绘制从 12 点钟方向开始的圆弧。
        /// </summary>
        private void UpdateValue()
        {
            // 取控件宽高的较小值作为布局边长，保证圆形不被拉伸
            this.layout.Width = Math.Min(this.RenderSize.Width, this.RenderSize.Height);
            double radius = Math.Min(this.RenderSize.Width, this.RenderSize.Height) / 2;   // 半径
            if (radius <= 0) return;                // 半径无效时直接返回

            double newValue = this.Value % 100.0;   // 进度值取模，确保在 0~99.99 范围内
            double newX = 0.0, newY = 0.0;
            // 通过三角函数计算圆弧终点坐标：(value * 3.6) 将百分比转为角度，减 90 度使起点在 12 点钟方向
            newX = radius + (radius - 3) * Math.Cos((newValue * 3.6 - 90) * Math.PI / 180);
            newY = radius + (radius - 3) * Math.Sin((newValue * 3.6 - 90) * Math.PI / 180);

            // 参考路径格式：M75 3 A75 75 0 0 1 147 75
            // 其中 large-arc-flag 根据进度是否超过 50% 决定（0 为小弧，1 为大弧）
            string pathDataStr = "M{0} 3A{1} {1} 0 {4} 1 {2} {3}";
            pathDataStr = string.Format(pathDataStr,
                radius + 0.01,                      // 起点 X（略偏移避免重叠）
                radius - 3,                         // 圆弧半径（留 3px 边距）
                newX,                               // 终点 X
                newY,                               // 终点 Y
                newValue < 50 && newValue > 0 ? 0 : 1  // 小弧/大弧标志
                );
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));  // 获取 Geometry 类型转换器
            this.path.Data = (Geometry)converter.ConvertFrom(pathDataStr); // 将路径字符串转为几何对象并赋值
        }
    }
}

