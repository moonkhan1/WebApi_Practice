using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace WebApi_Practice.Models
{
    public class StudentModel
    {
        public int Id { get; set; }

        //[Required]
        //[MaxLength(15)]
        public string Name { get; set; }

        //[Required]
        //[MaxLength(15)]
        public string Surname { get; set; }

        //[Range(100, 1000)]
        public double? Salary { get; set; }
        public DateTime DateOfBirth { get; set; }

        //[Required]
        //[Range(1, 3)]
        public int? GenderId { get; set; }
    }

    public class StudentValidator : AbstractValidator<StudentModel>
    {
        public StudentValidator()
        {
            RuleFor(x => x.Name).Length(1, 10);
            RuleFor(x => x.Surname).Length(1, 10);
            RuleFor(x => x.Salary).InclusiveBetween(100, 1000);
            RuleFor(x => x.GenderId).InclusiveBetween(1, 3);

        }
    }
}
