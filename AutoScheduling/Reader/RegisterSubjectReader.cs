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
        private readonly string fileName = Constant.REGISTER_SUBJECT_FILE;
            // @"\tmp\register_subject_1.csv";
        public List<(int, string, List<string>, bool, bool, bool, bool, bool, bool,bool,int)> readRegisterSubjectFile(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var list = new List<(int, string, List<string>, bool, bool, bool, bool, bool, bool, bool, int)>();
                for (int i = 0; i< 3; i++ ) reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('\"');
                    bool A1, A2, A3, A4, A5, A6, isColab;
                    if (parts.Length < 3)
                    {
                        string[] parts1 = line.Split(',');
                        int lecturerId1 = int.Parse(parts1[1]);
                        string lecturerName1 = parts1[2];
                        var subjects1 = new List<String>();
                        subjects1.Add(parts1[3].Trim());
                        var check1 = parts1[4];
                        if (check1.Equals("x", StringComparison.OrdinalIgnoreCase)) A1 = true;
                        else A1 = false;
                        //A2
                        check1 = parts1[5];
                        if (check1.Equals("x", StringComparison.OrdinalIgnoreCase)) A2 = true;
                        else A2 = false;
                        //A3
                        check1 = parts1[6];
                        if (check1.Equals("x", StringComparison.OrdinalIgnoreCase)) A3 = true;
                        else A3 = false;

                        //A4
                        check1 = parts1[7];
                        if (check1.Equals("x", StringComparison.OrdinalIgnoreCase)) A4 = true;
                        else A4 = false;

                        //A5
                        check1 = parts1[8];
                        if (check1.Equals("x", StringComparison.OrdinalIgnoreCase)) A5 = true;
                        else A5 = false;

                        //A6
                        check1 = parts1[9];
                        if (check1.Equals("x", StringComparison.OrdinalIgnoreCase)) A6 = true;
                        else A6 = false;

                        check1 = parts1[10];
                        if (check1.Equals("x", StringComparison.OrdinalIgnoreCase)) isColab = true;
                        else isColab = false;

                        int di1 = int.Parse(parts1[11]);
                        list.Add((lecturerId1, lecturerName1, subjects1, A1, A2, A3, A4, A5, A6, isColab, di1));
                        continue;
                    }
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

                    check = thirdPart[7];
                    if (check.Equals("x", StringComparison.OrdinalIgnoreCase)) isColab = true;
                    else isColab = false;

                    int di = int.Parse(thirdPart[8]);
                    list.Add((lecturerId, lecturerName, subjects, A1, A2, A3, A4, A5, A6,isColab,di));

                }
                return list;
            }
        }

        //UserDic: item1 is UserIndex, item2 is userId in database
        public void createRegisterSubjectFromFile(List<(int,int,string)> userDic , List<(int,string)> subjectDic,
            List<(int, string, List<string>, bool, bool, bool, bool, bool, bool,bool,int)> list,out  int[,] registerSubject)
        {

            registerSubject = new int[userDic.Count, subjectDic.Count];
            for (int i = 0; i< list.Count; i++)
            {
                var a = list[i];
                    int userIndex = userDic.FirstOrDefault(x => x.Item2 == a.Item1).Item1;
                foreach (var s in a.Item3)
                {
                    if (String.IsNullOrEmpty(s)) continue;
                    var subjectIndex = subjectDic.First(x=> x.Item2.ToLower().Equals(s.ToLower().Trim())).Item1;
                    registerSubject[userIndex, subjectIndex] = 1;
                }
            }
        }
        public void createAbleSubject(List<(int, int, string)> userDic, List<(int, string)> subjectDic, Dictionary<int,List<string>> userAblesubject
            , out int[,] ableSubject)
        {
            ableSubject = new int[userDic.Count, subjectDic.Count];
            foreach(var a in userAblesubject)
            {
                int userIndex = userDic.First(x => x.Item2 == a.Key).Item1;
                foreach(var subjectName in a.Value)
                {
                    int subjectIndex = subjectDic.First(x => x.Item2 == subjectName).Item1;
                    ableSubject[userIndex, subjectIndex] = 1;
                }
            }
        }
        public void createTeacher_Day_Slot(List<(int, int,string)> userDic,List<(int, string, List<string>, bool, bool, bool, bool, bool, bool, bool,int)> list, int[,] registerSubject,
             out int[,,] teacher_day_slot)
        {
            teacher_day_slot = new int[userDic.Count, 3, 4];
            for (int i = 0; i < list.Count; i++)
            {
                var a = list[i];
                int userIndex = userDic.FirstOrDefault(x => x.Item2 == a.Item1).Item1;
                if (a.Item4)
                {
                    teacher_day_slot[userIndex, 0, 0] = 1;
                    teacher_day_slot[userIndex, 0, 1] = 1;
                  
                }
                if (a.Item5)
                {
                    teacher_day_slot[userIndex, 0, 2] = 1;
                    teacher_day_slot[userIndex, 0, 3] = 1;
                
                }
                if (a.Item6)
                {
                    teacher_day_slot[userIndex, 1, 0] = 1;
                    teacher_day_slot[userIndex, 1, 1] = 1;
           
                }
                if (a.Item7)
                {
                    teacher_day_slot[userIndex, 1, 2] = 1;
                    teacher_day_slot[userIndex, 1, 3] = 1;
                    
                }
                if (a.Item8)
                {
                    teacher_day_slot[userIndex, 2, 0] = 1;
                    teacher_day_slot[userIndex, 2, 1] = 1;
                    
                }
                if (a.Item9)
                {
                    teacher_day_slot[userIndex, 2, 2] = 1;
                    teacher_day_slot[userIndex, 2, 3] = 1;
                }
            }
        }
        
        public void create_teacher_iscollab(List<(int, int, string)> userDic, List<(int, string, List<string>, bool, bool, bool, bool, bool, bool, bool,int)> list,
            out bool[] userIndex_isColab)
        {
            userIndex_isColab = new bool[userDic.Count];
            foreach(var a in list)
            {
                int userIndex = userDic.First(x => x.Item2 == a.Item1).Item1;
                userIndex_isColab[userIndex] = a.Item10;
            }
        }
        public void createRegisterSubjectFileFromDatabase(int semesterId)
        {
            string filePath = fileName;
            var getter = new RegisterSubjectGetter();
            var register_subject_slot = getter.readRegisterSubject(semesterId);
            var csv = new StringBuilder();
            csv.AppendLine(",,Please use a comma to separate 2 subjects");
            csv.AppendLine(",,See courses list sheet to choose combo with (A & P) course to register");
            csv.AppendLine("No.,LecturerId,Lecturer,Subjects,\"Morning 2,5\",\"Afternoon 2,5\",\"Morning 3,6\",\"Afternoon 3,6\",\"Morning 4,7\",\"Afternoon 4,7\",Is Collaborator, Min Num Class");
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
                if (a.User.IsColab == true)
                {
                    registerSubjects.Append(",x");
                }
                else
                {
                    registerSubjects.Append(",");
                }
                registerSubjects.Append($",{a.User.NumMinClass}");
                csv.AppendLine(registerSubjects.ToString());
            }
            File.WriteAllText(filePath, csv.ToString());
            
            
        }
    }
}
