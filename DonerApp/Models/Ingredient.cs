using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonerApp.Models
{
    [Table("ingredient")]
    public class Ingredient
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("unit")]
        public string Unit { get; set; } = null!;

        [Column("stock_quantity")]
        public decimal StockQuantity { get; set; }

        [Column("min_stock")]
        public decimal MinStock { get; set; }

        [Column("unit_price")]
        public decimal? UnitPrice { get; set; }

        [Column("supplier_id")]
        public int? SupplierId { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Supplier? Supplier { get; set; }
        public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}