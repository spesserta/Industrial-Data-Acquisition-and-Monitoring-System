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
    /// 报表管理页面，用于展示和导出生设备运行数据报表。
    /// 当前为页面容器，具体报表逻辑可在后续开发中扩展。
    /// </summary>
    public partial class ReportManagement : UserControl
    {
        /// <summary>
        /// 构造函数：初始化页面组件。
        /// </summary>
        public ReportManagement()
        {
            InitializeComponent();
        }
    }
}

