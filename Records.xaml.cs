using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
//using System.Windows.Media;
//using VisioForge.Controls.UI.WPF;

namespace IPCamera
{
    /// <summary>
    /// Interaction logic for Records.xaml
    /// </summary>
    public partial class Records : Window
    {

        public List<String> cameras_videos_dates = new List<String>();
        public List<String> cameras_videos_times = new List<String>();
        public List<String> cameras_videos_paths = new List<String>();
        public List<Video> videos = new List<Video>();

        public List<String> cameras_pictures_dates = new List<String>();
        public List<String> cameras_pictures_times = new List<String>();
        public List<String> cameras_pictures_paths = new List<String>();
        public List<Picture> pictures = new List<Picture>();



        public Records()
        {
            InitializeComponent();

            // Get Records
            this.GetRecordsPath();

            // Load Videos And Images ComboBoxes
            this.SetComboBoxesVideos();
            this.SetComboBoxeesImages();
            this.CreateMediaPlayers();
            this.CreatePictures();
        }


        // Create Media Players For Every File
        private void CreateMediaPlayers()
        {
            // Order List
            List<Video> SortedList = this.videos.OrderBy(o => o.CamName).ToList();
            Console.WriteLine($"Videos Count:  {SortedList.Count}");
            // Start Creating The Media
            videos_grid.Children.Clear();
            videos_grid.RowDefinitions.Clear();
            videos_grid.ColumnDefinitions.Clear();
            int columns_pointer_videos = 0;
            int rows_pointer_videos = 0;
            int size = 377;

            // Add First Row
            RowDefinition row_1 = new RowDefinition();
            row_1.Height = new GridLength(size);
            videos_grid.RowDefinitions.Add(row_1);

            // Add 3 Columns
            ColumnDefinition column = new ColumnDefinition();
            column.Width = new GridLength(size);
            videos_grid.ColumnDefinitions.Add(column);
            column = new ColumnDefinition();
            column.Width = new GridLength(size);
            videos_grid.ColumnDefinitions.Add(column);
            column = new ColumnDefinition();
            column.Width = new GridLength(size);
            videos_grid.ColumnDefinitions.Add(column);

            foreach (Video video in SortedList)
            {
                // Somthing Rong With Rows
                if(columns_pointer_videos == 3) // New Row
                {
                    Console.WriteLine("New Row.");
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(size);
                    videos_grid.RowDefinitions.Add(row);
                    rows_pointer_videos++;
                    this.CreateVideoGrid(columns_pointer_videos, rows_pointer_videos, video);
                    columns_pointer_videos = 0;
                }
                this.CreateVideoGrid(columns_pointer_videos, rows_pointer_videos, video);
                columns_pointer_videos++;
            }
            Console.WriteLine($"Columns: {videos_grid.ColumnDefinitions.Count}    Rows: {videos_grid.RowDefinitions.Count}");
        }

        // Create Video Grid
        private void CreateVideoGrid(int column, int row, Video video)
        {
            // Create Video Grid And Add it to Video_Grid
            Grid vid_Grid = new Grid();
            vid_Grid.Background = Brushes.Gray;
            vid_Grid.Margin = new Thickness(3);
            Grid.SetRow(vid_Grid, row);
            Grid.SetColumn(vid_Grid, column);
            videos_grid.Children.Add(vid_Grid);
            // Add 2 Rows
            RowDefinition row_2 = new RowDefinition();
            row_2.Height = new GridLength(30);
            RowDefinition row_3 = new RowDefinition();
            row_3.Height = new GridLength(0, GridUnitType.Auto);
            vid_Grid.RowDefinitions.Add(row_2);
            vid_Grid.RowDefinitions.Add(row_3);
            // Add New Grid Grid At Row 0 (Title Grid)
            Grid titleGrid = new Grid();
            Grid.SetRow(titleGrid, 0);
            vid_Grid.Children.Add(titleGrid);
            // Tittle Grid Has 3 Columns
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            // Add 3 Labels
            Label name = new Label();
            name.Content = video.CamName;
            name.Foreground = Brushes.LightGray;
            Grid.SetColumn(name, 0);
            Label date = new Label();
            date.Content = video.Date;
            date.Foreground = Brushes.LightGray;
            Grid.SetColumn(date, 1);
            Label time = new Label();
            time.Content = video.Time;
            time.Foreground = Brushes.LightGray;
            Grid.SetColumn(time, 2);
            titleGrid.Children.Add(name);
            titleGrid.Children.Add(date);
            titleGrid.Children.Add(time);
            // Create MediaPlayer
            MediaElement player = new MediaElement();
            player.Source = new Uri(video.Path);
            player.Margin = new Thickness(0,7,0,0);
            Grid.SetRow(player, 1);
            vid_Grid.Children.Add(player);
            // Print To Console
            Console.WriteLine($"Creating Video Grid: {video.CamName}  {video.Date}  {video.Time}");
        }

        // Create Pictures For Every File
        private void CreatePictures()
        {
            Console.WriteLine("Create Picture.");

            // Order List
            List<Picture> SortedList = this.pictures.OrderBy(o => o.CamName).ToList();

            // Start Creating The Media
            foreach (Picture pic in SortedList)
            {
                Console.WriteLine($"{pic.CamName}  {pic.Date}  {pic.Time}");
            }
        }



        // Find Selected Videos
        private void Videos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateVideoRecords();
            this.CreateMediaPlayers();
        }
        // Find Selected Images
        private void Pictures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateImageRecords();
            this.CreatePictures();
        }
        private void UpdateVideoRecords()
        {
            String date = (String)dates_v.SelectedValue;
            String time = (String)times_v.SelectedValue;
            try
            {
                this.videos.Clear();
                foreach (String path in this.cameras_videos_paths)
                {
                    if (path.Contains(date) && path.Contains(time))
                    {
                        this.videos.Add(new Video(path));
                    }
                }
            }
            catch (System.ArgumentNullException ex)
            {
                Console.WriteLine($"\n\n {ex}\n\n");
            }
        }
        private void UpdateImageRecords()
        {
            String date = (String)dates_i.SelectedValue;
            String time = (String)times_i.SelectedValue;
            List<String> paths = new List<String>();
            try
            {
                this.pictures.Clear();
                foreach (String path in this.cameras_pictures_paths)
                {
                    if (path.Contains(date) && path.Contains(time))
                    {
                        this.pictures.Add(new Picture(path));
                    }
                }
            }
            catch (System.ArgumentNullException ex)
            {
                Console.WriteLine($"\n\n {ex}\n\n");
            }
        }



        // Load Videos ComboBoxes
        private void SetComboBoxesVideos()
        {
            this.cameras_videos_dates.Sort();
            this.cameras_videos_dates.Reverse();
            dates_v.ItemsSource = this.cameras_videos_dates;
            dates_v.SelectedValue = this.cameras_videos_dates[0];
            this.cameras_videos_times.Sort();
            this.cameras_videos_times.Reverse();
            times_v.ItemsSource = this.cameras_videos_times;
            times_v.SelectedValue = this.cameras_videos_times[0];
            this.CreateMediaPlayers();
        }

        // Load Images ComboBoxes
        private void SetComboBoxeesImages()
        {
            this.cameras_pictures_dates.Sort();
            this.cameras_pictures_dates.Reverse();
            dates_i.ItemsSource = this.cameras_pictures_dates;
            dates_i.SelectedValue = this.cameras_pictures_dates[0];
            this.cameras_pictures_times.Sort();
            this.cameras_pictures_times.Reverse();
            times_i.ItemsSource = this.cameras_pictures_times;
            times_i.SelectedValue = this.cameras_pictures_times[0];
        }

        // Get Records
        private void GetRecordsPath()
        {
            // Get All Videos
            this.GetDirsSubDirsFiles(Camera.videos_dir, x =>
            {
                String[] parts = x.Split('\\');
                if (!this.cameras_videos_dates.Contains(parts[7]))
                {
                    this.cameras_videos_dates.Add(parts[7]);
                }
                if(!this.cameras_videos_times.Contains(parts[8].Substring(0, parts[8].Length - 4)))
                {
                    this.cameras_videos_times.Add(parts[8].Substring(0, parts[8].Length - 4));
                }
                if (!this.cameras_videos_paths.Contains(x))
                {
                    this.cameras_videos_paths.Add(x);
                }
            });
            // Get All Images
            this.GetDirsSubDirsFiles(Camera.pictures_dir, x =>
            {
                String[] parts = x.Split('\\');
                if (!this.cameras_pictures_dates.Contains(parts[7]))
                {
                    this.cameras_pictures_dates.Add(parts[7]);
                }
                if (!this.cameras_pictures_times.Contains(parts[8].Substring(0, parts[8].Length - 4)))
                {
                    this.cameras_pictures_times.Add(parts[8].Substring(0, parts[8].Length - 4));
                }
                if (!this.cameras_pictures_paths.Contains(x))
                {
                    this.cameras_pictures_paths.Add(x);
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
            String[] paths = path.Split('\\');
            this.CamName = paths[6];
            this.Date = paths[7];
            this.Time = paths[8].Substring(0, 8);
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
            String[] paths = path.Split('\\');
            this.CamName = paths[6];
            this.Date = paths[7];
            this.Time = paths[8].Substring(0, 8);
        }
    }

}
