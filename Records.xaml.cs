using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace IPCamera
{

    public partial class Records : Window
    {
        public List<Video> videos = new List<Video>();
        public List<Picture> pictures = new List<Picture>();
        public List<Video> selectedVideos = new List<Video>();
        public List<Picture> selectedPictures = new List<Picture>();
        bool allVideos = false;
        bool allPictures = false;
        public bool fullscreen = false;
        RecordFullScreen fullscreen_page;

        public Records()
        {
            InitializeComponent();
            //main_container.Visibility = Visibility.Hidden;

            // Get Records
            this.GetRecordsPath();
            // Load Videos And Images ComboBoxes
            this.SetComboBoxesVideos();
            this.SetComboBoxeesImages();

            // Setup To Show All Cameres
            all_v.IsChecked = false;
            all_i.IsChecked = false;
        }






        // If CheckBox All Videos Changeded
        private void Videos_SelectionChanged_all(object sender, RoutedEventArgs e)
        {
            this.allVideos = (bool)all_v.IsChecked;


            String date = (String)dates_v.SelectedValue;
            String time = (String)times_v.SelectedValue;
            String cam = (String)cams_v.SelectedValue;
            List<Video> vids;
            if (date != "" && time != "" && cam != "")
            {
                if (!this.allVideos)
                {
                    vids = (from video in this.videos where video.Date == date && video.Time == time && video.CamName == cam select video).ToList();
                }
                else
                {
                    vids = (from video in this.videos where video.Date == date && video.Time == time select video).ToList();
                }
                Console.WriteLine($"date: {date}  time: {time}  vids.Count:  {vids.Count}");
                this.selectedVideos.Clear();
                this.selectedVideos.AddRange(vids);
                this.CreateMediaPlayers();
            }
        }

        // If CheckBox All Pictures Changeded
        private void Pictures_SelectionChanged_all(object sender, RoutedEventArgs e)
        {
            this.allPictures = (bool)all_i.IsChecked;

            String date = (String)dates_i.SelectedValue;
            String time = (String)times_i.SelectedValue;
            String cam = (String)cams_i.SelectedValue;
            List<Picture> pics;
            if (date != "" && time != "" && cam != "")
            {
                if (!this.allPictures)
                {
                    pics = (from picture in this.pictures where picture.Date == date && picture.Time == time && picture.CamName == cam select picture).ToList();
                }
                else
                {
                    pics = (from picture in this.pictures where picture.Date == date && picture.Time == time select picture).ToList();
                }
                Console.WriteLine($"date: {date}  time: {time}  pics.Count:  {pics.Count}");
                this.selectedPictures.Clear();
                this.selectedPictures.AddRange(pics);
                this.CreatePictures();
            }
        }

        // When Select Cameras Name On Videos
        private void Videos_SelectionChanged_cams(object sender, SelectionChangedEventArgs e)
        {
            String date = (String)dates_v.SelectedValue;
            String time = (String)times_v.SelectedValue;
            String cam = (String)cams_v.SelectedValue;
            if (date != "" && time != "" && cam != "")
            {
                if (!this.allVideos)
                {
                    List<Video> vids = (from video in this.videos where video.Date == date && 
                                        video.Time == time && video.CamName == cam select video).ToList();
                    Console.WriteLine($"date: {date}  time: {time}  vids.Count:  {vids.Count}");
                    this.selectedVideos.Clear();
                    this.selectedVideos.AddRange(vids);
                    this.CreateMediaPlayers();
                }
            }
        }

        // When Select Cameras Name On Pictures
        private void Pictures_SelectionChanged_cams(object sender, SelectionChangedEventArgs e)
        {
            String date = (String)dates_i.SelectedValue;
            String time = (String)times_i.SelectedValue;
            String cam = (String)cams_i.SelectedValue;
            if(date != "" && time != "" && cam != "")
            {
                if(!this.allPictures)
                {
                    List<Picture> pics = (from picture in this.pictures where picture.Date == date && 
                                          picture.Time == time && picture.CamName == cam select picture).ToList();
                    Console.WriteLine($"date: {date}  time: {time}  pics.Count:  {pics.Count}");
                    this.selectedPictures.Clear();
                    this.selectedPictures.AddRange(pics);
                    this.CreatePictures();
                }
            }
        }


        // When Select Date On Videos
        private void Videos_SelectionChanged_date(object sender, SelectionChangedEventArgs e)
        {
            String date = (String)dates_v.SelectedValue;
            if (date != "")
            {
                try
                {
                    // Set ComboBox Times
                    HashSet<String> times = new HashSet<String>((from video in this.videos where video.Date == date select video.Time).ToList());
                    List<String> times_l = times.ToList();
                    times_l.Sort();
                    times_l.Reverse();
                    times_v.ItemsSource = times_l;
                    times_v.SelectedValue = times_l[0];
                }
                catch(System.InvalidOperationException ex)
                {
                    Console.WriteLine($"Exception: {ex}");
                }
            }
        }

        // When Select Time On Videos
        private void Videos_SelectionChanged_time(object sender, SelectionChangedEventArgs e)
        {
            String date = (String)dates_v.SelectedValue;
            String time = (String)times_v.SelectedValue;
            if (date != "" && time != "")
            {
                try
                {
                    HashSet<String> camerasNames = new HashSet<String>((from video in this.videos where 
                                                                        video.Date == date select video.CamName).ToList());
                    List<String> camerasNames_l = camerasNames.ToList();
                    camerasNames_l.Sort();
                    camerasNames_l.Reverse();
                    cams_v.ItemsSource = camerasNames_l;
                    cams_v.SelectedValue = camerasNames_l[0];
                    if (this.allVideos)
                    {
                        List<Video> vids = (from video in this.videos where video.Date == date && 
                                            video.Time == time select video).ToList();
                        Console.WriteLine($"date: {date}  time: {time}  vids.Count:  {vids.Count}");
                        this.selectedVideos.Clear();
                        this.selectedVideos.AddRange(vids);
                        this.CreateMediaPlayers();
                    }
                    
                }
                catch (System.InvalidOperationException ex)
                {
                    Console.WriteLine($"Exception: {ex}");
                }
            }
        }

        // When Select Date On Pictures
        private void Pictures_SelectionChanged_date(object sender, SelectionChangedEventArgs e)
        {
            String date = (String)dates_i.SelectedValue;
            if (date != "")
            {
                try
                {
                    // Set ComboBox Times
                    HashSet<String> times = new HashSet<String>((from picture in this.pictures where picture.Date == date select picture.Time).ToList());
                    List<String> times_l = times.ToList();
                    times_l.Sort();
                    times_l.Reverse();
                    times_i.ItemsSource = times_l;
                    times_i.SelectedValue = times_l[0];
                }
                catch (System.InvalidOperationException ex)
                {
                    Console.WriteLine($"Exception: {ex}");
                }
            }
        }

        // When Select Time On Pictures
        private void Pictures_SelectionChanged_time(object sender, SelectionChangedEventArgs e)
        {
            String date = (String)dates_i.SelectedValue;
            String time = (String)times_i.SelectedValue;
            if (date != "" && time != "")
            {
                try
                {
                    HashSet<String> camerasNames = new HashSet<String>((from picture in this.pictures where picture.Date == date select picture.CamName).ToList());
                    List<String> camerasNames_l = camerasNames.ToList();
                    camerasNames_l.Sort();
                    camerasNames_l.Reverse();
                    cams_i.ItemsSource = camerasNames_l;
                    cams_i.SelectedValue = camerasNames_l[0];
                    if (this.allPictures )
                    {
                        List<Picture> pics = (from picture in this.pictures where picture.Date == date && 
                                              picture.Time == time select picture).ToList();
                        Console.WriteLine($"date: {date}  time: {time}  pics.Count:  {pics.Count}");
                        this.selectedPictures.Clear();
                        this.selectedPictures.AddRange(pics);
                        this.CreatePictures();
                    }
                }
                catch (System.InvalidOperationException ex)
                {
                    Console.WriteLine($"Exception: {ex}");
                }
            }
        }

        // Load Videos ComboBoxes
        private void SetComboBoxesVideos()
        {
            try
            {
                // Dates
                HashSet<String> dates = new HashSet<String>((from video in this.videos select video.Date).ToList());
                List<String> dates_l = dates.ToList();
                dates_l.Sort();
                dates_l.Reverse();
                dates_v.ItemsSource = dates_l;
                dates_v.SelectedValue = dates_l[0];
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception:  {ex}");
            }
            
        }

        // Load Images ComboBoxes
        private void SetComboBoxeesImages()
        {
            try
            {
                // Dates
                HashSet<String> dates = new HashSet<String>((from picture in this.pictures select picture.Date).ToList());
                List<String> dates_l = dates.ToList();
                dates_l.Sort();
                dates_l.Reverse();
                dates_i.ItemsSource = dates_l;
                dates_i.SelectedValue = dates_l[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception:  {ex}");
            }
        }

        // Get Records
        private void GetRecordsPath()
        {
            // Get All Videos
            this.GetDirsSubDirsFiles(Camera.videos_dir, x =>
            {
                try
                {
                    Video video = new Video(x);
                    if (!this.videos.Contains(video))
                    {
                        this.videos.Add(video);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception:  {ex}");
                }
            });
            // Get All Images
            this.GetDirsSubDirsFiles(Camera.pictures_dir, x =>
            {
                try
                {
                    Picture picture = new Picture(x);
                    if (!this.pictures.Contains(picture))
                    {
                        this.pictures.Add(picture);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception:  {ex}");
                }
            });
        }
        // Search All Dirs And Sub Dirs and excecute a Function
        public delegate void InsideDirsFunction(String path);
        private void GetDirsSubDirsFiles(String path, InsideDirsFunction func)
        {
            if (File.Exists(path)) // Is File
            {
                func(path);
            }
            else if (Directory.Exists(path))  // Is Dir
            {
                string[] sub_dirss = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                foreach (string f_path in sub_dirss)
                {
                    this.GetDirsSubDirsFiles(f_path, func);
                }
            }
        }









        // Create Media Players For Every File
        private void CreateMediaPlayers()
        {
            try
            {
                if (videos_grid != null)
                {
                    // Order List
                    List<Video> SortedList = this.selectedVideos.OrderBy(o => o.CamName).ToList();
                    Console.WriteLine($"Videos Count:  {SortedList.Count}");
                    // Start Creating The Media
                    videos_grid.Children.Clear();
                    videos_grid.RowDefinitions.Clear();
                    videos_grid.ColumnDefinitions.Clear();
                    int columns_pointer_videos = 0;
                    int rows_pointer_videos = 0;
                    // Add First Row
                    RowDefinition row = new RowDefinition();
                    row.MaxHeight = 400;
                    videos_grid.RowDefinitions.Add(row);
                    // Add 3 Columns
                    videos_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    videos_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    videos_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    videos_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    Player play;
                    foreach (Video video in SortedList)
                    {
                        // Somthing Rong With Rows
                        if (columns_pointer_videos == 4) // New Row
                        {
                            RowDefinition row_2 = new RowDefinition();
                            row_2.MaxHeight = 400;
                            videos_grid.RowDefinitions.Add(row_2);
                            rows_pointer_videos++;
                            play = new Player(videos_grid, video, columns_pointer_videos, rows_pointer_videos);
                            play.CreateVideo();
                            columns_pointer_videos = 0;
                        }
                        play = new Player(videos_grid, video, columns_pointer_videos, rows_pointer_videos);
                        play.CreateVideo();
                        columns_pointer_videos++;
                    }
                    //Console.WriteLine($"Columns: {videos_grid.ColumnDefinitions.Count}    Rows: {videos_grid.RowDefinitions.Count}");
                }
            }
            catch (System.NullReferenceException ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
        }

        // Create Pictures For Every File
        private void CreatePictures()
        {
            try
            {
                if (images_grid != null)
                {
                    // Order List
                    List<Picture> SortedList = this.selectedPictures.OrderBy(o => o.CamName).ToList();
                    Console.WriteLine($"Pictures Count:  {SortedList.Count}");
                    // Start Creating The Media
                    images_grid.Children.Clear();
                    images_grid.RowDefinitions.Clear();
                    images_grid.ColumnDefinitions.Clear();
                    int columns_pointer_pictures = 0;
                    int rows_pointer_pictures = 0;
                    // Add First Row
                    RowDefinition row = new RowDefinition();
                    row.MaxHeight = 300;
                    images_grid.RowDefinitions.Add(row);
                    // Add 3 Columns
                    images_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    images_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    images_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    foreach (Picture picture in SortedList)
                    {
                        // Somthing Rong With Rows
                        if (columns_pointer_pictures == 3) // New Row
                        {
                            RowDefinition row_2 = new RowDefinition();
                            row_2.MaxHeight = 300;
                            images_grid.RowDefinitions.Add(row_2);
                            rows_pointer_pictures++;
                            this.CreateImage(rows_pointer_pictures, columns_pointer_pictures, picture);
                            columns_pointer_pictures = 0;
                        }
                        this.CreateImage(rows_pointer_pictures, columns_pointer_pictures, picture);
                        columns_pointer_pictures++;
                    }
                    Console.WriteLine($"Columns: {images_grid.ColumnDefinitions.Count}    Rows: {images_grid.RowDefinitions.Count}");
                }
            }
            catch(System.NullReferenceException ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            catch( Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
        }


        private void CreateImage(int row, int column, Picture pic)
        {
            // Card Grid
            Grid img_grid = new Grid();
            img_grid.MaxHeight = 300;
            img_grid.Margin = new Thickness(3);
            RowDefinition row_1 = new RowDefinition();
            row_1.Height = new GridLength(23);
            RowDefinition row_2 = new RowDefinition();
            row_2.Height = new GridLength(23);
            RowDefinition row_3 = new RowDefinition();
            row_3.Height = new GridLength(23);
            RowDefinition row_4 = new RowDefinition();
            row_4.Height = new GridLength(0, GridUnitType.Auto);
            RowDefinition row_5 = new RowDefinition();
            row_5.Height = new GridLength(23);
            img_grid.RowDefinitions.Add(row_1);
            img_grid.RowDefinitions.Add(row_2);
            img_grid.RowDefinitions.Add(row_3);
            img_grid.RowDefinitions.Add(row_4);
            img_grid.RowDefinitions.Add(row_5);
            img_grid.Background = System.Windows.Media.Brushes.Gray;
            Grid.SetRow(img_grid, row);
            Grid.SetColumn(img_grid, column);
            images_grid.Children.Add(img_grid);

            // Labels StackPanel
            StackPanel panel = new StackPanel();
            Grid.SetRow(panel, 0);
            panel.Orientation = Orientation.Horizontal;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Center;
            panel.Margin = new Thickness(5,0,5,0);
            img_grid.Children.Add(panel);
            // Label
            Label label_1 = new Label();
            label_1.Content = pic.CamName;
            label_1.Foreground = System.Windows.Media.Brushes.DarkRed;
            label_1.FontSize = 12;
            panel.Children.Add(label_1);

            // Labels StackPanel
            StackPanel panel_2 = new StackPanel();
            Grid.SetRow(panel_2, 1);
            panel_2.Orientation = Orientation.Horizontal;
            panel_2.HorizontalAlignment = HorizontalAlignment.Center;
            panel_2.VerticalAlignment = VerticalAlignment.Center;
            panel_2.Margin = new Thickness(5, 0, 5, 0);
            //panel.Background = System.Windows.Media.Brushes.Green;
            img_grid.Children.Add(panel_2);
            // Label
            Label label_2 = new Label();
            label_2.Content = pic.Date;
            label_2.Foreground = System.Windows.Media.Brushes.DarkRed;
            label_2.FontSize = 12;
            panel_2.Children.Add(label_2);

            // Labels StackPanel
            StackPanel panel_3 = new StackPanel();
            Grid.SetRow(panel_3, 2);
            panel_3.Orientation = Orientation.Horizontal;
            panel_3.HorizontalAlignment = HorizontalAlignment.Center;
            panel_3.VerticalAlignment = VerticalAlignment.Center;
            panel_3.Margin = new Thickness(5, 0, 5, 0);
            //panel.Background = System.Windows.Media.Brushes.Green;
            img_grid.Children.Add(panel_3);
            // Label
            Label label_3 = new Label();
            label_3.Content = pic.Time;
            label_3.Foreground = System.Windows.Media.Brushes.DarkRed;
            label_3.FontSize = 12;
            panel_3.Children.Add(label_3);

            // Create Media Element
            MediaElement image = new MediaElement();
            Grid.SetRow(image, 3);
            image.Source = new Uri(pic.Path);
            image.Margin = new Thickness(1);
            image.Height = 200;
            image.VerticalAlignment = VerticalAlignment.Center;
            img_grid.Children.Add(image);

            // Buttons StackPanel
            StackPanel panel_b = new StackPanel();
            Grid.SetRow(panel_b, 4);
            panel_b.Orientation = Orientation.Horizontal;
            panel_b.HorizontalAlignment = HorizontalAlignment.Center;
            panel_b.VerticalAlignment = VerticalAlignment.Center;
            //panel_b.Margin = new Thickness(7, 0, 7, 0);
            panel_b.Background = System.Windows.Media.Brushes.Orange;
            img_grid.Children.Add(panel_b);
            Button open = new Button();
            open.Content = "Open";
            open.FontSize = 12;
            open.Padding = new Thickness(7, 0, 7, 0);
            open.Click += (object obj, RoutedEventArgs e) =>
            {
                RecordFullScreen fullscreen_page;
                if (!this.fullscreen)
                {
                    this.fullscreen = true;
                    this.fullscreen_page = new RecordFullScreen(pic, this);
                    this.fullscreen_page.Show();
                }
                else
                {
                    this.fullscreen_page.Activate();
                }
            };
            panel_b.Children.Add(open);
            Button delete = new Button();
            delete.Content = "Delete";
            delete.FontSize = 12;
            delete.Padding = new Thickness(7, 0, 7, 0);
            delete.Click += (object obj, RoutedEventArgs e) =>
            {
                if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Delete(pic.Path);
                }
            };
            panel_b.Children.Add(delete);
        }



        private void X_Button_R_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.logged)
            {
                MainWindow.records_oppened = false;
                Console.WriteLine("records_oppened: " + Convert.ToString(MainWindow.records_oppened));
                this.Close();
            }
        }


        // On Close Window
        protected override void OnClosed(EventArgs e)
        {
            MainWindow.records_oppened = false;
            Console.WriteLine("records_oppened: " + Convert.ToString(MainWindow.records_oppened));
            this.Close();
        }

        
    }


    public class Video
    {
        public String CamName { get; set; }
        public String Date { get; set; }
        public String Time { get; set; }
        public String Path { get; set; }

        public Video(String path)
        {
            this.Path = path;
            String[] parts = this.Path.Split('\\');
            this.CamName = parts[parts.Length - 3];
            this.Date = parts[parts.Length - 2];
            this.Time = parts[parts.Length - 1].Substring(0, parts[parts.Length - 1].Length - 7);
        }
    }

    public class Picture
    {
        public String CamName { get; set; }
        public String Date { get; set; }
        public String Time { get; set; }
        public String Path { get; set; }

        public Picture(String path)
        {
            this.Path = path;
            String[] parts = path.Split('\\');
            this.CamName = parts[parts.Length - 3];
            this.Date = parts[parts.Length - 2];
            this.Time = parts[parts.Length - 1].Substring(0, parts[parts.Length - 1].Length - 7);
        }
    }

}
