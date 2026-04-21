using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonerApp.Models
{
    [Table("stock_movement")]
    public class StockMovement
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("ingredient_id")]
        public int IngredientId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("movement_type")]
        public string MovementType { get; set; } = null!;

        [Column("moved_at")]
        public DateTime MovedAt { get; set; } = DateTime.Now;

        [Column("notes")]
        public string? Notes { get; set; }

        public Ingredient Ingredient { get; set; } = null!;
    }
}