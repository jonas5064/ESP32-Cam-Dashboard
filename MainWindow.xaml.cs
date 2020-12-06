using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
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
        //public static List<String> urls = new List<string>();
        public static Dictionary<String, String> urls = new Dictionary<String, String>();
        public static int urls_num = 0;
        public static List<String> id_s = new List<String>();
        private List<VideoCapture> cameras_list = new List<VideoCapture>(); // List whos captures all cameras frames
        public static String DB_connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Alexp\\source\\repos\\IPCamera\\Database1.mdf;Integrated Security=True";

        public MainWindow()
        {
            InitializeComponent();

            // Update Urls From Database
            updateUrlsFromDB();

            // Open he Cameras Windows
            createVideos(urls_num);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Start Cameras
            try
            {
                // Cameras Names, URLS
                var urls_list = urls.Keys.ToList();
                int counter = 0;
                foreach(VideoCapture camera in cameras_list)
                {
                    camera.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings() { URL = urls_list[counter], Type = VisioForge.Types.VFIPSource.RTSP_HTTP_FFMPEG };
                    camera.Audio_PlayAudio = camera.Audio_RecordAudio = false;
                    camera.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
                    camera.Start();
                    counter++;
                }
            } catch
            {
                
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Stop Cameras
            try
            {
                foreach (VideoCapture camera in cameras_list)
                {
                    camera.Stop();
                }
            }
            catch
            {

            }
        }


        // Setings
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Settings OP = new Settings(); // Open Settings.xaml
            //var host = new Window();
            //host.Content = OP;
            OP.Show();
            //this.Close(); // Close the current Window
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
                        urls.Add(url_cl, name);
                        id_s.Add(id_cl);
                        
                    }
                    urls_num = id_s.Count;
                }
            }
        }

        // Method to create tha right video captures
        private void createVideos(int num)
        {
            // Cameras Names, URLS
            var names_list = urls.Values.ToList();
            var urls_list = urls.Keys.ToList();
            if (num == 1) // Camera 1 OK
            {
                // Create Grid
                Grid Camera_Container = new Grid();
                Grid.SetRow(Camera_Container, 0);
                main_grid.Children.Add(Camera_Container);
                // Grid Rows
                RowDefinition rowtitle = new RowDefinition();
                RowDefinition rowvideo = new RowDefinition();
                rowtitle.Height = new GridLength(50);
                rowtitle.Height = new GridLength(1,GridUnitType.Star);
                //rowvideo.Height = new GridLength(1, GridUnitType.Star);
                Camera_Container.RowDefinitions.Add(rowtitle);
                Camera_Container.RowDefinitions.Add(rowvideo);
                // Create Title Label
                Label title = new Label();
                title.Content = names_list[0];
                title.FontSize = 18;
                title.FontWeight = FontWeights.Bold;
                title.HorizontalAlignment = HorizontalAlignment.Center;
                title.VerticalAlignment = VerticalAlignment.Bottom;
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
                // Create The Panle Buttons
                StackPanel button_panel = new StackPanel();
                button_panel.Height = 25;
                button_panel.Width = 239;
                button_panel.Margin = new Thickness(799, 883, 0, -852);
                button_panel.HorizontalAlignment = HorizontalAlignment.Center;
                button_panel.VerticalAlignment = VerticalAlignment.Top;
                button_panel.Orientation = Orientation.Horizontal;
                Grid.SetRow(button_panel, 1);
                main_grid.Children.Add(button_panel);
                // Create The Buttons
                        ///
            }
            else if (num == 2)
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
