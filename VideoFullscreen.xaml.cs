using System;
using System.Collections.Generic;
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
    /// Interaction logic for VideoFullscreen.xaml
    /// </summary>
    public partial class VideoFullscreen : Window
    {
        Camera cam;

        public VideoFullscreen()
        {
            InitializeComponent();
        }

        public VideoFullscreen(Camera cam)
        {
            InitializeComponent();

            this.cam = cam;
            Grid.SetRow(this.cam.video, 1);
            main_grid.Children.Add(this.cam.video);
        }

        protected override void OnClosed(EventArgs e)
        {
            main_grid.Children.Remove(this.cam.video);
            this.cam.fullscreen = false;
            Grid.SetColumn(this.cam.video, this.cam.coll);
            Grid.SetRow(this.cam.video, this.cam.row);
            MainWindow.cams_grid.Children.Add(this.cam.video);
            this.Close();
        }

    }
}
