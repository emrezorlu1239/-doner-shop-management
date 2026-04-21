using DonerApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Windows.Forms;

namespace DonerApp
{
    public partial class OrderForm : Form
    {
        private int _orderId;
        private int _tableNumber;
        private Panel pnlItems = new Panel();
        private Label lblTotalValue = new Label();
        private Label lblSubtotalValue = new Label();

        public OrderForm(int orderId = 0, int tableNumber = 0)
        {
            InitializeComponent();
            if (!Permission.CanAccess("Orders"))
            {
                Permission.Deny("Orders");
                this.Load += (s, e) => this.Close();
                return;
            }
            _orderId = orderId;
            _tableNumber = tableNumber;
            ApplyDesign();
            LoadProducts();
            LoadOrderItems();
        }

        private void ApplyDesign()
        {
            this.Text = "Order Management";
            this.Size = new Size(1100, 820);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Top bar
            var pnlTop = new Panel();
            pnlTop.Size = new Size(1100, 60);
            pnlTop.Location = new Point(0, 0);
            pnlTop.BackColor = Color.FromArgb(41, 128, 185);

            var lblTitle = new Label();
            lblTitle.Text = _tableNumber > 0 ? $"Order — Table {_tableNumber}" : "New Order";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            var lblTime = new Label();
            lblTime.Text = $"Opened: {DateTime.Now:HH:mm}";
            lblTime.Font = new Font("Segoe UI", 10);
            lblTime.ForeColor = Color.FromArgb(180, 220, 255);
            lblTime.AutoSize = true;
            lblTime.Location = new Point(300, 20);

            var btnBack = new Button();
            btnBack.Text = "← Back";
            btnBack.Size = new Size(90, 32);
            btnBack.Location = new Point(985, 14);
            btnBack.Font = new Font("Segoe UI", 9);
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.FlatAppearance.BorderColor = Color.White;
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => this.Close();

            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(lblTime);
            pnlTop.Controls.Add(btnBack);

            // ── Left panel — product menu ──
            var pnlLeft = new Panel();
            pnlLeft.Size = new Size(480, 700);
            pnlLeft.Location = new Point(10, 68);
            pnlLeft.BackColor = Color.White;
            pnlLeft.Name = "pnlLeft";

            var lblMenuTitle = new Label();
            lblMenuTitle.Text = "Menu";
            lblMenuTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblMenuTitle.ForeColor = Color.FromArgb(40, 40, 40);
            lblMenuTitle.AutoSize = true;
            lblMenuTitle.Location = new Point(15, 15);

            // Category buttons
            string[] categories = { "All", "Doner", "Sandwich", "Portion", "Beverage", "Extra" };
            Color[] catColors = {
                Color.FromArgb(230,126,34),
                Color.FromArgb(41,128,185),
                Color.FromArgb(39,174,96),
                Color.FromArgb(142,68,173),
                Color.FromArgb(22,160,133),
                Color.FromArgb(243,156,18)
            };

            for (int i = 0; i < categories.Length; i++)
            {
                var btn = new Button();
                btn.Name = $"btnCat_{categories[i]}";
                btn.Text = categories[i];
                btn.Size = new Size(68, 28);
                btn.Location = new Point(15 + i * 74, 45);
                btn.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                btn.BackColor = i == 0 ? catColors[i] : Color.FromArgb(240, 240, 240);
                btn.ForeColor = i == 0 ? Color.White : Color.FromArgb(80, 80, 80);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Cursor = Cursors.Hand;
                btn.Tag = categories[i];
                btn.Click += BtnCategory_Click;
                pnlLeft.Controls.Add(btn);
            }

            // Product list panel
            var pnlProducts = new Panel();
            pnlProducts.Name = "pnlProducts";
            pnlProducts.Size = new Size(460, 620);
            pnlProducts.Location = new Point(10, 82);
            pnlProducts.BackColor = Color.Transparent;
            pnlProducts.AutoScroll = true;

            pnlLeft.Controls.Add(lblMenuTitle);
            pnlLeft.Controls.Add(pnlProducts);

            // ── Right panel — order summary ──
            var pnlRight = new Panel();
            pnlRight.Size = new Size(590, 700);
            pnlRight.Location = new Point(500, 68);
            pnlRight.BackColor = Color.White;

            var lblOrderTitle = new Label();
            lblOrderTitle.Text = "Current Order";
            lblOrderTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblOrderTitle.ForeColor = Color.FromArgb(40, 40, 40);
            lblOrderTitle.AutoSize = true;
            lblOrderTitle.Location = new Point(15, 15);

            // Table header
            var pnlHeader = new Panel();
            pnlHeader.Size = new Size(560, 32);
            pnlHeader.Location = new Point(15, 45);
            pnlHeader.BackColor = Color.FromArgb(245, 245, 245);

            string[] headers = { "Product", "Qty", "Unit Price", "Total", "" };
            int[] hWidths = { 200, 60, 100, 100, 60 };
            int hX = 10;
            foreach (var (h, w) in System.Linq.Enumerable.Zip(headers, hWidths))
            {
                var lh = new Label();
                lh.Text = h;
                lh.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                lh.ForeColor = Color.Gray;
                lh.Size = new Size(w, 32);
                lh.Location = new Point(hX, 0);
                lh.TextAlign = ContentAlignment.MiddleLeft;
                pnlHeader.Controls.Add(lh);
                hX += w;
            }

            // Order items panel
            pnlItems.Name = "pnlItems";
            pnlItems.Size = new Size(560, 420);
            pnlItems.Location = new Point(15, 82);
            pnlItems.BackColor = Color.Transparent;
            pnlItems.AutoScroll = true;

            // Divider line
            var divider = new Panel();
            divider.Size = new Size(560, 1);
            divider.Location = new Point(15, 510);
            divider.BackColor = Color.FromArgb(220, 220, 220);

            // Subtotal
            var lblSubtotalLabel = new Label();
            lblSubtotalLabel.Text = "Subtotal:";
            lblSubtotalLabel.Font = new Font("Segoe UI", 10);
            lblSubtotalLabel.ForeColor = Color.Gray;
            lblSubtotalLabel.AutoSize = true;
            lblSubtotalLabel.Location = new Point(340, 520);

            lblSubtotalValue.Name = "lblSubtotalValue";
            lblSubtotalValue.Text = "₺0.00";
            lblSubtotalValue.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblSubtotalValue.ForeColor = Color.FromArgb(40, 40, 40);
            lblSubtotalValue.AutoSize = true;
            lblSubtotalValue.Location = new Point(480, 520);

            // Total
            var lblTotalLabel = new Label();
            lblTotalLabel.Text = "TOTAL:";
            lblTotalLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTotalLabel.ForeColor = Color.FromArgb(40, 40, 40);
            lblTotalLabel.AutoSize = true;
            lblTotalLabel.Location = new Point(340, 550);

            lblTotalValue.Name = "lblTotalValue";
            lblTotalValue.Text = "₺0.00";
            lblTotalValue.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTotalValue.ForeColor = Color.FromArgb(230, 126, 34);
            lblTotalValue.AutoSize = true;
            lblTotalValue.Location = new Point(460, 550);

            // Payment buttons
            var btnCash = new Button();
            btnCash.Name = "btnCash";
            btnCash.Text = "💵  Cash";
            btnCash.Size = new Size(175, 46);
            btnCash.Location = new Point(15, 635);
            btnCash.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCash.BackColor = Color.FromArgb(39, 174, 96);
            btnCash.ForeColor = Color.White;
            btnCash.FlatStyle = FlatStyle.Flat;
            btnCash.FlatAppearance.BorderSize = 0;
            btnCash.Cursor = Cursors.Hand;
            btnCash.Click += (s, e) => ProcessPayment("cash");

            var btnCard = new Button();
            btnCard.Name = "btnCard";
            btnCard.Text = "💳  Card";
            btnCard.Size = new Size(175, 46);
            btnCard.Location = new Point(200, 635);
            btnCard.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCard.BackColor = Color.FromArgb(41, 128, 185);
            btnCard.ForeColor = Color.White;
            btnCard.FlatStyle = FlatStyle.Flat;
            btnCard.FlatAppearance.BorderSize = 0;
            btnCard.Cursor = Cursors.Hand;
            btnCard.Click += (s, e) => ProcessPayment("card");

            var btnCancel = new Button();
            btnCancel.Name = "btnCancel";
            btnCancel.Text = "✕  Cancel Order";
            btnCancel.Size = new Size(175, 46);
            btnCancel.Location = new Point(385, 635);
            btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(231, 76, 60);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += BtnCancel_Click;

            pnlRight.Controls.Add(lblOrderTitle);
            pnlRight.Controls.Add(pnlHeader);
            pnlRight.Controls.Add(pnlItems);
            pnlRight.Controls.Add(divider);
            pnlRight.Controls.Add(lblSubtotalLabel);
            pnlRight.Controls.Add(lblSubtotalValue);
            pnlRight.Controls.Add(lblTotalLabel);
            pnlRight.Controls.Add(lblTotalValue);
            pnlRight.Controls.Add(btnCash);
            pnlRight.Controls.Add(btnCard);
            pnlRight.Controls.Add(btnCancel);

            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlLeft);
            this.Controls.Add(pnlRight);
        }

        // ── Load products ──
        private void LoadProducts(string category = "All")
        {
            var pnlProducts = FindControl<Panel>("pnlProducts");
            if (pnlProducts == null) return;
            pnlProducts.Controls.Clear();

            using var db = new AppDbContext();
            var products = db.Products
                .Where(p => p.IsActive)
                .Where(p => category == "All" || p.Category == category.ToLower())
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToList();

            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];
                var row = new Panel();
                row.Size = new Size(440, 56);
                row.Location = new Point(0, i * 58);
                row.BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(250, 250, 250);

                var lblName = new Label();
                lblName.Text = product.Name;
                lblName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lblName.ForeColor = Color.FromArgb(40, 40, 40);
                lblName.AutoSize = true;
                lblName.Location = new Point(10, 8);

                var lblCat = new Label();
                lblCat.Text = product.Category;
                lblCat.Font = new Font("Segoe UI", 8);
                lblCat.ForeColor = Color.Gray;
                lblCat.AutoSize = true;
                lblCat.Location = new Point(10, 30);

                var lblPrice = new Label();
                lblPrice.Text = $"₺{product.Price:0.00}";
                lblPrice.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lblPrice.ForeColor = Color.FromArgb(230, 126, 34);
                lblPrice.AutoSize = true;
                lblPrice.Location = new Point(290, 18);

                var btnAdd = new Button();
                btnAdd.Text = "+ Add";
                btnAdd.Size = new Size(75, 32);
                btnAdd.Location = new Point(368, 12);
                btnAdd.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                btnAdd.BackColor = Color.FromArgb(39, 174, 96);
                btnAdd.ForeColor = Color.White;
                btnAdd.FlatStyle = FlatStyle.Flat;
                btnAdd.FlatAppearance.BorderSize = 0;
                btnAdd.Cursor = Cursors.Hand;
                btnAdd.Tag = product.Id;
                btnAdd.Click += BtnAddProduct_Click;

                row.Controls.Add(lblName);
                row.Controls.Add(lblCat);
                row.Controls.Add(lblPrice);
                row.Controls.Add(btnAdd);
                pnlProducts.Controls.Add(row);
            }
        }

        // ── Load order items ──
        private void LoadOrderItems()
        {
            pnlItems.Controls.Clear();

            if (_orderId == 0)
            {
                UpdateTotal(0);
                return;
            }

            using var db = new AppDbContext();
            var items = db.OrderItems
                .Include(i => i.Product)
                .Where(i => i.OrderId == _orderId && i.Status != "cancelled")
                .ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var row = new Panel();
                row.Size = new Size(540, 52);
                row.Location = new Point(0, i * 54);
                row.BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(250, 250, 250);

                var lblN = new Label();
                lblN.Text = item.Product?.Name ?? "";
                lblN.Font = new Font("Segoe UI", 10);
                lblN.Size = new Size(200, 52);
                lblN.Location = new Point(10, 0);
                lblN.TextAlign = ContentAlignment.MiddleLeft;

                var lblQ = new Label();
                lblQ.Text = item.Quantity.ToString();
                lblQ.Font = new Font("Segoe UI", 10);
                lblQ.Size = new Size(60, 52);
                lblQ.Location = new Point(210, 0);
                lblQ.TextAlign = ContentAlignment.MiddleCenter;

                var lblUP = new Label();
                lblUP.Text = $"₺{item.UnitPrice:0.00}";
                lblUP.Font = new Font("Segoe UI", 10);
                lblUP.Size = new Size(100, 52);
                lblUP.Location = new Point(270, 0);
                lblUP.TextAlign = ContentAlignment.MiddleRight;

                var lblT = new Label();
                lblT.Text = $"₺{item.Quantity * item.UnitPrice:0.00}";
                lblT.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lblT.ForeColor = Color.FromArgb(230, 126, 34);
                lblT.Size = new Size(100, 52);
                lblT.Location = new Point(370, 0);
                lblT.TextAlign = ContentAlignment.MiddleRight;

                var btnRemove = new Button();
                btnRemove.Text = "✕";
                btnRemove.Size = new Size(32, 32);
                btnRemove.Location = new Point(500, 10);
                btnRemove.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                btnRemove.ForeColor = Color.White;
                btnRemove.BackColor = Color.FromArgb(231, 76, 60);
                btnRemove.FlatStyle = FlatStyle.Flat;
                btnRemove.FlatAppearance.BorderSize = 0;
                btnRemove.Cursor = Cursors.Hand;
                btnRemove.Tag = item.Id;
                btnRemove.Click += BtnRemoveItem_Click;

                row.Controls.Add(lblN);
                row.Controls.Add(lblQ);
                row.Controls.Add(lblUP);
                row.Controls.Add(lblT);
                row.Controls.Add(btnRemove);
                pnlItems.Controls.Add(row);
            }

            decimal total = items.Sum(i => i.Quantity * i.UnitPrice);
            UpdateTotal(total);
        }

        private void UpdateTotal(decimal total)
        {
            lblSubtotalValue.Text = $"₺{total:0.00}";
            lblTotalValue.Text = $"₺{total:0.00}";
        }

        // ── Add Product ──
        private void BtnAddProduct_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int productId) return;

            using var db = new AppDbContext();

            if (_orderId == 0)
            {
                var newOrder = new Order
                {
                    EmployeeId = Session.CurrentEmployee?.Id,
                    OpenedAt = DateTime.Now,
                    Status = "open",
                    TotalAmount = 0
                };
                db.Orders.Add(newOrder);
                db.SaveChanges();
                _orderId = newOrder.Id;
            }

            var product = db.Products.Find(productId);
            if (product == null) return;

            var existing = db.OrderItems
                .FirstOrDefault(i => i.OrderId == _orderId
                    && i.ProductId == productId
                    && i.Status != "cancelled");

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                db.OrderItems.Add(new OrderItem
                {
                    OrderId = _orderId,
                    ProductId = productId,
                    Quantity = 1,
                    UnitPrice = product.Price,
                    Status = "pending"
                });
            }

            db.SaveChanges();
            LoadOrderItems();
        }

        // ── Remove item ──
        private void BtnRemoveItem_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int itemId) return;

            using var db = new AppDbContext();
            var item = db.OrderItems.Find(itemId);
            if (item == null) return;

            if (item.Quantity > 1)
                item.Quantity--;
            else
                item.Status = "cancelled";

            db.SaveChanges();
            LoadOrderItems();
        }

        // ── Category filter ──
        private void BtnCategory_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            string category = btn.Tag?.ToString() ?? "All";

            // Reset button colors
            var pnlLeft = FindControl<Panel>("pnlLeft");
            if (pnlLeft != null)
            {
                foreach (var b in pnlLeft.Controls.OfType<Button>())
                {
                    b.BackColor = Color.FromArgb(240, 240, 240);
                    b.ForeColor = Color.FromArgb(80, 80, 80);
                }
            }

            btn.BackColor = Color.FromArgb(230, 126, 34);
            btn.ForeColor = Color.White;

            LoadProducts(category);
        }

        // ── Process payment ──
        private void ProcessPayment(string method)
        {
            if (_orderId == 0) return;

            using var db = new AppDbContext();
            var order = db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == _orderId);

            if (order == null) return;

            var activeItems = order.OrderItems
                .Where(i => i.Status != "cancelled").ToList();

            if (!activeItems.Any())
            {
                MessageBox.Show("Cannot close an empty order.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal total = activeItems.Sum(i => i.Quantity * i.UnitPrice);

            var confirm = MessageBox.Show(
                $"Collect payment of ₺{total:0.00} via {method.ToUpper()}?",
                "Confirm Payment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            // Save payment
            db.Payments.Add(new Payment
            {
                OrderId = _orderId,
                Amount = total,
                PaymentMethod = method,
                PaidAt = DateTime.Now
            });

            // Close order
            order.Status = "completed";
            order.ClosedAt = DateTime.Now;
            order.TotalAmount = total;

            // Clear the table
            var table = db.RestaurantTables
                .FirstOrDefault(t => t.ActiveOrderId == _orderId);
            if (table != null)
            {
                table.Status = "cleaning";
                table.ActiveOrderId = null;
            }

            DeductStock(db, activeItems);

            db.SaveChanges();

            MessageBox.Show(
                $"Payment successful! ₺{total:0.00} collected via {method.ToUpper()}.",
                "Payment Complete",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.Close();
        }

        private void DeductStock(AppDbContext db, List<OrderItem> orderItems)
        {
            foreach (var item in orderItems)
            {
                var productRelations = db.ProductIngredients
                    .Include(pi => pi.Ingredient)
                    .Where(pi => pi.ProductId == item.ProductId)
                    .ToList();

                foreach (var relation in productRelations)
                {
                    if (relation.Ingredient == null) continue;

                    decimal deduction = relation.Quantity * item.Quantity;
                    relation.Ingredient.StockQuantity -= deduction;
                    relation.Ingredient.UpdatedAt = DateTime.Now;

                    db.StockMovements.Add(new StockMovement
                    {
                        IngredientId = relation.IngredientId,
                        Quantity = -deduction,
                        MovementType = "sale",
                        MovedAt = DateTime.Now,
                        Notes = $"Sale: {item.Product?.Name} x{item.Quantity}"
                    });
                }
            }
        }

        // ── Cancel order ──
        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            if (_orderId == 0) { this.Close(); return; }

            var confirm = MessageBox.Show(
                "Are you sure you want to cancel this order?",
                "Cancel Order",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            using var db = new AppDbContext();
            var order = db.Orders.Find(_orderId);
            if (order != null)
            {
                order.Status = "cancelled";
                order.ClosedAt = DateTime.Now;
            }

            var table = db.RestaurantTables
                .FirstOrDefault(t => t.ActiveOrderId == _orderId);
            if (table != null)
            {
                table.Status = "available";
                table.ActiveOrderId = null;
            }

            db.SaveChanges();

            MessageBox.Show("Order cancelled.", "Cancelled",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }

        private void OrderForm_Load(object sender, EventArgs e)
        {

        }

        // ── Helper: find control ──
        private T? FindControl<T>(string name) where T : Control
        {
            return FindControlRecursive<T>(this, name);
        }

        private static T? FindControlRecursive<T>(Control parent, string name) where T : Control
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is T match && ctrl.Name == name) return match;
                var found = FindControlRecursive<T>(ctrl, name);
                if (found != null) return found;
            }
            return null;
        }
    }
}