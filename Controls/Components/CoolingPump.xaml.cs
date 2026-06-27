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
    /// 冷却水泵 UI 组件，继承自 ComponentBase。
    /// 用于在工业监控界面上展示冷却水泵的运行状态、故障状态等视觉信息，
    /// 并通过 XAML 视觉状态实现动态效果。
    /// </summary>
    public partial class CoolingPump : ComponentBase
    {
        /// <summary>
        /// 构造函数，初始化 XAML 组件。
        /// </summary>
        public CoolingPump()
        {
            InitializeComponent();          // 加载 CoolingPump.xaml 定义的 UI 布局和样式
        }
    }
}

