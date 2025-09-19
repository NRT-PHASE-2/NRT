using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;

namespace NRTVending
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Music.PlayLoop("3.wav"); //fun fact all music was produced by me

            while (true)
            {
                Console.Clear();
                ShowBanner();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Welcome to NRT Vending Machine! Choose an option:")
                        .AddChoices(new[] {
                            "User Login",
                            "Admin Login",             
                            "Exit"
                        }));

                switch (choice)
                {
                    case "User Login":
                        UserLogin();
                        break;
                    case "Admin Login":
                        AdminLogin();
                        break;                
                    case "Exit":
                        Music.Stop();
                        return;
                }
            }
        }

        static void ShowBanner()
        {
            var figlet = new FigletText("NRT VENDING").Centered().Color(Color.Red1);
            AnsiConsole.Write(figlet);
            Console.WriteLine();
        }


        // ----------------- USER LOGIN -----------------
        static void UserLogin()
        {
            Console.Clear();
            ShowBanner();

            string studentId = "";
            while (true)
            {
                studentId = AnsiConsole.Ask<string>("Enter [orange1]Student ID[/]:");
                if (studentId.Length < 6 || studentId.Length > 10)
                {
                    AnsiConsole.MarkupLine("[red1]Student ID must be 6–10 characters![/]");
                    continue;
                }
                break;
            }

            string password = AnsiConsole.Prompt(new TextPrompt<string>("Enter [orange1]Password[/]:").Secret());
            var user = Database.GetUser(studentId, password);
            if (user == null)
            {
                AnsiConsole.MarkupLine("[red1]Invalid credentials! Press any key to return...[/]");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.Clear();
                ShowBanner();
                AnsiConsole.MarkupLine($"Welcome [bold green1]{user.Name}[/]! Balance: R{user.Balance:0.00}\n");

                ShowItemsTable();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] { "Deposit", "Withdraw", "Purchase", "Logout" }));

                switch (choice)
                {
                    case "Deposit":
                        Deposit(user);
                        break;
                    case "Withdraw":
                        Withdraw(user);
                        break;
                    case "Purchase":
                        Purchase(user);
                        break;
                    case "Logout":
                        return;
                }
            }
        }

        // ----------------- ADMIN LOGIN -----------------
        static void AdminLogin()
        {
            Console.Clear();
            ShowBanner();

            string username = AnsiConsole.Ask<string>("Enter [orange1]Admin Username[/]:");
            string password = AnsiConsole.Prompt(new TextPrompt<string>("Enter [orange1]Password[/]:").Secret());
            var admin = Database.GetAdmin(username, password);
            if (admin == null)
            {
                AnsiConsole.MarkupLine("[red1]Invalid credentials! Press any key to return...[/]");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.Clear();
                ShowBanner();
                AnsiConsole.MarkupLine($"Welcome [bold green1]{admin.Username}[/]! Balance: R{admin.Balance:0.00}\n");

                ShowUsersTable();
                ShowItemsTable();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] { "Add User", "Add Item", "Delete Item", "Retrieve Vending Machine Balance", "Logout" }));

                switch (choice)
                {
                    case "Add User":
                        AddUser();
                        break;
                    case "Add Item":
                        AddItem();
                        break;
                    case "Delete Item":
                        DeleteItem();
                        break;
                    case "Retrieve Vending Machine Balance":
                        RetrieveVendingBalance(admin);
                        break;
                    case "Logout":
                        return;
                }
            }
        }

        // ----------------- METHODS -----------------
        static void ShowItemsTable()
        {
            var items = Database.GetAllItems();
            var table = new Table().Centered();
            table.AddColumn(new TableColumn("[red1]ID[/]").Centered());
            table.AddColumn(new TableColumn("[cyan1]Name[/]").Centered());
            table.AddColumn(new TableColumn("[green1]Type[/]").Centered());
            table.AddColumn(new TableColumn("[orange1]Price[/]").Centered());
            table.AddColumn(new TableColumn("[orange1]Stock[/]").Centered());

            foreach (var i in items)
            {
                table.AddRow(i.Id.ToString(), i.Name, i.Type, $"R{i.Price:0.00}", i.Stock.ToString());
            }

            AnsiConsole.Write(table);
            Console.WriteLine();
        }

        static void ShowUsersTable()
        {
            var users = Database.GetAllUsers();
            var table = new Table().Centered();
            table.AddColumn(new TableColumn("[red1]ID[/]").Centered());
            table.AddColumn(new TableColumn("[orange1]StudentID[/]").Centered());
            table.AddColumn(new TableColumn("[cyan1]Name[/]").Centered());
            table.AddColumn(new TableColumn("[green1]Balance[/]").Centered());

            foreach (var u in users)
            {
                table.AddRow(u.Id.ToString(), u.StudentId, u.Name, $"R{u.Balance:0.00}");
            }

            AnsiConsole.Write(table);
            Console.WriteLine();
        }

        static void Deposit(User user)
        {
            decimal amount = AnsiConsole.Ask<decimal>("Enter deposit amount:");
            user.Balance += amount;
            Database.UpdateUserBalance(user);
        }

        static void Withdraw(User user)
        {
            decimal amount = AnsiConsole.Ask<decimal>("Enter withdraw amount:");
            if (amount > user.Balance)
            {
                AnsiConsole.MarkupLine("[red1]Insufficient balance![/]");
                Console.ReadKey();
                return;
            }
            user.Balance -= amount;
            Database.UpdateUserBalance(user);
        }

        static void Purchase(User user)
        {
            int id = AnsiConsole.Ask<int>("Enter [yellow]Item ID[/] to purchase:");
            var item = Database.GetItem(id);
            if (item == null || item.Stock <= 0)
            {
                AnsiConsole.MarkupLine("[red1]Item not available![/]");
                Console.ReadKey();
                return;
            }
            if (user.Balance < item.Price)
            {
                AnsiConsole.MarkupLine("[red1]Insufficient balance![/]");
                Console.ReadKey();
                return;
            }

            Database.PurchaseItem(user, item);
        }

        static void AddUser()
        {
            string studentId = AnsiConsole.Ask<string>("Enter Student ID:");
            string name = AnsiConsole.Ask<string>("Enter Name:");
            string password = AnsiConsole.Prompt(new TextPrompt<string>("Enter Password:").Secret());
            Database.AddUser(new User { StudentId = studentId, Name = name, Password = password });
        }

        static void AddItem()
        {
            string name = AnsiConsole.Ask<string>("Enter Item Name:");
            string type = AnsiConsole.Ask<string>("Enter Type:");
            decimal price = AnsiConsole.Ask<decimal>("Enter Price:");
            int stock = AnsiConsole.Ask<int>("Enter Stock:");
            Database.AddItem(new Item { Name = name, Type = type, Price = price, Stock = stock });
        }

        static void DeleteItem()
        {
            int id = AnsiConsole.Ask<int>("Enter [red1]Item ID[/] to delete:");
            Database.DeleteItem(id);
        }

        static void RetrieveVendingBalance(Admin admin)
        {
            decimal vmBalance = Database.GetVendingBalance();
            if (vmBalance <= 0)
            {
                AnsiConsole.MarkupLine("[yellow]No funds in vending machine![/]");
                Console.ReadKey();
                return;
            }

            admin.Balance += vmBalance;
            Database.UpdateAdminBalance(admin);
            Database.ResetVendingBalance();

            AnsiConsole.MarkupLine($"[green1]R{vmBalance:0.00} retrieved from vending machine![/]");
            Console.ReadKey();
        }
    }
}
