namespace DonerApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using var db = new AppDbContext();
            try
            {
                db.Database.EnsureCreated();
                var tableExists = db.RestaurantTables.Any();
                MessageBox.Show("Bağlantı başarılı! Veritabanı hazır.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
    }
}
