using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MvcNormcontrol.Models
{
    public class StudentEditViewModel : StudentCreateViewModel
    {
        public int ID { get; set; }
        public string ExistingDocName { get; set; }
        public string ExistingUniqueDocName { get; set; }

        public static StudentEditViewModel CreateNewStudent(Student student)
        {
            return new StudentEditViewModel
            {
                ID = student.ID,
                Lastname = student.Lastname,
                Name = student.Name,
                Patronymic = student.Patronymic,
                Group = student.Group,
                Discipline = student.Discipline,
                ExistingDocName = student.DocName,
                ExistingUniqueDocName = student.UniqueDocName,
            };
        }
    }
}
