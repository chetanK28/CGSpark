using System.ComponentModel.DataAnnotations;

namespace CGSpark.Data.Models
{
    public class Submission
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        public string? Title { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public bool IsFixed { get; set; }

        public string? FilePath { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public string? Tag { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; } // ❗ Made nullable to prevent circular insert attempts
    }
}
