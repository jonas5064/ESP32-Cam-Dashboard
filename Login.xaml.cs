﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IPCamera
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            MainWindow.login_oppened = false;
            Console.WriteLine("login_oppened: " + Convert.ToString(MainWindow.login_oppened));
            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            String email = Email.Text;
            String password = Password.Text;
            try
            {
                var v = from u in MainWindow.myUsers where (u.Email.Equals(email)) && (u.Password.Equals(password)) select u;
                MainWindow.user = v.First();
                MainWindow.loged = true;
                MainWindow.main_window.login_logout_b.Content = "Logout";
                MainWindow.main_window.login_logout_b.Click += (object send, RoutedEventArgs ev) =>
                {
                    MainWindow.main_window.Loggout_clicked();
                };
                this.Close();
            } catch (Exception)
            {
                MainWindow.loged = false;
                MessageBox.Show("User Not Finded;");
            }
            
        }
    }
}
