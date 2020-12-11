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
//using VisioForge.Controls.UI.WinForms;


namespace IPCamera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static MainWindow main_window;
        private Grid Camera_Container;
        public static List<Camera> cameras = new List<Camera>();


        public MainWindow()
        {
            InitializeComponent();
            // Set a Hundeler for this main window
            main_window = this;
            // Update Urls From Database
            updatesFromDB();
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
        public void updatesFromDB()
        {
            // Save Data To Database
            using (SqlConnection connection = new SqlConnection(Camera.DB_connection_string))
            {
                String query = "SELECT id, urls, name, Face_Detection, Face_Recognition, " +
                    "Brightness, Contrast FROM dbo.MyCameras";
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
                        int brightness = (int)dataReader["Brightness"];
                        int contrast = (int)dataReader["Contrast"];
                        try
                        {
                            bool dec = (detection == "True"? true : false);
                            bool rec = (recognition == "True" ? true : false);
                            Camera cam = new Camera(url, name, id, dec, rec);
                            cam.Brightness = brightness;
                            cam.Contrast = contrast;
                            cameras.Add(cam);
                        }
                        catch (System.ArgumentException)
                        {

                        }
                    }
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
                    foreach (Camera  cam in cameras)
                    {
                        if (cam.video.Equals(camera))
                        {
                            WindowControll win_controll = new WindowControll(cam);
                            win_controll.Show();
                        }
                    }
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
            try
            {
                foreach (Camera cam in cameras)
                {
                    cam.start();
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
                foreach (Camera cam in cameras)
                {
                    cam.stop();
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



        // On Error EVnt
        private void Video_OnError(object sender, VisioForge.Types.ErrorsEventArgs e)
        {
            System.Windows.MessageBox.Show("Camera not found!");
            //throw new NotImplementedException();
        }


        // Find How Many Cameras is connected and open the write UI
        public void createVideosPage()
        {
            // Create Grid
            Camera_Container = new Grid();
            // One Camera
            if (Camera.count == 1)
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
                    Content = cameras[0].name,
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                Grid.SetRow(title, 0);
                Camera_Container.Children.Add(title);
                // Setup the Camera
                cameras[0].video.HorizontalAlignment = HorizontalAlignment.Center;
                cameras[0].video.Margin = new Thickness(0, 5, 0, -804);
                cameras[0].video.Width = 883;
                cameras[0].video.Height = 800;
                cameras[0].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[0].video.OnError += Video_OnError;
                Grid.SetRow(cameras[0].video, 1);
                Camera_Container.Children.Add(cameras[0].video);
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
            else if (Camera.count == 2)
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
                    Content = cameras[0].name,
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
                    Content = cameras[1].name,
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_2, 0);
                Grid.SetColumn(title_2, 1);
                Camera_Container.Children.Add(title_2);
                // Setup the Camera
                cameras[0].video.Width = 900;
                cameras[0].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                 {
                     camerasFocused(sender, e);
                     Console.WriteLine("Cmera 1 Mouse Event");
                 };
                cameras[0].video.OnError += Video_OnError;
                Grid.SetRow(cameras[0].video, 1);
                Grid.SetColumn(cameras[0].video, 0);
                Camera_Container.Children.Add(cameras[0].video);
                // Setup the Camera
                cameras[1].video.Width = 900;
                cameras[1].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                    Console.WriteLine("Cmera 2 Mouse Event");
                };
                cameras[1].video.OnError += Video_OnError;
                Grid.SetRow(cameras[1].video, 1);
                Grid.SetColumn(cameras[1].video, 1);
                Camera_Container.Children.Add(cameras[1].video);
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
            else if (Camera.count == 3)
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
                    Content = cameras[0].name,
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
                    Content = cameras[1].name,
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
                    Content = cameras[2].name,
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_3, 2);
                Grid.SetColumn(title_3, 0);
                Camera_Container.Children.Add(title_3);
                // Setup the Camera
                cameras[0].video.Width = 700;
                cameras[0].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[0].video.OnError += Video_OnError;
                Grid.SetRow(cameras[0].video, 1);
                Grid.SetColumn(cameras[0].video, 0);
                Camera_Container.Children.Add(cameras[0].video);
                // Setup the Camera
                cameras[1].video.Width = 700;
                cameras[1].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[1].video.OnError += Video_OnError;
                Grid.SetRow(cameras[1].video, 1);
                Grid.SetColumn(cameras[1].video, 1);
                Camera_Container.Children.Add(cameras[1].video);
                // Setup the Camera
                cameras[2].video.Width = 700;
                cameras[2].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[2].video.OnError += Video_OnError;
                Grid.SetRow(cameras[2].video, 3);
                Grid.SetColumn(cameras[2].video, 0);
                Camera_Container.Children.Add(cameras[2].video);
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
            else if (Camera.count == 4)
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
                    Content = cameras[0].name,
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
                    Content = cameras[1].name,
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
                    Content = cameras[2].name,
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
                    Content = cameras[3].name,
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_4, 2);
                Grid.SetColumn(title_4, 1);
                Camera_Container.Children.Add(title_4);
                // Setup the Camera
                cameras[0].video.Width = 700;
                cameras[0].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[0].video.OnError += Video_OnError;
                Grid.SetRow(cameras[0].video, 1);
                Grid.SetColumn(cameras[0].video, 0);
                Camera_Container.Children.Add(cameras[0].video);
                // Setup the Camera
                cameras[1].video.Width = 700;
                cameras[1].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[1].video.OnError += Video_OnError;
                Grid.SetRow(cameras[1].video, 1);
                Grid.SetColumn(cameras[1].video, 1);
                Camera_Container.Children.Add(cameras[1].video);
                // Setup the Camera
                cameras[2].video.Width = 700;
                cameras[2].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[2].video.OnError += Video_OnError;
                Grid.SetRow(cameras[2].video, 3);
                Grid.SetColumn(cameras[2].video, 0);
                Camera_Container.Children.Add(cameras[2].video);
                // Setup the Camera
                cameras[3].video.Width = 700;
                cameras[3].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[3].video.OnError += Video_OnError;
                Grid.SetRow(cameras[3].video, 3);
                Grid.SetColumn(cameras[3].video, 1);
                Camera_Container.Children.Add(cameras[3].video);
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
            else if (Camera.count == 5)
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
                    Content = cameras[0].name,
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
                    Content = cameras[1].name,
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
                    Content = cameras[2].name,
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
                    Content = cameras[3].name,
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
                    Content = cameras[4].name,
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_5, 0);
                Grid.SetColumn(title_5, 2);
                Camera_Container.Children.Add(title_5);
                // Setup the Camera
                cameras[0].video.Width = 500;
                cameras[0].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[0].video.OnError += Video_OnError;
                Grid.SetRow(cameras[0].video, 1);
                Grid.SetColumn(cameras[0].video, 0);
                Camera_Container.Children.Add(cameras[0].video);
                // Setup the Camera
                cameras[1].video.Width = 500;
                cameras[1].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[1].video.OnError += Video_OnError;
                Grid.SetRow(cameras[1].video, 1);
                Grid.SetColumn(cameras[1].video, 1);
                Camera_Container.Children.Add(cameras[1].video);
                // Setup the Camera
                cameras[2].video.Width = 500;
                cameras[2].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[2].video.OnError += Video_OnError;
                Grid.SetRow(cameras[2].video, 3);
                Grid.SetColumn(cameras[2].video, 0);
                Camera_Container.Children.Add(cameras[2].video);
                // Setup the Camera
                cameras[3].video.Width = 500;
                cameras[3].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[3].video.OnError += Video_OnError;
                Grid.SetRow(cameras[3].video, 3);
                Grid.SetColumn(cameras[3].video, 1);
                Camera_Container.Children.Add(cameras[3].video);
                // Setup the Camera
                cameras[4].video.Width = 500;
                cameras[4].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[4].video.OnError += Video_OnError;
                Grid.SetRow(cameras[4].video, 1);
                Grid.SetColumn(cameras[4].video, 2);
                Camera_Container.Children.Add(cameras[4].video);
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
            else if (Camera.count == 6)
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
                    Content = cameras[0].name,
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
                    Content = cameras[1].name,
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
                    Content = cameras[2].name,
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
                    Content = cameras[3].name,
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
                    Content = cameras[4].name,
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
                    Content = cameras[5].name,
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_6, 2);
                Grid.SetColumn(title_6, 2);
                Camera_Container.Children.Add(title_6);
                // Setup the Camera
                cameras[0].video.Width = 500;
                cameras[0].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[0].video.OnError += Video_OnError;
                Grid.SetRow(cameras[0].video, 1);
                Grid.SetColumn(cameras[0].video, 0);
                Camera_Container.Children.Add(cameras[0].video);
                // Setup the Camera
                cameras[1].video.Width = 500;
                cameras[1].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[1].video.OnError += Video_OnError;
                Grid.SetRow(cameras[1].video, 1);
                Grid.SetColumn(cameras[1].video, 1);
                Camera_Container.Children.Add(cameras[1].video);
                // Setup the Camera
                cameras[2].video.Width = 500;
                cameras[2].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[2].video.OnError += Video_OnError;
                Grid.SetRow(cameras[2].video, 3);
                Grid.SetColumn(cameras[2].video, 0);
                Camera_Container.Children.Add(cameras[2].video);
                // Setup the Camera
                cameras[3].video.Width = 500;
                cameras[3].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[3].video.OnError += Video_OnError;
                Grid.SetRow(cameras[3].video, 3);
                Grid.SetColumn(cameras[3].video, 1);
                Camera_Container.Children.Add(cameras[3].video);
                // Setup the Camera
                cameras[4].video.Width = 500;
                cameras[4].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[4].video.OnError += Video_OnError;
                Grid.SetRow(cameras[4].video, 1);
                Grid.SetColumn(cameras[4].video, 2);
                Camera_Container.Children.Add(cameras[4].video);
                // Setup the Camera
                cameras[5].video.Width = 500;
                cameras[5].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[5].video.OnError += Video_OnError;
                Grid.SetRow(cameras[5].video, 3);
                Grid.SetColumn(cameras[5].video, 2);
                Camera_Container.Children.Add(cameras[5].video);
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
            else if (Camera.count == 7)
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
                    Content = cameras[0].name,
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
                    Content = cameras[1].name,
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
                    Content = cameras[2].name,
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
                    Content = cameras[3].name,
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
                    Content = cameras[4].name,
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
                    Content = cameras[5].name,
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
                    Content = cameras[6].name,
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_7, 0);
                Grid.SetColumn(title_7, 3);
                Camera_Container.Children.Add(title_7);
                // Setup the Camera
                cameras[0].video.Width = 400;
                cameras[0].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[0].video.OnError += Video_OnError;
                Grid.SetRow(cameras[0].video, 1);
                Grid.SetColumn(cameras[0].video, 0);
                Camera_Container.Children.Add(cameras[0].video);
                // Setup the Camera
                cameras[1].video.Width = 400;
                cameras[1].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[1].video.OnError += Video_OnError;
                Grid.SetRow(cameras[1].video, 1);
                Grid.SetColumn(cameras[1].video, 1);
                Camera_Container.Children.Add(cameras[1].video);
                // Setup the Camera
                cameras[2].video.Width = 400;
                cameras[2].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[2].video.OnError += Video_OnError;
                Grid.SetRow(cameras[2].video, 3);
                Grid.SetColumn(cameras[2].video, 0);
                Camera_Container.Children.Add(cameras[2].video);
                // Setup the Camera
                cameras[3].video.Width = 400;
                cameras[3].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[3].video.OnError += Video_OnError;
                Grid.SetRow(cameras[3].video, 3);
                Grid.SetColumn(cameras[3].video, 1);
                Camera_Container.Children.Add(cameras[3].video);
                // Setup the Camera
                cameras[4].video.Width = 400;
                cameras[4].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[4].video.OnError += Video_OnError;
                Grid.SetRow(cameras[4].video, 1);
                Grid.SetColumn(cameras[4].video, 2);
                Camera_Container.Children.Add(cameras[4].video);
                // Setup the Camera
                cameras[5].video.Width = 400;
                cameras[5].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[5].video.OnError += Video_OnError;
                Grid.SetRow(cameras[5].video, 3);
                Grid.SetColumn(cameras[5].video, 2);
                Camera_Container.Children.Add(cameras[5].video);
                // Setup the Camera
                cameras[6].video.Width = 400;
                cameras[6].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[6].video.OnError += Video_OnError;
                Grid.SetRow(cameras[6].video, 1);
                Grid.SetColumn(cameras[6].video, 3);
                Camera_Container.Children.Add(cameras[6].video);
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
            else if (Camera.count == 8)
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
                    Content = cameras[0].name,
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
                    Content = cameras[1].name,
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
                    Content = cameras[2].name,
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
                    Content = cameras[3].name,
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
                    Content = cameras[4].name,
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
                    Content = cameras[5].name,
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
                    Content = cameras[6].name,
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
                    Content = cameras[7].name,
                    FontSize = 24,
                    Foreground = Brushes.Gray,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(title_8, 2);
                Grid.SetColumn(title_8, 3);
                Camera_Container.Children.Add(title_8);
                // Setup the Camera
                cameras[0].video.Width = 400;
                cameras[0].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[0].video.OnError += Video_OnError;
                Grid.SetRow(cameras[0].video, 1);
                Grid.SetColumn(cameras[0].video, 0);
                Camera_Container.Children.Add(cameras[0].video);
                // Setup the Camera
                cameras[1].video.Width = 400;
                cameras[1].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[1].video.OnError += Video_OnError;
                Grid.SetRow(cameras[1].video, 1);
                Grid.SetColumn(cameras[1].video, 1);
                Camera_Container.Children.Add(cameras[1].video);
                // Setup the Camera
                cameras[2].video.Width = 400;
                cameras[2].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[2].video.OnError += Video_OnError;
                Grid.SetRow(cameras[2].video, 3);
                Grid.SetColumn(cameras[2].video, 0);
                Camera_Container.Children.Add(cameras[2].video);
                // Setup the Camera
                cameras[3].video.Width = 400;
                cameras[3].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[3].video.OnError += Video_OnError;
                Grid.SetRow(cameras[3].video, 3);
                Grid.SetColumn(cameras[3].video, 1);
                Camera_Container.Children.Add(cameras[3].video);
                // Setup the Camera
                cameras[4].video.Width = 400;
                cameras[4].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[4].video.OnError += Video_OnError;
                Grid.SetRow(cameras[4].video, 1);
                Grid.SetColumn(cameras[4].video, 2);
                Camera_Container.Children.Add(cameras[4].video);
                // Setup the Camera
                cameras[5].video.Width = 400;
                cameras[5].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[5].video.OnError += Video_OnError;
                Grid.SetRow(cameras[5].video, 3);
                Grid.SetColumn(cameras[5].video, 2);
                Camera_Container.Children.Add(cameras[5].video);
                // Setup the Camera
                cameras[6].video.Width = 400;
                cameras[6].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[6].video.OnError += Video_OnError;
                Grid.SetRow(cameras[6].video, 1);
                Grid.SetColumn(cameras[6].video, 3);
                Camera_Container.Children.Add(cameras[6].video);
                // Setup the Camera
                cameras[7].video.Width = 400;
                cameras[7].video.MouseUp += (object sender, MouseButtonEventArgs e) =>
                {
                    camerasFocused(sender, e);
                };
                cameras[7].video.OnError += Video_OnError;
                Grid.SetRow(cameras[7].video, 3);
                Grid.SetColumn(cameras[7].video, 3);
                Camera_Container.Children.Add(cameras[7].video);
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
