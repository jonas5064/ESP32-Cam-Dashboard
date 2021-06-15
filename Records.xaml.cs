using System;
using System.Collections.Generic;
using System.IO;
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

        public List<List<List<String>>> cameras_videos = new List<List<List<String>>>();
        public List<List<List<String>>> cameras_pictures = new List<List<List<String>>>();

        int count = 0;
        String name_before = "";
        int camera_pointer = 0;

        public Records()
        {
            InitializeComponent();

            // Get All Videos
            this.GetDirsSubDirsFiles(Camera.videos_dir, x =>
            {
                String[] parts = x.Split('\\');
                if(!parts[6].Equals(this.name_before)) // Every Defrent Camera
                {
                    //Console.WriteLine($"Name: {parts[6]}  Date: {parts[7]}  Time: {parts[8]}  Path: {x}\n");
                    this.name_before = parts[6];
                    this.cameras_videos.Add(new List<List<String>>());
                    this.camera_pointer++;
                }
                List<String> camera = new List<String>();
                camera.Add(parts[6]); // Name
                camera.Add(parts[7]); // Date
                camera.Add(parts[8].Substring(0,parts[8].Length-4)); // Time
                camera.Add(x);        // Path
                this.cameras_videos[this.camera_pointer-1].Add(camera);
                this.count++;
            });
            // Restart Pointers
            this.count = 0;
            this.camera_pointer = 0;
            // Get All Images
            this.GetDirsSubDirsFiles(Camera.pictures_dir, x =>
            {
                String[] parts = x.Split('\\');
                if (!parts[6].Equals(this.name_before)) // Every Defrent Camera
                {
                    //Console.WriteLine($"Name: {parts[6]}  Date: {parts[7]}  Time: {parts[8]}  Path: {x}\n");
                    this.name_before = parts[6];
                    this.cameras_pictures.Add(new List<List<String>>());
                    this.camera_pointer++;
                }
                List<String> camera = new List<String>();
                camera.Add(parts[6]); // Name
                camera.Add(parts[7]); // Date
                camera.Add(parts[8].Substring(0, parts[8].Length - 4)); // Time
                camera.Add(x);        // Path
                this.cameras_pictures[this.camera_pointer-1].Add(camera);
                this.count++;
            });
            // Load Grids With files
            //this.LoadGrids();
        }

        // Load Grids With files
        private void LoadGrids()
        {

            Console.WriteLine($"\n\n\nvideos Count: {this.cameras_videos.Count}\n\n");

            /*
                int count_rows = 0;
                int count_columns = 0;
                foreach (Camera cam in cameras)
                {
                    // New Row
                    if (count_columns == 3)
                    {
                        cameras_grid.RowDefinitions.Add(new RowDefinition());
                        count_rows++;
                        Grid.SetColumn(cam.video, count_columns);
                        cam.coll = count_columns;
                        Grid.SetRow(cam.video, count_rows);
                        cam.row = count_rows;
                        cameras_grid.Children.Add(cam.video);
                        count_columns = 0;
                    }
                    else
                    {
                        Grid.SetColumn(cam.video, count_columns);
                        cam.coll = count_columns;
                        Grid.SetRow(cam.video, count_rows);
                        cam.row = count_rows;
                        cameras_grid.Children.Add(cam.video);
                        cameras_grid.ColumnDefinitions.Add(new ColumnDefinition());
                        count_columns++;
                    }
                } 
            */

            // Dynamic add columns and rows
            int cam_grid_column = 0;
            int cam_grid_row = 0;
            videos_grid.ShowGridLines = true;
            // Load Videos Grid
            foreach (List<List<String>> cameras in this.cameras_videos)
            {
                // Create A grid Fro Every Camera
                Grid cam_grid = new Grid();
                cam_grid.ShowGridLines = true;
                cam_grid.Width = 400;
                cam_grid.Height = 400;

                Random rnd = new Random();
                int num1 = rnd.Next(0,255);
                int num2 = rnd.Next(0, 255);
                int num3 = rnd.Next(0, 255);
                cam_grid.Background = new SolidColorBrush(Color.FromRgb( (byte)num1, (byte)num2, (byte)num3 ));

                if (cam_grid_column > 2) // New Row
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(100);
                    cam_grid.RowDefinitions.Add(row);
                    cam_grid_row++;
                    cam_grid_column = 0;
                    Grid.SetRow(cam_grid, cam_grid_row);
                    Grid.SetColumn(cam_grid, cam_grid_column);
                }
                else // New Column
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(100);
                    cam_grid.ColumnDefinitions.Add(column);
                    cam_grid_column++;
                    Grid.SetColumn(cam_grid, cam_grid_column);
                    Grid.SetRow(cam_grid, cam_grid_row);
                }
                videos_grid.Children.Add(cam_grid);

                /*
                // Cameras Name Ass Title
                Label title = new Label();
                title.Content = cameras[0][0];
                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString("#FFDFD991");
                title.Foreground = brush;
                Grid.SetRow(title, 0);
                Grid.SetColumn(title, 0);
                cam_grid.Children.Add(title);
                */

            /*
            // Create Videos Grid
            int videos_rows = 0;
            int videos_columns = 0;
            foreach (List<String> camera in cameras)
            {
                // Media Player
                Console.WriteLine($"Name: {camera[0]}  Date: {camera[1]}  Time: {camera[2]}  Path: {camera[3]}");
                MediaElement media = new MediaElement();
                media.Width = 100;
                media.Height = 100;
                media.Source = new System.Uri(camera[3]);
                //media.Name = part[1] + " " + part[2];
                media.LoadedBehavior = MediaState.Manual;
                media.Play();

                if (videos_columns > 2) // New Row
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(50);
                    cam_grid.RowDefinitions.Add(row);
                    videos_rows++;
                    videos_columns = 0;
                    Grid.SetRow(media, videos_rows);
                    Grid.SetColumn(media, videos_columns);
                }
                else // New Column
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(50);
                    cam_grid.ColumnDefinitions.Add(column);
                    Grid.SetColumn(media, videos_columns);
                    Grid.SetRow(media, videos_rows);
                    videos_columns++;
                }
                cam_grid.Children.Add(media);
            }
            */

        }

        // Load Pictures Grids
        ///
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

        protected override void OnClosed(EventArgs e)
        {
            MainWindow.records_oppened = false;
            Console.WriteLine("records_oppened: " + Convert.ToString(MainWindow.records_oppened));
            this.Close();
        }
    }

}
