using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Data.SqlClient;




namespace SmartWasteAi

{
    public partial class FormPersonel : Form
    {

        ComboBox cmbMahalle;
        TextBox txtAracId;

        private void Panel2IcerigiOlusturVeBagla()
        {
            panel2.Controls.Clear();
            panel2.BackColor = Color.White;

            Color niluferMavi = ColorTranslator.FromHtml("#009EE0");
            Color niluferYesil = ColorTranslator.FromHtml("#78BE20");

            // --- 1. MAHALLE KISMI ---
            Label lblIcon1 = new Label();
            lblIcon1.Text = "📍";
            lblIcon1.Font = new Font("Segoe UI Symbol", 18);
            lblIcon1.ForeColor = Color.Gray;
            lblIcon1.AutoSize = true;
            lblIcon1.Location = new Point(20, 40);
            panel2.Controls.Add(lblIcon1);

            Label lblBaslik1 = new Label();
            lblBaslik1.Text = "MAHALLE SEÇİMİ";
            lblBaslik1.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblBaslik1.ForeColor = niluferYesil;
            lblBaslik1.Location = new Point(60, 45);
            lblBaslik1.AutoSize = true;
            panel2.Controls.Add(lblBaslik1);

            // --- COMBOBOX (O MAVİYİ YOK EDEN KOD) ---
            cmbMahalle = new ComboBox();
            cmbMahalle.Location = new Point(60, 65);
            cmbMahalle.Width = 220;
            cmbMahalle.Font = new Font("Segoe UI", 11);
            cmbMahalle.DropDownStyle = ComboBoxStyle.DropDownList;

            // BURASI ÇOK ÖNEMLİ: Çizimi biz yapacağız diyoruz
            cmbMahalle.DrawMode = DrawMode.OwnerDrawFixed;

            // Çizim Olayı (Windows'un mavisini eziyoruz)
            cmbMahalle.DrawItem += (sender, e) =>
            {
                e.DrawBackground();
                if (e.Index >= 0)
                {
                    // Seçiliyse Nilüfer Yeşili yap, değilse Beyaz
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(niluferYesil), e.Bounds); // Seçim Rengi
                        TextRenderer.DrawText(e.Graphics, cmbMahalle.Items[e.Index].ToString(), cmbMahalle.Font, e.Bounds, Color.White, TextFormatFlags.Left);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds); // Normal Renk
                        TextRenderer.DrawText(e.Graphics, cmbMahalle.Items[e.Index].ToString(), cmbMahalle.Font, e.Bounds, Color.Black, TextFormatFlags.Left);
                    }
                }
                e.DrawFocusRectangle();
            };

            panel2.Controls.Add(cmbMahalle);

            Panel cizgi1 = new Panel();
            cizgi1.BackColor = niluferMavi;
            cizgi1.Size = new Size(260, 2);
            cizgi1.Location = new Point(20, 98);
            panel2.Controls.Add(cizgi1);

            // --- 2. ARAÇ ID ---
            Label lblIcon2 = new Label();
            lblIcon2.Text = "🚛";
            lblIcon2.Font = new Font("Segoe UI Symbol", 18);
            lblIcon2.ForeColor = Color.Gray;
            lblIcon2.AutoSize = true;
            lblIcon2.Location = new Point(20, 130);
            panel2.Controls.Add(lblIcon2);

            Label lblBaslik2 = new Label();
            lblBaslik2.Text = "ARAÇ PLAKA / ID";
            lblBaslik2.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblBaslik2.ForeColor = niluferYesil;
            lblBaslik2.Location = new Point(60, 135);
            lblBaslik2.AutoSize = true;
            panel2.Controls.Add(lblBaslik2);

            txtAracId = new TextBox();
            txtAracId.BorderStyle = BorderStyle.None;
            txtAracId.Font = new Font("Segoe UI", 12);
            txtAracId.Location = new Point(65, 155);
            txtAracId.Width = 215;
            txtAracId.BackColor = Color.White;
            panel2.Controls.Add(txtAracId);

            Panel cizgi2 = new Panel();
            cizgi2.BackColor = niluferMavi;
            cizgi2.Size = new Size(260, 2);
            cizgi2.Location = new Point(20, 180);
            panel2.Controls.Add(cizgi2);

            // --- 3. BUTON ---
            Button btnRota = new Button();
            btnRota.Text = "ROTA OLUŞTUR";
            btnRota.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnRota.BackColor = niluferMavi;
            btnRota.ForeColor = Color.White;
            btnRota.FlatStyle = FlatStyle.Flat;
            btnRota.FlatAppearance.BorderSize = 0;
            btnRota.Size = new Size(260, 50);
            btnRota.Location = new Point(20, 240);
            btnRota.Cursor = Cursors.Hand;
            btnRota.Click += BtnRota_Click;
            panel2.Controls.Add(btnRota);

            MahalleleriGetir();
        }

        private void MahalleleriGetir()
        {
            // BAĞLANTI ADRESİNİ GARANTİYE ALIYORUZ
            // 1. localhost dene, 2. SQLEXPRESS dene.
            // TrustServerCertificate=True ekledik ki hata vermesin.
            string connectionString = @"Server=DESKTOP-Q5KKARU\\SQLEXPRESS;Database=SMARTWASTEAI;Integrated Security=True;TrustServerCertificate=True;";

            // Eğer yukarıdaki çalışmazsa alt satırı yorumdan çıkar onu dene:
            // string connectionString = @"Server=.\SQLEXPRESS;Database=SMARTWASTEAI;Integrated Security=True;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection baglanti = new SqlConnection(connectionString))
                {
                    baglanti.Open();

                    string sql = @"
                SELECT DISTINCT m.mahalle 
                FROM [sw_ai].[isEmriMahalle] AS iem
                INNER JOIN [sw_ai].[mahalle] AS m ON iem.mahalleId = m.id
                ORDER BY m.mahalle ASC";

                    using (SqlCommand komut = new SqlCommand(sql, baglanti))
                    {
                        SqlDataReader oku = komut.ExecuteReader();
                        cmbMahalle.Items.Clear();

                        while (oku.Read())
                        {
                            if (oku["mahalle"] != DBNull.Value)
                                cmbMahalle.Items.Add(oku["mahalle"].ToString());
                        }

                        if (cmbMahalle.Items.Count > 0)
                            cmbMahalle.SelectedIndex = 0;
                        else
                            cmbMahalle.Items.Add("İş Emri Yok");
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata mesajını kopyalayıp bana atarsın
                MessageBox.Show("Hala bağlanamadım sadıç.\n" +
                    "Denediğim Sunucu: localhost (veya SQLEXPRESS)\n" +
                    "Hata Mesajı: " + ex.Message, "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRota_Click(object sender, EventArgs e)
        {
            if (cmbMahalle.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtAracId.Text))
            {
                MessageBox.Show("Lütfen mahalle seç ve araç ID giriniz!", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string secilenMahalle = cmbMahalle.SelectedItem.ToString();
            string girilenArac = txtAracId.Text;

            string connectionString = @"Server=DESKTOP-Q5KKARU\\SQLEXPRESS;Database=SMARTWASTEAI;Integrated Security=True;";

            try
            {
                using (SqlConnection baglanti = new SqlConnection(connectionString))
                {
                    baglanti.Open();
                    string sql = "INSERT INTO [sw_ai].[rota_kayitlari] (mahalle, arac_id) VALUES (@mah, @arac)";

                    using (SqlCommand komut = new SqlCommand(sql, baglanti))
                    {
                        komut.Parameters.AddWithValue("@mah", secilenMahalle);
                        komut.Parameters.AddWithValue("@arac", girilenArac);
                        komut.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Rota oluşturuldu, araç yola çıkıyor!", "İşlem Tamam", MessageBoxButtons.OK, MessageBoxIcon.Information);

                panel2.Visible = false;
                panel1.Visible = true;

                cmbMahalle.SelectedIndex = -1;
                txtAracId.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kayıt hatası: " + ex.Message);
            }
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public FormPersonel()
        {
            InitializeComponent();
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 30, 30));
        }

        private void FormPersonel_Load(object sender, EventArgs e)
        {
            Panel2IcerigiOlusturVeBagla();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
