using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcNormcontrol.Data;
using MvcNormcontrol.Models;

namespace MvcNormcontrol.Controllers
{
    public class StudentsController : Controller
    {
        private readonly MvcNormcontrolContext _context;
        private readonly IWebHostEnvironment hostingEnvironment;

        public StudentsController(MvcNormcontrolContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Students
        public async Task<IActionResult> Index(string studentGroup, string searchString)
        {
            IQueryable<string> groupQuery = from m in _context.Student
                                            orderby m.Group
                                            select m.Group;
            var students = from m in _context.Student
                           select m;
            if (!string.IsNullOrEmpty(searchString))
                students = students.Where(s => s.Lastname.Contains(searchString));
            if (!string.IsNullOrEmpty(studentGroup))
                students = students.Where(x => x.Group == studentGroup);
            var studentGroupVM = new StudentGroupViewModel
            {
                Groups = new SelectList(await groupQuery.Distinct().ToListAsync()),
                Students = await students.ToListAsync()
            };
            return View(studentGroupVM);
        }

        public IActionResult GetFile(string fileName, string shortName)
        {
            if (fileName == null)
                return Content("Файл не загружен");
            string file_path = Path.Combine(hostingEnvironment.WebRootPath, "Documents", fileName);
            string file_type = "application/vnd.ms-word";
            return PhysicalFile(file_path, file_type, shortName);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.ID == id);
            var studentDetails = new StudentDetailsViewModel(student);
            if (student == null)
            {
                return NotFound();
            }

            return View(studentDetails);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateViewModel student)
        {
            if (ModelState.IsValid)
            {
                Student newStudent = Student.CreateNewStudent(student);
                Algorithm.UploadDownloadFile.UploadFile(student, newStudent,hostingEnvironment);
                Algorithm.UploadDownloadFile.Normcontrol(newStudent,hostingEnvironment);
                _context.Add(newStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction("details", new { id = newStudent.ID });
            }
            return View();
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            var studentEditViewModel = StudentEditViewModel.CreateNewStudent(student);
            if (student == null)
            {
                return NotFound();
            }
            return View(studentEditViewModel);
        }

        

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentEditViewModel student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var updateStudent = await _context.Student.FindAsync(student.ID);
                    updateStudent = Student.UpdateStudent(updateStudent, student);
                    if (student.Document != null)
                    {
                        Algorithm.UploadDownloadFile.DeleteFile(student,hostingEnvironment);
                        Algorithm.UploadDownloadFile.UploadFile(student, updateStudent,hostingEnvironment);
                        Algorithm.UploadDownloadFile.Normcontrol(updateStudent, hostingEnvironment);
                    }
                    _context.Update(updateStudent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("details", new { id = student.ID });
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            Algorithm.UploadDownloadFile.DeleteFile(student,hostingEnvironment);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.ID == id);
        }
    }
}
