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
                var newStudent = new Student
                {
                    Name = student.Name,
                    Lastname = student.Lastname,
                    Patronymic = student.Patronymic,
                    Group = student.Group,
                    Discipline = student.Discipline,
                    CompletionDate = DateTime.Today
                };
                var nameAndPath = ProcessUploadedFile(student);
                newStudent.DocName = nameAndPath[0];
                newStudent.UniqueDocName = nameAndPath[1];
                _context.Add(newStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction("details", new { id = newStudent.ID });
            }
            return View();
        }

        private string[] ProcessUploadedFile(StudentCreateViewModel student)
        {
            string fileName = null;
            string uniqueFileName = null;
            if (student.Document != null)
            {
                fileName = student.Document.FileName;
                var uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Documents");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + student.Document.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                student.Document.CopyTo(fileStream);
            }
            return new string[] { fileName, uniqueFileName };
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            var studentEditViewModel = new StudentEditViewModel
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
                    updateStudent.Lastname = student.Lastname;
                    updateStudent.Name = student.Name;
                    updateStudent.Patronymic = student.Patronymic;
                    updateStudent.Group = student.Group;
                    updateStudent.Discipline = student.Discipline;
                    updateStudent.CompletionDate = DateTime.Today;
                    if (student.Document != null)
                    {
                        if (student.ExistingUniqueDocName != null)
                        {
                            string filePath = Path.Combine(hostingEnvironment.WebRootPath, "Documents", student.ExistingUniqueDocName);
                            System.IO.File.Delete(filePath);
                        }
                        var nameAndPath = ProcessUploadedFile(student);
                        updateStudent.DocName = nameAndPath[0];
                        updateStudent.UniqueDocName = nameAndPath[1];
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
            if (student.UniqueDocName != null)
            {
                var filePath = Path.Combine(hostingEnvironment.WebRootPath, "Documents", student.UniqueDocName);
                System.IO.File.Delete(filePath);
            }
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
