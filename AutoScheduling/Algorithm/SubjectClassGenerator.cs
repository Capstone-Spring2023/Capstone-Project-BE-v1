using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.Sat;
namespace AutoScheduling.Algorithm
{
    public class SubjectClassGenerator
    {
        public List<(int, string)> subjectDic { get; set; }
        public List<(int,int)> create_subject_class(int num_classes)
        {
            //CpModel model = new CpModel();
            var res = new List<(int,int)>();
            //IntVar[,]
            //MATH
            for (int i = 0; i < 36; i++)
            {
                if (i < 12) res.Add((0, i));
                if (i>= 12 && i < 24) res.Add((1, i));
                if (i>= 24 && i < 36) res.Add((2, i));
            }
            //PRN + PRJ
            for (int  i= 0; i < 40; i++)
            {
                if (i < 10) res.Add((3, i +36));
                if (i >= 10 && i < 20) res.Add((4, i + 36));
                if (i >= 20 && i < 30) res.Add((5, i + 36));
                if (i >= 30 && i < 40) res.Add((6, i + 36));
            }
            // Politic
            for (int i = 0; i < 15; i++)
            {
                if (i < 5) res.Add((7, i + 76));
                if (i >= 5 && i < 10) res.Add((8, i + 76));
                if (i >= 10 && i < 15) res.Add((9, i + 76));
            }
            return res;
        }

    }
}
