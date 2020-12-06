using System;
using System.Collections.Generic;
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
        String[] url = { 
                        "http://192.168.1.33:152/stream?username=alexandrosplatanios&password=Platanios719791",
                        "http://192.168.1.34:152/stream?username=alexandrosplatanios&password=Platanios719791" 
        };

        //public static List<String> urls = new List<string>();
        public static Dictionary<String, String> urls = new Dictionary<String, String>();
        public static int urls_num = 0;
        public static String DB_connection_string = "(localdb)\\MSSQLLocalDB;Initial Catalog=C:\\USERS\\ALEXP\\SOURCE\\REPOS\\IPCAMERA\\DATABASE1.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Camera 1
            IPCameraFrame_1.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings() { URL = url[0], Type = VisioForge.Types.VFIPSource.RTSP_HTTP_FFMPEG };
            IPCameraFrame_1.Audio_PlayAudio = IPCameraFrame_1.Audio_RecordAudio = false;
            IPCameraFrame_1.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
            IPCameraFrame_1.Start();

            // Camera 2
            IPCameraFrame_2.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings() { URL = url[1], Type = VisioForge.Types.VFIPSource.RTSP_HTTP_FFMPEG };
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
    }

    
}
