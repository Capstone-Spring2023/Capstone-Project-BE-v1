using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public enum StatusCode
    {
        OK = 200,
        SUCCESS = 201,
        NOTFOUND = 404,
        BADREQUEST = 500
    }
    public class ExamPaperStatus
    {
        public static string APPROVED = "Approved";
        public static string REJECTED = "Rejected";
        public static string PENDING = "Pending";
        public static string APPROVED_MANUAL = "Waiting-Instruction";
    }
}
