using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisioForge.Controls.UI.WPF;

namespace IPCamera
{
    public class Camera
    {
        public String url = "";
        public string name = "";
        public string id = "";
        public bool detection = false;
        public bool recognition = false;
        public VideoCapture video;
        public static int count = 0;

        public Camera(String url, String name, String id)
        {
            this.url = url;
            this.name = name;
            this.id = id;
            count++;
        }

        public Camera(String url, String name, String id, bool detection, bool recognition)
        {
            this.url = url;
            this.name = name;
            this.id = id;
            this.detection = detection;
            this.recognition = recognition;
            count++;
        }

        ~Camera()
        {
            count--;
        }
    }
}
