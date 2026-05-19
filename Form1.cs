using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MusicGuide.Models;
using MusicGuide.Controllers;

namespace MusicGuide
{
    public partial class Form1 : Form
    {
        private readonly MusicController _controller;

        public Form1()
        {
            InitializeComponent();
            _controller = new MusicController();

            // 1. Спочатку завантажуємо початкові списки
            UpdateList();

            // 2. ПРИМУСОВО прив'язуємо подію до правильного контролера прямо в коді!
            listBoxDiscs.SelectedIndexChanged += listBoxDiscs_SelectedIndexChanged;

            // 3. Одразу синхронізуємо дані для першого альбому в списку
            SyncDiscDetails();
        }

        private void UpdateList()
        {
            // Використовуємо .ToList() щоб уникнути проблем з прив'язкою даних
            listBoxArtists.DataSource = _controller.GetArtists().ToList();
            listBoxArtists.DisplayMember = "Name";

            cmbSongArtist.DataSource = _controller.GetArtists().ToList();
            cmbSongArtist.DisplayMember = "Name";
            cmbSongArtist.ValueMember = "Id";

            listBoxSongs.DataSource = _controller.GetSongs().ToList();
            listBoxSongs.DisplayMember = "Title";

            listBoxDiscs.DataSource = _controller.GetDiscs().ToList();
            listBoxDiscs.DisplayMember = "Title";

            clbDiscSongs.Items.Clear();
            foreach (var song in _controller.GetSongs())
            {
                clbDiscSongs.Items.Add(song);
            }
            clbDiscSongs.DisplayMember = "Title";
        }

        // --- Управління виконавцями ---
        private void btnAddArtist_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtArtistName.Text))
            {
                _controller.AddArtist(txtArtistName.Text.Trim());
                UpdateList();
                txtArtistName.Clear();
            }
        }

        private void btnEditArtist_Click(object sender, EventArgs e)
        {
            if (listBoxArtists.SelectedItem is Artist selected && !string.IsNullOrWhiteSpace(txtArtistName.Text))
            {
                _controller.EditArtist(selected, txtArtistName.Text.Trim());
                UpdateList();
            }
        }

        private void btnDeleteArtist_Click(object sender, EventArgs e)
        {
            if (listBoxArtists.SelectedItem is Artist selected)
            {
                _controller.DeleteArtist(selected);
                UpdateList();
            }
        }

        // --- Управління піснями ---
        private void btnAddSong_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSongTitle.Text) && cmbSongArtist.SelectedItem is Artist art)
            {
                _controller.AddSong(txtSongTitle.Text.Trim(), art.Id);
                UpdateList();
                txtSongTitle.Clear();
            }
        }

        private void btnEditSong_Click(object sender, EventArgs e)
        {
            if (listBoxSongs.SelectedItem is Song selected &&
                !string.IsNullOrWhiteSpace(txtSongTitle.Text) &&
                cmbSongArtist.SelectedItem is Artist art)
            {
                _controller.EditSong(selected, txtSongTitle.Text.Trim(), art.Id);
                UpdateList();
            }
        }

        private void btnDeleteSong_Click(object sender, EventArgs e)
        {
            if (listBoxSongs.SelectedItem is Song selected)
            {
                _controller.DeleteSong(selected);
                UpdateList();
            }
        }

        // --- Управління альбомами ---
        private void btnAddDisc_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtDiscTitle.Text) && int.TryParse(txtDiscYear.Text, out int year))
            {
                var songIds = GetCheckedSongIds();
                _controller.AddDisc(txtDiscTitle.Text.Trim(), year, songIds);

                UpdateList();
                txtDiscTitle.Clear();
                txtDiscYear.Clear();
            }
        }


        private void btnDeleteDisc_Click(object sender, EventArgs e)
        {
            if (listBoxDiscs.SelectedItem is Disc selected)
            {
                _controller.DeleteDisc(selected);
                UpdateList();
            }
        }

        // Допоміжний метод для збору ID обраних пісень
        private List<Guid> GetCheckedSongIds()
        {
            var songIds = new List<Guid>();
            foreach (var item in clbDiscSongs.CheckedItems)
            {
                if (item is Song s) songIds.Add(s.Id);
            }
            return songIds;
        }
        // --- UI логіка для альбомів ---

        private void SyncDiscDetails()
        {
            if (listBoxDiscs.SelectedItem is Disc selected)
            {
                txtDiscTitle.Text = selected.Title;
                txtDiscYear.Text = selected.Year.ToString();

                for (int i = 0; i < clbDiscSongs.Items.Count; i++)
                {
                    if (clbDiscSongs.Items[i] is Song s)
                    {
                        clbDiscSongs.SetItemChecked(i, selected.SongIds.Contains(s.Id));
                    }
                }
            }
        }

        private void listBoxDiscs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxDiscs.SelectedIndex == -1) return;
            SyncDiscDetails();
        }

        private void RefreshDiscsList()
        {
            int currentIndex = listBoxDiscs.SelectedIndex;

            // Тимчасово відписуємося, щоб уникнути зациклення при обнуленні DataSource
            listBoxDiscs.SelectedIndexChanged -= listBoxDiscs_SelectedIndexChanged;

            listBoxDiscs.DataSource = null;
            listBoxDiscs.DataSource = _controller.GetDiscs().ToList();
            listBoxDiscs.DisplayMember = "Title";

            if (currentIndex >= 0 && currentIndex < listBoxDiscs.Items.Count)
            {
                listBoxDiscs.SelectedIndex = currentIndex;
            }

            // Підписуємося назад
            listBoxDiscs.SelectedIndexChanged += listBoxDiscs_SelectedIndexChanged;

            // Оновлюємо галочки для поточного альбому
            SyncDiscDetails();
        }

        // --- Обробник кнопки Редагувати ---
        private void btnEditDisc_Click(object sender, EventArgs e)
        {
            if (listBoxDiscs.SelectedItem is Disc selected &&
                !string.IsNullOrWhiteSpace(txtDiscTitle.Text) &&
                int.TryParse(txtDiscYear.Text, out int year))
            {
                var songIds = GetCheckedSongIds();

                // Тепер сюди точно заходить і зберігає в data.json!
                _controller.EditDisc(selected, txtDiscTitle.Text.Trim(), year, songIds);

                RefreshDiscsList();
            }
            else
            {
                MessageBox.Show("Перевірте, чи вибрано альбом та чи правильно вказано рік!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)

        {

            // Реалізація F1 – допомога

            if (keyData == Keys.F1)

            {

                ShowHelp();

                return true;

            }



            // Реалізація Esc – закриття або скасування

            if (keyData == Keys.Escape)

            {

                var result = MessageBox.Show("Ви впевнені, що хочете закрити програму?",

                    "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) this.Close();

                return true;

            }



            return base.ProcessCmdKey(ref msg, keyData);

        }



        private void ShowHelp()

        {

            string helpText = "";

            string title = "";



            // Визначаємо контекст довідки залежно від обраної вкладки

            switch (tabControl1.SelectedIndex)

            {

                case 0: // Вкладка "Виконавці"

                    title = "Довідка: Виконавці";

                    helpText = "На цій вкладці ви керуєте списком артистів:\n\n" +

                               "• Щоб додати: введіть ім'я у поле та натисніть Enter.\n" +

                               "• Щоб змінити: оберіть артиста у списку, відредагуйте текст і натисніть «Редагувати».\n" +

                               "• Щоб видалити: оберіть запис і натисніть клавішу «Видалити».";

                    break;



                case 1: // Вкладка "Пісні"

                    title = "Довідка: Пісні";

                    helpText = "Тут ви створюєте базу ваших треків:\n\n" +

                               "1. Оберіть виконавця зі списку (це обов'язково).\n" +

                               "2. Введіть назву пісні.\n" +

                               "3. Натисніть «Додати».\n\n" +

                               "Кожна пісня прив'язується до конкретного артиста через унікальний ідентифікатор (GUID).";

                    break;



                case 2: // Вкладка "Альбоми"

                    title = "Довідка: Альбоми";

                    helpText = "Ця вкладка реалізує логіку зв'язків:\n\n" +

                               "• Додавання: введіть назву та рік, позначте галочками потрібні пісні і натисніть «Додати».\n" +

                               "• Редагування: оберіть альбом, змініть склад пісень і ОБОВ'ЯЗКОВО натисніть «Редагувати», щоб зберегти нові галочки.\n" +

                               "• Видалення: оберіть альбом і натисніть «Видалити».";

                    break;



                default:

                    title = "Загальна довідка";

                    helpText = "Використовуйте Tab для навігації та Esc для виходу.";

                    break;

            }



            // Виводимо повідомлення з відповідною іконкою та заголовком

            MessageBox.Show(helpText, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)

        {

            // Міняємо "головну кнопку" при переході на іншу вкладку

            switch (tabControl1.SelectedIndex)

            {

                case 0: this.AcceptButton = btnAddArtist; break;

                case 1: this.AcceptButton = btnAddSong; break;

                case 2: this.AcceptButton = btnAddDisc; break;

            }

        }

    }

}