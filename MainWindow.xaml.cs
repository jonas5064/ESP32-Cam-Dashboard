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
using VisioForge.Controls.UI.WPF;
using VisioForge.Types.OutputFormat;
using VisioForge.Types.VideoEffects;


namespace IPCamera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow main_window;
        public static Dictionary<String, String> urls = new Dictionary<String, String>();
        public static int urls_num = 0;
        public static List<String> id_s = new List<String>();
        private List<VideoCapture> cameras_list = new List<VideoCapture>(); // List whos captures all cameras frames
        public static String DB_connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Alexp\\source\\repos\\IPCamera\\Database1.mdf;Integrated Security=True";



        public MainWindow()
        {
            InitializeComponent();
            // Set a Hundeler for this main window
            main_window = this;
            // Update Urls From Database
            updateUrlsFromDB();
            // Open he Cameras Windows
            createVideosPage();
        }




        // Read From Database
        public static void updateUrlsFromDB()
        {
            // Save Data To Database
            using (SqlConnection connection = new SqlConnection(MainWindow.DB_connection_string))
            {
                String query = "SELECT id, urls, name FROM dbo.MyCameras";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        String id = (string)dataReader["id"];
                        String url = (string)dataReader["urls"];
                        String name = (string)dataReader["name"];
                        String id_cl = String.Concat(id.Where(c => !Char.IsWhiteSpace(c)));
                        String url_cl = String.Concat(url.Where(c => !Char.IsWhiteSpace(c)));
                        //String name_cl = String.Concat(name.Where(c => !Char.IsWhiteSpace(c)));
                        try
                        {
                            urls.Add(url_cl, name);
                            id_s.Add(id_cl);
                        }
                        catch (System.ArgumentException)
                        {

                        }



                    }
                    urls_num = id_s.Count;
                }
            }
        }



        // Method to create tha right video captures
        public void createVideosPage()
        {
            // Cameras Names, URLS
            var names_list = urls.Values.ToList();
            var urls_list = urls.Keys.ToList();
            // One Camera
            if (urls_num == 1)
            {
                // Create Grid
                Grid Camera_Container = new Grid();
                Grid.SetRow(Camera_Container, 0);
                main_grid.Children.Add(Camera_Container);
                // Grid Rows
                RowDefinition rowtitle = new RowDefinition();
                RowDefinition rowvideo = new RowDefinition();
                RowDefinition rowbuttons = new RowDefinition();
                RowDefinition rowspace = new RowDefinition();
                rowtitle.Height = new GridLength(50);
                rowvideo.Height = new GridLength(1, GridUnitType.Auto);
                rowbuttons.Height = new GridLength(50, GridUnitType.Star);
                rowspace.Height = new GridLength(100);
                Camera_Container.RowDefinitions.Add(rowtitle);
                Camera_Container.RowDefinitions.Add(rowvideo);
                Camera_Container.RowDefinitions.Add(rowbuttons);
                Camera_Container.RowDefinitions.Add(rowspace);
                // Create Title Label
                Label title = new Label();
                title.Content = names_list[0];
                title.FontSize = 18;
                title.FontWeight = FontWeights.Bold;
                title.HorizontalAlignment = HorizontalAlignment.Center;
                title.VerticalAlignment = VerticalAlignment.Bottom;
                title.Padding = new Thickness(167, 0, 0, 0);
                Grid.SetRow(title, 0);
                Camera_Container.Children.Add(title);
                // Create Video Capture
                VideoCapture camera = new VideoCapture();
                camera.HorizontalAlignment = HorizontalAlignment.Center;
                camera.Margin = new Thickness(0,5,0,-804);
                camera.Width = 883;
                camera.Height = 800;
                //Grid.SetColumnSpan(camera, 1);
                Grid.SetRow(camera, 1);
                Camera_Container.Children.Add(camera);
                // Add Camera to my Video Captures
                cameras_list.Add(camera);
                // Create The Panle Buttons                     This manes the same
                StackPanel button_panel = new StackPanel();
                //button_panel.Height = 25;
                //button_panel.Width = 239;
                //button_panel.Margin = new Thickness(827, 883, 0, -852);
                button_panel.HorizontalAlignment = HorizontalAlignment.Center;
                button_panel.VerticalAlignment = VerticalAlignment.Bottom;
                button_panel.Orientation = Orientation.Horizontal;
                Grid.SetRow(button_panel, 2);
                Camera_Container.Children.Add(button_panel);
                // Create The Start Buttons
                Button start_button = new Button();
                start_button.Content = "Start";
                start_button.Padding = new Thickness(0, 0, 5.0, 0);
                start_button.HorizontalAlignment = HorizontalAlignment.Left;
                start_button.VerticalAlignment = VerticalAlignment.Top;
                start_button.Width = 74;
                //start_button.Padding = new System.Windows.Forms.Padding(3);
                start_button.Click += (sender, args) =>
                {
                    try
                    {
                        int counter = 0;
                        foreach (VideoCapture cam in cameras_list)
                        {
                            cam.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings() { URL = urls_list[counter], Type = VisioForge.Types.VFIPSource.RTSP_HTTP_FFMPEG };
                            cam.Audio_PlayAudio = camera.Audio_RecordAudio = false;
                            cam.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
                            cam.Start();
                            counter++;
                        }
                    }
                    catch
                    {

                    }
                };
                button_panel.Children.Add(start_button);
                // Create Grid Splitter
                GridSplitter gsp1 = new GridSplitter();
                gsp1.HorizontalAlignment = HorizontalAlignment.Stretch;
                gsp1.Width = 7;
                //button_panel.Children.Add(gsp1);
                // Create Stop Button
                Button stop_button = new Button();
                stop_button.Content = "Stop";
                stop_button.Padding = new Thickness(5.0, 0, 5.0, 0);
                stop_button.HorizontalAlignment = HorizontalAlignment.Left;
                stop_button.VerticalAlignment = VerticalAlignment.Top;
                stop_button.Width = 74;
                //stop_button.Padding = new System.Windows.Forms.Padding(3);
                stop_button.Click += (sender, args) =>
                {
                    try
                    {
                        foreach (VideoCapture cam in cameras_list)
                        {
                            cam.Stop();
                        }
                    }
                    catch
                    {

                    }
                };
                button_panel.Children.Add(stop_button);
                // Create Grid Splitter
                GridSplitter gsp2 = new GridSplitter();
                gsp2.HorizontalAlignment = HorizontalAlignment.Stretch;
                gsp2.Width = 7;
                //button_panel.Children.Add(gsp2);
                // Create Settings Button
                Button settings_button = new Button();
                settings_button.Content = "Settings";
                settings_button.Padding = new Thickness(5.0, 0, 0, 0);
                settings_button.HorizontalAlignment = HorizontalAlignment.Left;
                settings_button.VerticalAlignment = VerticalAlignment.Top;
                settings_button.Width = 74;
                //settings_button.Padding = new System.Windows.Forms.Padding(3);
                settings_button.Click += (sender, args) =>
                {
                    try
                    {
                        foreach (VideoCapture cam in cameras_list)
                        {
                            Settings OP = new Settings();
                            OP.Show();
                        }
                    }
                    catch
                    {

                    }
                };
                button_panel.Children.Add(settings_button);
            }
            // Tow Camera
            else if (urls_num == 2)
            {
                /*
                // Grid Sise
                Camera_Container.Width = 1000;
                Camera_Container.Height = 1000;
                // Grid Columns
                ColumnDefinition coll_1 = new ColumnDefinition();
                ColumnDefinition coll_2 = new ColumnDefinition();
                Camera_Container.ColumnDefinitions.Add(coll_1);
                Camera_Container.ColumnDefinitions.Add(coll_2);
                // Grid Rows
                RowDefinition rowtitle = new RowDefinition();
                RowDefinition rowvideo = new RowDefinition();
                Camera_Container.RowDefinitions.Add(rowtitle);
                Camera_Container.RowDefinitions.Add(rowvideo);
                // Create Title 1 Label
                Label title_1 = new Label();
                title_1.Content = names_list[0];
                title_1.FontSize = 18;
                title_1.FontWeight = FontWeights.Bold;
                Grid.SetColumnSpan(title_1, 1);
                Grid.SetRow(title_1, 1);
                Camera_Container.Children.Add(title_1);
                // Create Title 2 Label
                Label title_2 = new Label();
                title_2.Content = names_list[0];
                title_2.FontSize = 18;
                title_2.FontWeight = FontWeights.Bold;
                Grid.SetColumnSpan(title_2, 2);
                Grid.SetRow(title_2, 1);
                Camera_Container.Children.Add(title_2);
                // Create Video Capture 1
                VideoCapture camera_1 = new VideoCapture();
                //camera_1.Name = names_list[0];
                Grid.SetColumnSpan(camera_1, 1);
                Grid.SetRow(camera_1, 2);
                Camera_Container.Children.Add(camera_1);
                cameras_list.Add(camera_1);
                // Create Video Capture 2
                VideoCapture camera_2 = new VideoCapture();
                //camera_2.Name = names_list[0];
                Grid.SetColumnSpan(camera_2, 2);
                Grid.SetRow(camera_2, 2);
                Camera_Container.Children.Add(camera_2);
                // Add Camera to my Video Captures
                cameras_list.Add(camera_2);
                */
            }
            
        }

        
    }

    
}
