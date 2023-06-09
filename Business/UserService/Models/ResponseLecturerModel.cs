﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.UserService.Models
{
    public class ResponseLecturerModel
    {
        public string fullName { get; set; }
        public string semester { get; set; }
        public int semesterId { get; set; }
        public string subjectName { get; set; }
        public bool isLeader { get; set; }
        public int userId { get; set; }
        public int availableSubjectId { get; set; }
        public string approvalUserName { get; set; }
        public string status { get; set; }
        public string examLink { get; set; }
        public bool? isCol { get; set; }
    }

    public class ResponseTeacher
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public bool Status { get; set; }
    }
}
