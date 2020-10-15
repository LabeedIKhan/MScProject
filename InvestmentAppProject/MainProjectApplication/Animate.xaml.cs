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
using System.ComponentModel;
using System.Threading;

namespace MainProjectApplication
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        /*  This class run animation window 
         *  called from InterAPI.cs method 
         *  StartAnimationThread() closes
         *  when thead is terminated that runs 
         *  it
         */

        public Window3()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception)
            {
               
                throw;
            }
        }

        public void finiThread()
        {
            this.Close();
        }
    }
}
