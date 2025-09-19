using System;

namespace DesclutionRecords
{
    public class Disk
    {
        public int Id { get; set; }                 // DB PK
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public string SongPath { get; set; }

        public Disk(int id, string title, string artist, double price, int stock, string songPath, string genre)
        {
            Id = id;
            Title = title ?? "";
            Artist = artist ?? "";
            Price = price;
            Stock = stock;
            SongPath = songPath ?? "";
            Genre = genre ?? "";
            IsActive = true;
        }

        public bool IsAvailable() => IsActive && Stock > 0;
        public void ReduceStock() { if (Stock > 0) Stock--; }
        public void SetStock(int newStock) => Stock = newStock;
    }
}
