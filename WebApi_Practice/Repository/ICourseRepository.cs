using Microsoft.EntityFrameworkCore;
using WebApi_Practice.Data;
using WebApi_Practice.Data.Entities;

namespace WebApi_Practice.Repository
{
    public interface ICourseRepository : IRepository<Course, int>
    {
        Task<Course> FindByName(string name);
    }

    public class CourseRepository : EfRepository<Course, int>, ICourseRepository
    {
        private readonly StudentDBContext _studentDBContext;

        public CourseRepository(StudentDBContext studentDBContext) : base(studentDBContext)
        {
            _studentDBContext = studentDBContext;            
        }

        public async Task<Course> FindByName(string name)
        {
            var course = await _studentDBContext.Set<Course>()
                .Where(c => c.Name.Contains(name))
                .FirstOrDefaultAsync();
            return course;
        }
    }
}
