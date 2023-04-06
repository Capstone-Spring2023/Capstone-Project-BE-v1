using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.RegisterSubjectService.Models
{
    public class CreateRegisterSubjectModel
    {
        public int UserId { get; set; }
        public int AvailableSubjectId { get; set; }
        public string Slot { get; set; }

    }
}
