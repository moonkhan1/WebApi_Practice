using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi_Practice.Data.Entities
{
    [Table("tblCourse")]
    public class Course : BaseEntity<int>
    {
        [Key]
        public override int Id { get; set; }
        [MaxLength(20)]
        [Required]
        public string Name { get; set; }
        [Column("CreatedDate", TypeName = "datetime")]
        public DateTime? CreationTime { get; set; }
        [NotMapped]
        public ICollection<StudentCourses> StudentCourses { get; set; }
    }
}
