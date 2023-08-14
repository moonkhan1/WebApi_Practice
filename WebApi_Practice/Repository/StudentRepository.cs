using Microsoft.EntityFrameworkCore;
using WebApi_Practice.Data;
using WebApi_Practice.Data.Entities;

namespace WebApi_Practice.Repository
{
    public class StudentRepository : IStudentRepository
    {

        private readonly StudentDBContext _context;

        public StudentRepository(StudentDBContext context)
        {
            _context = context;
        }
        public async Task<Student> Add(Student entity)
        {
            await _context.Students.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Student> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student> Get(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task<IEnumerable<Student>> GetAll()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student> Update(Student studentEntity)
        {
            _context.Entry(studentEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return studentEntity;

        }
    }
}
