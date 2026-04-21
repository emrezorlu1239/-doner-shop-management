using System.Drawing;
using System.Windows.Forms;

namespace DonerApp
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            ApplyDesign();
        }

        private void ApplyDesign()
        {
            this.Text = "Settings";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            var pnlTop = new Panel();
            pnlTop.Size = new Size(700, 60);
            pnlTop.Location = new Point(0, 0);
            pnlTop.BackColor = Color.FromArgb(127, 140, 141);

            var lblTitle = new Label();
            lblTitle.Text = "Settings";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            var btnBack = new Button();
            btnBack.Text = "← Back";
            btnBack.Size = new Size(90, 32);
            btnBack.Location = new Point(585, 14);
            btnBack.Font = new Font("Segoe UI", 9);
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.FlatAppearance.BorderColor = Color.White;
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => this.Close();

            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(btnBack);

            // Current user info
            var pnlInfo = new Panel();
            pnlInfo.Size = new Size(660, 100);
            pnlInfo.Location = new Point(20, 80);
            pnlInfo.BackColor = Color.White;

            var lblUser = new Label();
            lblUser.Text = $"Logged in as: {Session.CurrentEmployee?.FullName}";
            lblUser.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblUser.ForeColor = Color.FromArgb(40, 40, 40);
            lblUser.AutoSize = true;
            lblUser.Location = new Point(20, 20);

            var lblRole = new Label();
            lblRole.Text = $"Role: {Session.CurrentEmployee?.Role?.ToUpper()}";
            lblRole.Font = new Font("Segoe UI", 10);
            lblRole.ForeColor = Color.Gray;
            lblRole.AutoSize = true;
            lblRole.Location = new Point(20, 50);

            pnlInfo.Controls.Add(lblUser);
            pnlInfo.Controls.Add(lblRole);

            // Change password section
            var pnlPass = new Panel();
            pnlPass.Size = new Size(660, 220);
            pnlPass.Location = new Point(20, 200);
            pnlPass.BackColor = Color.White;

            var lblPassTitle = new Label();
            lblPassTitle.Text = "Change Password";
            lblPassTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblPassTitle.ForeColor = Color.FromArgb(40, 40, 40);
            lblPassTitle.AutoSize = true;
            lblPassTitle.Location = new Point(20, 20);

            var lblCurrent = new Label();
            lblCurrent.Text = "Current Password";
            lblCurrent.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblCurrent.ForeColor = Color.Gray;
            lblCurrent.AutoSize = true;
            lblCurrent.Location = new Point(20, 55);

            var txtCurrent = new TextBox();
            txtCurrent.Size = new Size(300, 28);
            txtCurrent.Location = new Point(20, 75);
            txtCurrent.Font = new Font("Segoe UI", 10);
            txtCurrent.BorderStyle = BorderStyle.FixedSingle;
            txtCurrent.PasswordChar = '*';

            var lblNew = new Label();
            lblNew.Text = "New Password";
            lblNew.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblNew.ForeColor = Color.Gray;
            lblNew.AutoSize = true;
            lblNew.Location = new Point(20, 115);

            var txtNew = new TextBox();
            txtNew.Size = new Size(300, 28);
            txtNew.Location = new Point(20, 135);
            txtNew.Font = new Font("Segoe UI", 10);
            txtNew.BorderStyle = BorderStyle.FixedSingle;
            txtNew.PasswordChar = '*';

            var btnChange = new Button();
            btnChange.Text = "Change Password";
            btnChange.Size = new Size(180, 40);
            btnChange.Location = new Point(20, 175);
            btnChange.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnChange.BackColor = Color.FromArgb(127, 140, 141);
            btnChange.ForeColor = Color.White;
            btnChange.FlatStyle = FlatStyle.Flat;
            btnChange.FlatAppearance.BorderSize = 0;
            btnChange.Cursor = Cursors.Hand;
            btnChange.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(txtCurrent.Text) || string.IsNullOrEmpty(txtNew.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var db = new AppDbContext();
                var emp = db.Employees.Find(Session.CurrentEmployee?.Id);
                if (emp == null) return;

                string currentHash = LoginForm.HashPassword(txtCurrent.Text);
                if (currentHash != emp.PasswordHash)
                {
                    MessageBox.Show("Current password is incorrect.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                emp.PasswordHash = LoginForm.HashPassword(txtNew.Text);
                db.SaveChanges();

                MessageBox.Show("Password changed successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCurrent.Clear();
                txtNew.Clear();
            };

            pnlPass.Controls.Add(lblPassTitle);
            pnlPass.Controls.Add(lblCurrent);
            pnlPass.Controls.Add(txtCurrent);
            pnlPass.Controls.Add(lblNew);
            pnlPass.Controls.Add(txtNew);
            pnlPass.Controls.Add(btnChange);

            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlInfo);
            this.Controls.Add(pnlPass);
        }
    }
}