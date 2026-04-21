using DonerApp.Models;
using System.Drawing;
using System.Windows.Forms;

namespace DonerApp
{
    public partial class ProductForm : Form
    {
        private Panel pnlRows = new Panel();
        private string _currentCategory = "All";

        public ProductForm()
        {
            InitializeComponent();
            if (!Permission.CanAccess("Products"))
            {
                Permission.Deny("Products");
                this.Load += (s, e) => this.Close();
                return;
            }
            ApplyDesign();
            LoadProducts();
        }

        private void ApplyDesign()
        {
            this.Text = "Product Management";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            var pnlTop = new Panel();
            pnlTop.Size = new Size(1100, 60);
            pnlTop.Location = new Point(0, 0);
            pnlTop.BackColor = Color.FromArgb(142, 68, 173);

            var lblTitle = new Label();
            lblTitle.Text = "Product Management";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            var btnAddProduct = new Button();
            btnAddProduct.Text = "+ Add Product";
            btnAddProduct.Size = new Size(130, 32);
            btnAddProduct.Location = new Point(840, 14);
            btnAddProduct.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAddProduct.ForeColor = Color.White;
            btnAddProduct.FlatStyle = FlatStyle.Flat;
            btnAddProduct.FlatAppearance.BorderColor = Color.White;
            btnAddProduct.Cursor = Cursors.Hand;
            btnAddProduct.Click += BtnAddProduct_Click;

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
            pnlTop.Controls.Add(btnAddProduct);
            pnlTop.Controls.Add(btnBack);

            // Category tabs
            var pnlTabs = new Panel();
            pnlTabs.Size = new Size(1060, 44);
            pnlTabs.Location = new Point(20, 68);
            pnlTabs.BackColor = Color.Transparent;

            string[] tabs = { "All", "Doner", "Sandwich", "Portion", "Beverage", "Extra", "Other" };
            Color[] tabColors = {
                Color.FromArgb(142, 68, 173), Color.FromArgb(41,128,185),
                Color.FromArgb(39,174,96),    Color.FromArgb(142,68,173),
                Color.FromArgb(22,160,133),   Color.FromArgb(243,156,18),
                Color.FromArgb(127,140,141)
            };

            for (int i = 0; i < tabs.Length; i++)
            {
                var tab = tabs[i];
                var btn = new Button();
                btn.Name = $"btnTab_{tab}";
                btn.Text = tab;
                btn.Size = new Size(110, 36);
                btn.Location = new Point(i * 116, 4);
                btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                btn.BackColor = i == 0 ? tabColors[i] : Color.White;
                btn.ForeColor = i == 0 ? Color.White : Color.FromArgb(80, 80, 80);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = i == 0 ? 0 : 1;
                btn.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                btn.Cursor = Cursors.Hand;
                btn.Tag = tab;
                btn.Click += BtnTab_Click;
                pnlTabs.Controls.Add(btn);
            }

            // Header
            var pnlHeader = new Panel();
            pnlHeader.Size = new Size(1060, 36);
            pnlHeader.Location = new Point(20, 120);
            pnlHeader.BackColor = Color.FromArgb(245, 245, 245);

            string[] headers = { "Product Name", "Category", "Price", "Status", "Description", "Actions" };
            int[] widths = { 220, 110, 90, 90, 300, 200 };
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

            pnlRows.Size = new Size(1060, 540);
            pnlRows.Location = new Point(20, 158);
            pnlRows.BackColor = Color.White;
            pnlRows.AutoScroll = true;

            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlTabs);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlRows);
        }

        private void LoadProducts()
        {
            pnlRows.Controls.Clear();

            using var db = new AppDbContext();
            var query = db.Products.AsQueryable();

            if (_currentCategory != "All")
                query = query.Where(p => p.Category == _currentCategory.ToLower());

            var products = query.OrderBy(p => p.Category).ThenBy(p => p.Name).ToList();

            Color CatColor(string c) => c switch
            {
                "doner" => Color.FromArgb(41, 128, 185),
                "sandwich" => Color.FromArgb(39, 174, 96),
                "portion" => Color.FromArgb(142, 68, 173),
                "beverage" => Color.FromArgb(22, 160, 133),
                "extra" => Color.FromArgb(243, 156, 18),
                _ => Color.Gray
            };

            for (int i = 0; i < products.Count; i++)
            {
                var p = products[i];
                var row = new Panel();
                row.Size = new Size(1040, 52);
                row.Location = new Point(0, i * 54);
                row.BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(250, 250, 250);

                var lName = new Label();
                lName.Text = p.Name;
                lName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lName.ForeColor = Color.FromArgb(40, 40, 40);
                lName.Size = new Size(220, 52);
                lName.Location = new Point(10, 0);
                lName.TextAlign = ContentAlignment.MiddleLeft;

                var lCat = new Label();
                lCat.Text = p.Category.ToUpper();
                lCat.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                lCat.ForeColor = CatColor(p.Category);
                lCat.Size = new Size(110, 52);
                lCat.Location = new Point(230, 0);
                lCat.TextAlign = ContentAlignment.MiddleLeft;

                var lPrice = new Label();
                lPrice.Text = $"₺{p.Price:0.00}";
                lPrice.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lPrice.ForeColor = Color.FromArgb(230, 126, 34);
                lPrice.Size = new Size(90, 52);
                lPrice.Location = new Point(340, 0);
                lPrice.TextAlign = ContentAlignment.MiddleLeft;

                var lStatus = new Label();
                lStatus.Text = p.IsActive ? "ACTIVE" : "INACTIVE";
                lStatus.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                lStatus.ForeColor = p.IsActive ? Color.FromArgb(39, 174, 96) : Color.Gray;
                lStatus.Size = new Size(90, 52);
                lStatus.Location = new Point(430, 0);
                lStatus.TextAlign = ContentAlignment.MiddleLeft;

                var lDesc = new Label();
                lDesc.Text = p.Description ?? "—";
                lDesc.Font = new Font("Segoe UI", 8);
                lDesc.ForeColor = Color.Gray;
                lDesc.Size = new Size(290, 52);
                lDesc.Location = new Point(520, 0);
                lDesc.TextAlign = ContentAlignment.MiddleLeft;

                var btnEdit = new Button();
                btnEdit.Text = "Edit";
                btnEdit.Size = new Size(55, 30);
                btnEdit.Location = new Point(818, 11);
                btnEdit.Font = new Font("Segoe UI", 8);
                btnEdit.BackColor = Color.FromArgb(41, 128, 185);
                btnEdit.ForeColor = Color.White;
                btnEdit.FlatStyle = FlatStyle.Flat;
                btnEdit.FlatAppearance.BorderSize = 0;
                btnEdit.Cursor = Cursors.Hand;
                btnEdit.Tag = p.Id;
                btnEdit.Click += BtnEdit_Click;

                var btnToggle = new Button();
                btnToggle.Text = p.IsActive ? "Disable" : "Enable";
                btnToggle.Size = new Size(70, 30);
                btnToggle.Location = new Point(878, 11);
                btnToggle.Font = new Font("Segoe UI", 8);
                btnToggle.BackColor = p.IsActive
                    ? Color.FromArgb(231, 76, 60)
                    : Color.FromArgb(39, 174, 96);
                btnToggle.ForeColor = Color.White;
                btnToggle.FlatStyle = FlatStyle.Flat;
                btnToggle.FlatAppearance.BorderSize = 0;
                btnToggle.Cursor = Cursors.Hand;
                btnToggle.Tag = p.Id;
                btnToggle.Click += BtnToggle_Click;

                row.Controls.Add(lName);
                row.Controls.Add(lCat);
                row.Controls.Add(lPrice);
                row.Controls.Add(lStatus);
                row.Controls.Add(lDesc);
                row.Controls.Add(btnEdit);
                row.Controls.Add(btnToggle);
                pnlRows.Controls.Add(row);
            }
        }

        private void BtnTab_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            _currentCategory = btn.Tag?.ToString() ?? "All";

            foreach (var b in FindAllControls<Button>(this)
                .Where(b => b.Name.StartsWith("btnTab_")))
            {
                b.BackColor = Color.White;
                b.ForeColor = Color.FromArgb(80, 80, 80);
                b.FlatAppearance.BorderSize = 1;
            }
            btn.BackColor = Color.FromArgb(142, 68, 173);
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderSize = 0;
            LoadProducts();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int id) return;

            using var db = new AppDbContext();
            var p = db.Products.Find(id);
            if (p == null) return;

            ShowProductDialog(p);
        }

        private void BtnToggle_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int id) return;

            using var db = new AppDbContext();
            var p = db.Products.Find(id);
            if (p == null) return;
            p.IsActive = !p.IsActive;
            db.SaveChanges();
            LoadProducts();
        }

        private void BtnAddProduct_Click(object? sender, EventArgs e)
        {
            ShowProductDialog(null);
        }

        private void ShowProductDialog(Product? existing)
        {
            bool isNew = existing == null;
            var dlg = new Form();
            dlg.Text = isNew ? "Add Product" : $"Edit — {existing!.Name}";
            dlg.Size = new Size(420, 440);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.BackColor = Color.White;

            int y = 20;

            // Name
            AddLabel(dlg, "Product Name", ref y);
            var txtName = AddTextBox(dlg, existing?.Name ?? "", ref y);

            // Category
            AddLabel(dlg, "Category", ref y);
            var cmbCat = new ComboBox();
            cmbCat.Size = new Size(360, 28);
            cmbCat.Location = new Point(20, y);
            cmbCat.Font = new Font("Segoe UI", 10);
            cmbCat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCat.Items.AddRange(new object[] { "doner", "sandwich", "portion", "beverage", "extra", "other" });
            cmbCat.SelectedItem = existing?.Category ?? "doner";
            dlg.Controls.Add(cmbCat);
            y += 50;

            // Price
            AddLabel(dlg, "Price (₺)", ref y);
            var txtPrice = AddTextBox(dlg, existing?.Price.ToString() ?? "0", ref y);

            // Description
            AddLabel(dlg, "Description", ref y);
            var txtDesc = AddTextBox(dlg, existing?.Description ?? "", ref y);

            var btnSave = new Button();
            btnSave.Text = isNew ? "Add Product" : "Save Changes";
            btnSave.Size = new Size(175, 40);
            btnSave.Location = new Point(20, y + 10);
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(142, 68, 173);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrEmpty(txtName.Text.Trim()))
                {
                    MessageBox.Show("Product name cannot be empty.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal price))
                {
                    MessageBox.Show("Invalid price.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var db = new AppDbContext();
                if (isNew)
                {
                    db.Products.Add(new Product
                    {
                        Name = txtName.Text.Trim(),
                        Category = cmbCat.SelectedItem?.ToString() ?? "other",
                        Price = price,
                        Description = txtDesc.Text.Trim(),
                        IsActive = true
                    });
                    MessageBox.Show("Product added.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var prod = db.Products.Find(existing!.Id);
                    if (prod != null)
                    {
                        prod.Name = txtName.Text.Trim();
                        prod.Category = cmbCat.SelectedItem?.ToString() ?? prod.Category;
                        prod.Price = price;
                        prod.Description = txtDesc.Text.Trim();
                    }
                    MessageBox.Show("Product updated.", "Saved",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                db.SaveChanges();
                dlg.Close();
                LoadProducts();
            };

            var btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(175, 40);
            btnCancel.Location = new Point(205, y + 10);
            btnCancel.Font = new Font("Segoe UI", 10);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Click += (s, ev) => dlg.Close();

            dlg.Controls.Add(btnSave);
            dlg.Controls.Add(btnCancel);
            dlg.ShowDialog(this);
        }

        private static void AddLabel(Form f, string text, ref int y)
        {
            var l = new Label();
            l.Text = text;
            l.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            l.ForeColor = Color.Gray;
            l.AutoSize = true;
            l.Location = new Point(20, y);
            f.Controls.Add(l);
            y += 20;
        }

        private static TextBox AddTextBox(Form f, string val, ref int y)
        {
            var t = new TextBox();
            t.Text = val;
            t.Size = new Size(360, 28);
            t.Location = new Point(20, y);
            t.Font = new Font("Segoe UI", 10);
            t.BorderStyle = BorderStyle.FixedSingle;
            f.Controls.Add(t);
            y += 50;
            return t;
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
    }
}