using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  // ✅ Enables .Include()
using CGSpark.Data;
using CGSpark.Data.Models;


[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Submissions(string type = null, string status = null)
    {
        var submissions = _context.Submissions.Include(s => s.User).AsQueryable();

        if (!string.IsNullOrEmpty(type))
            submissions = submissions.Where(s => s.Type == type);

        if (!string.IsNullOrEmpty(status))
            submissions = submissions.Where(s => s.Status == status);

        return View(submissions.ToList());
    }

    [HttpPost]
    public IActionResult UpdateStatus(int id, string action)
    {
        var submission = _context.Submissions.Find(id);
        if (submission != null)
        {
            submission.Status = action;
            _context.SaveChanges();
        }
        return RedirectToAction("Submissions");
    }

    [HttpPost]
    public IActionResult AddTag(int id, string tag)
    {
        var submission = _context.Submissions.Find(id);
        if (submission != null)
        {
            submission.Tag = tag;
            _context.SaveChanges();
        }
        return RedirectToAction("Submissions");
    }

    [HttpGet]
    public IActionResult ApiGetSubmissions(string type = null, string status = null)
    {
        var submissions = _context.Submissions.Include(s => s.User).AsQueryable();
        if (!string.IsNullOrEmpty(type)) submissions = submissions.Where(s => s.Type == type);
        if (!string.IsNullOrEmpty(status)) submissions = submissions.Where(s => s.Status == status);
        return Json(submissions.ToList());
    }
}
