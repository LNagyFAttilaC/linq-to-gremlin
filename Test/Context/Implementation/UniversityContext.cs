using LINQtoGremlin.Core.Extensions;
using LINQtoGremlin.Core.Graph;
using Microsoft.EntityFrameworkCore;
using Test.Model;

namespace Test.Context
{
    public class UniversityContext
        : GDbContext
    {
        #region Constructors

        public UniversityContext(
            string host = "localhost",
            int port = 8182,
            bool enableSSL = false,
            string username = null,
            string password = null)
            : base(
                  host,
                  port,
                  enableSSL,
                  username,
                  password)
        {

        }

        #endregion

        #region Properties

        public DbSet<Department> Departments
        {
            get;
            set;
        }

        public DbSet<Faculty> Faculties
        {
            get;
            set;
        }

        public DbSet<Person> People
        {
            get;
            set;
        }

        public DbSet<Subject> Subjects
        {
            get;
            set;
        }

        public DbSet<Teacher> Teachers
        {
            get;
            set;
        }

        public DbSet<University> Universities
        {
            get;
            set;
        }

        #endregion

        #region Methods

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            base
                .OnConfiguring(
                    optionsBuilder);

            optionsBuilder
                .GetGDbContextOptionsExtension()
                .Vertex(
                    new GDbModelDescriptor(
                        typeof(Department))
                        .SetLabel(
                            "Department")
                        .AddProperty(
                            nameof(Department.FoundedIn))
                        .AddProperty(
                            nameof(Department.Name))
                        .AddEdge(
                            nameof(Department.Leader))
                        .ToOne(
                            typeof(Teacher),
                            nameof(Teacher.LeaderAtDepartment))
                        .AddEdge(
                            nameof(Department.Subjects))
                        .ToMany(
                            typeof(Subject),
                            nameof(Subject.Department)))
                .Vertex(
                    new GDbModelDescriptor(
                        typeof(Faculty))
                        .SetLabel(
                            "Faculty")
                        .AddProperty(
                            nameof(Faculty.FoundedIn))
                        .AddProperty(
                            nameof(Faculty.Name))
                        .AddEdge(
                            nameof(Faculty.Dean))
                        .ToOne(
                            typeof(Teacher),
                            nameof(Teacher.DeanAtFaculty))
                        .AddEdge(
                            nameof(Faculty.Departments))
                        .ToMany(
                            typeof(Department),
                            nameof(Department.Faculty)))
                .Vertex(
                    new GDbModelDescriptor(
                        typeof(Person))
                        .SetLabel(
                            "Person")
                        .AddProperty(
                            nameof(Person.Age))
                        .AddProperty(
                            nameof(Person.Name))
                        .AddProperty(
                            nameof(Person.PlaceOfBirth)))
                .Vertex(
                    new GDbModelDescriptor(
                        typeof(Subject))
                        .SetLabel(
                            "Subject")
                        .AddProperty(
                            nameof(Subject.Name))
                        .AddEdge(
                            nameof(Subject.Teachers))
                        .ToMany(
                            typeof(Subject_Teacher),
                            nameof(Subject_Teacher.Subject)))
                .Vertex(
                    new GDbModelDescriptor(
                        typeof(Subject_Teacher))
                        .SetLabel(
                            "Subject_Teacher"))
                .Vertex(
                    new GDbModelDescriptor(
                        typeof(Teacher))
                        .SetLabel(
                            "Teacher")
                        .AddAncestor(
                            typeof(Person))
                        .AddProperty(
                            nameof(Teacher.TeacherCode))
                        .AddEdge(
                            nameof(Teacher.Subjects))
                        .ToMany(
                            typeof(Subject_Teacher),
                            nameof(Subject_Teacher.Teacher))
                        .AddEdge(
                            nameof(Teacher.University))
                        .ToOne(
                            typeof(University)))
                .Vertex(
                    new GDbModelDescriptor(
                        typeof(University))
                        .SetLabel(
                            "University")
                        .AddProperty(
                            nameof(University.FoundedIn))
                        .AddProperty(
                            nameof(University.Name))
                        .AddEdge(
                            nameof(University.Faculties))
                        .ToMany(
                            typeof(Faculty),
                            nameof(Faculty.University))
                        .AddEdge(
                            nameof(University.Rector))
                        .ToOne(
                            typeof(Teacher),
                            nameof(Teacher.RectorAtUniversity)));
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base
                .OnModelCreating(
                    modelBuilder);

            modelBuilder
                .Entity<Department>()
                .HasOne(
                    d => d.Leader)
                .WithOne(
                    t => t.LeaderAtDepartment)
                .HasForeignKey<Department>(
                    d => d.LeaderId)
                .IsRequired(false);

            modelBuilder
                .Entity<Faculty>()
                .HasOne(
                    f => f.Dean)
                .WithOne(
                    t => t.DeanAtFaculty)
                .HasForeignKey<Faculty>(
                    f => f.DeanId)
                .IsRequired(false);

            modelBuilder
                .Entity<University>()
                .HasOne(
                    u => u.Rector)
                .WithOne(
                    t => t.RectorAtUniversity)
                .HasForeignKey<University>(
                    u => u.RectorId)
                .IsRequired(false);
        }

        #endregion
    }
}
