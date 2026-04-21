using DonerApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Windows.Forms;

namespace DonerApp
{
    public partial class TableForm : Form
    {
        private Panel pnlGrid = new Panel();

        public TableForm()
        {
            InitializeComponent();
            ApplyDesign();
            LoadTables();
        }

        private void ApplyDesign()
        {
            // Access control
            if (!Permission.CanAccess("Tables"))
            {
                Permission.Deny("Tables");
                this.Load += (s, e) => this.Close();
                return;
            }

            this.Text = "Table Management";
            this.Size = new Size(1100, 700);
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
            lblTitle.Text = "Table Management";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            var btnRefresh = new Button();
            btnRefresh.Text = "↻ Refresh";
            btnRefresh.Size = new Size(90, 32);
            btnRefresh.Location = new Point(880, 14);
            btnRefresh.Font = new Font("Segoe UI", 9);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderColor = Color.White;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadTables();

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
            pnlTop.Controls.Add(btnRefresh);
            pnlTop.Controls.Add(btnBack);

            // Legend
            var pnlLegend = new Panel();
            pnlLegend.Size = new Size(1060, 40);
            pnlLegend.Location = new Point(20, 70);
            pnlLegend.BackColor = Color.Transparent;

            string[] legendTexts = { "Available", "Occupied", "Reserved", "Cleaning" };
            Color[] legendColors = {
                Color.FromArgb(39,  174, 96),
                Color.FromArgb(231, 76,  60),
                Color.FromArgb(243, 156, 18),
                Color.FromArgb(127, 140, 141)
            };

            for (int i = 0; i < legendTexts.Length; i++)
            {
                var dot = new Panel();
                dot.Size = new Size(14, 14);
                dot.Location = new Point(i * 160, 13);
                dot.BackColor = legendColors[i];

                var lbl = new Label();
                lbl.Text = legendTexts[i];
                lbl.Font = new Font("Segoe UI", 9);
                lbl.ForeColor = Color.FromArgb(80, 80, 80);
                lbl.AutoSize = true;
                lbl.Location = new Point(i * 160 + 20, 12);

                pnlLegend.Controls.Add(dot);
                pnlLegend.Controls.Add(lbl);
            }

            // Grid panel
            pnlGrid.Size = new Size(1060, 540);
            pnlGrid.Location = new Point(20, 118);
            pnlGrid.BackColor = Color.Transparent;
            pnlGrid.AutoScroll = true;

            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlLegend);
            this.Controls.Add(pnlGrid);
        }

        private void LoadTables()
        {
            pnlGrid.Controls.Clear();

            using var db = new AppDbContext();
            var tables = db.RestaurantTables
                .OrderBy(t => t.TableNumber)
                .ToList();

            int cols = 5;
            int cardW = 190;
            int cardH = 160;
            int gap = 16;

            foreach (var table in tables)
            {
                int i = table.TableNumber - 1;
                int row = i / cols;
                int col = i % cols;

                Color statusColor = GetStatusColor(table.Status);
                bool isAvailable = table.Status == "available";

                var card = new Panel();
                card.Size = new Size(cardW, cardH);
                card.Location = new Point(col * (cardW + gap), row * (cardH + gap));
                card.BackColor = Color.White;
                card.Cursor = Cursors.Hand;
                card.Tag = table.Id;

                var topBar = new Panel();
                topBar.Size = new Size(cardW, 6);
                topBar.Location = new Point(0, 0);
                topBar.BackColor = statusColor;

                var lblNum = new Label();
                lblNum.Text = $"Table {table.TableNumber}";
                lblNum.Font = new Font("Segoe UI", 13, FontStyle.Bold);
                lblNum.ForeColor = Color.FromArgb(40, 40, 40);
                lblNum.AutoSize = true;
                lblNum.Location = new Point(15, 22);

                var lblCap = new Label();
                lblCap.Text = $"👤 {table.SeatCapacity} seats";
                lblCap.Font = new Font("Segoe UI", 9);
                lblCap.ForeColor = Color.Gray;
                lblCap.AutoSize = true;
                lblCap.Location = new Point(15, 50);

                var lblStatus = new Label();
                lblStatus.Text = table.Status.ToUpper();
                lblStatus.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                lblStatus.ForeColor = statusColor;
                lblStatus.AutoSize = true;
                lblStatus.Location = new Point(15, 75);

                var btnAction = new Button();
                btnAction.Text = table.Status switch
                {
                    "available" => "Open Order",
                    "occupied" => "View Order",
                    "cleaning" => "Mark as Clean",
                    _ => "View"
                };
                btnAction.Size = new Size(160, 30);
                btnAction.Location = new Point(15, 115);
                btnAction.Font = new Font("Segoe UI", 9);
                btnAction.BackColor = table.Status switch
                {
                    "available" => Color.FromArgb(230, 126, 34),
                    "occupied" => Color.FromArgb(41, 128, 185),
                    "cleaning" => Color.FromArgb(127, 140, 141),
                    _ => Color.Gray
                };
                btnAction.ForeColor = Color.White;
                btnAction.FlatStyle = FlatStyle.Flat;
                btnAction.FlatAppearance.BorderSize = 0;
                btnAction.Cursor = Cursors.Hand;
                btnAction.Tag = table.Id;
                btnAction.Click += BtnAction_Click;

                card.Controls.Add(topBar);
                card.Controls.Add(lblNum);
                card.Controls.Add(lblCap);
                card.Controls.Add(lblStatus);
                card.Controls.Add(btnAction);
                pnlGrid.Controls.Add(card);
            }
        }

        private void BtnAction_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int tableId) return;

            using var db = new AppDbContext();
            var table = db.RestaurantTables.Find(tableId);
            if (table == null) return;

            if (table.Status == "available")
            {
                var confirm = MessageBox.Show(
                    $"Open new order for Table {table.TableNumber}?",
                    "Open Order",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                var newOrder = new Order
                {
                    TableId = table.Id,
                    EmployeeId = Session.CurrentEmployee?.Id,
                    OpenedAt = DateTime.Now,
                    Status = "open",
                    TotalAmount = 0
                };

                db.Orders.Add(newOrder);
                db.SaveChanges();

                table.Status = "occupied";
                table.ActiveOrderId = newOrder.Id;
                db.SaveChanges();

                MessageBox.Show(
                    $"Order opened for Table {table.TableNumber}!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadTables();
            }
            else if (table.Status == "occupied")
            {
                var order = db.Orders
                    .FirstOrDefault(o => o.TableId == tableId && o.Status == "open");

                if (order == null)
                {
                    MessageBox.Show("No active order found.",
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var orderForm = new OrderForm(order.Id, table.TableNumber);
                orderForm.FormClosed += (s, args) => LoadTables();
                orderForm.Show();
            }
            else if (table.Status == "cleaning")
            {
                var confirm = MessageBox.Show(
                    $"Mark Table {table.TableNumber} as available?",
                    "Table Ready",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                table.Status = "available";
                db.SaveChanges();

                LoadTables();
            }
        }

        private static Color GetStatusColor(string status) => status switch
        {
            "occupied" => Color.FromArgb(231, 76, 60),
            "reserved" => Color.FromArgb(243, 156, 18),
            "cleaning" => Color.FromArgb(127, 140, 141),
            _ => Color.FromArgb(39, 174, 96)
        };
    }
}