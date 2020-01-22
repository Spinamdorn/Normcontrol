using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MvcNormcontrol.Algorithm;

namespace MvcNormcontrol.Models
{
    public class StudentDetailsViewModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Фамилия")]
        public string Lastname { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Группа")]
        public string Group { get; set; }

        [Display(Name = "Дисциплина")]
        public string Discipline { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата сдачи")]
        public DateTime CompletionDate { get; set; }

        [DisplayFormat(NullDisplayText = "Нет результата")]
        [Display(Name = "Результат проверки")]
        public Student.Status? ReportStatus { get; set; }

        public List<string> ErrorList { get; set; }

        public string UniqueDocName { get; set; }
        [Display(Name = "Имя файла")]
        public string DocName { get; set; }

        public StudentDetailsViewModel(Student student)
        {
            ID = student.ID;
            Lastname = student.Lastname;
            Name = student.Name;
            Patronymic = student.Patronymic;
            Group = student.Group;
            Discipline = student.Discipline;
            CompletionDate = student.CompletionDate;
            ReportStatus = student.ReportStatus;
            if(student.ErrorList!=null)
                ErrorList = WordDocument.ErrorsForOut(student.ErrorList);
            UniqueDocName = student.UniqueDocName;
            DocName = student.DocName;
        }
    }
}
