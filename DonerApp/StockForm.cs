using DonerApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Windows.Forms;

namespace DonerApp
{
    public partial class StockForm : Form
    {
        private Panel pnlRows = new Panel();
        private string _currentFilter = "All";
        private string _searchText = "";

        public StockForm()
        {
            InitializeComponent();
            ApplyDesign();
            LoadIngredients();
        }

        private void ApplyDesign()
        {
            if (!Permission.CanAccess("Stock"))
            {
                Permission.Deny("Stock");
                this.Load += (s, e) => this.Close();
                return;
            }

            // Kitchen role can only view, not add stock
            bool isKitchen = Session.CurrentEmployee?.Role == "kitchen";

            this.Text = "Stock Tracking";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Top bar
            var pnlTop = new Panel();
            pnlTop.Size = new Size(1100, 60);
            pnlTop.Location = new Point(0, 0);
            pnlTop.BackColor = Color.FromArgb(39, 174, 96);

            var lblTitle = new Label();
            lblTitle.Text = "Stock Tracking";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            var btnAddNew = new Button();
            btnAddNew.Text = "+ Add Ingredient";
            btnAddNew.Size = new Size(150, 32);
            btnAddNew.Location = new Point(820, 14);
            btnAddNew.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAddNew.ForeColor = Color.White;
            btnAddNew.FlatStyle = FlatStyle.Flat;
            btnAddNew.FlatAppearance.BorderColor = Color.White;
            btnAddNew.Cursor = Cursors.Hand;
            btnAddNew.Click += BtnAddIngredient_Click;

            btnAddNew.Visible = !isKitchen;

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
            pnlTop.Controls.Add(btnAddNew);
            pnlTop.Controls.Add(btnBack);

            // Summary cards — dynamic, updated in LoadIngredients
            var cardNames = new[] { "pnlCardTotal", "pnlCardLow", "pnlCardOut", "pnlCardValue" };
            var cardTitles = new[] { "Total Ingredients", "Low Stock", "Out of Stock", "Total Value" };
            var cardColors = new[] {
                Color.FromArgb(41,  128, 185),
                Color.FromArgb(243, 156, 18),
                Color.FromArgb(231, 76,  60),
                Color.FromArgb(39,  174, 96)
            };

            for (int i = 0; i < cardTitles.Length; i++)
            {
                var card = new Panel();
                card.Name = cardNames[i];
                card.Size = new Size(255, 82);
                card.Location = new Point(20 + i * 268, 68);
                card.BackColor = Color.White;

                var bar = new Panel();
                bar.Size = new Size(255, 5);
                bar.Location = new Point(0, 0);
                bar.BackColor = cardColors[i];

                var lblVal = new Label();
                lblVal.Name = $"lblVal_{i}";
                lblVal.Text = "—";
                lblVal.Font = new Font("Segoe UI", 22, FontStyle.Bold);
                lblVal.ForeColor = cardColors[i];
                lblVal.Size = new Size(235, 40);
                lblVal.Location = new Point(15, 10);
                lblVal.TextAlign = ContentAlignment.MiddleLeft;

                var lblLbl = new Label();
                lblLbl.Text = cardTitles[i];
                lblLbl.Font = new Font("Segoe UI", 9);
                lblLbl.ForeColor = Color.Gray;
                lblLbl.Size = new Size(235, 22);
                lblLbl.Location = new Point(15, 52);

                card.Controls.Add(bar);
                card.Controls.Add(lblVal);
                card.Controls.Add(lblLbl);
                this.Controls.Add(card);
            }

            // Search & filter bar
            var pnlSearch = new Panel();
            pnlSearch.Size = new Size(1060, 44);
            pnlSearch.Location = new Point(20, 160);
            pnlSearch.BackColor = Color.White;

            var txtSearch = new TextBox();
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(300, 30);
            txtSearch.Location = new Point(10, 7);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.PlaceholderText = "Search ingredient...";
            txtSearch.TextChanged += (s, e) =>
            {
                _searchText = txtSearch.Text.Trim();
                LoadIngredients();
            };

            string[] filterTexts = { "All", "Low Stock", "Out of Stock" };
            for (int i = 0; i < filterTexts.Length; i++)
            {
                var filter = filterTexts[i];
                var btn = new Button();
                btn.Name = $"btnFilter_{filter}";
                btn.Text = filter;
                btn.Size = new Size(110, 30);
                btn.Location = new Point(325 + i * 116, 7);
                btn.Font = new Font("Segoe UI", 9);
                btn.BackColor = i == 0 ? Color.FromArgb(39, 174, 96) : Color.FromArgb(240, 240, 240);
                btn.ForeColor = i == 0 ? Color.White : Color.FromArgb(80, 80, 80);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Cursor = Cursors.Hand;
                btn.Tag = filter;
                btn.Click += BtnFilter_Click;
                pnlSearch.Controls.Add(btn);
            }

            pnlSearch.Controls.Add(txtSearch);

            // Table header
            var pnlHeader = new Panel();
            pnlHeader.Size = new Size(1060, 36);
            pnlHeader.Location = new Point(20, 212);
            pnlHeader.BackColor = Color.FromArgb(245, 245, 245);

            string[] headers = { "Ingredient", "Unit", "Stock", "Min Stock", "Unit Price", "Supplier", "Status", "Actions" };
            int[] widths = { 180, 70, 80, 80, 100, 160, 110, 180 };
            int hx = 10;
            foreach (var (h, w) in System.Linq.Enumerable.Zip(headers, widths))
            {
                var lh = new Label();
                lh.Text = h;
                lh.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                lh.ForeColor = Color.Gray;
                lh.Size = new Size(w, 36);
                lh.Location = new Point(hx, 0);
                lh.TextAlign = ContentAlignment.MiddleLeft;
                pnlHeader.Controls.Add(lh);
                hx += w;
            }

            // Rows panel
            pnlRows.Size = new Size(1060, 440);
            pnlRows.Location = new Point(20, 250);
            pnlRows.BackColor = Color.White;
            pnlRows.AutoScroll = true;

            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlRows);
        }

        private void LoadIngredients()
        {
            pnlRows.Controls.Clear();

            using var db = new AppDbContext();
            var query = db.Ingredients
                .Include(i => i.Supplier)
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(_searchText))
                query = query.Where(i => i.Name.ToLower().Contains(_searchText.ToLower()));

            // Status filter
            if (_currentFilter == "Low Stock")
                query = query.Where(i => i.StockQuantity <= i.MinStock && i.StockQuantity > 0);
            else if (_currentFilter == "Out of Stock")
                query = query.Where(i => i.StockQuantity == 0);

            var ingredients = query.OrderBy(i => i.Name).ToList();
            var all = db.Ingredients.ToList();

            // Update cards
            UpdateCard(0, all.Count.ToString());
            UpdateCard(1, all.Count(i => i.StockQuantity <= i.MinStock && i.StockQuantity > 0).ToString());
            UpdateCard(2, all.Count(i => i.StockQuantity == 0).ToString());
            UpdateCard(3, $"₺{all.Sum(i => i.StockQuantity * (i.UnitPrice ?? 0)):0}");

            for (int idx = 0; idx < ingredients.Count; idx++)
            {
                var ing = ingredients[idx];
                bool isLow = ing.StockQuantity <= ing.MinStock;
                bool isOut = ing.StockQuantity == 0;

                var row = new Panel();
                row.Size = new Size(1040, 48);
                row.Location = new Point(0, idx * 50);
                row.BackColor = isOut ? Color.FromArgb(255, 240, 240)
                              : isLow ? Color.FromArgb(255, 250, 235)
                              : (idx % 2 == 0 ? Color.White : Color.FromArgb(250, 250, 250));

                int[] colW = { 180, 70, 80, 80, 100, 160, 110, 180 };
                int rx = 10;

                // Name
                AddLabel(row, ing.Name, new Font("Segoe UI", 10, FontStyle.Bold),
                    Color.FromArgb(40, 40, 40), colW[0], rx); rx += colW[0];

                // Unit
                AddLabel(row, ing.Unit, new Font("Segoe UI", 9),
                    Color.Gray, colW[1], rx); rx += colW[1];

                // Stock qty
                AddLabel(row, ing.StockQuantity.ToString("0.###"),
                    new Font("Segoe UI", 10, FontStyle.Bold),
                    isOut ? Color.FromArgb(231, 76, 60)
                    : isLow ? Color.FromArgb(243, 156, 18)
                    : Color.FromArgb(39, 174, 96),
                    colW[2], rx); rx += colW[2];

                // Min stock
                AddLabel(row, ing.MinStock.ToString("0.###"),
                    new Font("Segoe UI", 9), Color.Gray, colW[3], rx); rx += colW[3];

                // Unit price
                AddLabel(row, $"₺{ing.UnitPrice:0.00}",
                    new Font("Segoe UI", 9),
                    Color.FromArgb(230, 126, 34), colW[4], rx); rx += colW[4];

                // Supplier
                AddLabel(row, ing.Supplier?.Name ?? "—",
                    new Font("Segoe UI", 9),
                    Color.FromArgb(60, 60, 60), colW[5], rx); rx += colW[5];

                // Status
                AddLabel(row,
                    isOut ? "OUT OF STOCK" : isLow ? "LOW STOCK" : "OK",
                    new Font("Segoe UI", 8, FontStyle.Bold),
                    isOut ? Color.FromArgb(231, 76, 60)
                    : isLow ? Color.FromArgb(243, 156, 18)
                    : Color.FromArgb(39, 174, 96),
                    colW[6], rx); rx += colW[6];

                // Edit button
                var btnEdit = new Button();
                btnEdit.Text = "Edit";
                btnEdit.Size = new Size(55, 28);
                btnEdit.Location = new Point(rx, 10);
                btnEdit.Font = new Font("Segoe UI", 8);
                btnEdit.BackColor = Color.FromArgb(41, 128, 185);
                btnEdit.ForeColor = Color.White;
                btnEdit.FlatStyle = FlatStyle.Flat;
                btnEdit.FlatAppearance.BorderSize = 0;
                btnEdit.Cursor = Cursors.Hand;
                btnEdit.Tag = ing.Id;
                btnEdit.Click += BtnEdit_Click;

                // + Stock button
                var btnStock = new Button();
                btnStock.Text = "+ Stock";
                btnStock.Size = new Size(75, 28);
                btnStock.Location = new Point(rx + 60, 10);
                btnStock.Font = new Font("Segoe UI", 8);
                btnStock.BackColor = Color.FromArgb(39, 174, 96);
                btnStock.ForeColor = Color.White;
                btnStock.FlatStyle = FlatStyle.Flat;
                btnStock.FlatAppearance.BorderSize = 0;
                btnStock.Cursor = Cursors.Hand;
                btnStock.Tag = ing.Id;
                btnStock.Click += BtnAddStock_Click;

                row.Controls.Add(btnEdit);
                row.Controls.Add(btnStock);
                pnlRows.Controls.Add(row);
            }
        }

        private void UpdateCard(int index, string value)
        {
            string[] names = { "pnlCardTotal", "pnlCardLow", "pnlCardOut", "pnlCardValue" };
            var card = FindControl<Panel>(names[index]);
            if (card == null) return;
            var lbl = card.Controls.OfType<Label>().FirstOrDefault(l => l.Name == $"lblVal_{index}");
            if (lbl != null) lbl.Text = value;
        }

        // ── Filter button ──
        private void BtnFilter_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            _currentFilter = btn.Tag?.ToString() ?? "All";

            foreach (var b in FindAllControls<Button>(this)
                .Where(b => b.Name.StartsWith("btnFilter_")))
            {
                b.BackColor = Color.FromArgb(240, 240, 240);
                b.ForeColor = Color.FromArgb(80, 80, 80);
            }

            btn.BackColor = Color.FromArgb(39, 174, 96);
            btn.ForeColor = Color.White;
            LoadIngredients();
        }

        // ── Add stock ──
        private void BtnAddStock_Click(object? sender, EventArgs e)
        {
            if (Session.CurrentEmployee?.Role == "kitchen")
            {
                MessageBox.Show("Kitchen staff can only view stock levels.",
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (sender is not Button btn) return;
            if (btn.Tag is not int id) return;

            using var db = new AppDbContext();
            var ing = db.Ingredients.Find(id);
            if (ing == null) return;

            string input = ShowInputBox(
                $"Add stock for {ing.Name}\nCurrent: {ing.StockQuantity} {ing.Unit}",
                "Add Stock", "");

            if (string.IsNullOrEmpty(input)) return;

            if (!decimal.TryParse(input.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid positive number.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ing.StockQuantity += amount;
            ing.UpdatedAt = DateTime.Now;

            db.StockMovements.Add(new StockMovement
            {
                IngredientId = id,
                Quantity = amount,
                MovementType = "purchase",
                MovedAt = DateTime.Now,
                Notes = $"Manual stock addition by {Session.CurrentEmployee?.FullName}"
            });

            db.SaveChanges();
            LoadIngredients();

            MessageBox.Show($"Added {amount} {ing.Unit} to {ing.Name}.",
                "Stock Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ── Edit ingredient ──
        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (Session.CurrentEmployee?.Role == "kitchen")
            {
                MessageBox.Show("Kitchen staff can only view stock levels.",
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (sender is not Button btn) return;
            if (btn.Tag is not int id) return;

            using var db = new AppDbContext();
            var ing = db.Ingredients.Include(i => i.Supplier).FirstOrDefault(i => i.Id == id);
            if (ing == null) return;

            var editForm = new Form();
            editForm.Text = $"Edit — {ing.Name}";
            editForm.Size = new Size(420, 380);
            editForm.StartPosition = FormStartPosition.CenterParent;
            editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            editForm.MaximizeBox = false;
            editForm.BackColor = Color.White;

            int y = 20;
            var fields = new (string Label, string Value, string Name)[]
            {
                ("Name",       ing.Name,                    "txtName"),
                ("Unit",       ing.Unit,                    "txtUnit"),
                ("Min Stock",  ing.MinStock.ToString(),     "txtMin"),
                ("Unit Price", ing.UnitPrice?.ToString()    ?? "0", "txtPrice"),
            };

            var textBoxes = new Dictionary<string, TextBox>();
            foreach (var (label, value, name) in fields)
            {
                var lbl = new Label();
                lbl.Text = label;
                lbl.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                lbl.ForeColor = Color.Gray;
                lbl.AutoSize = true;
                lbl.Location = new Point(20, y);
                editForm.Controls.Add(lbl);

                var txt = new TextBox();
                txt.Name = name;
                txt.Text = value;
                txt.Size = new Size(360, 28);
                txt.Location = new Point(20, y + 20);
                txt.Font = new Font("Segoe UI", 10);
                txt.BorderStyle = BorderStyle.FixedSingle;
                editForm.Controls.Add(txt);
                textBoxes[name] = txt;
                y += 60;
            }

            var btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Size = new Size(175, 40);
            btnSave.Location = new Point(20, y + 10);
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(39, 174, 96);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, ev) =>
            {
                using var db2 = new AppDbContext();
                var ing2 = db2.Ingredients.Find(id);
                if (ing2 == null) return;

                ing2.Name = textBoxes["txtName"].Text.Trim();
                ing2.Unit = textBoxes["txtUnit"].Text.Trim();

                if (decimal.TryParse(textBoxes["txtMin"].Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal min))
                    ing2.MinStock = min;

                if (decimal.TryParse(textBoxes["txtPrice"].Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal price))
                    ing2.UnitPrice = price;

                ing2.UpdatedAt = DateTime.Now;
                db2.SaveChanges();

                MessageBox.Show("Ingredient updated.", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                editForm.Close();
                LoadIngredients();
            };

            var btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(175, 40);
            btnCancel.Location = new Point(205, y + 10);
            btnCancel.Font = new Font("Segoe UI", 10);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Click += (s, ev) => editForm.Close();

            editForm.Controls.Add(btnSave);
            editForm.Controls.Add(btnCancel);
            editForm.ShowDialog(this);
        }

        // ── Add new ingredient ──
        private void BtnAddIngredient_Click(object? sender, EventArgs e)
        {
            using var db = new AppDbContext();
            var suppliers = db.Suppliers.Where(s => s.IsActive).ToList();

            var addForm = new Form();
            addForm.Text = "Add New Ingredient";
            addForm.Size = new Size(420, 480);
            addForm.StartPosition = FormStartPosition.CenterParent;
            addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addForm.MaximizeBox = false;
            addForm.BackColor = Color.White;

            int y = 20;
            var fields = new (string Label, string Default, string Name)[]
            {
                ("Name",          "",  "txtName"),
                ("Unit (kg/piece/litre/package)", "kg", "txtUnit"),
                ("Stock Quantity", "0", "txtStock"),
                ("Min Stock",      "0", "txtMin"),
                ("Unit Price",     "0", "txtPrice"),
            };

            var textBoxes = new Dictionary<string, TextBox>();
            foreach (var (label, def, name) in fields)
            {
                var lbl = new Label();
                lbl.Text = label;
                lbl.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                lbl.ForeColor = Color.Gray;
                lbl.AutoSize = true;
                lbl.Location = new Point(20, y);
                addForm.Controls.Add(lbl);

                var txt = new TextBox();
                txt.Name = name;
                txt.Text = def;
                txt.Size = new Size(360, 28);
                txt.Location = new Point(20, y + 20);
                txt.Font = new Font("Segoe UI", 10);
                txt.BorderStyle = BorderStyle.FixedSingle;
                addForm.Controls.Add(txt);
                textBoxes[name] = txt;
                y += 58;
            }

            // Supplier selection
            var lblSup = new Label();
            lblSup.Text = "Supplier";
            lblSup.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblSup.ForeColor = Color.Gray;
            lblSup.AutoSize = true;
            lblSup.Location = new Point(20, y);
            addForm.Controls.Add(lblSup);

            var cmbSupplier = new ComboBox();
            cmbSupplier.Size = new Size(360, 28);
            cmbSupplier.Location = new Point(20, y + 20);
            cmbSupplier.Font = new Font("Segoe UI", 10);
            cmbSupplier.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var sup in suppliers)
                cmbSupplier.Items.Add(new { Text = sup.Name, Value = sup.Id });
            cmbSupplier.DisplayMember = "Text";
            if (cmbSupplier.Items.Count > 0) cmbSupplier.SelectedIndex = 0;
            addForm.Controls.Add(cmbSupplier);
            y += 58;

            var btnSave = new Button();
            btnSave.Text = "Add Ingredient";
            btnSave.Size = new Size(175, 40);
            btnSave.Location = new Point(20, y + 10);
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(39, 174, 96);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrEmpty(textBoxes["txtName"].Text.Trim()))
                {
                    MessageBox.Show("Name cannot be empty.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal.TryParse(textBoxes["txtStock"].Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal stock);
                decimal.TryParse(textBoxes["txtMin"].Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal min);
                decimal.TryParse(textBoxes["txtPrice"].Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal price);

                int? supplierId = null;
                if (cmbSupplier.SelectedItem != null)
                {
                    dynamic selected = cmbSupplier.SelectedItem;
                    supplierId = (int)selected.Value;
                }

                using var db2 = new AppDbContext();
                db2.Ingredients.Add(new Ingredient
                {
                    Name = textBoxes["txtName"].Text.Trim(),
                    Unit = textBoxes["txtUnit"].Text.Trim(),
                    StockQuantity = stock,
                    MinStock = min,
                    UnitPrice = price,
                    SupplierId = supplierId,
                    UpdatedAt = DateTime.Now
                });
                db2.SaveChanges();

                MessageBox.Show("Ingredient added successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                addForm.Close();
                LoadIngredients();
            };

            var btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(175, 40);
            btnCancel.Location = new Point(205, y + 10);
            btnCancel.Font = new Font("Segoe UI", 10);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Click += (s, ev) => addForm.Close();

            addForm.Controls.Add(btnSave);
            addForm.Controls.Add(btnCancel);
            addForm.ShowDialog(this);
        }

        // ── Helper methods ──
        private static void AddLabel(Panel row, string text, Font font,
            Color color, int width, int x)
        {
            var lbl = new Label();
            lbl.Text = text;
            lbl.Font = font;
            lbl.ForeColor = color;
            lbl.Size = new Size(width, 48);
            lbl.Location = new Point(x, 0);
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            row.Controls.Add(lbl);
        }

        private T? FindControl<T>(string name) where T : Control
        {
            return FindAllControls<T>(this).FirstOrDefault(c => c.Name == name);
        }

        private static IEnumerable<T> FindAllControls<T>(Control parent) where T : Control
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is T match) yield return match;
                foreach (var child in FindAllControls<T>(ctrl))
                    yield return child;
            }
        }

        private static string ShowInputBox(string prompt, string title, string defaultValue)
        {
            var form = new Form { Width = 400, Height = 150, FormBorderStyle = FormBorderStyle.FixedDialog, Text = title, StartPosition = FormStartPosition.CenterParent, MaximizeBox = false, MinimizeBox = false, BackColor = Color.White };
            var lbl = new Label { Left = 20, Top = 20, Text = prompt, AutoSize = true, Font = new Font("Segoe UI", 10) };
            var txt = new TextBox { Left = 20, Top = 50, Width = 340, Text = defaultValue, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
            var btnOk = new Button { Text = "OK", Left = 205, Width = 80, Top = 85, FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Left = 295, Width = 80, Top = 85, FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };
            form.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;
            return form.ShowDialog() == DialogResult.OK ? txt.Text : "";
        }
    }
}