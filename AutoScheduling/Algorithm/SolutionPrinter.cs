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
        const int num_classes = 60;
        const int num_days = 6;
        const int num_slots_per_day = 4;
        const int num_lecturers = 12;
        const int num_subjects = 10;

        private IntVar[,,,] f;
        private int solutionLimit_;
        public SolutionPrinter( IntVar[,,,] f, int limit)
        {
          
            this.f = f;
            solutionLimit_ = limit;
        }

        public override void OnSolutionCallback()
        {
            Console.WriteLine($"Solution #{solutionCount_}:");
            for (int j = 0; j < num_days; j++)
            {
                for (int k = 0; k < num_classes; k++)
                {
                    for (int l = 0; l < num_days; l++)
                        for (int i = 0; i < num_slots_per_day; i++)
                        {
                            
                        }
                }
            }
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
