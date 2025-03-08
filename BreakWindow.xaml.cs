using System;
using System.Windows;
using System.Windows.Threading;

namespace BreakTimer
{
    /// <summary>
    /// BreakWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BreakWindow : Window
    {
        public DispatcherTimer timer = null;
        DateTime targetTime;
        public BreakWindow(int breakMinute)
        {
            InitializeComponent();
            this.Topmost = true;
            StartBreakTimer(breakMinute);
        }
        private void StartBreakTimer(int breakMinute)
        {
            var timeSpan = TimeSpan.FromMinutes(breakMinute);
            TxtBreakTime.Text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
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
            TxtBreakTime.Text = $"{timeGap.Minutes:D2}:{timeGap.Seconds:D2}";
            if (timeGap < TimeSpan.Zero)
            {
                this.Close();
            }
        }

        private void ExitBreakBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
