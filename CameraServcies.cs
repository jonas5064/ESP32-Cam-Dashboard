using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisioForge.Controls.UI.WPF;
using VisioForge.Types;

namespace IPCamera
{
    public class CameraServcies
    {
        public VideoCapture video { get; set; }
        public string cameraId { get; set; }
        WindowControll Win_controll { get; set; }
        public bool Fullscreen { get; set; }
        public int On_move_recording_time { get; set; }
        public String Net_stream_ip { get; set; }
        public bool Running { get; set; }
        public VideoCapture Video { get; set; }
        HttpServer Server { get; set; }
        public bool Camera_oppened { get; set; }
        public CameraServcies(MyCamera camera)
        {
            this.cameraId = camera.Id;
            if (!camera.isEsp32)
            {
                this.video = new VideoCapture
                {
                    IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings()
                    {
                        URL = camera.urls,
                        Login = camera.username,
                        Password = camera.password
                    }
                };
            }
            else
            {
                this.video = new VideoCapture
                {
                    IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings()
                    {
                        URL = camera.urls
                    }
                };
            }
            this.video.OnError += OnError;
            this.video.MouseUp += CamerasFocused;
            this.video.Audio_PlayAudio = video.Audio_RecordAudio = false;
            this.video.Video_Effects_Enabled = true;
            this.video.IP_Camera_Source.Type = VisioForge.Types.VFIPSource.HTTP_MJPEG_LowLatency;
            this.video.Motion_Detection = new MotionDetectionSettings
            {
                Enabled = true,
                Highlight_Enabled = false
            };
            this.video.OnMotion += OnMotion;
            //camera.video.Video_Still_Frames_Grabber_Enabled = true;
            this.video.Video_FrameRate = camera.fps;
        }
        // TODO: Camera Start
        public void Start()
        {
            /*
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
            */
        }
        // TODO: Camera Stop
        public void Stop()
        {
            /*
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
            */
        }
        // TODO: Camera Take Picture
        public void Take_pic()
        {/*
            string picturesDirPath = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                      where f.Name.Equals("Pictures")
                                      select f.Path).FirstOrDefault();
            // Create Folder With Cameras Name for Name
            String dir_path = picturesDirPath + "\\" + this.Name;
            if (!Directory.Exists(dir_path))
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
            this.Video.Frame_Save(file, VisioForge.Types.VFImageFormat.JPEG, 85);*/
        }
        // TODO: Camera Start Recording
        public void StartRecording()
        {
            /*
            string videoDirPath = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                   where f.Name.Equals("Videos")
                                   select f.Path).FirstOrDefault();
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
                String dir_path = videoDirPath + "\\" + this.Name;
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
                houre = houre.Replace(":", ".");
                FilesFormat fileFormat = (from f in MainWindow.Main_window.DBModels.FilesFormats select f).FirstOrDefault();
                // AVI
                if (fileFormat.avi)
                {
                    String file = dir_path + "\\" + houre + ".avi";
                    this.Video.Output_Filename = file;
                    VFAVIOutput avi = new VFAVIOutput();
                    this.Video.Output_Format = new VFAVIOutput();
                }
                // MP4
                if (fileFormat.mp4)
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
            */
        }
        // TODO: Camera Stop Recording
        public void StopRecording()
        {/*
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
            }*/
        }
        private void OnError(object sender, VisioForge.Types.ErrorsEventArgs ex)
        {
            Console.WriteLine($@"\n\nOnError: Level:{ex.Level}\n
                                    StackTrace:{ex.StackTrace}\n
                                    Message:{ex.Message}\n\n");
        }
        // TODO: Camera Focused
        public void CamerasFocused(object sender, MouseButtonEventArgs e)
        {/*
            if (this.Running)
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    User user = (from u in MainWindow.DBModels.Users where u.Logged == true select u).FirstOrDefault();
                    if (MainWindow.Logged && user.Licences.Equals("Admin"))
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
            }*/
        }
        // This Happends when camera detectets a motion
        // TODO: Camera On Motion Detection
        DateTime last_email_date_onmove = DateTime.Now.AddMinutes(-1);
        public void OnMotion(object sender, MotionDetectionEventArgs e)
        {/*
            if (e.Level > this.On_move_sensitivity)
            {
                //Console.WriteLine($"Motion Detection!!!   Matrix: {e.Matrix.Length.ToString()}   Level: {e.Level}");
                if (this.On_move_email)
                {
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
                        List<User> users = (from u in MainWindow.DBModels.Users select u).ToList();
                        foreach (User u in users)
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
                            mailMessage.To.Add(new MailboxAddress(u.FirstName + " " + u.LastName, u.Email));
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
                        List<User> users = (from u in MainWindow.DBModels.Users select u).ToList();
                        foreach (User u in users)
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
            }*/
        }
    }
}
