using DonerApp.Models;
using System.Drawing;
using System.Windows.Forms;

namespace DonerApp
{
    public partial class EmployeeForm : Form
    {
        private Panel pnlRows = new Panel();
        private string _searchText = "";
        private string _currentRole = "All";

        public EmployeeForm()
        {
            InitializeComponent();
            if (!Permission.CanAccess("Employees"))
            {
                Permission.Deny("Employees");
                this.Load += (s, e) => this.Close();
                return;
            }
            ApplyDesign();
            LoadEmployees();
        }

        private void ApplyDesign()
        {
            this.Text = "Employee Management";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Top bar
            var pnlTop = new Panel();
            pnlTop.Size = new Size(1100, 60);
            pnlTop.Location = new Point(0, 0);
            pnlTop.BackColor = Color.FromArgb(22, 160, 133);

            var lblTitle = new Label();
            lblTitle.Text = "Employee Management";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            var btnAdd = new Button();
            btnAdd.Text = "+ Add Employee";
            btnAdd.Size = new Size(140, 32);
            btnAdd.Location = new Point(830, 14);
            btnAdd.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderColor = Color.White;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += BtnAddEmployee_Click;

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
            string[] cardNames = { "pnlCardTotal", "pnlCardActive", "pnlCardManager", "pnlCardKitchen" };
            string[] cardTitles = { "Total Employees", "Active", "Managers", "Kitchen Staff" };
            Color[] cardColors = {
                Color.FromArgb(22,  160, 133),
                Color.FromArgb(39,  174, 96),
                Color.FromArgb(41,  128, 185),
                Color.FromArgb(230, 126, 34)
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
                lblVal.Name = $"lblCardVal_{i}";
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

            // Search & role filter
            var pnlSearch = new Panel();
            pnlSearch.Size = new Size(1060, 44);
            pnlSearch.Location = new Point(20, 160);
            pnlSearch.BackColor = Color.White;

            var txtSearch = new TextBox();
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(280, 30);
            txtSearch.Location = new Point(10, 7);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.PlaceholderText = "Search employee...";
            txtSearch.TextChanged += (s, e) =>
            {
                _searchText = txtSearch.Text.Trim();
                LoadEmployees();
            };

            string[] roles = { "All", "Manager", "Cashier", "Waiter", "Kitchen" };
            for (int i = 0; i < roles.Length; i++)
            {
                var role = roles[i];
                var btn = new Button();
                btn.Name = $"btnRole_{role}";
                btn.Text = role;
                btn.Size = new Size(90, 30);
                btn.Location = new Point(305 + i * 96, 7);
                btn.Font = new Font("Segoe UI", 9);
                btn.BackColor = i == 0 ? Color.FromArgb(22, 160, 133) : Color.FromArgb(240, 240, 240);
                btn.ForeColor = i == 0 ? Color.White : Color.FromArgb(80, 80, 80);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Cursor = Cursors.Hand;
                btn.Tag = role;
                btn.Click += BtnRole_Click;
                pnlSearch.Controls.Add(btn);
            }

            pnlSearch.Controls.Add(txtSearch);

            // Table header
            var pnlHeader = new Panel();
            pnlHeader.Size = new Size(1060, 36);
            pnlHeader.Location = new Point(20, 212);
            pnlHeader.BackColor = Color.FromArgb(245, 245, 245);

            string[] headers = { "Employee", "Role", "Phone", "Hired Date", "Status", "Actions" };
            int[] widths = { 260, 120, 160, 140, 100, 240 };
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
            pnlRows.Size = new Size(1060, 450);
            pnlRows.Location = new Point(20, 250);
            pnlRows.BackColor = Color.White;
            pnlRows.AutoScroll = true;

            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlRows);
        }

        private void LoadEmployees()
        {
            pnlRows.Controls.Clear();

            using var db = new AppDbContext();
            var query = db.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
                query = query.Where(e => e.FullName.ToLower().Contains(_searchText.ToLower()));

            if (_currentRole != "All")
                query = query.Where(e => e.Role == _currentRole.ToLower());

            var employees = query.OrderBy(e => e.FullName).ToList();
            var all = db.Employees.ToList();

            UpdateCard(0, all.Count.ToString());
            UpdateCard(1, all.Count(e => e.IsActive).ToString());
            UpdateCard(2, all.Count(e => e.Role == "manager").ToString());
            UpdateCard(3, all.Count(e => e.Role == "kitchen").ToString());

            Color RoleColor(string r) => r switch
            {
                "manager" => Color.FromArgb(192, 57, 43),
                "cashier" => Color.FromArgb(41, 128, 185),
                "waiter" => Color.FromArgb(39, 174, 96),
                "kitchen" => Color.FromArgb(230, 126, 34),
                _ => Color.Gray
            };

            for (int i = 0; i < employees.Count; i++)
            {
                var emp = employees[i];

                var row = new Panel();
                row.Size = new Size(1040, 62);
                row.Location = new Point(0, i * 64);
                row.BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(250, 250, 250);

                // Avatar
                var avatar = new Panel();
                avatar.Size = new Size(38, 38);
                avatar.Location = new Point(10, 12);
                avatar.BackColor = RoleColor(emp.Role);

                var parts = emp.FullName.Split(' ');
                var initials = parts.Length >= 2
                    ? $"{parts[0][0]}{parts[1][0]}"
                    : $"{parts[0][0]}";

                var lblInit = new Label();
                lblInit.Text = initials.ToUpper();
                lblInit.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                lblInit.ForeColor = Color.White;
                lblInit.Size = new Size(38, 38);
                lblInit.TextAlign = ContentAlignment.MiddleCenter;
                avatar.Controls.Add(lblInit);

                // Name
                var lName = new Label();
                lName.Text = emp.FullName;
                lName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lName.ForeColor = emp.IsActive ? Color.FromArgb(40, 40, 40) : Color.Gray;
                lName.Size = new Size(200, 62);
                lName.Location = new Point(58, 0);
                lName.TextAlign = ContentAlignment.MiddleLeft;

                // Role
                var lRole = new Label();
                lRole.Text = emp.Role.ToUpper();
                lRole.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                lRole.ForeColor = RoleColor(emp.Role);
                lRole.Size = new Size(110, 62);
                lRole.Location = new Point(260, 0);
                lRole.TextAlign = ContentAlignment.MiddleLeft;

                // Phone
                var lPhone = new Label();
                lPhone.Text = emp.Phone ?? "—";
                lPhone.Font = new Font("Segoe UI", 9);
                lPhone.ForeColor = Color.FromArgb(60, 60, 60);
                lPhone.Size = new Size(150, 62);
                lPhone.Location = new Point(372, 0);
                lPhone.TextAlign = ContentAlignment.MiddleLeft;

                // Hired date
                var lHired = new Label();
                lHired.Text = emp.HiredAt?.ToString("dd MMM yyyy") ?? "—";
                lHired.Font = new Font("Segoe UI", 9);
                lHired.ForeColor = Color.Gray;
                lHired.Size = new Size(120, 62);
                lHired.Location = new Point(524, 0);
                lHired.TextAlign = ContentAlignment.MiddleLeft;

                // Status
                var lStatus = new Label();
                lStatus.Text = emp.IsActive ? "ACTIVE" : "INACTIVE";
                lStatus.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                lStatus.ForeColor = emp.IsActive ? Color.FromArgb(39, 174, 96) : Color.Gray;
                lStatus.Size = new Size(80, 62);
                lStatus.Location = new Point(646, 0);
                lStatus.TextAlign = ContentAlignment.MiddleLeft;

                // Edit
                var btnEdit = new Button();
                btnEdit.Text = "Edit";
                btnEdit.Size = new Size(52, 30);
                btnEdit.Location = new Point(730, 16);
                btnEdit.Font = new Font("Segoe UI", 8);
                btnEdit.BackColor = Color.FromArgb(41, 128, 185);
                btnEdit.ForeColor = Color.White;
                btnEdit.FlatStyle = FlatStyle.Flat;
                btnEdit.FlatAppearance.BorderSize = 0;
                btnEdit.Cursor = Cursors.Hand;
                btnEdit.Tag = emp.Id;
                btnEdit.Click += BtnEdit_Click;

                // Reset Password
                var btnReset = new Button();
                btnReset.Text = "Reset Pwd";
                btnReset.Size = new Size(82, 30);
                btnReset.Location = new Point(788, 16);
                btnReset.Font = new Font("Segoe UI", 8);
                btnReset.BackColor = Color.FromArgb(243, 156, 18);
                btnReset.ForeColor = Color.White;
                btnReset.FlatStyle = FlatStyle.Flat;
                btnReset.FlatAppearance.BorderSize = 0;
                btnReset.Cursor = Cursors.Hand;
                btnReset.Tag = emp.Id;
                btnReset.Click += BtnResetPassword_Click;

                // Deactivate / Activate
                var btnToggle = new Button();
                btnToggle.Text = emp.IsActive ? "Deactivate" : "Activate";
                btnToggle.Size = new Size(88, 30);
                btnToggle.Location = new Point(876, 16);
                btnToggle.Font = new Font("Segoe UI", 8);
                btnToggle.BackColor = emp.IsActive
                    ? Color.FromArgb(231, 76, 60)
                    : Color.FromArgb(39, 174, 96);
                btnToggle.ForeColor = Color.White;
                btnToggle.FlatStyle = FlatStyle.Flat;
                btnToggle.FlatAppearance.BorderSize = 0;
                btnToggle.Cursor = Cursors.Hand;
                btnToggle.Tag = emp.Id;
                btnToggle.Click += BtnToggleActive_Click;

                row.Controls.Add(avatar);
                row.Controls.Add(lName);
                row.Controls.Add(lRole);
                row.Controls.Add(lPhone);
                row.Controls.Add(lHired);
                row.Controls.Add(lStatus);
                row.Controls.Add(btnEdit);
                row.Controls.Add(btnReset);
                row.Controls.Add(btnToggle);
                pnlRows.Controls.Add(row);
            }
        }

        private void UpdateCard(int index, string value)
        {
            var lbl = FindAllControls<Label>(this)
                .FirstOrDefault(l => l.Name == $"lblCardVal_{index}");
            if (lbl != null) lbl.Text = value;
        }

        // ── Role filter ──
        private void BtnRole_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            _currentRole = btn.Tag?.ToString() ?? "All";

            foreach (var b in FindAllControls<Button>(this)
                .Where(b => b.Name.StartsWith("btnRole_")))
            {
                b.BackColor = Color.FromArgb(240, 240, 240);
                b.ForeColor = Color.FromArgb(80, 80, 80);
            }

            btn.BackColor = Color.FromArgb(22, 160, 133);
            btn.ForeColor = Color.White;
            LoadEmployees();
        }

        // ── Add employee ──
        private void BtnAddEmployee_Click(object? sender, EventArgs e)
        {
            ShowEmployeeDialog(null);
        }

        // ── Edit employee ──
        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int id) return;

            using var db = new AppDbContext();
            var emp = db.Employees.Find(id);
            if (emp != null) ShowEmployeeDialog(emp);
        }

        private void ShowEmployeeDialog(Employee? existing)
        {
            bool isNew = existing == null;

            var dlg = new Form();
            dlg.Text = isNew ? "Add Employee" : $"Edit — {existing!.FullName}";
            dlg.Size = new Size(420, 420);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.BackColor = Color.White;

            int y = 20;

            // Full Name
            AddFieldLabel(dlg, "Full Name", ref y);
            var txtName = AddFieldTextBox(dlg, existing?.FullName ?? "", ref y);

            // Role
            AddFieldLabel(dlg, "Role", ref y);
            var cmbRole = new ComboBox();
            cmbRole.Size = new Size(360, 28);
            cmbRole.Location = new Point(20, y);
            cmbRole.Font = new Font("Segoe UI", 10);
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.Items.AddRange(new object[] { "manager", "cashier", "waiter", "kitchen" });
            cmbRole.SelectedItem = existing?.Role ?? "waiter";
            dlg.Controls.Add(cmbRole);
            y += 50;

            // Phone
            AddFieldLabel(dlg, "Phone", ref y);
            var txtPhone = AddFieldTextBox(dlg, existing?.Phone ?? "", ref y);

            // Password (only for new employee)
            TextBox? txtPassword = null;
            if (isNew)
            {
                AddFieldLabel(dlg, "Password", ref y);
                txtPassword = AddFieldTextBox(dlg, "", ref y);
                txtPassword.PasswordChar = '*';
            }

            // Save button
            var btnSave = new Button();
            btnSave.Text = isNew ? "Add Employee" : "Save Changes";
            btnSave.Size = new Size(175, 40);
            btnSave.Location = new Point(20, y + 10);
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(22, 160, 133);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrEmpty(txtName.Text.Trim()))
                {
                    MessageBox.Show("Name cannot be empty.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var db = new AppDbContext();

                if (isNew)
                {
                    string hash = string.IsNullOrEmpty(txtPassword?.Text)
                        ? LoginForm.HashPassword("1234")
                        : LoginForm.HashPassword(txtPassword.Text);

                    db.Employees.Add(new Employee
                    {
                        FullName = txtName.Text.Trim(),
                        Role = cmbRole.SelectedItem?.ToString() ?? "waiter",
                        Phone = txtPhone.Text.Trim(),
                        IsActive = true,
                        HiredAt = DateOnly.FromDateTime(DateTime.Now),
                        PasswordHash = hash
                    });

                    MessageBox.Show("Employee added successfully.\nDefault password: 1234",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var emp = db.Employees.Find(existing!.Id);
                    if (emp != null)
                    {
                        emp.FullName = txtName.Text.Trim();
                        emp.Role = cmbRole.SelectedItem?.ToString() ?? emp.Role;
                        emp.Phone = txtPhone.Text.Trim();
                    }

                    MessageBox.Show("Employee updated.", "Saved",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                db.SaveChanges();
                dlg.Close();
                LoadEmployees();
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

        // ── Reset password ──
        private void BtnResetPassword_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int id) return;

            using var db = new AppDbContext();
            var emp = db.Employees.Find(id);
            if (emp == null) return;

            string newPass = Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter new password for {emp.FullName}:",
                "Reset Password", "");

            if (string.IsNullOrEmpty(newPass)) return;

            emp.PasswordHash = LoginForm.HashPassword(newPass);
            db.SaveChanges();

            MessageBox.Show($"Password reset for {emp.FullName}.",
                "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ── Active / Inactive toggle ──
        private void BtnToggleActive_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int id) return;

            if (id == Session.CurrentEmployee?.Id)
            {
                MessageBox.Show("You cannot deactivate your own account.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var db = new AppDbContext();
            var emp = db.Employees.Find(id);
            if (emp == null) return;

            emp.IsActive = !emp.IsActive;
            db.SaveChanges();
            LoadEmployees();
        }

        // ── Helper methods ──
        private static void AddFieldLabel(Form form, string text, ref int y)
        {
            var lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lbl.ForeColor = Color.Gray;
            lbl.AutoSize = true;
            lbl.Location = new Point(20, y);
            form.Controls.Add(lbl);
            y += 20;
        }

        private static TextBox AddFieldTextBox(Form form, string value, ref int y)
        {
            var txt = new TextBox();
            txt.Text = value;
            txt.Size = new Size(360, 28);
            txt.Location = new Point(20, y);
            txt.Font = new Font("Segoe UI", 10);
            txt.BorderStyle = BorderStyle.FixedSingle;
            form.Controls.Add(txt);
            y += 50;
            return txt;
        }

        private void EmployeeForm_Load(object sender, EventArgs e)
        {

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