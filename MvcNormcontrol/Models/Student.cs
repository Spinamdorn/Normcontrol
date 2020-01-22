using System;
using System.ComponentModel.DataAnnotations;

namespace MvcNormcontrol.Models
{
    public class Student
    {
        public enum Status {
            [Display(Name ="Сдано")]
            Passed,
            [Display(Name ="Не сдано")]
            Falled
        };

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
        public Status? ReportStatus { get; set; }

        public string ErrorList { get; set; }

        public string UniqueDocName { get; set; }

        [Display(Name = "Имя файла")]
        public string DocName { get; set; }
    }
}
