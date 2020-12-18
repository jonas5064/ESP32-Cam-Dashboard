using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Forms;


namespace IPCamera
{
    public partial class Settings : Window

    {

        public Settings()
        {
            InitializeComponent();

            Update_settings_page();
        }




        private void Button_pictures_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result.ToString().Equals("OK"))
                {
                    if(dialog.SelectedPath != "")
                    {
                        Camera.pictures_dir = dialog.SelectedPath;
                        txtEditor_pictures.Text = Camera.pictures_dir;
                    }
                }
            }
        }



        private void Button_videos_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result.ToString().Equals("OK"))
                {
                    if (dialog.SelectedPath != "")
                    {
                        Camera.videos_dir = dialog.SelectedPath;
                        txtEditor_videos.Text = Camera.videos_dir;
                    }
                }
            }
        }




        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            // Save Paths
            if (txtEditor_pictures.Text != "" && txtEditor_videos.Text != "")
            {
                // Clear DataBase
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = "DELETE FROM dbo.FilesDirs";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Save to DataBase Pictures
                using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
                {
                    query = $"INSERT INTO dbo.FilesDirs (id, Name, Path) VALUES (@id,@name,@path)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", 1);
                        command.Parameters.AddWithValue("@name", "Pictures");
                        command.Parameters.AddWithValue("@path", txtEditor_pictures.Text);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                }
                // Save to DataBase Videos
                using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
                {
                    query = $"INSERT INTO dbo.FilesDirs (id, Name, Path) VALUES (@id,@name,@path)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", 2);
                        command.Parameters.AddWithValue("@name", "Videos");
                        command.Parameters.AddWithValue("@path", txtEditor_videos.Text);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                }
            }

            // Save URLS
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



        private void Update_settings_page()
        {
            // Feel files paths
            txtEditor_pictures.Text = Camera.pictures_dir;
            txtEditor_videos.Text = Camera.videos_dir;
            // Saved Files Formats
            avi_checkbox.IsChecked = Camera.avi_format;
            mp4_checkbox.IsChecked = Camera.mp4_format;
            webm_checkbox.IsChecked = Camera.webm_format;
            // Feel the urls
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




        // Files format checkboxes
        private void AVI_chencked(object sender, EventArgs e)
        {
            Camera.avi_format = true;
            Camera.mp4_format = false;
            Camera.webm_format = false;
            mp4_checkbox.IsChecked = false;
            webm_checkbox.IsChecked = false;
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.FilesFormats SET avi='{1}', mp4='{0}', webm='{0}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }
        private void AVI_unchencked(object sender, EventArgs e)
        {
            Camera.avi_format = false;
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.FilesFormats SET avi='{0}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }

        private void MP4_chencked(object sender, EventArgs e)
        {
            Camera.mp4_format = true;
            Camera.avi_format = false;
            Camera.webm_format = false;
            avi_checkbox.IsChecked = false;
            webm_checkbox.IsChecked = false;
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.FilesFormats SET mp4='{1}', avi='{0}', webm='{0}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }
        private void MP4_unchencked(object sender, EventArgs e)
        {
            Camera.mp4_format = false;
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.FilesFormats SET mp4='{0}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }

        private void WEBM_chencked(object sender, EventArgs e)
        {
            Camera.webm_format = true;
            Camera.mp4_format = false;
            Camera.avi_format = false;
            mp4_checkbox.IsChecked = false;
            avi_checkbox.IsChecked = false;
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.FilesFormats SET webm='{1}', avi='{0}', mp4='{0}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }
        private void WEBM_unchencked(object sender, EventArgs e)
        {
            Camera.webm_format = false;
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.FilesFormats SET webm='{0}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }



    }
}
