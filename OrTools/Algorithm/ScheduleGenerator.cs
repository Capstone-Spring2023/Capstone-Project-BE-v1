using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrTools
{
    public class ScheduleGenerator
    {
        public static int[,,] Create_Schedule(int num_classes,int num_days, int num_slots,List<(int,int)> subjects_class, List<(int,string)> subjectDic)
        {
            // Define the problem variables

            // Create the solver
            CpModel model = new CpModel();

            // Define the decision variables
            IntVar[,,] schedule = new IntVar[num_classes, num_days, num_slots];
            for (int i = 0; i < num_classes; i++)
            {
                for (int j = 0; j < num_days; j++)
                {
                    for (int k = 0; k < num_slots; k++)
                        schedule[i, j, k] = model.NewIntVar(0L, 1L, $"class_{i}_day_{j}_slot_{k}");

                }
            }

            // Each class must have exactly 2 slots scheduled

            for (int i = 0; i < num_classes; i++)
            {
                int count = 0;
                IntVar[] slots = new IntVar[num_days * num_slots];
                for (int j = 0; j < num_days; j++)
                {
                    for (int k = 0; k < num_slots; k++)
                    {
                        slots[count] = schedule[i, j, k];
                        count++;
                    }

                }
                LinearExpr linearExpr = LinearExpr.Sum(slots);
                model.Add(linearExpr == 2);
            }

            for (int i = 0; i < num_classes; i++)
            {
                int count = 0;

                for (int j = 0; j < num_days / 2; j++)
                {
                    for (int k = 0; k < num_slots; k++)
                    {
                        IntVar slots1 = schedule[i, j, k];
                        IntVar slots2 = schedule[i, j + 3, k];


                        model.Add(slots1 == slots2);
                    }
                }

            }


            // Each day must have 8 Slot scheduled
            for (int j = 0; j < num_days; j++)
            {
                for (int k = 0; k < num_slots; k++)
                {
                    IntVar[] slots = new IntVar[num_classes];
                    int count = 0;

                    for (int i = 0; i < num_classes; i++)
                    {
                        slots[count] = schedule[i, j, k];
                        count++;
                    }


                    LinearExpr linearExpr = LinearExpr.Sum(slots);

                    model.Add(linearExpr >= 6);
                    model.Add(linearExpr <= 10);
                }
            }
            // smart schedule

            for (int j = 0; j < num_days; j++)
                for (int k = 0; k < num_slots; k++)
                {
                    IntVar[] num_class_in_1_slot = new IntVar[36];
                    for (int i = 0; i < 36; i++)
                    {
                        num_class_in_1_slot[i] = schedule[i, j, k];
                    }
                    LinearExpr linearExpr = LinearExpr.Sum(num_class_in_1_slot);
                    model.Add(linearExpr <= 5);

                    IntVar[] num_class_in_1_slot_PRN = new IntVar[40];
                    for (int i =0; i< 40; i++)
                    {
                         num_class_in_1_slot_PRN[i] = schedule[i + 36, j, k];
                    }
                    LinearExpr linearExpr1 = LinearExpr.Sum(num_class_in_1_slot_PRN);
                    model.Add(linearExpr1 <= 4);

                    IntVar[] num_class_in_1_slot_MLN = new IntVar[15];
                    for (int i = 0; i < 15; i++)
                    {
                        num_class_in_1_slot_MLN[i] = schedule[i + 76, j, k];
                    }
                    LinearExpr linearExpr2 = LinearExpr.Sum(num_class_in_1_slot_MLN);
                    model.Add(linearExpr2 <= 2);
                }

            //var printer = new SolutionPrinter(num_classes,num_days,num_slots,num_slots_per_shift, schedule, 5);
            //         CpSolver solver = new CpSolver();
            CpSolver solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);
            Console.WriteLine(status);
            int[,,] s = new int[num_classes, num_days, num_slots];
            for (int j = 0; j < num_days/2; j++)
            {
                for (int k = 0; k < num_slots; k++)
                {
                    for (int i = 0; i < num_classes; i++)
                    {
                        if (solver.Value(schedule[i, j, k]) == 1)
                        {
                            s[i, j, k] = 1;
                            int subjectId = subjects_class.FirstOrDefault(x=> x.Item2 == i).Item1;
                            string a = subjectDic.First(x => x.Item1 == subjectId).Item2;
                            Console.WriteLine($"Day_{j}_Slot_{k}_Class: {a}_{i}");
                        }
                    }
                }
            }
            var csv = new StringBuilder();
            csv.AppendLine("Subject Name, Class Name,Slot");
            for (int i = 0; i < num_classes; i++)
            {
                for (int j = 0; j < num_days/2; j++)
                    for (int k = 0; k < num_slots; k++) 
                        if (s[i,j,k] == 1)
                        {
                            int subjectIndex = subjects_class.First(x => x.Item2 == i).Item1;
                            string subjectName = subjectDic.First(x => x.Item1 == subjectIndex).Item2;
                            string class_subject_name = subjectName + "_" + i;
                            csv.AppendLine($"{subjectName},{class_subject_name},{day_slot_to_APx(j,k)}");
                        }
            }
            string filePath = @"D:\Schedule\Input\class_day_slot.csv";
            File.WriteAllText(filePath, csv.ToString());
            return s;
        }
        public static string day_slot_to_APx(int day, int slot)
        {
            
            if ( slot  <= 2)
            {
                int index = day * 2 + slot  ;
                return "A" +index.ToString();
            }
            else 
            {
                int index = day * 2 + slot - 2;
                return "P" + index.ToString();
            }

        }
    }
}
