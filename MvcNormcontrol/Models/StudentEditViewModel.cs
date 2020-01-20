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
    }
}
