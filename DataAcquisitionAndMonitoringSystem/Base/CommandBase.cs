using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Indus.Industrial.Base
{
    /// <summary>
    /// 命令基类，实现 ICommand 接口，用于 WPF 命令绑定
    /// </summary>
    public class CommandBase : ICommand
    {

        public event EventHandler CanExecuteChanged;



        public bool CanExecute(object parameter)
        {
            return true;
        }


        public void Execute(object parameter)
        {
            // 调用外部注册的执行逻辑
            this.DoExecute?.Invoke(parameter);
        }


        public Action<object> DoExecute { get; set; }


        public CommandBase() { }


        public CommandBase(Action<object> action)
        {
            DoExecute = action;
        }
    }
}

