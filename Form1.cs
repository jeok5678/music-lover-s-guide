using System;
using System.Windows.Forms;
// Якщо твої класи в файлі Guide.cs мають namespace MelomaniacGuide, переконайся, що він тут такий самий

namespace MelomaniacGuide
{
    public partial class Form1 : Form
    {
        // Створюємо екземпляр нашої бази даних
        private MusicDatabase db = new MusicDatabase();

        public Form1()
        {
            InitializeComponent();
        }

        // Ця подія спрацьовує при запуску програми
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                db.LoadData(); // Завантажуємо дані з JSON
                RefreshArtistsList(); // Оновлюємо візуальний список
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження: " + ex.Message);
            }
        }

        // Метод для оновлення списку виконавців на екрані (READ)
        private void RefreshArtistsList()
        {
            listBoxArtists.DataSource = null; // Очищаємо прив'язку
            listBoxArtists.DataSource = db.Artists; // Підключаємо наш список
            listBoxArtists.DisplayMember = "Name"; // Вказуємо, що саме показувати (властивість Name)
        }

        // Обробник кнопки "Додати" (CREATE)
        private void btnAddArtist_Click(object sender, EventArgs e)
        {
            // Захист від пустих значень (щоб не падало)
            if (string.IsNullOrWhiteSpace(txtArtistName.Text))
            {
                MessageBox.Show("Введіть назву виконавця!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newArtist = new Artist { Name = txtArtistName.Text.Trim() };
            db.Artists.Add(newArtist);
            
            db.SaveData(); // Зберігаємо у файл
            RefreshArtistsList(); // Оновлюємо екран
            txtArtistName.Clear(); // Очищаємо поле вводу
        }

        // Обробник кнопки "Редагувати" (UPDATE)
        private void btnEditArtist_Click(object sender, EventArgs e)
        {
            // Перевіряємо, чи вибрано когось у списку
            if (listBoxArtists.SelectedItem is Artist selectedArtist)
            {
                if (string.IsNullOrWhiteSpace(txtArtistName.Text))
                {
                    MessageBox.Show("Введіть нову назву!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedArtist.Name = txtArtistName.Text.Trim(); // Змінюємо ім'я
                
                db.SaveData();
                RefreshArtistsList();
                txtArtistName.Clear();
            }
            else
            {
                MessageBox.Show("Оберіть виконавця для редагування!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обробник кнопки "Видалити" (DELETE)
        private void btnDeleteArtist_Click(object sender, EventArgs e)
        {
            if (listBoxArtists.SelectedItem is Artist selectedArtist)
            {
                // Підтвердження дії від користувача
                var result = MessageBox.Show($"Ви впевнені, що хочете видалити {selectedArtist.Name}?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    // Видаляємо всі пісні цього виконавця (щоб не було "мертвих" даних)
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

        // Щоб при кліку на список ім'я з'являлося в текстовому полі для зручного редагування
        private void listBoxArtists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxArtists.SelectedItem is Artist selectedArtist)
            {
                txtArtistName.Text = selectedArtist.Name;
            }
        }
    }
}
