using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduling
{
    public class CsvWriter
    {
        public static void writeScheduleFile(int num_slots,int num_days,int num_lecturers, int num_classes
            ,int num_subjects,int[,] subject_class,List<(int,int,string)> subject_class_className, List<(int,int,string)> userDic,
                CpSolver solver,IntVar[,,,] f )
        {
            var csvSchedule = new StringBuilder();
            csvSchedule.AppendLine("Slot,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday");
           
            for (int l = 0; l < num_slots; l++)
            {
                Dictionary<int, List<string>> schedule_for_1_slot = new Dictionary<int, List<string>>();
                for (int k = 0; k< num_days; k++)
                {
                    schedule_for_1_slot.Add(k, new List<string>());
                }
                for (int i = 0; i < num_lecturers; i++)
                    for (int j = 0; j < num_classes; j++)
                        for (int k = 0; k < num_days; k++)
                            if (solver.Value(f[i, j, k, l]) == 1)
                            {
                                int subjectId = 0;
                                string className = subject_class_className.First(x => x.Item2 == j).Item3;
                                string lecturerName = userDic.First(x => x.Item1 == i).Item3;
                                var list = schedule_for_1_slot[k];
                                list.Add($"lecturer_{lecturerName}_class{className}");
                            }
                int[] check_if_still_exist = new int[num_days];
                check_if_still_exist =  check_if_still_exist.Select(i => 1).ToArray();
                int index = -1;
                while (true)
                {
                    index++;
                    //kiểm tra xem slot này còn lịch nữa hay k
                    for (int k = 0; k < 6; k++)
                        if (check_if_still_exist[k] == 1 && schedule_for_1_slot[k].Count() <= index)
                        {
                            check_if_still_exist [k] = 0;
                        }
                    if (check_if_still_exist.Sum(i => i) == 0) break;
                    //tạo line và append vào nó
                    var line = new StringBuilder();
                    line.Append($"Slot_{l}");
                    for (int k = 0; k < 6; k++)
                        if (check_if_still_exist[k] == 1 && schedule_for_1_slot[k].Count() > index)
                        {
                            var list = schedule_for_1_slot[k];
                            line.Append("," + list[index]);
                        }
                        else
                        {
                            check_if_still_exist[k] = 0;
                            line.Append(",X");
                        }
                    //append cái line vào csv chính
                    csvSchedule.AppendLine(line.ToString());
                }
            }


            //string filePath = "/"
            string filePath = @"D:\Schedule\schedule.csv";
            File.WriteAllText(filePath, csvSchedule.ToString());
        }

        public static void writeScheduleFileV2(int num_slots, int num_days, int num_lecturers, int num_classes
            , int num_subjects, int[,] subject_class, List<(int, int, string)> subject_class_className, List<(int, int, string)> userDic,
                CpSolver solver, IntVar[,,,] f)
        {
            var csvSchedule = new StringBuilder();
            csvSchedule.AppendLine("LecturerId,Lecturer,Subject,Class,Slot");
            for (int i = 0; i < num_lecturers; i++)
            {
                string lecturerName = userDic.First(x=> x.Item1 == i).Item3;
                for (int j = 0; j < num_classes; j++)
                    for (int k = 0; k < num_days / 2 ; k++)
                        for (int l = 0; l < num_slots; l++)
                        {
                            if (solver.Value(f[i,j,k,l]) == 1)
                            {
                                string className = subject_class_className.First(x => x.Item2 == j).Item3;
                                string subjectName = className.Split('_')[0];
                                var lecturerId = userDic.First(x => x.Item1 == i).Item2;
                                csvSchedule.AppendLine($"{lecturerId},{lecturerName},{subjectName},{className},{ScheduleGenerator.day_slot_to_APx(k, l)}");
                            }
                        }
            }
            string filePath = @"D:\Schedule\schedule.csv";
            File.WriteAllText(filePath, csvSchedule.ToString());
        }

        public void writeScheduleForEachTeacher(int num_slots, int num_days, int num_lecturers, int num_classes
            , int num_subjects, int[,] subject_class, CpSolver solver, IntVar[,,,] f)
        {
           
            
            for (int i = 0; i < num_lecturers; i++)
            {
                var csv = new StringBuilder();
                csv.AppendLine("Slot,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday");
                Dictionary<int, List<string>> schedule_for_1_slot = new Dictionary<int, List<string>>();
                for (int k = 0; k < num_days; k++)
                {
                    schedule_for_1_slot.Add(k, new List<string>());
                }

                for (int l = 0; l < num_slots; l++)
                    for (int k = 0; k < num_days; k++)
                    {
                        int subjectId = 0;
                        for (int j = 0; j < num_classes; j++)
                            if (solver.Value(f[i, j, k, l]) == 1)
                            {
                                
                                for (int subject = 0; subject < num_subjects; subject++)
                                {
                                    if (subject_class[subject, j] == 1)
                                    {
                                        subjectId = subject;
                                        break;
                                    }
                                }
                                break;
                            }
                    }
                
                
            }
        }
    }
}
