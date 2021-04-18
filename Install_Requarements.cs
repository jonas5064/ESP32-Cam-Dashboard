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
            string root_dir = Path.GetFullPath(Path.Combine(cur_dir, @"..\..\"));
            return root_dir;
        }

        public static void Install_Req()
        {
            String req_dir = $"{GetRootDir()}\\Requarements\\";
            String[] exes =
                    Directory.GetFiles(req_dir, "*.EXE", SearchOption.AllDirectories)
                    .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
                    .AsEnumerable()
                    .ToArray();
            foreach(String file in exes)
            {
                String exe = $"{req_dir}{file}.exe";
                Console.WriteLine(exe);
                try
                {
                    System.Diagnostics.Process.Start(exe);
                    Thread.Sleep(2000);
                } catch (Exception ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                }
            }
        }

    }
}
