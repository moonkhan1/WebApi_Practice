using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using WebApi_Practice.Data.DbQueries;
using WebApi_Practice.Data.Entities;

namespace WebApi_Practice.Data
{
    public class StudentDBContext : DbContext
    {
        private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo =
            typeof(StudentDBContext).GetMethod(nameof(ConfigureGlobalFilters),
                BindingFlags.Instance | BindingFlags.NonPublic);
        public StudentDBContext(DbContextOptions<StudentDBContext> options) : base(options)   
        {
            
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<StudentCourses> StudentCourses { get; set; }
        public DbSet<StudentCourseQuery> StudentCourseQueries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Course>().ToTable("tblCourse");
            modelBuilder.Entity<Course>().Property(x => x.CreationTime).HasDefaultValueSql("Getdate()");
            modelBuilder.Entity<Gender>().ToTable("tblGender");
            modelBuilder.Entity<StudentCourses>().ToTable("tblStudentCourses");

            // Fluent API always work before EF Data Annotation (Temiz shekilde yazmaq istesek her birine ayri configuration classi acmaliyiq(Ex.StudentConfigurations))
            /*
             modelBuilder.Entity<Student>(s =>
            {
                s.HasKey(k => k.Id);
                s.ToTable("tblStudent");
                s.Property(p => p.Surname).HasMaxLength(50);
                s.Property(p => p.Surname).HasMaxLength(25);
                s.HasIndex(i => i.Name).IsUnique().HasDatabaseName("UIX_tblStudent_Name");
                s.HasOne(x => x.Gender).WithMany(x => x.Students).HasForeignKey(x => x.GenderId)
                .HasConstraintName("FK_tblStudent_tblGender_GenderId");
            });
            */ // Eger ayri class-a kocursek asagidaki kimi Apply Configurations etmeliyik
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            #region DbQuery configs

            modelBuilder.Entity<StudentCourseQuery>().HasNoKey();

            #endregion

            modelBuilder.Entity<StudentCourses>().HasKey(x => new { x.StudentId, x.CourseId });

            #region global configs
            // hard code method
            //ConfigureGlobalFilters<Student>(modelBuilder);
            //ConfigureGlobalFilters<Course>(modelBuilder);
            //ConfigureGlobalFilters<Gender>(modelBuilder);
            //ConfigureGlobalFilters<StudentCourses>(modelBuilder);

            // non-hard code method
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo
                   .MakeGenericMethod(entityType.ClrType)
                   .Invoke(this, new object[] { modelBuilder });
            }
            #endregion

        }
        private void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder) where TEntity : class
        {
            if(ShouldFilterEntity<TEntity>())
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if(filterExpression != null)
                {
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                }
            }
        }

        protected virtual bool ShouldFilterEntity<TEntity>() where TEntity : class
        {
            if(typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }
            return false;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>() where TEntity : class
        {
            Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete)e).IsDelete;
            return softDeleteFilter;
        }
    }
}
