using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VisioForge.Controls.UI.WPF;

namespace IPCamera
{
    /// <summary>
    /// Interaction logic for WindowControll.xaml
    /// </summary>
    public partial class WindowControll : Window
    {

        private VideoCapture camera;

        public WindowControll(VideoCapture cam)
        {
            InitializeComponent();
            this.DataContext = this;
            this.camera = cam;
            Start_cam();

            
        }

        protected override void OnClosed(EventArgs e)
        {
            MainWindow.RestartApp();
            this.Close();
        }




        // Create And Start Video Capture
        private void Start_cam()
        {
            this.camera.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.camera.VerticalAlignment = VerticalAlignment.Stretch;
            this.camera.Margin = new Thickness(0, 0, 0, 0);
            this.camera.Width = Double.NaN;
            this.camera.Height = Double.NaN;
            vidoe_grid.Children.Add(this.camera);
        }

    }
}
