using System;
using System.Collections.Generic;
using System.Globalization;
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
        CameraServcies CameraService { get; set; }
        MyCamera Cam { get; set; }
        public VideoFullscreen()
        {
            InitializeComponent();
        }
        public VideoFullscreen(CameraServcies camSe)
        {
            InitializeComponent();
            this.Cam = (from c in MainWindow.Main_window.DBModels.MyCameras where c.Id == camSe.cameraId select c).FirstOrDefault();
            this.CameraService = camSe;
            title.Content = this.Cam.name;
            Grid.SetRow(this.CameraService.video, 1);
            main_grid.Children.Add(this.CameraService.video);
            // Update Current Time
            time.Content = DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));
            //  DispatcherTimer setup (Thread Excecutes date update every 1 second)
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        protected override void OnClosed(EventArgs e)
        {
            main_grid.Children.Remove(this.CameraService.video);
            this.CameraService.Fullscreen = false;
            Grid.SetColumn(this.CameraService.video, (int)this.Cam.Coll);
            Grid.SetRow(this.CameraService.video, (int)this.Cam.Row);
            MainWindow.Main_window.cameras_grid.Children.Add(this.CameraService.video);
            this.Close();
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current time 
            time.Content = DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));
        }

    }
}
