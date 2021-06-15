using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IPCamera
{
    /// <summary>
    /// Interaction logic for Records.xaml
    /// </summary>
    public partial class Records : Window
    {

        public List<List<String>> videos = new List<List<String>>();
        public List<List<String>> pictures = new List<List<String>>();


        public Records()
        {
            InitializeComponent();

            // Get All My Data From Files
            Console.WriteLine("Videos Dirs:");
            this.GetDirsSubDirsFiles(Camera.videos_dir, x =>
            {
                String[] parts = x.Split('\\');
                List<String> camera = new List<String>();
                camera.Add(parts[6]); // Name
                camera.Add(parts[7]); // Date
                camera.Add(parts[8].Substring(0,parts[8].Length-4)); // Time
                camera.Add(x);        // Path
                this.videos.Add(camera);
            });
            Console.WriteLine("Pictures Dirs:");
            this.GetDirsSubDirsFiles(Camera.pictures_dir, x =>
            {
                String[] parts = x.Split('\\');
                List<String> camera = new List<String>();
                camera.Add(parts[6]); // Name
                camera.Add(parts[7]); // Date
                camera.Add(parts[8].Substring(0,parts[8].Length-4)); // Time
                camera.Add(x);        // Path
                this.pictures.Add(camera);
            });
            foreach(List<String> part in this.videos)
            {
                Console.WriteLine($"Name: {part[0]}  Date: {part[1]}  Time: {part[2]}  Path: {part[3]}");
            }


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
