using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.UserService.Models
{
    public class ResponseDepartment
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
    }
}
