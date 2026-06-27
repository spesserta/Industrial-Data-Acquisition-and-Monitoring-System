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
    /// 报警管理页面，对应 AlarmManageModel 视图模型的用户界面。
    /// 提供设备列表选择、监控点位阈值展示与编辑功能，并支持保存修改后的报警阈值。
    /// </summary>
    public partial class AlarmManange : UserControl
    {
        /// <summary>
        /// 构造函数：初始化页面组件。
        /// </summary>
        public AlarmManange()
        {
            InitializeComponent();
        }
    }
}

