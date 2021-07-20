using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Threading;

namespace IPCamera
{
    public partial class Settings : Window
    {
        List<Users> _users;
        public List<Users> Users
        {
            get
            {
                return this._users;
            }
            set
            {
                this._users = value;
            }
        }


        public Settings()
        {

            InitializeComponent();

            this.Update_settings_page();

            // Fill The Users in The Users DataGrid
            this.FillUsers();

            // Fill the TextBoxes With the Data
            sms_account_ssid.Text = MainWindow.TwilioAccountSID;
            sms_account_token.Text = MainWindow.TwilioAccountToken;
            sms_account_phone.Text = MainWindow.TwilioNumber;
        }

        // Progress Bar Event Method
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);

        protected override void OnClosed(EventArgs e)
        {
            MainWindow.Settings_oppened = false;
            Console.WriteLine("Settings_oppened: " + Convert.ToString(MainWindow.Settings_oppened));
            this.Close();
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
                        Camera.Pictures_dir = dialog.SelectedPath;
                        txtEditor_pictures.Text = Camera.Pictures_dir;
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
                        Camera.Videos_dir = dialog.SelectedPath;
                        txtEditor_videos.Text = Camera.Videos_dir;
                    }
                }
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            progressBarPageOne.Visibility = Visibility.Visible;
            progressBarTracking.Visibility = Visibility.Visible;
            this.ApplyFirstPageSettings();
            progressBarPageOne.Visibility = Visibility.Hidden;
            progressBarTracking.Visibility = Visibility.Hidden;
        }

        private async void ApplyFirstPageSettings()
        {
            // ProgressBar Object
            UpdateProgressBarDelegate updateProgressBaDelegate = new UpdateProgressBarDelegate(progressBarPageOne.SetValue);
            UpdateProgressBarDelegate updateProgressBaDelegateTow = new UpdateProgressBarDelegate(progressBarTracking.SetValue);
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(20) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(20) });

            using (SqlConnection cn = new SqlConnection(App.DB_connection_string))
            {
                String query;
                int result;
                // Save Paths
                if (txtEditor_pictures.Text != "" && txtEditor_videos.Text != "")
                {
                    // Clear DataBase
                    query = "DELETE FROM FilesDirs";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        result = await cmd.ExecuteNonQueryAsync();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                    }
                        // Save to DataBase Pictures
                        query = $"INSERT INTO FilesDirs (id, Name, Path) VALUES (@id,@name,@path)";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@id", 1);
                        cmd.Parameters.AddWithValue("@name", "Pictures");
                        cmd.Parameters.AddWithValue("@path", txtEditor_pictures.Text);
                        cn.Open();
                        result = await cmd.ExecuteNonQueryAsync();
                        // Check Error
                        if (result < 0)
                        {
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        }
                    }
                }
                cn.Close();
                query = $"INSERT INTO FilesDirs (id, Name, Path) VALUES (@id,@name,@path)";
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@id", 2);
                    cmd.Parameters.AddWithValue("@name", "Videos");
                    cmd.Parameters.AddWithValue("@path", txtEditor_videos.Text);
                    cn.Open();
                    result = await cmd.ExecuteNonQueryAsync();
                    // Check Error
                    if (result < 0)
                    {
                        System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    cn.Close();
                }

                // Update ProgressBar
                Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(40) });
                Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(40) });

                // Save URLS
                List<Cameras> cams = new List<Cameras>(8);
                try
                {
                    if (url_1.Text.Length > 0 && name_1.Text.Length > 0 &&
                        name_1.Text.Length > 0 && password_1.Password.Length > 0)
                    {
                        cams.Add(new Cameras(url_1.Text, name_1.Text, username_1.Text, password_1.Password, fps_1.Text, camera1_esp32.IsChecked.Value));
                    }
                    if (url_2.Text.Length > 0 && name_2.Text.Length > 0 &&
                        name_2.Text.Length > 0 && password_2.Password.Length > 0)
                    {
                        cams.Add(new Cameras(url_2.Text, name_2.Text, username_2.Text, password_2.Password, fps_2.Text, camera2_esp32.IsChecked.Value));
                    }
                    if (url_3.Text.Length > 0 && name_3.Text.Length > 0 &&
                        name_3.Text.Length > 0 && password_3.Password.Length > 0)
                    {
                        cams.Add(new Cameras(url_3.Text, name_3.Text, username_3.Text, password_3.Password, fps_3.Text, camera3_esp32.IsChecked.Value));
                    }
                    if (url_4.Text.Length > 0 && name_4.Text.Length > 0 &&
                        name_4.Text.Length > 0 && password_4.Password.Length > 0)
                    {
                        cams.Add(new Cameras(url_4.Text, name_4.Text, username_4.Text, password_4.Password, fps_4.Text, camera4_esp32.IsChecked.Value));
                    }
                    if (url_5.Text.Length > 0 && name_5.Text.Length > 0 &&
                        name_5.Text.Length > 0 && password_5.Password.Length > 0)
                    {
                        cams.Add(new Cameras(url_5.Text, name_5.Text, username_5.Text, password_5.Password, fps_5.Text, camera5_esp32.IsChecked.Value));
                    }
                    if (url_6.Text.Length > 0 && name_6.Text.Length > 0 &&
                        name_6.Text.Length > 0 && password_6.Password.Length > 0)
                    {
                        cams.Add(new Cameras(url_6.Text, name_6.Text, username_6.Text, password_6.Password, fps_6.Text, camera6_esp32.IsChecked.Value));
                    }
                    if (url_7.Text.Length > 0 && name_7.Text.Length > 0 &&
                        name_7.Text.Length > 0 && password_7.Password.Length > 0)
                    {
                        cams.Add(new Cameras(url_7.Text, name_7.Text, username_7.Text, password_7.Password, fps_7.Text, camera7_esp32.IsChecked.Value));
                    }
                    if (url_8.Text.Length > 0 && name_8.Text.Length > 0 &&
                        name_8.Text.Length > 0 && password_8.Password.Length > 0)
                    {
                        cams.Add(new Cameras(url_8.Text, name_8.Text, username_8.Text, password_8.Password, fps_8.Text, camera8_esp32.IsChecked.Value));
                    }
                }
                catch (System.ArgumentException ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nParamnAME:{ex.ParamName}\n{ex.Message}");
                }

                // Update ProgressBar
                Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(60) });
                Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(60) });

                int urls_num = cams.Count;
                // If urls.Count > 0
                if (urls_num > 0)
                {
                    // Clear Database
                    query = "DELETE FROM MyCameras";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        await cmd.ExecuteNonQueryAsync();
                        cn.Close();
                    }
                    foreach (Cameras d in cams)
                    {
                        Guid guid = Guid.NewGuid();
                        String my_id = guid.ToString();
                        // Save Data To Database
                        query = $"INSERT INTO MyCameras (id,urls,name,username,password,fps,isEsp32 ) VALUES (@id,@urls,@name,@username,@password,@fps,@isESP)";
                        using (SqlCommand cmd = new SqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@id", my_id);
                            cmd.Parameters.AddWithValue("@urls", d.url);
                            cmd.Parameters.AddWithValue("@name", d.name);
                            cmd.Parameters.AddWithValue("@username", d.username);
                            cmd.Parameters.AddWithValue("@password", d.password);
                            cmd.Parameters.AddWithValue("@fps", d.fps);
                            cmd.Parameters.AddWithValue("@isESP", d.isEsp32);
                            cn.Open();
                            result = await cmd.ExecuteNonQueryAsync();
                            // Check Error
                            if (result < 0)
                            {
                                System.Windows.MessageBox.Show("Error inserting data into Database!");
                            }
                            cn.Close();
                        }
                    }
                }
                else
                {
                    // Clear Database
                    // Clear Database
                    query = "DELETE FROM MyCameras";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        await cmd.ExecuteNonQueryAsync();
                        cn.Close();
                    }
                }

                // Update ProgressBar
                Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(80) });
                Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(80) });

                // Save Email Sender And Password
                if ((!email_send_textbox.Text.Equals(MainWindow.Email_send)) ||
                        (!pass_send_textbox.Password.Equals(MainWindow.Pass_send)))
                {
                    Console.WriteLine(pass_send_textbox.Password);
                    // If email is an valid email
                    try
                    {
                        var addr = new System.Net.Mail.MailAddress(email_send_textbox.Text);
                        if (addr.Address == email_send_textbox.Text)
                        {
                            // Delete From Table The Last
                            query = $"DELETE FROM EmailSender";
                            using (SqlCommand cmd = new SqlCommand(query, cn))
                            {
                                cn.Open();
                                result = await cmd.ExecuteNonQueryAsync();
                                if (result < 0)
                                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                                cn.Close();
                            }
                            // Save Data To Database
                            query = $"INSERT INTO EmailSender (Email,Pass) VALUES (@email,@pass)";
                            using (SqlCommand cmd = new SqlCommand(query, cn))
                            {
                                cmd.Parameters.AddWithValue("@email", email_send_textbox.Text);
                                cmd.Parameters.AddWithValue("@pass", pass_send_textbox.Password);
                                cn.Open();
                                result = await cmd.ExecuteNonQueryAsync();
                                // Check Error
                                if (result < 0)
                                {
                                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                                }
                            }
                        }
                        else
                        {
                            if (!email_send_textbox.Text.Equals(""))
                            {
                                System.Windows.MessageBox.Show("Not Valid Email!");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        if (!email_send_textbox.Text.Equals(""))
                        {
                            System.Windows.MessageBox.Show("Not Valid Email!");
                        }
                        else
                        {
                            Console.WriteLine($"Source:{ex.Source}\n\n{ex.Message}");
                        }
                    }
                }

                // Update ProgressBar
                Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(100) });
                Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(100) });

                // Save SMS sid, token, phone
                if (!sms_account_ssid.Text.Equals(MainWindow.TwilioAccountSID) ||
                    !sms_account_token.Text.Equals(MainWindow.TwilioAccountToken) ||
                    !sms_account_phone.Text.Equals(MainWindow.TwilioNumber))
                {
                    // Delete From Table The Last
                    query = $"DELETE FROM SMS";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        result = await cmd.ExecuteNonQueryAsync();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                    }
                    // Save Data To Database
                    query = $"INSERT INTO SMS (AccountSID,AccountTOKEN,Phone) VALUES (@sid,@token,@phone)";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@sid", sms_account_ssid.Text);
                        cmd.Parameters.AddWithValue("@token", sms_account_token.Text);
                        cmd.Parameters.AddWithValue("@phone", sms_account_phone.Text);
                        cn.Open();
                        result = await cmd.ExecuteNonQueryAsync();
                        // Check Error
                        if (result < 0)
                        {
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        }
                    }
                }

                // Update ProgressBar
                Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(120) });
                Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(120) });

                // Update History Files Length
                query = $"UPDATE FilesFormats SET history_time='{MainWindow.Video_recording_history_length}'";
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cn.Open();
                    result = await cmd.ExecuteNonQueryAsync();
                    if (result < 0)
                    {
                        System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    cn.Close();
                }

                // Update ProgressBar
                Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(140) });
                Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(140) });
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
            // Update files paths
            txtEditor_pictures.Text = Camera.Pictures_dir;
            txtEditor_videos.Text = Camera.Videos_dir;
            // Update Files Formats
            avi_checkbox.IsChecked = Camera.Avi_format;
            mp4_checkbox.IsChecked = Camera.Mp4_format;
            // Update Recording History Time
            recordingTime_ComboBox.SelectedIndex = MainWindow.Video_recording_history_length-1;

            // Feel the urls
            if (Camera.Count > 0)
            {
                if (MainWindow.Cameras[0].Url != "" && MainWindow.Cameras[0].Name != "")
                {
                    url_1.Text = MainWindow.Cameras[0].Url;
                    name_1.Text = MainWindow.Cameras[0].Name;
                    username_1.Text = MainWindow.Cameras[0].Username;
                    password_1.Password = MainWindow.Cameras[0].Password;
                    fps_1.Text = MainWindow.Cameras[0].Framerate.ToString();
                    camera1_esp32.IsChecked = MainWindow.Cameras[0].IsEsp32;
                }
            }
            if (Camera.Count > 1)
            {
                if (MainWindow.Cameras[1].Url != "" && MainWindow.Cameras[1].Name != "")
                {
                    url_2.Text = MainWindow.Cameras[1].Url;
                    name_2.Text = MainWindow.Cameras[1].Name;
                    username_2.Text = MainWindow.Cameras[1].Username;
                    password_2.Password = MainWindow.Cameras[1].Password;
                    fps_2.Text = MainWindow.Cameras[1].Framerate.ToString();
                    camera2_esp32.IsChecked = MainWindow.Cameras[1].IsEsp32;
                }
            }
            if (Camera.Count > 2)
            {
                if (MainWindow.Cameras[2].Url != "" && MainWindow.Cameras[2].Name != "")
                {
                    url_3.Text = MainWindow.Cameras[2].Url;
                    name_3.Text = MainWindow.Cameras[2].Name;
                    username_3.Text = MainWindow.Cameras[2].Username;
                    password_3.Password = MainWindow.Cameras[2].Password;
                    fps_3.Text = MainWindow.Cameras[2].Framerate.ToString();
                    camera3_esp32.IsChecked = MainWindow.Cameras[2].IsEsp32;
                }
            }
            if (Camera.Count > 3)
            {
                if (MainWindow.Cameras[3].Url != "" && MainWindow.Cameras[3].Name != "")
                {
                    url_4.Text = MainWindow.Cameras[3].Url;
                    name_4.Text = MainWindow.Cameras[3].Name;
                    username_4.Text = MainWindow.Cameras[3].Username;
                    password_4.Password = MainWindow.Cameras[3].Password;
                    fps_4.Text = MainWindow.Cameras[3].Framerate.ToString();
                    camera4_esp32.IsChecked = MainWindow.Cameras[3].IsEsp32;
                }
            }
            if (Camera.Count > 4)
            {
                if (MainWindow.Cameras[4].Url != "" && MainWindow.Cameras[4].Name != "")
                {
                    url_5.Text = MainWindow.Cameras[4].Url;
                    name_5.Text = MainWindow.Cameras[4].Name;
                    username_5.Text = MainWindow.Cameras[4].Username;
                    password_5.Password = MainWindow.Cameras[4].Password;
                    fps_5.Text = MainWindow.Cameras[4].Framerate.ToString();
                    camera5_esp32.IsChecked = MainWindow.Cameras[4].IsEsp32;
                }
            }
            if (Camera.Count > 5)
            {
                if (MainWindow.Cameras[5].Url != "" && MainWindow.Cameras[5].Name != "")
                {
                    url_6.Text = MainWindow.Cameras[5].Url;
                    name_6.Text = MainWindow.Cameras[5].Name;
                    username_6.Text = MainWindow.Cameras[5].Username;
                    password_6.Password = MainWindow.Cameras[5].Password;
                    fps_6.Text = MainWindow.Cameras[5].Framerate.ToString();
                    camera6_esp32.IsChecked = MainWindow.Cameras[5].IsEsp32;
                }
            }
            if (Camera.Count > 6)
            {
                if (MainWindow.Cameras[6].Url != "" && MainWindow.Cameras[6].Name != "")
                {
                    url_7.Text = MainWindow.Cameras[6].Url;
                    name_7.Text = MainWindow.Cameras[6].Name;
                    username_7.Text = MainWindow.Cameras[6].Username;
                    password_7.Password = MainWindow.Cameras[6].Password;
                    fps_7.Text = MainWindow.Cameras[6].Framerate.ToString();
                    camera7_esp32.IsChecked = MainWindow.Cameras[6].IsEsp32;
                }
            }
            if (Camera.Count > 7)
            {
                if (MainWindow.Cameras[7].Url != "" && MainWindow.Cameras[7].Name != "")
                {
                    url_8.Text = MainWindow.Cameras[7].Url;
                    name_8.Text = MainWindow.Cameras[7].Name;
                    username_8.Text = MainWindow.Cameras[7].Username;
                    password_8.Password = MainWindow.Cameras[7].Password;
                    fps_8.Text = MainWindow.Cameras[7].Framerate.ToString();
                    camera8_esp32.IsChecked = MainWindow.Cameras[7].IsEsp32;
                }
            }
            // Update Email Sender And Pasword
            email_send_textbox.Text = MainWindow.Email_send;
            pass_send_textbox.Password = MainWindow.Pass_send;
            // Update Robotic . CameraSelector cameras
            camera_selector.Items.Add("Select a camera");
            camera_selector.SelectedIndex = camera_selector.Items.IndexOf("Select a camera");
            foreach (Camera cam in MainWindow.Cameras)
            {
                camera_selector.Items.Add(cam.Name);
            }
        }


        // Fill Users Table With Users
        public void FillUsers()
        {
            // Save list with users before
            this.Users = new List<Users>(MainWindow.MyUsers);
            // Add the List To DataGrid
            users_grid.ItemsSource = this.Users;
            // Make Id Column No Editable
            users_grid.AutoGeneratingColumn += (object sender, DataGridAutoGeneratingColumnEventArgs e) =>
            {
                if (e.Column.Header.ToString() == "Id")
                {
                    e.Column.IsReadOnly = true; // Makes the column as read only
                    e.Column.Width = 33;
                }
                if (e.Column.Header.ToString() == "FirstName")
                {
                    e.Column.MinWidth = 111;
                }
                if (e.Column.Header.ToString() == "LastName")
                {
                    e.Column.MinWidth = 111;
                }
                if (e.Column.Header.ToString() == "Email")
                {
                    e.Column.MinWidth = 311;
                }
                if (e.Column.Header.ToString() == "Password")
                {
                    e.Column.MinWidth = 200;
                    e.Cancel = true;
                }
                if (e.Column.Header.ToString() == "Licences")
                {
                    e.Column.IsReadOnly = true; // Makes the column as read only
                }
            };
        }

        // Users Apply Button
        private void U_Apply_Click(object sender, RoutedEventArgs e)
        {
            progressBarPageUsers.Visibility = Visibility.Visible;
            this.UserApply();
            progressBarPageUsers.Visibility = Visibility.Hidden;
        }

        private async void UserApply()
        {
            // ProgressBar Object
            UpdateProgressBarDelegate updateProgressBaDelegate = new UpdateProgressBarDelegate(progressBarPageUsers.SetValue);
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(70) });

            // Commit Changes to the List with this._users
            users_grid.CommitEdit();
            // Chech If Delete a users
            if (this._users.Count > MainWindow.MyUsers.Count)
            {
                Console.WriteLine("DELETE OK");
                foreach (Users u in this._users)
                {
                    if (!MainWindow.MyUsers.Contains(u))
                    {
                        // Delete This User From DB
                        using (SqlConnection cn = new SqlConnection(App.DB_connection_string))
                        {
                            String query = $"DELETE FROM Users WHERE Id='{u.Id}'";
                            using (SqlCommand cmd = new SqlCommand(query, cn))
                            {
                                cn.Open();
                                int result = await cmd.ExecuteNonQueryAsync();
                                if (result < 0)
                                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                                cn.Close();
                            }
                        }
                    }
                }
            }
            else // Update Users On DB
            {
                int counter = 0;
                foreach (Users u in MainWindow.MyUsers)
                {
                    Users old_user = this._users[counter];
                    // If A record changeds updated
                    if ((old_user.Firstname.Equals(u.Firstname)) ||
                            (old_user.Lastname.Equals(u.Lastname)) ||
                            (old_user.Email.Equals(u.Email)) ||
                            (old_user.Phone.Equals(u.Phone)))
                    {
                        Console.WriteLine("UPDATE OK");
                        //Console.WriteLine($"ID: {u.Id}  FName: {u.Firstname}  LName: {u.Lastname}  Email: {u.Email}  Phone: {u.Phone}");
                        // Update DataBase with this user
                        using (SqlConnection cn = new SqlConnection(App.DB_connection_string))
                        {
                            String query = $"UPDATE Users SET FirstName='{u.Firstname}', " +
                                                            $"LastName='{u.Lastname}', Email='{u.Email}', " +
                                                            $"Phone='{u.Phone}' WHERE Id='{u.Id}'";
                            using (SqlCommand cmd = new SqlCommand(query, cn))
                            {
                                cn.Open();
                                int result = await cmd.ExecuteNonQueryAsync();
                                if (result < 0)
                                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                                cn.Close();
                            }
                        }
                        counter++;
                    }
                }
            }
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(110) });
            Thread.Sleep(1000);
            // Refresch Users Table
            this.Close();
            new Settings().Show();
        }

        // Users Add Button
        private void U_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String fname = FirstName.Text;
                String lname = LastName.Text;
                String email = Email.Text;
                String phone = Phone.Text;
                String selection = new_user_licenses.SelectedValue.ToString();
                if (selection.Contains("Admin"))
                {
                    selection = "Admin";
                }
                else if (selection.Contains("Employee"))
                {
                    selection = "Employee";
                }
                String password = Password.Password;
                String repeat_pass = Repeat_Pass.Password;
                if (password.Equals(repeat_pass))
                {
                    // Insert to DB First to create an Id and then update MainWindow.MyUsers
                    String query = $"INSERT INTO Users (FirstName, LastName, Email, Phone, Licences, Password)" +
                                                            $" VALUES (@fname, @lname, @email, @phone, @licences, @pass)";
                    using (SqlConnection connection = new SqlConnection(App.DB_connection_string))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@fname", fname);
                            command.Parameters.AddWithValue("@lname", lname);
                            command.Parameters.AddWithValue("@email", email);
                            command.Parameters.AddWithValue("@phone", phone);
                            command.Parameters.AddWithValue("@licences", selection);
                            command.Parameters.AddWithValue("@pass", password);
                            connection.Open();
                            int result = command.ExecuteNonQuery();
                            // Check Error
                            if (result < 0)
                                System.Windows.MessageBox.Show("Error inserting data into Database!");
                            connection.Close();
                        }
                        //connection.Close();
                        // Get The New User User From DB And Add Him To MainWindow.MyUsers
                        query = $"SELECT Id, FirstName, LastName, Email, Phone, Licences, Password FROM Users " +
                                                    $"WHERE FirstName=@fname AND LastName=@lname AND Email=@email AND Phone=@phone AND Password=@pass";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@fname", fname);
                            command.Parameters.AddWithValue("@lname", lname);
                            command.Parameters.AddWithValue("@email", email);
                            command.Parameters.AddWithValue("@phone", phone);
                            command.Parameters.AddWithValue("@pass", password);
                            connection.Open();
                            SqlDataReader dataReader = command.ExecuteReader();
                            while (dataReader.Read())
                            {
                                int id = (int)dataReader["Id"];
                                String fname2 = dataReader["FirstName"].ToString().Trim();
                                String lname2 = dataReader["LastName"].ToString().Trim();
                                String email2 = dataReader["Email"].ToString().Trim();
                                String phone2 = dataReader["Phone"].ToString().Trim();
                                String licences = dataReader["Licences"].ToString().Trim();
                                String pass = dataReader["Password"].ToString().Trim();
                                // Create The Usres Objects
                                Users user = new Users(id, fname2, lname2, email2, phone2, licences, pass);
                                MainWindow.MyUsers.Add(user);
                            }
                        }
                        connection.Close();
                    }
                    Console.WriteLine("ADD OK");
                    // Refresch Users Table
                    this.Close();
                    new Settings().Show();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Rong Password.");
                }
            }
            catch(MySqlException ex)
            {
                Console.WriteLine($"Source: {ex.Message}");
                if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
                {
                    System.Windows.MessageBox.Show("This User Exists");
                }
            }
        }

        // Files format checkboxes
        private void AVI_chencked(object sender, EventArgs e)
        {
            Camera.Avi_format = true;
            Camera.Mp4_format = false;
            mp4_checkbox.IsChecked = false;
            try
            {
                // Delete Data From DB
                using (SqlConnection cn = new SqlConnection(App.DB_connection_string))
                {
                    String query = $"DELETE FROM FilesFormats";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        int result = cmd.ExecuteNonQuery();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                    }
                    // Insert Data To DB
                    query = $"INSERT INTO FilesFormats (avi, mp4) VALUES (@avi, @mp4)";
                    using (SqlCommand command = new SqlCommand(query, cn))
                    {
                        command.Parameters.AddWithValue("@avi", 1);
                        command.Parameters.AddWithValue("@mp4", 0);
                        cn.Open();
                        int result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    cn.Close();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Source: {ex.Message}");
            }
        }
        private void AVI_unchencked(object sender, EventArgs e)
        {
            Camera.Avi_format = false;
            try
            {
                // Delete Data From DB
                using (SqlConnection cn = new SqlConnection(App.DB_connection_string))
                {
                    String query = $"DELETE FROM FilesFormats";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        int result = cmd.ExecuteNonQuery();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                    }
                    // Insert Data To DB
                    query = $"INSERT INTO FilesFormats (avi, mp4) VALUES (@avi, @mp4)";
                    using (SqlCommand command = new SqlCommand(query, cn))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 0);
                        cn.Open();
                        int result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    cn.Close();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Source: {ex.Message}");
            }
        }

        private void MP4_chencked(object sender, EventArgs e)
        {
            Camera.Mp4_format = true;
            Camera.Avi_format = false;
            avi_checkbox.IsChecked = false;
            try
            {
                // Delete Data From DB
                using (SqlConnection cn = new SqlConnection(App.DB_connection_string))
                {
                    String query = $"DELETE FROM FilesFormats";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        int result = cmd.ExecuteNonQuery();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                    }
                    // Insert Data To DB
                    query = $"INSERT INTO FilesFormats (avi, mp4) VALUES (@avi, @mp4)";
                    using (SqlCommand command = new SqlCommand(query, cn))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 1);
                        cn.Open();
                        int result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    cn.Close();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Source: {ex.Message}");
            }
        }
        private void MP4_unchencked(object sender, EventArgs e)
        {
            Camera.Mp4_format = false;
            try
            {
                // Delete Data From DB
                using (SqlConnection cn = new SqlConnection(App.DB_connection_string))
                {
                    String query = $"DELETE FROM FilesFormats";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        int result = cmd.ExecuteNonQuery();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                    } 
                    // Insert Data To DB
                    query = $"INSERT INTO FilesFormats (avi, mp4) VALUES (@avi, @mp4)";
                
                        using (SqlCommand command = new SqlCommand(query, cn))
                    {
                        command.Parameters.AddWithValue("@avi", 0);
                        command.Parameters.AddWithValue("@mp4", 0);
                        cn.Open();
                        int result = command.ExecuteNonQuery();
                        // Check Error
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                    }
                    cn.Close();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Source: {ex.Message}");
            }
        }


        // ComboBox Selected Recording Time Changed
        private void ComboBox_Selected_Recording_Time_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem typeItem = (ComboBoxItem)recordingTime_ComboBox.SelectedItem;
                if (typeItem.Content != null)
                {
                    string value = typeItem.Content.ToString();
                    switch (value)
                    {
                        case "1 Month":
                            MainWindow.Video_recording_history_length = 1;
                            Console.WriteLine("1 Month");
                            break;
                        case "2 Month":
                            MainWindow.Video_recording_history_length = 2;
                            Console.WriteLine("2 Month");
                            break;
                        case "3 Month":
                            MainWindow.Video_recording_history_length = 3;
                            Console.WriteLine("3 Month");
                            break;
                        case "4 Month":
                            MainWindow.Video_recording_history_length = 4;
                            Console.WriteLine("4 Month");
                            break;
                        case "5 Month":
                            MainWindow.Video_recording_history_length = 5;
                            Console.WriteLine("5 Month");
                            break;
                        case "6 Month":
                            MainWindow.Video_recording_history_length = 6;
                            Console.WriteLine("6 Month");
                            break;
                        case "7 Month":
                            MainWindow.Video_recording_history_length = 7;
                            Console.WriteLine("7 Month");
                            break;
                        case "8 Month":
                            MainWindow.Video_recording_history_length = 8;
                            Console.WriteLine("8 Month");
                            break;
                        case "9 Month":
                            MainWindow.Video_recording_history_length = 9;
                            Console.WriteLine("9 Month");
                            break;
                        case "10 Month":
                            MainWindow.Video_recording_history_length = 10;
                            Console.WriteLine("10 Month");
                            break;
                        case "11 Month":
                            MainWindow.Video_recording_history_length = 11;
                            Console.WriteLine("11 Month");
                            break;
                        case "12 Month":
                            MainWindow.Video_recording_history_length = 12;
                            Console.WriteLine("12 Month");
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"\n\n\nException: {ex.Message}");
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
                foreach (Camera cam in MainWindow.Cameras)
                {
                    if (cam.Name.Equals(cam_name))
                    {
                        up_text.Text = cam.Up_req;
                        down_text.Text = cam.Down_req;
                        right_text.Text = cam.Right_req;
                        left_text.Text = cam.Left_req;
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
            progressBarPageRobotic.Visibility = Visibility.Visible;
            this.ApplyRobotics();
            progressBarPageRobotic.Visibility = Visibility.Hidden;
        }

        private async void ApplyRobotics()
        {
            // ProgressBar Object
            UpdateProgressBarDelegate updateProgressBaDelegate = new UpdateProgressBarDelegate(progressBarPageRobotic.SetValue);
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(50) });

            // Save UP, DOWN, RIGHT, LEFT Buttons
            if (CheckURL(up_text.Text) && CheckURL(down_text.Text) &&
                    CheckURL(right_text.Text) && CheckURL(left_text.Text))
            {
                String cam_name = camera_selector.SelectedItem.ToString();
                if (!cam_name.Equals("Select a camera"))
                {
                    Console.WriteLine("Update DATABASE");
                    // Update Data To Database
                    using (SqlConnection cn = new SqlConnection(App.DB_connection_string))
                    {
                        String query = "UPDATE MyCameras SET Up_req=@up, Down_req=@down, Left_req=@left, Right_req=@right WHERE name=@cam_name";
                        using (SqlCommand cmd = new SqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@up", up_text.Text);
                            cmd.Parameters.AddWithValue("@down", down_text.Text);
                            cmd.Parameters.AddWithValue("@left", left_text.Text);
                            cmd.Parameters.AddWithValue("@right", right_text.Text);
                            cmd.Parameters.AddWithValue("@cam_name", cam_name);
                            cn.Open();
                            int result = await cmd.ExecuteNonQueryAsync();
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
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Select a camera.");
                }
                // Update ProgressBar
                Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(110) });
            }
            else
            {
                System.Windows.MessageBox.Show("Setup cameras http/https urls");
            }
        }


        // Check if the texts is a valis urls
        private static bool CheckURL(String url)
        {
            bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

    }



    public class Cameras
    {
        public String url;
        public String name;
        public String username;
        public String password;
        public int fps;
        public bool isEsp32 = false;
        public Cameras(String u, String n, String un, String p, String fps, bool isEsp)
        {
            this.url = u;
            this.name = n;
            this.username = un;
            this.password = p;
            this.fps = Int16.Parse(fps);
            this.isEsp32 = isEsp;
        }
        ~Cameras()
        {

        }
    }

}
