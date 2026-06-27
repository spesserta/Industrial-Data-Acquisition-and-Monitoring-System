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
using Indus.Industrial.ViewModel;

namespace Indus.Industrial
{
    /// <summary>
    /// 主窗口类，承载系统的主要界面布局。
    /// 提供窗口拖拽移动和关闭按钮功能，并将主视图模型绑定到 DataContext。
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 构造函数：初始化界面组件并绑定主视图模型。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // 将 MainViewModel 设置为窗口的数据上下文，用于界面数据绑定
            this.DataContext = new MainViewModel();
        }

        /// <summary>
        /// 鼠标移动事件处理：当鼠标左键按下时拖拽移动整个窗口。
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">鼠标事件参数</param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            // 左键按下时调用 DragMove 实现窗口拖拽
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        /// <summary>
        /// 关闭按钮点击事件：关闭当前窗口。
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">路由事件参数</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

