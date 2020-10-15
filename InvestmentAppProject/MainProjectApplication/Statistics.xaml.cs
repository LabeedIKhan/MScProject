using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MainProjectApplication
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    /// 
    /* =========Charts===========
     * This class uses and exnternal chart creation package
     * by livecharts added to project as nuget package 
     * from visual studio nuget package manager. 
     * The website of live charts is https://lvcharts.net/
     * usage idea has been taken from 
     * https://lvcharts.net/App/examples/wpf/start
     * and more information is avaiable from nuget
     * official site https://www.nuget.org/packages/LiveCharts.Wpf/
     * auther and owner BetoRodriguez 
     * To see installed packages right click on project name
     * and select Manage Nuget package and select installed packages.
     */
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

       public void ShowStatisticsGrid(string[][] data, string[] date, double[] stockMovement)
        {
            /* This method uses live charts package for show
             * values as chart, inforamtion about package and 
             * links given in description above. 
             */

            StatGrid.ItemsSource = data;
            // Create Line Seriese
            LineSeries line = new LineSeries { Title = "Stock", Values = new ChartValues<double>(stockMovement) };
            // Add Line series to charts
            IndividualStock.Series.Add(line);

            List<string> dateList = new List<string>();
            if (date != null)
            {
                for (int i = 0; i < date.Length; i++)
                {
                    dateList.Add(date[i]);
                }
                // add labels to charts
                IndividualStock.AxisX.First().Labels = dateList;
            }

            double Up = 0.0;
            double down = 0.0;

            for (int i = 0; i < stockMovement.Length-1; i++)
            {
                if (stockMovement[i] > stockMovement[i+1])
                {
                    Up++;
                }
                else
                {
                    down++;
                }
            }
            // add pieseries to pie chart
            StockMovement.Series.Add(new PieSeries { Title = "UP", Values = new ChartValues<double> { Up } });
            StockMovement.Series.Add(new PieSeries { Title = "Down", Values = new ChartValues<double> { down } });
        }

        private void StatClose_Click(object sender, RoutedEventArgs e)
        {
            // on pressing ok from this window close it
            this.Close();
        }
    }
}
