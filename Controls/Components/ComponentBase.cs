using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Indus.Controls.Components
{
    /// <summary>
    /// 工业监控组件的基类，提供选中状态、运行状态、故障状态的统一管理，
    /// 以及命令绑定和视觉状态切换功能。所有核心组件（泵、塔、管道等）均继承此类。
    /// </summary>
    public class ComponentBase : UserControl
    {

        private bool _isSelected;  //是否选中


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;

                if (value)                          //设置为选中状态时
                {
                    // 获取父级 Grid 容器
                    var parent = VisualTreeHelper.GetParent(this) as Grid;
                    if (parent != null)
                    {
                        // 遍历父容器下的所有子元素，取消其他 ComponentBase 的选中状态
                        foreach (var item in parent.Children)
                        {
                            if (item is ComponentBase)
                                (item as ComponentBase).IsSelected = false;
                        }
                    }
                }

                // 切换到对应的视觉状态（选中/未选中）
                VisualStateManager.GoToState(this, value ? "SelectedState" : "UnselectedState", false);
            }
        }

        /// <summary>
        /// 获取或设置组件的运行状态。状态变化时通过 VisualStateManager 切换
        /// "RunState"（运行中）/ "StopState"（停止）视觉效果。
        /// </summary>
        public bool IsRunning    //获取或设置组件的运行状态
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }
        /// <summary>
        /// IsRunning 依赖属性，运行状态变化时触发 OnRunningStateChanged 回调。
        /// </summary>
        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register("IsRunning", typeof(bool), typeof(ComponentBase), new PropertyMetadata(default(bool), new PropertyChangedCallback(OnRunningStateChanged)));

        /// <summary>
        /// 运行状态变化的静态回调，切换 "RunState" 或 "StopState" 视觉状态。
        /// </summary>
        private static void OnRunningStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool state = (bool)e.NewValue;
            VisualStateManager.GoToState(d as ComponentBase, state ? "RunState" : "StopState", false);
        }

        /// <summary>
        /// 获取或设置组件的故障状态。状态变化时通过 VisualStateManager 切换
        /// "FaultState"（故障）/ "NormalState"（正常）视觉效果。
        /// </summary>
        public bool IsFault
        {
            get { return (bool)GetValue(IsFaultProperty); }
            set { SetValue(IsFaultProperty, value); }
        }
        /// <summary>
        /// IsFault 依赖属性，故障状态变化时触发 OnFaultStateChanged 回调。
        /// </summary>
        public static readonly DependencyProperty IsFaultProperty =
            DependencyProperty.Register("IsFault", typeof(bool), typeof(ComponentBase), new PropertyMetadata(default(bool), new PropertyChangedCallback(OnFaultStateChanged)));

        /// <summary>
        /// 故障状态变化的静态回调，切换 "FaultState" 或 "NormalState" 视觉状态。
        /// </summary>
        private static void OnFaultStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(d as ComponentBase, (bool)e.NewValue ? "FaultState" : "NormalState", false);
        }

        /// <summary>
        /// 获取或设置鼠标点击时要执行的命令，用于将交互行为绑定到 ViewModel。
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        /// <summary>
        /// Command 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ComponentBase), new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// 获取或设置命令参数，传递给 Command 的 Execute 方法。
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        /// <summary>
        /// CommandParameter 依赖属性。
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ComponentBase), new PropertyMetadata(default(object)));


        /// <summary>
        /// 构造函数，注册鼠标左键按下事件，用于切换选中状态和执行命令。
        /// </summary>
        public ComponentBase()
        {
            // 注册鼠标左键按下（Preview 隧道事件）的处理器
            this.PreviewMouseLeftButtonDown += ComponentBase_PreviewMouseLeftButtonDown;
        }

        /// <summary>
        /// 鼠标左键按下时的处理逻辑：切换选中状态，并执行绑定的命令。
        /// </summary>
        private void ComponentBase_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsSelected = !this.IsSelected;     // 切换选中/未选中状态

            this.Command?.Execute(this.CommandParameter);   // 如果绑定了命令则执行

            e.Handled = true;                       // 标记事件已处理，阻止继续冒泡
        }
    }
}

