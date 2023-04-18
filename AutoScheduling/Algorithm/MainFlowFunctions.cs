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
        public static Dictionary<(int, int, int, int), IntVar> generateFirstConstraint(int num_lecturers, int num_subjects,
            int num_classes, int num_days, int num_slots, List<(int,int)> subject_class
            , int[,,] class_day_slot, int[,] registerSubject, int[,,] teacher_day_slot, bool[] userIndex_Colab, CpModel model, Dictionary<(int, int, int, int), IntVar> f)
        {
            //model.Model.Variables.Capacity = num_lecturers * num_classes;
            
            for (int i = 0; i < num_lecturers; i++)
                for (int j = 0; j < num_classes; j++)
                {

                    for (int k = 0; k < num_days; k++)
                        for (int l = 0; l < num_slots; l++)
                        {
                            if (class_day_slot[j, k, l] == 1)
                            {
                                if (!userIndex_Colab[i])
                                {
                                    f.Add((i, j, k, l), model.NewIntVar(0, 1, $"lecturer: {i} - class: {j} - day: {k} - slot: {l}"));
                                }
                                else
                                {
                                    int subjectId = -1;
                                    try
                                    {
                                        subjectId = subject_class.First(x => x.Item2 == j).Item1;
                                        if (teacher_day_slot[i, k, l] == 1 && registerSubject[i, subjectId] == 1)
                                        {
                                            f.Add((i, j, k, l), model.NewIntVar(0, 1, $"lecturer: {i} - class: {j} - day: {k} - slot: {l}"));
                                        }
                                    }catch (Exception ex)
                                    {
                                        Console.WriteLine("Fault: " + j + " " +subjectId);
                                    }
                                }
                                
                            }
                            
                        }
                }
            return f;
        }
        //Mỗi gv phải được dạy ít nhất số slot họ mong muốn
        public static void teacher_teaching_mustEqualOrMoreThan_di(int num_lecturers, int num_classes, int num_days,
            int num_slots, int[] d, Dictionary<(int, int, int, int), IntVar> f, CpModel model,bool[] userIndex_isColab, List<(int, int, string)> userDic)
        {
            for (int i = 0; i < num_lecturers; i++)
            {
                List<IntVar> teach_num_classes = new List<IntVar>();
                for (int j = 0; j < num_classes; j++)
                    for (int k = 0; k < num_days; k++)
                        for (int l = 0; l < num_slots; l++)
                            if (f.ContainsKey((i, j, k, l)))
                            {
                                teach_num_classes.Add(f[(i, j, k, l)]);
                            }
                LinearExpr linearExpr = LinearExpr.Sum(teach_num_classes);

                //model.Add(linearExpr >= d[i]);
                //model.Add(linearExpr <= 15);
                if (userIndex_isColab[i])
                {
                    var a = userDic.First(x=> x.Item1 == i);
                    Console.WriteLine($"i: {i} - userId: {a.Item2} - user FullName: {a.Item3} -d[i]: {d[i]}");
                }
                model.AddLinearConstraint(linearExpr, d[i], 12);

            }
        }
        public static void everyClassHaveTeacher(int num_lecturers, int num_classes, int num_days, int num_slots
            , int[,,] class_day_slot, Dictionary<(int, int, int, int), IntVar> f, CpModel model)
        {
            List<IntVar> b = new List<IntVar>();
            for (int j = 0; j < num_classes; j++)
                for (int k = 0; k < num_days; k++)
                    for (int l = 0; l < num_slots; l++)
                        if (class_day_slot[j, k, l] == 1)
                        {
                            List<IntVar> a = new List<IntVar>();
                            for (int i = 0; i < num_lecturers; i++)
                                if (f.ContainsKey((i, j, k, l)))
                                {
                                    //a.Add(f[i, j, k, l]);
                                    b.Add(f[(i, j, k, l)]);
                                }
                            //model.Add(LinearExpr.Sum(a) == 1);

                        }
            model.Add(LinearExpr.Sum(b) == num_classes);
        }
        public static void noDuplicateClass(int num_lecturers, int num_classes, int num_days, int num_slots
            , int[,,] class_day_slot, Dictionary<(int, int, int, int), IntVar> f, CpModel model)
        {
            for (int j = 0; j < num_classes; j++)
                for (int k = 0; k < num_days; k++)
                    for (int l = 0; l < num_slots; l++)
                        if (class_day_slot[j, k, l] == 1)
                        {
                            List<IntVar> a = new List<IntVar>();
                            for (int i = 0; i < num_lecturers; i++)
                                if (f.ContainsKey((i, j, k, l)))
                                {
                                    a.Add(f[(i, j, k, l)]);
                                }
                            
                            model.Add(LinearExpr.Sum(a) <= 1);
                        }
        }

    }
}
