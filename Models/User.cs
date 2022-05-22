using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Models
{
    public class User : TableEntity
    {
        public User()
        {

        }
        public User(string UserId, string userName, string password, string firstName, string lastName, string email):base(UserId, userName)
        {
            Id = UserId;
            FirstName = firstName;
            LastName = lastName;
            username = userName;
            Password = password;
            Email = email;
        }
        public User(User us):base(us.Id, us.username)
        {
            Id=us.Id;
            FirstName=us.FirstName;
            LastName=us.LastName;
            username = us.username;
            Email = us.Email;
            Password = us.Password;
        }

        public string Id { get; set; }
        public string username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
