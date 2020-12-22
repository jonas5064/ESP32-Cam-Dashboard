using System;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Input;
using VisioForge.Types;
using System.Windows.Media;
using System.Windows.Controls;

namespace IPCamera
{
    /// <summary>
    /// Interaction logic for WindowControll.xaml
    /// </summary>
    public partial class WindowControll : Window
    {
        public Camera camera;

        public WindowControll(Camera cam)
        {
            InitializeComponent();
            this.DataContext = this;
           
            // Setup this_camera
            this.camera = cam;
            // Chech if Face_Recognition, Face Detection  is checked
            Face_det.IsChecked = (this.camera.detection);
            Face_rec.IsChecked = (this.camera.recognition);
            // Setup Brightness and Contrast Labels and Sliders
            brightness_label.Content = $"Brightness: {this.camera.Brightness}";
            contrast_label.Content   = $"Contrast:   {this.camera.Contrast}";
            darkness_label.Content   = $"Darkness:   {this.camera.Darkness}";
            brightness_slider.Value  = this.camera.Brightness;
            contrast_slider.Value    = this.camera.Contrast;
            darkness_slider.Value    = this.camera.Darkness;
            // Setup recording Button
            if (this.camera.Recording)
            {
                rec_label.Content = "Recording";
                rec_label.Foreground = Brushes.Red;
            }
            else
            {
                rec_label.Content = "Stop Recording";
                rec_label.Foreground = Brushes.Gray;
            }
            // Add Title
            cameras_title.Content = this.camera.name;
        }



        protected override void OnClosed(EventArgs e)
        {
            this.Close();
        }


        // Face Detection Checked
        private void Face_Detection_Chencked(object sender, EventArgs e)
        {
            if (!this.camera.Detection)
            {
                this.camera.Detection = true;
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
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Face Detection Unchecked
        private void Face_Detection_UNChencked(object sender, EventArgs e)
        {
            if (this.camera.Detection)
            {
                this.camera.Detection = false;
                if (this.camera.Recognition)
                {
                    this.camera.Recognition = false;
                    Face_rec.IsChecked = (this.camera.recognition);
                }
                try
                {
                    // Update DataBase this Camera Object field Face Detection 0
                    SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                    String query = $"UPDATE dbo.MyCameras SET Face_Detection='{0}', Face_Recognition='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
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
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Face Recognition Chekced
        private void Face_Recognition_Chencked(object sender, EventArgs e)
        {
            if (!this.camera.Recognition)
            {
                this.camera.Recognition = true;
                if (!this.camera.Detection)
                {
                    this.camera.Detection = true;
                    Face_det.IsChecked = (this.camera.detection);
                }
                try
                {
                    // Update DataBase this Camera Object field Face Detection 1
                    SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                    String query = $"UPDATE dbo.MyCameras SET Face_Recognition='{1}', Face_Detection='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
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
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Face Recognition Unchecked
        private void Face_Recognition_UNChencked(object sender, EventArgs e)
        {
            if (this.camera.Recognition)
            {
                this.camera.Recognition = false;
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
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }


        // Britness slider function
        private void Brightness_func(object sender, RoutedPropertyChangedEventArgs<double> e)
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
        private void Contrast_func(object sender, RoutedPropertyChangedEventArgs<double> e)
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

        // Darkness slider function
        private void Darkness_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            darkness_label.Content = $"Darkness: {val}";
            this.camera.Darkness = val;
            // Save data to Database
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Darkness='{val}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
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
            this.camera.Take_pic();
        }

        private void Start_REC_button_click(object sender, MouseButtonEventArgs e)
        {
            this.camera.Recording = true;
            if (this.camera.Recording)
            {
                rec_label.Content = "Recording";
                rec_label.Foreground = Brushes.Red;
                // Update DataBase this Camera Object field Recording 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET Recording='{true}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            // Restart Camera
            this.camera.Stop();
            this.camera.Start();
        }

        private void Stop_REC_button_click(object sender, MouseButtonEventArgs e)
        {
            this.camera.Recording = false;
            if ( this.camera.Recording == false)
            {
                rec_label.Content = "Stop Recording";
                rec_label.Foreground = Brushes.Gray;
                // Update DataBase this Camera Object field Recording 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET Recording='{false}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            // Restart Camera
            this.camera.Stop();
            this.camera.Start();
        }
    }
}
