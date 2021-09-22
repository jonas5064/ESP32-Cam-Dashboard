using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Linq;

namespace IPCamera
{
    public partial class Settings : Window
    {
        List<User> _users;
        public List<User> Users
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
        SM sms;
        EmailSender emailSender;
        string picturesDirPath = "";
        string videoDirPath = "";
        public Settings()
        {

            InitializeComponent();

            this.sms = (from s in MainWindow.Main_window.DBModels.SMS select s).FirstOrDefault();
            this.emailSender = (from es in MainWindow.Main_window.DBModels.EmailSenders select es).FirstOrDefault();
            this.picturesDirPath = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                      where f.Name.Equals("Pictures")
                                      select f.Path).FirstOrDefault();
            this.videoDirPath = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                   where f.Name.Equals("Videos")
                                   select f.Path).FirstOrDefault();

            this.Update_settings_page();

            // Fill The Users in The Users DataGrid
            this.FillUsers();

            // Fill the TextBoxes With the Data
            sms_account_ssid.Text = this.sms.AccountSID;
            sms_account_token.Text = this.sms.AccountTOKEN;
            sms_account_phone.Text = this.sms.Phone;
        }
        // Progress Bar Event Method
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
        protected override void OnClosed(EventArgs e)
        {
            MainWindow.Main_window.Settings_oppened = false;
            Console.WriteLine("Settings_oppened: " + Convert.ToString(MainWindow.Main_window.Settings_oppened));
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
                        FilesDir fdp = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                        where f.Name.Equals("Pictures")
                                        select f).FirstOrDefault();
                        fdp.Path = dialog.SelectedPath;
                        MainWindow.Main_window.DBModels.SaveChanges();
                        txtEditor_pictures.Text = fdp.Path;
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
                    if (dialog.SelectedPath.Length > 0)
                    {
                        FilesDir fdd = (from f in MainWindow.Main_window.DBModels.FilesDirs where f.Name.Equals("Videos") select f).FirstOrDefault();
                        fdd.Path = dialog.SelectedPath;
                        MainWindow.Main_window.DBModels.SaveChanges();
                        txtEditor_videos.Text = fdd.Path;
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
            // Save Paths
            if (txtEditor_pictures.Text != "" && txtEditor_videos.Text != "")
            {
                foreach(var f in MainWindow.Main_window.DBModels.FilesDirs)
                {
                    MainWindow.Main_window.DBModels.FilesDirs.Remove(f);
                }
                // Save to DataBase Pictures Path
                FilesDir fdp = new FilesDir();
                fdp.Id = 1;
                fdp.Name = "Pictures";
                fdp.Path = txtEditor_pictures.Text;
                MainWindow.Main_window.DBModels.FilesDirs.Add(fdp);
                // Save to DataBase Video Path
                FilesDir fdv = new FilesDir();
                fdv.Id = 1;
                fdv.Name = "Videos";
                fdv.Path = txtEditor_videos.Text;
                MainWindow.Main_window.DBModels.FilesDirs.Add(fdv);
                MainWindow.Main_window.DBModels.SaveChanges();
            }
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(40) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(40) });
            // Save URLS
            try
            {
                MyCamera cam = new MyCamera();
                if (url_1.Text.Length > 0 && name_1.Text.Length > 0 &&
                    name_1.Text.Length > 0 && password_1.Password.Length > 0)
                {
                    cam.urls = url_1.Text;
                    cam.name = name_1.Text;
                    cam.username = username_1.Text;
                    cam.password = password_1.Password;
                    cam.fps = int.Parse(fps_1.Text);
                    cam.isEsp32 = camera1_esp32.IsChecked.Value;
                    MainWindow.Main_window.DBModels.MyCameras.Add(cam);
                    MainWindow.Main_window.DBModels.SaveChanges();
                }
                if (url_2.Text.Length > 0 && name_2.Text.Length > 0 &&
                    name_2.Text.Length > 0 && password_2.Password.Length > 0)
                {
                    cam.urls = url_2.Text;
                    cam.name = name_2.Text;
                    cam.username = username_2.Text;
                    cam.password = password_2.Password;
                    cam.fps = int.Parse(fps_2.Text);
                    cam.isEsp32 = camera2_esp32.IsChecked.Value;
                    MainWindow.Main_window.DBModels.MyCameras.Add(cam);
                    MainWindow.Main_window.DBModels.SaveChanges();
                }
                if (url_3.Text.Length > 0 && name_3.Text.Length > 0 &&
                    name_3.Text.Length > 0 && password_3.Password.Length > 0)
                {
                    cam.urls = url_3.Text;
                    cam.name = name_3.Text;
                    cam.username = username_3.Text;
                    cam.password = password_3.Password;
                    cam.fps = int.Parse(fps_3.Text);
                    cam.isEsp32 = camera3_esp32.IsChecked.Value;
                    MainWindow.Main_window.DBModels.MyCameras.Add(cam);
                    MainWindow.Main_window.DBModels.SaveChanges();
                }
                if (url_4.Text.Length > 0 && name_4.Text.Length > 0 &&
                    name_4.Text.Length > 0 && password_4.Password.Length > 0)
                {
                    cam.urls = url_4.Text;
                    cam.name = name_4.Text;
                    cam.username = username_4.Text;
                    cam.password = password_4.Password;
                    cam.fps = int.Parse(fps_4.Text);
                    cam.isEsp32 = camera4_esp32.IsChecked.Value;
                    MainWindow.Main_window.DBModels.MyCameras.Add(cam);
                    MainWindow.Main_window.DBModels.SaveChanges();
                }
                if (url_5.Text.Length > 0 && name_5.Text.Length > 0 &&
                    name_5.Text.Length > 0 && password_5.Password.Length > 0)
                {
                    cam.urls = url_5.Text;
                    cam.name = name_5.Text;
                    cam.username = username_5.Text;
                    cam.password = password_5.Password;
                    cam.fps = int.Parse(fps_5.Text);
                    cam.isEsp32 = camera5_esp32.IsChecked.Value;
                    MainWindow.Main_window.DBModels.MyCameras.Add(cam);
                    MainWindow.Main_window.DBModels.SaveChanges();
                }
                if (url_6.Text.Length > 0 && name_6.Text.Length > 0 &&
                    name_6.Text.Length > 0 && password_6.Password.Length > 0)
                {
                    cam.urls = url_6.Text;
                    cam.name = name_6.Text;
                    cam.username = username_6.Text;
                    cam.password = password_6.Password;
                    cam.fps = int.Parse(fps_6.Text);
                    cam.isEsp32 = camera6_esp32.IsChecked.Value;
                    MainWindow.Main_window.DBModels.MyCameras.Add(cam);
                    MainWindow.Main_window.DBModels.SaveChanges();
                }
                if (url_7.Text.Length > 0 && name_7.Text.Length > 0 &&
                    name_7.Text.Length > 0 && password_7.Password.Length > 0)
                {
                    cam.urls = url_7.Text;
                    cam.name = name_7.Text;
                    cam.username = username_7.Text;
                    cam.password = password_7.Password;
                    cam.fps = int.Parse(fps_7.Text);
                    cam.isEsp32 = camera7_esp32.IsChecked.Value;
                    MainWindow.Main_window.DBModels.MyCameras.Add(cam);
                    MainWindow.Main_window.DBModels.SaveChanges();
                }
                if (url_8.Text.Length > 0 && name_8.Text.Length > 0 &&
                    name_8.Text.Length > 0 && password_8.Password.Length > 0)
                {
                    cam.urls = url_8.Text;
                    cam.name = name_8.Text;
                    cam.username = username_8.Text;
                    cam.password = password_8.Password;
                    cam.fps = int.Parse(fps_8.Text);
                    cam.isEsp32 = camera8_esp32.IsChecked.Value;
                    MainWindow.Main_window.DBModels.MyCameras.Add(cam);
                    MainWindow.Main_window.DBModels.SaveChanges();
                }
            }
            catch (System.ArgumentException ex)
            {
                Console.WriteLine($"Source:{ex.Source}\nParamnAME:{ex.ParamName}\n{ex.Message}");
            }
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(60) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(60) });
            // Save Email Sender And Password
            if ((!email_send_textbox.Text.Equals(this.emailSender.Email)) ||
                    (!pass_send_textbox.Password.Equals(this.emailSender.Pass)))
            {
                var addr = new System.Net.Mail.MailAddress(email_send_textbox.Text);
                if (addr.Address == email_send_textbox.Text)
                {
                    foreach (var entity in MainWindow.Main_window.DBModels.EmailSenders)
                    {
                        MainWindow.Main_window.DBModels.EmailSenders.Remove(entity);
                    }
                    EmailSender es = new EmailSender();
                    es.Email = email_send_textbox.Text;
                    es.Pass = pass_send_textbox.Password;
                    MainWindow.Main_window.DBModels.EmailSenders.Add(es);
                    MainWindow.Main_window.DBModels.SaveChanges();
                }
                else
                {
                    if (!email_send_textbox.Text.Equals(""))
                    {
                        System.Windows.MessageBox.Show("Not Valid Email!");
                    }

                }
            }
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(80) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(80) });
            // Save SMS sid, token, phone
            if (!sms_account_ssid.Text.Equals(this.sms.AccountSID) ||
                !sms_account_token.Text.Equals(this.sms.AccountTOKEN) ||
                !sms_account_phone.Text.Equals(this.sms.Phone))
            {
                foreach(var s in MainWindow.Main_window.DBModels.SMS)
                {
                    MainWindow.Main_window.DBModels.SMS.Remove(s);
                }
                SM sms = new SM();
                sms.Id = 1;
                sms.AccountSID = sms_account_ssid.Text;
                sms.AccountTOKEN = sms_account_token.Text;
                sms.Phone = sms_account_phone.Text;
                MainWindow.Main_window.DBModels.SMS.Add(sms);
                MainWindow.Main_window.DBModels.SaveChanges();
            }
            // Update ProgressBar
            Dispatcher.Invoke(updateProgressBaDelegate, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(100) });
            Dispatcher.Invoke(updateProgressBaDelegateTow, DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, Convert.ToDouble(100) });
        }
        private void Update_settings_page()
        {
            string picturesDirPath = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                      where f.Name.Equals("Pictures")
                                      select f.Path).FirstOrDefault();
            string videoDirPath = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                   where f.Name.Equals("Videos")
                                   select f.Path).FirstOrDefault();
            FilesFormat fileFormat = (from f in MainWindow.Main_window.DBModels.FilesFormats select f).FirstOrDefault();
            // Update files paths
            txtEditor_pictures.Text = picturesDirPath;
            txtEditor_videos.Text = videoDirPath;
            // Update Files Formats
            avi_checkbox.IsChecked = fileFormat.avi;
            mp4_checkbox.IsChecked = fileFormat.mp4;
            // Update Recording History Time
            FilesFormat filesFormats = (from f in MainWindow.Main_window.DBModels.FilesFormats select f).FirstOrDefault();
            recordingTime_ComboBox.SelectedIndex = filesFormats.history_time-1;
            // Feel the urls
            List<MyCamera> cameras = (from camera in MainWindow.Main_window.DBModels.MyCameras select camera).ToList();
            if (cameras.Count > 0)
            {
                if (cameras[0].urls != "" && cameras[0].name != "")
                {
                    url_1.Text = cameras[0].urls;
                    name_1.Text = cameras[0].name;
                    username_1.Text = cameras[0].username;
                    password_1.Password = cameras[0].password;
                    fps_1.Text = cameras[0].fps.ToString();
                    camera1_esp32.IsChecked = cameras[0].isEsp32;
                }
            }
            if (cameras.Count > 1)
            {
                if (cameras[1].urls != "" && cameras[1].name != "")
                {
                    url_2.Text = cameras[1].urls;
                    name_2.Text = cameras[1].name;
                    username_2.Text = cameras[1].username;
                    password_2.Password = cameras[1].password;
                    fps_2.Text = cameras[1].fps.ToString();
                    camera2_esp32.IsChecked = cameras[1].isEsp32;
                }
            }
            if (cameras.Count > 2)
            {
                if (cameras[2].urls != "" && cameras[2].name != "")
                {
                    url_3.Text = cameras[2].urls;
                    name_3.Text = cameras[2].name;
                    username_3.Text = cameras[2].username;
                    password_3.Password = cameras[2].password;
                    fps_3.Text = cameras[2].fps.ToString();
                    camera3_esp32.IsChecked = cameras[2].isEsp32;
                }
            }
            if (cameras.Count > 3)
            {
                if (cameras[3].urls != "" && cameras[3].name != "")
                {
                    url_4.Text = cameras[3].urls;
                    name_4.Text = cameras[3].name;
                    username_4.Text = cameras[3].username;
                    password_4.Password = cameras[3].password;
                    fps_4.Text = cameras[3].fps.ToString();
                    camera4_esp32.IsChecked = cameras[3].isEsp32;
                }
            }
            if (cameras.Count > 4)
            {
                if (cameras[4].urls != "" && cameras[4].name != "")
                {
                    url_5.Text = cameras[4].urls;
                    name_5.Text = cameras[4].name;
                    username_5.Text = cameras[4].username;
                    password_5.Password = cameras[4].password;
                    fps_5.Text = cameras[4].fps.ToString();
                    camera5_esp32.IsChecked = cameras[4].isEsp32;
                }
            }
            if (cameras.Count > 5)
            {
                if (cameras[5].urls != "" && cameras[5].name != "")
                {
                    url_6.Text = cameras[5].urls;
                    name_6.Text = cameras[5].name;
                    username_6.Text = cameras[5].username;
                    password_6.Password = cameras[5].password;
                    fps_6.Text = cameras[5].fps.ToString();
                    camera6_esp32.IsChecked = cameras[5].isEsp32;
                }
            }
            if (cameras.Count > 6)
            {
                if (cameras[6].urls != "" && cameras[6].name != "")
                {
                    url_7.Text = cameras[6].urls;
                    name_7.Text = cameras[6].name;
                    username_7.Text = cameras[6].username;
                    password_7.Password = cameras[6].password;
                    fps_7.Text = cameras[6].fps.ToString();
                    camera7_esp32.IsChecked = cameras[6].isEsp32;
                }
            }
            if (cameras.Count > 7)
            {
                if (cameras[7].urls != "" && cameras[7].name != "")
                {
                    url_8.Text = cameras[7].urls;
                    name_8.Text = cameras[7].name;
                    username_8.Text = cameras[7].username;
                    password_8.Password = cameras[7].password;
                    fps_8.Text = cameras[7].fps.ToString();
                    camera8_esp32.IsChecked = cameras[7].isEsp32;
                }
            }
            // Update Email Sender And Pasword
            email_send_textbox.Text = emailSender.Email;
            pass_send_textbox.Password = emailSender.Pass;
            // Update Robotic . CameraSelector cameras
            camera_selector.Items.Add("Select a camera");
            camera_selector.SelectedIndex = camera_selector.Items.IndexOf("Select a camera");
            foreach (MyCamera cam in cameras)
            {
                camera_selector.Items.Add(cam.name);
            }
        }
        // Fill Users Table With Users
        public void FillUsers()
        {
            // Save list with users before
            List<User> users = (from u in MainWindow.Main_window.DBModels.Users select u).ToList();
            this.Users = new List<User>(users);
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
            MainWindow.Main_window.DBModels.SaveChanges();
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
                User user = new User();
                user.FirstName = FirstName.Text;
                user.LastName = LastName.Text;
                user.Email = Email.Text;
                user.Phone = Phone.Text;
                user.Password = Password.Password;
                String selection = new_user_licenses.SelectedValue.ToString();
                if (selection.Contains("Admin"))
                {
                    selection = "Admin";
                }
                if (selection.Contains("Employee"))
                {
                    selection = "Employee";
                }
                user.Licences = selection;
                String repeat_pass = Repeat_Pass.Password;
                if (user.Password.Equals(repeat_pass))
                {
                    MainWindow.Main_window.DBModels.Users.Add(user);
                    MainWindow.Main_window.DBModels.SaveChanges();
                    // Refresch Users Table
                    this.Close();
                    new Settings().Show();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Rong Password.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Settings.cs Line[588]\nError: {ex.Message}");
            }
        }
        // Files format checkboxes
        private void AVI_chencked(object sender, EventArgs e)
        {
            FilesFormat ff = (from f in MainWindow.Main_window.DBModels.FilesFormats select f).FirstOrDefault();
            ff.avi = true;
            MainWindow.Main_window.DBModels.SaveChanges();
        }
        private void AVI_unchencked(object sender, EventArgs e)
        {
            FilesFormat ff = (from f in MainWindow.Main_window.DBModels.FilesFormats select f).FirstOrDefault();
            ff.avi = false;
            MainWindow.Main_window.DBModels.SaveChanges();
        }
        private void MP4_chencked(object sender, EventArgs e)
        {
            FilesFormat ff = (from f in MainWindow.Main_window.DBModels.FilesFormats select f).FirstOrDefault();
            ff.mp4 = true;
            MainWindow.Main_window.DBModels.SaveChanges();
        }
        private void MP4_unchencked(object sender, EventArgs e)
        {
            FilesFormat ff = (from f in MainWindow.Main_window.DBModels.FilesFormats select f).FirstOrDefault();
            ff.mp4 = false;
            MainWindow.Main_window.DBModels.SaveChanges();
        }
        // ComboBox Selected Recording Time Changed
        private void ComboBox_Selected_Recording_Time_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem typeItem = (ComboBoxItem)recordingTime_ComboBox.SelectedItem;
                if (typeItem.Content != null)
                {
                    FilesFormat filesFormats = (from f in MainWindow.Main_window.DBModels.FilesFormats select f).FirstOrDefault();
                    string value = typeItem.Content.ToString();
                    switch (value)
                    {
                        case "1 Month":
                            filesFormats.history_time = 1;
                            break;
                        case "2 Month":
                            filesFormats.history_time = 2;
                            break;
                        case "3 Month":
                            filesFormats.history_time = 3;
                            break;
                        case "4 Month":
                            filesFormats.history_time = 4;
                            break;
                        case "5 Month":
                            filesFormats.history_time = 5;
                            break;
                        case "6 Month":
                            filesFormats.history_time = 6;
                            break;
                        case "7 Month":
                            filesFormats.history_time = 7;
                            break;
                        case "8 Month":
                            filesFormats.history_time = 8;
                            break;
                        case "9 Month":
                            filesFormats.history_time = 9;
                            break;
                        case "10 Month":
                            filesFormats.history_time = 10;
                            break;
                        case "11 Month":
                            filesFormats.history_time = 11;
                            break;
                        case "12 Month":
                            filesFormats.history_time = 12;
                            break;
                    }
                    MainWindow.Main_window.DBModels.SaveChanges();
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
                foreach (MyCamera cam in MainWindow.Main_window.DBModels.MyCameras)
                {
                    if (cam.name.Equals(cam_name))
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
                    MyCamera camera = (from cam in MainWindow.Main_window.DBModels.MyCameras where cam.name == cam_name select cam).FirstOrDefault();
                    camera.Up_req = up_text.Text;
                    camera.Down_req = down_text.Text;
                    camera.Left_req = left_text.Text;
                    camera.Right_req = right_text.Text;
                    MainWindow.Main_window.DBModels.SaveChanges();
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
}
