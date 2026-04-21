using System.Drawing;
using System.Windows.Forms;

namespace DonerApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ApplyDesign();
        }

        private void ApplyDesign()
        {
            // Form settings
            this.Text = "Doner Shop Management";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Top bar
            var pnlTop = new Panel();
            pnlTop.Size = new Size(1100, 60);
            pnlTop.Location = new Point(0, 0);
            pnlTop.BackColor = Color.FromArgb(230, 126, 34);

            var lblTitle = new Label();
            lblTitle.Text = "Doner Shop Management";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            var lblUser = new Label();
            lblUser.Name = "lblUser";
            lblUser.Text = "Welcome, User";
            lblUser.Font = new Font("Segoe UI", 10);
            lblUser.ForeColor = Color.FromArgb(255, 220, 180);
            lblUser.AutoSize = true;
            lblUser.Location = new Point(20, 20);

            var btnLogout = new Button();
            btnLogout.Name = "btnLogout";
            btnLogout.Text = "Logout";
            btnLogout.Size = new Size(80, 30);
            btnLogout.Location = new Point(990, 15);
            btnLogout.Font = new Font("Segoe UI", 9);
            btnLogout.ForeColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderColor = Color.White;
            btnLogout.Cursor = Cursors.Hand;

            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(lblUser);
            pnlTop.Controls.Add(btnLogout);

            // Menu grid
            var pnlGrid = new Panel();
            pnlGrid.Size = new Size(1060, 520);
            pnlGrid.Location = new Point(20, 80);
            pnlGrid.BackColor = Color.Transparent;

            string[] btnNames = { "btnTables", "btnOrders", "btnStock", "btnProducts", "btnReports", "btnEmployees", "btnSuppliers", "btnSettings" };
            string[] btnTexts = { "Table\nManagement", "Orders", "Stock\nTracking", "Products", "Reports", "Employees", "Suppliers", "Settings" };
            string[] btnIcons = { "🪑", "🧾", "📦", "🌯", "📊", "👤", "🚚", "⚙️" };
            Color[] btnColors = {
                Color.FromArgb(230, 126, 34),
                Color.FromArgb(41, 128, 185),
                Color.FromArgb(39, 174, 96),
                Color.FromArgb(142, 68, 173),
                Color.FromArgb(192, 57, 43),
                Color.FromArgb(22, 160, 133),
                Color.FromArgb(243, 156, 18),
                Color.FromArgb(127, 140, 141)
            };

            int cols = 4;
            int btnW = 240;
            int btnH = 200;
            int gap = 20;

            for (int i = 0; i < btnTexts.Length; i++)
            {
                int row = i / cols;
                int col = i % cols;

                var card = new Panel();
                card.Name = btnNames[i];
                card.Size = new Size(btnW, btnH);
                card.Location = new Point(col * (btnW + gap), row * (btnH + gap));
                card.BackColor = Color.White;
                card.Cursor = Cursors.Hand;

                // Top color bar
                var topBar = new Panel();
                topBar.Size = new Size(btnW, 6);
                topBar.Location = new Point(0, 0);
                topBar.BackColor = btnColors[i];

                // Icon
                var lblIcon = new Label();
                lblIcon.Text = btnIcons[i];
                lblIcon.Font = new Font("Segoe UI Emoji", 32);
                lblIcon.AutoSize = true;
                lblIcon.Location = new Point(85, 40);
                lblIcon.BackColor = Color.Transparent;

                // Text
                var lblText = new Label();
                lblText.Text = btnTexts[i];
                lblText.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                lblText.ForeColor = Color.FromArgb(50, 50, 50);
                lblText.Size = new Size(btnW - 20, 50);
                lblText.Location = new Point(10, 130);
                lblText.TextAlign = ContentAlignment.MiddleCenter;

                card.Controls.Add(topBar);
                card.Controls.Add(lblIcon);
                card.Controls.Add(lblText);
                pnlGrid.Controls.Add(card);
            }

            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlGrid);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Update user name
            var lblUser = FindAllControls<Label>(this)
    .FirstOrDefault(l => l.Name == "lblUser");
            if (lblUser != null && Session.CurrentEmployee != null)
            {
                lblUser.Text = $"Welcome, {Session.CurrentEmployee.FullName}  |  {Session.CurrentEmployee.Role.ToUpper()}";
                lblUser.AutoSize = true;
                // Sağa yasla — logout butonunun soluna
                lblUser.Location = new Point(985 - lblUser.PreferredWidth - 10, 20);
            }
            // Logout button
            var btnLogout = FindAllControls<Button>(this)
                .FirstOrDefault(l => l.Name == "btnLogout");
            if (btnLogout != null)
                btnLogout.Click += (s, args) =>
                {
                    var confirm = MessageBox.Show(
                        "Are you sure you want to logout?",
                        "Logout",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes) this.Close();
                };

            // Access control for menu cards
            var menuActions = new Dictionary<string, (string Module, Func<Form> Factory)>
                {
                    { "btnTables",    ("Tables",    () => new TableForm())    },
                    { "btnOrders",    ("Orders",    () => new OrderForm())    },
                    { "btnStock",     ("Stock",     () => new StockForm())    },
                    { "btnProducts",  ("Products",  () => new ProductForm())  },
                    { "btnReports",   ("Reports",   () => new ReportForm())   },
                    { "btnEmployees", ("Employees", () => new EmployeeForm()) },
                    { "btnSuppliers", ("Suppliers", () => new SupplierForm()) },
                    { "btnSettings", ("Settings", () => new SettingsForm()) },
                };

            foreach (var (name, info) in menuActions)
            {
                var card = FindAllControls<Panel>(this)
                    .FirstOrDefault(p => p.Name == name);
                if (card == null) continue;

                bool hasAccess = Permission.CanAccess(info.Module);

                // Dim the card if no access
                if (!hasAccess)
                {
                    card.BackColor = Color.FromArgb(245, 245, 245);
                    foreach (Control child in card.Controls)
                    {
                        if (child is Label lbl)
                            lbl.ForeColor = Color.FromArgb(180, 180, 180);
                        if (child is Panel bar && bar.Height == 6)
                            bar.BackColor = Color.FromArgb(200, 200, 200);
                    }
                }

                card.Click += (s, args) =>
                {
                    if (!Permission.CanAccess(info.Module))
                    { Permission.Deny(info.Module); return; }
                    info.Factory().Show();
                };

                foreach (Control child in card.Controls)
                {
                    child.Click += (s, args) =>
                    {
                        if (!Permission.CanAccess(info.Module))
                        { Permission.Deny(info.Module); return; }
                        info.Factory().Show();
                    };
                }
            }
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



        // Helper method to find any control by its name recursively
        private static T? FindControlByName<T>(Control parent, string name) where T : Control
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is T match && ctrl.Name == name)
                    return match;

                var found = FindControlByName<T>(ctrl, name);
                if (found != null) return found;
            }
            return null;
        }
    }
}