using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
            this.LoadGrids();
        }

        // Load Grids With files
        private void LoadGrids()
        {

            Console.WriteLine($"\n\n\nvideos Count: {this.cameras_videos.Count}\n\n");

            // Dynamic add columns and rows
            int count_rows = 0;
            int counter = 0;
            videos_grid.ShowGridLines = true;
            // Load Videos Grid
            foreach (List<List<String>> cameras in this.cameras_videos)
            {
                // Create A grid Fro Every Camera
                        //
                foreach(List<String> camera in cameras)
                {

                }
                /*
                Console.WriteLine($"Name: {part[0]}  Date: {part[1]}  Time: {part[2]}  Path: {part[3]}");
                MediaElement media = new MediaElement();
                media.Source = new System.Uri(part[3]);
                //media.Name = part[1] + " " + part[2];
                media.LoadedBehavior = MediaState.Manual;
                media.Play();
                // New Row
                if (counter == 3)
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(300);
                    videos_grid.RowDefinitions.Add(row);
                    count_rows++;
                    Grid.SetColumn(media, counter);
                    Grid.SetRow(media, count_rows);
                    videos_grid.Children.Add(media);
                    counter = 0;
                }
                else
                {
                    Grid.SetColumn(media, counter);
                    Grid.SetRow(media, count_rows);
                    videos_grid.Children.Add(media);
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(300);
                    videos_grid.ColumnDefinitions.Add(column);
                    counter++;
                }
                */
            }

            
            count_rows = 0;
            counter = 0;

            // Load Pictures Grids

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
