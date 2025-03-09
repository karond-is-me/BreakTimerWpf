using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace BreakTimer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer timer = null;
        DateTime targetTime;
        private long lockTimeTick = -1;

        private AppConfig userConfig = ConfigWindow.LoadAppConfig();
        public MainWindow()
        {
            InitializeComponent();
            // 订阅会话切换事件
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            StartBreakTimer();
        }

        private void StartBreakTimer()
        {
            var timeSpan = TimeSpan.FromMinutes(userConfig.PeriodMinute-userConfig.BreakMinute);
            TxtCountDown.Text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += timer_Tick;
            targetTime = DateTime.Now + timeSpan;
            timer.Start();
        }

        /// <summary>
        /// 计时逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            var timeGap = (targetTime - DateTime.Now);
            TxtCountDown.Text = $"{timeGap.Minutes:D2}:{timeGap.Seconds:D2}";
            if (timeGap < TimeSpan.Zero)
            {
                StartOneBreak();
            }
        }

        void StartOneBreak()
        {
            timer.Stop();
            var bw = new BreakWindow(userConfig.BreakMinute);
            bw.ShowDialog();
            var timeSpan = TimeSpan.FromMinutes(userConfig.PeriodMinute - userConfig.BreakMinute);
            targetTime = DateTime.Now + timeSpan;
            timer.Start();
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            // 判断事件类型
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    lockTimeTick = DateTime.Now.Ticks;
                    timer.Stop();
                    break;

                case SessionSwitchReason.SessionUnlock:
                    var nowTick = DateTime.Now.Ticks;
                    if (lockTimeTick != -1)
                    {
                        var idleMinute = (nowTick - lockTimeTick) / (double)TimeSpan.TicksPerMinute;
                        if (idleMinute > userConfig.IdleResetMinute)
                        {
                            var timeSpan = TimeSpan.FromMinutes(userConfig.PeriodMinute - userConfig.BreakMinute);
                            targetTime = DateTime.Now + timeSpan;
                        }
                        lockTimeTick = -1;
                    }
                    if (!timer.IsEnabled)
                    {
                        timer.Start();
                    }
                    
                    break;

                // 其他事件类型（可选）
                case SessionSwitchReason.ConsoleConnect:
                case SessionSwitchReason.ConsoleDisconnect:
                case SessionSwitchReason.RemoteConnect:
                case SessionSwitchReason.RemoteDisconnect:
                    // 处理其他会话切换事件
                    break;
            }
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        // 取消订阅事件（在不再需要监听时调用）
        public void Dispose()
        {
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }

        private void TaskbarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Show();
        }


        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void StartBreakBtn_Click(object sender, RoutedEventArgs e)
        {
            StartOneBreak();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            var timeSpan = TimeSpan.FromMinutes(userConfig.PeriodMinute - userConfig.BreakMinute);
            targetTime = DateTime.Now + timeSpan;
            timer.Start();
        }

        private void BtnStopOrStart_Click(object sender, RoutedEventArgs e)
        {
            if(BtnStopOrStart.Content.ToString() == "暂停")
            {
                timer.Stop();
                BtnStopOrStart.Content = "继续";
            }
            else
            {
                BtnStopOrStart.Content = "暂停";
                timer.Start();
            }
        }

        private void MenuItem_Config_Click(object sender, RoutedEventArgs e)
        {
            var configWin = new ConfigWindow();
            configWin.ShowDialog();
            userConfig = ConfigWindow.LoadAppConfig();
        }
    }
}
