using System.Drawing;
using System.Windows.Forms;

namespace DonerApp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            ApplyDesign();
        }

        private void ApplyDesign()
        {
            // Form settings
            this.Text = "Login";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Left panel
            var pnlLeft = new Panel();
            pnlLeft.Size = new Size(340, 500);
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.BackColor = Color.FromArgb(230, 126, 34);

            var lblAppName = new Label();
            lblAppName.Text = "Doner Shop\nManagement";
            lblAppName.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblAppName.ForeColor = Color.White;
            lblAppName.Size = new Size(300, 120);
            lblAppName.Location = new Point(20, 160);

            var lblSlogan = new Label();
            lblSlogan.Text = "Fast · Reliable · Easy";
            lblSlogan.Font = new Font("Segoe UI", 11);
            lblSlogan.ForeColor = Color.FromArgb(255, 220, 180);
            lblSlogan.AutoSize = true;
            lblSlogan.Location = new Point(30, 290);

            pnlLeft.Controls.Add(lblAppName);
            pnlLeft.Controls.Add(lblSlogan);

            // Right panel
            var pnlRight = new Panel();
            pnlRight.Size = new Size(460, 500);
            pnlRight.Location = new Point(340, 0);
            pnlRight.BackColor = Color.White;

            var lblWelcome = new Label();
            lblWelcome.Text = "Welcome Back";
            lblWelcome.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblWelcome.ForeColor = Color.FromArgb(40, 40, 40);
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point(60, 80);

            var lblSub = new Label();
            lblSub.Text = "Please sign in to continue";
            lblSub.Font = new Font("Segoe UI", 10);
            lblSub.ForeColor = Color.Gray;
            lblSub.AutoSize = true;
            lblSub.Location = new Point(60, 120);

            var lblUsername = new Label();
            lblUsername.Text = "Username";
            lblUsername.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblUsername.ForeColor = Color.FromArgb(80, 80, 80);
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(60, 175);

            var txtUsername = new TextBox();
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(340, 36);
            txtUsername.Location = new Point(60, 198);
            txtUsername.Font = new Font("Segoe UI", 11);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.PlaceholderText = "Enter your username";

            var lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(80, 80, 80);
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(60, 250);

            var txtPassword = new TextBox();
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(340, 36);
            txtPassword.Location = new Point(60, 273);
            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.PasswordChar = '*';
            txtPassword.PlaceholderText = "Enter your password";

            var lblError = new Label();
            lblError.Name = "lblError";
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 9);
            lblError.ForeColor = Color.Red;
            lblError.AutoSize = true;
            lblError.Location = new Point(60, 318);

            var btnLogin = new Button();
            btnLogin.Name = "btnLogin";
            btnLogin.Text = "Sign In";
            btnLogin.Size = new Size(340, 44);
            btnLogin.Location = new Point(60, 345);
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogin.BackColor = Color.FromArgb(230, 126, 34);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;

            pnlRight.Controls.Add(lblWelcome);
            pnlRight.Controls.Add(lblSub);
            pnlRight.Controls.Add(lblUsername);
            pnlRight.Controls.Add(txtUsername);
            pnlRight.Controls.Add(lblPassword);
            pnlRight.Controls.Add(txtPassword);
            pnlRight.Controls.Add(lblError);
            pnlRight.Controls.Add(btnLogin);

            this.Controls.Add(pnlLeft);
            this.Controls.Add(pnlRight);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Bind the click event to the Sign In button
            var btnLogin = FindControlByName<Button>(this, "btnLogin");
            if (btnLogin != null)
                btnLogin.Click += BtnLogin_Click;

            // Allow login by pressing the Enter key
            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            var txtUsername = FindControlByName<TextBox>(this, "txtUsername");
            var txtPassword = FindControlByName<TextBox>(this, "txtPassword");
            var lblError = FindControlByName<Label>(this, "lblError");

            string username = txtUsername?.Text.Trim() ?? "";
            string password = txtPassword?.Text ?? "";

            // Empty field validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                if (lblError != null)
                    lblError.Text = "Username and password cannot be empty.";
                return;
            }

            using var db = new AppDbContext();

            var employee = db.Employees
                .FirstOrDefault(e => e.FullName == username && e.IsActive);

            if (employee == null)
            {
                if (lblError != null)
                    lblError.Text = "User not found or account is inactive.";
                return;
            }

            // Password hash must be set for security
            if (string.IsNullOrEmpty(employee.PasswordHash))
            {
                if (lblError != null)
                    lblError.Text = "Password not set. Contact manager.";
                return;
            }

            string hashedInput = HashPassword(password);
            if (hashedInput != employee.PasswordHash)
            {
                if (lblError != null)
                    lblError.Text = "Incorrect password.";

                if (txtPassword != null)
                    txtPassword.Clear();

                return;
            }

            Login(employee);
        }
        private void Login(Models.Employee employee)
        {
            Session.CurrentEmployee = employee;

            var mainForm = new MainForm();
            mainForm.Show();
            this.Hide();

            mainForm.FormClosed += (s, args) =>
            {
                Session.Logout();
                this.Show();
                var txtPassword = FindControlByName<TextBox>(this, "txtPassword");
                var txtUsername = FindControlByName<TextBox>(this, "txtUsername");
                var lblError = FindControlByName<Label>(this, "lblError");
                if (txtPassword != null) txtPassword.Clear();
                if (txtUsername != null) txtUsername.Clear();
                if (lblError != null) lblError.Text = "";
            };
        }

        public static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
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