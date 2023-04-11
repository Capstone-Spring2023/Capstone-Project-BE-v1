using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using AutoScheduling.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduling.Reader
{
    public class RegisterSubjectReader
    {
        private readonly string fileName = @"\tmp\register_subject.csv";
            // @"\tmp\register_subject_1.csv";
        public List<(int, string, List<string>, bool, bool, bool, bool, bool, bool)> readRegisterSubjectFile()
        {
            using (var reader = new StreamReader(fileName))
            {
                var list = new List<(int,string,List<string>,bool,bool,bool,bool,bool,bool)>();
                for (int i = 0; i< 4; i++ ) reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('\"');
                    if (parts.Length < 3) continue;
                    //Lấy lecturer 
                    string[] firstpart = parts[0].Split(',');
                    int lecturerId = int.Parse(firstpart[1]);
                    string lecturerName = firstpart[2];
                    //Lấy Subject
                    string[] secondPart = parts[1].Split(',');
                    var subjects = new List<String>();
                    foreach (var subject in secondPart)
                    {
                        var a = subject.Replace('"', ' ');
                        a = a.Trim();
                        if (!string.IsNullOrEmpty(a)) subjects.Add(a);
                    }
                    //Lấy lịch expect
                    string[] thirdPart = parts[2].Split(",");
                    bool A1, A2, A3, A4, A5, A6;


                    //A1
                    var check = thirdPart[1];
                    if (check.Equals("x", StringComparison.OrdinalIgnoreCase)) A1 = true;
                    else A1 = false;    
                    //A2
                    check = thirdPart[2];
                    if (check.Equals("x", StringComparison.OrdinalIgnoreCase)) A2 = true;
                    else A2 = false;
                    //A3
                    check = thirdPart[3];
                    if (check.Equals("x", StringComparison.OrdinalIgnoreCase)) A3 = true;
                    else A3 = false;

                    //A4
                    check = thirdPart[4];
                    if (check.Equals("x", StringComparison.OrdinalIgnoreCase)) A4 = true;
                    else A4 = false;

                    //A5
                    check = thirdPart[5];
                    if (check.Equals("x", StringComparison.OrdinalIgnoreCase)) A5 = true;
                    else A5 = false;

                    //A6
                    check = thirdPart[6];
                    if (check.Equals("x", StringComparison.OrdinalIgnoreCase)) A6 = true;
                    else A6 = false;

                    list.Add((lecturerId, lecturerName, subjects, A1, A2, A3, A4, A5, A6));

                }
                return list;
            }
        }

        //UserDic: item1 is UserIndex, item2 is userId in database
        public void createRegisterSubjectFromFile(List<(int,int,string)> userDic , List<(int,string)> subjectDic,
            List<(int, string, List<string>, bool, bool, bool, bool, bool, bool)> list,out  int[,] registerSubject)
        {

            registerSubject = new int[userDic.Count, subjectDic.Count];
            for (int i = 0; i< list.Count; i++)
            {
                var a = list[i];
                int userIndex = userDic.FirstOrDefault(x => x.Item2 == a.Item1).Item1;
                foreach (var s in a.Item3)
                {
                    var subjectIndex = subjectDic.First(x=> x.Item2.ToLower().Equals(s.ToLower().Trim())).Item1;
                    registerSubject[userIndex, subjectIndex] = 1;
                }
            }
        }
        public void createTeacher_Day_Slot(List<(int, int,string)> userDic,List<(int, string, List<string>, bool, bool, bool, bool, bool, bool)> list, int[,] registerSubject,
             out int[,,] teacher_day_slot)
        {
            teacher_day_slot = new int[userDic.Count, 6, 4];
            for (int i = 0; i < list.Count; i++)
            {
                var a = list[i];
                int userIndex = userDic.FirstOrDefault(x => x.Item2 == a.Item1).Item1;
                if (a.Item4)
                {
                    teacher_day_slot[userIndex, 0, 0] = 1;
                    teacher_day_slot[userIndex, 0, 1] = 1;
                    teacher_day_slot[userIndex, 3, 0] = 1;
                    teacher_day_slot[userIndex, 3, 1] = 1;
                }
                if (a.Item5)
                {
                    teacher_day_slot[userIndex, 0, 2] = 1;
                    teacher_day_slot[userIndex, 0, 3] = 1;
                    teacher_day_slot[userIndex, 3, 2] = 1;
                    teacher_day_slot[userIndex, 3, 3] = 1;
                }
                if (a.Item6)
                {
                    teacher_day_slot[userIndex, 1, 0] = 1;
                    teacher_day_slot[userIndex, 1, 1] = 1;
                    teacher_day_slot[userIndex, 4, 0] = 1;
                    teacher_day_slot[userIndex, 4, 1] = 1;
                }
                if (a.Item7)
                {
                    teacher_day_slot[userIndex, 1, 2] = 1;
                    teacher_day_slot[userIndex, 1, 3] = 1;
                    teacher_day_slot[userIndex, 4, 2] = 1;
                    teacher_day_slot[userIndex, 4, 3] = 1;
                }
                if (a.Item8)
                {
                    teacher_day_slot[userIndex, 2, 0] = 1;
                    teacher_day_slot[userIndex, 2, 1] = 1;
                    teacher_day_slot[userIndex, 5, 0] = 1;
                    teacher_day_slot[userIndex, 5, 1] = 1;
                }
                if (a.Item9)
                {
                    teacher_day_slot[userIndex, 2, 2] = 1;
                    teacher_day_slot[userIndex, 2, 3] = 1;
                    teacher_day_slot[userIndex, 5, 2] = 1;
                    teacher_day_slot[userIndex, 5, 3] = 1;
                }
            }
        }
        
        public void createRegisterSubjectFileFromDatabase()
        {
            string filePath = fileName;
            var getter = new RegisterSubjectGetter();
            var register_subject_slot = getter.readRegisterSubject();
            var csv = new StringBuilder();
            csv.AppendLine(",,Please use a comma to separate 2 subjects");
            csv.AppendLine(",,See courses list sheet to choose combo with (A & P) course to register");
            csv.AppendLine("No.,LecturerId,Lecturer,Subjects,\"Morning 2,5\",\"Afternoon 2,5\",\"Morning 3,6\",\"Afternoon 3,6\",\"Morning 4,7\",\"Afternoon 4,7\"");
            List<(string, string)> list = new List<(string, string)>()
            {
                ("A1","A2"),("P1","P2"),
                ("A3","A4"),("P3","P4"),
                ("A5","A6"),("P5","P6")
            };
            int count = 1;
            foreach (var a in register_subject_slot)
            {
                StringBuilder registerSubjects = new StringBuilder();
                registerSubjects.Append($"{count},{a.User.UserId},{a.User.FullName}");
                registerSubjects.Append($",\"");
                var check2 = false;
                foreach (var b in a.RegisterSubjects)
                {
                    check2 = true;
                    registerSubjects.Append($"{b.AvailableSubject.SubjectName},");
                }
                if (check2) registerSubjects.Remove(registerSubjects.Length - 1, 1);
                registerSubjects.Append($"\"");
                //var registerSlot
                
                for (int i = 0; i< list.Count; i++)
                {
                    var b = list[i];
                    bool check = (a.RegisterSlots.Exists(x => x.Slot.Trim() == b.Item1 || x.Slot.Trim() == b.Item2));
                    if (check)
                    {
                        registerSubjects.Append(",x");
                    }
                    else
                    {
                        registerSubjects.Append(",");
                    }
                }
                csv.AppendLine(registerSubjects.ToString());
            }
            File.WriteAllText(filePath, csv.ToString());
            
            
        }
    }
}
