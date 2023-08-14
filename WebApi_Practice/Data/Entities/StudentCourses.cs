using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi_Practice.Data.Entities
{
    public class StudentCourses
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }

    }
}
