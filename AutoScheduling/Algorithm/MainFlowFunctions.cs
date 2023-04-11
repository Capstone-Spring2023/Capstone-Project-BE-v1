using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduling
{
    public class MainFlowFunctions
    {
        public static IntVar[,,,] generateFirstConstraint(int num_lecturers, int num_subjects,
            int num_classes, int num_days, int num_slots, int[,] subject_class
            , int[,,] class_day_slot, int[,] registerSubject, int[,,] teacher_day_slot, CpModel model, IntVar[,,,] f)
        {
            for (int i = 0; i < num_lecturers; i++)
                for (int j = 0; j < num_classes; j++)
                {
                    int subjectId = 0;
                    for (int k = 0; k < num_subjects; k++)
                        if (subject_class[k, j] == 1)
                        {
                            subjectId = k;
                            break;
                        }
                    for (int k = 0; k < num_days; k++)
                        for (int l = 0; l < num_slots; l++)
                        {
                            if (class_day_slot[j, k, l] == 1 // class ở ngày k slot l CÓ
                            &&
                            registerSubject[i, subjectId] == 1  // Lecturer có đký Subject của class này
                            && teacher_day_slot[i, k, l] == 1 // Lecturer Được dạy slot họ yêu cầu
                            )
                            {
                                f[i, j, k, l] = model.NewIntVar(0, 1, "");
                            }
                            else f[i, j, k, l] = model.NewIntVar(0, 0, "");
                        }
                }
            return f;
        }
        //Mỗi gv phải được dạy ít nhất số slot họ mong muốn
        public static void teacher_teaching_mustEqualOrMoreThan_di(int num_lecturers, int num_classes, int num_days,
            int num_slots, int[] d, IntVar[,,,] f, CpModel model)
        {
            for (int i = 0; i < num_lecturers; i++)
            {
                int count = 0;
                IntVar[] teach_num_classes = new IntVar[num_classes * num_days * num_slots];
                for (int j = 0; j < num_classes; j++)
                    for (int k = 0; k < num_days; k++)
                        for (int l = 0; l < num_slots; l++)
                        {
                            teach_num_classes[count] = f[i, j, k, l];
                            count++;
                        }
                LinearExpr linearExpr = LinearExpr.Sum(teach_num_classes);
                if (d[i] > 0)
                model.Add(linearExpr >= d[i]  );
                
                //model.Add(linearExpr <= d[i] * 5);
            }
        }
        public static void teachAllSlotOfAClass(int num_lecturers, int num_classes, int num_days, int num_slots
            , IntVar[,,,] f, CpModel model)
        {
            for (int i = 0; i < num_lecturers; i++)
            {
                for (int j = 0; j < num_classes; j++)
                {
                    int count = 0;
                    IntVar[] teach_class = new IntVar[num_days * num_slots];
                    for (int k = 0; k < num_days; k++)
                        for (int l = 0; l < num_slots; l++)
                        {
                            teach_class[count] = f[i, j, k, l];
                            count++;
                        }

                    LinearExpr linearExpr = LinearExpr.Sum(teach_class);

                    model.Add(linearExpr >= 0);
                    model.Add(linearExpr != 1);
                    model.Add(linearExpr <= 2);
                }
            }
        }
        public static void everyClassHaveTeacher(int num_lecturers,int num_classes, int num_days, int num_slots
            ,int [,,] class_day_slot, IntVar[,,,] f, CpModel model)
        {
            for (int j = 0; j < num_classes; j++)
                for (int k = 0; k < num_days; k++)
                    for (int l = 0; l < num_slots; l++)
                        if (class_day_slot[j, k, l] == 1)
                        {
                            IntVar[] a = new IntVar[num_lecturers];
                            for (int i = 0; i < num_lecturers; i++)
                            {
                                a[i] = f[i, j, k, l];
                            }
                            model.Add(LinearExpr.Sum(a) == 1);
                        }
        }
        public static void teacher_teaching_mustSatisfy_di(int num_lecturers, int num_classes, int num_days,
            int num_slots, int[] d, IntVar[,,,] f, CpModel model)
        {
            
        }


    }
}
