using Google.OrTools.Sat;
using AutoScheduling.DataLayer;
using AutoScheduling.Reader;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AutoScheduling
{
    public delegate void Last_Constraint(int num_lecturers, int num_classes, int num_days, int num_slots
            , int[,,] class_day_slot, Dictionary<(int, int, int, int), IntVar> f, CpModel model);
    public class Best
    {
        public int index { get; set; } 
        public float bestSol { get; set; }
    }
    public class AutoSchedulingMain
    {
        const int num_days = 3;
        const int num_slots_per_day = 4;
        public  bool MainFlow(IFormFile register_subject_file, IFormFile class_day_slot_file, Last_Constraint @delegate)
        {
            int num_classes, num_subject, num_lecturers;
            const int num_days = 3;
            const int num_slots_per_day = 4;
            RegisterSubjectReader registerSubjectReader = new RegisterSubjectReader();
            ClassDaySlotReader classDaySlotReader = new ClassDaySlotReader();
            var register_subject_list_raw = registerSubjectReader.readRegisterSubjectFile(register_subject_file);
            Updater updater = new Updater();
            updater.UpdateDNumberAndIsCollab(1, register_subject_list_raw);
            Getter getter = new Getter();

            var userDic_andD = getter.getAllUser();

            var userDic = userDic_andD.Item1;
            var d = userDic_andD.Item2.ToArray();
            var alphaIndexs = userDic_andD.Item3.ToArray();
            var subjectDic = getter.getAllSubject(1);

            num_subject = subjectDic.Count;
            num_lecturers = userDic.Count;

            
            //tạo register_subject
            int[,] registerSubject;//= new int[num_lecturers, num_subject];
            registerSubjectReader.createRegisterSubjectFromFile(userDic, subjectDic, register_subject_list_raw, out registerSubject);
            //tạo teacher_day_slot
            int[,,] teacher_day_slot;
            registerSubjectReader.createTeacher_Day_Slot(userDic, register_subject_list_raw, registerSubject, out teacher_day_slot);

            //check xem gv có phải collaborator hay không
            bool[] userIndex_isColab;
            registerSubjectReader.create_teacher_iscollab(userDic, register_subject_list_raw,out  userIndex_isColab);

            // tạo class_day_slot
            List < (int, int, string)> subject_class_className;
            // Tạo Class_day_slot  + subject_class_className
            var class_day_slot = classDaySlotReader.readClassDaySlotCsv(class_day_slot_file, subjectDic, out subject_class_className);


           
            //create Subject_class
            num_classes = subject_class_className.Count;
            var subject_class = new List<(int,int)>();
            foreach (var a in subject_class_className)
            {
                subject_class.Add((a.Item1, a.Item2)); 
            }
            var res = MainFlow1a(num_lecturers,num_subject,num_classes,class_day_slot, registerSubject, subject_class, teacher_day_slot,subject_class_className,subjectDic,userDic,
                d,alphaIndexs,userIndex_isColab, @delegate);
            /*
            var res1b = MainFlow1b(num_lecturers, num_subject, num_classes, class_day_slot, registerSubject, subject_class, teacher_day_slot, subject_class_className, subjectDic, userDic,
                d, alphaIndexs, @delegate, res.Item1, res.Item2);
            int count = 0;
            while (count < 10)
            {
                res1b = MainFlow1b(num_lecturers, num_subject, num_classes, class_day_slot, registerSubject, subject_class, teacher_day_slot, subject_class_className, subjectDic, userDic,
                d, alphaIndexs, @delegate, res.Item1,  res.Item2);
                if (!res1b.Item3) count++;
                else count = 0;
            }
            */
            Best best = res.Item2;
            Console.WriteLine("********** Solution best U is : " + best.bestSol);
           CsvWriter.writeScheduleFileV2(num_slots_per_day,num_days,num_lecturers,num_classes,num_subject,subject_class_className,userDic
                ,res.Item1);

            
            return true;
        }
        public (int[,,,], Best) MainFlow1a(int num_lecturers, int num_subjects, int num_classes, int[,,] class_day_slot, int[,] registerSubject,List<(int,int)> subject_class,
           int[,,] teacher_day_slot, List<(int, int, string)> subject_class_className, List<(int, string)> subjectDic,
           List<(int, int, string)> userDic,
           int[] d, float[] alphaIndexs, bool[] userIndex_isColab, Last_Constraint @delegate)
        {
            Console.WriteLine("MAIN FLOW 1A");
            CpModel model = new CpModel();
            var f = new Dictionary<(int, int, int, int), IntVar>();
            MainFlowFunctions.generateFirstConstraint(num_lecturers, num_subjects, num_classes, num_days, num_slots_per_day
                , subject_class, class_day_slot, registerSubject, teacher_day_slot, userIndex_isColab, model, f);

            //Mỗi gv phải được dạy ít nhất số slot họ mong muốn
            //
            MainFlowFunctions.teacher_teaching_mustEqualOrMoreThan_di(num_lecturers, num_classes, num_days, num_slots_per_day, d, f, model,userIndex_isColab
                ,  userDic);

            //Mỗi gv khi dạy 1 lớp 1 slot phải dạy slot còn lại 
            //MainFlowFunctions.teachAllSlotOfAClass(num_lecturers, num_classes, num_days, num_slots_per_day, f, model);
            // Đảm bảo tất cả các lớp đều có người dạy
            //MainFlowFunctions.everyClassHaveTeacher(num_lecturers, num_classes, num_days, num_slots_per_day, class_day_slot, f, model);

            @delegate.Invoke(num_lecturers, num_classes, num_days, num_slots_per_day, class_day_slot, f, model);
            MainFlowFunctions.noDuplicateClass(num_lecturers, num_classes, num_days, num_slots_per_day, class_day_slot, f, model);

            //Đảm bảo 1 người dạy 1 slot ở 1 ngày

            for (int i = 0; i < num_lecturers; i++)
                for (int k = 0; k < num_days; k++)
                    for (int l = 0; l < num_slots_per_day; l++)
                    {
                        var slotsTaken = new List<IntVar>();
                        for (int j = 0; j < num_classes; j++)
                            if (f.ContainsKey((i, j, k, l)))
                            {
                                slotsTaken.Add(f[(i, j, k, l)]);
                            }
                        model.AddLinearConstraint(LinearExpr.Sum(slotsTaken), 0, 1);
                    }

            int[,,,] res = new int[num_lecturers,num_classes,num_days,num_slots_per_day];
            Best best1 = new Best()
            {
                index = -1,
                bestSol = - 99999
            };
            CpSolver solver = new CpSolver();
            solver.StringParameters += "linearization_level:1 " + "enumerate_all_solutions:true ";
            var cb = new SolutionPrinter(f,num_lecturers, num_subjects, num_classes, class_day_slot, registerSubject
                , teacher_day_slot, subject_class_className, subjectDic, userDic,d,alphaIndexs, 500, ref res,  best1);

            
            CpSolverStatus status = solver.Solve(model, cb);
            Console.WriteLine($"Solve status: {status}");

            Console.WriteLine("Statistics");
            Console.WriteLine($"  conflicts: {solver.NumConflicts()}");
            Console.WriteLine($"  branches : {solver.NumBranches()}");
            Console.WriteLine($"  wall time: {solver.WallTime()}s");
            Console.WriteLine($"  Best Solution: {best1.bestSol}");
            for (int i = 0; i < num_lecturers; i++)
            {
                int count = 0;
                for (int j = 0; j < num_classes; j++)
                    for (int k = 0; k < num_days; k++)
                        for (int l = 0; l < num_slots_per_day; l++)
                        {
                            count += (int)res[i, j, k, l];
                            if (res[i,j,k,l] == 1 && i == 0)
                            {
                                var a = subject_class_className.First(x => x.Item2 == j);
                                var subjectName = subjectDic.First(x => x.Item1 == a.Item1).Item2;
                                Console.WriteLine($"Subject: {subjectName} - day: {k} - slot: {l} - className: {a.Item3}");
                            }
                        }
                var lecturerName = userDic.First(x => x.Item1 == i).Item3;
                Console.WriteLine($"id:{i}_name:{lecturerName}_numslots:{count}");
            }
            
            return (res,best1);

        }
    }

}