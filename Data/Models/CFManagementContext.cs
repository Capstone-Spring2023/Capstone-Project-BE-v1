using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Data.Models
{
    public partial class CFManagementContext : DbContext
    {
        public CFManagementContext()
        {
        }

        public CFManagementContext(DbContextOptions<CFManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AvailableSubject> AvailableSubjects { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<ClassAsubject> ClassAsubjects { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<CurrentHeader> CurrentHeaders { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<ExamPaper> ExamPapers { get; set; } = null!;
        public virtual DbSet<ExamSchedule> ExamSchedules { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<RegisterSlot> RegisterSlots { get; set; } = null!;
        public virtual DbSet<RegisterSubject> RegisterSubjects { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
        public virtual DbSet<Semester> Semesters { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<Type> Types { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=13.212.106.245,1433;Initial Catalog=CFManagement;User ID=sa;Password=1234567890Aa");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AvailableSubject>(entity =>
            {
                entity.ToTable("AvailableSubject");

                entity.Property(e => e.LeaderName).HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.SubjectName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Leader)
                    .WithMany(p => p.AvailableSubjects)
                    .HasForeignKey(d => d.LeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AvailableSubject_Users");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.AvailableSubjects)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AvailableSubject_Semester");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.AvailableSubjects)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AvailableSubject_Subjects");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("Class");

                entity.Property(e => e.ClassCode).HasMaxLength(50);

                entity.Property(e => e.Slot)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.RegisterSubject)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.RegisterSubjectId)
                    .HasConstraintName("FK_Class_RegisterSubjects");
            });

            modelBuilder.Entity<ClassAsubject>(entity =>
            {
                entity.HasKey(e => new { e.ClassId, e.AsubjectId });

                entity.ToTable("Class_ASubject");

                entity.Property(e => e.AsubjectId).HasColumnName("ASubjectId");

                entity.Property(e => e.SubjectName).HasMaxLength(50);

                entity.HasOne(d => d.Asubject)
                    .WithMany(p => p.ClassAsubjects)
                    .HasForeignKey(d => d.AsubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Class_ASubject_AvailableSubject");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.ClassAsubjects)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Class_ASubject_Class");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentId).HasColumnName("commentId");

                entity.Property(e => e.CommentContent).HasMaxLength(200);

                entity.Property(e => e.LeaderId).HasColumnName("leaderId");

                entity.HasOne(d => d.ExamPaper)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ExamPaperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comments_ExamPaper");
            });

            modelBuilder.Entity<CurrentHeader>(entity =>
            {
                entity.HasKey(e => e.HeaderId)
                    .HasName("PK_Headers");

                entity.Property(e => e.HeaderId).HasColumnName("headerId");

                entity.Property(e => e.DepartmentId).HasColumnName("departmentId");

                entity.Property(e => e.SemesterId).HasColumnName("semesterId");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.CurrentHeaders)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CurrentHeaders_Departments");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.CurrentHeaders)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CurrentHeaders_Semester");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.DepartmentId).HasColumnName("departmentId");

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(50)
                    .HasColumnName("departmentName");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<ExamPaper>(entity =>
            {
                entity.ToTable("ExamPaper");

                entity.Property(e => e.ExamPaperId).HasColumnName("examPaperId");

                entity.Property(e => e.ExamContent)
                    .HasMaxLength(200)
                    .HasColumnName("examContent");

                entity.Property(e => e.ExamInstruction)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("examInstruction");

                entity.Property(e => e.ExamLink)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("examLink");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.HasOne(d => d.ExamSchedule)
                    .WithMany(p => p.ExamPapers)
                    .HasForeignKey(d => d.ExamScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamPaper_ExamSchedule");
            });

            modelBuilder.Entity<ExamSchedule>(entity =>
            {
                entity.ToTable("ExamSchedule");

                entity.Property(e => e.ExamScheduleId).HasColumnName("examScheduleId");

                entity.Property(e => e.Deadline)
                    .HasColumnType("date")
                    .HasColumnName("deadline");

                entity.Property(e => e.ExamLink)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.RegisterSubjectId).HasColumnName("registerSubjectId");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Tittle)
                    .HasMaxLength(100)
                    .HasColumnName("tittle");

                entity.Property(e => e.TypeId).HasColumnName("typeId");

                entity.HasOne(d => d.RegisterSubject)
                    .WithMany(p => p.ExamSchedules)
                    .HasForeignKey(d => d.RegisterSubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamSchedule_RegisterSubjects");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotiId);

                entity.ToTable("Notification");

                entity.Property(e => e.NotiId).HasColumnName("NotiID");

                entity.Property(e => e.Message).HasMaxLength(1000);

                entity.Property(e => e.Sender).HasMaxLength(100);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.SubjectCode).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_Users");
            });

            modelBuilder.Entity<RegisterSlot>(entity =>
            {
                entity.ToTable("RegisterSlot");

                entity.Property(e => e.Slot)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.RegisterSlots)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisterSlot_Semester");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RegisterSlots)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisterSlot_Users");
            });

            modelBuilder.Entity<RegisterSubject>(entity =>
            {
                entity.Property(e => e.RegisterSubjectId).HasColumnName("registerSubjectId");

                entity.Property(e => e.ClassId).HasColumnName("classId");

                entity.Property(e => e.IsRegistered).HasColumnName("isRegistered");

                entity.Property(e => e.RegisterDate)
                    .HasColumnType("date")
                    .HasColumnName("registerDate");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.AvailableSubject)
                    .WithMany(p => p.RegisterSubjects)
                    .HasForeignKey(d => d.AvailableSubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisterSubjects_AvailableSubject");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RegisterSubjects)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegisterSubjects_Users");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .HasColumnName("roleName");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.Property(e => e.ScheduleDate).HasColumnType("date");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Schedule_Class");
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.ToTable("Semester");

                entity.Property(e => e.SemesterId).HasColumnName("semesterId");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("startDate");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.Property(e => e.SubjectId).HasColumnName("subjectId");

                entity.Property(e => e.DepartmentId).HasColumnName("departmentId");

                entity.Property(e => e.ExamId).HasColumnName("examId");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.SubjectCode)
                    .HasMaxLength(50)
                    .HasColumnName("subjectCode");

                entity.Property(e => e.SubjectName)
                    .HasMaxLength(50)
                    .HasColumnName("subjectName");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Subjects_Departments");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Subjects_Types");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.Property(e => e.TypeId).HasColumnName("typeId");

                entity.Property(e => e.TypeName)
                    .HasMaxLength(50)
                    .HasColumnName("typeName");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasColumnName("address");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.FullName)
                    .HasMaxLength(50)
                    .HasColumnName("fullName");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userCode");

                entity.Property(e => e.UserCodeMustEliminate)
                    .HasMaxLength(50)
                    .HasColumnName("userCode-mustEliminate");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Roles");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
