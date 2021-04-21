using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IPCamera
{
    class Install_Requarements
    {
        public static String GetRootDir()
        {
            String cur_dir = Environment.CurrentDirectory;
            //string root_dir = Path.GetFullPath(Path.Combine(cur_dir, @"..\..\"));
            return cur_dir;
        }

        public static void Install_Req()
        {
            String req_dir = $"{GetRootDir()}\\Requarements\\";
            /*
            String[] exes =
                    Directory.GetFiles(req_dir, "*.EXE", SearchOption.AllDirectories)
                    .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
                    .AsEnumerable()
                    .ToArray();
            */
            String[] exes = {
                "NDP472-KB4054531-Web" , "NetFx64",
                "redist_dotnet_base_x64" , "redist_dotnet_base_x86",
                "redist_dotnet_ffmpeg_exe_x64" , "redist_dotnet_ffmpeg_exe_x86",
                "redist_dotnet_ffmpeg_x86" , "redist_dotnet_lav_x64",
                "redist_dotnet_lav_x86" , "redist_dotnet_mp4_x64",
                "redist_dotnet_mp4_x86" , "redist_dotnet_vlc_x64",
                "redist_dotnet_vlc_x86" , "redist_dotnet_webm_x86",
                "redist_dotnet_webm_x86" , "redist_dotnet_xiph_x86",
                "SQLServer2016-SSEI-Expr"
            };
            foreach (String file in exes)
            {
                String exe = $"{req_dir}{file}.exe";
                Console.WriteLine(exe);
                try
                {
                    System.Diagnostics.Process.Start(exe);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                }
                Thread.Sleep(2000);
            }

        }
    }
}
