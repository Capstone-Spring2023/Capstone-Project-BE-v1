using Google.OrTools.Sat;
using AutoScheduling.Algorithm;
using AutoScheduling.DataLayer;
using AutoScheduling.Reader;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutoScheduling
{
    public class Program
    {
        const int num_classes = 60;
        const int num_days = 6;
        const int num_slots_per_day = 4;
        const int num_lecturers = 12;
        const int num_subjects = 10;


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


        public static void MainFlow()
        {
            int num_classes, num_subject, num_lecturers;
            const int num_days = 6;
            const int num_slots_per_day = 4;
            RegisterSubjectReader registerSubjectReader = new RegisterSubjectReader();
            ClassDaySlotReader classDaySlotReader = new ClassDaySlotReader();
            Getter getter = new Getter();

            var userDic = getter.getAllUser();
            var subjectDic = getter.getAllSubject(1);

            num_subject = subjectDic.Count;
            num_lecturers = userDic.Count;

            var register_subject_list_raw = registerSubjectReader.readRegisterSubjectFile();
            //tạo register_subject
            int[,] registerSubject;//= new int[num_lecturers, num_subject];
            registerSubjectReader.createRegisterSubjectFromFile(userDic, subjectDic, register_subject_list_raw, out registerSubject);
            //tạo teacher_day_slot
            int[,,] teacher_day_slot;
            registerSubjectReader.createTeacher_Day_Slot(userDic, register_subject_list_raw, registerSubject, out teacher_day_slot);
            // tạo class_day_slot
            List < (int, int, string)> subject_class_className;
            
            var class_day_slot = classDaySlotReader.readClassDaySlotCsv(subjectDic, out subject_class_className);


            int[] d = new int[num_lecturers];

            for (int i = 0; i < num_lecturers; i++)
            {
                int count = 0;
                for (int day = 0; day < num_days; day++)
                {
                    for (int slot = 0; slot < num_slots_per_day; slot++)
                    {
                        count += teacher_day_slot[i, day, slot];
                    }
                }
                if (count < 8) d[i] = 0;
                else d[i] = 8;
            }
            //create Subject_class
            num_classes = subject_class_className.Count;
            var subject_class = new int[num_subject, num_classes];
            foreach (var a in subject_class_className)
            {
                subject_class[a.Item1, a.Item2] = 1; 
            }
            bool check = MainFlow1a(num_lecturers,num_subject,num_classes,class_day_slot, registerSubject, subject_class, teacher_day_slot,subject_class_className,subjectDic,userDic,
                d);

        }
        public static bool MainFlow1a(int num_lecturers,int num_subjects,int num_classes,int[,,] class_day_slot, int[,] registerSubject, int[,] subject_class,
           int[,,] teacher_day_slot, List<(int, int, string)> subject_class_classNam, List<(int, string)> subjectDic,
           List<(int, int, string)> userDic,
           int[] d)
        {
            CpModel model = new CpModel();
            IntVar[,,,] f = new IntVar[num_lecturers, num_classes, num_days, num_slots_per_day];
            MainFlowFunctions.generateFirstConstraint(num_lecturers, num_subjects, num_classes, num_days, num_slots_per_day
                , subject_class, class_day_slot, registerSubject, teacher_day_slot, model, f);

            //Mỗi gv phải được dạy ít nhất số slot họ mong muốn
            //
            MainFlowFunctions.teacher_teaching_mustEqualOrMoreThan_di(num_lecturers, num_classes, num_days, num_slots_per_day, d, f, model);

            //Mỗi gv khi dạy 1 lớp 1 slot phải dạy slot còn lại 
            MainFlowFunctions.teachAllSlotOfAClass(num_lecturers, num_classes, num_days, num_slots_per_day, f, model);
            // Đảm bảo tất cả các lớp đều có người dạy
            MainFlowFunctions.everyClassHaveTeacher(num_lecturers, num_classes, num_days, num_slots_per_day, class_day_slot, f, model);
            
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
                        model.Add(LinearExpr.Sum(slotsTaken) <= 1);
                    }
            CpSolver solver = new CpSolver();
            var cb = new SolutionPrinter(f, 2);
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
                                count +=(int) solver.Value(f[i, j, k, l]);
                            }
                    var lecturerName = userDic.First(x => x.Item1 == i).Item3;
                    Console.WriteLine($"id:{i}_name:{lecturerName}_numslots:{count}");
                }
                CsvWriter.writeScheduleFileV2(num_slots_per_day, num_days, num_lecturers, num_classes, num_subjects, subject_class,
                    subject_class_classNam, userDic, solver, f);
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
        
        public static int[,] create_subject_class()
        {
            int[,] b = new int[num_subjects, num_classes];
            for (int i = 0; i < num_subjects; i++)
                for (int j = i * 6; j < (i + 1) * 6; j++)
                {
                    b[i, j] = 1;
                }
            return b;
        }
       
        public static (int[,,,],int[,,]) MainFlow1b(int[,,] class_day_slot, int[,] registerSubject, int[,] subject_class, int[,,] teacher_day_slot, int[] d)
        {
            CpModel model = new CpModel();
            IntVar[,,,] f = new IntVar[num_lecturers, num_classes, num_days, num_slots_per_day];
            MainFlowFunctions.generateFirstConstraint(num_lecturers, num_subjects, num_classes, num_days, num_slots_per_day
                , subject_class, class_day_slot, registerSubject, teacher_day_slot, model, f);

            //Mỗi gv phải được dạy ít nhất số slot họ mong muốn
            MainFlowFunctions.teacher_teaching_mustEqualOrMoreThan_di(num_lecturers, num_classes, num_days, num_slots_per_day, d, f, model);

            //Mỗi gv khi dạy 1 lớp 1 slot phải dạy slot còn lại 
            MainFlowFunctions.teachAllSlotOfAClass(num_lecturers, num_classes, num_days, num_slots_per_day, f, model);
            // Đảm bảo tất cả các lớp đều có người dạy
            //MainFlowFunctions.everyClassHaveTeacher(num_lecturers, num_classes, num_days, num_slots_per_day, class_day_slot, f, model);

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
                        model.Add(LinearExpr.Sum(slotsTaken) <= 1);
                    }
            CpSolver solver = new CpSolver();
            var cb = new SolutionPrinter(f, 2);
            CpSolverStatus status = solver.Solve(model, cb);
            Console.WriteLine($"Solve status: {status}");

            Console.WriteLine("Statistics");
            Console.WriteLine($"  conflicts: {solver.NumConflicts()}");
            Console.WriteLine($"  branches : {solver.NumBranches()}");
            Console.WriteLine($"  wall time: {solver.WallTime()}s");

            if (status.Equals(CpSolverStatus.Optimal))
            {
               // CsvWriter.writeScheduleFile(num_slots_per_day, num_days, num_lecturers, num_classes, num_subjects, subject_class,
                //   solver, f);

            }
            int[,,] schedule_left = new int[num_classes, num_days, num_slots_per_day];
            //tìm lịch còn thiếu còn lại
            for (int j = 0; j < num_classes; j++)
                for (int k = 0; k < num_days; k++)
                    for (int l = 0; l < num_slots_per_day; l++)
                        if (class_day_slot[j, k, l] == 1)
                        {
                            bool check = false;
                            for (int i = 0; i < num_lecturers; i++)
                            {
                                if (solver.Value(f[i, j, k, l]) == 1)
                                {
                                    check = true;
                                    break;
                                }
                            }
                            if (check) schedule_left[j, k, l] = 0;
                            else schedule_left[j, k, l] = 1;
                        }
            int[,,,] f1 = new int[num_lecturers, num_classes, num_days, num_slots_per_day];
            for (int i = 0; i < num_lecturers; i++)
                for (int k = 0; k < num_days; k++)
                    for (int l = 0; l < num_slots_per_day; l++)
                    {
                        for (int j = 0; j < num_classes; j++)
                        {
                            if (solver.Value(f[i, j, k, l]) == 1)
                            {
                                f1[i, j, k, l] = 1;
                            }
                            else
                            {
                                f1[i, j, k, l] = 0;
                            }
                        }

                    }
            return (f1, schedule_left);
        }
    }

}