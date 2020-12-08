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



        private void Face_Detection_Chencked(object sender, EventArgs e)
        {
            System.Windows.MessageBox.Show("Face_Detection_Chencked!");
            // Update DataBase this Camera Object field Face Detection 1
        }
        private void Face_Detection_UNChencked(object sender, EventArgs e)
        {
            System.Windows.MessageBox.Show("Face_Detection_UNChencked!");
            // Update DataBase this Camera Object field Face Detection 0
        }

        private void Face_Recognition_Chencked(object sender, EventArgs e)
        {
            System.Windows.MessageBox.Show("Face_Recognition_Chencked!");
            // Update DataBase this Camera Object field Face Recognition 1
        }
        private void Face_Recognition_UNChencked(object sender, EventArgs e)
        {
            System.Windows.MessageBox.Show("Face_Recognition_UNChencked!");
            // Update DataBase this Camera Object field Face Recognition 0
        }


        private void UP_button_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("Mouse UP!");
        }

        private void DOWN_button_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("Mouse DOWN!");
        }

        private void LEFT_button_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("Mouse LEFT!");
        }

        private void RIGHT_button_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("Mouse RIGHT!");
        }

        private void TAKE_PIC_button_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("Mouse TAKE PICTURE!");
        }

    }
}
