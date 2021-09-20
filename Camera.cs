using System;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using VisioForge.Controls.UI.WPF;
using VisioForge.Types;
using VisioForge.Types.OutputFormat;
// https://help.visioforge.com/sdks_net/html/T_VisioForge_Controls_UI_WPF_VideoCapture.htm
using VisioForge.Types.VideoEffects;
using MimeKit;
using MimeKit.Utils;
using System.Globalization;

namespace IPCamera
{
    public class Camera
    {
        public String Url { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public bool IsEsp32 { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        WindowControll Win_controll { get; set; }
        public bool Fullscreen { get; set; }
        public int Row { get; set; }
        public int Coll { get; set; }
        public int On_move_recording_time { get; set; }
        public String Net_stream_ip { get; set; }
        public String Net_stream_port { get; set; }
        public String Net_stream_prefix { get; set; }
        public bool Running { get; set; }
        public VideoCapture Video { get; set; }    
        public static int Count { get; set; }
        public static String Pictures_dir { get; set; }
        public static String Videos_dir { get; set; }
        public static bool Avi_format { get; set; }
        public static bool Mp4_format { get; set; }
        public String Up_req { get; set; }
        public String Down_req { get; set; }
        public String Right_req { get; set; }
        public String Left_req { get; set; }
        HttpServer Server { get; set; }
        public bool Camera_oppened { get; set; }
        public bool Recognition { get; set; }
        public bool On_move_sms { get; set; }
        public bool On_move_email { get; set; }
        public bool On_move_pic { get; set; }
        public bool On_move_rec { get; set; }
        public int On_move_sensitivity { get; set; }
        // Set Frame Rates
        private int _framerate = 0;
        public int Framerate
        {
            get
            {
                return this._framerate;
            }
            set
            {
                this._framerate = value;
                this.Video.Video_FrameRate = this._framerate;
            }
        }
        // Setup Brightness Effext
        private int _brightness = 0;
        public int Brightness
        {
            get { return this._brightness; }
            set
            {
                this._brightness = value;
                // Add the efect
                IVFVideoEffectLightness lightness;
                var effect_l = this.Video.Video_Effects_Get("Lightness");
                if (effect_l == null)
                {
                    lightness = new VFVideoEffectLightness(true, this._brightness, 0, "Lightness");
                    this.Video.Video_Effects_Add(lightness);
                }
                else
                {
                    lightness = effect_l as IVFVideoEffectLightness;
                    if (lightness != null)
                    {
                        lightness.Value = this._brightness;
                    }
                }
            }
        }
        // Setup The Net Stream
        private bool _net_stream = false;
        public bool Net_stream
        {
            get { return this._net_stream; }
            set { 
                this._net_stream = value;
                if (this.Net_stream_ip.Length > 0 && this.Net_stream_port.Length > 0)
                {
                    Console.WriteLine("Server,Run: " + Convert.ToString(this.Net_stream));
                    if (this.Net_stream)
                    {
                        Server = new HttpServer(this);
                        this.Server.Run = this.Net_stream;
                        _ = this.Server.ListenAsync();
                    }
                    else
                    {
                        if (this.Server != null)
                        {
                            Console.WriteLine("Try to Stop The Server.");
                            if (this.Server.Run)
                            {
                                Console.WriteLine("Server is Stoping");
                                this.Server.Close();
                                Console.WriteLine("Server Stoped.");
                            }
                        }
                    }
                }
            }
        }
        // Setup Contrast Effext
        private int _contrast = 0;
        public int Contrast
        {
            get { return this._contrast; }
            set
            {
                this._contrast = value;
                // Add the efect
                IVFVideoEffectContrast contrast;
                var effect_c = this.Video.Video_Effects_Get("Contrast");
                if (effect_c == null)
                {
                    contrast = new VFVideoEffectContrast(true, this._contrast, 0, "Contrast");
                    this.Video.Video_Effects_Add(contrast);
                }
                else
                {
                    contrast = effect_c as IVFVideoEffectContrast;
                    if (contrast != null)
                    {
                        contrast.Value = this._contrast;
                    }
                }
            }
        }
        // Setup Drkness Effext
        private int _darkness = 0;
        public int Darkness
        {
            get { return this._darkness; }
            set
            {
                this._darkness = value;
                // Add the efect
                IVFVideoEffectDarkness darkness;
                var effect_d = this.Video.Video_Effects_Get("Darkness");
                if (effect_d == null)
                {
                    darkness = new VFVideoEffectDarkness(true, this._darkness, 0, "Darkness");
                    this.Video.Video_Effects_Add(darkness);
                }
                else
                {
                    darkness = effect_d as IVFVideoEffectDarkness;
                    if (darkness != null)
                    {
                        darkness.Value = this._darkness;
                    }
                }
            }
        }
        // When Change Detection
        private bool _detection = false;
        public bool Detection
        {
            get { return this._detection; }
            set
            {
                this._detection = value;
                if (this._detection)
                {
                    /*
                    this.video.Face_Tracking = new FaceTrackingSettings
                    {
                        ColorMode = CamshiftMode.RGB,
                        Highlight = true,
                        MinimumWindowSize = 25,
                        ScalingMode = ObjectDetectorScalingMode.GreaterToSmaller,
                        ScaleFactor = (float)1.7,
                        SearchMode = ObjectDetectorSearchMode.Single
                    };
                    */
                    this.Video.Face_Tracking = new FaceTrackingSettings()
                    {
                        Highlight = true
                    };
                    this.Video.OnFaceDetected += (object sender, AFFaceDetectionEventArgs e) =>
                    {
                        foreach (Rectangle faceRectangle in e.FaceRectangles)
                        {
                            // If Recognition is enable
                            if (this.Recognition)
                            {

                            }
                            else
                            {
                                Console.WriteLine($"Face Detection:   left-right({faceRectangle.Left}, {faceRectangle.Right}), " +
                                    $"top-bottom({faceRectangle.Top}, {faceRectangle.Bottom}),  width-height({faceRectangle.Width}, " +
                                    $"{faceRectangle.Height})  {Environment.NewLine}");
                            }
                        }
                    };
                }
                else
                {
                    this.Video.Face_Tracking = new FaceTrackingSettings();
                    this.Video.OnFaceDetected += (object sender, AFFaceDetectionEventArgs e) => { };
                }
            }
        }
        // Start / Stop Recording
        private bool _recording = false;
        public bool Recording
        {
            get { return this._recording; }
            set { 
                this._recording = value;
                if (this._recording)
                {
                    Console.WriteLine("Start Recording.");
                    this.StartRecording();
                }
                else
                {
                    Console.WriteLine("Stop Recording.");
                    this.StopRecording();
                }
            }
        }

        public Camera(String url, String name, String id, bool rec, bool isesp)
        {
            this.Url = url;
            this.Name = name;
            this.Id = id;
            this.Net_stream_ip = "localhost";
            this.Recognition = false;
            this.On_move_sms = false;
            this.On_move_email = false;
            this.On_move_pic = false;
            this.On_move_rec = false;
            this.On_move_sensitivity = 4;
            this.IsEsp32 = false;
            this.Fullscreen = false;
            this.On_move_recording_time = 10000;
            this.Running = false;
            Avi_format = false;
            Mp4_format = false;
            this.Camera_oppened = false;
            this.IsEsp32 = isesp;

            // Create an VideoCapture
            if (!this.IsEsp32)
            {
                this.Video = new VideoCapture
                {
                    IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings()
                    {
                        URL = this.Url,
                        Login = this.Username,
                        Password = this.Password
                    }
                };
            }
            else
            {
                this.Video = new VideoCapture
                {
                    IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings()
                    {
                        URL = this.Url/*,
                        Type = VisioForge.Types.VFIPSource.Auto_LAV*/
                    }
                };
            }
            this.Video.OnError += OnError;
            this.Video.MouseUp += CamerasFocused;
            this.Video.Audio_PlayAudio = this.Video.Audio_RecordAudio = false;
            this.Video.Video_Effects_Enabled = true;
            this.Video.IP_Camera_Source.Type = VisioForge.Types.VFIPSource.HTTP_MJPEG_LowLatency;
            // Motion Detection Setup
            this.Video.Motion_Detection = new MotionDetectionSettings
            {
                Enabled = true,
                Highlight_Enabled = false
            };
            this.Video.OnMotion += this.OnMotion;
            //this.video.Video_Still_Frames_Grabber_Enabled = true;
            this.Framerate = 33;
            this.Video.Video_FrameRate = this.Framerate;
            // Set Recording Variable
            this.Recording = rec;
            Count++;
        }


        ~Camera()
        {
            Count--;
        }

        // Start the Camera
        public void Start()
        {
            if (this.Video.Status != VisioForge.Types.VFVideoCaptureStatus.Work)
            {
                try
                {
                    Console.WriteLine($"Camera Start.");
                    this.Video.Start();
                    this.Running = true;
                    //this.video.StartAsync();
                }
                catch (System.AccessViolationException ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    Console.WriteLine($"OnStart: Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                }
            }
        }

        // Stop The Camera
        public void Stop()
        {
            if (this.Video.Status == VisioForge.Types.VFVideoCaptureStatus.Work)
            {
                try
                {
                    Console.WriteLine($"Camera Stop.");
                    this.Video.Stop();
                    this.Running = false;
                    //this.video.StopAsync();
                }
                catch (System.AccessViolationException ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    Console.WriteLine($"OnStop: Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                }

            }
        }

        // Take Picture
        public void Take_pic()
        {
            // Create Folder With Cameras Name for Name
            String dir_path = Camera.Pictures_dir + "\\" + this.Name;
            if (! Directory.Exists(dir_path))
            {
                Directory.CreateDirectory(dir_path);
            }
            DateTime now = DateTime.Now;
            // Create SubFolder With The Current Date
            String date = now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE"));
            date = date.Replace(".", "-");
            dir_path += "\\" + date;
            if (!Directory.Exists(dir_path))
            {
                Directory.CreateDirectory(dir_path);
            }
            // Create The JPeg File With the Current Time For Name
            String houre = now.ToString("T", CultureInfo.CreateSpecificCulture("de-DE"));
            houre = houre.Replace(":", ".");
            String file = dir_path + "\\" + houre + ".jpg";
            Console.WriteLine($"\n\nCreate File: {file}\n\n");
            this.Video.Frame_Save(file, VisioForge.Types.VFImageFormat.JPEG, 85);
        }


        // Start Recording
        public void StartRecording()
        {
            try
            {
                bool was_running = this.Running;
                if (this.Running)
                {
                    this.Video.Stop();
                }
                // Video mode == capture
                this.Video.Mode = VisioForge.Types.VFVideoCaptureMode.IPCapture;
                // Create Dir With Cameras Name
                String dir_path = Camera.Videos_dir + "\\" + this.Name;
                if (!Directory.Exists(dir_path)) // Directory with the name of the camera
                {
                    Directory.CreateDirectory(dir_path);
                }
                // Create Dir With Current Date For Name
                DateTime now = DateTime.Now;
                String date = now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE"));
                date = date.Replace(".", "-");
                dir_path += "\\" + date;
                Console.WriteLine($"Camera Start Recording.\nDate Dir: {dir_path}");
                if (!Directory.Exists(dir_path)) // Directory with the name of the camera
                {
                    Directory.CreateDirectory(dir_path);
                }
                // Start Recording and Creating The Video Files
                String houre = now.ToString("T", CultureInfo.CreateSpecificCulture("de-DE"));
                houre = houre.Replace(":",".");
                // AVI
                if (Avi_format)
                {
                    String file = dir_path + "\\" + houre + ".avi";
                    this.Video.Output_Filename = file;
                    VFAVIOutput avi = new VFAVIOutput();
                    this.Video.Output_Format = new VFAVIOutput();
                }
                // MP4
                if (Mp4_format)
                {
                    String file = dir_path + "\\" + houre + ".mp4";
                    this.Video.Output_Filename = file;
                    this.Video.Output_Format = new VFMP4v8v10Output();
                }
                if (this.Running && was_running)
                {
                    this.Video.Start();
                }
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Console.WriteLine($"SetupRecordingMode: Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
            }
        }

        // Stop Recording
        public void StopRecording()
        {
            bool was_running = this.Running;
            if (this.Running)
            {
                this.Video.Stop();
            }
            this.Video.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
            Console.WriteLine($"Camera Stop Recording");
            if (this.Running && was_running)
            {
                this.Video.Start();
            }
        }

        // On Error EVnt
        private void OnError(object sender, VisioForge.Types.ErrorsEventArgs ex)
        {
            Console.WriteLine($"\n\nOnError: Level:{ex.Level}\nStackTrace:{ex.StackTrace}\nMessage:{ex.Message}\n\n");
            //throw new NotImplementedException();
        }

        // When click on camera
        public void CamerasFocused(object sender, MouseButtonEventArgs e)
        {
            if (this.Running)
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    if ( MainWindow.Logged && MainWindow.MyUsers.Contains(MainWindow.User) && (MainWindow.User.Licences.Equals("Admin")) )
                    {
                        if (this.Camera_oppened == false)
                        {
                            this.Camera_oppened = true;
                            this.Win_controll = new WindowControll(this);
                            Win_controll.Show();
                        }
                        else
                        {
                            this.Win_controll.Activate();
                        }
                    }
                }
                if (e.ChangedButton == MouseButton.Left)
                {
                    // Open A new Window And Show This Camera FullScreen
                    if (!this.Fullscreen)
                    {
                        MainWindow.cams_grid.Children.Remove(this.Video);
                        this.Fullscreen = true;
                        VideoFullscreen fullscreen = new VideoFullscreen(this);
                        fullscreen.Show();
                    }
                }
            }
        }

        // This Happends when camera detectets a motion
        DateTime last_email_date_onmove = DateTime.Now.AddMinutes(-1);
        public void OnMotion(object sender, MotionDetectionEventArgs e)
        {
            if (e.Level > this.On_move_sensitivity)
            {
                //Console.WriteLine($"Motion Detection!!!   Matrix: {e.Matrix.Length.ToString()}   Level: {e.Level}");
                if (this.On_move_email)
                {
                    Console.WriteLine($"Motion Detected Send Email Message.  [Before] Time.now: {DateTime.Now} " +
                        $"Time.before: {last_email_date_onmove.AddMinutes(1)}");

                    // When Send Get the DateTime
                    if (DateTime.Now > last_email_date_onmove.AddMinutes(1))
                    {
                        last_email_date_onmove = DateTime.Now;

                        // Return to Sending Email
                        String host = "";
                        int port = 587;
                        String fromEmail = MainWindow.Email_send;
                        String fromPassword = MainWindow.Pass_send;
                        String subject = this.Name;

                        if (fromEmail.Contains("gmail"))
                        {
                            host = "smtp.gmail.com";
                        }
                        else if (fromEmail.Contains("yahoo"))
                        {
                            host = "imap.mail.yahoo.com";
                        }
                        else if (fromEmail.Contains("live"))
                        {
                            host = "smtp.live.com";
                        }

                        // Create a File with a pic
                        String img_name = "email_pic.jpeg";
                        this.Video.Frame_Save(img_name, VisioForge.Types.VFImageFormat.JPEG, 100, 300, 300);

                        // Add All Recievers
                        foreach (Users u in MainWindow.MyUsers)
                        {

                            // Create HTML Body
                            var builder = new BodyBuilder();
                            var pathImage = Path.Combine(Directory.GetCurrentDirectory(), img_name);
                            var image = builder.LinkedResources.Add(pathImage);
                            image.ContentId = MimeUtils.GenerateMessageId();
                            builder.HtmlBody = string.Format($"<html>" +
                                                                   "<head>" +
                                                                   "</head" +
                                                                   "<body>" +
                                                                       "<h1>" + $"[{this.Name}]" + "</h1>" +
                                                                       "<h3>" + "Detect Motion at:" + "</h3>" +
                                                                       "<h2>" + $"[{DateTime.Now}]" + "</h2>" +
                                                                       @"<img src=""cid:{0}"">" +
                                                                   "</body>" +
                                                               "</head>"
                                                                , image.ContentId
                                                            );

                            // Create Email Message
                            var mailMessage = new MimeMessage();
                            mailMessage.From.Add(new MailboxAddress("Officee", fromEmail));
                            mailMessage.To.Add(new MailboxAddress(u.Firstname + " " + u.Lastname, u.Email));
                            mailMessage.Subject = subject;
                            mailMessage.Body = builder.ToMessageBody();

                            // Send Email Message
                            using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
                            {
                                Console.WriteLine($"\nHost: {host}    Email: {fromEmail}    Password: {fromPassword}\n");
                                smtpClient.Connect(host, port, false);
                                smtpClient.Authenticate(fromEmail, fromPassword);
                                smtpClient.Send(mailMessage);
                                smtpClient.Disconnect(true);
                            }
                        }

                        // Delete The Image
                        File.Delete(img_name);
                    }
                }
                if (this.On_move_pic)
                {
                    Console.WriteLine("Take Picture.");
                    this.Take_pic();
                }
                if (this.On_move_rec)
                {
                    
                }
                if (this.On_move_sms)
                {
                    // Send SMS
                    if (DateTime.Now > last_email_date_onmove.AddMinutes(1))
                    {
                        last_email_date_onmove = DateTime.Now;
                        // Find your Account Sid and Token at https://account.apifonica.com/
                        Console.WriteLine($"Before Send SMS.   ssid: {MainWindow.TwilioAccountSID}   token: {MainWindow.TwilioAccountToken}");
                        TwilioClient.Init(MainWindow.TwilioAccountSID, MainWindow.TwilioAccountToken);
                        foreach (Users u in MainWindow.MyUsers)
                        {
                            var message = MessageResource.Create(
                                body: $"[{this.Name}]  Detect Motion at  [{DateTime.Now}]",
                                from: new Twilio.Types.PhoneNumber(MainWindow.TwilioNumber),
                                to: new Twilio.Types.PhoneNumber(u.Phone)
                            );
                            Console.WriteLine($"Send SMS To: {message.Sid}.");
                        }
                    }
                }
            }
        }


        /*
        public static Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }
        */
    }
}
