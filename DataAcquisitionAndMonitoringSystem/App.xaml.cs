using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Indus.Industrial.Base;

namespace Indus.Industrial
{
    /// <summary>
    /// 应用程序入口类，负责系统启动初始化与退出时的资源清理。
    /// 在应用启动时启动全局监控组件，启动失败时弹出错误提示并关闭应用。
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 重写应用程序启动方法，初始化全局监控模块。
        /// 启动成功后显示主窗口；启动失败则弹出错误消息并关闭应用。
        /// </summary>
        /// <param name="e">启动事件参数</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 启动全局监控模块，传入成功回调和失败回调
            GlobalMonitor.Start(
                () =>
                {
                    // 成功回调：在 UI 线程上显示主窗口
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        new MainWindow().Show();
                    });
                },
                (msg) =>
                {
                    // 失败回调：在 UI 线程上弹出错误消息并退出程序
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(msg, "系统启动失败");
                        Application.Current.Shutdown();
                    });
                });
        }

        /// <summary>
        /// 重写应用程序退出方法，释放全局监控占用的资源。
        /// </summary>
        /// <param name="e">退出事件参数</param>
        protected override void OnExit(ExitEventArgs e)
        {
            // 释放全局监控模块资源
            GlobalMonitor.Dispose();

            base.OnExit(e);
        }
    }
}

