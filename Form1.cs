using System;
using System.Drawing;
using System.Windows.Forms;

namespace MelomaniacGuide
{
    public partial class Form1 : Form
    {
        // База даних
        private MusicDatabase db = new MusicDatabase();

        // Оголошуємо елементи інтерфейсу
        private ListBox listBoxArtists;
        private TextBox txtArtistName;
        private Button btnAddArtist;
        private Button btnEditArtist;
        private Button btnDeleteArtist;
        private Label lblArtistTitle;

        public Form1()
        {
            // Викликаємо наш власний метод побудови інтерфейсу
            InitializeCustomComponent();
            
            // Підписуємося на подію завантаження форми
            this.Load += Form1_Load;
        }

        // --- ВІЗУАЛЬНА ЧАСТИНА (Заміна дизайнера) ---
        private void InitializeCustomComponent()
        {
            // Налаштування самої форми
            this.Text = "Довідник меломана - Керування виконавцями";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 1. Надпис (Label)
            lblArtistTitle = new Label();
            lblArtistTitle.Text = "Назва групи/виконавця:";
            lblArtistTitle.Location = new Point(220, 15);
            lblArtistTitle.Size = new Size(200, 20);

            // 2. Поле вводу (TextBox)
            txtArtistName = new TextBox();
            txtArtistName.Location = new Point(220, 40);
            txtArtistName.Size = new Size(180, 25);

            // 3. Кнопка "Додати"
            btnAddArtist = new Button();
            btnAddArtist.Text = "Додати";
            btnAddArtist.Location = new Point(220, 80);
            btnAddArtist.Size = new Size(180, 35);
            btnAddArtist.Click += btnAddArtist_Click; // Прив'язка події

            // 4. Кнопка "Редагувати"
            btnEditArtist = new Button();
            btnEditArtist.Text = "Редагувати";
            btnEditArtist.Location = new Point(220, 125);
            btnEditArtist.Size = new Size(180, 35);
            btnEditArtist.Click += btnEditArtist_Click;

            // 5. Кнопка "Видалити"
            btnDeleteArtist = new Button();
            btnDeleteArtist.Text = "Видалити";
            btnDeleteArtist.Location = new Point(220, 170);
            btnDeleteArtist.Size = new Size(180, 35);
            btnDeleteArtist.BackColor = Color.LightCoral; // Зробимо кнопку червоною для небезпечної дії
            btnDeleteArtist.Click += btnDeleteArtist_Click;

            // 6. Список (ListBox)
            listBoxArtists = new ListBox();
            listBoxArtists.Location = new Point(15, 15);
            listBoxArtists.Size = new Size(190, 270);
            listBoxArtists.SelectedIndexChanged += listBoxArtists_SelectedIndexChanged;

            // Додаємо всі створені елементи на форму
            this.Controls.Add(lblArtistTitle);
            this.Controls.Add(txtArtistName);
            this.Controls.Add(btnAddArtist);
            this.Controls.Add(btnEditArtist);
            this.Controls.Add(btnDeleteArtist);
            this.Controls.Add(listBoxArtists);
        }

        // --- ЛОГІКА РОБОТИ ПРОГРАМИ ---

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                db.LoadData(); 
                RefreshArtistsList(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження: " + ex.Message);
            }
        }

        private void RefreshArtistsList()
        {
            listBoxArtists.DataSource = null; 
            listBoxArtists.DataSource = db.Artists; 
            listBoxArtists.DisplayMember = "Name"; 
        }

        private void btnAddArtist_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtArtistName.Text))
            {
                MessageBox.Show("Введіть назву виконавця!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newArtist = new Artist { Name = txtArtistName.Text.Trim() };
            db.Artists.Add(newArtist);
            
            db.SaveData(); 
            RefreshArtistsList(); 
            txtArtistName.Clear(); 
        }

        private void btnEditArtist_Click(object sender, EventArgs e)
        {
            if (listBoxArtists.SelectedItem is Artist selectedArtist)
            {
                if (string.IsNullOrWhiteSpace(txtArtistName.Text))
                {
                    MessageBox.Show("Введіть нову назву!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedArtist.Name = txtArtistName.Text.Trim(); 
                db.SaveData();
                RefreshArtistsList();
                txtArtistName.Clear();
            }
            else
            {
                MessageBox.Show("Оберіть виконавця для редагування!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDeleteArtist_Click(object sender, EventArgs e)
        {
            if (listBoxArtists.SelectedItem is Artist selectedArtist)
            {
                var result = MessageBox.Show($"Ви впевнені, що хочете видалити {selectedArtist.Name}?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    db.Songs.RemoveAll(s => s.ArtistId == selectedArtist.Id);
                    db.Artists.Remove(selectedArtist);
                    
                    db.SaveData();
                    RefreshArtistsList();
                }
            }
            else
            {
                MessageBox.Show("Оберіть виконавця для видалення!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void listBoxArtists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxArtists.SelectedItem is Artist selectedArtist)
            {
                txtArtistName.Text = selectedArtist.Name;
            }
        }
    }
}
