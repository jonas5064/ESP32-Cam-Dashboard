﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;


namespace IPCamera
{
    public partial class Settings : Window
    {

        List<Users> users;

        public Settings()
        {
            InitializeComponent();

            this.Update_settings_page();

            this.FillUsers();

            // Fill the TextBoxes With the Data
            sms_account_sid.Text = MainWindow.twilioAccountSID;
            sms_account_token.Text = MainWindow.twilioAccountToken;
            sms_account_phone.Text = MainWindow.twilioNumber;
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
            }
            // Save Email Sender And Password
            if (email_send_textbox.Text.Equals("") && pass_send_textbox.Text.Equals(""))
            {
                if ( (!email_send_textbox.Text.Equals(MainWindow.email_send)) ||
                        (!pass_send_textbox.Text.Equals(MainWindow.pass_send)))
                {
                    // Delete From Table The Last
                    SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                    String query = $"DELETE FROM dbo.EmailSender";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cn.Open();
                    int result = cmd.ExecuteNonQuery();
                    if (result < 0)
                        System.Windows.MessageBox.Show("Error inserting data into Database!");
                    cn.Close();
                    // Save Data To Database
                    using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
                    {
                        query = $"INSERT INTO dbo.EmailSender (Email,Pass) VALUES (@email,@pass)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@email", email_send_textbox.Text);
                            command.Parameters.AddWithValue("@pass", pass_send_textbox.Text);
                            connection.Open();
                            result = command.ExecuteNonQuery();
                            // Check Error
                            if (result < 0)
                                System.Windows.MessageBox.Show("Error inserting data into Database!");
                        }
                    }
                }
            }
            // Save SMS sid, token, phone
            if (!sms_account_sid.Text.Equals("") && 
                !sms_account_token.Text.Equals("") && 
                !sms_account_phone.Text.Equals(""))
            {
                // Delete From Table The Last
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"DELETE FROM dbo.SMS";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Save Data To Database
                using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
                {
                    query = $"INSERT INTO dbo.SMS (AccountSID,AccountTOKEN,Phone) VALUES (@sid,@token,@phone)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@sid", sms_account_sid.Text);
                        command.Parameters.AddWithValue("@token", sms_account_token.Text);
                        command.Parameters.AddWithValue("@phone", sms_account_phone.Text);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                }
            }
            // Ask to Restart The Application
            MessageBoxResult res = System.Windows.MessageBox.Show("Restart ?", "Question", (MessageBoxButton)MessageBoxButtons.OKCancel);
            if (res.ToString() == "OK")
            {
                // Close Settings Window
                this.Close();
                // Restart App Application
                MainWindow.RestartApp();
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
            // Update Email Sender And Pasword
            email_send_textbox.Text = MainWindow.email_send;
            pass_send_textbox.Text = MainWindow.pass_send;
            // Update Robotic . CameraSelector cameras
            camera_selector.Items.Add("Select a camera");
            camera_selector.SelectedIndex = camera_selector.Items.IndexOf("Select a camera");
            foreach (Camera cam in MainWindow.cameras)
            {
                camera_selector.Items.Add(cam.name);
            }
        }

        
        // Fill Users Table With Users
        public void FillUsers()
        {
            // Save list with users before
            users = new List<Users>(MainWindow.myUsers);
            // Add the List To DataGrid
            users_grid.ItemsSource = MainWindow.myUsers;
            // Make Id Column No Editable
            users_grid.AutoGeneratingColumn += (object sender, DataGridAutoGeneratingColumnEventArgs e) =>
            {
                if (e.Column.Header.ToString() == "Id")
                {
                    e.Column.IsReadOnly = true; // Makes the column as read only
                    e.Column.Width = 33;
                }
                if (e.Column.Header.ToString() == "Email")
                {
                    e.Column.Width = 333;
                }
            };
            users_grid.CanUserDeleteRows = true;
        }

        // Users Apply Button
        private void U_Apply_Click(object sender, RoutedEventArgs e)
        {
            // Commit Changes to the List with users
            users_grid.CommitEdit();
            // Chech If Delete a users
            if (users.Count > MainWindow.myUsers.Count)
            {
                Console.WriteLine("DELETE OK");
                foreach (Users u in users)
                {
                    if (!MainWindow.myUsers.Contains(u))
                    {
                        // Delete This User From DB
                        SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                        String query = $"DELETE FROM dbo.Users WHERE Id='{u.Id}'";
                        SqlCommand cmd = new SqlCommand(query, cn);
                        cn.Open();
                        int result = cmd.ExecuteNonQuery();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                    }
                }
            }
            else // Update Users On DB
            {
                int counter = 0;
                foreach (Users u in MainWindow.myUsers)
                {
                    Users old_user = users[counter];
                    // If A record changeds updated
                    if ((old_user.Firstname.Equals(u.Firstname)) || 
                            (old_user.Lastname.Equals(u.Lastname)) || 
                            (old_user.Email.Equals(u.Email)) || 
                            (old_user.Phone.Equals(u.Phone)))
                    {
                        Console.WriteLine("UPDATE OK");
                        //Console.WriteLine($"ID: {u.Id}  FName: {u.Firstname}  LName: {u.Lastname}  Email: {u.Email}  Phone: {u.Phone}");
                        // Update DataBase with this user
                        SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                        String query = $"UPDATE dbo.Users SET FirstName='{u.Firstname}', " +
                                                        $"LastName='{u.Lastname}', Email='{u.Email}', " +
                                                        $"Phone='{u.Phone}' WHERE Id='{u.Id}'";
                        SqlCommand cmd = new SqlCommand(query, cn);
                        cn.Open();
                        int result = cmd.ExecuteNonQuery();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                        counter++;
                    }
                }
            }
            // Refresch Users Table
            this.Close();
            new Settings().Show();
        }

        // Users Add Button
        private void U_Add_Click(object sender, RoutedEventArgs e)
        {
            String fname = FirstName.Text;
            String lname = LastName.Text;
            String email = Email.Text;
            String phone = Phone.Text;
            // Insert to DB First to create an Id and then update MainWindow.myUsers
            String query = $"INSERT INTO dbo.Users (FirstName, LastName, Email, Phone) VALUES (@fname, @lname, @email, @phone)";
            using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@fname", fname);
                    command.Parameters.AddWithValue("@lname", lname);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@phone", phone);
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    // Check Error
                    if (result < 0)
                        System.Windows.MessageBox.Show("Error inserting data into Database!");
                }
                connection.Close();
                // Get The New User User From DB And Add Him To MainWindow.myUsers
                query = $"SELECT Id, FirstName, LastName, Email, Phone FROM dbo.Users " +
                                            $"WHERE FirstName=@fname AND LastName=@lname AND Email=@email AND Phone=@phone ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@fname", fname);
                    command.Parameters.AddWithValue("@lname", lname);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@phone", phone);
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        int id = (int)dataReader["Id"];
                        String fname2 = dataReader["FirstName"].ToString().Trim();
                        String lname2 = dataReader["LastName"].ToString().Trim();
                        String email2 = dataReader["Email"].ToString().Trim();
                        String phone2 = dataReader["Phone"].ToString().Trim();
                        // Create The Usres Objects
                        Users user = new Users(id, fname2, lname2, email2, phone2);
                        MainWindow.myUsers.Add(user);
                    }
                }
                connection.Close();
            }
            Console.WriteLine("ADD OK");
            // Refresch Users Table
            this.Close();
            new Settings().Show();
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
                // Delete Data From DB
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"DELETE FROM dbo.FilesFormats";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Insert Data To DB
                query = $"INSERT INTO dbo.FilesFormats (avi, mp4, webm) VALUES (@avi, @mp4, @webm)";
                using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@avi", 1);
                        command.Parameters.AddWithValue("@mp4", 0);
                        command.Parameters.AddWithValue("@webm", 0);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    connection.Close();
                }
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
                // Delete Data From DB
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"DELETE FROM dbo.FilesFormats";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Insert Data To DB
                query = $"INSERT INTO dbo.FilesFormats (avi, mp4, webm) VALUES (@avi, @mp4, @webm)";
                using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 0);
                        command.Parameters.AddWithValue("@webm", 0);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    connection.Close();
                }
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
                // Delete Data From DB
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"DELETE FROM dbo.FilesFormats";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Insert Data To DB
                query = $"INSERT INTO dbo.FilesFormats (avi, mp4, webm) VALUES (@avi, @mp4, @webm)";
                using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 1);
                        command.Parameters.AddWithValue("@webm", 0);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    connection.Close();
                }
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
                // Delete Data From DB
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"DELETE FROM dbo.FilesFormats";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Insert Data To DB
                query = $"INSERT INTO dbo.FilesFormats (avi, mp4, webm) VALUES (@avi, @mp4, @webm)";
                using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 0);
                        command.Parameters.AddWithValue("@webm", 0);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    connection.Close();
                }
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
                // Delete Data From DB
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"DELETE FROM dbo.FilesFormats";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Insert Data To DB
                query = $"INSERT INTO dbo.FilesFormats (avi, mp4, webm) VALUES (@avi, @mp4, @webm)";
                using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 0);
                        command.Parameters.AddWithValue("@webm", 1);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    connection.Close();
                }
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
                // Delete Data From DB
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"DELETE FROM dbo.FilesFormats";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Insert Data To DB
                query = $"INSERT INTO dbo.FilesFormats (avi, mp4, webm) VALUES (@avi, @mp4, @webm)";
                using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 0);
                        command.Parameters.AddWithValue("@webm", 0);
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    connection.Close();
                }
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }

        // When select a tab
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // SMS Hyper Link Func
        private void Hyperlink_RequestNavigate(object sender,
                                       System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        // Whene Robotic Select Camera Combo Box change
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String cam_name = camera_selector.SelectedItem.ToString();
            if (!cam_name.Equals("Select a camera"))
            {
                Console.WriteLine($"Selected Camera: {cam_name}");
                foreach (Camera cam in MainWindow.cameras)
                {
                    if (cam.name.Equals(cam_name))
                    {
                        up_text.Text = cam.up_req;
                        down_text.Text = cam.down_req;
                        right_text.Text = cam.right_req;
                        left_text.Text = cam.left_req;
                    }
                }
            }
            else
            {
                Console.WriteLine("No Camera Selected");
                up_text.Text = "";
                down_text.Text = "";
                right_text.Text = "";
                left_text.Text = "";
            }
        }

        // Save the cameras remote controll settings
        private void Apply_get_req_Click(object sender, RoutedEventArgs e)
        {
            // Save UP, DOWN, RIGHT, LEFT Buttons
            if (CheckURL(up_text.Text) && CheckURL(down_text.Text) &&
                    CheckURL(right_text.Text) && CheckURL(left_text.Text))
            {
                String cam_name = camera_selector.SelectedItem.ToString();
                if (!cam_name.Equals("Select a camera"))
                {
                    Console.WriteLine("Update DATABASE");
                    // Update Data To Database
                    SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                    String query = "UPDATE dbo.myCameras SET Up_req=@up, Down_req=@down, Left_req=@left, Right_req=@right WHERE name=@cam_name";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@up", up_text.Text);
                    cmd.Parameters.AddWithValue("@down", down_text.Text);
                    cmd.Parameters.AddWithValue("@left", left_text.Text);
                    cmd.Parameters.AddWithValue("@right", right_text.Text);
                    cmd.Parameters.AddWithValue("@cam_name", cam_name);
                    cn.Open();
                    int result = cmd.ExecuteNonQuery();
                    cn.Close();
                    if (result < 0)
                        System.Windows.MessageBox.Show("Error inserting data into Database!");
                    else
                    {
                        // Ask to Restart The Application
                        MessageBoxResult res = System.Windows.MessageBox.Show("Restart ?", "Question", (MessageBoxButton)MessageBoxButtons.OKCancel);
                        if (res.ToString() == "OK")
                        {
                            // Close Settings Window
                            this.Close();
                            // Restart App Application
                            MainWindow.RestartApp();
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Select a camera.");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Setup cameras http/https urls");
            }
        }


        // Check if the texts is a valis urls
        private static bool CheckURL(String url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }
        


    }
}
