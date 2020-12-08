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
        private Grid Camera_Container;
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


        // Restart Application
        public static void RestartApp()
        {
            MainWindow old_win = main_window;
            System.Windows.Forms.Application.Restart();
            old_win.Close();
        }


        // Get The saved Cameras From Database
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
                        String id = dataReader["id"].ToString().Trim();
                        String url = dataReader["urls"].ToString().Trim();
                        String name = dataReader["name"].ToString().Trim();
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




        // When Click on Video
        private void camerasFocused(object sender, MouseButtonEventArgs e)
        {

            // Get Urls And Names
            VideoCapture camera = ((VideoCapture)sender);
            // If this camera is working
            if (camera.Status == VisioForge.Types.VFVideoCaptureStatus.Work)
            {
                try
                {
                    Camera_Container.Children.Remove(camera);
                }
                catch (System.NullReferenceException nre)
                {
                    System.Windows.MessageBox.Show(nre.ToString());
                }
                if (!Camera_Container.Children.Contains(camera))
                {
                    //System.Windows.MessageBox.Show("Opening EDitor!");
                    WindowControll win_controll = new WindowControll(camera);
                    win_controll.Show();
                }
                else
                {
                    // Ask to Restart The Application
                    MessageBoxResult res = System.Windows.MessageBox.Show("Error When Opening Editor! Restart ?", "Question", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.OKCancel);
                    if (res.ToString() == "OK")
                    {
                        // Close Settings Window
                        this.Close();
                        // Restart App Application
                        MainWindow.RestartApp();
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("No cameras has found!");
            }
            
            
        }


        // When Click Start Button
        private void start_clicked(object sender, RoutedEventArgs e)
        {
            var urls_list = urls.Keys.ToList();
            try
            {
                int counter = 0;
                foreach (VideoCapture cam in cameras_list)
                {
                    cam.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings() { URL = urls_list[counter], Type = VisioForge.Types.VFIPSource.RTSP_HTTP_FFMPEG };
                    cam.Audio_PlayAudio = cam.Audio_RecordAudio = false;
                    cam.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
                    cam.Start();
                    counter++;
                }
            }
            catch
            {

            }
        }

        // When Clecked Stop Button
        private void stop_clicked(object sender, RoutedEventArgs e)
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
        }

        // When Click Settings Button
        private void settings_clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings OP = new Settings();
                OP.Show();
            }
            catch
            {

            }
        }


        // Find How Many Cameras is connected and open the write UI
        public void createVideosPage()
        {
            // Cameras Names, URLS
            var names_list = urls.Values.ToList();
            // Create Grid
            Camera_Container = new Grid();
            // One Camera
            if (urls_num == 1)
            {
                // Setup Grid
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
                Label title = new Label
                {
                    Content = names_list[0],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                Grid.SetRow(title, 0);
                Camera_Container.Children.Add(title);
                // Create Video Capture
                VideoCapture camera = new VideoCapture
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 5, 0, -804),
                    Width = 883,
                    Height = 800
                };
                camera.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                Grid.SetRow(camera, 1);
                // Add Camera to my Video Captures
                cameras_list.Add(camera);
                Camera_Container.Children.Add(camera);
                // Create The Panle Buttons
                StackPanel button_panel = new StackPanel
                {
                    Height = 58,
                    Width = 550,
                    Margin = new Thickness(662, 908, 664, 52),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Orientation = Orientation.Horizontal
                };
                main_grid.Children.Add(button_panel);
                // Create The Start Buttons
                Button start_button = new Button
                {
                    Content = "Start",
                    Width = 184
                };
                start_button.Click += (sender, args) =>
                {
                    start_clicked(sender, args);
                };
                button_panel.Children.Add(start_button);
                // Create Stop Button
                Button stop_button = new Button
                {
                    Content = "Stop",
                    Width = 184
                };
                stop_button.Click += (sender, args) =>
                {
                    stop_clicked(sender, args);
                };
                button_panel.Children.Add(stop_button);
                // Create Settings Button
                Button settings_button = new Button
                {
                    Content = "Settings",
                    Width = 184
                };
                settings_button.Click += (sender, args) =>
                {
                    settings_clicked(sender, args);
                };
                button_panel.Children.Add(settings_button);
            }
            // Tow Camera
            else if (urls_num == 2)
            {
                // Create tow row for the main grid
                RowDefinition mainrow_1 = new RowDefinition();
                RowDefinition mainrow_2 = new RowDefinition();
                mainrow_1.Height = new GridLength(869);
                main_grid.RowDefinitions.Add(mainrow_1);
                main_grid.RowDefinitions.Add(mainrow_2);
                // Setup Grid
                Grid.SetRow(Camera_Container, 0);
                main_grid.Children.Add(Camera_Container);
                // Grid Columns
                ColumnDefinition colum_1 = new ColumnDefinition();
                ColumnDefinition colum_2 = new ColumnDefinition();
                Camera_Container.ColumnDefinitions.Add(colum_1);
                Camera_Container.ColumnDefinitions.Add(colum_2);
                // Grid Rows
                RowDefinition row_1 = new RowDefinition();
                RowDefinition row_2 = new RowDefinition();
                row_1.Height = new GridLength(50);
                row_2.Height = new GridLength(500, GridUnitType.Star);
                Camera_Container.RowDefinitions.Add(row_1);
                Camera_Container.RowDefinitions.Add(row_2);
                // Create Title Label 1
                Label title_1 = new Label
                {
                    Content = names_list[0],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_1, 0);
                Grid.SetColumn(title_1, 0);
                Camera_Container.Children.Add(title_1);
                // Create Title Label 2
                Label title_2 = new Label
                {
                    Content = names_list[1],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_2, 0);
                Grid.SetColumn(title_2, 1);
                Camera_Container.Children.Add(title_2);
                // Create Video Capture 1
                VideoCapture camera_1 = new VideoCapture
                {
                    Width = 900
                };
                camera_1.MouseUp += (object sender, MouseButtonEventArgs e) =>
                 {
                     camerasFocused(sender, e);
                     Console.WriteLine("Cmera 1 Mouse Event");
                 };
                cameras_list.Add(camera_1);
                Grid.SetRow(camera_1, 1);
                Grid.SetColumn(camera_1, 0);
                Camera_Container.Children.Add(camera_1);
                // Create Video Capture 2
                VideoCapture camera_2 = new VideoCapture
                {
                    Width = 900
                };
                camera_2.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                    Console.WriteLine("Cmera 2 Mouse Event");
                };
                cameras_list.Add(camera_2);
                Grid.SetRow(camera_2, 1);
                Grid.SetColumn(camera_2, 1);
                Camera_Container.Children.Add(camera_2);
                // Create The Panle Buttons
                StackPanel button_panel = new StackPanel
                {
                    Height = 58,
                    Width = 550,
                    Margin = new Thickness(659, 31, 667, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Horizontal
                };
                Grid.SetRow(button_panel, 1);
                main_grid.Children.Add(button_panel);
                // Create The Start Buttons
                Button start_button = new Button
                {
                    Content = "Start",
                    Width = 184
                };
                start_button.Click += (sender, args) =>
                {
                    start_clicked(sender, args);
                };
                button_panel.Children.Add(start_button);
                // Create Stop Button
                Button stop_button = new Button
                {
                    Content = "Stop",
                    Width = 184
                };
                stop_button.Click += (sender, args) =>
                {
                    stop_clicked(sender, args);
                };
                button_panel.Children.Add(stop_button);
                // Create Settings Button
                Button settings_button = new Button
                {
                    Content = "Settings",
                    Width = 184
                };
                settings_button.Click += (sender, args) =>
                {
                    settings_clicked(sender, args);
                };
                button_panel.Children.Add(settings_button);
            }
            // Three Camera
            else if (urls_num == 3)
            {
                // Create tow row for the main grid
                RowDefinition mainrow_1 = new RowDefinition();
                RowDefinition mainrow_2 = new RowDefinition();
                mainrow_1.Height = new GridLength(923);
                main_grid.RowDefinitions.Add(mainrow_1);
                main_grid.RowDefinitions.Add(mainrow_2);
                // Setup Grid
                Grid.SetRow(Camera_Container, 0);
                main_grid.Children.Add(Camera_Container);
                // Grid Columns
                ColumnDefinition colum_1 = new ColumnDefinition();
                ColumnDefinition colum_2 = new ColumnDefinition();
                Camera_Container.ColumnDefinitions.Add(colum_1);
                Camera_Container.ColumnDefinitions.Add(colum_2);
                // Grid Rows
                RowDefinition row_1 = new RowDefinition();
                RowDefinition row_2 = new RowDefinition();
                RowDefinition row_3 = new RowDefinition();
                RowDefinition row_4 = new RowDefinition();
                row_1.Height = new GridLength(40);
                row_2.Height = new GridLength(420);
                row_3.Height = new GridLength(40);
                row_4.Height = new GridLength(420);
                Camera_Container.RowDefinitions.Add(row_1);
                Camera_Container.RowDefinitions.Add(row_2);
                Camera_Container.RowDefinitions.Add(row_3);
                Camera_Container.RowDefinitions.Add(row_4);
                // Create Title Label 1
                Label title_1 = new Label
                {
                    Content = names_list[0],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_1, 0);
                Grid.SetColumn(title_1, 0);
                Camera_Container.Children.Add(title_1);
                // Create Title Label 2
                Label title_2 = new Label
                {
                    Content = names_list[1],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_2, 0);
                Grid.SetColumn(title_2, 1);
                Camera_Container.Children.Add(title_2);
                // Create Title Label 3
                Label title_3 = new Label
                {
                    Content = names_list[2],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_3, 2);
                Grid.SetColumn(title_3, 0);
                Camera_Container.Children.Add(title_3);

                // Create Video Capture 1
                VideoCapture camera_1 = new VideoCapture
                {
                    Width = 700
                };
                camera_1.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_1);
                Grid.SetRow(camera_1, 1);
                Grid.SetColumn(camera_1, 0);
                Camera_Container.Children.Add(camera_1);
                // Create Video Capture 2
                VideoCapture camera_2 = new VideoCapture
                {
                    Width = 700
                };
                camera_2.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_2);
                Grid.SetRow(camera_2, 1);
                Grid.SetColumn(camera_2, 1);
                Camera_Container.Children.Add(camera_2);
                // Create Video Capture 3
                VideoCapture camera_3 = new VideoCapture
                {
                    Width = 700
                };
                camera_3.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_3);
                Grid.SetRow(camera_3, 3);
                Grid.SetColumn(camera_3, 0);
                Camera_Container.Children.Add(camera_3);
                // Create The Panle Buttons
                StackPanel button_panel = new StackPanel
                {
                    Height = 58,
                    Width = 550,
                    Margin = new Thickness(659, 10, 667, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Horizontal
                };
                Grid.SetRow(button_panel, 1);
                main_grid.Children.Add(button_panel);
                // Create The Start Buttons
                Button start_button = new Button
                {
                    Content = "Start",
                    Width = 184
                };
                start_button.Click += (sender, args) =>
                {
                    start_clicked(sender, args);
                };
                button_panel.Children.Add(start_button);
                // Create Stop Button
                Button stop_button = new Button
                {
                    Content = "Stop",
                    Width = 184
                };
                stop_button.Click += (sender, args) =>
                {
                    stop_clicked(sender, args);
                };
                button_panel.Children.Add(stop_button);
                // Create Settings Button
                Button settings_button = new Button
                {
                    Content = "Settings",
                    Width = 184
                };
                settings_button.Click += (sender, args) =>
                {
                    settings_clicked(sender, args);
                };
                button_panel.Children.Add(settings_button);
            }
            // Four Camera
            else if (urls_num == 4)
            {
                // Create tow row for the main grid
                RowDefinition mainrow_1 = new RowDefinition();
                RowDefinition mainrow_2 = new RowDefinition();
                mainrow_1.Height = new GridLength(923);
                main_grid.RowDefinitions.Add(mainrow_1);
                main_grid.RowDefinitions.Add(mainrow_2);
                // Setup Grid
                Grid.SetRow(Camera_Container, 0);
                main_grid.Children.Add(Camera_Container);
                // Grid Columns
                ColumnDefinition colum_1 = new ColumnDefinition();
                ColumnDefinition colum_2 = new ColumnDefinition();
                Camera_Container.ColumnDefinitions.Add(colum_1);
                Camera_Container.ColumnDefinitions.Add(colum_2);
                // Grid Rows
                RowDefinition row_1 = new RowDefinition();
                RowDefinition row_2 = new RowDefinition();
                RowDefinition row_3 = new RowDefinition();
                RowDefinition row_4 = new RowDefinition();
                row_1.Height = new GridLength(40);
                row_2.Height = new GridLength(420);
                row_3.Height = new GridLength(40);
                row_4.Height = new GridLength(420);
                Camera_Container.RowDefinitions.Add(row_1);
                Camera_Container.RowDefinitions.Add(row_2);
                Camera_Container.RowDefinitions.Add(row_3);
                Camera_Container.RowDefinitions.Add(row_4);
                // Create Title Label 1
                Label title_1 = new Label
                {
                    Content = names_list[0],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_1, 0);
                Grid.SetColumn(title_1, 0);
                Camera_Container.Children.Add(title_1);
                // Create Title Label 2
                Label title_2 = new Label
                {
                    Content = names_list[1],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_2, 0);
                Grid.SetColumn(title_2, 1);
                Camera_Container.Children.Add(title_2);
                // Create Title Label 3
                Label title_3 = new Label
                {
                    Content = names_list[2],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_3, 2);
                Grid.SetColumn(title_3, 0);
                Camera_Container.Children.Add(title_3);
                // Create Title Label 4
                Label title_4 = new Label
                {
                    Content = names_list[3],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_4, 2);
                Grid.SetColumn(title_4, 1);
                Camera_Container.Children.Add(title_4);
                // Create Video Capture 1
                VideoCapture camera_1 = new VideoCapture
                {
                    Width = 700
                };
                camera_1.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_1);
                Grid.SetRow(camera_1, 1);
                Grid.SetColumn(camera_1, 0);
                Camera_Container.Children.Add(camera_1);
                // Create Video Capture 2
                VideoCapture camera_2 = new VideoCapture
                {
                    Width = 700
                };
                camera_2.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_2);
                Grid.SetRow(camera_2, 1);
                Grid.SetColumn(camera_2, 1);
                Camera_Container.Children.Add(camera_2);
                // Create Video Capture 3
                VideoCapture camera_3 = new VideoCapture
                {
                    Width = 700
                };
                camera_3.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_3);
                Grid.SetRow(camera_3, 3);
                Grid.SetColumn(camera_3, 0);
                Camera_Container.Children.Add(camera_3);
                // Create Video Capture 4
                VideoCapture camera_4 = new VideoCapture
                {
                    Width = 700
                };
                camera_4.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_4);
                Grid.SetRow(camera_4, 3);
                Grid.SetColumn(camera_4, 1);
                Camera_Container.Children.Add(camera_4);
                // Create The Panle Buttons
                StackPanel button_panel = new StackPanel
                {
                    Height = 58,
                    Width = 550,
                    Margin = new Thickness(659, 10, 667, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Horizontal
                };
                Grid.SetRow(button_panel, 1);
                main_grid.Children.Add(button_panel);
                // Create The Start Buttons
                Button start_button = new Button
                {
                    Content = "Start",
                    Width = 184
                };
                start_button.Click += (sender, args) =>
                {
                    start_clicked(sender, args);
                };
                button_panel.Children.Add(start_button);
                // Create Stop Button
                Button stop_button = new Button
                {
                    Content = "Stop",
                    Width = 184
                };
                stop_button.Click += (sender, args) =>
                {
                    stop_clicked(sender, args);
                };
                button_panel.Children.Add(stop_button);
                // Create Settings Button
                Button settings_button = new Button
                {
                    Content = "Settings",
                    Width = 184
                };
                settings_button.Click += (sender, args) =>
                {
                    settings_clicked(sender, args);
                };
                button_panel.Children.Add(settings_button);
            }
            // Five Camera
            else if (urls_num == 5)
            {
                // Create tow row for the main grid
                RowDefinition mainrow_1 = new RowDefinition();
                RowDefinition mainrow_2 = new RowDefinition();
                mainrow_1.Height = new GridLength(923);
                main_grid.RowDefinitions.Add(mainrow_1);
                main_grid.RowDefinitions.Add(mainrow_2);
                // Setup Grid
                Grid.SetRow(Camera_Container, 0);
                main_grid.Children.Add(Camera_Container);
                // Grid Columns
                ColumnDefinition colum_1 = new ColumnDefinition();
                ColumnDefinition colum_2 = new ColumnDefinition();
                ColumnDefinition colum_3 = new ColumnDefinition();
                Camera_Container.ColumnDefinitions.Add(colum_1);
                Camera_Container.ColumnDefinitions.Add(colum_2);
                Camera_Container.ColumnDefinitions.Add(colum_3);
                // Grid Rows
                RowDefinition row_1 = new RowDefinition();
                RowDefinition row_2 = new RowDefinition();
                RowDefinition row_3 = new RowDefinition();
                RowDefinition row_4 = new RowDefinition();
                row_1.Height = new GridLength(40);
                row_2.Height = new GridLength(420);
                row_3.Height = new GridLength(40);
                row_4.Height = new GridLength(420);
                Camera_Container.RowDefinitions.Add(row_1);
                Camera_Container.RowDefinitions.Add(row_2);
                Camera_Container.RowDefinitions.Add(row_3);
                Camera_Container.RowDefinitions.Add(row_4);
                // Create Title Label 1
                Label title_1 = new Label
                {
                    Content = names_list[0],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_1, 0);
                Grid.SetColumn(title_1, 0);
                Camera_Container.Children.Add(title_1);
                // Create Title Label 2
                Label title_2 = new Label
                {
                    Content = names_list[1],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_2, 0);
                Grid.SetColumn(title_2, 1);
                Camera_Container.Children.Add(title_2);
                // Create Title Label 3
                Label title_3 = new Label
                {
                    Content = names_list[2],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_3, 2);
                Grid.SetColumn(title_3, 0);
                Camera_Container.Children.Add(title_3);
                // Create Title Label 4
                Label title_4 = new Label
                {
                    Content = names_list[3],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_4, 2);
                Grid.SetColumn(title_4, 1);
                Camera_Container.Children.Add(title_4);
                // Create Title Label 5
                Label title_5 = new Label
                {
                    Content = names_list[4],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_5, 0);
                Grid.SetColumn(title_5, 2);
                Camera_Container.Children.Add(title_5);
                // Create Video Capture 1
                VideoCapture camera_1 = new VideoCapture
                {
                    Width = 500
                };
                camera_1.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_1);
                Grid.SetRow(camera_1, 1);
                Grid.SetColumn(camera_1, 0);
                Camera_Container.Children.Add(camera_1);
                // Create Video Capture 2
                VideoCapture camera_2 = new VideoCapture
                {
                    Width = 500
                };
                camera_2.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_2);
                Grid.SetRow(camera_2, 1);
                Grid.SetColumn(camera_2, 1);
                Camera_Container.Children.Add(camera_2);
                // Create Video Capture 3
                VideoCapture camera_3 = new VideoCapture
                {
                    Width = 500
                };
                camera_3.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_3);
                Grid.SetRow(camera_3, 3);
                Grid.SetColumn(camera_3, 0);
                Camera_Container.Children.Add(camera_3);
                // Create Video Capture 4
                VideoCapture camera_4 = new VideoCapture
                {
                    Width = 500
                };
                camera_4.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_4);
                Grid.SetRow(camera_4, 3);
                Grid.SetColumn(camera_4, 1);
                Camera_Container.Children.Add(camera_4);
                // Create Video Capture 5
                VideoCapture camera_5 = new VideoCapture
                {
                    Width = 500
                };
                camera_5.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_5);
                Grid.SetRow(camera_5, 1);
                Grid.SetColumn(camera_5, 2);
                Camera_Container.Children.Add(camera_5);
                // Create The Panle Buttons
                StackPanel button_panel = new StackPanel
                {
                    Height = 58,
                    Width = 550,
                    Margin = new Thickness(659, 10, 667, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Horizontal
                };
                Grid.SetRow(button_panel, 1);
                main_grid.Children.Add(button_panel);
                // Create The Start Buttons
                Button start_button = new Button
                {
                    Content = "Start",
                    Width = 184
                };
                start_button.Click += (sender, args) =>
                {
                    start_clicked(sender, args);
                };
                button_panel.Children.Add(start_button);
                // Create Stop Button
                Button stop_button = new Button
                {
                    Content = "Stop",
                    Width = 184
                };
                stop_button.Click += (sender, args) =>
                {
                    stop_clicked(sender, args);
                };
                button_panel.Children.Add(stop_button);
                // Create Settings Button
                Button settings_button = new Button
                {
                    Content = "Settings",
                    Width = 184
                };
                settings_button.Click += (sender, args) =>
                {
                    settings_clicked(sender, args);
                };
                button_panel.Children.Add(settings_button);
            }
            // Six Camera
            else if (urls_num == 6)
            {
                // Create tow row for the main grid
                RowDefinition mainrow_1 = new RowDefinition();
                RowDefinition mainrow_2 = new RowDefinition();
                mainrow_1.Height = new GridLength(923);
                main_grid.RowDefinitions.Add(mainrow_1);
                main_grid.RowDefinitions.Add(mainrow_2);
                // Setup Grid
                Grid.SetRow(Camera_Container, 0);
                main_grid.Children.Add(Camera_Container);
                // Grid Columns
                ColumnDefinition colum_1 = new ColumnDefinition();
                ColumnDefinition colum_2 = new ColumnDefinition();
                ColumnDefinition colum_3 = new ColumnDefinition();
                Camera_Container.ColumnDefinitions.Add(colum_1);
                Camera_Container.ColumnDefinitions.Add(colum_2);
                Camera_Container.ColumnDefinitions.Add(colum_3);
                // Grid Rows
                RowDefinition row_1 = new RowDefinition();
                RowDefinition row_2 = new RowDefinition();
                RowDefinition row_3 = new RowDefinition();
                RowDefinition row_4 = new RowDefinition();
                row_1.Height = new GridLength(40);
                row_2.Height = new GridLength(420);
                row_3.Height = new GridLength(40);
                row_4.Height = new GridLength(420);
                Camera_Container.RowDefinitions.Add(row_1);
                Camera_Container.RowDefinitions.Add(row_2);
                Camera_Container.RowDefinitions.Add(row_3);
                Camera_Container.RowDefinitions.Add(row_4);
                // Create Title Label 1
                Label title_1 = new Label
                {
                    Content = names_list[0],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_1, 0);
                Grid.SetColumn(title_1, 0);
                Camera_Container.Children.Add(title_1);
                // Create Title Label 2
                Label title_2 = new Label
                {
                    Content = names_list[1],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_2, 0);
                Grid.SetColumn(title_2, 1);
                Camera_Container.Children.Add(title_2);
                // Create Title Label 3
                Label title_3 = new Label
                {
                    Content = names_list[2],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_3, 2);
                Grid.SetColumn(title_3, 0);
                Camera_Container.Children.Add(title_3);
                // Create Title Label 4
                Label title_4 = new Label
                {
                    Content = names_list[3],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_4, 2);
                Grid.SetColumn(title_4, 1);
                Camera_Container.Children.Add(title_4);
                // Create Title Label 5
                Label title_5 = new Label
                {
                    Content = names_list[4],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_5, 0);
                Grid.SetColumn(title_5, 2);
                Camera_Container.Children.Add(title_5);
                // Create Title Label 6
                Label title_6 = new Label
                {
                    Content = names_list[5],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_6, 2);
                Grid.SetColumn(title_6, 2);
                Camera_Container.Children.Add(title_6);
                // Create Video Capture 1
                VideoCapture camera_1 = new VideoCapture
                {
                    Width = 500
                };
                camera_1.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_1);
                Grid.SetRow(camera_1, 1);
                Grid.SetColumn(camera_1, 0);
                Camera_Container.Children.Add(camera_1);
                // Create Video Capture 2
                VideoCapture camera_2 = new VideoCapture
                {
                    Width = 500
                };
                camera_2.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_2);
                Grid.SetRow(camera_2, 1);
                Grid.SetColumn(camera_2, 1);
                Camera_Container.Children.Add(camera_2);
                // Create Video Capture 3
                VideoCapture camera_3 = new VideoCapture
                {
                    Width = 500
                };
                camera_3.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_3);
                Grid.SetRow(camera_3, 3);
                Grid.SetColumn(camera_3, 0);
                Camera_Container.Children.Add(camera_3);
                // Create Video Capture 4
                VideoCapture camera_4 = new VideoCapture
                {
                    Width = 500
                };
                camera_4.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_4);
                Grid.SetRow(camera_4, 3);
                Grid.SetColumn(camera_4, 1);
                Camera_Container.Children.Add(camera_4);
                // Create Video Capture 5
                VideoCapture camera_5 = new VideoCapture
                {
                    Width = 500
                };
                camera_5.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_5);
                Grid.SetRow(camera_5, 1);
                Grid.SetColumn(camera_5, 2);
                Camera_Container.Children.Add(camera_5);
                // Create Video Capture 6
                VideoCapture camera_6 = new VideoCapture
                {
                    Width = 500
                };
                camera_6.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_6);
                Grid.SetRow(camera_6, 3);
                Grid.SetColumn(camera_6, 2);
                Camera_Container.Children.Add(camera_6);
                // Create The Panle Buttons
                StackPanel button_panel = new StackPanel
                {
                    Height = 58,
                    Width = 550,
                    Margin = new Thickness(659, 10, 667, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Horizontal
                };
                Grid.SetRow(button_panel, 1);
                main_grid.Children.Add(button_panel);
                // Create The Start Buttons
                Button start_button = new Button
                {
                    Content = "Start",
                    Width = 184
                };
                start_button.Click += (sender, args) =>
                {
                    start_clicked(sender, args);
                };
                button_panel.Children.Add(start_button);
                // Create Stop Button
                Button stop_button = new Button
                {
                    Content = "Stop",
                    Width = 184
                };
                stop_button.Click += (sender, args) =>
                {
                    stop_clicked(sender, args);
                };
                button_panel.Children.Add(stop_button);
                // Create Settings Button
                Button settings_button = new Button
                {
                    Content = "Settings",
                    Width = 184
                };
                settings_button.Click += (sender, args) =>
                {
                    settings_clicked(sender, args);
                };
                button_panel.Children.Add(settings_button);
            }
            // Seven Camera
            else if (urls_num == 7)
            {
                // Create tow row for the main grid
                RowDefinition mainrow_1 = new RowDefinition();
                RowDefinition mainrow_2 = new RowDefinition();
                mainrow_1.Height = new GridLength(923);
                main_grid.RowDefinitions.Add(mainrow_1);
                main_grid.RowDefinitions.Add(mainrow_2);
                // Setup Grid
                Grid.SetRow(Camera_Container, 0);
                main_grid.Children.Add(Camera_Container);
                // Grid Columns
                ColumnDefinition colum_1 = new ColumnDefinition();
                ColumnDefinition colum_2 = new ColumnDefinition();
                ColumnDefinition colum_3 = new ColumnDefinition();
                ColumnDefinition colum_4 = new ColumnDefinition();
                Camera_Container.ColumnDefinitions.Add(colum_1);
                Camera_Container.ColumnDefinitions.Add(colum_2);
                Camera_Container.ColumnDefinitions.Add(colum_3);
                Camera_Container.ColumnDefinitions.Add(colum_4);
                // Grid Rows
                RowDefinition row_1 = new RowDefinition();
                RowDefinition row_2 = new RowDefinition();
                RowDefinition row_3 = new RowDefinition();
                RowDefinition row_4 = new RowDefinition();
                row_1.Height = new GridLength(40);
                row_2.Height = new GridLength(420);
                row_3.Height = new GridLength(40);
                row_4.Height = new GridLength(420);
                Camera_Container.RowDefinitions.Add(row_1);
                Camera_Container.RowDefinitions.Add(row_2);
                Camera_Container.RowDefinitions.Add(row_3);
                Camera_Container.RowDefinitions.Add(row_4);
                // Create Title Label 1
                Label title_1 = new Label
                {
                    Content = names_list[0],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_1, 0);
                Grid.SetColumn(title_1, 0);
                Camera_Container.Children.Add(title_1);
                // Create Title Label 2
                Label title_2 = new Label
                {
                    Content = names_list[1],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_2, 0);
                Grid.SetColumn(title_2, 1);
                Camera_Container.Children.Add(title_2);
                // Create Title Label 3
                Label title_3 = new Label
                {
                    Content = names_list[2],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_3, 2);
                Grid.SetColumn(title_3, 0);
                Camera_Container.Children.Add(title_3);
                // Create Title Label 4
                Label title_4 = new Label
                {
                    Content = names_list[3],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_4, 2);
                Grid.SetColumn(title_4, 1);
                Camera_Container.Children.Add(title_4);
                // Create Title Label 5
                Label title_5 = new Label
                {
                    Content = names_list[4],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_5, 0);
                Grid.SetColumn(title_5, 2);
                Camera_Container.Children.Add(title_5);
                // Create Title Label 6
                Label title_6 = new Label
                {
                    Content = names_list[5],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_6, 2);
                Grid.SetColumn(title_6, 2);
                Camera_Container.Children.Add(title_6);
                // Create Title Label 7
                Label title_7 = new Label
                {
                    Content = names_list[6],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_7, 0);
                Grid.SetColumn(title_7, 3);
                Camera_Container.Children.Add(title_7);
                // Create Video Capture 1
                VideoCapture camera_1 = new VideoCapture
                {
                    Width = 400
                };
                camera_1.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_1);
                Grid.SetRow(camera_1, 1);
                Grid.SetColumn(camera_1, 0);
                Camera_Container.Children.Add(camera_1);
                // Create Video Capture 2
                VideoCapture camera_2 = new VideoCapture
                {
                    Width = 400
                };
                camera_2.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_2);
                Grid.SetRow(camera_2, 1);
                Grid.SetColumn(camera_2, 1);
                Camera_Container.Children.Add(camera_2);
                // Create Video Capture 3
                VideoCapture camera_3 = new VideoCapture
                {
                    Width = 400
                };
                camera_3.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_3);
                Grid.SetRow(camera_3, 3);
                Grid.SetColumn(camera_3, 0);
                Camera_Container.Children.Add(camera_3);
                // Create Video Capture 4
                VideoCapture camera_4 = new VideoCapture
                {
                    Width = 400
                };
                camera_4.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_4);
                Grid.SetRow(camera_4, 3);
                Grid.SetColumn(camera_4, 1);
                Camera_Container.Children.Add(camera_4);
                // Create Video Capture 5
                VideoCapture camera_5 = new VideoCapture
                {
                    Width = 400
                };
                camera_5.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_5);
                Grid.SetRow(camera_5, 1);
                Grid.SetColumn(camera_5, 2);
                Camera_Container.Children.Add(camera_5);
                // Create Video Capture 6
                VideoCapture camera_6 = new VideoCapture
                {
                    Width = 400
                };
                camera_6.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_6);
                Grid.SetRow(camera_6, 3);
                Grid.SetColumn(camera_6, 2);
                Camera_Container.Children.Add(camera_6);
                // Create Video Capture 7
                VideoCapture camera_7 = new VideoCapture
                {
                    Width = 400
                };
                camera_7.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_7);
                Grid.SetRow(camera_7, 1);
                Grid.SetColumn(camera_7, 3);
                Camera_Container.Children.Add(camera_7);
                // Create The Panle Buttons
                StackPanel button_panel = new StackPanel
                {
                    Height = 58,
                    Width = 550,
                    Margin = new Thickness(659, 10, 667, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Horizontal
                };
                Grid.SetRow(button_panel, 1);
                main_grid.Children.Add(button_panel);
                // Create The Start Buttons
                Button start_button = new Button
                {
                    Content = "Start",
                    Width = 184
                };
                start_button.Click += (sender, args) =>
                {
                    start_clicked(sender, args);
                };
                button_panel.Children.Add(start_button);
                // Create Stop Button
                Button stop_button = new Button
                {
                    Content = "Stop",
                    Width = 184
                };
                stop_button.Click += (sender, args) =>
                {
                    stop_clicked(sender, args);
                };
                button_panel.Children.Add(stop_button);
                // Create Settings Button
                Button settings_button = new Button
                {
                    Content = "Settings",
                    Width = 184
                };
                settings_button.Click += (sender, args) =>
                {
                    settings_clicked(sender, args);
                };
                button_panel.Children.Add(settings_button);
            }
            // Eight Camera
            else if (urls_num == 8)
            {
                // Create tow row for the main grid
                RowDefinition mainrow_1 = new RowDefinition();
                RowDefinition mainrow_2 = new RowDefinition();
                mainrow_1.Height = new GridLength(923);
                main_grid.RowDefinitions.Add(mainrow_1);
                main_grid.RowDefinitions.Add(mainrow_2);
                // Setup Grid
                Grid.SetRow(Camera_Container, 0);
                main_grid.Children.Add(Camera_Container);
                // Grid Columns
                ColumnDefinition colum_1 = new ColumnDefinition();
                ColumnDefinition colum_2 = new ColumnDefinition();
                ColumnDefinition colum_3 = new ColumnDefinition();
                ColumnDefinition colum_4 = new ColumnDefinition();
                Camera_Container.ColumnDefinitions.Add(colum_1);
                Camera_Container.ColumnDefinitions.Add(colum_2);
                Camera_Container.ColumnDefinitions.Add(colum_3);
                Camera_Container.ColumnDefinitions.Add(colum_4);
                // Grid Rows
                RowDefinition row_1 = new RowDefinition();
                RowDefinition row_2 = new RowDefinition();
                RowDefinition row_3 = new RowDefinition();
                RowDefinition row_4 = new RowDefinition();
                row_1.Height = new GridLength(40);
                row_2.Height = new GridLength(420);
                row_3.Height = new GridLength(40);
                row_4.Height = new GridLength(420);
                Camera_Container.RowDefinitions.Add(row_1);
                Camera_Container.RowDefinitions.Add(row_2);
                Camera_Container.RowDefinitions.Add(row_3);
                Camera_Container.RowDefinitions.Add(row_4);
                // Create Title Label 1
                Label title_1 = new Label
                {
                    Content = names_list[0],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_1, 0);
                Grid.SetColumn(title_1, 0);
                Camera_Container.Children.Add(title_1);
                // Create Title Label 2
                Label title_2 = new Label
                {
                    Content = names_list[1],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_2, 0);
                Grid.SetColumn(title_2, 1);
                Camera_Container.Children.Add(title_2);
                // Create Title Label 3
                Label title_3 = new Label
                {
                    Content = names_list[2],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_3, 2);
                Grid.SetColumn(title_3, 0);
                Camera_Container.Children.Add(title_3);
                // Create Title Label 4
                Label title_4 = new Label
                {
                    Content = names_list[3],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_4, 2);
                Grid.SetColumn(title_4, 1);
                Camera_Container.Children.Add(title_4);
                // Create Title Label 5
                Label title_5 = new Label
                {
                    Content = names_list[4],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_5, 0);
                Grid.SetColumn(title_5, 2);
                Camera_Container.Children.Add(title_5);
                // Create Title Label 6
                Label title_6 = new Label
                {
                    Content = names_list[5],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_6, 2);
                Grid.SetColumn(title_6, 2);
                Camera_Container.Children.Add(title_6);
                // Create Title Label 7
                Label title_7 = new Label
                {
                    Content = names_list[6],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_7, 0);
                Grid.SetColumn(title_7, 3);
                Camera_Container.Children.Add(title_7);
                // Create Title Label 8
                Label title_8 = new Label
                {
                    Content = names_list[7],
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_8, 2);
                Grid.SetColumn(title_8, 3);
                Camera_Container.Children.Add(title_8);
                // Create Video Capture 1
                VideoCapture camera_1 = new VideoCapture
                {
                    Width = 400
                };
                camera_1.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_1);
                Grid.SetRow(camera_1, 1);
                Grid.SetColumn(camera_1, 0);
                Camera_Container.Children.Add(camera_1);
                // Create Video Capture 2
                VideoCapture camera_2 = new VideoCapture
                {
                    Width = 400
                };
                camera_2.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_2);
                Grid.SetRow(camera_2, 1);
                Grid.SetColumn(camera_2, 1);
                Camera_Container.Children.Add(camera_2);
                // Create Video Capture 3
                VideoCapture camera_3 = new VideoCapture
                {
                    Width = 400
                };
                camera_3.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_3);
                Grid.SetRow(camera_3, 3);
                Grid.SetColumn(camera_3, 0);
                Camera_Container.Children.Add(camera_3);
                // Create Video Capture 4
                VideoCapture camera_4 = new VideoCapture
                {
                    Width = 400
                };
                camera_4.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_4);
                Grid.SetRow(camera_4, 3);
                Grid.SetColumn(camera_4, 1);
                Camera_Container.Children.Add(camera_4);
                // Create Video Capture 5
                VideoCapture camera_5 = new VideoCapture
                {
                    Width = 400
                };
                camera_5.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_5);
                Grid.SetRow(camera_5, 1);
                Grid.SetColumn(camera_5, 2);
                Camera_Container.Children.Add(camera_5);
                // Create Video Capture 6
                VideoCapture camera_6 = new VideoCapture
                {
                    Width = 400
                };
                camera_6.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_6);
                Grid.SetRow(camera_6, 3);
                Grid.SetColumn(camera_6, 2);
                Camera_Container.Children.Add(camera_6);
                // Create Video Capture 7
                VideoCapture camera_7 = new VideoCapture
                {
                    Width = 400
                };
                camera_7.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_7);
                Grid.SetRow(camera_7, 1);
                Grid.SetColumn(camera_7, 3);
                Camera_Container.Children.Add(camera_7);
                // Create Video Capture 8
                VideoCapture camera_8 = new VideoCapture
                {
                    Width = 400
                };
                camera_8.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras_list.Add(camera_8);
                Grid.SetRow(camera_8, 3);
                Grid.SetColumn(camera_8, 3);
                Camera_Container.Children.Add(camera_8);
                // Create The Panle Buttons
                StackPanel button_panel = new StackPanel
                {
                    Height = 58,
                    Width = 550,
                    Margin = new Thickness(659, 10, 667, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Horizontal
                };
                Grid.SetRow(button_panel, 1);
                main_grid.Children.Add(button_panel);
                // Create The Start Buttons
                Button start_button = new Button
                {
                    Content = "Start",
                    Width = 184
                };
                start_button.Click += (sender, args) =>
                {
                    start_clicked(sender, args);
                };
                button_panel.Children.Add(start_button);
                // Create Stop Button
                Button stop_button = new Button
                {
                    Content = "Stop",
                    Width = 184
                };
                stop_button.Click += (sender, args) =>
                {
                    stop_clicked(sender, args);
                };
                button_panel.Children.Add(stop_button);
                // Create Settings Button
                Button settings_button = new Button
                {
                    Content = "Settings",
                    Width = 184
                };
                settings_button.Click += (sender, args) =>
                {
                    settings_clicked(sender, args);
                };
                button_panel.Children.Add(settings_button);
            }
            else
            {
                Settings OP = new Settings();
                OP.Show();
            }

        }


    }


}
