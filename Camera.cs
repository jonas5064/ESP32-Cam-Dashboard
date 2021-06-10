using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows.Input;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Threading;

//using VisioForge.Controls.UI.WinForms;
using VisioForge.Controls.UI.WPF;
using VisioForge.Types;
using VisioForge.Types.OutputFormat;
// https://help.visioforge.com/sdks_net/html/T_VisioForge_Controls_UI_WPF_VideoCapture.htm
using VisioForge.Types.VideoEffects;
using MailKit;
using MailKit.Security;
using MimeKit;
using System.Windows.Media.Imaging;
using MimeKit.Utils;

namespace IPCamera
{
    public class Camera
    {
        public String url = "";
        public string name = "";
        public string id = "";
        private string username = "";
        private string password = "";
        public bool isEsp32 = false;
        WindowControll win_controll;
        public int row = 0;
        public int coll = 0;
        private bool detection = false;
        private bool recognition = false;
        private bool on_move_sms = false;
        private bool on_move_email = false;
        private bool on_move_pic = false;
        private bool on_move_rec = false;
        private int on_move_sensitivity = 4;
        private int brightness = 0;
        private int contrast = 0;
        private int darkness = 0;
        private bool recording = false;
        public bool running = false;
        public VideoCapture video;     
        public static int count = 0;
        public static String pictures_dir;
        public static String videos_dir;

        public static bool avi_format = false;
        public static bool mp4_format = false;
        System.Timers.Timer recordingTimer;

        public String up_req = "";
        public String down_req = "";
        public String right_req = "";
        public String left_req = "";
        private String net_stream_ip = "localhost";
        private String net_stream_port = "";
        private String net_stream_prefix = "";
        private bool net_stream = false;
        HttpServer server;
        public bool camera_oppened = false;
        public int framerate = 0;

        public Camera(String url, String name, String id, bool rec)
        {
            this.url = url;
            this.name = name;
            this.id = id;

            // Create an VideoCapture
            if (!this.isEsp32)
            {
                this.video = new VideoCapture
                {
                    IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings()
                    {
                        URL = this.url,
                        Login = this.Username,
                        Password = this.Password
                    }
                };
            } else
            {
                this.video = new VideoCapture
                {
                    IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings()
                    {
                        URL = this.url/*,
                        Type = VisioForge.Types.VFIPSource.Auto_LAV*/
                    }
                };
            }
            this.video.OnError += OnError;
            this.video.MouseUp += CamerasFocused;
            this.video.Audio_PlayAudio = this.video.Audio_RecordAudio = false;
            this.video.Video_Effects_Enabled = true;
            this.video.IP_Camera_Source.Type = VisioForge.Types.VFIPSource.HTTP_MJPEG_LowLatency;
            // Motion Detection Setup
            this.video.Motion_Detection = new MotionDetectionSettings
            {
                Enabled = true,
                Highlight_Enabled = false
            };
            this.video.OnMotion += this.OnMotion;
            //this.video.Video_Still_Frames_Grabber_Enabled = true;
            this.Framerate = 33;
            this.video.Video_FrameRate = this.framerate;
            // Set Recording Variable
            this.Recording = rec;
            count++;
        }


        ~Camera()
        {
            count--;
        }

        // Return this video capture
        public VideoCapture Get()
        {
            return this.video;
        }

        // Set Frame Rates
        public int Framerate
        {
            get
            {
                return this.framerate;
            }
            set
            {
                this.framerate = value;
                this.video.Video_FrameRate = this.framerate;
            }
        }


        // Setup Username
        public String Username
        {
            get { return this.username; }
            set { this.username = value; }
        }

        // Setup Password
        public String Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        // Setup Brightness Effext
        public int Brightness
        {
            get { return this.brightness; }
            set
            {
                this.brightness = value;
                // Add the efect
                IVFVideoEffectLightness lightness;
                var effect_l = this.video.Video_Effects_Get("Lightness");
                if (effect_l == null)
                {
                    lightness = new VFVideoEffectLightness(true, this.brightness, 0, "Lightness");
                    this.video.Video_Effects_Add(lightness);
                }
                else
                {
                    lightness = effect_l as IVFVideoEffectLightness;
                    if (lightness != null)
                    {
                        lightness.Value = this.brightness;
                    }
                }
            }
        }

        public bool Net_stream
        {
            get { return this.net_stream; }
            set { 
                this.net_stream = value;
                /*
                if (this.Net_stream_ip.Length > 0 && this.Net_stream_port.Length > 0)
                {
                    if (this.Net_stream)
                    {
                        //server = new HttpServer(this, this.Net_stream_ip, this.Net_stream_port, this.Net_stream_prefix);
                        //this.server.run = this.Net_stream;
                        Console.WriteLine("Server,Run: " + Convert.ToString(this.Net_stream));
                        //var result = this.server.ListenAsync();
                    }
                    else
                    {
                        if(this.server.run)
                        {
                            this.server.close();
                        }
                    }
                }
                */
            }
        }

        public String Net_stream_ip
        {
            get { return this.net_stream_ip; }
            set { this.net_stream_ip = value; }
        }

        public String Net_stream_port
        {
            get { return this.net_stream_port; }
            set { this.net_stream_port = value; }
        }

        public String Net_stream_prefix
        {
            get { return this.net_stream_prefix; }
            set { this.net_stream_prefix = value; }
        }

        // Setup Contrast Effext
        public int Contrast
        {
            get { return this.contrast; }
            set
            {
                this.contrast = value;
                // Add the efect
                IVFVideoEffectContrast contrast;
                var effect_c = this.video.Video_Effects_Get("Contrast");
                if (effect_c == null)
                {
                    contrast = new VFVideoEffectContrast(true, this.contrast, 0, "Contrast");
                    this.video.Video_Effects_Add(contrast);
                }
                else
                {
                    contrast = effect_c as IVFVideoEffectContrast;
                    if (contrast != null)
                    {
                        contrast.Value = this.contrast;
                    }
                }
            }
        }

        // Setup Drkness Effext
        public int Darkness
        {
            get { return this.darkness; }
            set
            {
                this.darkness = value;
                // Add the efect
                IVFVideoEffectDarkness darkness;
                var effect_d = this.video.Video_Effects_Get("Darkness");
                if (effect_d == null)
                {
                    darkness = new VFVideoEffectDarkness(true, this.darkness, 0, "Darkness");
                    this.video.Video_Effects_Add(darkness);
                }
                else
                {
                    darkness = effect_d as IVFVideoEffectDarkness;
                    if (darkness != null)
                    {
                        darkness.Value = this.darkness;
                    }
                }
            }
        }

        // When Change Detection
        public bool Detection
        {
            get { return this.detection; }
            set
            {
                this.detection = value;
                if (this.detection)
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
                    this.video.Face_Tracking = new FaceTrackingSettings()
                    {
                        Highlight = true
                    };
                    this.video.OnFaceDetected += (object sender, AFFaceDetectionEventArgs e) =>
                    {
                        foreach (Rectangle faceRectangle in e.FaceRectangles)
                        {
                            // If Recognition is enable
                            if (this.recognition)
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
                    this.video.Face_Tracking = new FaceTrackingSettings();
                    this.video.OnFaceDetected += (object sender, AFFaceDetectionEventArgs e) => { };
                }
            }
        }

        // When Change recognition
        public bool Recognition
        {
            get { return this.recognition; }
            set { this.recognition = value; }
        }

        // Start / Stop Recording
        public bool Recording
        {
            get { return this.recording; }
            set { 
                this.recording = value;
                if (this.recording)
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

        // On Move SmS
        public bool On_move_sms
        {
            get { return this.on_move_sms; }
            set { this.on_move_sms = value; }
        }

        // On Move Email
        public bool On_move_email
        {
            get { return this.on_move_email; }
            set { this.on_move_email = value; }
        }

        // On Move Pic
        public bool On_move_pic
        {
            get { return this.on_move_pic; }
            set { this.on_move_pic = value; }
        }
        
        // On Move Rec
        public bool On_move_rec
        {
            get { return this.on_move_rec; }
            set { this.on_move_rec = value; }
        }

        // Setup On Move Sensitivity
        public int On_move_sensitivity
        {
            get { return this.on_move_sensitivity; }
            set { this.on_move_sensitivity = value; }
        }

        // Start the Camera
        public void Start()
        {
            if (this.video.Status != VisioForge.Types.VFVideoCaptureStatus.Work)
            {
                try
                {
                    Console.WriteLine($"Camera Start.");
                    this.video.Start();
                    this.running = true;
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
            if (this.video.Status == VisioForge.Types.VFVideoCaptureStatus.Work)
            {
                try
                {
                    Console.WriteLine($"Camera Stop.");
                    this.video.Stop();
                    this.running = false;
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
            DateTime now = DateTime.Now;
            String date = now.ToString("F");
            date = date.Replace(":", ".");
            String dir_path = Camera.pictures_dir + "\\" + this.name;
            if (! Directory.Exists(dir_path))
            {
                Directory.CreateDirectory(dir_path);
            }
            String file = dir_path + "\\" + date + ".jpg";
            this.video.Frame_Save(file, VisioForge.Types.VFImageFormat.JPEG, 85);
        }


        // Start Recording
        public void StartRecording()
        {
            try
            {
                bool was_running = this.running ? true : false;
                if (this.running)
                {
                    this.video.Stop();
                }
                // Video mode == capture
                this.video.Mode = VisioForge.Types.VFVideoCaptureMode.IPCapture;
                // Setup the right file name
                DateTime now = DateTime.Now;
                String date = now.ToString("F");
                date = date.Replace(":", ".");
                String dir_path = Camera.videos_dir + "\\" + this.name;
                Console.WriteLine($"\nRecording File Path:  {dir_path}\n");
                if (!Directory.Exists(dir_path)) // Directory with the name of the camera
                {
                    Directory.CreateDirectory(dir_path);
                }
                // Start Recording
                // AVI
                if (avi_format)
                {
                    String file = dir_path + "\\" + date + ".avi";
                    this.video.Output_Filename = file;
                    VFAVIOutput avi = new VFAVIOutput();
                    this.video.Output_Format = new VFAVIOutput();
                }
                // MP4
                if (mp4_format)
                {
                    String file = dir_path + "\\" + date + ".mp4";
                    this.video.Output_Filename = file;
                    this.video.Output_Format = new VFMP4v8v10Output();
                }
                if (this.running && was_running)
                {
                    this.video.Start();
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
            bool was_running = this.running ? true : false;
            if (this.running)
            {
                this.video.Stop();
            }
            this.video.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
            Console.WriteLine($"Camera Stop Recording");
            if (this.running && was_running)
            {
                this.video.Start();
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
            if (MainWindow.Logged && MainWindow.myUsers.Contains(MainWindow.user)
                && (MainWindow.user.Licences.Equals("Admin")))
            {
                if (this.camera_oppened == false)
                {
                    this.camera_oppened = true;
                    this.win_controll = new WindowControll(this);
                    win_controll.Show();
                }
                else
                {
                    this.win_controll.Activate();
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
                        String fromEmail = MainWindow.email_send;
                        String fromPassword = MainWindow.pass_send;
                        String subject = this.name;

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
                        this.video.Frame_Save(img_name, VisioForge.Types.VFImageFormat.JPEG, 100, 300, 300);

                        // Add All Recievers
                        foreach (Users u in MainWindow.myUsers)
                        {
                            /// Create HTML Body
                            var builder = new BodyBuilder();
                            var pathImage = Path.Combine(Directory.GetCurrentDirectory(), img_name);
                            var image = builder.LinkedResources.Add(pathImage);
                            image.ContentId = MimeUtils.GenerateMessageId();
                            builder.HtmlBody = string.Format($"<html>" +
                                                                   "<head>" +
                                                                   "</head" +
                                                                   "<body>" +
                                                                       "<h1>" + $"[{this.name}]" + "</h1>" +
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
                        Console.WriteLine($"Before Send SMS.   ssid: {MainWindow.twilioAccountSID}   token: {MainWindow.twilioAccountToken}");
                        TwilioClient.Init(MainWindow.twilioAccountSID, MainWindow.twilioAccountToken);
                        foreach (Users u in MainWindow.myUsers)
                        {
                            var message = MessageResource.Create(
                                body: $"[{this.name}]  Detect Motion at  [{DateTime.Now}]",
                                from: new Twilio.Types.PhoneNumber(MainWindow.twilioNumber),
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
