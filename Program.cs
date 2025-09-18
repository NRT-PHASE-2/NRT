using System;
using System.Linq;
using Spectre.Console;
using NRTVending;

namespace NRTVending
{
    class Program
    {
        static void Main()
        {
            var db = new Database();
            User currentUser = null;
            Admin currentAdmin = null;

            while (true)
            {
                if (currentUser == null && currentAdmin == null)
                {
                    var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Welcome to [yellow]NRT Vending Machine[/]! Choose an option:")
                            .AddChoices("Login as Student", "Login as Admin", "Exit"));

                    if (choice == "Exit") break;

                    if (choice == "Login as Student")
                    {
                        string studentId = AnsiConsole.Ask<string>("Enter your [green]StudentId[/]:");
                        string password = AnsiConsole.Ask<string>("Enter your [green]Password[/]:");
                        var user = db.TryLogin(studentId, password);
                        if (user == null)
                        {
                            AnsiConsole.MarkupLine("[red]Invalid StudentId or password[/]");
                        }
                        else
                        {
                            currentUser = user;
                            AnsiConsole.MarkupLine($"[green]Welcome {currentUser.Name}![/]");
                        }
                    }
                    else if (choice == "Login as Admin")
                    {
                        string username = AnsiConsole.Ask<string>("Admin Username:");
                        string password = AnsiConsole.Ask<string>("Password:");
                        var admin = db.TryAdminLogin(username, password);
                        if (admin == null)
                        {
                            AnsiConsole.MarkupLine("[red]Invalid admin credentials[/]");
                        }
                        else
                        {
                            currentAdmin = admin;
                            AnsiConsole.MarkupLine($"[yellow]Welcome Admin {currentAdmin.Username}![/]");
                        }
                    }
                }
                else if (currentUser != null)
                {
                    // Student menu
                    AnsiConsole.MarkupLine($"[green]Balance: {currentUser.Balance:C}[/]");

                    var items = db.LoadItems();
                    var table = new Table().Centered();
                    table.AddColumn("ID");
                    table.AddColumn("Name");
                    table.AddColumn("Type");
                    table.AddColumn("Price");
                    table.AddColumn("Stock");

                    foreach (var i in items)
                        table.AddRow(i.Id.ToString(), i.Name, i.Type, i.Price.ToString("C"), i.Stock.ToString());

                    AnsiConsole.Write(table);

                    var action = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("What do you want to do?")
                            .AddChoices("Buy Item", "Logout"));

                    if (action == "Logout")
                    {
                        currentUser = null;
                        continue;
                    }

                    int itemId = AnsiConsole.Ask<int>("Enter Item ID to buy:");
                    var selected = items.FirstOrDefault(x => x.Id == itemId);
                    if (selected == null)
                    {
                        AnsiConsole.MarkupLine("[red]Invalid item ID[/]");
                        continue;
                    }

                    var success = db.BuyItem(currentUser, selected);
                    if (success)
                    {
                        currentUser.Balance -= selected.Price; // update local
                        AnsiConsole.MarkupLine($"[green]Bought {selected.Name}![/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Purchase failed (stock or balance issue).[/]");
                    }
                }
                else if (currentAdmin != null)
                {
                    // Admin menu
                    var adminAction = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[yellow]Admin Panel[/]")
                            .AddChoices("Add User", "Add Item", "Delete Item", "Update Stock", "Withdraw Money", "View Machine Balance", "Logout"));

                    if (adminAction == "Logout")
                    {
                        currentAdmin = null;
                        continue;
                    }

                    if (adminAction == "Add User")
                    {
                        string sid = AnsiConsole.Ask<string>("StudentId:");
                        string name = AnsiConsole.Ask<string>("Name:");
                        string pwd = AnsiConsole.Ask<string>("Password:");
                        double bal = AnsiConsole.Ask<double>("Initial balance:");

                        if (db.RegisterUser(sid, name, pwd, bal))
                            AnsiConsole.MarkupLine("[green]User registered successfully![/]");
                        else
                            AnsiConsole.MarkupLine("[red]StudentId already exists.[/]");
                    }
                    else if (adminAction == "Add Item")
                    {
                        string name = AnsiConsole.Ask<string>("Item Name:");
                        string type = AnsiConsole.Ask<string>("Type (Snack/Drink):");
                        double price = AnsiConsole.Ask<double>("Price:");
                        int stock = AnsiConsole.Ask<int>("Initial Stock:");

                        if (db.AddItem(name, type, price, stock))
                            AnsiConsole.MarkupLine("[green]Item added![/]");
                        else
                            AnsiConsole.MarkupLine("[red]Failed to add item.[/]");
                    }
                    else if (adminAction == "Delete Item")
                    {
                        int id = AnsiConsole.Ask<int>("Enter Item ID to delete:");
                        if (db.DeleteItem(id))
                            AnsiConsole.MarkupLine("[green]Item deleted.[/]");
                        else
                            AnsiConsole.MarkupLine("[red]Delete failed.[/]");
                    }
                    else if (adminAction == "Update Stock")
                    {
                        int id = AnsiConsole.Ask<int>("Enter Item ID to update:");
                        int newStock = AnsiConsole.Ask<int>("Enter new stock:");
                        if (db.UpdateStock(id, newStock))
                            AnsiConsole.MarkupLine("[green]Stock updated.[/]");
                        else
                            AnsiConsole.MarkupLine("[red]Update failed.[/]");
                    }
                    else if (adminAction == "Withdraw Money")
                    {
                        double amt = AnsiConsole.Ask<double>("Enter amount to withdraw:");
                        if (db.WithdrawMoney(currentAdmin.Id, amt))
                        {
                            currentAdmin.Balance += amt;
                            AnsiConsole.MarkupLine($"[green]Withdrew {amt:C}! Admin balance: {currentAdmin.Balance:C}[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Not enough in machine balance.[/]");
                        }
                    }
                    else if (adminAction == "View Machine Balance")
                    {
                        double bal = db.GetMachineBalance();
                        AnsiConsole.MarkupLine($"[blue]Machine Balance: {bal:C}[/]");
                    }
                }
            }
        }
    }
}
