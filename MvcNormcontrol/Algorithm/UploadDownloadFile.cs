using System;
using System.IO;
using System.Net;
using MvcNormcontrol.Models;
using MvcNormcontrol.Controllers;
using Microsoft.AspNetCore.Hosting;

namespace MvcNormcontrol.Algorithm
{
    public class UploadDownloadFile
    {
       public static void UploadFile(StudentCreateViewModel student, Student newStudent, IWebHostEnvironment hostingEnvironment)
        {
            var nameAndPath = ProcessUploadedFile(student, hostingEnvironment);
            newStudent.DocName = nameAndPath[0];
            newStudent.UniqueDocName = nameAndPath[1];
        }

        private static string[] ProcessUploadedFile(StudentCreateViewModel student, IWebHostEnvironment hostingEnvironment)
        {
            string fileName = null;
            string uniqueFileName = null;
            if (student.Document != null)
            {
                fileName = WebUtility.HtmlEncode(student.Document.FileName);
                var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Documents");
                uniqueFileName = Path.GetRandomFileName();
                //uniqueFileName = Guid.NewGuid().ToString() + "_" + student.Document.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                student.Document.CopyTo(fileStream);
            }
            return new string[] { fileName, uniqueFileName };
        }

        public static void DeleteFile(StudentEditViewModel student, IWebHostEnvironment hostingEnvironment)
        {
            if (student.ExistingUniqueDocName != null)
            {
                string filePath = Path.Combine(hostingEnvironment.WebRootPath, "Documents", student.ExistingUniqueDocName);
                File.Delete(filePath);
            }
        }

        public static void DeleteFile(Student student, IWebHostEnvironment hostingEnvironment)
        {
            if (student.UniqueDocName != null)
            {
                string filePath = Path.Combine(hostingEnvironment.WebRootPath, "Documents", student.UniqueDocName);
                File.Delete(filePath);
            }
        }

        public static void Normcontrol(Student student, IWebHostEnvironment hostingEnvironment)
        {
            if (student.UniqueDocName != null)
            {
                var filePath = Path.Combine(hostingEnvironment.WebRootPath, "Documents", student.UniqueDocName);
                string errors = null;
                var result = WordDocument.WorkWithDocument(filePath, ref errors);
                if (result)
                    student.ReportStatus = Student.Status.Falled;
                else student.ReportStatus = Student.Status.Passed;
                student.ErrorList = errors;
            }
        }
    }
}
