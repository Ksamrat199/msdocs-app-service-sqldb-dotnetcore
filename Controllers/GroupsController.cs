using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreSqlDb.Controllers
{
    public class GroupsController : Controller
    {
        private readonly MyDatabaseContext _db;

        public GroupsController(MyDatabaseContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var groups = await _db.Groups.Include(g => g.Members).ThenInclude(m => m.Student).AsNoTracking().ToListAsync();
            return View(groups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutoGroup(int groupSize = 4)
        {
            if (groupSize < 2) groupSize = 2;

            var students = await _db.Students.AsNoTracking().ToListAsync();
            if (students.Count == 0)
            {
                TempData["Message"] = "No students to group.";
                return RedirectToAction(nameof(Index));
            }

            // Simple greedy diversity grouping by skills
            var unassigned = new HashSet<int>(students.Select(s => s.Id));
            var newGroups = new List<Group>();

            while (unassigned.Count > 0)
            {
                var group = new Group
                {
                    Name = $"Group {newGroups.Count + 1}",
                    Description = $"Auto-generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC"
                };

                // Seed with the student with most skills remaining
                var seedId = students
                    .Where(s => unassigned.Contains(s.Id))
                    .OrderByDescending(s => s.ExtractedSkills.Count)
                    .Select(s => s.Id)
                    .First();

                AddMember(group, students.First(s => s.Id == seedId));
                unassigned.Remove(seedId);

                while (group.Members.Count < groupSize && unassigned.Count > 0)
                {
                    var currentSkills = new HashSet<string>(group.Members
                        .SelectMany(m => m.Student.ExtractedSkills), StringComparer.OrdinalIgnoreCase);

                    var nextCandidate = students
                        .Where(s => unassigned.Contains(s.Id))
                        .Select(s => new
                        {
                            Student = s,
                            NewSkillCount = s.ExtractedSkills.Where(skill => !currentSkills.Contains(skill)).Count()
                        })
                        .OrderByDescending(x => x.NewSkillCount)
                        .ThenByDescending(x => x.Student.ExtractedSkills.Count)
                        .First().Student;

                    AddMember(group, nextCandidate);
                    unassigned.Remove(nextCandidate.Id);
                }

                // Compute aggregated skills
                var aggregated = group.Members.SelectMany(m => m.Student.ExtractedSkills)
                    .GroupBy(s => s, StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .ToList();
                group.AggregatedSkillsJson = JsonSerializer.Serialize(aggregated);

                newGroups.Add(group);
            }

            _db.Groups.AddRange(newGroups);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private static void AddMember(Group group, Student student)
        {
            group.Members.Add(new GroupMember
            {
                Student = student
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();
            _db.Groups.Remove(group);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
