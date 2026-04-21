using DonerApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Windows.Forms;

namespace DonerApp
{
    public partial class ReportForm : Form
    {
        private Panel pnlRows = new Panel();
        private Panel pnlTopProducts = new Panel();

        public ReportForm()
        {
            InitializeComponent();
            if (!Permission.CanAccess("Reports"))
            {
                Permission.Deny("Reports");
                this.Load += (s, e) => this.Close();
                return;
            }
            ApplyDesign();
            LoadReports(DateTime.Now.Date, DateTime.Now.Date);
        }

        private void ApplyDesign()
        {
            this.Text = "Reports";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            var pnlTop = new Panel();
            pnlTop.Size = new Size(1100, 60);
            pnlTop.Location = new Point(0, 0);
            pnlTop.BackColor = Color.FromArgb(192, 57, 43);

            var lblTitle = new Label();
            lblTitle.Text = "Reports & Analytics";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

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
            pnlTop.Controls.Add(btnBack);

            // Filter bar
            var pnlFilter = new Panel();
            pnlFilter.Size = new Size(1060, 50);
            pnlFilter.Location = new Point(20, 68);
            pnlFilter.BackColor = Color.White;

            var lblFrom = new Label();
            lblFrom.Text = "From:";
            lblFrom.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblFrom.ForeColor = Color.Gray;
            lblFrom.AutoSize = true;
            lblFrom.Location = new Point(15, 15);

            var dtFrom = new DateTimePicker();
            dtFrom.Name = "dtFrom";
            dtFrom.Size = new Size(160, 28);
            dtFrom.Location = new Point(65, 12);
            dtFrom.Font = new Font("Segoe UI", 9);
            dtFrom.Value = DateTime.Now.AddDays(-7);

            var lblTo = new Label();
            lblTo.Text = "To:";
            lblTo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblTo.ForeColor = Color.Gray;
            lblTo.AutoSize = true;
            lblTo.Location = new Point(237, 15);

            var dtTo = new DateTimePicker();
            dtTo.Name = "dtTo";
            dtTo.Size = new Size(160, 28);
            dtTo.Location = new Point(267, 12);
            dtTo.Font = new Font("Segoe UI", 9);
            dtTo.Value = DateTime.Now;

            // Quick filter buttons
            var quickFilters = new (string Label, int Days)[]
{
    ("Today", 0), ("This Week", 7), ("This Month", 30)
};

            var quickBtns = new List<Button>();

            for (int i = 0; i < quickFilters.Length; i++)
            {
                var (label, days) = quickFilters[i];
                var btn = new Button();
                btn.Text = label;
                btn.Size = new Size(100, 30);
                btn.Location = new Point(440 + i * 110, 10);
                btn.Font = new Font("Segoe UI", 9);
                btn.BackColor = i == 0 ? Color.FromArgb(192, 57, 43) : Color.FromArgb(240, 240, 240);
                btn.ForeColor = i == 0 ? Color.White : Color.FromArgb(80, 80, 80);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Cursor = Cursors.Hand;
                btn.Tag = days;
                btn.Click += (s, e) =>
                {
                    foreach (var b in quickBtns)
                    {
                        b.BackColor = Color.FromArgb(240, 240, 240);
                        b.ForeColor = Color.FromArgb(80, 80, 80);
                    }
                    btn.BackColor = Color.FromArgb(192, 57, 43);
                    btn.ForeColor = Color.White;

                    int d = (int)(btn.Tag ?? 0);
                    dtFrom.Value = DateTime.Now.AddDays(-d).Date;
                    dtTo.Value = DateTime.Now.Date;
                    LoadReports(dtFrom.Value, dtTo.Value);
                };
                quickBtns.Add(btn);
                pnlFilter.Controls.Add(btn);
            }

            var btnApply = new Button();
            btnApply.Text = "Apply";
            btnApply.Size = new Size(80, 30);
            btnApply.Location = new Point(785, 10);
            btnApply.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnApply.BackColor = Color.FromArgb(192, 57, 43);
            btnApply.ForeColor = Color.White;
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Cursor = Cursors.Hand;
            btnApply.Click += (s, e) => LoadReports(dtFrom.Value, dtTo.Value);

            pnlFilter.Controls.Add(lblFrom);
            pnlFilter.Controls.Add(dtFrom);
            pnlFilter.Controls.Add(lblTo);
            pnlFilter.Controls.Add(dtTo);
            pnlFilter.Controls.Add(btnApply);

            // Summary cards
            string[] cardNames = { "pnlRCard0", "pnlRCard1", "pnlRCard2", "pnlRCard3" };
            string[] cardTitles = { "Total Orders", "Total Revenue", "Cash", "Card" };
            Color[] cardColors = {
                Color.FromArgb(41,  128, 185),
                Color.FromArgb(39,  174, 96),
                Color.FromArgb(243, 156, 18),
                Color.FromArgb(142, 68,  173)
            };

            for (int i = 0; i < cardTitles.Length; i++)
            {
                var card = new Panel();
                card.Name = cardNames[i];
                card.Size = new Size(255, 82);
                card.Location = new Point(20 + i * 268, 128);
                card.BackColor = Color.White;

                var bar = new Panel();
                bar.Size = new Size(255, 5);
                bar.Location = new Point(0, 0);
                bar.BackColor = cardColors[i];

                var lblVal = new Label();
                lblVal.Name = $"lblRVal_{i}";
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

            // Table header
            var pnlHeader = new Panel();
            pnlHeader.Size = new Size(1060, 36);
            pnlHeader.Location = new Point(20, 220);
            pnlHeader.BackColor = Color.FromArgb(245, 245, 245);

            string[] headers = { "Date", "Orders", "Revenue", "Cash", "Card", "Avg. Order" };
            int[] widths = { 160, 100, 160, 150, 150, 140 };
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

            pnlRows.Size = new Size(1060, 260);
            pnlRows.Location = new Point(20, 258);
            pnlRows.BackColor = Color.White;
            pnlRows.AutoScroll = true;

            // Top products label
            var lblTop = new Label();
            lblTop.Name = "lblTopProducts";
            lblTop.Text = "Top Selling Products";
            lblTop.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTop.ForeColor = Color.FromArgb(40, 40, 40);
            lblTop.AutoSize = true;
            lblTop.Location = new Point(20, 530);

            pnlTopProducts.Size = new Size(1060, 120);
            pnlTopProducts.Location = new Point(20, 560);
            pnlTopProducts.BackColor = Color.Transparent;

            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlFilter);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlRows);
            this.Controls.Add(lblTop);
            this.Controls.Add(pnlTopProducts);
        }

        private void LoadReports(DateTime from, DateTime to)
        {
            pnlRows.Controls.Clear();
            pnlTopProducts.Controls.Clear();

            using var db = new AppDbContext();
            var orders = db.Orders
                .Include(o => o.OrderItems)
                            .Where(o => o.Status == "completed"
                    && o.ClosedAt.HasValue
                    && o.ClosedAt.Value.Date >= from.Date
                    && o.ClosedAt.Value.Date <= to.Date)
                .ToList();

            // Summary cards
            int totalOrders = orders.Count;
            decimal totalRev = orders.Sum(o => o.TotalAmount);
            var orderIds = orders.Select(o => o.Id).ToList();
            var payments = db.Payments.Where(p => orderIds.Contains(p.OrderId)).ToList();
            decimal totalCash = payments.Where(p => p.PaymentMethod == "cash").Sum(p => p.Amount);
            decimal totalCard = payments.Where(p => p.PaymentMethod == "card").Sum(p => p.Amount);

            UpdateCard(0, totalOrders.ToString());
            UpdateCard(1, $"₺{totalRev:0}");
            UpdateCard(2, $"₺{totalCash:0}");
            UpdateCard(3, $"₺{totalCard:0}");

            // Daily breakdown
            var daily = orders
                .GroupBy(o => o.ClosedAt!.Value.Date)
                .OrderByDescending(g => g.Key)
                .ToList();

            Color[] rowColors = { Color.White, Color.FromArgb(250, 250, 250) };

            for (int i = 0; i < daily.Count; i++)
            {
                var g = daily[i];
                decimal rev = g.Sum(o => o.TotalAmount);
                var gIds = g.Select(o => o.Id).ToList();
                var gPayments = payments.Where(p => gIds.Contains(p.OrderId)).ToList();
                decimal cash = gPayments.Where(p => p.PaymentMethod == "cash").Sum(p => p.Amount);
                decimal card = gPayments.Where(p => p.PaymentMethod == "card").Sum(p => p.Amount);
                decimal avg = g.Count() > 0 ? rev / g.Count() : 0;

                var row = new Panel();
                row.Size = new Size(1040, 44);
                row.Location = new Point(0, i * 46);
                row.BackColor = rowColors[i % 2];

                int[] colW = { 160, 100, 160, 150, 150, 140 };
                string[] vals = {
                    g.Key.ToString("dd MMM yyyy"),
                    g.Count().ToString(),
                    $"₺{rev:0.00}",
                    $"₺{cash:0.00}",
                    $"₺{card:0.00}",
                    $"₺{avg:0.00}"
                };

                int rx = 10;
                for (int j = 0; j < vals.Length; j++)
                {
                    var lbl = new Label();
                    lbl.Text = vals[j];
                    lbl.Font = j == 2
                        ? new Font("Segoe UI", 10, FontStyle.Bold)
                        : new Font("Segoe UI", 10);
                    lbl.ForeColor = j == 2
                        ? Color.FromArgb(39, 174, 96)
                        : Color.FromArgb(50, 50, 50);
                    lbl.Size = new Size(colW[j], 44);
                    lbl.Location = new Point(rx, 0);
                    lbl.TextAlign = ContentAlignment.MiddleLeft;
                    row.Controls.Add(lbl);
                    rx += colW[j];
                }

                pnlRows.Controls.Add(row);
            }

            // Top selling products
            var topProducts = db.OrderItems
                .Include(i => i.Product)
                .Where(i => i.Status != "cancelled"
                    && i.Order.Status == "completed"
                    && i.Order.ClosedAt.HasValue
                    && i.Order.ClosedAt.Value.Date >= from.Date
                    && i.Order.ClosedAt.Value.Date <= to.Date)
                .GroupBy(i => i.Product!.Name)
                .Select(g => new { Name = g.Key, Total = g.Sum(i => i.Quantity) })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToList();

            Color[] topColors = {
                Color.FromArgb(192,57,43), Color.FromArgb(41,128,185),
                Color.FromArgb(39,174,96), Color.FromArgb(243,156,18),
                Color.FromArgb(142,68,173)
            };

            for (int i = 0; i < topProducts.Count; i++)
            {
                var tp = topProducts[i];
                var card = new Panel();
                card.Size = new Size(200, 110);
                card.Location = new Point(i * 212, 0);
                card.BackColor = Color.White;

                var bar = new Panel();
                bar.Size = new Size(200, 5);
                bar.Location = new Point(0, 0);
                bar.BackColor = topColors[i];

                var lblName = new Label();
                lblName.Text = tp.Name;
                lblName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lblName.ForeColor = Color.FromArgb(40, 40, 40);
                lblName.AutoSize = true;
                lblName.Location = new Point(12, 14);

                var lblSold = new Label();
                lblSold.Text = $"{tp.Total} sold";
                lblSold.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                lblSold.ForeColor = topColors[i];
                lblSold.AutoSize = true;
                lblSold.Location = new Point(12, 38);

                card.Controls.Add(bar);
                card.Controls.Add(lblName);
                card.Controls.Add(lblSold);
                pnlTopProducts.Controls.Add(card);
            }

            if (!topProducts.Any())
            {
                var lblEmpty = new Label();
                lblEmpty.Text = "No sales data for the selected period.";
                lblEmpty.Font = new Font("Segoe UI", 10);
                lblEmpty.ForeColor = Color.Gray;
                lblEmpty.AutoSize = true;
                lblEmpty.Location = new Point(10, 40);
                pnlTopProducts.Controls.Add(lblEmpty);
            }
        }

        private void UpdateCard(int index, string value)
        {
            var lbl = FindAllControls<Label>(this)
                .FirstOrDefault(l => l.Name == $"lblRVal_{index}");
            if (lbl != null) lbl.Text = value;
        }

        private void ReportForm_Load(object sender, EventArgs e)
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