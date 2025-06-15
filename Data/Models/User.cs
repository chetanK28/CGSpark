using System.ComponentModel.DataAnnotations;
using CGSpark.Data.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string Role { get; set; } = "Employee"; // Default role

    public ICollection<Submission> Submissions { get; set; }
}
