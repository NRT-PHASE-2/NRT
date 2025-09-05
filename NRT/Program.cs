//using Spectre.Console;
//using System;
//using System.Threading;

//namespace NRT_Vending_Machine.NRT
//{
//    internal class Program
//    {
//        /*
//        ╔═════════════════════════════════════════════════════════════════════════════╗
//        ║   If you look at the Solution Explorer, our file structure is as follows:   ║
//        ╚═════════════════════════════════════════════════════════════════════════════╝

//        NRT_Vending_Machine/
//        │
//        ├── Program.cs                # Main guy
//        │
//        ├── Models/                   # Hold Data of vending Items (Blueprint)
//        │   └── VendingItem.cs        // Class for vending items (replaces struct VendingItem)  //?
//        │
//        ├── Services/                 # Services: Handles logic/functionality/operations (buying, stock updates, revenue, etc).
//        │   ├── VendingService.cs     // Handles purchases, stock, revenue
//        │   └── AdminService.cs       // Handles admin functions (resupply, withdraw, auth)
//        │
//        ├── Data/                     # Manages storing and retrieveing Data
//        │   ├── DatabaseConnection.cs // Manages DB connection
//        │   ├── VendingRepository.cs  // Handles CRUD for vending items
//        │   └── AdminRepository.cs    // (Optional) stores admin credentials, logs, etc
//        │
//        ├── UI/                       # User Interface
//        │   ├── MenuUI.cs             // Main menu display logic
//        │   ├── AdminUI.cs            // Admin menu display logic
//        │   └── DisplayHelper.cs      // Color, tables, headers


//        ╔═════════════════════════════════════════════════════════════════════════════╗
//        ║                             NOTES FOR PHASE 2:                              ║
//        ╚═════════════════════════════════════════════════════════════════════════════╝
//        */



//        // ================================================================
//        // Move this struct into Models/VendingItem.cs and make it a class.
//        // Each item will come from the database instead of hardcoded.
//        // ================================================================
//        public struct VendingItem
//        {
//            public string Category { get; set; }
//            public string Name { get; set; }
//            public decimal Price { get; set; }
//            public int Stock { get; set; }
//            public int InitialStock { get; set; }
//        }

//        // ================================================================
//        // PHASE 2:
//        // Static list will be removed. Use VendingRepository to fetch items from DB.
//        // ================================================================
//        private static readonly List<VendingItem> VendingMachineItems = new List<VendingItem>();

//        // ================================================================
//        // PHASE 2:
//        // Remove hardcoded admin password. Store it in DB or config.
//        // ================================================================
//        private const string AdminPassword = "NRT_Owner1738";
//        private const decimal MAX_MONEY_INPUT = 250.00m; // PHASE 2: move to AdminService

//        private static decimal totalRevenue = 0; // PHASE 2: track revenue in DB

//        static void Main(string[] args)
//        {
//            Console.Title = "NRT Vending Machine";

//            // ================================================================
//            // Phase 2: Remove PopulateVendingMachineData()
//            // Instead fetch all items from DB via VendingRepository.
//            // ================================================================
//            PopulateVendingMachineData();

//            // ================================================================
//            // Phase 2: Move ShowMainMenu to UI/MenuUI.cs
//            // Program.cs should only start the app and connect services.
//            // ================================================================
//            ShowMainMenu();
//        }

//        // ================================================================
//        // PopulateVendingMachineData()
//        // Phase 2: Remove this. Database will handle item data.
//        // ================================================================
//        static void PopulateVendingMachineData()
//        {
//            VendingMachineItems.Clear();
//            VendingMachineItems.AddRange(new VendingItem[]
//            {
//                new VendingItem { Category = "Snacks", Name = "Lays", Price = 15.00m, Stock = 8, InitialStock = 8 },
//                new VendingItem { Category = "Snacks", Name = "Doritos", Price = 16.99m, Stock = 6, InitialStock = 6 },
//                new VendingItem { Category = "Snacks", Name = "Chocolates", Price = 22.00m, Stock = 5, InitialStock = 5 },
//                new VendingItem { Category = "Snacks", Name = "Nuts", Price = 13.50m, Stock = 3, InitialStock = 3 },
//                new VendingItem { Category = "Drinks", Name = "Water", Price = 10.00m, Stock = 10, InitialStock = 10 },
//                new VendingItem { Category = "Drinks", Name = "Energy Drink", Price = 23.00m, Stock = 1, InitialStock = 5 },
//                new VendingItem { Category = "Drinks", Name = "Juice", Price = 15.00m, Stock = 4, InitialStock = 6 },
//                new VendingItem { Category = "Drinks", Name = "Cold Drinks", Price = 12.50m, Stock = 11, InitialStock = 12 },
//                new VendingItem { Category = "Drinks", Name = "Sports Drink", Price = 22.00m, Stock = 7, InitialStock = 7 },
//                new VendingItem { Category = "Drinks", Name = "Tea", Price = 13.00m, Stock = 6, InitialStock = 8 }
//            });
//        }

//        // ================================================================
//        // DISPLAY METHODS
//        // Phase 2: move these to UI/DisplayHelper.cs
//        // Any drawing logic stays in UI, call services to get data.
//        // ================================================================
//        static void DrawHeader() { /* ... */ }
//        static void DisplayItemsTable() { /* ... */ }

//        // ================================================================
//        // MENU METHODS
//        // Phase 2: move ShowMainMenu, AdminMenu, ShowAdminOptions to UI/
//        // Call VendingService and AdminService instead of using static list.
//        // ================================================================
//        static void ShowMainMenu() { /* ... */ }
//        static void ItemSelectionAndPurchase() { /* ... */ }
//        static void AdminMenu() { /* ... */ }
//        static void ShowAdminOptions() { /* ... */ }

//        // ================================================================
//        // ADMIN FUNCTIONS
//        // Phase 2: move WithdrawRevenue and ResupplyAllStock to Services/AdminService.cs
//        // Update the database instead of static variables/lists.
//        // ================================================================
//        static void WithdrawRevenue() { /* ... */ }
//        static void ResupplyAllStock() { /* ... */ }

//        // ================================================================
//        // HELPERS
//        // Phase 2: keep in UI/Style.cs
//        // ================================================================
//        static void WriteLineInColor(string text, ConsoleColor color) { /* ... */ }
//        static void PauseAndReturn() { /* ... */ }
//        static void PlayNRT() { /* ... */ }
//        //static string ReadPassword() { /* ... */ }

//        // ================================================================
//        // PHASE 2 NOTES FOR NRT TEAM:
//        // 1. Create database tables: VendingItems, Admins, Revenue
//        // 2. Add cart functionality: CartItem + CartService
//        // 3. Any menu should call services/repositories instead of using static data
//        // 4. Remove all hardcoded data where possible
//        // ================================================================
//    }
//}






//TO TEST FATABASE:
using System;
using NRT_Vending_Machine.Data;

namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DBConnection db = new DBConnection();
            db.TestConnection();  // Try connecting
        }
    }
}
