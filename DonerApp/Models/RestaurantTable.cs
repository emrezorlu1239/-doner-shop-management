using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonerApp.Models
{
    [Table("restaurant_table")]
    public class RestaurantTable
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("table_number")]
        public int TableNumber { get; set; }

        [Column("seat_capacity")]
        public int SeatCapacity { get; set; }

        [Column("status")]
        public string Status { get; set; } = "available";

        [Column("active_order_id")]
        public int? ActiveOrderId { get; set; }

        public Order? ActiveOrder { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}