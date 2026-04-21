using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonerApp.Models
{
    [Table("employee")]
    public class Employee
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("full_name")]
        public string FullName { get; set; } = null!;

        [Column("role")]
        public string Role { get; set; } = null!;

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("hired_at")]
        public DateOnly? HiredAt { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}