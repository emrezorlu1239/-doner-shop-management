using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonerApp.Models
{
    [Table("product_ingredient")]
    public class ProductIngredient
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("ingredient_id")]
        public int IngredientId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("unit")]
        public string Unit { get; set; } = null!;

        public Product Product { get; set; } = null!;
        public Ingredient Ingredient { get; set; } = null!;
    }
}