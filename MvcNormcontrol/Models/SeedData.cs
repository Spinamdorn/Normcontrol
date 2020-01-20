using System;
using MvcNormcontrol.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace MvcNormcontrol.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MvcNormcontrolContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcNormcontrolContext>>()))
            {
                //Look for any students
                if (context.Student.Any())
                {
                    return;
                }

                context.Student.AddRange(
                    new Student
                    {
                        Lastname = "Петров",
                        Name = "Сергей",
                        Patronymic = "Александрович",
                        Group = "РИ-370010",
                        Discipline = "Математика",
                        CompletionDate=new DateTime(2020,1,1)
                    },
                    new Student
                    {
                        Lastname = "Иванова",
                        Name = "Ольга",
                        Patronymic = "Дмитриевна",
                        Group = "РИ-370010",
                        Discipline = "Математика",
                        CompletionDate = new DateTime(2020, 1, 1)
                    },
                    new Student
                    {
                        Lastname = "Васильев",
                        Name = "Олег",
                        Patronymic = "Сергеевич",
                        Group = "РИ-370011",
                        Discipline = "Математика",
                        CompletionDate = new DateTime(2020, 1, 1)
                    },
                    new Student
                    {
                        Lastname = "Пушкина",
                        Name = "Александра",
                        Patronymic = "Сергеевна",
                        Group = "РИ-370011",
                        Discipline = "Математика",
                        CompletionDate = new DateTime(2020, 1, 1)
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
