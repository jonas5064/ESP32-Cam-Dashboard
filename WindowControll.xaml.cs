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
using System.Data.SqlClient;


namespace IPCamera
{
    /// <summary>
    /// Interaction logic for WindowControll.xaml
    /// </summary>
    public partial class WindowControll : Window
    {
        private Camera camera;
        public int brightness = 0;



        public WindowControll(Camera cam)
        {
            InitializeComponent();
            this.DataContext = this;

            // Setup this_camera
            this.camera = cam;
            // Chech if Face_Recognition, Face Detection  is checked
            Face_det.IsChecked = (camera.detection ? true : false);
            Face_rec.IsChecked = (camera.recognition ? true : false);
            // Setup Brightness and Contrast Labels and Sliders
            brightness_label.Content = $"Brightness: {this.camera.Brightness.ToString()}";
            contrast_label.Content   = $"Contrast:   {this.camera.Contrast.ToString()}";
            brightness_slider.Value  = this.camera.Brightness;
            contrast_slider.Value    = this.camera.Contrast;
            // Start Camera
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
            this.camera.video.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.camera.video.VerticalAlignment = VerticalAlignment.Stretch;
            this.camera.video.Margin = new Thickness(0, 0, 0, 0);
            this.camera.video.Width = Double.NaN;
            this.camera.video.Height = Double.NaN;
            vidoe_grid.Children.Add(this.camera.video);
        }


        // Face Detection Checked
        private void Face_Detection_Chencked(object sender, EventArgs e)
        {
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Face_Detection='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }

        // Face Detection Unchecked
        private void Face_Detection_UNChencked(object sender, EventArgs e)
        {
            try
            {
                // Update DataBase this Camera Object field Face Detection 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Face_Detection='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }

        // Face Recognition Chekced
        private void Face_Recognition_Chencked(object sender, EventArgs e)
        {
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Face_Recognition='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }

        // Face Recognition Unchecked
        private void Face_Recognition_UNChencked(object sender, EventArgs e)
        {
            try
            {
                // Update DataBase this Camera Object field Face Detection 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Face_Recognition='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }


        // Britness slider function
        private void brightness_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            brightness_label.Content = $"Brightness: {val}";
            this.camera.Brightness = val;
            // Save data to Database
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Brightness='{val}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }


        // Contrast slider function
        private void contrast_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            contrast_label.Content = $"Contrast: {val}";
            this.camera.Contrast = val;
            // Save data to Database
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Contrast='{val}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
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
