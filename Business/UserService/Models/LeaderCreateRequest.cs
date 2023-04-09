using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.UserService.Models
{
    public class LeaderCreateRequest
    {
        public int AvailableSubjectId { get; set; }
        public int UserId { get; set; }
    }
}
