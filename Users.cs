using System;

namespace IPCamera
{
    public class Users
    {
        public static int Count;

        public int Id { get; set; }

        public String Firstname { get; set; }

        public String Lastname { get; set; }

        public String Email { get; set; }

        public String Phone { get; set; }

        public String Password { get; set; }

        public String Licences { get; set; }

        // Constructor
        public Users(int id, String fname, String lname, String email, String phone, String licences, String password)
        {
            this.Id = id;
            this.Firstname = fname;
            this.Lastname = lname;
            this.Email = email;
            this.Phone = phone;
            this.Licences = licences;
            this.Password = password;
            Count++;
        }

        // Destructor
        ~Users()
        {
            Count--;
        }
    }
}
