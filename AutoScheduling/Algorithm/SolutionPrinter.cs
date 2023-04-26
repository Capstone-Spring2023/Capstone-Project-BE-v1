using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduling
{
    public class SolutionPrinter : CpSolverSolutionCallback
    {
        private int solutionCount_;
        const int num_days = 3;
        const int num_slots_per_day = 4;

        private Dictionary<(int, int, int, int), IntVar> f;
        private int num_lecturers;
        private int num_subjects;
        private int num_classes;
        private int[,,] class_day_slot;
        private int[,] registerSubject;
       
        private List<(int, int, string)> subject_class_className;
        private List<(int, string)> subjectDic;    
        private List<(int, int, string)> userDic;
        private int[,,] teacher_day_slot;
        private int[] d;
        private float[] alphaIndexs;
        private int solutionLimit_;
        private float[] u;
        int[,,,] res;
        int[,] ableSubject;
        Best best;
        public SolutionPrinter(Dictionary<(int,int,int,int), IntVar> f,int num_lecturers, int num_subjects, int num_classes, int[,,] class_day_slot, int[,] registerSubject,
           int[,,] teacher_day_slot, List<(int, int, string)> subject_class_className, List<(int, string)> subjectDic,
           List<(int, int, string)> userDic,
           int[] d, float[] alphaIndexs, int limit, int[,] ableSubject, ref int[,,,] res,Best best)
        {
            this.f = f;
            this.num_lecturers = num_lecturers; 
            this.num_subjects = num_subjects; 
            this.num_classes = num_classes; 
            this.class_day_slot = class_day_slot;   
            this.registerSubject = registerSubject; 
           
            this.teacher_day_slot = teacher_day_slot;   
            this.subject_class_className = subject_class_className;
            this.subjectDic = subjectDic;
            this.userDic = userDic;
            this.d = d;
            this.alphaIndexs = alphaIndexs;
            this.ableSubject = ableSubject;
            this.res = res;
            solutionLimit_ = limit;
            u = new float[num_lecturers];
            this.best = best;
        }

        public override void OnSolutionCallback()
        {
            //Console.WriteLine($"-------------------------------------------");
           // Console.WriteLine($"Start Of Solution: #{solutionCount_}");

            float a = 0;
            for (int i = 0; i < num_lecturers; i++)
            {
                u[i] = Constant.POINT;
                //Console.WriteLine($"+++++++++++++++++++++++++++++++++++++++++++++");
                // Console.WriteLine($"u{i} - {userDic.First(x => x.Item1 == i).Item3}");
                int count_num_teaching_class = 0;
                var subjectIndexAlreadyMinus = new List<int>();

                for (int k = 0; k < num_days; k++)
                {

                    int previous_slot = -1;
                    for (int l = 0; l < num_slots_per_day; l++)
                    {
                        for (int j = 0; j < num_classes; j++)
                        {
                            if (f.ContainsKey((i, j, k, l)))

                                if (Value(f[(i, j, k, l)]) == 1)
                                {
                                    // Check độ hài lòng ở slot dạy

                                    if (previous_slot != -1 && l - previous_slot > 1)
                                    {

                                        u[i] -= (l - previous_slot - 1);
                                        //  if (i == 0) Console.WriteLine($"Minus in tight slot. day {k} previous slot: {previous_slot}, current slot: {l}, u[i] = {u[i]}");
                                    }
                                    previous_slot = l;
                                    count_num_teaching_class++;

                                    // Check độ hài lòng với register subject
                                    int subjectIndex = subject_class_className.First(x => x.Item2 == j).Item1;
                                    if (ableSubject[i, subjectIndex] == 0 || registerSubject[i, subjectIndex] == 0)
                                    {
                                        if (!subjectIndexAlreadyMinus.Contains(subjectIndex))
                                        {
                                            subjectIndexAlreadyMinus.Add(subjectIndex);
                                            if (ableSubject[i, subjectIndex] == 0) u[i] -= 3;
                                            else if (registerSubject[i, subjectIndex] == 0) u[i] -= 1;
                                            if (i == 0)
                                            {
                                                //   Console.WriteLine($"Minus in register subject: {subjectDic.First(x => x.Item1 == subjectIndex)} " +
                                                //   $"- class {subject_class_className.First(x => x.Item2 == j).Item3} - u[i] = {u[i]}");
                                            }
                                        }
                                    }

                                    //Check độ hài lòng của register slot
                                    if (teacher_day_slot[i, k, l] == 0)
                                    {
                                        u[i] -= 1;
                                        // if (i == 0)  Console.WriteLine($"Minus in teacher_day_slot: day{k} - slot {l} - u[i] = {u[i]}");
                                    }


                                }
                        }
                    }
                }
                // Check đô hài lòng với d[i]

                if (count_num_teaching_class < d[i] || count_num_teaching_class > 10)
                {
                    u[i] -= Math.Abs(count_num_teaching_class - d[i]);
                }
                else
                {
                    u[i] += Math.Abs(count_num_teaching_class - d[i]);
                }
                //if (i == 0) Console.WriteLine($"num classes: {count_num_teaching_class} - d[i]: {d[i]}");
                //if (i == 0)  Console.WriteLine($"u{i}: {u[i]}##{userDic.First(x => x.Item1 == i).Item3}  - alphaIndex: {alphaIndexs[i]} - value : {u[i] * alphaIndexs[i]}");
                a += u[i] * alphaIndexs[i];
            }



            if (a > best.bestSol)
            {
                best.bestSol = a;
                best.index = solutionCount_;
                for (int i = 0; i < num_lecturers; i++)
                {
                    for (int j = 0; j < num_classes; j++)
                    {
                        for (int k = 0; k < num_days; k++)
                        {
                            for (int l = 0; l < num_slots_per_day; l++)
                                if (f.ContainsKey((i, j, k, l)))
                                {
                                    res[i, j, k, l] = (int)Value(f[(i, j, k, l)]);
                                }
                        }
                    }
                }
            }


            //Console.Write($"Solution #{solutionCount_}: {a} - ");
            solutionCount_++;
            if (solutionCount_ % 100 == 0)
            {
                Console.WriteLine($"Solution #{solutionCount_}: {best.bestSol}");
            }
            if (solutionCount_ >= solutionLimit_)
            {
                Console.WriteLine($"Stop search after {solutionLimit_} solutions");
                StopSearch();
            }
        }

        public int SolutionCount()
        {
            return solutionCount_;
        }

  
    }
}
