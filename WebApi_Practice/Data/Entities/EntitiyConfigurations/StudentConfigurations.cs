using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace WebApi_Practice.Data.Entities.EntitiyConfigurations
{
    public class StudentConfigurations : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> modelBuilder)
        {
            modelBuilder.HasKey(k => k.Id);
            modelBuilder.ToTable("tblStudent");
            modelBuilder.Property(p => p.Surname).HasMaxLength(50);
            modelBuilder.Property(p => p.Surname).HasMaxLength(25);
            modelBuilder.HasIndex(i => i.Name).IsUnique().HasDatabaseName("UIX_tblStudent_Name");
            modelBuilder.HasOne(x => x.Gender).WithMany(x => x.Students).HasForeignKey(x => x.GenderId)
                .HasConstraintName("FK_tblStudent_tblGender_GenderId");

            modelBuilder.HasQueryFilter(s => !s.IsDelete); // Silinmis telebeleri getirme
        }
    }
}
