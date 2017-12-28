﻿using AutumnBox.Basic.Function.Args;
using AutumnBox.Basic.Function.Modules;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using AutumnBox.Basic.Devices;
using AutumnBox.Basic.Function;
using AutumnBox.GUI.Helper;
using AutumnBox.GUI.Windows;

namespace AutumnBox.GUI.UI.FuncPanels
{
    /// <summary>
    /// RecoveryFunctions.xaml 的交互逻辑
    /// </summary>
    public partial class RecFuncPanel : UserControl, IRefreshable
    {
        private DeviceBasicInfo _currentDevInfo;
        public RecFuncPanel()
        {
            InitializeComponent();
        }

        public void Refresh(DeviceBasicInfo devInfo)
        {
            this._currentDevInfo = devInfo;
            UIHelper.SetGridButtonStatus(MainGrid,
                (_currentDevInfo.State == DeviceState.Recovery || _currentDevInfo.State == DeviceState.Sideload));
        }

        public void Reset()
        {
            _currentDevInfo = new DeviceBasicInfo() { State = DeviceState.None };
            UIHelper.SetGridButtonStatus(MainGrid, false);
        }

        private void ButtonPushFileToSdcard_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Reset();
            fileDialog.Title = App.Current.Resources["SelecteAFile"].ToString();
            fileDialog.Filter = "刷机包/压缩包文件(*.zip)|*.zip|镜像文件(*.img)|*.img|全部文件(*.*)|*.*";
            fileDialog.Multiselect = false;
            if (fileDialog.ShowDialog() == true)
            {
                var fmp = FunctionModuleProxy.Create<FileSender>(new FileSenderArgs(_currentDevInfo) { FilePath = fileDialog.FileName });
                fmp.Finished += ((MainWindow)App.Current.MainWindow).FuncFinish;
                fmp.AsyncRun();
                new FileSendingWindow(fmp).ShowDialog();
            }
            else
            {
                return;
            }
        }

        private void ButtonSideload_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
