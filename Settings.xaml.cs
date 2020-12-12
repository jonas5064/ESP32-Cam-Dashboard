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
using System.IO;

namespace IPCamera
{
    public partial class Settings : Window

    {

        private String pictures_dir;
        private String videos_dir;

        public Settings()
        {
            InitializeComponent();

            update_settings_page();
        }




        private void button_pictures_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result.ToString().Equals("OK"))
                {
                    if(dialog.SelectedPath != "")
                    {
                        this.pictures_dir = dialog.SelectedPath;
                        txtEditor_pictures.Text = this.pictures_dir;
                    }
                }
            }
        }



        private void button_videos_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result.ToString().Equals("OK"))
                {
                    if (dialog.SelectedPath != "")
                    {
                        this.videos_dir = dialog.SelectedPath;
                        txtEditor_videos.Text = this.videos_dir;
                    }
                }
            }
        }




        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<String, String> urls = new Dictionary<String, String>();
            // Setup a list with the urls and the number of them.
            if (url_1.Text != "" && name_1.Text != "")
            {
                try
                {
                    urls.Add(url_1.Text, name_1.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_2.Text != "" && name_2.Text != "")
            {
                try
                {
                    urls.Add(url_2.Text, name_2.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_3.Text != "" && name_3.Text != "")
            {
                try
                {
                    urls.Add(url_3.Text, name_3.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_4.Text != "" && name_4.Text != "")
            {
                try
                {
                    urls.Add(url_4.Text, name_4.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_5.Text != "" && name_5.Text != "")
            {
                try
                {
                    urls.Add(url_5.Text, name_5.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_6.Text != "" && name_6.Text != "")
            {
                try
                {
                    urls.Add(url_6.Text, name_6.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_7.Text != "" && name_7.Text != "")
            {
                try
                {
                    urls.Add(url_7.Text, name_7.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            if (url_8.Text != "" && name_8.Text != "")
            {
                try
                {
                    urls.Add(url_8.Text, name_8.Text);
                }
                catch (System.ArgumentException)
                {

                }
            }
            int urls_num = urls.Count;
            // If urls.Count > 0
            if (urls_num > 0)
            {
                // Clear Database
                SqlConnection con = new SqlConnection(Camera.DB_connection_string);
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "DELETE FROM dbo.MyCameras ",
                    Connection = con
                };
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                foreach (var d in urls)
                {
                    Guid guid = Guid.NewGuid();
                    String my_id = guid.ToString();
                    // Save Data To Database
                    using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
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
                // Ask to Restart The Application
                MessageBoxResult res = System.Windows.MessageBox.Show("Restart ?", "Question", (MessageBoxButton)MessageBoxButtons.OKCancel);
                if ( res.ToString() == "OK" )
                {
                    // Close Settings Window
                    this.Close();
                    // Restart App Application
                    MainWindow.RestartApp();
                }
            }
        }



        private void update_settings_page()
        {
            // Feel the page with the current data
            if (Camera.count > 0)
            {
                if (MainWindow.cameras[0].url != "" && MainWindow.cameras[0].name != "")
                {
                    url_1.Text = MainWindow.cameras[0].url;
                    name_1.Text = MainWindow.cameras[0].name;
                }
            }
            if (Camera.count > 1)
            {
                if (MainWindow.cameras[1].url != "" && MainWindow.cameras[1].name != "")
                {
                    url_2.Text = MainWindow.cameras[1].url;
                    name_2.Text = MainWindow.cameras[1].name;
                }
            }
            if (Camera.count > 2)
            {
                if (MainWindow.cameras[2].url != "" && MainWindow.cameras[2].name != "")
                {
                    url_3.Text = MainWindow.cameras[2].url;
                    name_3.Text = MainWindow.cameras[2].name;
                }
            }
            if (Camera.count > 3)
            {
                if (MainWindow.cameras[3].url != "" && MainWindow.cameras[3].name != "")
                {
                    url_4.Text = MainWindow.cameras[3].url;
                    name_4.Text = MainWindow.cameras[3].name;
                }
            }
            if (Camera.count > 4)
            {
                if (MainWindow.cameras[4].url != "" && MainWindow.cameras[4].name != "")
                {
                    url_5.Text = MainWindow.cameras[4].url;
                    name_5.Text = MainWindow.cameras[4].name;
                }
            }
            if (Camera.count > 5)
            {
                if (MainWindow.cameras[5].url != "" && MainWindow.cameras[5].name != "")
                {
                    url_6.Text = MainWindow.cameras[5].url;
                    name_6.Text = MainWindow.cameras[5].name;
                }
            }
            if (Camera.count > 6)
            {
                if (MainWindow.cameras[6].url != "" && MainWindow.cameras[6].name != "")
                {
                    url_7.Text = MainWindow.cameras[6].url;
                    name_7.Text = MainWindow.cameras[6].name;
                }
            }
            if (Camera.count > 7)
            {
                if (MainWindow.cameras[7].url != "" && MainWindow.cameras[7].name != "")
                {
                    url_8.Text = MainWindow.cameras[7].url;
                    name_8.Text = MainWindow.cameras[7].name;
                }
            }
        }

    }
}
