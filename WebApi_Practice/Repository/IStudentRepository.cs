using WebApi_Practice.Data.Entities;

namespace WebApi_Practice.Repository
{
    public interface IStudentRepository
    {
        Task<Student> Get(int id);

        Task<IEnumerable<Student>> GetAll();

        Task<Student> Add(Student entity);

        Task<Student> Delete(int id);

        Task<Student> Update(Student studentEntity);
    }
}
