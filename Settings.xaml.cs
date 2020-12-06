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
            if (url_1.Text != "" && name_1.Text != "")
            {
                MainWindow.urls.Add(url_1.Text, name_1.Text);
            }
            if (url_2.Text != "" && name_2.Text != "")
            {
                MainWindow.urls.Add(url_2.Text, name_2.Text);
            }
            if (url_3.Text != "" && name_3.Text != "")
            {
                MainWindow.urls.Add(url_3.Text, name_3.Text);
            }
            if (url_4.Text != "" && name_4.Text != "")
            {
                MainWindow.urls.Add(url_4.Text, name_4.Text);
            }
            if (url_5.Text != "" && name_5.Text != "")
            {
                MainWindow.urls.Add(url_5.Text, name_5.Text);
            }
            if (url_6.Text != "" && name_6.Text != "")
            {
                MainWindow.urls.Add(url_6.Text, name_6.Text);
            }
            if (url_7.Text != "" && name_7.Text != "")
            {
                MainWindow.urls.Add(url_7.Text, name_7.Text);
            }
            if (url_8.Text != "" && name_8.Text != "")
            {
                MainWindow.urls.Add(url_8.Text, name_8.Text);
            }
            MainWindow.urls_num = MainWindow.urls.Count;
            // If urls.Count > 0
            if (MainWindow.urls_num > 0)
            {
                foreach (var d in MainWindow.urls)
                {
                    Guid guid = Guid.NewGuid();
                    String my_id = guid.ToString();
                    // Save Data To Database
                    using (SqlConnection connection = new SqlConnection(MainWindow.DB_connection_string))
                    {
                        String query = $"INSERT INTO dbo.MyCameras (id,urls,Name) VALUES (@{my_id},@{d.Key},@{d.Value})";
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
    }
}
