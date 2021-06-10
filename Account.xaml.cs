using MySql.Data.MySqlClient;
using System;
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

    public partial class Account : Window
    {

        public Users user;
        public Users User
        {
            get { return this.user; }
            set
            {
                this.user = value;
            }
        }


        public Account(Users user)
        {
            InitializeComponent();
            this.User = user;
            Console.WriteLine($"\n\nUserName: {this.User.Firstname}  {this.User.Lastname}");
            user_grid.DataContext = this.User;
        }


        // When Close The Window
        protected override void OnClosed(EventArgs e)
        {
            MainWindow.account_oppened = false;
            Console.WriteLine("account_oppened: " + Convert.ToString(MainWindow.account_oppened));
            this.Close();
        }

        // When Apply Button Clicks Update the current user
        public void Apply_Click(object sender, RoutedEventArgs e)
        {
            // Save to DataBase
            MySqlConnection connection = new MySqlConnection(App.DB_connection_string);
            String query = $"UPDATE Users SET FirstName='{this.User.Firstname}', LastName='{this.User.Lastname}', " +
                            $"Email='{this.User.Email}', Phone='{this.User.Phone}', Password='{this.User.Password}' " +
                            $"WHERE Email='{this.User.Email}'";
            MySqlCommand command = new MySqlCommand(query, connection);
            connection.Open();
            var result = command.ExecuteNonQuery();
            // Check Error
            if (result < 0)
                System.Windows.MessageBox.Show("Error inserting data into Database!");
            connection.Close();
            // Save in RAM
            MainWindow.user = this.User;
            int counter = 0;
            Boolean checker = false;
            foreach(Users user in MainWindow.myUsers)
            {
                if (user.Email.Equals(this.User.Email))
                {
                    MainWindow.myUsers[counter] = this.User;
                    checker = true;
                    break;
                }
                counter++;
            }
            if (checker)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Can't Find this email.","Warning!!!");
            }
        }
    }
}
