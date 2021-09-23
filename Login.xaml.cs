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
            MainWindow.Main_window.Login_oppened = false;
            Console.WriteLine("login_oppened: " + Convert.ToString(MainWindow.Main_window.Login_oppened));
            this.Close();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            String email = Email.Text;
            String password = Password.Password;
            if ( email.Length > 0 && password.Length > 0)
            {
                User user = (from u in MainWindow.Main_window.DBModels.Users where u.Email.Equals(email) && u.Password.Equals(password) select u).FirstOrDefault();
                if (user != null)
                {
                    if (MainWindow.Main_window.DBModels.Users.Where(u => u.Logged).Any())
                    {
                        User oldLoggedUser = (from u in MainWindow.Main_window.DBModels.Users where u.Logged == true select u).FirstOrDefault();
                        oldLoggedUser.Logged = false;
                        MainWindow.Main_window.DBModels.SaveChanges();
                    }
                    user.Logged = true;
                    MainWindow.Main_window.DBModels.SaveChanges();
                    MainWindow.Main_window.Logged = true;
                    MainWindow.Main_window.login_logout_b.Content = "Logout";
                    MainWindow.Main_window.login_logout_b.Click += (object send, RoutedEventArgs ev) =>
                    {
                        MainWindow.Main_window.Loggout_clicked();
                    };
                    this.Close();
                }
                else
                {
                    MainWindow.Main_window.Logged = false;
                    MessageBox.Show("Sign Up!");
                }
            }
            else
            {
                MainWindow.Main_window.Logged = false;
                MessageBox.Show("Check your Email or Password!");
            }
            
        }
    }
}
