using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace SmartWasteAi
{
    public partial class Form1 : Form
    {
        
        private Color niluferMavi = ColorTranslator.FromHtml("#009EE0"); // Turkuaz
        private Color niluferYesil = ColorTranslator.FromHtml("#78BE20"); // Canlı Yeşil
        private Color yaziGri = Color.FromArgb(64, 64, 64);
        private Color arkaplanBeyaz = Color.White;


        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();


        private TextBox txtKullaniciAdi;
        private TextBox txtSifre;
        private Button btnGiris;

        public Form1()
        {
            InitializeComponent(); // Standart VS metodu çalışsın
            ModernTasarimiYukle(); // Bizim kodlarımız devreye girsin
        }

        private void ModernTasarimiYukle()
        {

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(360, 640); // Telefon Boyutu
            this.BackColor = arkaplanBeyaz;


            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 30, 30));


            Panel pnlHeader = new Panel();
            pnlHeader.Size = new Size(this.Width, 60);
            pnlHeader.BackColor = niluferMavi;
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.MouseDown += Panel_MouseDown; // Sürükleme özelliği
            this.Controls.Add(pnlHeader);

            Label lblClose = new Label();
            lblClose.Text = "X";
            lblClose.ForeColor = Color.White;
            lblClose.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblClose.Location = new Point(this.Width - 30, 15);
            lblClose.Cursor = Cursors.Hand;
            lblClose.Click += (s, e) => Application.Exit();
            pnlHeader.Controls.Add(lblClose);


            Label lblBaslik = new Label();
            lblBaslik.Text = "NİLÜFER\nBELEDİYESİ";
            lblBaslik.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblBaslik.ForeColor = niluferMavi;
            lblBaslik.TextAlign = ContentAlignment.MiddleCenter;
            lblBaslik.Size = new Size(this.Width, 90);
            lblBaslik.Location = new Point(0, 90);
            this.Controls.Add(lblBaslik);

            Label lblAlt = new Label();
            lblAlt.Text = "Personel Takip Sistemi";
            lblAlt.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblAlt.ForeColor = niluferYesil;
            lblAlt.TextAlign = ContentAlignment.MiddleCenter;
            lblAlt.Size = new Size(this.Width, 30);
            lblAlt.Location = new Point(0, 170);
            this.Controls.Add(lblAlt);


            Panel pnlUser = InputPaneliOlustur(230, "👤", "Kullanıcı Adı", false, out txtKullaniciAdi);
            this.Controls.Add(pnlUser);


            Panel pnlPass = InputPaneliOlustur(300, "🔒", "Şifre", true, out txtSifre);
            this.Controls.Add(pnlPass);


            btnGiris = new Button();
            btnGiris.Text = "GİRİŞ YAP";
            btnGiris.BackColor = niluferYesil;
            btnGiris.ForeColor = Color.White;
            btnGiris.FlatStyle = FlatStyle.Flat;
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnGiris.Size = new Size(280, 50);
            btnGiris.Location = new Point(40, 400);
            btnGiris.Cursor = Cursors.Hand;


            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, 40, 40, 90, 180);
            path.AddArc(btnGiris.Width - 40, 0, 40, 40, 270, 180);
            path.CloseAllFigures();
            btnGiris.Region = new Region(path);

            btnGiris.Click += BtnGiris_Click;
            this.Controls.Add(btnGiris);


            Label lblFooter = new Label();
            lblFooter.Text = "© 2025 SmartWasteAI";
            lblFooter.ForeColor = Color.Gray;
            lblFooter.Font = new Font("Segoe UI", 8);
            lblFooter.AutoSize = false;
            lblFooter.TextAlign = ContentAlignment.MiddleCenter;
            lblFooter.Size = new Size(this.Width, 30);
            lblFooter.Location = new Point(0, this.Height - 40);
            this.Controls.Add(lblFooter);
        }


        private Panel InputPaneliOlustur(int y, string ikon, string placeHolder, bool isPassword, out TextBox olusanTextBox)
        {
            Panel p = new Panel();
            p.Size = new Size(280, 50);
            p.Location = new Point(40, y);
            p.BackColor = Color.White;


            Label lblIkon = new Label();
            lblIkon.Text = ikon;
            lblIkon.Font = new Font("Segoe UI Symbol", 14);
            lblIkon.AutoSize = true;
            lblIkon.ForeColor = Color.Gray;
            lblIkon.Location = new Point(0, 5);
            p.Controls.Add(lblIkon);


            TextBox t = new TextBox();
            t.BorderStyle = BorderStyle.None;
            t.Font = new Font("Segoe UI", 12);
            t.ForeColor = yaziGri;
            t.Location = new Point(35, 8);
            t.Width = 240;
            if (isPassword) t.PasswordChar = '●';
            t.Text = placeHolder; 

            // Tıklayınca yazıyı sil
            t.Enter += (s, e) => { if (t.Text == placeHolder) { t.Text = ""; t.ForeColor = Color.Black; } };
            t.Leave += (s, e) => { if (t.Text == "") { t.Text = placeHolder; t.ForeColor = yaziGri; } };

            p.Controls.Add(t);
            olusanTextBox = t;

            Panel cizgi = new Panel();
            cizgi.Size = new Size(280, 2);
            cizgi.BackColor = niluferMavi; 
            cizgi.Location = new Point(0, 35);
            p.Controls.Add(cizgi);

            return p;
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void BtnGiris_Click(object sender, EventArgs e)
        {
            string kAdi = txtKullaniciAdi.Text;
            string sifre = txtSifre.Text;

            // Placeholder kontrolü (Kullanıcı boş basarsa)
            if (kAdi == "Kullanıcı Adı" || string.IsNullOrWhiteSpace(kAdi))
            {
                MessageBox.Show("Lütfen kullanıcı adını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Windows Authentication Bağlantı Cümlesi
            // Server=. (Localhost demektir), Database=SMARTWASTEAI
            string connectionString = "Server=DESKTOP-Q5KKARU\\SQLEXPRESS;Database=SMARTWASTEAI;Integrated Security=True;";

            try
            {
                using (SqlConnection baglanti = new SqlConnection(connectionString))
                {
                    baglanti.Open();

                    // SQL Injection olmasın diye parametreli sorgu kullanıyoruz
                    string sqlSorgu = "SELECT tip FROM [sw_ai].[kullaniciHesap] WHERE kullaniciadi = @user AND sifre = @pass";

                    using (SqlCommand komut = new SqlCommand(sqlSorgu, baglanti))
                    {
                        komut.Parameters.AddWithValue("@user", kAdi);
                        komut.Parameters.AddWithValue("@pass", sifre);

                        // ExecuteScalar tek bir değer (hücre) döndürür. Biz sadece 'tip'i istiyoruz.
                        object sonuc = komut.ExecuteScalar();

                        if (sonuc != null)
                        {
                            // Giriş Başarılı!
                            int uyeTipi = Convert.ToInt32(sonuc);

                            // Formu Gizle
                            this.Hide();

                            // Tipe Göre Yönlendirme
                            if (uyeTipi == 1)
                            {
                                // Tip 2 ise Yönetici Formunu Aç
                                FormAdmin adminFormu = new FormAdmin(); // Projende bu form olmalı!
                                adminFormu.Show();
                                // Admin formu kapanınca uygulamayı komple kapatmak istersen:
                                adminFormu.FormClosed += (s, args) => Application.Exit();
                            }
                            else
                            {
                                // Tip 0 veya 1 ise Personel Formunu Aç
                                FormPersonel personelFormu = new FormPersonel(); // Projende bu form olmalı!
                                personelFormu.Show();
                                personelFormu.FormClosed += (s, args) => Application.Exit();
                            }
                        }
                        else
                        {
                            // Sonuç null ise kullanıcı adı veya şifre yanlıştır
                            MessageBox.Show("Kullanıcı adı veya şifre hatalı be sadıç!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Bağlantı hatası vs. olursa
                MessageBox.Show("Veritabanına bağlanırken bir sıkıntı çıktı:\n" + ex.Message, "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}