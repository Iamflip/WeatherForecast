using MetricsManagerClient.Responces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MetricsManagerClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AllCpuMetricsApiResponse _responce;
        private int _last = 0;
        private DispatcherTimer _timer = null;

        public MainWindow(AllCpuMetricsApiResponse responce)
        {
            _responce = responce;
            InitializeComponent();
            timerStart();
        }
        
        private void timerStart()
        {
            _timer = new DispatcherTimer(); 
            _timer.Tick += new EventHandler(timerTick);
            _timer.Interval = new TimeSpan(0, 0, 5);
            _timer.Start();
        }
        private void timerTick(object sender, EventArgs e)
        {
            if (_responce.Metrics != null && _responce.Metrics.Count > _last)
            {
                if (CpuChart.ColumnServiesValues[0].Values.Count == 10)
                {
                    CpuChart.ColumnServiesValues[0].Values.RemoveAt(0);
                }
                CpuChart.ColumnServiesValues[0].Values.Add(_responce.Metrics[_last].Value);
                _last++;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_responce.Metrics != null && _responce.Metrics.Count > _last)
            {
                if (CpuChart.ColumnServiesValues[0].Values.Count == 10)
                {
                    CpuChart.ColumnServiesValues[0].Values.RemoveAt(0);
                }
                CpuChart.ColumnServiesValues[0].Values.Add(_responce.Metrics[_last].Value);
                _last++;
            }
        }
    }
}
