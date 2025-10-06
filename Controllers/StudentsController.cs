using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
using DotNetCoreSqlDb.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreSqlDb.Controllers
{
    public class StudentsController : Controller
    {
        private readonly MyDatabaseContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ICvTextExtractor _cvTextExtractor;
        private readonly ISkillExtractor _skillExtractor;

        public StudentsController(MyDatabaseContext db, IWebHostEnvironment env, ICvTextExtractor cvTextExtractor, ISkillExtractor skillExtractor)
        {
            _db = db;
            _env = env;
            _cvTextExtractor = cvTextExtractor;
            _skillExtractor = skillExtractor;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _db.Students.AsNoTracking().OrderByDescending(s => s.UploadedAtUtc).ToListAsync();
            return View(students);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, string fullName, string? email)
        {
            if (file == null || file.Length == 0 || string.IsNullOrWhiteSpace(fullName))
            {
                ModelState.AddModelError(string.Empty, "Full name and a file are required.");
                return View();
            }

            // Ensure uploads directory
            var uploadsDir = System.IO.Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            if (!System.IO.Directory.Exists(uploadsDir))
            {
                System.IO.Directory.CreateDirectory(uploadsDir);
            }

            var safeFileName = $"{Guid.NewGuid()}_{System.IO.Path.GetFileName(file.FileName)}";
            var savePath = System.IO.Path.Combine(uploadsDir, safeFileName);
            await using (var stream = System.IO.File.Create(savePath))
            {
                await file.CopyToAsync(stream);
            }

            var extractedText = await _cvTextExtractor.ExtractTextAsync(file);
            var skills = await _skillExtractor.ExtractSkillsAsync(extractedText);
            var skillsJson = System.Text.Json.JsonSerializer.Serialize(skills);

            var student = new Student
            {
                FullName = fullName.Trim(),
                Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                UploadedFileName = $"/uploads/{safeFileName}",
                UploadedContentType = file.ContentType,
                ExtractedText = extractedText,
                ExtractedSkillsJson = skillsJson,
                UploadedAtUtc = DateTime.UtcNow
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = student.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null) return NotFound();
            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
