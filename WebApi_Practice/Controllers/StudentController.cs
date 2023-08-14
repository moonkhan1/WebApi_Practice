using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using WebApi_Practice.Data.Entities;
using WebApi_Practice.Helpers;
using WebApi_Practice.Models;
using WebApi_Practice.UnitOfWork;

namespace WebApi_Practice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin"] // Class-daki butun metodlara Authorization teleb olunacaq
    public class StudentController : Controller
    {
        private static List<StudentModel> _students = new ();

        private readonly ISingletonOperation _singletonOperation;
        private readonly IScopedOperation _scopedOperation;
        private readonly ITransientOperation _transientOperation;
        private readonly IConfiguration _configuration;
        //private readonly IStudentRepository _studentRepository;
        private readonly ILogger<StudentController> _logger;

        /* REPOSITORY PATTERN */
        //private readonly IRepository<Student, int> _studentRepository;
        //private readonly IRepository<Course, int> _courseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StudentController(ISingletonOperation singletonOperation,
            ITransientOperation transientOperation,
            IScopedOperation scopedOperation,
            IConfiguration configuration,
            //IStudentRepository studentRepository
            //IRepository<Student, int> studentRepository,
            //IRepository<Course, int> courseRepository
            IUnitOfWork unitOfWork,
            ILogger<StudentController> logger)
        {
            _singletonOperation = singletonOperation;
            _transientOperation = transientOperation;
            _scopedOperation = scopedOperation;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _logger = logger;
            //_studentRepository = studentRepository;
            //_courseRepository = courseRepository;
        }

        [HttpGet("Guid")]
        //[Authorize(Roles = "Admin, User")] // Ikisinden biri olmalidi
        /* Her ikisi olmalidi
        [Authorize(Roles = "Admin"]
        [Authorize(Roles = "User"]
        */
        [Authorize("AdminOnly")] // Policy
        public object GetGuids()
        {
            var user = HttpContext.User; 
            var isInAdminRole = user.IsInRole("Admin");
            var data = new
            {
                Singleton = _singletonOperation.Id,
                Transient = _transientOperation.Id,
                Scoped = _scopedOperation.Id
            };
            return data;
        }

        [HttpGet("all")]
        [MyAuthorize]
        public async Task<object> GetAll()
        {
            var user = HttpContext.User;

            _logger.LogInformation("Request succesfully accepted at {date}", DateTime.Now);
            var query = await _unitOfWork.StudentRepository.GetAllList();

            var result =  query.ToList();
            _logger.LogWarning("Request succesfully completed at {date}, result: {result}", DateTime.Now, JsonConvert.SerializeObject(result));
            
            return result;

        }

        //[HttpGet("studentCoursesReport")]
        //public async Task<object> GetStudentCourseReport()
        //{
        //    /*var query = from sc in _studentDbContext.StudentCourses
        //                join s in _studentDbContext.Students on sc.StudentId equals s.Id
        //                join c in _studentDbContext.Courses on sc.CourseId equals c.Id
        //                join g in _studentDbContext.Genders on s.GenderId equals g.Id
        //                select new
        //                {
        //                    s.Name,
        //                    s.Surname,
        //                    s.DateOfBirth,
        //                    GenderName = g.Name,
        //                    CourseName = c.Name,
        //                    sc.StartDate,
        //                    sc.EndDate
        //                };
        //    */
        //    //await _studentDbContext.Database.ExecuteSqlRawAsync("Insert into tblGender Values (3, 'Unknown')"); 

        //    // var sqlQuery = query.ToQueryString();// Giving SQL Query to Entity Framework to Run 

        //    var query = _studentDbContext.StudentCourseQueries.FromSqlRaw(
        //        @"Select s.Name, s.Surname, s.DateOfBirth, c.Name CourseName, sc.StartDate, sc.EndDate from tblStudentCourses sc
        //        join tblStudent s on sc.StudentId = s.Id
        //        join tblCourse c on sc.CourseId = c.Id");
        //    return await query.ToListAsync();
        //}
       
        //[HttpGet("genders")]
        //public async Task<object> GetGenders()
        //{
        //    var genders = await _studentDbContext.Genders.Include(g => g.Students)
        //        .Select(g => new
        //        {
        //            g.Name,
        //            Students = g.Students.Select(s => new
        //            {
        //                s.Name,
        //                s.Surname,
        //                s.Salary,
        //                s.DateOfBirth,
        //                GenderName = s.Gender.Name
        //            })

        //        }).ToListAsync();
        //    return genders;
        //}

        [HttpGet("student/{id}")]
        [AllowAnonymous] // Authorizationu ignore edir bu metod ucun
        public async Task<IActionResult?> GetStudent(int id, string name, int age) 
        {
            var student = await _unitOfWork.StudentRepository.Find(id);
            if(student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentModel studentModel, [FromServices]IOptions<ApiBehaviorOptions> apiBehaviour)
        {
            if (ModelState.IsValid)
            {
                var student = new Student()
                {
                    DateOfBirth = studentModel.DateOfBirth,
                    Name = studentModel.Name,
                    Surname = studentModel.Surname,
                    Salary = studentModel.Salary,
                    GenderId = studentModel.GenderId
                };
                var newStudentId = await _unitOfWork.StudentRepository.Find(student.Id);

                await _unitOfWork.StudentRepository.Add(student);
                await _unitOfWork.Commit();
                return Created($"/api/student/student/{student.Id}", student);
            }
            return apiBehaviour.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        [HttpPut("update")]
        public async Task<Student?> UpdateStudent(int id, Student student)
        {
            var tempStudent = await _unitOfWork.StudentRepository.Find(id);
            if (tempStudent != null)
            {
                tempStudent.Name = student.Name;
                tempStudent.Salary = student.Salary;

                await _unitOfWork.StudentRepository.Update(tempStudent);
                await _unitOfWork.Commit();
                return tempStudent;
            }
            return null;
        }
        [HttpDelete("delete")]
        public async Task<Student?> DeleteStudent(int id)
        {
            try
            {
                _logger.LogInformation("Request accepted to delete student with id: {id}", id);
                var student = await _unitOfWork.StudentRepository.Find(id);
                _logger.LogDebug("Student is fetched from database successfully", id);

                await _unitOfWork.StudentRepository.Delete(student);
                await _unitOfWork.Commit();

                _logger.LogDebug("Student is deleted from db and transaction committed {id}", id);
                _logger.LogInformation("Request is succcesfully complited to delete student with id: {id}", id);
                return student;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Exception occured while deleting student with id: {id}", id);
                throw;
            }
            //var student = await _unitOfWork.StudentRepository.Find(id);

            //if (student != null)
            //{
            //    await _unitOfWork.StudentRepository.Delete(student);
            //    await _unitOfWork.Commit();
            //    return student;
            //}
            //return null;
        }
    }
}
