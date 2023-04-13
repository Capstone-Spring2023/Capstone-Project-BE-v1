using Google.OrTools.Sat;
using AutoScheduling.Algorithm;
using AutoScheduling.DataLayer;
using AutoScheduling.Reader;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AutoScheduling
{
    public delegate void Last_Constraint(int num_lecturers, int num_classes, int num_days, int num_slots
            , int[,,] class_day_slot, IntVar[,,,] f, CpModel model);
    public class AutoSchedulingMain
    {
        const int num_days = 3;
        const int num_slots_per_day = 4;

        public static async Task Main123(string[] args)
        {
            //createSchedule();
            

            
            Stopwatch timer = new Stopwatch();
            timer.Start();
           /// ClassDaySlotReader classDaySlotReader = new ClassDaySlotReader();
            //classDaySlotReader.readClassDaySlotCsvToDb();

            ///RegisterSubjectReader registerSubjectReader = new RegisterSubjectReader();
           // registerSubjectReader.createRegisterSubjectFileFromDatabase();

            //OutOfFlow flow = new OutOfFlow();
            //await flow.createRegisterSubjectDatabaseFromFile();

            //MainFlow();
            
            //ScheduleReader scheduleReader = new ScheduleReader();
            //scheduleReader.fromScheduleFile_writeToDatabase();
            Console.WriteLine(timer.Elapsed.ToString());
            timer.Stop();
        }


        public  bool MainFlow(IFormFile register_subject_file, IFormFile class_day_slot_file, Last_Constraint @delegate)
        {
            int num_classes, num_subject, num_lecturers;
            const int num_days = 3;
            const int num_slots_per_day = 4;
            RegisterSubjectReader registerSubjectReader = new RegisterSubjectReader();
            ClassDaySlotReader classDaySlotReader = new ClassDaySlotReader();
            Getter getter = new Getter();

            var userDic_andD = getter.getAllUser();

            var userDic = userDic_andD.Item1;
            var d = userDic_andD.Item2.ToArray();
            var alphaIndexs = userDic_andD.Item3.ToArray();
            var subjectDic = getter.getAllSubject(1);

            num_subject = subjectDic.Count;
            num_lecturers = userDic.Count;

            var register_subject_list_raw = registerSubjectReader.readRegisterSubjectFile(register_subject_file);
            //tạo register_subject
            int[,] registerSubject;//= new int[num_lecturers, num_subject];
            registerSubjectReader.createRegisterSubjectFromFile(userDic, subjectDic, register_subject_list_raw, out registerSubject);
            //tạo teacher_day_slot
            int[,,] teacher_day_slot;
            registerSubjectReader.createTeacher_Day_Slot(userDic, register_subject_list_raw, registerSubject, out teacher_day_slot);


            // tạo class_day_slot
            List < (int, int, string)> subject_class_className;
            // Tạo Class_day_slot  + subject_class_className
            var class_day_slot = classDaySlotReader.readClassDaySlotCsv(class_day_slot_file, subjectDic, out subject_class_className);


           
            //create Subject_class
            num_classes = subject_class_className.Count;
            var subject_class = new int[num_subject, num_classes];
            foreach (var a in subject_class_className)
            {
                subject_class[a.Item1, a.Item2] = 1; 
            }
            bool check = MainFlow1a(num_lecturers,num_subject,num_classes,class_day_slot, registerSubject, subject_class, teacher_day_slot,subject_class_className,subjectDic,userDic,
                d,alphaIndexs, @delegate);
            return check;
        }
        public  bool MainFlow1a(int num_lecturers,int num_subjects,int num_classes,int[,,] class_day_slot, int[,] registerSubject, int[,] subject_class,
           int[,,] teacher_day_slot, List<(int, int, string)> subject_class_className, List<(int, string)> subjectDic,
           List<(int, int, string)> userDic,
           int[] d, float[] alphaIndexs, Last_Constraint @delegate)
        {
            CpModel model = new CpModel();
            IntVar[,,,] f = new IntVar[num_lecturers, num_classes, num_days, num_slots_per_day];
            MainFlowFunctions.generateFirstConstraint(num_lecturers, num_subjects, num_classes, num_days, num_slots_per_day
                , subject_class, class_day_slot, registerSubject, teacher_day_slot, model, f);

            //Mỗi gv phải được dạy ít nhất số slot họ mong muốn
            //
            MainFlowFunctions.teacher_teaching_mustEqualOrMoreThan_di(num_lecturers, num_classes, num_days, num_slots_per_day, d, f, model);

            //Mỗi gv khi dạy 1 lớp 1 slot phải dạy slot còn lại 
            //MainFlowFunctions.teachAllSlotOfAClass(num_lecturers, num_classes, num_days, num_slots_per_day, f, model);
            // Đảm bảo tất cả các lớp đều có người dạy
            //MainFlowFunctions.everyClassHaveTeacher(num_lecturers, num_classes, num_days, num_slots_per_day, class_day_slot, f, model);

            @delegate.Invoke(num_lecturers, num_classes, num_days, num_slots_per_day, class_day_slot, f, model);
            //MainFlowFunctions.noDuplicateClass(num_lecturers, num_classes, num_days, num_slots_per_day, class_day_slot, f, model);

            //Đảm bảo 1 người dạy 1 slot ở 1 ngày
            for (int i = 0; i < num_lecturers; i++)
                for (int k = 0; k < num_days; k++)
                    for (int l = 0; l < num_slots_per_day; l++)
                    {
                        IntVar[] slotsTaken = new IntVar[num_classes];
                        for (int j = 0; j < num_classes; j++)
                        {
                            slotsTaken[j] = f[i, j, k, l];
                        }
                        model.AddLinearConstraint(LinearExpr.Sum(slotsTaken) ,0, 1);
                    }
            int[,,,] res;
            CpSolver solver = new CpSolver();
            var cb = new SolutionPrinter(f,num_lecturers, num_subjects, num_classes, class_day_slot, registerSubject, subject_class
                , teacher_day_slot, subject_class_className, subjectDic, userDic,d,alphaIndexs, 500, out res);

            
            CpSolverStatus status = solver.Solve(model, cb);
            Console.WriteLine($"Solve status: {status}");

            Console.WriteLine("Statistics");
            Console.WriteLine($"  conflicts: {solver.NumConflicts()}");
            Console.WriteLine($"  branches : {solver.NumBranches()}");
            Console.WriteLine($"  wall time: {solver.WallTime()}s");

            if (status.Equals(CpSolverStatus.Optimal))
            {
                for (int i = 0; i < num_lecturers; i++)
                {
                    int count = 0;
                    for (int j = 0; j < num_classes; j++)
                        for (int k = 0; k < num_days; k++)
                            for (int l = 0; l < num_slots_per_day; l++)
                            {
                                count += (int)res[i, j, k, l];
                            }
                    var lecturerName = userDic.First(x => x.Item1 == i).Item3;
                    Console.WriteLine($"id:{i}_name:{lecturerName}_numslots:{count}");
                }
                CsvWriter.writeScheduleFileV2(num_slots_per_day, num_days, num_lecturers, num_classes, num_subjects, subject_class,
                    subject_class_className, userDic, solver, res);
                return true;
            }
            else
            {
                return false;
            }

        }
        public static void createSchedule()
        {
            int num_classes = 91;
            Getter getter = new Getter();
            var subject_Dic = getter.getAllSubject(1);
            //int[,] subject_class = create_subject_class();
            var subjectClassGenerator = new SubjectClassGenerator()
            {
                subjectDic = subject_Dic,
            };
            List<(int, int)> subject_class_list = subjectClassGenerator.create_subject_class(num_classes);
            
            //for (int i = 0)
            int[,,] class_day_slot = ScheduleGenerator.Create_Schedule(num_classes, num_days, num_slots_per_day, subject_class_list, subject_Dic);
        }
        public static void OldFlow()
        {/*
            int[] d = new int[num_lecturers];
            Random r = new Random();

            for (int i = 0; i < num_lecturers; i++)
            {
                d[i] = (int)r.NextInt64(0, 2);
                d[i] = d[i] * 2 + 4;
            }
            int[,] subject_class = create_subject_class();
            int[,,] class_day_slot = ScheduleGenerator.Create_Schedule(num_classes, num_days, num_slots_per_day);
            
            int[,] register_subject = RegisterSubjectGenerator.Create_Lecturer_Subject(num_lecturers, num_subjects);


            int[,,] teacher_day_slot = LecturerRegisterSlotGenerator
                .generate(num_lecturers, num_subjects, num_classes, num_days, num_slots_per_day, d, subject_class, register_subject, class_day_slot);

            bool check = MainFlow1a(class_day_slot, register_subject, subject_class, teacher_day_slot, d);

            if (!check)
            {
                //var a = MainFlow1b(class_day_slot, register_subject, subject_class, teacher_day_slot, d);

            }
            //RegisterSubjectReader reader = new RegisterSubjectReader();
            //reader.readRegisterSubjectFile();*/
        }
       
    }

}