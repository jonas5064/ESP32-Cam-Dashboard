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

namespace IPCamera
{
    public partial class Settings : Window

    {

        public Settings()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Setup a list with the urls and the number of them.
            MainWindow.urls.Add(url_1.Text);
            MainWindow.urls.Add(url_2.Text);
            MainWindow.urls.Add(url_3.Text);
            MainWindow.urls.Add(url_4.Text);
            MainWindow.urls.Add(url_5.Text);
            MainWindow.urls.Add(url_6.Text);
            MainWindow.urls.Add(url_7.Text);
            MainWindow.urls.Add(url_8.Text);
            MainWindow.urls_num = MainWindow.urls.Count;
        }
    }
}
