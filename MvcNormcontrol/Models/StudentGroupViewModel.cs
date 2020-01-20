using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MvcNormcontrol.Models
{
    public class StudentGroupViewModel
    {
        public List<Student> Students { get; set; }
        public SelectList Groups { get; set; }
        public string StudentGroup { get; set; }
        public string SearchString { get; set; }
    }
}
