using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public static MainWindow Main_window { get; set; }
        public static List<Camera> Cameras = new List<Camera>();
        public static List<Users> MyUsers = new List<Users>();
        public static Users User { get; set; }
        public static Grid cams_grid;
        public static String Email_send { get; set; }
        public static String Pass_send { get; set; }
        public Login Login { get; set; }
        private static bool _logged = false;
        public static int Video_files_time_size = 3600000; // 1 Hour
        public static int Video_recording_history_length = 1; // 1 Month

        public static bool Logged
        {
            set
            {
                MainWindow._logged = value;
                // Save the logged User
                if (MainWindow._logged)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(App.DB_connection_string))
                        {
                            String query = $"INSERT INTO Logged (Id) VALUES (@user)";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@user", MainWindow.User.Email);
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
                        using (SqlConnection cn = new SqlConnection(App.DB_connection_string))
                        {
                            String query = "DELETE FROM Logged";
                            using (SqlCommand cmd = new SqlCommand(query, cn))
                            {
                                cn.Open();
                                int result = cmd.ExecuteNonQuery();
                                if (result < 0)
                                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                                cn.Close();
                            }
                        }                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"Source: {ex.Message}");
                    }
                }
            }
            get { return MainWindow._logged; }
        }
        private Settings _settings;
        public static bool Settings_oppened { get; set; }
        public static bool Login_oppened { get; set; }
        private Account _account;
        public static bool Account_oppened { get; set; }
        private Records _records;
        public static bool Records_oppened { get; set; }
        public static String TwilioNumber { get; set; }
        public static String TwilioAccountSID { get; set; }
        public static String TwilioAccountToken { get; set; }


        public MainWindow()
        {
            Video_files_time_size = 3600000; // 1 Hore
            Video_recording_history_length = 1; // 1 Month
            Settings_oppened = false;
            Login_oppened = false;
            Account_oppened = false;
            Records_oppened = false;
            //Logged = false;

            try
            {
                Console.WriteLine("Staring Main...");
                // Runs only one time and install some requarements
                if (Install_Requarements.First_time_runs)
                {
                    Console.WriteLine("Inside First Time Run.");
                    try
                    {
                        // Open a Window And Set DataBase Imfos
                                ////

                        // Install Requarements
                        Install_Requarements.Install_Req();
                        // Create an Admin User
                        try
                        {
                            String query = $"INSERT INTO Users (FirstName, LastName, Email, Phone, Licences, Password)" +
                                                                    $" VALUES (@fname, @lname, @email, @phone, @licences, @pass)";
                            using (SqlConnection connection = new SqlConnection(App.DB_connection_string))
                            {
                                using (SqlCommand command = new SqlCommand(query, connection))
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
                Main_window = this;
                // Setup Login logout button for start
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
            System.Timers.Timer recording_Cicle = new System.Timers.Timer
            {
                Interval = Video_files_time_size
            };
            recording_Cicle.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) =>
            {
                Dispatcher.Invoke((Action)delegate ()
               {
                   try
                   {
                       foreach (Camera cam in MainWindow.Cameras)
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




        // Delete Oldes Videos And Images
        public void DeleteOldFiles()
        {
            // Delete Pictures
            this.GetDirsSubDirsFiles(Camera.Pictures_dir, myPath =>
            {
                // Get The Months Folder And If Month is Smaller From MainWindow.Video_recording_history_length Delete them
                FileInfo info = new FileInfo(myPath);
                String name = info.FullName;
                String[] dirs = name.Split('\\');
                String date_string = dirs[dirs.Length - 2].Trim();
                DateTime date = DateTime.ParseExact(date_string, "dd-MM-yyyy", null);
                if (date.Month < DateTime.Today.AddMonths(-(MainWindow.Video_recording_history_length)).Month)
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
            this.GetDirsSubDirsFiles(Camera.Videos_dir, myPath =>
            {
                // Get The Months Folder And If Month is Smaller From MainWindow.Video_recording_history_length Delete them
                FileInfo info = new FileInfo(myPath);
                String name = info.FullName;
                String[] dirs = name.Split('\\');
                String date_string = dirs[dirs.Length - 2].Trim();
                DateTime date = DateTime.ParseExact(date_string, "dd-MM-yyyy", null);
                Console.WriteLine($"Folder Month: {date.Month}  Current Month: {DateTime.Today.Month}");
                if (date.Month < DateTime.Today.AddMonths(-(MainWindow.Video_recording_history_length)).Month)
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
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current time 
            date.Content = DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));
        }

        // Restart Application
        public static void RestartApp()
        {
            MainWindow old_win = Main_window;
            System.Windows.Forms.Application.Restart();
            old_win.Close();
        }


        // Get The saved Cameras From Database
        public void UpdatesFromDB()
        {
            Cameras.Clear();
            // Get Data From DB
            using (SqlConnection connection = new SqlConnection(App.DB_connection_string))
            {
                // Get Files Paths Data
                String query = "SELECT Name, Path FROM FilesDirs";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        String name = dataReader["Name"].ToString().Trim();
                        String path = dataReader["Path"].ToString().Trim();
                        Console.WriteLine($"\n FilesDirs: {name}  {path}");
                        if (name == "Pictures")
                        {
                            Camera.Pictures_dir = path;
                        }
                        if (name == "Videos")
                        {
                            Camera.Videos_dir = path;
                        }
                    }
                }
                connection.Close();

                // Get  Files Format Data
                query = "SELECT * FROM FilesFormats";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        // String avi = dataReader["avi"].ToString().Trim();
                        //String mp4 = dataReader["mp4"].ToString().Trim();
                        //String webm = dataReader["webm"].ToString().Trim();
                        Camera.Avi_format = (dataReader["avi"].ToString().Trim() == "True");
                        Camera.Mp4_format = (dataReader["mp4"].ToString().Trim() == "True");
                        MainWindow.Video_recording_history_length = Int32.Parse(dataReader["history_time"].ToString());
                        Console.WriteLine($"\nFilesFormats: {Camera.Avi_format}  {Camera.Mp4_format}");
                    }
                }
                connection.Close();

                // Get Cameras Data
                query = "SELECT * FROM MyCameras";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
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
                                Detection = (detection == "True"),
                                Recognition = (recognition == "True"),
                                On_move_sms = (on_move_sms == "True"),
                                On_move_email = (on_move_email == "True"),
                                On_move_pic = (on_move_pic == "True"),
                                On_move_rec = (on_move_rec == "True"),
                                On_move_sensitivity = move_sensitivity,
                                Up_req = up,
                                Down_req = down,
                                Right_req = right,
                                Left_req = left,
                                Net_stream_port = net_stream_port_l,
                                Net_stream_prefix = net_stream_prefix_l,
                                Net_stream = (net_stream_l == "True"),
                                IsEsp32 = isEsp
                            };

                            Console.WriteLine($"\n\n\nCamera.NetStream_Prefix:  {cam.Net_stream}\n\n\n");
                            MainWindow.Cameras.Add(cam);
                        }
                        catch (System.ArgumentException ex)
                        {
                            Console.WriteLine($"Source:{ex.Source}\nParamName:{ex.ParamName}\n{ex.Message}");
                        }
                    }
                }
                connection.Close();

                // Get Users Data
                MyUsers.Clear();
                query = "SELECT Id, FirstName, LastName, Email, Phone, Licences, Password FROM Users";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
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
                        MainWindow.MyUsers.Add(user);
                        Console.WriteLine($"\nUser: {id} {fname} {lname} {email} {phone} {licences} {pass}");
                    }
                }
                connection.Close();

                // Get Email_send Pass_send
                query = "SELECT Email, Pass FROM EmailSender";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        Email_send = dataReader["Email"].ToString().Trim();
                        Pass_send = dataReader["Pass"].ToString().Trim();
                        Console.WriteLine($"\nEmailSender: {Email_send}  {Pass_send}");
                    }
                }
                connection.Close();

                // Get SMS SID, SMS TOKEN, SMS PHONE
                query = "SELECT AccountSID,AccountTOKEN,Phone FROM SMS";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        TwilioAccountSID = dataReader["AccountSID"].ToString().Trim();
                        TwilioAccountToken = dataReader["AccountTOKEN"].ToString().Trim();
                        TwilioNumber = dataReader["Phone"].ToString().Trim();
                        Console.WriteLine($"\nSMS  {TwilioAccountSID}  {TwilioAccountToken}  {TwilioNumber}");
                    }
                }
                connection.Close();

                // Get Logged User If Existes
                query = "SELECT Id FROM Logged";
                String user_email = "";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        user_email = dataReader["Id"].ToString().Trim();
                        Console.WriteLine($"\nLogged: {user_email}");
                    }
                }
                connection.Close();
                try
                {
                    var u = from user in MainWindow.MyUsers where user.Email.Equals(user_email) select user;
                    MainWindow.User = u.Single();
                    MainWindow._logged = true;
                    MainWindow.Main_window.login_logout_b.Content = "Logout";
                    MainWindow.Main_window.login_logout_b.Click += (object send, RoutedEventArgs ev) =>
                    {
                        MainWindow.Main_window.Loggout_clicked();
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

        // Loggout Button Click
        public void Loggout_clicked()
        {
            MainWindow.User = null;
            MainWindow.Logged = false;
            MainWindow.Main_window.login_logout_b.Content = "Login";
            login_logout_b.Click += (object sender, RoutedEventArgs e) =>
            {
                this.Loggin_clicked();
            };
        }

        // When Click Start Button
        private void Start_clicked(object sender, RoutedEventArgs e)
        {
            if(Logged)
            {
                foreach (Camera cam in Cameras)
                {
                    Console.WriteLine("Starting: " + cam.Url);
                    cam.Start();
                }
            }
        }

        // When Clecked Stop Button
        private void Stop_clicked(object sender, RoutedEventArgs e)
        {
            if (Logged)
            {
                foreach (Camera cam in Cameras)
                {
                    Console.WriteLine("Stoping: " + cam.Url);
                    cam.Stop();
                }
            }
        }

        // On Close Button
        protected override void OnClosed(EventArgs e)
        {
            foreach (Camera cam in Cameras)
            {
                cam.Stop();
            }
            this.Close();
        }

        // When Click Settings Button
        private void Settings_clicked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Logged && MainWindow.MyUsers.Contains(MainWindow.User)
                && (MainWindow.User.Licences.Equals("Admin")) )
            {
                if (Settings_oppened == false)
                {
                    Settings_oppened = true;
                    Console.WriteLine("Settings_oppened: " + Convert.ToString(Settings_oppened));
                    this._settings = new Settings();
                    this._settings.Show();
                }
                else
                {
                    this._settings.Activate();
                }
            }
        }

        // Account Button Clicked
        private void Account_clicked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Logged && MainWindow.MyUsers.Contains(MainWindow.User))
            {
                if(Account_oppened == false)
                {
                    Account_oppened = true;
                    Console.WriteLine("Account_oppened: " + Convert.ToString(Account_oppened));
                    this._account = new Account(MainWindow.User);
                    this._account.Show();
                }
                else
                {
                    this._account.Activate();
                }
            }
        }

        // Records Button Clicked
        private void Records_clicked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Logged && MainWindow.MyUsers.Contains(MainWindow.User))
            {
                if (Records_oppened == false)
                {
                    Records_oppened = true;
                    Console.WriteLine("Records_oppened: " + Convert.ToString(Records_oppened));
                    this._records = new Records();
                    this._records.Show();
                }
                else
                {
                    this._records.Activate();
                }
            }
        }

        // X Button Click
        private void X_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Logged)
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
            foreach (Camera cam in Cameras)
            {
                // New Row
                if (count_columns == 3)
                {
                    cameras_grid.RowDefinitions.Add(new RowDefinition());
                    count_rows++;
                    Grid.SetColumn(cam.Video, count_columns);
                    cam.Coll = count_columns;
                    Grid.SetRow(cam.Video, count_rows);
                    cam.Row = count_rows;
                    cameras_grid.Children.Add(cam.Video);
                    count_columns = 0;
                }
                else
                {
                    Grid.SetColumn(cam.Video, count_columns);
                    cam.Coll = count_columns;
                    Grid.SetRow(cam.Video, count_rows);
                    cam.Row = count_rows;
                    cameras_grid.Children.Add(cam.Video);
                    cameras_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    count_columns++;
                }
            }  
        }
    } // Stop Class

}
