using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Indus.Industrial.Base;

namespace Indus.Industrial.ViewModel
{
    /// 主视图模型，管理主界面右侧内容的切换。
    public class MainViewModel : NotifyPropertyBase
    {
        private UIElement _mainContent;
  
        /// 主内容区域显示的 UI 元素，根据当前选中的标签页动态切换。
 
        public UIElement MainContent
        {
            get { return _mainContent; }
            set
            {
                // 通过泛型 Set 方法触发属性变更通知
                Set<UIElement>(ref _mainContent, value);
            }
        }


        /// 标签切换命令，绑定到导航标签的点击事件。
        public CommandBase TabChangedCommand { get; set; }

        /// 构造函数：初始化标签切换命令，并默认加载系统监控视图。

        public MainViewModel()
        {
            // 初始化标签切换命令，绑定 OnTabChanged 方法
            TabChangedCommand = new CommandBase(OnTabChanged);

            // 启动时默认显示系统监控页面
            OnTabChanged("Indus.Industrial.View.SystemMonitor");
        }

        /// 标签切换处理：根据传入的视图类型全名，通过反射创建对应的 UserControl 实例并显示。
        private void OnTabChanged(object obj)
        {
            if (obj == null) return;

            // 完整方式（注释留作参考）：从外部程序集加载类型
            //string[] strValues = o.ToString().Split('|');
            //Assembly assembly = Assembly.LoadFrom(strValues[0]);
            //Type type = assembly.GetType(strValues[1]);
            //this.MainContent = (UIElement)Activator.CreateInstance(type);

            // 简化方式：通过类型全名在当前程序集中创建实例（必须在同一个程序集下）
            Type type = Type.GetType(obj.ToString());
            this.MainContent = (UIElement)Activator.CreateInstance(type);
        }
    }
}

