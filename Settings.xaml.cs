using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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

            update_settings_page();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Creal Urls List
            MainWindow.urls.Clear();
            // Setup a list with the urls and the number of them.
            if (url_1.Text != "" && name_1.Text != "")
            {
                try
                {
                    MainWindow.urls.Add(url_1.Text, name_1.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_2.Text != "" && name_2.Text != "")
            {
                try
                {
                    MainWindow.urls.Add(url_2.Text, name_2.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_3.Text != "" && name_3.Text != "")
            {
                try
                {
                    MainWindow.urls.Add(url_3.Text, name_3.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_4.Text != "" && name_4.Text != "")
            {
                try
                {
                    MainWindow.urls.Add(url_4.Text, name_4.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_5.Text != "" && name_5.Text != "")
            {
                try
                {
                    MainWindow.urls.Add(url_5.Text, name_5.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_6.Text != "" && name_6.Text != "")
            {
                try
                {
                    MainWindow.urls.Add(url_6.Text, name_6.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_7.Text != "" && name_7.Text != "")
            {
                try
                {
                    MainWindow.urls.Add(url_7.Text, name_7.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_8.Text != "" && name_8.Text != "")
            {
                try
                {
                    MainWindow.urls.Add(url_8.Text, name_8.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            MainWindow.urls_num = MainWindow.urls.Count;
            // If urls.Count > 0
            if (MainWindow.urls_num > 0)
            {
                // Clear Database
                SqlConnection con = new SqlConnection(MainWindow.DB_connection_string);
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "DELETE FROM dbo.MyCameras ",
                    Connection = con
                };
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                foreach (var d in MainWindow.urls)
                {
                    Guid guid = Guid.NewGuid();
                    String my_id = guid.ToString();
                    // Save Data To Database
                    using (SqlConnection connection = new SqlConnection(MainWindow.DB_connection_string))
                    {
                        String query = $"INSERT INTO dbo.MyCameras (id,urls,Name) VALUES (@id,@urls,@name)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", my_id);
                            command.Parameters.AddWithValue("@urls", d.Key);
                            command.Parameters.AddWithValue("@name", d.Value);
                            connection.Open();
                            int result = command.ExecuteNonQuery();
                            // Check Error
                            if (result < 0)
                                System.Windows.MessageBox.Show("Error inserting data into Database!");
                        }
                    }
                }
                // Close Settings Window
                this.Close();

                // Restart App Application
                MainWindow.RestartApp();

            }
        }



        private void update_settings_page()
        {
            // Feel the page with the current data
            var urls_list = MainWindow.urls.Keys.ToList();
            var names_list = MainWindow.urls.Values.ToList();
            if (MainWindow.urls_num > 0)
            {
                if (urls_list[0] != "" && names_list[0] != "")
                {
                    url_1.Text = urls_list[0];
                    name_1.Text = names_list[0];
                }
            }
            if (MainWindow.urls_num > 1)
            {
                if (urls_list[1] != "" && names_list[1] != "")
                {
                    url_2.Text = urls_list[1];
                    name_2.Text = names_list[1];
                }
            }
            if (MainWindow.urls_num > 2)
            {
                if (urls_list[2] != "" && names_list[2] != "")
                {
                    url_3.Text = urls_list[2];
                    name_3.Text = names_list[2];
                }
            }
            if (MainWindow.urls_num > 3)
            {
                if (urls_list[3] != "" && names_list[3] != "")
                {
                    url_4.Text = urls_list[3];
                    name_4.Text = names_list[3];
                }
            }
            if (MainWindow.urls_num > 4)
            {
                if (urls_list[4] != "" && names_list[4] != "")
                {
                    url_5.Text = urls_list[4];
                    name_5.Text = names_list[4];
                }
            }
            if (MainWindow.urls_num > 5)
            {
                if (urls_list[5] != "" && names_list[5] != "")
                {
                    url_6.Text = urls_list[5];
                    name_6.Text = names_list[5];
                }
            }
            if (MainWindow.urls_num > 6)
            {
                if (urls_list[6] != "" && names_list[6] != "")
                {
                    url_7.Text = urls_list[6];
                    name_7.Text = names_list[6];
                }
            }
            if (MainWindow.urls_num > 7)
            {
                if (urls_list[7] != "" && names_list[7] != "")
                {
                    url_8.Text = urls_list[7];
                    name_8.Text = names_list[7];
                }
            }
        }

    }
}
