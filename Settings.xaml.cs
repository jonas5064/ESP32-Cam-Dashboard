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

        public List<String> urls = new List<string>();
        private int urls_num = 0;

        public Settings()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Setup a list with the urls and the number of them.
            urls.Add(url_1.Text);
            urls.Add(url_2.Text);
            urls.Add(url_3.Text);
            urls.Add(url_4.Text);
            urls.Add(url_5.Text);
            urls.Add(url_6.Text);
            urls.Add(url_7.Text);
            urls.Add(url_8.Text);
            urls_num = urls.Count;
            Console.WriteLine("Inside saved Urls Path:  " + MainWindow.saved_data_path);
            // Write to File
            foreach (String url in urls)
            {
                Files.write(MainWindow.saved_data_path, url);
            }
            this.Close();
        }
    }
}
