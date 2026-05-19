using System;
using System.Collections.Generic;
using System.Linq;
using MusicGuide.Models;

namespace MusicGuide.Controllers
{
    public class MusicController
    {
        private readonly MusicDatabase _db;

        public MusicController()
        {
            _db = new MusicDatabase();
            _db.Load();  // Контролер сам завантажує дані при створенні
        }

        // --- Отримання даних ---
        public IReadOnlyList<Artist> GetArtists() => _db.Artists;
        public IReadOnlyList<Song> GetSongs() => _db.Songs;
        public IReadOnlyList<Disc> GetDiscs() => _db.Discs;

        // --- Управління виконавцями ---
        public void AddArtist(string name)
        {
            _db.Artists.Add(new Artist { Name = name });
            _db.Save();
        }

        public void EditArtist(Artist artist, string newName)
        {
            artist.Name = newName;
            _db.Save();
        }

        public void DeleteArtist(Artist artist)
        {
            _db.Artists.Remove(artist);
            _db.Save();
        }

        // --- Управління піснями ---
        public void AddSong(string title, Guid artistId)
        {
            _db.Songs.Add(new Song { Title = title, ArtistId = artistId });
            _db.Save();
        }

        public void EditSong(Song song, string newTitle, Guid artistId)
        {
            song.Title = newTitle;
            song.ArtistId = artistId;
            _db.Save();
        }

        public void DeleteSong(Song song)
        {
            _db.Songs.Remove(song);
            _db.Save();
        }

        // --- Управління альбомами ---
        public void AddDisc(string title, int year, List<Guid> songIds)
        {
            _db.Discs.Add(new Disc { Title = title, Year = year, SongIds = songIds });
            _db.Save();
        }

        public void EditDisc(Disc disc, string newTitle, int newYear, List<Guid> songIds)
        {
            disc.Title = newTitle;
            disc.Year = newYear;

            disc.SongIds.Clear();
            disc.SongIds.AddRange(songIds);

            _db.Save();
        }

        public void DeleteDisc(Disc disc)
        {
            _db.Discs.Remove(disc);
            _db.Save();
        }
    }
}