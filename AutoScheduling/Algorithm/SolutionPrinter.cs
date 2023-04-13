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

        private IntVar[,,,] f;
        private int num_lecturers;
        private int num_subjects;
        private int num_classes;
        private int[,,] class_day_slot;
        private int[,] registerSubject;
        private int[,] subject_class;
        private List<(int, int, string)> subject_class_className;
        private List<(int, string)> subjectDic;    
        private List<(int, int, string)> userDic;
        private int[,,] teacher_day_slot;
        private int[] d;
        private float[] alphaIndexs;
        private int solutionLimit_;
        private float[] u;
        private float U = -9999999;
        int[,,,] res;
        public SolutionPrinter(IntVar[,,,] f,int num_lecturers, int num_subjects, int num_classes, int[,,] class_day_slot, int[,] registerSubject, int[,] subject_class,
           int[,,] teacher_day_slot, List<(int, int, string)> subject_class_className, List<(int, string)> subjectDic,
           List<(int, int, string)> userDic,
           int[] d, float[] alphaIndexs, int limit,out int[,,,] res)
        {
            this.f = f;
            this.num_lecturers = num_lecturers; 
            this.num_subjects = num_subjects; 
            this.num_classes = num_classes; 
            this.class_day_slot = class_day_slot;   
            this.registerSubject = registerSubject; 
            this.subject_class = subject_class;
            this.teacher_day_slot = teacher_day_slot;   
            this.subject_class_className = subject_class_className;
            this.subjectDic = subjectDic;
            this.userDic = userDic;
            this.d = d;
            this.alphaIndexs = alphaIndexs;
            res = new int[num_lecturers, num_classes, num_days, num_slots_per_day];
            this.res = res;
            solutionLimit_ = limit;
            u = new float[num_lecturers];
        }

        public override void OnSolutionCallback()
        {
            Console.WriteLine($"Start Of Solution: #{solutionCount_}: {U}");
            float a = 0;
            for (int i = 0; i < num_lecturers; i++)
            {
                u[i] = 20;
                int count_num_teaching_class = 0;
                for (int j = 0; j< num_classes; j++)
                {
                    for (int k = 0; k < num_days; k++)
                    {

                        int previous_slot = -1;
                        for (int l = 0; l < num_slots_per_day; l++)
                        {
                            if (Value(f[i,j,k,l]) == 1)
                            {
                                // Check độ hài lòng ở slot dạy
                                if (l - previous_slot > 1) u[i] -= (l - previous_slot);
                                previous_slot = l;
                                count_num_teaching_class++;

                                // Check độ hài lòng với register subject
                                int subjectIndex = subject_class_className.First(x => x.Item2 == j).Item1;
                                if (registerSubject[i, subjectIndex] == 0) u[i] -= 1;

                                //Check độ hài lòng của register slot
                                if (teacher_day_slot[i, k, l] == 0) u[i] -= 1;

                               
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
                Console.WriteLine($"u{i}: {u[i]} ## alphaIndex: {alphaIndexs[i]} ##value {i}: {u[i] * alphaIndexs[i]}");
                a += u[i] * alphaIndexs[i];
            }
            if (a > U)
            {
                U = a;
                for (int i = 0; i < num_lecturers; i++)
                {
                    for (int j = 0; j< num_classes; j++)
                    {
                        for (int k = 0; k < num_days; k++)
                        {
                            for (int l = 0; l < num_slots_per_day; l++)
                            {
                                res[i, j, k, l] = (int) Value(f[i, j, k, l]);

                            }
                        }
                    }
                }
            }

             Console.WriteLine($"Solution #{solutionCount_}: {U}");
            solutionCount_++;
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
