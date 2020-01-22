using Microsoft.EntityFrameworkCore;
using MvcNormcontrol.Models;

namespace MvcNormcontrol.Data
{
    public class MvcNormcontrolContext : DbContext
    {
        public MvcNormcontrolContext(DbContextOptions<MvcNormcontrolContext> options)
            : base(options) { }

        public DbSet<Student> Student { get; set; }
    }
}
/*Представленный выше код создает свойство DbSet для набора сущностей.
 * В терминологии Entity Framework набор сущностей обычно соответствует
 * таблице базы данных, а сущность — строке в этой таблице.*/
