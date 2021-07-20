using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using System.Windows;

namespace IPCamera
{

    public partial class Account : Window
    {

        public Users User { get; set; }

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
            MainWindow.Account_oppened = false;
            Console.WriteLine("account_oppened: " + Convert.ToString(MainWindow.Account_oppened));
            this.Close();
        }

        // When Apply Button Clicks Update the current user
        public void Apply_Click(object sender, RoutedEventArgs e)
        {
            // Save to DataBase
            using (SqlConnection connection = new SqlConnection(App.DB_connection_string))
            {
                String query = $"UPDATE Users SET FirstName='{this.User.Firstname}', LastName='{this.User.Lastname}', " +
                                $"Email='{this.User.Email}', Phone='{this.User.Phone}', Password='{this.User.Password}' " +
                                $"WHERE Email='{this.User.Email}'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteNonQuery();
                    // Check Error
                    if (result < 0)
                        System.Windows.MessageBox.Show("Error inserting data into Database!");
                    connection.Close();
                }  
            }
            // Save in RAM
            MainWindow.User = this.User;
            int counter = 0;
            Boolean checker = false;
            foreach(Users user in MainWindow.MyUsers)
            {
                if (user.Email.Equals(this.User.Email))
                {
                    MainWindow.MyUsers[counter] = this.User;
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
