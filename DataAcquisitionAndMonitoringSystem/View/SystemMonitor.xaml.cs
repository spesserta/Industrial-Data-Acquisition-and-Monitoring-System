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

namespace Indus.Industrial.View
{
    /// <summary>
    /// 系统监控页面，展示设备工艺流程图的用户控件。
    /// 支持通过鼠标滚轮缩放主视图区域，以及通过鼠标拖拽平移视图。
    /// </summary>
    public partial class SystemMonitor : UserControl
    {
        /// <summary>
        /// 构造函数：初始化页面组件。
        /// </summary>
        public SystemMonitor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 鼠标滚轮事件处理：缩放主视图区域的大小并自动居中。
        /// 当缩放后尺寸小于最小值时自动限制在最小值。
        /// </summary>
        /// <param name="sender">事件源，应为 Canvas</param>
        /// <param name="e">鼠标滚轮事件参数</param>
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 计算新的宽度和高度：在现有尺寸基础上加减滚轮滚动量
            double newWidth = this.mainView.ActualWidth + e.Delta;
            double newHeight = this.mainView.ActualHeight + e.Delta;

            // 限制最小宽度和高度，防止视图缩放过小
            if (newWidth < 500) newWidth = 500;
            if (newHeight < 100) newHeight = 100;

            // 更新主视图的宽高
            this.mainView.Width = newWidth;
            this.mainView.Height = newHeight;

            // 缩放后重新计算 Canvas.Left，使主视图居于容器中央
            this.mainView.SetValue(Canvas.LeftProperty, (this.RenderSize.Width - this.mainView.Width) / 2);

            // TODO: 可进行扩展，使用鼠标光标位置作为缩放中心点
        }

        // 拖拽移动相关状态变量
        bool _isMoving = false;             // 是否正在拖拽
        Point _downPoint = new Point(0, 0); // 鼠标按下时的坐标
        double left = 0, top = 0;           // 拖拽开始时主视图的 Left 和 Top 位置

        /// <summary>
        /// 鼠标左键按下事件处理：开始拖拽模式，记录按下位置并捕获鼠标。
        /// </summary>
        /// <param name="sender">事件源，应为 Canvas</param>
        /// <param name="e">鼠标按钮事件参数</param>
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMoving = true;
            // 记录鼠标按下时相对于 Canvas 的位置
            _downPoint = e.GetPosition(sender as Canvas);

            // 记录当前主视图的 Left 和 Top 位置
            left = double.Parse(this.mainView.GetValue(Canvas.LeftProperty).ToString());
            top = double.Parse(this.mainView.GetValue(Canvas.TopProperty).ToString());

            // 捕获鼠标，确保拖拽过程中不会丢失事件
            (sender as Canvas).CaptureMouse();
            e.Handled = true;
        }

        /// <summary>
        /// 鼠标左键松开事件处理：结束拖拽模式并释放鼠标捕获。
        /// </summary>
        /// <param name="sender">事件源，应为 Canvas</param>
        /// <param name="e">鼠标按钮事件参数</param>
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMoving = false;
            // 释放鼠标捕获
            (sender as Canvas).ReleaseMouseCapture();
            e.Handled = true;
        }

        /// <summary>
        /// 鼠标移动事件处理：在拖拽模式下，根据鼠标位移更新主视图的位置。
        /// </summary>
        /// <param name="sender">事件源，应为 Canvas</param>
        /// <param name="e">鼠标事件参数</param>
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMoving)
            {
                // 获取当前鼠标位置
                Point currentPoint = e.GetPosition(sender as Canvas);

                // 根据鼠标偏移量更新主视图的 Left 和 Top（实现平移效果）
                this.mainView.SetValue(Canvas.LeftProperty, left + (currentPoint.X - _downPoint.X));
                this.mainView.SetValue(Canvas.TopProperty, top + (currentPoint.Y - _downPoint.Y));

                e.Handled = true;
            }
        }
    }
}

