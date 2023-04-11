using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.Sat;

namespace AutoScheduling
{
    public class LecturerRegisterSlotGenerator
    {
        public static int[,,] generate(int num_lecturers,int num_subjects, int num_classes, int num_days, int num_slots, int[] d, int[,]b, int[,] registerSubject, int[,,] s)
        {
            CpModel model = new CpModel();
            IntVar[,,] c = new IntVar[num_lecturers, num_days, num_slots];
            //Tạo list slot tương ứng với từng class theo mảng s (để giảm độ phức tạp)
            var class_day_slot = new Dictionary<int, List<(int, int)>>();
            for (int i = 0; i < num_classes; i++)
            {
                var list = new List<(int, int)>();  
                for (int j = 0; j < num_days; j++)
                    for (int k = 0; k < num_slots; k++)
                    {
                        if (s[i, j, k] == 1) list.Add((j, k));
                    }
                class_day_slot.Add(i, list);
            }

           // ghép teacher với từng class tương ứng
            int[,] teacher_class = new int [num_lecturers, num_classes];
            for (int i =0; i< num_lecturers; i++)
                for (int j =0; j< num_subjects; j++)
                {
                    if (registerSubject[i,j] == 1)
                    {
                        for (int k = 0; k < num_classes; k++)
                        {
                            if (b[j,k] == 1) teacher_class[i, k] = 1;
                        }
                    }
                }

            //tạo constraint cho lecturer để đký chỉ những slot mà mình có đký dạy môn tương ứng
            int[,,] teacher_day_slot = new int[num_lecturers, num_days, num_slots];
            for (int i = 0; i< num_lecturers; i++)
                for (int j = 0; j< num_classes; j++)
                {
                    foreach (var a in class_day_slot[j])
                    {
                        teacher_day_slot[i, a.Item1, a.Item2] = 1;
                    }
                }

            for (int i = 0; i < num_lecturers; i++)
                for( int j = 0; j < num_days; j++)
                    for ( int k = 0; k < num_slots; k++)
                    {
                        if (teacher_day_slot[i,j,k] == 1)
                        {
                            c[i, j, k] = model.NewIntVar(0, 1, "");
                        }
                        else
                        {
                            c[i, j, k] = model.NewIntVar(0, 0, "");
                        }
                    }
            
            //Mỗi teacher phải đăng ký dạy sao cho số slot đăng ký >= số slot lồn đó phải dạy
            for (int i = 0; i < num_lecturers; i++)
            {
                int count = 0;
                IntVar[] num_teaching_slots = new IntVar[num_days * num_slots];
                for (int j = 0; j < num_days; j++)
                    for (int k = 0; k < num_slots; k++)
                    {
                        num_teaching_slots[count] = c[i, j, k];
                        count++;
                    }
                LinearExpr expr = LinearExpr.Sum(num_teaching_slots);
                model.Add(expr >= (d[i] * 1));
                model.Add(expr <= (d[i] * 4));
            }
            
            //nếu đã đăng ký slot a ngày b thì phải đký slot a ngày b+3
            for (int i = 0; i < num_lecturers; i++)
            {
                for (int j = 0; j < num_days / 2; j++)
                {
                    
                    for (int k = 0; k < num_slots; k++)
                    {
                        model.Add(c[i, j, k] == c[i, j + 3, k]);
                    }
                }
            }
            // day_slot lồn nào cũng phải có thằng lz nào đó đăng ký
            for (int j = 0; j < num_days / 2; j++)
                for (int k = 0; k < num_slots; k++)
                {
                    IntVar[] count = new IntVar[num_lecturers];
                    for (int i = 0; i<  num_lecturers; i++) count[i] = c[i, j, k];
                    model.Add(LinearExpr.Sum(count) >= 1);
                }
            // cân bằng?

            CpSolver solver = new CpSolver();
            solver.Solve(model);
            int[,,] c1 = new int[num_lecturers, num_days, num_slots];
            for (int i = 0; i < num_lecturers; i++)
            {
                for (int j = 0; j < num_days; j++)
                {
                    for (int k = 0; k < num_slots; k++)
                    {
                        if (solver.Value(c[i,j,k])== 1)
                        {
                            c1[i, j, k] = 1;
                            //Console.WriteLine($"lecturer_{i}__day_{j}__slot_{k}");
                        }
                    }
                }
            }

            var csv = new StringBuilder();
            csv.Append("Day_Slot");
            for (int i = 0; i < num_lecturers; i++) csv.Append($",lecturer_{i}");
            csv.AppendLine();
            //csv

            for (int j = 0; j < num_days; j++)
            {
                for (int k = 0; k < num_slots; k++)
                {
                    var line = new StringBuilder();
                    line.Append($"Day_{j}_Slot{k}");
                    for (int i = 0; i < num_lecturers; i++)
                    {
                        if (c1[i,j,k] == 1)
                        {
                            line.Append($",X");
                        }
                        else
                        {
                            line.Append(",");
                        }
                    }
                    csv.AppendLine(line.ToString());
                }
            }
            string filePath = @"D:\Schedule\teacher_day_slot\teacher_day_slot.csv";
            File.WriteAllText(filePath, csv.ToString());
            return c1;
        }
    }
}
