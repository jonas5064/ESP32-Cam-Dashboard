using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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
        private double window_width;
        private double window_height;

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

            // When This Window Resized
            this.SizeChanged += OnWindowSizeChanged;
        }

        ~Records()
        {
            this.videos.Clear();
            this.pictures.Clear();
            this.selectedVideos.Clear();
            this.selectedPictures.Clear();
            this.window_width = 0;
            this.window_height = 0;
        }

        // When This Window Resized
        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.window_width = e.NewSize.Width;
            this.window_height = e.NewSize.Height;
            _ = e.PreviousSize.Width;
            _ = e.PreviousSize.Height;
            if ( true )
            {
                Console.WriteLine($"\nWindow Width: {this.window_width}");
            }
            if (true)
            {
                Console.WriteLine($"Window Height: {this.window_height}");
            }
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
                //Console.WriteLine($"date: {date}  time: {time}  vids.Count:  {vids.Count}");
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
                //Console.WriteLine($"date: {date}  time: {time}  pics.Count:  {pics.Count}");
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
                    //Console.WriteLine($"date: {date}  time: {time}  vids.Count:  {vids.Count}");
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
                    //Console.WriteLine($"date: {date}  time: {time}  pics.Count:  {pics.Count}");
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
                        //Console.WriteLine($"date: {date}  time: {time}  vids.Count:  {vids.Count}");
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
                        //Console.WriteLine($"date: {date}  time: {time}  pics.Count:  {pics.Count}");
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
            this.GetDirsSubDirsFiles(Camera.Videos_dir, x =>
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
            this.GetDirsSubDirsFiles(Camera.Pictures_dir, x =>
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
                    //Console.WriteLine($"Videos Count:  {SortedList.Count}");
                    // Start Creating The Media
                    videos_grid.Children.Clear();
                    videos_grid.RowDefinitions.Clear();
                    videos_grid.ColumnDefinitions.Clear();
                    int columns_pointer_videos = 0;
                    int rows_pointer_videos = 0;
                    // Add First Row
                    RowDefinition row = new RowDefinition
                    {
                        MaxHeight = 400
                    };
                    videos_grid.RowDefinitions.Add(row);
                    // Add 3 Columns
                    videos_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    videos_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    videos_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    Player play;
                    foreach (Video video in SortedList)
                    {
                        // Somthing Rong With Rows
                        if (columns_pointer_videos == 3) // New Row
                        {
                            RowDefinition row_2 = new RowDefinition
                            {
                                MaxHeight = 400
                            };
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
                    //Console.WriteLine($"Pictures Count:  {SortedList.Count}");
                    // Start Creating The Media
                    images_grid.Children.Clear();
                    images_grid.RowDefinitions.Clear();
                    images_grid.ColumnDefinitions.Clear();
                    int columns_pointer_pictures = 0;
                    int rows_pointer_pictures = 0;
                    // Add First Row
                    RowDefinition row = new RowDefinition
                    {
                        MaxHeight = 333
                    };
                    images_grid.RowDefinitions.Add(row);
                    // Add 3 Columns
                    images_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    images_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    images_grid.ColumnDefinitions.Add(new ColumnDefinition());
                    Player play;
                    foreach (Picture picture in SortedList)
                    {
                        // Somthing Rong With Rows
                        if (columns_pointer_pictures == 3) // New Row
                        {
                            RowDefinition row_2 = new RowDefinition
                            {
                                MaxHeight = 333
                            };
                            images_grid.RowDefinitions.Add(row_2);
                            rows_pointer_pictures++;
                            play = new Player(images_grid, picture, this, columns_pointer_pictures, rows_pointer_pictures);
                            play.CreatePicture();
                            columns_pointer_pictures = 0;
                        }
                        play = new Player(images_grid, picture, this, columns_pointer_pictures, rows_pointer_pictures);
                            play.CreatePicture();
                        columns_pointer_pictures++;
                    }
                    //Console.WriteLine($"Columns: {images_grid.ColumnDefinitions.Count}    Rows: {images_grid.RowDefinitions.Count}");
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


        private void X_Button_R_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Logged)
            {
                MainWindow.Records_oppened = false;
                Console.WriteLine("Records_oppened: " + Convert.ToString(MainWindow.Records_oppened));
                this.Close();
            }
        }


        // On Close Window
        protected override void OnClosed(EventArgs e)
        {
            MainWindow.Records_oppened = false;
            Console.WriteLine("Records_oppened: " + Convert.ToString(MainWindow.Records_oppened));
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
