using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.UserService.Models
{
    public class UserModel
    {
        public UserModel(string fullName, string phone, string address, int roleId, string roleName)
        {
            this.fullName = fullName;
            this.phone = phone;
            this.address = address;
            this.roldId= roleId;
            this.roleName = roleName;
        }

        public UserModel()
        {
        }

        public string fullName { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public int roldId { get; set; }
        public string roleName { get; set; }
    }
}
