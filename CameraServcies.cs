using MimeKit;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using VisioForge.Controls.UI.WPF;
using VisioForge.Types;
using VisioForge.Types.OutputFormat;

namespace IPCamera
{
    public class CameraServcies
    {
        public VideoCapture video { get; set; }
        public string cameraId { get; set; }
        WindowControll Win_controll { get; set; }
        public bool Fullscreen { get; set; }
        public bool Running { get; set; }
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
            this.video.Audio_PlayAudio = this.video.Audio_RecordAudio = false;
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
        public void Start() // TODO: CameraServicies: Line[68], Function[Start], ERROR[this.video.Start() == NullRefrencesException].
        {
            if (this.video.Status == VisioForge.Types.VFVideoCaptureStatus.Free)
            {
                try
                {
                    this.video.Start();
                    this.Running = true;
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
        public void Stop()
        {
            if (this.video.Status == VisioForge.Types.VFVideoCaptureStatus.Work)
            {
                try
                {
                    this.video.Stop();
                    this.Running = false;
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
        public void Take_pic()
        {
            string picturesDirPath = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                      where f.Name.Equals("Pictures")
                                      select f.Path).FirstOrDefault();
            // Create Folder With Cameras Name for Name
            string camName = (from c in MainWindow.Main_window.DBModels.MyCameras where c.Id == this.cameraId select c.name).FirstOrDefault();
            String dir_path = picturesDirPath + "\\" + camName;
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
            this.video.Frame_Save(file, VisioForge.Types.VFImageFormat.JPEG, 85);
        }
        public void StartRecording()
        {
            string videoDirPath = (from f in MainWindow.Main_window.DBModels.FilesDirs
                                   where f.Name.Equals("Videos")
                                   select f.Path).FirstOrDefault();
            try
            {
                bool was_running = this.Running;
                if (this.Running)
                {
                    this.video.Stop();
                }
                // Video mode == capture
                this.video.Mode = VisioForge.Types.VFVideoCaptureMode.IPCapture;
                // Create Dir With Cameras Name
                string camName = (from c in MainWindow.Main_window.DBModels.MyCameras where c.Id == this.cameraId select c.name).FirstOrDefault();
                String dir_path = videoDirPath + "\\" + camName;
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
                    this.video.Output_Filename = file;
                    VFAVIOutput avi = new VFAVIOutput();
                    this.video.Output_Format = new VFAVIOutput();
                }
                // MP4
                if (fileFormat.mp4)
                {
                    String file = dir_path + "\\" + houre + ".mp4";
                    this.video.Output_Filename = file;
                    this.video.Output_Format = new VFMP4v8v10Output();
                }
                if (this.Running && was_running)
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
        public void StopRecording()
        {
            bool was_running = this.Running;
            if (this.Running)
            {
                this.video.Stop();
            }
            this.video.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
            Console.WriteLine($"Camera Stop Recording");
            if (this.Running && was_running)
            {
                this.video.Start();
            }
        }
        private void OnError(object sender, VisioForge.Types.ErrorsEventArgs ex)
        {
            Console.WriteLine($@"\n\nOnError: Level:{ex.Level}\n
                                    StackTrace:{ex.StackTrace}\n
                                    Message:{ex.Message}\n\n");
        }
        public void CamerasFocused(object sender, MouseButtonEventArgs e)
        {
            if (this.Running)
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    User user = (from u in MainWindow.Main_window.DBModels.Users where u.Logged == true select u).FirstOrDefault();
                    if (MainWindow.Main_window.Logged && user.Licences.Equals("Admin"))
                    {
                        if (this.Camera_oppened == false)
                        {
                            this.Camera_oppened = true;
                            MyCamera cam = (from c in MainWindow.Main_window.DBModels.MyCameras where c.Id == this.cameraId select c).FirstOrDefault();
                            this.Win_controll = new WindowControll(cam);
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
                        MainWindow.Main_window.cameras_grid.Children.Remove(this.video);
                        this.Fullscreen = true;
                        VideoFullscreen fullscreen = new VideoFullscreen(this);
                        fullscreen.Show();
                    }
                }
            }
        }
        DateTime last_email_date_onmove = DateTime.Now.AddMinutes(-1);
        public void OnMotion(object sender, MotionDetectionEventArgs e)
        {
            MyCamera cam = (from c in MainWindow.Main_window.DBModels.MyCameras where c.Id == this.cameraId select c).FirstOrDefault();
            if (e.Level > cam.Move_Sensitivity)
            {
                if (cam.On_Move_EMAIL)
                {
                    // When Send Get the DateTime
                    if (DateTime.Now > last_email_date_onmove.AddMinutes(1))
                    {
                        last_email_date_onmove = DateTime.Now;
                        // Return to Sending Email
                        String host = "";
                        int port = 587;
                        EmailSender emailSender = (from es in MainWindow.Main_window.DBModels.EmailSenders select es).FirstOrDefault();
                        String subject = cam.name;

                        if (emailSender.Email.Contains("gmail"))
                        {
                            host = "smtp.gmail.com";
                        }
                        else if (emailSender.Email.Contains("yahoo"))
                        {
                            host = "imap.mail.yahoo.com";
                        }
                        else if (emailSender.Email.Contains("live"))
                        {
                            host = "smtp.live.com";
                        }
                        // Create a File with a pic
                        String img_name = "email_pic.jpeg";
                        this.video.Frame_Save(img_name, VisioForge.Types.VFImageFormat.JPEG, 100, 300, 300);
                        // Add All Recievers
                        List<User> users = (from u in MainWindow.Main_window.DBModels.Users select u).ToList();
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
                                                                       "<h1>" + $"[{cam.name}]" + "</h1>" +
                                                                       "<h3>" + "Detect Motion at:" + "</h3>" +
                                                                       "<h2>" + $"[{DateTime.Now}]" + "</h2>" +
                                                                       @"<img src=""cid:{0}"">" +
                                                                   "</body>" +
                                                               "</head>"
                                                                , image.ContentId
                                                            );
                            // Create Email Message
                            var mailMessage = new MimeMessage();
                            mailMessage.From.Add(new MailboxAddress("Officee", emailSender.Email));
                            mailMessage.To.Add(new MailboxAddress(u.FirstName + " " + u.LastName, u.Email));
                            mailMessage.Subject = subject;
                            mailMessage.Body = builder.ToMessageBody();
                            // Send Email Message
                            using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
                            {
                                smtpClient.Connect(host, port, false);
                                smtpClient.Authenticate(emailSender.Email, emailSender.Pass);
                                smtpClient.Send(mailMessage);
                                smtpClient.Disconnect(true);
                            }
                        }
                        // Delete The Image
                        File.Delete(img_name);
                    }
                }
                if (cam.On_Move_Pic)
                {
                    Console.WriteLine("Take Picture.");
                    this.Take_pic();
                }
                if (cam.On_Move_Rec)
                {

                }
                if (cam.On_Move_SMS)
                {
                    // Send SMS
                    if (DateTime.Now > last_email_date_onmove.AddMinutes(1))
                    {
                        last_email_date_onmove = DateTime.Now;
                        // Find your Account Sid and Token at https://account.apifonica.com/
                        SM sms = (from s in MainWindow.Main_window.DBModels.SMS select s).FirstOrDefault();
                        TwilioClient.Init(sms.AccountSID, sms.AccountTOKEN);
                        List<User> users = (from u in MainWindow.Main_window.DBModels.Users select u).ToList();
                        foreach (User u in users)
                        {
                            var message = MessageResource.Create(
                                body: $"[{cam.name}]  Detect Motion at  [{DateTime.Now}]",
                                from: new Twilio.Types.PhoneNumber(sms.Phone),
                                to: new Twilio.Types.PhoneNumber(u.Phone)
                            );
                            Console.WriteLine($"Send SMS To: {message.Sid}.");
                        }
                    }
                }
            }
        }
    }
}
