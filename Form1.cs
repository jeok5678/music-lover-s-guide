private void Form1_Load(object sender, EventArgs e)
{
    try
    {
        db.LoadData();
        RefreshUI(); // Твій метод для оновлення ListBox або DataGridView
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
private void Form1_Load(object sender, EventArgs e)
{
    try
    {
        db.LoadData();
        RefreshUI(); // Твій метод для оновлення ListBox або DataGridView
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
private void btnAddArtist_Click(object sender, EventArgs e)
{
    if (string.IsNullOrWhiteSpace(txtArtistName.Text))
    {
        MessageBox.Show("Будь ласка, введіть назву групи!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return; // Зупиняємо виконання, програма не падає
    }

    var newArtist = new Artist { Name = txtArtistName.Text.Trim() };
    db.Artists.Add(newArtist);
    db.SaveData(); // Одразу зберігаємо
    RefreshUI();
}
private void btnDeleteArtist_Click(object sender, EventArgs e)
{
    // Перевіряємо, чи вибрано хоч щось у ListBox
    if (listBoxArtists.SelectedIndex == -1)
    {
        MessageBox.Show("Оберіть групу для видалення!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }

    // Припускаємо, що ти зберігаєш об'єкти Artist прямо в ListBox
    if (listBoxArtists.SelectedItem is Artist selectedArtist)
    {
        // Каскадне видалення (за бажанням): видаляти і пісні цієї групи
        db.Songs.RemoveAll(s => s.ArtistId == selectedArtist.Id); 
        
        db.Artists.Remove(selectedArtist);
        db.SaveData();
        RefreshUI();
    }
}
