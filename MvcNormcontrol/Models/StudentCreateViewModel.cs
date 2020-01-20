using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MvcNormcontrol.Models
{
    public class StudentCreateViewModel
    {
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

        public IFormFile Document { get; set; }
    }
}
