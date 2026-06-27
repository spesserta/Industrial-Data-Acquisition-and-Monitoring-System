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
    /// 冷却塔 UI 组件，继承自 ComponentBase。
    /// 用于在工业监控界面上展示冷却塔的运行状态、故障告警等视觉信息，
    /// XAML 中定义了冷却塔的图形外观和动画效果。
    /// </summary>
    public partial class CoolingTower : ComponentBase
    {
        /// <summary>
        /// 构造函数，初始化 XAML 组件。
        /// </summary>
        public CoolingTower()
        {
            InitializeComponent();          // 加载 CoolingTower.xaml 定义的 UI 布局和样式
        }
    }
}

