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
        public static String DB_connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Alexp\\source\\repos\\IPCamera\\Database1.mdf;Integrated Security=True";

        public MainWindow()
        {
            InitializeComponent();

            // Update Urls From Database
            updateUrlsFromDB();
            // Upload labels
            var names_list = urls.Values.ToList();
            // Camera 1
            Console.WriteLine("Camera 1 name: " + names_list[0]);
            cam_1.Content = names_list[0];
            // Camera 2
            Console.WriteLine("Camera 2 name: " + names_list[1]);
            cam_2.Content = names_list[1];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var urls_list = urls.Keys.ToList();

            // In The future i will creat the page with defrent number o streamings
            ///

            // Camera 1
            IPCameraFrame_1.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings() { URL = urls_list[0], Type = VisioForge.Types.VFIPSource.RTSP_HTTP_FFMPEG };
            IPCameraFrame_1.Audio_PlayAudio = IPCameraFrame_1.Audio_RecordAudio = false;
            IPCameraFrame_1.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
            IPCameraFrame_1.Start();

            // Camera 2
            IPCameraFrame_2.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings() { URL = urls_list[1], Type = VisioForge.Types.VFIPSource.RTSP_HTTP_FFMPEG };
            IPCameraFrame_2.Audio_PlayAudio = IPCameraFrame_1.Audio_RecordAudio = false;
            IPCameraFrame_2.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
            IPCameraFrame_2.Start();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Camera 1
            IPCameraFrame_1.Stop();
            // Camera 2
            IPCameraFrame_2.Stop();
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
                        urls.Add(url, name);
                        id_s.Add(id);
                        
                    }
                    urls_num = id_s.Count;
                }
            }
        }
    }

    
}
