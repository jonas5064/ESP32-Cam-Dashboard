using System;
using System.Linq;
using System.Windows;

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
            MainWindow.Login_oppened = false;
            Console.WriteLine("login_oppened: " + Convert.ToString(MainWindow.Login_oppened));
            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            String email = Email.Text;
            String password = Password.Password;
            try
            {
                var v = from user in MainWindow.MyUsers where user.Email.Equals(email) && user.Password.Equals(password) select user;
                MainWindow.User = v.Single();
                MainWindow.Logged = true;
                MainWindow.Main_window.login_logout_b.Content = "Logout";
                MainWindow.Main_window.login_logout_b.Click += (object send, RoutedEventArgs ev) =>
                {
                    MainWindow.Main_window.Loggout_clicked();
                };
                this.Close();
            }
            catch (Exception ex)
            {
                MainWindow.Logged = false;
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
                if(ex.Message.Contains("Sequence contains no elements"))
                {
                    MessageBox.Show("Check your Email or Password!");
                }
            }
            
        }
    }
}
