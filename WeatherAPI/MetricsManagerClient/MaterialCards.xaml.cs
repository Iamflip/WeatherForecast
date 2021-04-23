using LiveCharts;
using LiveCharts.Wpf;
using MetricsManagerClient.Responces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace MetricsManagerClient
{
    /// <summary>
    /// Логика взаимодействия для MaterialCards.xaml
    /// </summary>
    public partial class MaterialCards : UserControl, INotifyPropertyChanged
    {
        public MaterialCards()
        {
            InitializeComponent();


            ColumnServiesValues = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<int> { 0 }
                }
            };

            DataContext = this;
        }

        public SeriesCollection ColumnServiesValues { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateOnСlick(object sender, RoutedEventArgs e)
        {
            TimePowerChart.Update(true);
        }
    }
}
