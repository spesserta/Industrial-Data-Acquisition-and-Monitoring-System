using System;
using System.Collections.Generic;
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

namespace Indus.Controls.Components
{
    /// <summary>
    /// 管道（Pipeline）UI 组件，用于在工业监控界面上展示液体输送管道。
    /// 支持液体流向切换（自西向东 / 自东向西）和液体颜色自定义，
    /// 通过 VisualStateManager 切换流向动画。
    /// </summary>
    public partial class Pipeline : UserControl
    {
        /// <summary>
        /// 液体流向标识：1 表示自西向东（WEFlowState），2 表示自东向西（EWFlowState）。
        /// </summary>
        public int Direction
        {
            get { return (int)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }
        /// <summary>
        /// Direction 依赖属性，值变化时触发 OnDirectionChanged 回调以切换流向动画状态。
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(int), typeof(Pipeline), new PropertyMetadata(default(int), new PropertyChangedCallback(OnDirectionChanged)));

        /// <summary>
        /// 流向变化的静态回调，根据传入的值（1 或 2）切换到对应的 VisualState。
        /// </summary>
        private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int value = int.Parse(e.NewValue.ToString());
            // 1 为自西向东流向，2 为自东向西流向
            VisualStateManager.GoToState(d as Pipeline, value == 1 ? "WEFlowState" : "EWFlowState", false);
        }

        /// <summary>
        /// 管道内液体的颜色。默认橙色，用于视觉指示介质类型或温度区间。
        /// </summary>
        public Brush LiquidColor
        {
            get { return (Brush)GetValue(LiguidColorProperty); }
            set { SetValue(LiguidColorProperty, value); }
        }
        /// <summary>
        /// LiquidColor 依赖属性，注意注册名因笔误为 "LiguidColor"（无 u），使用需保持一致。
        /// </summary>
        public static readonly DependencyProperty LiguidColorProperty =
            DependencyProperty.Register("LiquidColor", typeof(Brush), typeof(Pipeline), new PropertyMetadata(Brushes.Orange));

        /// <summary>
        /// 管道端部圆角半径，用于控制管道两端圆弧的大小。
        /// </summary>
        public int CapRadius
        {
            get { return (int)GetValue(CapRadiusProperty); }
            set { SetValue(CapRadiusProperty, value); }
        }
        /// <summary>
        /// CapRadius 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CapRadiusProperty =
            DependencyProperty.Register("CapRadius", typeof(int), typeof(Pipeline), new PropertyMetadata(default(int)));



        /// <summary>
        /// 构造函数，初始化 XAML 组件。
        /// </summary>
        public Pipeline()
        {
            InitializeComponent();          // 加载 Pipeline.xaml 定义的 UI 布局和样式
        }
    }
}

