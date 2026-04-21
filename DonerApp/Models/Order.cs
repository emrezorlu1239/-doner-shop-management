using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonerApp.Models
{
    [Table("order")]
    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("table_id")]
        public int? TableId { get; set; }

        [Column("employee_id")]
        public int? EmployeeId { get; set; }

        [Column("opened_at")]
        public DateTime OpenedAt { get; set; } = DateTime.Now;

        [Column("closed_at")]
        public DateTime? ClosedAt { get; set; }

        [Column("status")]
        public string Status { get; set; } = "open";

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        public RestaurantTable? Table { get; set; }
        public Employee? Employee { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
    }
}