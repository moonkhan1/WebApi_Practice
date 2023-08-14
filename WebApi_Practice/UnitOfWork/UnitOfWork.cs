using WebApi_Practice.Data;
using WebApi_Practice.Data.Entities;
using WebApi_Practice.Repository;

namespace WebApi_Practice.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRepository<Student, int> StudentRepository { get; set; }
        public ICourseRepository CourseRepository { get; set; }

        private readonly StudentDBContext _studentDBContext;

        public UnitOfWork(StudentDBContext studentDBContext)
        {
            _studentDBContext = studentDBContext;
            StudentRepository = new EfRepository<Student, int>(studentDBContext);
            CourseRepository = new CourseRepository(studentDBContext);
        }
        public async Task Commit()
        {
            await _studentDBContext.SaveChangesAsync();
        }        
    }
}
