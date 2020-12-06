using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            // Save Data To Database
            using (SqlConnection connection = new SqlConnection(MainWindow.DB_connection_string))
            {
                String query = "INSERT INTO dbo.MyCameras (id,urls,password,Name) VALUES (@id,@username,@password, @email)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", "abc");
                    command.Parameters.AddWithValue("@urls", "abc");
                    command.Parameters.AddWithValue("@Name", "abc");
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    // Check Error
                    if (result < 0)
                        Console.WriteLine("Error inserting data into Database!");
                }
            }
        }
    }
}
