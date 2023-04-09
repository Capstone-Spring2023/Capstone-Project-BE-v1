using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AuthenticationService.Models
{
    public class ResponseLogin
    {
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string FullName { get; set; }
        public int UserId { get; set; }
    }
}
