using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using VisioForge.Controls.UI.WPF;

namespace IPCamera
{
    public partial class MainWindow : Window
    {
        public static MainWindow Main_window { get; set; }
        public IpCamerasEntities DBModels = new IpCamerasEntities();
        public List<CameraServcies> camerasServicies { get; set; } = new List<CameraServcies>();
        public Login Login { get; set; }
        private bool _logged = false;
        public bool Logged
        {
            set
            {
                this._logged = value;
                if (!MainWindow.Main_window._logged)
                {
                    if (MainWindow.Main_window.DBModels.Users.Where(u => u.Logged).Any())
                    {
                        User oldLoggedUser = (from u in MainWindow.Main_window.DBModels.Users where u.Logged == true select u).FirstOrDefault();
                        oldLoggedUser.Logged = false;
                        MainWindow.Main_window.DBModels.SaveChanges();
                    } 
                }
            }
            get { return this._logged; }
        }
        private Settings _settings;
        public bool Settings_oppened { get; set; } = false;
        public bool Login_oppened { get; set; } = false;
        private Account _account;
        public bool Account_oppened { get; set; } = false;
        private Records _records;
        public bool Records_oppened { get; set; } = false;
        public MainWindow()
        {
            try
            {
                Main_window = this;
                // Runs only one time and install some requarements
                if (Install_Requarements.First_time_runs)
                {
                    Console.WriteLine("Inside First Time Run.");
                    // Install Requarements
                    Install_Requarements.Install_Req();
                    Install_Requarements.First_time_runs = false;
                    try
                    {
                        // Create Admin User
                        User user_a = new User();
                        user_a.FirstName = "admin";
                        user_a.LastName = "admin";
                        user_a.Email = "admin@admin.com";
                        user_a.Phone = "";
                        user_a.Licences = "Admin";
                        user_a.Password = "1234";
                        user_a.Logged = false;
                        User[] usersOld = (from c in Main_window.DBModels.Users
                                        where c.FirstName == user_a.FirstName
                                        where c.LastName == user_a.LastName
                                        where c.Email == user_a.Email
                                        where c.Phone == user_a.Phone
                                        where c.Licences == user_a.Licences
                                        where c.Password == user_a.Password
                                        where c.Logged == user_a.Logged
                                        select c).ToArray();
                        if(usersOld.Length == 0)
                        {
                            Main_window.DBModels.Users.Add(user_a);
                            MainWindow.Main_window.DBModels.SaveChanges();
                        }
                        // Files Format By Default
                        FilesFormat filef = new FilesFormat();
                        filef.Id = 1;
                        filef.avi = false;
                        filef.mp4 = true;
                        filef.history_time = 1;
                        filef.file_size = 3600000;
                        FilesFormat[] filesFormatOld = (from f in MainWindow.Main_window.DBModels.FilesFormats
                                                     where f.Id == filef.Id
                                                     where f.avi == filef.avi
                                                     where f.mp4 == filef.mp4
                                                     where f.history_time == filef.history_time
                                                     where f.file_size == filef.file_size
                                                     select f).ToArray();
                        if(filesFormatOld.Length > 0)
                        {
                            MainWindow.Main_window.DBModels.FilesFormats.Add(filef);
                            MainWindow.Main_window.DBModels.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Application Varaible to false this code won't runs again
                        Install_Requarements.First_time_runs = true;
                        Console.WriteLine($"\n\nSource:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}\n\n");
                        Thread.Sleep(5000);
                    }
                }
                #region "Initialize Main Window"
                try
                {
                    InitializeComponent();
                }
                catch (System.Windows.Markup.XamlParseException ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nLine:{ex.LineNumber}\n{ex.Message}");
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                    Thread.Sleep(5000);
                }
                #endregion
                // Set a Hundeler for this main window
                Main_window = this;
                // Setup Login/logout button for start
                login_logout_b.Click += (object sender, RoutedEventArgs e) =>
                {
                    this.Loggin_clicked();
                };
                login_logout_b.Click += (object send, RoutedEventArgs ev) =>
                {
                    MainWindow.Main_window.Loggout_clicked();
                };
                // If User Is Logged In
                if (MainWindow.Main_window.DBModels.Users.Where(u => u.Logged).Any())
                {
                    //User user = (from u in MainWindow.Main_window.DBModels.Users where u.Logged == true select u).FirstOrDefault();
                    MainWindow.Main_window.Logged = true;
                    MainWindow.Main_window.login_logout_b.Content = "Logout";

                }
                // Open he Cameras Windows
                CreateVideosPage();
                //  DispatcherTimer setup (Thread Excecutes date update every 1 second)
                date.Content = DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));
                System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
            }
            catch (System.IO.FileLoadException)
            {
                Console.WriteLine($"\n\n[ERROR] MainWindow\n\n");
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                Thread.Sleep(5000);
            }
            // If Cameras Recording Start Scheduling
            FilesFormat ff = (from f in Main_window.DBModels.FilesFormats select f).FirstOrDefault();
            System.Timers.Timer recording_Cicle = new System.Timers.Timer
            {
                Interval = double.Parse(ff.file_size.ToString())
            };
            recording_Cicle.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) =>
            {
                Dispatcher.Invoke((Action)delegate ()
               {
                   try
                   {
                       foreach (MyCamera cam in MainWindow.Main_window.DBModels.MyCameras)
                       {
                           if (cam.Recording)
                           {
                               CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == cam.Id select s).FirstOrDefault();
                               cs.StartRecording();
                               cs.StopRecording();
                           }
                       }
                   }
                   catch (System.InvalidOperationException ex)
                   {
                       Console.WriteLine($"\n\nInvalidOperationException:\nSource:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}\n\n");
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine($"\n\nException:\nSource:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}\n\n");
                   }
               });
            };
            recording_Cicle.AutoReset = true;
            recording_Cicle.Enabled = true;
            // Delete All Video Records Beforethe  " Video_recording_history_length " Start Scheduling
            System.Timers.Timer deleting_cicle = new System.Timers.Timer
            {
                Interval = 86400000 // Every 24 Hours
            };
            deleting_cicle.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) =>
            {
                Dispatcher.Invoke((Action)delegate ()
                {
                    try
                    {
                        this.DeleteOldFiles();
                    }
                    catch (System.InvalidOperationException ex)
                    {
                        Console.WriteLine($"\n\nInvalidOperationException:\nSource:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}\n\n");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n\nException:\nSource:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}\n\n");
                    }
                });
            };
            deleting_cicle.AutoReset = true;
            deleting_cicle.Enabled = true;
        }
        public void DeleteOldFiles()
        {
            string picturesDirPath = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                      where f.Name.Equals("Pictures")
                                      select f.Path).FirstOrDefault();
            string videoDirPath = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                   where f.Name.Equals("Videos")
                                   select f.Path).FirstOrDefault();
            // Delete Pictures
            this.GetDirsSubDirsFiles(picturesDirPath, myPath =>
            {
                // Get The Months Folder And If Month is Smaller From MainWindow.Video_recording_history_length Delete them
                FileInfo info = new FileInfo(myPath);
                String name = info.FullName;
                String[] dirs = name.Split('\\');
                String date_string = dirs[dirs.Length - 2].Trim();
                DateTime date = DateTime.ParseExact(date_string, "dd-MM-yyyy", null);
                FilesFormat filesFormats = (from f in MainWindow.Main_window.DBModels.FilesFormats select f).FirstOrDefault();
                if (date.Month < DateTime.Today.AddMonths(-(filesFormats.history_time)).Month)
                {
                    int last_dir_index = myPath.LastIndexOf('\\');
                    String folder = myPath.Substring(0, last_dir_index);
                    System.IO.DirectoryInfo di = new DirectoryInfo(folder);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    di.Delete(true);
                }
            });
            // Delete Videos
            this.GetDirsSubDirsFiles(videoDirPath, myPath =>
            {
                // Get The Months Folder And If Month is Smaller From MainWindow.Video_recording_history_length Delete them
                FileInfo info = new FileInfo(myPath);
                String name = info.FullName;
                String[] dirs = name.Split('\\');
                String date_string = dirs[dirs.Length - 2].Trim();
                DateTime date = DateTime.ParseExact(date_string, "dd-MM-yyyy", null);
                Console.WriteLine($"Folder Month: {date.Month}  Current Month: {DateTime.Today.Month}");
                FilesFormat filesFormats = (from f in MainWindow.Main_window.DBModels.FilesFormats select f).FirstOrDefault();
                if (date.Month < DateTime.Today.AddMonths(-(filesFormats.history_time)).Month)
                {
                    int last_dir_index = myPath.LastIndexOf('\\');
                    String folder = myPath.Substring(0, last_dir_index);
                    System.IO.DirectoryInfo di = new DirectoryInfo(folder);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    di.Delete(true);
                }
            });
        }
        public delegate void InsideDirsFunction(String path);
        private void GetDirsSubDirsFiles(String path, InsideDirsFunction func)
        {
            if ( File.Exists(path) ) // Is File
            {
                func(path);
            }
            else if ( Directory.Exists(path) )  // Is Dir
            {
                string[] sub_dirss = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                foreach (string f_path in sub_dirss)
                {
                    this.GetDirsSubDirsFiles(f_path, func);
                }
            }
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current time 
            date.Content = DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));
        }
        public static void RestartApp()
        {
            MainWindow old_win = Main_window;
            System.Windows.Forms.Application.Restart();
            old_win.Close();
        }
        public void Loggin_clicked()
        {
            if (Login_oppened == false)
            {
                Login_oppened = true;
                this.Login = new Login();
                this.Login.Show();
            }
            else
            {
                this.Login.Activate();
            }
        }
        public void Loggout_clicked()
        {
            MainWindow.Main_window.Logged = false;
            MainWindow.Main_window.login_logout_b.Content = "Login";
            login_logout_b.Click += (object sender, RoutedEventArgs e) =>
            {
                this.Loggin_clicked();
            };
        }
        private void Start_clicked(object sender, RoutedEventArgs e)
        {
            if(Logged)
            {
                foreach (MyCamera cam in MainWindow.Main_window.DBModels.MyCameras)
                {
                    CameraServcies cs = Main_window.camerasServicies
                        .Where(cs1 => cs1.cameraId == cam.Id)
                        .Select(cs2 => cs2).FirstOrDefault();
                    cs.Start();
                }
            }
        }
        private void Stop_clicked(object sender, RoutedEventArgs e)
        {
            if (Logged)
            {
                foreach (CameraServcies cs in MainWindow.Main_window.camerasServicies)
                {
                    cs.Stop();
                }
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            foreach (MyCamera cam in MainWindow.Main_window.DBModels.MyCameras)
            {
                foreach (CameraServcies cs in MainWindow.Main_window.camerasServicies)
                {
                    cs.Stop();
                }
            }
            this.Close();
        }
        private void Settings_clicked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Main_window.DBModels.Users.Where(u => u.Logged && u.Licences.Contains("Admin")).Any())
            {
                if (Settings_oppened == false)
                {
                    Settings_oppened = true;
                    this._settings = new Settings();
                    this._settings.Show();
                }
                else
                {
                    this._settings.Activate();
                }
            }
        }
        private void Account_clicked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Main_window.Logged)
            {
                if(Account_oppened == false)
                {
                    User user = (from u in MainWindow.Main_window.DBModels.Users where u.Logged == true select u).FirstOrDefault();
                    Account_oppened = true;
                    this._account = new Account(user);
                    this._account.Show();
                }
                else
                {
                    this._account.Activate();
                }
            }
        }
        private void Records_clicked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Main_window.Logged)
            {
                if (Records_oppened == false)
                {
                    Records_oppened = true;
                    this._records = new Records();
                    this._records.Show();
                }
                else
                {
                    this._records.Activate();
                }
            }
        }
        private void X_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Logged)
            {
                this.Close();
            }
        }
        public void CreateVideosPage()
        {
            // Dynamic add columns and rows
            int count_rows = 0;
            int count_columns = 0;
            foreach (MyCamera cam in MainWindow.Main_window.DBModels.MyCameras)
            {
                CameraServcies cs = new CameraServcies(cam);
                MainWindow.Main_window.camerasServicies.Add(cs);
                cam.VideoIndex = MainWindow.Main_window.camerasServicies.Count-1;
                // New Row
                if (count_columns == 3)
                {
                    cameras_grid.RowDefinitions.Add(new RowDefinition());
                    count_rows++;
                    Grid.SetColumn(MainWindow.Main_window.camerasServicies[(int)cam.VideoIndex].video, count_columns);
                    cam.Coll = count_columns;
                    Grid.SetRow(MainWindow.Main_window.camerasServicies[(int)cam.VideoIndex].video, count_rows);
                    cam.Row = count_rows;
                    cameras_grid.Children.Add(MainWindow.Main_window.camerasServicies[(int)cam.VideoIndex].video);
                    count_columns = 0;
                }
                else // New Column
                {
                    Grid.SetColumn(MainWindow.Main_window.camerasServicies[(int)cam.VideoIndex].video, count_columns);
                    cam.Coll = count_columns;
                    Grid.SetRow(MainWindow.Main_window.camerasServicies[(int)cam.VideoIndex].video, count_rows);
                    cam.Row = count_rows;
                    cameras_grid.Children.Add(MainWindow.Main_window.camerasServicies[(int)cam.VideoIndex].video);
                    cameras_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    count_columns++;
                }
            }  
        }
    }

}
