using Microsoft.EntityFrameworkCore;
using MvcNormcontrol.Models;

namespace MvcNormcontrol.Data
{
    public class MvcNormcontrolContext : DbContext
    {
        public MvcNormcontrolContext(DbContextOptions<MvcNormcontrolContext> options)
            : base(options) { }

        public DbSet<MvcNormcontrol.Models.Student> Student { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Student>().ToTable("Student");
        //    modelBuilder.Entity<Student>().HasKey(c => new { c.Lastname, c.Name, c.Group });
        //}
    }
}
/*Представленный выше код создает свойство DbSet для набора сущностей.
 * В терминологии Entity Framework набор сущностей обычно соответствует
 * таблице базы данных, а сущность — строке в этой таблице.*/
