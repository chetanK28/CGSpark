using CGSpark.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var email = User.FindFirst("emails")?.Value;
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        ViewBag.Name = user?.Name ?? "Unknown";

        return View();
    }

    [HttpGet]
    public IActionResult GetDashboardStats()
    {
        var data = new
        {
            achievements = _context.Submissions.Count(s => s.Type == "Achievement"),
            certifications = _context.Submissions.Count(s => s.Type == "Certification"),
            bugsFixed = _context.Submissions.Count(s => s.Type == "Bug" && s.IsFixed)
        };
        return Json(data);
    }
}
