﻿using AutumnBox.Basic.Devices;
using AutumnBox.GUI.Helper;
using AutumnBox.GUI.I18N;
using AutumnBox.GUI.Resources;
using AutumnBox.Support.CstmDebug;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AutumnBox.GUI.UI.FuncPanels
{

    [LogProperty(TAG = "DevInfoPanel")]
    /// <summary>
    /// DeviceInfoPanel.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceInfoPanel : UserControl, IAsyncRefreshable, ILogSender
    {
        public DeviceBasicInfo DeviceInfo { get; private set; }
        public Version CurrentDeviceAndroidVersion { get; private set; }
        public bool CurrentDeviceIsRoot { get; private set; }
        public string LogTag => "DevInfoPanel";
        public bool IsShowLog => true;
        public DeviceInfoPanel()
        {
            InitializeComponent();
        }
        public event EventHandler RefreshStart;
        public event EventHandler RefreshFinished;
        private void SetStatusShow(BitmapSource source, string key)
        {
            this.DeviceStatusImage.Source = source;
            this.DeviceStatusLabel.Content = App.Current.Resources[key] ?? key;
        }
        public void Refresh(DeviceBasicInfo devSimpleInfo)
        {
            this.DeviceInfo = devSimpleInfo;
            if (devSimpleInfo.State == DeviceState.Poweron || devSimpleInfo.State == DeviceState.Recovery)
            {
                SetByDeviceSimpleInfoAsync(devSimpleInfo);
                RefreshStart?.Invoke(this, new EventArgs());
            }
            else if (devSimpleInfo.State == DeviceState.Unauthorized)
            {
                Reset();
                UIHelper.SetGridLabelsContent(GridBuildInfo, App.Current.Resources["PleaseAllowUSBDebug"]);
                SetStatusShow(DevStatusBitmapGetter.Get(devSimpleInfo.State), "PleaseAllowUSBDebug");
            }
            else if (devSimpleInfo.State == DeviceState.Fastboot)
            {
                UIHelper.SetGridLabelsContent(GridBuildInfo, "...");
                UIHelper.SetGridLabelsContent(GridHardwareInfo, "....");
                UIHelper.SetGridLabelsContent(GridMemoryInfo, "....");
                SetStatusShow(DevStatusBitmapGetter.Get(devSimpleInfo.State), "DeviceInFastboot");
            }
            else if (devSimpleInfo.State == DeviceState.Offline)
            {
                UIHelper.SetGridLabelsContent(GridBuildInfo, "...");
                UIHelper.SetGridLabelsContent(GridHardwareInfo, "....");
                UIHelper.SetGridLabelsContent(GridMemoryInfo, "....");
                SetStatusShow(DevStatusBitmapGetter.Get(devSimpleInfo.State), "DeviceOffline");
            }
            else
            {
                Reset();
            }
        }
        public void Reset()
        {
            this.Dispatcher.Invoke(() =>
            {
                UIHelper.SetGridLabelsContent(GridBuildInfo, App.Current.Resources["PleaseSelectedADevice"]);
                UIHelper.SetGridLabelsContent(GridHardwareInfo, "....");
                UIHelper.SetGridLabelsContent(GridMemoryInfo, "....");
                SetStatusShow(DevStatusBitmapGetter.Get(DeviceState.None), "PleaseSelectedADevice");
            });
        }
        private async void SetByDeviceSimpleInfoAsync(DeviceBasicInfo devSimpleInfo)
        {
            using (var propGetter = new DeviceBuildPropGetter(devSimpleInfo.Serial))
            {
                var buildInfo = await Task.Run(() =>
                {
                    return propGetter.GetFull();
                });
                LabelAndroidVersion.Content = buildInfo["ro.build.version.release"] ?? App.Current.Resources["GetFail"].ToString();
                LabelModel.Content = buildInfo["ro.product.brand"] + " " + buildInfo["ro.product.model"] ?? App.Current.Resources["GetFail"].ToString();
                LabelCode.Content = buildInfo["ro.product.name"] ?? App.Current.Resources["GetFail"].ToString();
            }
            LabelRom.Content = App.Current.Resources["Getting"].ToString();
            LabelRam.Content = App.Current.Resources["Getting"].ToString();
            LabelBattery.Content = App.Current.Resources["Getting"].ToString();
            LabelSOC.Content = App.Current.Resources["Getting"].ToString();
            LabelScreen.Content = App.Current.Resources["Getting"].ToString();
            LabelFlashMemInfo.Content = App.Current.Resources["Getting"].ToString();
            LabelRootStatus.Content = App.Current.Resources["Getting"].ToString();
            try
            {
                CurrentDeviceAndroidVersion = new Version(LabelAndroidVersion.Content.ToString());
            }
            catch
            {
                CurrentDeviceAndroidVersion = null;
            }
            switch (devSimpleInfo.State)
            {
                case DeviceState.Fastboot:
                    SetStatusShow(DevStatusBitmapGetter.Get(devSimpleInfo.State), "DeviceInFastboot");
                    break;
                case DeviceState.Recovery:
                    SetStatusShow(DevStatusBitmapGetter.Get(devSimpleInfo.State), "DeviceInRecovery");
                    break;
                case DeviceState.Poweron:
                    SetStatusShow(DevStatusBitmapGetter.Get(devSimpleInfo.State), "DeviceInRunning");
                    break;
                case DeviceState.Sideload:
                    SetStatusShow(DevStatusBitmapGetter.Get(devSimpleInfo.State), "DeviceInSideload");
                    break;
            }
            //await Task.Run(() => { Thread.Sleep(1000); });
            RefreshFinished?.Invoke(this, new EventArgs());
            var advInfo = await Task.Run(() =>
            {
                return DeviceInfoHelper.GetHwInfo(devSimpleInfo.Serial);
            });
            LabelRom.Content = (advInfo.StorageTotal != null) ? advInfo.StorageTotal + "GB" : App.Current.Resources["GetFail"].ToString();
            LabelRam.Content = (advInfo.MemTotal != null) ? advInfo.MemTotal + "GB" : App.Current.Resources["GetFail"].ToString();
            LabelBattery.Content = (advInfo.BatteryLevel != null) ? advInfo.BatteryLevel + "%" : App.Current.Resources["GetFail"].ToString();
            LabelSOC.Content = advInfo.SOCInfo ?? App.Current.Resources["GetFail"].ToString();
            LabelScreen.Content = advInfo.ScreenInfo ?? App.Current.Resources["GetFail"].ToString();
            LabelFlashMemInfo.Content = advInfo.FlashMemoryType ?? App.Current.Resources["GetFail"].ToString();
            bool IsRoot = await Task.Run(() => { return DeviceInfoHelper.CheckRoot(devSimpleInfo.Serial); });
            LabelRootStatus.Content = IsRoot ? App.Current.Resources["RootEnable"].ToString() : App.Current.Resources["RootDisable"].ToString();
            CurrentDeviceIsRoot = IsRoot;
        }

        private void LabelAndroidVersion_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LanguageHelper.LanguageChanged += (s, ex) => { Refresh(DeviceInfo); };
        }

        private void ImgRefreshDevInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Refresh(DeviceInfo);
        }
    }
}
