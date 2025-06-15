using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Security.Claims;
using CGSpark.Data;
using CGSpark.Data.Models;

[Authorize]
public class SubmissionController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SubmissionController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpGet]
    public IActionResult SubmitAchievement() => View();

    [HttpPost]
    public async Task<IActionResult> SubmitAchievement(string description, IFormFile file)
    {
        var path = await SaveFile(file);
        if (path == null) return BadRequest("Invalid file.");

        var submission = new Submission
        {
            Type = "Achievement",
            Description = description,
            FilePath = path,
            UserId = GetOrCreateUserId(),
            Status = "Pending",
            SubmittedAt = DateTime.UtcNow
        };
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult SubmitCertification() => View();

    [HttpPost]
    public async Task<IActionResult> SubmitCertification(string description, IFormFile file)
    {
        var path = await SaveFile(file);
        if (path == null) return BadRequest("Invalid file.");

        var submission = new Submission
        {
            Type = "Certification",
            Description = description,
            FilePath = path,
            UserId = GetOrCreateUserId(),
            Status = "Pending",
            SubmittedAt = DateTime.UtcNow
        };
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult SubmitBug() => View();

    [HttpPost]
    public async Task<IActionResult> SubmitBug(string title, string description, bool isFixed)
    {
        var submission = new Submission
        {
            Type = "Bug",
            Title = title,
            Description = description,
            IsFixed = isFixed,
            UserId = GetOrCreateUserId(),
            Status = "Pending",
            SubmittedAt = DateTime.UtcNow
        };
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult MySubmissions()
    {
        var email = User.FindFirst("emails")?.Value;
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
            return RedirectToAction("Index", "Home");

        var submissions = _context.Submissions
            .Where(s => s.UserId == user.Id)
            .OrderByDescending(s => s.SubmittedAt)
            .ToList();

        return View(submissions);
    }

    [HttpGet]
    public IActionResult ViewCertificate(string filename)
    {
        if (string.IsNullOrEmpty(filename)) return NotFound();
        var path = Path.Combine(_env.WebRootPath, "uploads", filename);
        if (!System.IO.File.Exists(path)) return NotFound();

        var contentType = "application/octet-stream";
        return PhysicalFile(path, contentType, filename);
    }

    private async Task<string> SaveFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension) || file.Length > 2 * 1024 * 1024)
            return null;

        var uploads = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploads);

        var fileName = Guid.NewGuid().ToString() + extension;
        var filePath = Path.Combine(uploads, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }

    private int GetOrCreateUserId()
    {
        var email = User.FindFirst("emails")?.Value;
        var name = User.FindFirst("name")?.Value;

        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            user = new User
            {
                Name = name ?? "Unknown",
                Email = email ?? "noemail@unknown.com",
                Role = "Employee"
            };
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        return user.Id;
    }
}
