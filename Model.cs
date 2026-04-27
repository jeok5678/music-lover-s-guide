using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MelomaniacGuide;

public class Artist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    // Можна додати інші поля: Жанр, Рік заснування тощо
}

public class Song
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public Guid ArtistId { get; set; } // Зв'язок з виконавцем
}

public class Disc
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public List<Guid> SongIds { get; set; } = new List<Guid>(); // Перелік пісень на диску
}

public class MusicDatabase
{
    public List<Artist> Artists { get; set; } = new List<Artist>();
    public List<Song> Songs { get; set; } = new List<Song>();
    public List<Disc> Discs { get; set; } = new List<Disc>();

    private readonly string filePath = "music_data.json";

    // --- Збереження та завантаження ---
    public void SaveData()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            throw new Exception($"Помилка при збереженні файлу: {ex.Message}");
        }
    }

    public void LoadData()
    {
        if (!File.Exists(filePath)) return;

        try
        {
            string json = File.ReadAllText(filePath);
            var loadedData = JsonSerializer.Deserialize<MusicDatabase>(json);
            if (loadedData != null)
            {
                Artists = loadedData.Artists ?? new List<Artist>();
                Songs = loadedData.Songs ?? new List<Song>();
                Discs = loadedData.Discs ?? new List<Disc>();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Помилка завантаження бази даних. {ex.Message}");
        }
    }

    // --- Запити (згідно з вимогами курсової) ---
    
    // 1. Вибір всіх пісень заданої групи
    public List<Song> GetSongsByArtist(Guid artistId)
    {
        return Songs.Where(s => s.ArtistId == artistId).ToList();
    }

    // 2. Вибір всіх дисків, де зустрічається задана пісня
    public List<Disc> GetDiscsContainingSong(Guid songId)
    {
        return Discs.Where(d => d.SongIds.Contains(songId)).ToList();
    }
}
