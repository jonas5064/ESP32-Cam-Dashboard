using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace IPCamera
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        //string db_file_path = $"{Install_Requarements.GetRootDir()}\\Database1.mdf";
        public static String db_url = @"localhost";//"mysql-32698-0.cloudclusters.net";
        public static String db_port = "3306";//"32698";
        public static String db_name = "IPCameras";//"IPCameres_K_Manolis";
        public static String db_user_name = "root";//"admin";
        public static String db_user_password = "Platanios719791";//"12345678";
        public static String DB_connection_string = "";

        //public static String DB_connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\IPCameras\\Database1.mdf;Integrated Security=True";

        public App()
        {
            DB_connection_string = $"server={db_url};port={db_port};uid={db_user_name};pwd={db_user_password};database={db_name};Charset=utf8;Integrated Security=True";
        }
    }
}
