using System;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Input;
using VisioForge.Types;
using System.Windows.Media;
using System.Windows.Controls;
using System.Net.Http.Headers;
using System.Net.Http;

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
            Face_det.IsChecked = (this.camera.Detection);
            Face_rec.IsChecked = (this.camera.Recognition);
            // Setup Brightness and Contrast Labels and Sliders
            brightness_label.Content = $"Brightness: {this.camera.Brightness}";
            contrast_label.Content   = $"Contrast:   {this.camera.Contrast}";
            darkness_label.Content   = $"Darkness:   {this.camera.Darkness}";
            brightness_slider.Value  = this.camera.Brightness;
            contrast_slider.Value    = this.camera.Contrast;
            darkness_slider.Value    = this.camera.Darkness;
            sensitivity_value_label.Content = $"{this.camera.On_move_sensitivity}";
            sensitivity_slider.Value = this.camera.On_move_sensitivity;
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
            // Setup On Movement checkboxes
            sms_checkbox.IsChecked = (this.camera.On_move_sms);
            email_checkbox.IsChecked = (this.camera.On_move_email);
            pic_checkbox.IsChecked = (this.camera.On_move_pic);
            rec_checkbox.IsChecked = (this.camera.On_move_rec);
            // Add Title
            cameras_title.Content = "Cameras Name: " + this.camera.name;
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

        // UP, DOWN, LEFT,RIGHT use Http request api to controll he camera
        private void GET_request(String url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                String result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            client.Dispose();
        }

        private void UP_button_click(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Mouse UP!");
            GET_request("URL_get rest UP");
        }

        private void DOWN_button_click(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Mouse DOWN!");
            GET_request("URL_get rest DOWN");
        }

        private void LEFT_button_click(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Mouse LEFT!");
            GET_request("URL_get rest LEFT");
        }

        private void RIGHT_button_click(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Mouse RIGHT!");
            GET_request("URL_get rest RIGHT");
        }

        private void TAKE_PIC_button_click(object sender, MouseButtonEventArgs e)
        {
            this.camera.Take_pic();
        }

        // Start Recording is checked
        private void Start_REC_button_click(object sender, MouseButtonEventArgs e)
        {
            if (!this.camera.Recording)
            {
                this.camera.Recording = true;
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
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Stop Recording is Checked
        private void Stop_REC_button_click(object sender, MouseButtonEventArgs e)
        {
            if ( this.camera.Recording)
            {
                this.camera.Recording = false;
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
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }



        // SMS CheckBoxes
        private void Sms_chencked(object sender, EventArgs e)
        {
            if (!this.camera.On_move_sms)
            {
                this.camera.On_move_sms = true;
                // Update DataBase this Camera Object field On_Move_SMS 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_SMS='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }
        private void Sms_unchencked(object sender, EventArgs e)
        {
            if (this.camera.On_move_sms)
            {
                this.camera.On_move_sms = false;
                // Update DataBase this Camera Object field On_Move_SMS 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_SMS='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Email CheckBoxes
        private void Email_chencked(object sender, EventArgs e)
        {
            if (!this.camera.On_move_email)
            {
                this.camera.On_move_email = true;
                // Update DataBase this Camera Object field On_Move_EMAIL 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_EMAIL='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }
        private void Email_unchencked(object sender, EventArgs e)
        {
            if (this.camera.On_move_email)
            {
                this.camera.On_move_email = false;
                // Update DataBase this Camera Object field On_Move_EMAIL 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_EMAIL='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Picture Checkbox
        private void Pic_chencked(object sender, EventArgs e)
        {
            if (!this.camera.On_move_pic)
            {
                this.camera.On_move_pic = true;
                // Update DataBase this Camera Object field On_Move_Pic 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_Pic='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }
        private void Pic_unchencked(object sender, EventArgs e)
        {
            if (this.camera.On_move_pic)
            {
                this.camera.On_move_pic = false;
                // Update DataBase this Camera Object field On_Move_Pic 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_Pic='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Recording Checkbox
        private void Rec_chencked(object sender, EventArgs e)
        {
            if (!this.camera.On_move_rec)
            {
                this.camera.On_move_rec = true;
                // Update DataBase this Camera Object field On_Move_Rec 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_Rec='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }
        private void Rec_unchencked(object sender, EventArgs e)
        {
            if (this.camera.On_move_rec)
            {
                this.camera.On_move_rec = false;
                // Update DataBase this Camera Object field On_Move_Rec 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_Rec='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Set The Sensitivity
        private void Sensitivity_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                // Update Cameras Move_Sensitivity
                int val = Convert.ToInt32(e.NewValue);
                sensitivity_value_label.Content = $"{val}";
                if (this.camera != null)
                {
                    this.camera.On_move_sensitivity = val;
                    Console.WriteLine(this.camera.On_move_sensitivity.ToString());
                    // Update DataBases Move_Sensitivity
                    SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                    String query = $"UPDATE dbo.myCameras SET Move_Sensitivity='{this.camera.On_move_sensitivity}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cn.Open();
                    int result = cmd.ExecuteNonQuery();
                    if (result < 0)
                        System.Windows.MessageBox.Show("Error inserting data into Database!");
                    cn.Close();
                }
            }
            catch (System.Data.SqlClient.SqlException) { }
            catch (System.NullReferenceException) { }
        }

    }
}
