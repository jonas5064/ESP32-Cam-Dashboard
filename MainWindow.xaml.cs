using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace IPCamera
{

    public partial class MainWindow : Window
    {

        public static MainWindow main_window;
        public static List<Camera> cameras = new List<Camera>();
        public static List<Users> myUsers = new List<Users>();
        public static Users user;
        public static Grid cams_grid;
        public static String email_send;
        public static String pass_send;
        public Login login;
        public static bool logged = false;
        public static int video_files_time_size = 3600000; // 1 Hour
        public static int video_recording_history_length = 1; // 1 Month

        public static bool Logged
        {
            set
            {
                MainWindow.logged = value;
                // Save the logged User
                if (MainWindow.logged)
                {
                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(App.DB_connection_string))
                        {
                            String query = $"INSERT INTO Logged (Id) VALUES (@user)";
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@user", MainWindow.user.Email);
                                connection.Open();
                                int result = command.ExecuteNonQuery();
                                // Check Error
                                if (result < 0)
                                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                            }
                        }
                    }
                    catch( Exception ex)
                    {
                        MessageBox.Show($"MainWindows.Logged = True Error: \n{ex.Message}");
                    }
                    
                }
                else
                {
                    // Clear DataBase
                    try
                    {
                        MySqlConnection cn = new MySqlConnection(App.DB_connection_string);
                        String query = "DELETE FROM Logged";
                        MySqlCommand cmd = new MySqlCommand(query, cn);
                        cn.Open();
                        int result = cmd.ExecuteNonQuery();
                        if (result < 0)
                            System.Windows.MessageBox.Show("Error inserting data into Database!");
                        cn.Close();
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"Source: {ex.Message}");
                    }
                }
            }
            get { return MainWindow.logged; }
        }
        private Settings settings;
        public static bool settings_oppened = false;
        public static bool login_oppened = false;
        private Account account;
        public static bool account_oppened = false;
        private Records records;
        public static bool records_oppened = false;
        public static String twilioNumber;
        public static String twilioAccountSID;
        public static String twilioAccountToken;


        public MainWindow()
        {
            try
            {
                Console.WriteLine("Staring Main...");
                // Runs only one time and install some requarements
                if (Install_Requarements.First_time_runs)
                {
                    Console.WriteLine("Inside First Time Run.");
                    try
                    {
                        // Install Requarements
                        Install_Requarements.Install_Req();
                        // Create an Admin User
                        try
                        {
                            String query = $"INSERT INTO Users (FirstName, LastName, Email, Phone, Licences, Password)" +
                                                                    $" VALUES (@fname, @lname, @email, @phone, @licences, @pass)";
                            using (MySqlConnection connection = new MySqlConnection(App.DB_connection_string))
                            {
                                using (MySqlCommand command = new MySqlCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@fname", "admin");
                                    command.Parameters.AddWithValue("@lname", "admin");
                                    command.Parameters.AddWithValue("@email", "admin@admin.com");
                                    command.Parameters.AddWithValue("@phone", "");
                                    command.Parameters.AddWithValue("@licences", "Admin");
                                    command.Parameters.AddWithValue("@pass", "1234");
                                    connection.Open();
                                    int result = command.ExecuteNonQuery();
                                    // Check Error
                                    if (result < 0)
                                    {
                                        Console.WriteLine("Error inserting Admin into Database!");
                                    }
                                }
                                connection.Close();
                            }
                        }
                        catch (MySqlException ex)
                        {
                            Console.WriteLine($"Source: {ex.Message}");
                        }
                        // Application Varaible to false this code won't runs again
                        Install_Requarements.First_time_runs = false;
                    }
                    catch (Exception ex)
                    {
                        // Application Varaible to false this code won't runs again
                        Install_Requarements.First_time_runs = true;
                        Console.WriteLine($"\n\nSource:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}\n\n");
                        Thread.Sleep(5000);
                    }
                }
                Console.WriteLine("Starting the base application...");

                /*
                // Create Database Connection String 
                string db_file_path = $"{Install_Requarements.GetRootDir()}\\Database1.mdf";
                App.DB_connection_string = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={db_file_path};Integrated Security=True";
                Console.WriteLine($"\n\nDB Dir: {db_file_path}\n\nDatabase Connation String: {App.DB_connection_string}\n\n");
                */

                // Initialize Main Window
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


                // Set a Hundeler for this main window
                main_window = this;
                // Setup login logout button for start
                login_logout_b.Click += (object sender, RoutedEventArgs e) =>
                {
                    this.Loggin_clicked();
                };
                // Handler to cameras grid
                cams_grid = cameras_grid;
                // Update Urls From Database
                UpdatesFromDB();
                // Open he Cameras Windows
                CreateVideosPage();

                //  DispatcherTimer setup (Thread Excecutes date update every 1 second)
                date.Content = DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));
                System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
            }
            catch (System.IO.FileLoadException ex)
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
            System.Timers.Timer recording_Cicle = new System.Timers.Timer();
            recording_Cicle.Interval = video_files_time_size;
            recording_Cicle.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) =>
            {
                Dispatcher.Invoke((Action)delegate ()
               {
                   try
                   {
                       foreach (Camera cam in MainWindow.cameras)
                       {
                           if (cam.Recording)
                           {
                               cam.StopRecording();
                               cam.StartRecording();
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


            // Delete All Video Records Beforethe  " video_recording_history_length " Start Scheduling
            System.Timers.Timer deleting_cicle = new System.Timers.Timer();
            deleting_cicle.Interval = 86400000; // Every 24 Hours
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




        // Delete Oldes Videos And Images
        public void DeleteOldFiles()
        {
            // Delete Pictures
            this.GetDirsSubDirsFiles(Camera.pictures_dir, myPath =>
            {
                // Get The Months Folder And If Month is Smaller From MainWindow.video_recording_history_length Delete them
                FileInfo info = new FileInfo(myPath);
                String name = info.FullName;
                String[] dirs = name.Split('\\');
                String date_string = dirs[dirs.Length - 2].Trim();
                DateTime date = DateTime.ParseExact(date_string, "dd-MM-yyyy", null);
                if (date.Month < DateTime.Today.AddMonths(-(MainWindow.video_recording_history_length)).Month)
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
            this.GetDirsSubDirsFiles(Camera.videos_dir, myPath =>
            {
                // Get The Months Folder And If Month is Smaller From MainWindow.video_recording_history_length Delete them
                FileInfo info = new FileInfo(myPath);
                String name = info.FullName;
                String[] dirs = name.Split('\\');
                String date_string = dirs[dirs.Length - 2].Trim();
                DateTime date = DateTime.ParseExact(date_string, "dd-MM-yyyy", null);
                Console.WriteLine($"Folder Month: {date.Month}  Current Month: {DateTime.Today.Month}");
                if (date.Month < DateTime.Today.AddMonths(-(MainWindow.video_recording_history_length)).Month)
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


        // Search All Dirs And Sub Dirs and excecute a Function
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

        // Set DateTime
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current time 
            date.Content = DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));
        }

        // Restart Application
        public static void RestartApp()
        {
            MainWindow old_win = main_window;
            System.Windows.Forms.Application.Restart();
            old_win.Close();
        }


        // Get The saved Cameras From Database
        public void UpdatesFromDB()
        {
            cameras.Clear();
            // Get Data From DB
            using (MySqlConnection connection = new MySqlConnection(App.DB_connection_string))
            {
                // Get Files Paths Data
                String query = "SELECT Name, Path FROM FilesDirs";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    MySqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        String name = dataReader["Name"].ToString().Trim();
                        String path = dataReader["Path"].ToString().Trim();
                        Console.WriteLine($"\n FilesDirs: {name}  {path}");
                        if (name == "Pictures")
                        {
                            Camera.pictures_dir = path;
                        }
                        if (name == "Videos")
                        {
                            Camera.videos_dir = path;
                        }
                    }
                }
                connection.Close();

                // Get  Files Format Data
                query = "SELECT * FROM FilesFormats";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    MySqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        // String avi = dataReader["avi"].ToString().Trim();
                        //String mp4 = dataReader["mp4"].ToString().Trim();
                        //String webm = dataReader["webm"].ToString().Trim();
                        Camera.avi_format = (dataReader["avi"].ToString().Trim() == "True")? true : false;
                        Camera.mp4_format = (dataReader["mp4"].ToString().Trim() == "True")? true : false;
                        MainWindow.video_recording_history_length = Int32.Parse(dataReader["history_time"].ToString());
                        Console.WriteLine($"\nFilesFormats: {Camera.avi_format}  {Camera.mp4_format}");
                    }
                }
                connection.Close();

                // Get Cameras Data
                query = "SELECT * FROM MyCameras";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    MySqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        String id = dataReader["id"].ToString().Trim();
                        String url = dataReader["urls"].ToString().Trim();
                        String name = dataReader["name"].ToString().Trim();
                        String username = dataReader["username"].ToString().Trim();
                        String password = dataReader["password"].ToString().Trim();
                        int fps = (int)dataReader["fps"];
                        String detection = dataReader["Face_Detection"].ToString().Trim();
                        String recognition = dataReader["Face_Recognition"].ToString().Trim();
                        String recording = dataReader["Recording"].ToString().Trim();
                        String on_move_sms = dataReader["On_Move_SMS"].ToString().Trim();
                        String on_move_email = dataReader["On_Move_EMAIL"].ToString().Trim();
                        String on_move_pic = dataReader["On_Move_Pic"].ToString().Trim();
                        String on_move_rec = dataReader["On_Move_Rec"].ToString().Trim();
                        
                        String up = dataReader["Up_req"].ToString().Trim();
                        String down = dataReader["Down_req"].ToString().Trim();
                        String right = dataReader["Right_req"].ToString().Trim();
                        String left = dataReader["Left_req"].ToString().Trim();
                        if (up.Equals("NULL"))
                        {
                            up = "";
                        }
                        if (down.Equals("NULL"))
                        {
                            down = "";
                        }
                        if (right.Equals("NULL"))
                        {
                            right = "";
                        }
                        if (left.Equals("NULL"))
                        {
                            left = "";
                        }

                        int brightness = (int)dataReader["Brightness"];
                        int contrast = (int)dataReader["Contrast"];
                        int darkness = (int)dataReader["Darkness"];
                        int move_sensitivity = (int)dataReader["Move_Sensitivity"];
                        
                        String net_stream_port_l = (String)dataReader["net_stream_port"].ToString().Trim();
                        String net_stream_prefix_l = (String)dataReader["net_stream_prefix"].ToString().Trim();
                        String net_stream_l = (String)dataReader["net_stream"].ToString().Trim();
                        
                        Console.WriteLine($"\n\n\nNetStream:  {net_stream_l}\n\n\n");
                        Console.WriteLine($"\n\n\nNetStream_Port:  {net_stream_port_l}\n\n\n");
                        Console.WriteLine($"\n\n\nNetStream_Prefix:  {net_stream_prefix_l}\n\n\n");
                        
                        bool isEsp = (dataReader["isEsp32"].ToString().Trim() == "True");

                        Console.WriteLine($"\nMyCameras: {url} {name} {username} {password}");
                        try
                        {
                            bool rec = (recording == "True");
                            Camera cam = new Camera(url, name, id, rec)
                            {
                                Username = username,
                                Password = password,
                                Framerate = fps,
                                Brightness = brightness,
                                Contrast = contrast,
                                Darkness = darkness,
                                Detection = (detection == "True") ? true : false,
                                Recognition = (recognition == "True") ? true : false,
                                On_move_sms = (on_move_sms == "True") ? true : false,
                                On_move_email = (on_move_email == "True") ? true : false,
                                On_move_pic = (on_move_pic == "True") ? true : false,
                                On_move_rec = (on_move_rec == "True") ? true : false,
                                On_move_sensitivity = move_sensitivity,
                                up_req = up,
                                down_req = down,
                                right_req = right,
                                left_req = left,
                                Net_stream_port = net_stream_port_l,
                                Net_stream_prefix = net_stream_prefix_l,
                                Net_stream = (net_stream_l == "True") ? true : false,
                                isEsp32 = isEsp
                            };

                            Console.WriteLine($"\n\n\nCamera.NetStream_Prefix:  {cam.Net_stream}\n\n\n");
                            MainWindow.cameras.Add(cam);
                        }
                        catch (System.ArgumentException ex)
                        {
                            Console.WriteLine($"Source:{ex.Source}\nParamName:{ex.ParamName}\n{ex.Message}");
                        }
                    }
                }
                connection.Close();

                // Get Users Data
                myUsers.Clear();
                query = "SELECT Id, FirstName, LastName, Email, Phone, Licences, Password FROM Users";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    MySqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        int id = (int)dataReader["Id"];
                        String fname = dataReader["FirstName"].ToString().Trim();
                        String lname = dataReader["LastName"].ToString().Trim();
                        String email = dataReader["Email"].ToString().Trim();
                        String phone = dataReader["Phone"].ToString().Trim();
                        String licences = dataReader["Licences"].ToString().Trim();
                        String pass = dataReader["Password"].ToString().Trim();
                        // Create The Usres Objects
                        Users user = new Users(id, fname, lname, email, phone, licences, pass);
                        MainWindow.myUsers.Add(user);
                        Console.WriteLine($"\nUser: {id} {fname} {lname} {email} {phone} {licences} {pass}");
                    }
                }
                connection.Close();

                // Get Email_send Pass_send
                query = "SELECT Email, Pass FROM EmailSender";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    MySqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        email_send = dataReader["Email"].ToString().Trim();
                        pass_send = dataReader["Pass"].ToString().Trim();
                        Console.WriteLine($"\nEmailSender: {email_send}  {pass_send}");
                    }
                }
                connection.Close();

                // Get SMS SID, SMS TOKEN, SMS PHONE
                query = "SELECT AccountSID,AccountTOKEN,Phone FROM SMS";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    MySqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        twilioAccountSID = dataReader["AccountSID"].ToString().Trim();
                        twilioAccountToken = dataReader["AccountTOKEN"].ToString().Trim();
                        twilioNumber = dataReader["Phone"].ToString().Trim();
                        Console.WriteLine($"\nSMS  {twilioAccountSID}  {twilioAccountToken}  {twilioNumber}");
                    }
                }
                connection.Close();

                // Get Logged User If Existes
                query = "SELECT Id FROM Logged";
                String user_email = "";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    MySqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        user_email = dataReader["Id"].ToString().Trim();
                        Console.WriteLine($"\nLogged: {user_email}");
                    }
                }
                connection.Close();
                try
                {
                    var u = from user in MainWindow.myUsers where user.Email.Equals(user_email) select user;
                    MainWindow.user = u.Single();
                    MainWindow.logged = true;
                    MainWindow.main_window.login_logout_b.Content = "Logout";
                    MainWindow.main_window.login_logout_b.Click += (object send, RoutedEventArgs ev) =>
                    {
                        MainWindow.main_window.Loggout_clicked();
                    };
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
                }
                
            }
        }

        // Loggin Button Click
        public void Loggin_clicked()
        {
            if (login_oppened == false)
            {
                login_oppened = true;
                this.login = new Login();
                this.login.Show();
            }
            else
            {
                this.login.Activate();
            }
        }

        // Loggout Button Click
        public void Loggout_clicked()
        {
            MainWindow.user = null;
            MainWindow.Logged = false;
            MainWindow.main_window.login_logout_b.Content = "Login";
            login_logout_b.Click += (object sender, RoutedEventArgs e) =>
            {
                this.Loggin_clicked();
            };
        }

        // When Click Start Button
        private void Start_clicked(object sender, RoutedEventArgs e)
        {
            if(logged)
            {
                foreach (Camera cam in cameras)
                {
                    Console.WriteLine("Starting: " + cam.url);
                    cam.Start();
                }
            }
        }

        // When Clecked Stop Button
        private void Stop_clicked(object sender, RoutedEventArgs e)
        {
            if (logged)
            {
                foreach (Camera cam in cameras)
                {
                    Console.WriteLine("Stoping: " + cam.url);
                    cam.Stop();
                }
            }
        }

        // On Close Button
        protected override void OnClosed(EventArgs e)
        {
            foreach (Camera cam in cameras)
            {
                cam.Stop();
            }
            this.Close();
        }

        // When Click Settings Button
        private void Settings_clicked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Logged && MainWindow.myUsers.Contains(MainWindow.user)
                && (MainWindow.user.Licences.Equals("Admin")) )
            {
                if (settings_oppened == false)
                {
                    settings_oppened = true;
                    Console.WriteLine("settings_oppened: " + Convert.ToString(settings_oppened));
                    this.settings = new Settings();
                    this.settings.Show();
                }
                else
                {
                    this.settings.Activate();
                }
            }
        }

        // Account Button Clicked
        private void Account_clicked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Logged && MainWindow.myUsers.Contains(MainWindow.user))
            {
                if(account_oppened == false)
                {
                    account_oppened = true;
                    Console.WriteLine("account_oppened: " + Convert.ToString(account_oppened));
                    this.account = new Account(MainWindow.user);
                    this.account.Show();
                }
                else
                {
                    this.account.Activate();
                }
            }
        }

        // Records Button Clicked
        private void Records_clicked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Logged && MainWindow.myUsers.Contains(MainWindow.user))
            {
                if (records_oppened == false)
                {
                    records_oppened = true;
                    Console.WriteLine("records_oppened: " + Convert.ToString(records_oppened));
                    this.records = new Records();
                    this.records.Show();
                }
                else
                {
                    this.records.Activate();
                }
            }
        }

        // X Button Click
        private void X_Button_Click(object sender, RoutedEventArgs e)
        {
            if(logged)
            {
                this.Close();
            }
        }

        // Find How Many Cameras is connected and open the write UI
        public void CreateVideosPage()
        {
            // Dynamic add columns and rows
            int count_rows = 0;
            int count_columns = 0;
            foreach (Camera cam in cameras)
            {
                // New Row
                if (count_columns == 3)
                {
                    cameras_grid.RowDefinitions.Add(new RowDefinition());
                    count_rows++;
                    Grid.SetColumn(cam.video, count_columns);
                    cam.coll = count_columns;
                    Grid.SetRow(cam.video, count_rows);
                    cam.row = count_rows;
                    cameras_grid.Children.Add(cam.video);
                    count_columns = 0;
                }
                else
                {
                    Grid.SetColumn(cam.video, count_columns);
                    cam.coll = count_columns;
                    Grid.SetRow(cam.video, count_rows);
                    cam.row = count_rows;
                    cameras_grid.Children.Add(cam.video);
                    cameras_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    count_columns++;
                }
            }  
        }
    } // Stop Class

}
