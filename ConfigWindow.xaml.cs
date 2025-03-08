using System;
using System.IO;
using System.Windows;
using System.Text.Json;
using System.Linq;
using System.Windows.Documents;
using System.Collections.Generic;

namespace BreakTimer
{

    public class CombItem<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
    }

    public class AppConfig
    {
        public int PeriodMinute { get; set; } = 30;
        public int BreakMinute { get; set; } = 2;
        public int IdleResetMinute { get; set; } = 5;
    }
    /// <summary>
    /// ConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow : Window
    {

        private AppConfig userConfig = LoadAppConfig();

        public ConfigWindow()
        {
            InitializeComponent();
            SetupByUserConfig();
        }

        private void SetupByUserConfig()
        {
            var PeriodItem = new List<CombItem<int>>
            {
                new CombItem<int>{Name = "30",Value= 30 },
                new CombItem<int>{Name = "60",Value= 60 },
                new CombItem<int>{Name = "90",Value= 90 },
                new CombItem<int>{Name = "120",Value= 120 },
            };
            var BreakItem = new List<CombItem<int>>
            {
                new CombItem<int>{Name = "1",Value= 1 },
                new CombItem<int>{Name = "2",Value= 2 },
                new CombItem<int>{Name = "5",Value= 5 },
                new CombItem<int>{Name = "10",Value= 10 },
            };
            var IdleResetItem = new List<CombItem<int>>
            {
                new CombItem<int>{Name = "5",Value= 5 },
                new CombItem<int>{Name = "10",Value= 10 }
            };
            CombPeriodMinute.ItemsSource = PeriodItem;
            CombPeriodMinute.DisplayMemberPath = "Name";
            CombPeriodMinute.SelectedValuePath = "Value";
            CombBreakMinute.ItemsSource = BreakItem;
            CombBreakMinute.DisplayMemberPath = "Name";
            CombBreakMinute.SelectedValuePath = "Value";
            CombIdleResetMinute.ItemsSource = IdleResetItem;
            CombIdleResetMinute.DisplayMemberPath = "Name";
            CombIdleResetMinute.SelectedValuePath = "Value";
            CombPeriodMinute.SelectedValue = userConfig.PeriodMinute;
            CombBreakMinute.SelectedValue = userConfig.BreakMinute;
            CombIdleResetMinute.SelectedValue = userConfig.IdleResetMinute;
        }

        private void SaveFormConfigToFile()
        {
            userConfig.PeriodMinute = ((CombItem<int>)CombPeriodMinute.SelectedItem).Value;
            userConfig.BreakMinute = ((CombItem<int>)CombBreakMinute.SelectedItem).Value;
            userConfig.IdleResetMinute = ((CombItem<int>)CombIdleResetMinute.SelectedItem).Value;
            SaveAppConfig(userConfig);
        }

        public static AppConfig LoadAppConfig()
        {
            var userConfig = new AppConfig();
            // 获取用户的应用数据目录
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folderPath = System.IO.Path.Combine(appDataPath, "BreakTimerWpf"); // 替换为你的应用名称
            var configPath = System.IO.Path.Combine(folderPath, "config.txt");

            // 确保目录存在
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 如果数据库文件不存在，则创建一个空的数据库文件
            if (!File.Exists(configPath))
            {
                return userConfig;
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(configPath);

                    // 反序列化到配置类
                    userConfig = JsonSerializer.Deserialize<AppConfig>(json);
                    return userConfig;
                }
                catch (Exception e)
                {
                    File.Delete(configPath);
                } 
            }
            return userConfig;
        }

        public static bool SaveAppConfig(AppConfig userConfig)
        {
            // 获取用户的应用数据目录
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folderPath = System.IO.Path.Combine(appDataPath, "BreakTimerWpf"); // 替换为你的应用名称
            var configPath = System.IO.Path.Combine(folderPath, "config.txt");

            // 确保目录存在
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            // 配置序列化选项（可选）
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,    // 格式化输出（缩进）
                IncludeFields = false,   // 不包含字段（仅属性）
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 处理特殊字符
            };

            // 序列化为JSON字符串
            string json = JsonSerializer.Serialize(userConfig, options);

            File.WriteAllText(configPath, json);
            return true;
        }


        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFormConfigToFile();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
