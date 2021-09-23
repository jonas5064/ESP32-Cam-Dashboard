using System;
using System.Data.SqlClient;
using System.Windows;
using System.Linq;

namespace IPCamera
{

    public partial class Account : Window
    {
        public User User { get; set; }
        public Account(User user)
        {
            InitializeComponent();
            this.User = user;
            user_grid.DataContext = this.User;
        }
        // When Close The Window
        protected override void OnClosed(EventArgs e)
        {
            MainWindow.Main_window.Account_oppened = false;
            Console.WriteLine("account_oppened: " + Convert.ToString(MainWindow.Main_window.Account_oppened));
            this.Close();
        }
        // When Apply Button Clicks Update the current user
        public void Apply_Click(object sender, RoutedEventArgs e)
        {
            User user = (from u in MainWindow.Main_window.DBModels.Users where u.Email == this.User.Email select u).FirstOrDefault();
            user = this.User;
            MainWindow.Main_window.DBModels.SaveChanges();
        }
    }
}
