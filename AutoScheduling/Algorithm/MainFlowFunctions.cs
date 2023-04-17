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
            model.Model.Variables.Capacity = num_lecturers * num_classes * num_days * num_slots;
            for (int i = 0; i < num_lecturers; i++)
                for (int j = 0; j < num_classes; j++)
                {

                    for (int k = 0; k < num_days; k++)
                        for (int l = 0; l < num_slots; l++)
                        {
                            if (class_day_slot[j, k, l] == 1)
                            {
                                f[i, j, k, l] = model.NewIntVar(0, 10, $"lecturer: {i} - class: {j} - day: {k} - slot: {l}");
                            }
                            else
                            {
                                f[i, j, k, l] = model.NewIntVar(0, 0, $"lecturer: {i} - class: {j} - day: {k} - slot: {l}");
                            }
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
                
                    //model.Add(linearExpr >= d[i]);
                    //model.Add(linearExpr <= 15);
                    model.AddLinearConstraint(linearExpr, d[i], 12);
               
            }
        }
        public static void everyClassHaveTeacher(int num_lecturers, int num_classes, int num_days, int num_slots
            , int[,,] class_day_slot, IntVar[,,,] f, CpModel model)
        {
            List<IntVar> b = new List<IntVar>();
            for (int j = 0; j < num_classes; j++)
                for (int k = 0; k < num_days; k++)
                    for (int l = 0; l < num_slots; l++)
                        if (class_day_slot[j, k, l] == 1)
                        {
                            IntVar[] a = new IntVar[num_lecturers];
                            for (int i = 0; i < num_lecturers; i++)
                            {
                                //a[i] = f[i, j, k, l];
                                b.Add(f[i, j, k, l]);
                            }
                            //model.Add(LinearExpr.Sum(a) == 1);
                        }
            model.Add(LinearExpr.Sum(b) >= num_classes);
        }
        public static void noDuplicateClass(int num_lecturers, int num_classes, int num_days, int num_slots
            , int[,,] class_day_slot, IntVar[,,,] f, CpModel model)
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
                            model.Add(LinearExpr.Sum(a) <= 1);
                        }
        }

    }
}
