using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BreakTimer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            base.OnStartup(e);

            mutex = new Mutex(true, "breaktimerwpf", out bool createdNew);
            // 如果Mutex不是新创建的，表示已有实例正在运行
            if (!createdNew)
            {
                MessageBox.Show("该应用程序已经在运行。");
                Application.Current.Shutdown(); // 关闭新的实例
                return;
            }
            else
            {
                // 如果是新创建的Mutex，添加一个处理程序来在应用程序退出时释放Mutex
                Application.Current.Exit += (s, ee) => mutex.Dispose();
            }
        }
    }
}
