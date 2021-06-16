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

        public List<String> cameras_videos_names = new List<String>();
        public List<String> cameras_videos_dates = new List<String>();
        public List<String> cameras_videos_times = new List<String>();
        public List<String> cameras_videos_paths = new List<String>();

        public List<String> cameras_pictures_names = new List<String>();
        public List<String> cameras_pictures_dates = new List<String>();
        public List<String> cameras_pictures_times = new List<String>();
        public List<String> cameras_pictures_paths = new List<String>();

        public Records()
        {
            InitializeComponent();

            // Get Records
            this.GetRecordsPath();

            // Load Videos And Images ComboBoxes
            this.SetComboBoxesVideos();
            this.SetComboBoxeesImages();
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
            this.cameras_videos_names.Sort();
            cameras_v.ItemsSource = this.cameras_videos_names;
            cameras_v.SelectedValue = this.cameras_videos_names[0];
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
            this.cameras_pictures_names.Sort();
            cameras_i.ItemsSource = this.cameras_pictures_names;
            cameras_i.SelectedValue = this.cameras_pictures_names[0];
        }

        // Get Records
        private void GetRecordsPath()
        {
            // Get All Videos
            this.GetDirsSubDirsFiles(Camera.videos_dir, x =>
            {
                String[] parts = x.Split('\\');
                if (!this.cameras_videos_names.Contains(parts[6]))
                {
                    this.cameras_videos_names.Add(parts[6]);
                }
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
                if (!this.cameras_pictures_names.Contains(parts[6]))
                {
                    this.cameras_pictures_names.Add(parts[6]);
                }
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


        // Find Selected Videos
        private void Cameras_Videos_Find_Clickes(object sender, RoutedEventArgs e)
        {
            String date = (String)dates_v.SelectedValue;
            String time = (String)times_v.SelectedValue;
            String camera = (String)cameras_v.SelectedValue;

            List<String> paths = new List<String>();
            foreach(String path in this.cameras_videos_paths)
            {
                if (path.Contains(date) && path.Contains(time) && path.Contains(camera))
                {
                    paths.Add(path);
                }
            }

            // Print Paths
            foreach(String path in paths)
            {
                Console.WriteLine($"Video File:  {path}");
            }
            
        }

        // Find Selected Images
        private void Cameras_Images_Find_Clickes(object sender, RoutedEventArgs e)
        {
            String date = (String)dates_i.SelectedValue;
            String time = (String)times_i.SelectedValue;
            String camera = (String)cameras_i.SelectedValue;

            List<String> paths = new List<String>();
            foreach (String path in this.cameras_pictures_paths)
            {
                if (path.Contains(date) && path.Contains(time) && path.Contains(camera))
                {
                    paths.Add(path);
                }
            }

            // Print Paths
            foreach (String path in paths)
            {
                Console.WriteLine($"Picture File:  {path}");
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

}
