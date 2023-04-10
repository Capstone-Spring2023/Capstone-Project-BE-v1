using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrTools
{
    public class RegisterSubjectGenerator
    {
        public static int[,] Create_Lecturer_Subject(int num_lecturers, int num_subjects)
        {

            IntVar[,] f = new IntVar[num_lecturers, num_subjects];
            CpModel model = new CpModel();
            for (int i = 0; i < num_lecturers; i++)
            {
                for (int j = 0; j < num_subjects; j++)
                {
                    f[i, j] = model.NewIntVar(0L, 1L, $"lecturer_{i}_subject{j}");
                }
            }

            for (int i = 0; i < num_lecturers; i++)
            {
                IntVar[] subjects = new IntVar[num_subjects];
                for (int j = 0; j < num_subjects; j++)
                {
                    subjects[j] = f[i, j];
                }
                LinearExpr linearExpr = LinearExpr.Sum(subjects);
                model.Add(linearExpr >= 3);

            }
            for (int i = 0; i < num_lecturers; i++)
            {
                IntVar[] teachers = new IntVar[num_subjects];
                for(int j=0;j< num_subjects;j++)
                {
                    teachers[j] = f[i, j];
                }
                LinearExpr linearExpr = LinearExpr.Sum(teachers);
                model.Add(linearExpr >= 3);
                model.Add(linearExpr <= 5);
            }
            for (int j = 0; j < num_subjects;j++)
            {
                IntVar[] subjects = new IntVar[num_lecturers];
                for (int i = 0; i < num_lecturers; i++) 
                { 
                    subjects[i] = f[i, j];
                }
                LinearExpr linearExpr = LinearExpr.Sum(subjects);
                model.Add(linearExpr >= 4);
                model.Add(linearExpr <= 6);
            }

            CpSolver solver = new CpSolver();
            solver.Solve(model);
            int[,] a = new int[num_lecturers, num_subjects];
            for (int i = 0; i < num_lecturers; i++)
            {
                for (int j = 0; j < num_subjects; j++)
                {
                    if (solver.Value(f[i, j]) == 1)
                    {
                        a[i, j] = 1;
                        //Console.WriteLine($"lecturer_{i}_subject{j}");
                    }
                    else
                    {
                        a[i, j] = 0;
                    }
                }
            }
            var csv = new StringBuilder();
            csv.Append("Subject");
            for (int i = 0; i < num_lecturers; i++) csv.Append($",Lecturer_{i}");
            csv.AppendLine();
            for (int j = 0; j < num_subjects; j++)
            {
                var line = new StringBuilder();
                line.Append($"subject_{j}");
                for (int i = 0; i < num_lecturers; i++)
                {
                    if (a[i, j] == 1)
                    {
                        line.Append(",X");
                    }
                    else
                    {
                        line.Append(",");
                    }
                }
                csv.AppendLine(line.ToString());
            }
            string file = @"D:\Schedule\register_subject\register_subject.csv";
            File.WriteAllText(file, csv.ToString());
            return a;
        }
    }
}
