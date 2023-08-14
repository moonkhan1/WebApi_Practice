using WebApi_Practice.Data.Entities;
using WebApi_Practice.Repository;

namespace WebApi_Practice.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IRepository<Student, int> StudentRepository { get; set; }
        public ICourseRepository CourseRepository { get; set; }

        public Task Commit();
    }
}
