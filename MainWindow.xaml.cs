using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisioForge.Types.OutputFormat;
using VisioForge.Types.VideoEffects;
using VisioForge.Controls.UI.WPF;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace IPCamera
{

    public partial class MainWindow : Window
    {

        public static MainWindow main_window;
        public static List<Camera> cameras = new List<Camera>();
        public static List<Users> myUsers = new List<Users>();
        public static Grid cams_grid;
        public static String email_send;
        public static String pass_send;

        public static String twilioNumber;
        public static String twilioAccountSID;
        public static String twilioAccountToken;

        public static bool settings_oppened = false;

        public MainWindow()
        {
            InitializeComponent();
            // Set a Hundeler for this main window
            main_window = this;
            cams_grid = cameras_grid;
            // Update Urls From Database
            UpdatesFromDB();
            // Open he Cameras Windows
            CreateVideosPage();

            /*
            HttpServer server = new HttpServer();
            server.ip = "localhost";
            server.port = "8000";
            server.setup();
            _ = server.ListenAsync();
            //server.close();
            */

            //  DispatcherTimer setup (Thread Excecutes date update every 1 second)
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        // Set DateTime
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current time 
            date.Content = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
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
            using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
            {
                // Get Files Paths Data
                String query = "SELECT Name, Path FROM dbo.FilesDirs";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        String name = dataReader["Name"].ToString().Trim();
                        String path = dataReader["Path"].ToString().Trim();
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
                query = "SELECT avi, mp4, webm FROM dbo.FilesFormats";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        // String avi = dataReader["avi"].ToString().Trim();
                        //String mp4 = dataReader["mp4"].ToString().Trim();
                        //String webm = dataReader["webm"].ToString().Trim();
                        Camera.avi_format = (dataReader["avi"].ToString().Trim() == "True");
                        Camera.mp4_format = (dataReader["mp4"].ToString().Trim() == "True");
                        Camera.webm_format = (dataReader["webm"].ToString().Trim() == "True");
                    }
                }
                connection.Close();

                // Get Cameras Data
                query = "SELECT * FROM dbo.myCameras";
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
                        try
                        {
                            bool rec = (recording == "True");
                            Camera cam = new Camera(url, name, id, rec)
                            {
                                Username = username,
                                Password = password,
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
                                up_req = up,
                                down_req = down,
                                right_req = right,
                                left_req = left,
                                net_stream_port = net_stream_port_l,
                                net_stream_prefix = net_stream_prefix_l,
                                Net_stream = (net_stream_l == "True")
                            };
                            MainWindow.cameras.Add(cam);
                        }
                        catch (System.ArgumentException)
                        {

                        }
                    }
                }
                connection.Close();

                // Get Users Data
                myUsers.Clear();
                query = "SELECT Id, FirstName, LastName, Email, Phone FROM dbo.Users";
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
                        // Create The Usres Objects
                        Users user = new Users(id, fname, lname, email, phone);
                        MainWindow.myUsers.Add(user);
                    }
                }
                connection.Close();

                // Get Email_send Pass_send
                query = "SELECT Email, Pass FROM dbo.EmailSender";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        email_send = dataReader["Email"].ToString().Trim();
                        pass_send = dataReader["Pass"].ToString().Trim();
                    }
                }
                connection.Close();

                // Get SMS SID, SMS TOKEN, SMS PHONE
                query = "SELECT AccountSID,AccountTOKEN,Phone FROM dbo.SMS";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        twilioAccountSID = dataReader["AccountSID"].ToString().Trim();
                        twilioAccountToken = dataReader["AccountTOKEN"].ToString().Trim();
                        twilioNumber = dataReader["Phone"].ToString().Trim();
                    }
                }
                connection.Close();
            }
        }

        // When Click Start Button
        private void Start_clicked(object sender, RoutedEventArgs e)
        {
            foreach (Camera cam in cameras)
            {
                Console.WriteLine("Starting: " + cam.url);
                cam.Start();
            }
        }

        // When Clecked Stop Button
        private void Stop_clicked(object sender, RoutedEventArgs e)
        {
            foreach (Camera cam in cameras)
            {
                Console.WriteLine("Stoping: " + cam.url);
                cam.Stop();
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
            if (settings_oppened == false)
            {
                settings_oppened = true;
                Console.WriteLine("settings_oppened: " + Convert.ToString(settings_oppened));
                Settings settings = new Settings();
                settings.Show();
            }
        }


        // Find How Many Cameras is connected and open the write UI
        public void CreateVideosPage()
        {
            // Dynamic add columns and rows
            int count_rows = 0;
            int counter = 0;
            foreach (Camera cam in cameras)
            {
                // New Row
                if (counter == 3)
                {
                    cameras_grid.RowDefinitions.Add(new RowDefinition());
                    count_rows++;
                    Grid.SetColumn(cam.video, counter);
                    cam.coll = counter;
                    Grid.SetRow(cam.video, count_rows);
                    cam.row = count_rows;
                    cameras_grid.Children.Add(cam.video);
                    counter = 0;
                }
                else
                {
                    Grid.SetColumn(cam.video, counter);
                    cam.coll = counter;
                    Grid.SetRow(cam.video, count_rows);
                    cam.row = count_rows;
                    cameras_grid.Children.Add(cam.video);
                    cameras_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    counter++;
                }
            }  
        }
    } // Stop Class

}
