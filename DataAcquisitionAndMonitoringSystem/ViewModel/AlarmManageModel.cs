using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Indus.Industrial.Base;
using Indus.Industrial.BLL;
using Indus.Industrial.Model;

namespace Indus.Industrial.ViewModel
{

    public class AlarmManageModel : NotifyPropertyBase
    {
        //设备列表集合，用于界面绑定显示所有设备。
        public ObservableCollection<DeviceModel> DeviceList { get; set; } = new ObservableCollection<DeviceModel>();

        private DeviceModel _selectedDevice;
        //当前选中的设备对象，切换设备时自动重置选中的监控点位。
        public DeviceModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                this.RaisePropertyChanged();
                // 切换设备时重置选中监控点，避免残留旧设备的选中状态
                SelectedMonitorValue = null;
            }
        }

        private MonitorValueModel _selectedMonitorValue;
        /// 当前选中的监控点位，选中后自动将其现有阈值加载到可编辑字段中。
        public MonitorValueModel SelectedMonitorValue
        {
            get { return _selectedMonitorValue; }
            set
            {
                _selectedMonitorValue = value;
                this.RaisePropertyChanged();

                if (value != null)
                {
                    // 将选中点位的报警阈值加载到可编辑属性中，供用户修改
                    EditableLoLoAlarm = value.LoLoAlarm;
                    EditableLowAlarm = value.LowAlarm;
                    EditableHighAlarm = value.HighAlarm;
                    EditableHiHiAlarm = value.HiHiAlarm;
                }
            }
        }

        private double _editableLoLoAlarm;
        //可编辑的极低报警阈值（下限下限）。
        public double EditableLoLoAlarm
        {
            get { return _editableLoLoAlarm; }
            set { _editableLoLoAlarm = value; this.RaisePropertyChanged(); }
        }

        private double _editableLowAlarm;
        //编辑的低报警阈值（下限）。
        public double EditableLowAlarm
        {
            get { return _editableLowAlarm; }
            set { _editableLowAlarm = value; this.RaisePropertyChanged(); }
        }

        private double _editableHighAlarm;
        //可编辑的高报警阈值（上限）。
        public double EditableHighAlarm
        {
            get { return _editableHighAlarm; }
            set { _editableHighAlarm = value; this.RaisePropertyChanged(); }
        }

        private double _editableHiHiAlarm;
        //可编辑的极高报警阈值（上限上限）。
        public double EditableHiHiAlarm
        {
            get { return _editableHiHiAlarm; }
            set { _editableHiHiAlarm = value; this.RaisePropertyChanged(); }
        }

        //保存命令，绑定到保存按钮，执行阈值更新操作。
        public CommandBase SaveCommand { get; set; }

        //构造函数：初始化保存命令并加载设备列表数据。
        public AlarmManageModel()
        {
            // 初始化保存命令，绑定 OnSave 方法
            SaveCommand = new CommandBase(new Action<object>(OnSave));

            // 从全局监控数据中加载设备列表
            LoadData();
        }

        //从 GlobalMonitor 中加载所有设备到 DeviceList，并默认选中第一个设备。
        private void LoadData()
        {
            DeviceList.Clear();

            // 判断全局监控中是否存在设备数据
            if (GlobalMonitor.DeviceList != null && GlobalMonitor.DeviceList.Count > 0)
            {
                // 遍历设备列表并添加到 ObservableCollection
                foreach (var device in GlobalMonitor.DeviceList)
                {
                    DeviceList.Add(device);
                }
                // 默认选中第一个设备
                SelectedDevice = DeviceList.FirstOrDefault();
            }
        }

        //保存阈值变更：校验输入合法性后调用业务逻辑层更新数据库，并同步更新内存中的值。
        private void OnSave(object obj)
        {
            // 检查是否已选中监控点位
            if (SelectedMonitorValue == null)
            {
                MessageBox.Show("请先选择一个监控点位", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 简单校验：极低阈值必须小于低阈值
            if (EditableLoLoAlarm >= EditableLowAlarm)
            {
                MessageBox.Show("极低阈值(↓)必须小于低阈值(↓)", "输入校验", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // 简单校验：极高阈值必须大于高阈值
            if (EditableHiHiAlarm <= EditableHighAlarm)
            {
                MessageBox.Show("极高阈值(↑)必须大于高阈值(↑)", "输入校验", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 调用业务逻辑层更新阈值
            IndustrialBLL bll = new IndustrialBLL();
            var result = bll.UpdateAlarmThreshold(
                SelectedMonitorValue.ValueId,
                EditableLoLoAlarm,
                EditableLowAlarm,
                EditableHighAlarm,
                EditableHiHiAlarm
            );

            if (result.State)
            {
                // 更新成功：同步更新内存中 MonitorValueModel 的阈值字段
                SelectedMonitorValue.LoLoAlarm = EditableLoLoAlarm;
                SelectedMonitorValue.LowAlarm = EditableLowAlarm;
                SelectedMonitorValue.HighAlarm = EditableHighAlarm;
                SelectedMonitorValue.HiHiAlarm = EditableHiHiAlarm;

                MessageBox.Show("阈值更新成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // 更新失败：显示错误信息
                MessageBox.Show("更新失败：" + result.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

