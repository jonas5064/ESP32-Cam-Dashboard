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


namespace IPCamera
{

    public partial class MainWindow : Window
    {

        public static MainWindow main_window;
        public static List<Camera> cameras = new List<Camera>();
        public static Grid cams_grid;


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
            // Save Data To Database
            using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
            {

                // Insert Data Paths
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

                // Insert Saved Files Format
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

                // Insert Camera Data
                query = "SELECT id, urls, name, Face_Detection, Face_Recognition, " +
                    "Brightness, Contrast, Darkness, Recording FROM dbo.myCameras";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        String id = dataReader["id"].ToString().Trim();
                        String url = dataReader["urls"].ToString().Trim();
                        String name = dataReader["name"].ToString().Trim();
                        String detection = dataReader["Face_Detection"].ToString().Trim();
                        String recognition = dataReader["Face_Recognition"].ToString().Trim();
                        String recording = dataReader["Recording"].ToString().Trim();
                        int brightness = (int)dataReader["Brightness"];
                        int contrast = (int)dataReader["Contrast"];
                        int darkness = (int)dataReader["Darkness"];
                        try
                        {
                            bool rec = (recording == "True");
                            Camera cam = new Camera(url, name, id, rec)
                            {
                                Brightness = brightness,
                                Contrast = contrast,
                                Darkness = darkness,
                                Detection = (detection == "True"),
                                Recognition = (recognition == "True")
                            };
                            cameras.Add(cam);
                        }
                        catch (System.ArgumentException)
                        {

                        }
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
                cam.Start();
            }
        }

        // When Clecked Stop Button
        private void Stop_clicked(object sender, RoutedEventArgs e)
        {
            foreach (Camera cam in cameras)
            {
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
            Settings settings = new Settings();
            settings.Show();
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
