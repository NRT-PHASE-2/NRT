namespace NRTVending
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Snack or Drink
        public double Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }

        public Item(int id, string name, string type, double price, int stock)
        {
            Id = id;
            Name = name ?? "";
            Type = type ?? "";
            Price = price;
            Stock = stock;
            IsActive = true;
        }

        public bool IsAvailable() => IsActive && Stock > 0;
    }
}
