using DonerApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Windows.Forms;

namespace DonerApp
{
    public partial class SupplierForm : Form
    {
        private Panel pnlRows = new Panel();
        private string _searchText = "";

        public SupplierForm()
        {
            InitializeComponent();
            if (!Permission.CanAccess("Suppliers"))
            {
                Permission.Deny("Suppliers");
                this.Load += (s, e) => this.Close();
                return;
            }
            ApplyDesign();
            LoadSuppliers();
        }

        private void ApplyDesign()
        {
            this.Text = "Supplier Management";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            var pnlTop = new Panel();
            pnlTop.Size = new Size(1100, 60);
            pnlTop.Location = new Point(0, 0);
            pnlTop.BackColor = Color.FromArgb(243, 156, 18);

            var lblTitle = new Label();
            lblTitle.Text = "Supplier Management";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            var btnAdd = new Button();
            btnAdd.Text = "+ Add Supplier";
            btnAdd.Size = new Size(130, 32);
            btnAdd.Location = new Point(840, 14);
            btnAdd.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderColor = Color.White;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += BtnAdd_Click;

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
            pnlTop.Controls.Add(btnAdd);
            pnlTop.Controls.Add(btnBack);

            // Summary cards
            string[] cardNames = { "pnlSCard0", "pnlSCard1", "pnlSCard2", "pnlSCard3" };
            string[] cardTitles = { "Total Suppliers", "Active", "Total Ingredients", "Last Purchase" };
            Color[] cardColors = {
                Color.FromArgb(243, 156, 18),
                Color.FromArgb(39,  174, 96),
                Color.FromArgb(41,  128, 185),
                Color.FromArgb(142, 68,  173)
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
                lblVal.Name = $"lblSVal_{i}";
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

            // Search
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
            txtSearch.PlaceholderText = "Search supplier...";
            txtSearch.TextChanged += (s, e) =>
            {
                _searchText = txtSearch.Text.Trim();
                LoadSuppliers();
            };
            pnlSearch.Controls.Add(txtSearch);

            // Header
            var pnlHeader = new Panel();
            pnlHeader.Size = new Size(1060, 36);
            pnlHeader.Location = new Point(20, 212);
            pnlHeader.BackColor = Color.FromArgb(245, 245, 245);

            string[] headers = { "Supplier", "Phone", "Address", "Tax No", "Ingredients", "Status", "Actions" };
            int[] widths = { 200, 140, 180, 120, 100, 90, 180 };
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

            pnlRows.Size = new Size(1060, 460);
            pnlRows.Location = new Point(20, 250);
            pnlRows.BackColor = Color.White;
            pnlRows.AutoScroll = true;

            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlRows);
        }

        private void LoadSuppliers()
        {
            pnlRows.Controls.Clear();

            using var db = new AppDbContext();
            var query = db.Suppliers.Include(s => s.Ingredients).AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
                query = query.Where(s => s.Name.ToLower().Contains(_searchText.ToLower()));

            var suppliers = query.OrderBy(s => s.Name).ToList();
            var all = db.Suppliers.ToList();

            // Cards
            UpdateCard(0, all.Count.ToString());
            UpdateCard(1, all.Count(s => s.IsActive).ToString());
            UpdateCard(2, db.Ingredients.Count().ToString());

            var lastMovement = db.StockMovements
                .OrderByDescending(m => m.MovedAt)
                .FirstOrDefault();
            UpdateCard(3, lastMovement?.MovedAt.ToString("dd MMM") ?? "—");

            Color[] avatarColors = {
                Color.FromArgb(192,57,43), Color.FromArgb(39,174,96),
                Color.FromArgb(41,128,185), Color.FromArgb(230,126,34),
                Color.FromArgb(142,68,173)
            };

            for (int i = 0; i < suppliers.Count; i++)
            {
                var sup = suppliers[i];
                var row = new Panel();
                row.Size = new Size(1040, 62);
                row.Location = new Point(0, i * 64);
                row.BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(250, 250, 250);

                // Avatar
                var avatar = new Panel();
                avatar.Size = new Size(38, 38);
                avatar.Location = new Point(10, 12);
                avatar.BackColor = avatarColors[i % avatarColors.Length];

                var lblInit = new Label();
                lblInit.Text = sup.Name[0].ToString().ToUpper();
                lblInit.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                lblInit.ForeColor = Color.White;
                lblInit.Size = new Size(38, 38);
                lblInit.TextAlign = ContentAlignment.MiddleCenter;
                avatar.Controls.Add(lblInit);

                var lName = MakeLabel(sup.Name, new Font("Segoe UI", 10, FontStyle.Bold),
                    Color.FromArgb(40, 40, 40), 150, 58);

                var lPhone = MakeLabel(sup.Phone ?? "—", new Font("Segoe UI", 9),
                    Color.FromArgb(60, 60, 60), 140, 218);

                var lAddr = MakeLabel(sup.Address ?? "—", new Font("Segoe UI", 9),
                    Color.Gray, 180, 358);

                var lTax = MakeLabel(sup.TaxNumber ?? "—", new Font("Segoe UI", 9),
                    Color.Gray, 120, 538);

                var lIng = MakeLabel($"{sup.Ingredients.Count} items",
                    new Font("Segoe UI", 9, FontStyle.Bold),
                    Color.FromArgb(41, 128, 185), 100, 658);

                var lStatus = MakeLabel(
                    sup.IsActive ? "ACTIVE" : "INACTIVE",
                    new Font("Segoe UI", 8, FontStyle.Bold),
                    sup.IsActive ? Color.FromArgb(39, 174, 96) : Color.Gray,
                    90, 758);

                // Edit butonu
                var btnEdit = new Button();
                btnEdit.Text = "Edit";
                btnEdit.Size = new Size(55, 30);
                btnEdit.Location = new Point(855, 16);
                btnEdit.Font = new Font("Segoe UI", 8);
                btnEdit.BackColor = Color.FromArgb(41, 128, 185);
                btnEdit.ForeColor = Color.White;
                btnEdit.FlatStyle = FlatStyle.Flat;
                btnEdit.FlatAppearance.BorderSize = 0;
                btnEdit.Cursor = Cursors.Hand;
                btnEdit.Tag = sup.Id;
                btnEdit.Click += BtnEdit_Click;

                // Toggle butonu
                var btnToggle = new Button();
                btnToggle.Text = sup.IsActive ? "Deactivate" : "Activate";
                btnToggle.Size = new Size(90, 30);
                btnToggle.Location = new Point(915, 16);
                btnToggle.Font = new Font("Segoe UI", 8);
                btnToggle.BackColor = sup.IsActive
                    ? Color.FromArgb(231, 76, 60)
                    : Color.FromArgb(39, 174, 96);
                btnToggle.ForeColor = Color.White;
                btnToggle.FlatStyle = FlatStyle.Flat;
                btnToggle.FlatAppearance.BorderSize = 0;
                btnToggle.Cursor = Cursors.Hand;
                btnToggle.Tag = sup.Id;
                btnToggle.Click += BtnToggle_Click;

                row.Controls.Add(avatar);
                row.Controls.Add(lName);
                row.Controls.Add(lPhone);
                row.Controls.Add(lAddr);
                row.Controls.Add(lTax);
                row.Controls.Add(lIng);
                row.Controls.Add(lStatus);
                row.Controls.Add(btnEdit);
                row.Controls.Add(btnToggle);
                pnlRows.Controls.Add(row);
            }
        }

        private static Label MakeLabel(string text, Font font, Color color, int width, int x)
        {
            var l = new Label();
            l.Text = text;
            l.Font = font;
            l.ForeColor = color;
            l.Size = new Size(width, 62);
            l.Location = new Point(x, 0);
            l.TextAlign = ContentAlignment.MiddleLeft;
            return l;
        }

        private void UpdateCard(int index, string value)
        {
            var lbl = FindAllControls<Label>(this)
                .FirstOrDefault(l => l.Name == $"lblSVal_{index}");
            if (lbl != null) lbl.Text = value;
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int id) return;

            using var db = new AppDbContext();
            var sup = db.Suppliers.Find(id);
            if (sup != null) ShowDialog(sup);
        }

        private void BtnToggle_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int id) return;

            using var db = new AppDbContext();
            var sup = db.Suppliers.Find(id);
            if (sup == null) return;
            sup.IsActive = !sup.IsActive;
            db.SaveChanges();
            LoadSuppliers();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            ShowDialog(null);
        }

        private void ShowDialog(Supplier? existing)
        {
            bool isNew = existing == null;
            var dlg = new Form();
            dlg.Text = isNew ? "Add Supplier" : $"Edit — {existing!.Name}";
            dlg.Size = new Size(420, 400);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.BackColor = Color.White;

            int y = 20;

            var fields = new (string Label, string Value, string Name)[]
            {
                ("Supplier Name", existing?.Name        ?? "", "txtName"),
                ("Phone",         existing?.Phone       ?? "", "txtPhone"),
                ("Address",       existing?.Address     ?? "", "txtAddress"),
                ("Tax Number",    existing?.TaxNumber   ?? "", "txtTax"),
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
                dlg.Controls.Add(lbl);

                var txt = new TextBox();
                txt.Name = name;
                txt.Text = value;
                txt.Size = new Size(360, 28);
                txt.Location = new Point(20, y + 20);
                txt.Font = new Font("Segoe UI", 10);
                txt.BorderStyle = BorderStyle.FixedSingle;
                dlg.Controls.Add(txt);
                textBoxes[name] = txt;
                y += 58;
            }

            var btnSave = new Button();
            btnSave.Text = isNew ? "Add Supplier" : "Save Changes";
            btnSave.Size = new Size(175, 40);
            btnSave.Location = new Point(20, y + 10);
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(243, 156, 18);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrEmpty(textBoxes["txtName"].Text.Trim()))
                {
                    MessageBox.Show("Supplier name cannot be empty.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var db = new AppDbContext();
                if (isNew)
                {
                    db.Suppliers.Add(new Supplier
                    {
                        Name = textBoxes["txtName"].Text.Trim(),
                        Phone = textBoxes["txtPhone"].Text.Trim(),
                        Address = textBoxes["txtAddress"].Text.Trim(),
                        TaxNumber = textBoxes["txtTax"].Text.Trim(),
                        IsActive = true
                    });
                    MessageBox.Show("Supplier added.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var s2 = db.Suppliers.Find(existing!.Id);
                    if (s2 != null)
                    {
                        s2.Name = textBoxes["txtName"].Text.Trim();
                        s2.Phone = textBoxes["txtPhone"].Text.Trim();
                        s2.Address = textBoxes["txtAddress"].Text.Trim();
                        s2.TaxNumber = textBoxes["txtTax"].Text.Trim();
                    }
                    MessageBox.Show("Supplier updated.", "Saved",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                db.SaveChanges();
                dlg.Close();
                LoadSuppliers();
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