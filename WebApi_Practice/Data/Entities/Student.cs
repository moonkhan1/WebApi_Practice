using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi_Practice.Data.Entities
{
    public class Student : BaseEntity<int>, ISoftDelete
    {
        [Key]
        public override int Id { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public double? Salary { get; set; }

        [ForeignKey(nameof(Gender))]
        public int? GenderId { get; set; }
        public Gender Gender { get; set; }

        public ICollection<StudentCourses> StudentCourses { get; set; }
        public bool IsDelete { get; set; }
    }
}
