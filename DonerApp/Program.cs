using DonerApp;
using DonerApp.Models;
using System.Windows.Forms;

namespace DonerApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using (var db = new AppDbContext())
            {
                // 1. ÇALIŞANLAR
                if (!db.Employees.Any())
                {
                    db.Employees.AddRange(
                        new Employee { FullName = "Mehmet Yılmaz", Role = "manager", Phone = "0532 111 22 33", IsActive = true, HiredAt = new DateOnly(2022, 1, 10), PasswordHash = LoginForm.HashPassword("1234") },
                        new Employee { FullName = "Ayşe Kaya", Role = "cashier", Phone = "0533 222 33 44", IsActive = true, HiredAt = new DateOnly(2022, 3, 15), PasswordHash = LoginForm.HashPassword("1234") },
                        new Employee { FullName = "Ali Demir", Role = "waiter", Phone = "0534 333 44 55", IsActive = true, HiredAt = new DateOnly(2023, 6, 1), PasswordHash = LoginForm.HashPassword("1234") },
                        new Employee { FullName = "Fatma Çelik", Role = "waiter", Phone = "0535 444 55 66", IsActive = true, HiredAt = new DateOnly(2023, 6, 1), PasswordHash = LoginForm.HashPassword("1234") },
                        new Employee { FullName = "Hasan Arslan", Role = "kitchen", Phone = "0536 555 66 77", IsActive = true, HiredAt = new DateOnly(2022, 8, 20), PasswordHash = LoginForm.HashPassword("1234") },
                        new Employee { FullName = "Zeynep Polat", Role = "kitchen", Phone = "0537 666 77 88", IsActive = true, HiredAt = new DateOnly(2023, 9, 5), PasswordHash = LoginForm.HashPassword("1234") }
                    );
                    db.SaveChanges();
                }

                // 2. TEDARİKÇİLER
                if (!db.Suppliers.Any())
                {
                    db.Suppliers.AddRange(
                        new Supplier { Name = "Günaydın Et A.Ş.", Phone = "0212 111 22 33", Address = "İstanbul, Bağcılar", TaxNumber = "1234567890", IsActive = true },
                        new Supplier { Name = "Taze Sebze Pazarı", Phone = "0312 222 33 44", Address = "Ankara, Ulus", TaxNumber = "2345678901", IsActive = true },
                        new Supplier { Name = "Anadolu İçecek Ltd.", Phone = "0232 333 44 55", Address = "İzmir, Bornova", TaxNumber = "3456789012", IsActive = true },
                        new Supplier { Name = "Öz Ekmek Fırını", Phone = "0262 444 55 66", Address = "Sakarya, Adapazarı", TaxNumber = "4567890123", IsActive = true },
                        new Supplier { Name = "Kaliteli Yağ San. A.Ş.", Phone = "0224 555 66 77", Address = "Bursa, Osmangazi", TaxNumber = "5678901234", IsActive = true }
                    );
                    db.SaveChanges();
                }

                // 3. MALZEMELER
                if (!db.Ingredients.Any())
                {
                    var sup = db.Suppliers.ToList();
                    db.Ingredients.AddRange(
                        new Ingredient { Name = "Beef Doner", Unit = "kg", StockQuantity = 45, MinStock = 10, UnitPrice = 280, SupplierId = sup[0].Id },
                        new Ingredient { Name = "Chicken Doner", Unit = "kg", StockQuantity = 30, MinStock = 8, UnitPrice = 180, SupplierId = sup[0].Id },
                        new Ingredient { Name = "Lavash", Unit = "piece", StockQuantity = 200, MinStock = 50, UnitPrice = 3.5m, SupplierId = sup[3].Id },
                        new Ingredient { Name = "Bread", Unit = "piece", StockQuantity = 150, MinStock = 40, UnitPrice = 4, SupplierId = sup[3].Id },
                        new Ingredient { Name = "Tomato", Unit = "kg", StockQuantity = 12, MinStock = 3, UnitPrice = 15, SupplierId = sup[1].Id },
                        new Ingredient { Name = "Onion", Unit = "kg", StockQuantity = 10, MinStock = 2, UnitPrice = 8, SupplierId = sup[1].Id },
                        new Ingredient { Name = "Lettuce", Unit = "kg", StockQuantity = 8, MinStock = 2, UnitPrice = 12, SupplierId = sup[1].Id },
                        new Ingredient { Name = "Pickles", Unit = "kg", StockQuantity = 6, MinStock = 1.5m, UnitPrice = 20, SupplierId = sup[1].Id },
                        new Ingredient { Name = "Ayran", Unit = "piece", StockQuantity = 120, MinStock = 30, UnitPrice = 10, SupplierId = sup[2].Id },
                        new Ingredient { Name = "Cola", Unit = "piece", StockQuantity = 96, MinStock = 24, UnitPrice = 12, SupplierId = sup[2].Id },
                        new Ingredient { Name = "Water", Unit = "piece", StockQuantity = 80, MinStock = 20, UnitPrice = 4, SupplierId = sup[2].Id },
                        new Ingredient { Name = "Sunflower Oil", Unit = "litre", StockQuantity = 20, MinStock = 5, UnitPrice = 60, SupplierId = sup[4].Id },
                        new Ingredient { Name = "Potato", Unit = "kg", StockQuantity = 25, MinStock = 5, UnitPrice = 10, SupplierId = sup[1].Id }
                    );
                    db.SaveChanges();
                }

                // 4. ÜRÜNLER
                if (!db.Products.Any())
                {
                    db.Products.AddRange(
                        new Product { Name = "Beef Wrap", Category = "doner", Price = 180, IsActive = true, Description = "Beef doner, lavash, tomato, onion, lettuce" },
                        new Product { Name = "Chicken Wrap", Category = "doner", Price = 150, IsActive = true, Description = "Chicken doner, lavash, tomato, onion, lettuce" },
                        new Product { Name = "Beef Sandwich", Category = "sandwich", Price = 160, IsActive = true, Description = "Beef doner, bread, tomato, onion, lettuce" },
                        new Product { Name = "Chicken Sandwich", Category = "sandwich", Price = 130, IsActive = true, Description = "Chicken doner, bread, tomato, onion, lettuce" },
                        new Product { Name = "Beef Portion", Category = "portion", Price = 220, IsActive = true, Description = "Beef doner portion with rice and salad" },
                        new Product { Name = "Chicken Portion", Category = "portion", Price = 190, IsActive = true, Description = "Chicken doner portion with rice and salad" },
                        new Product { Name = "French Fries", Category = "extra", Price = 60, IsActive = true, Description = "Crispy french fries" },
                        new Product { Name = "Ayran", Category = "beverage", Price = 25, IsActive = true, Description = "Cold ayran 300ml" },
                        new Product { Name = "Cola", Category = "beverage", Price = 40, IsActive = true, Description = "Cola 330ml" },
                        new Product { Name = "Water", Category = "beverage", Price = 15, IsActive = true, Description = "Water 500ml" }
                    );
                    db.SaveChanges();
                }

                // 5. MASALAR
                if (!db.RestaurantTables.Any())
                {
                    for (int i = 1; i <= 25; i++)
                    {
                        db.RestaurantTables.Add(new RestaurantTable
                        {
                            TableNumber = i,
                            SeatCapacity = i <= 5 ? 2 : i <= 18 ? 4 : 6,
                            Status = "available"
                        });
                    }
                    db.SaveChanges();
                }

                // 6. ÜRÜN-MALZEME İLİŞKİLERİ
                if (!db.ProductIngredients.Any())
                {
                    var products = db.Products.ToList();
                    var ing = db.Ingredients.ToList();
                    db.ProductIngredients.AddRange(
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Wrap").Id, IngredientId = ing.First(i => i.Name == "Beef Doner").Id, Quantity = 0.150m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Wrap").Id, IngredientId = ing.First(i => i.Name == "Lavash").Id, Quantity = 1, Unit = "piece" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Wrap").Id, IngredientId = ing.First(i => i.Name == "Tomato").Id, Quantity = 0.05m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Wrap").Id, IngredientId = ing.First(i => i.Name == "Onion").Id, Quantity = 0.03m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Wrap").Id, IngredientId = ing.First(i => i.Name == "Lettuce").Id, Quantity = 0.04m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Wrap").Id, IngredientId = ing.First(i => i.Name == "Chicken Doner").Id, Quantity = 0.150m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Wrap").Id, IngredientId = ing.First(i => i.Name == "Lavash").Id, Quantity = 1, Unit = "piece" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Wrap").Id, IngredientId = ing.First(i => i.Name == "Tomato").Id, Quantity = 0.05m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Wrap").Id, IngredientId = ing.First(i => i.Name == "Onion").Id, Quantity = 0.03m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Wrap").Id, IngredientId = ing.First(i => i.Name == "Lettuce").Id, Quantity = 0.04m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Sandwich").Id, IngredientId = ing.First(i => i.Name == "Beef Doner").Id, Quantity = 0.150m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Sandwich").Id, IngredientId = ing.First(i => i.Name == "Bread").Id, Quantity = 1, Unit = "piece" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Sandwich").Id, IngredientId = ing.First(i => i.Name == "Tomato").Id, Quantity = 0.05m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Sandwich").Id, IngredientId = ing.First(i => i.Name == "Onion").Id, Quantity = 0.03m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Sandwich").Id, IngredientId = ing.First(i => i.Name == "Lettuce").Id, Quantity = 0.04m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Sandwich").Id, IngredientId = ing.First(i => i.Name == "Chicken Doner").Id, Quantity = 0.150m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Sandwich").Id, IngredientId = ing.First(i => i.Name == "Bread").Id, Quantity = 1, Unit = "piece" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Sandwich").Id, IngredientId = ing.First(i => i.Name == "Tomato").Id, Quantity = 0.05m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Sandwich").Id, IngredientId = ing.First(i => i.Name == "Onion").Id, Quantity = 0.03m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Sandwich").Id, IngredientId = ing.First(i => i.Name == "Lettuce").Id, Quantity = 0.04m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Beef Portion").Id, IngredientId = ing.First(i => i.Name == "Beef Doner").Id, Quantity = 0.250m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Chicken Portion").Id, IngredientId = ing.First(i => i.Name == "Chicken Doner").Id, Quantity = 0.250m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "French Fries").Id, IngredientId = ing.First(i => i.Name == "Potato").Id, Quantity = 0.200m, Unit = "kg" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "French Fries").Id, IngredientId = ing.First(i => i.Name == "Sunflower Oil").Id, Quantity = 0.02m, Unit = "litre" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Ayran").Id, IngredientId = ing.First(i => i.Name == "Ayran").Id, Quantity = 1, Unit = "piece" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Cola").Id, IngredientId = ing.First(i => i.Name == "Cola").Id, Quantity = 1, Unit = "piece" },
                        new ProductIngredient { ProductId = products.First(p => p.Name == "Water").Id, IngredientId = ing.First(i => i.Name == "Water").Id, Quantity = 1, Unit = "piece" }
                    );
                    db.SaveChanges();
                }
            }

            Application.Run(new LoginForm());
        }
    }
}