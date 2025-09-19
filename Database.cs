using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace NRTVending
{
    public static class Database
    {
        public static readonly string connString = "Data Source=DESKTOP-VNO2KRP\\SQLEXPRESS02;Initial Catalog=NRTVending;Integrated Security=True;";

        // ----------------- USERS -----------------
        public static User GetUser(string studentId, string password)
        {
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("SELECT * FROM Users WHERE StudentId=@sid AND Password=@pwd", con))
            {
                cmd.Parameters.AddWithValue("@sid", studentId);
                cmd.Parameters.AddWithValue("@pwd", password);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                        return new User
                        {
                            Id = (int)r["Id"],
                            StudentId = r["StudentId"].ToString(),
                            Name = r["Name"].ToString(),
                            Password = r["Password"].ToString(),
                            Balance = Convert.ToDecimal(r["Balance"])
                        };
                }
            }
            return null;
        }

        public static List<User> GetAllUsers()
        {
            var list = new List<User>();
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("SELECT Id, StudentId, Name, Balance FROM Users", con))
            {
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(new User
                        {
                            Id = (int)r["Id"],
                            StudentId = r["StudentId"].ToString(),
                            Name = r["Name"].ToString(),
                            Balance = Convert.ToDecimal(r["Balance"])
                        });
                }
            }
            return list;
        }

        public static void AddUser(User u)
        {
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("INSERT INTO Users(StudentId, Name, Password, Balance) VALUES(@sid,@name,@pwd,0)", con))
            {
                cmd.Parameters.AddWithValue("@sid", u.StudentId);
                cmd.Parameters.AddWithValue("@name", u.Name);
                cmd.Parameters.AddWithValue("@pwd", u.Password);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateUserBalance(User u)
        {
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("UPDATE Users SET Balance=@bal WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@bal", u.Balance);
                cmd.Parameters.AddWithValue("@id", u.Id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ----------------- ITEMS -----------------
        public static Item GetItem(int id)
        {
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("SELECT * FROM Items WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                        return new Item
                        {
                            Id = (int)r["Id"],
                            Name = r["Name"].ToString(),
                            Type = r["Type"].ToString(),
                            Price = Convert.ToDecimal(r["Price"]),
                            Stock = (int)r["Stock"]
                        };
                }
            }
            return null;
        }

        public static List<Item> GetAllItems()
        {
            var list = new List<Item>();
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("SELECT * FROM Items", con))
            {
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(new Item
                        {
                            Id = (int)r["Id"],
                            Name = r["Name"].ToString(),
                            Type = r["Type"].ToString(),
                            Price = Convert.ToDecimal(r["Price"]),
                            Stock = (int)r["Stock"]
                        });
                }
            }
            return list;
        }

        public static void AddItem(Item i)
        {
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("INSERT INTO Items(Name, Type, Price, Stock) VALUES(@name,@type,@price,@stock)", con))
            {
                cmd.Parameters.AddWithValue("@name", i.Name);
                cmd.Parameters.AddWithValue("@type", i.Type);
                cmd.Parameters.AddWithValue("@price", i.Price);
                cmd.Parameters.AddWithValue("@stock", i.Stock);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteItem(int id)
        {
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("DELETE FROM Items WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void PurchaseItem(User user, Item item)
        {
            item.Stock--;
            user.Balance -= item.Price;

            using (var con = new SqlConnection(connString))
            {
                con.Open();
                using (var cmd = new SqlCommand("UPDATE Items SET Stock=@stock WHERE Id=@id", con))
                {
                    cmd.Parameters.AddWithValue("@stock", item.Stock);
                    cmd.Parameters.AddWithValue("@id", item.Id);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd2 = new SqlCommand("UPDATE Users SET Balance=@bal WHERE Id=@uid", con))
                {
                    cmd2.Parameters.AddWithValue("@bal", user.Balance);
                    cmd2.Parameters.AddWithValue("@uid", user.Id);
                    cmd2.ExecuteNonQuery();
                }

                using (var cmd3 = new SqlCommand("UPDATE VendingBalance SET Balance = Balance + @amount", con))
                {
                    cmd3.Parameters.AddWithValue("@amount", item.Price);
                    cmd3.ExecuteNonQuery();
                }
            }
        }

        // ----------------- ADMIN -----------------
        public static Admin GetAdmin(string username, string password)
        {
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("SELECT * FROM Admins WHERE Username=@u AND Password=@p", con))
            {
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                        return new Admin
                        {
                            Id = (int)r["Id"],
                            Username = r["Username"].ToString(),
                            Password = r["Password"].ToString(),
                            Balance = Convert.ToDecimal(r["Balance"])
                        };
                }
            }
            return null;
        }

        public static void UpdateAdminBalance(Admin a)
        {
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("UPDATE Admins SET Balance=@bal WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@bal", a.Balance);
                cmd.Parameters.AddWithValue("@id", a.Id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ----------------- VENDING MACHINE BALANCE -----------------
        public static void AddToVendingBalance(decimal amount) => ExecuteScalar($"UPDATE VendingBalance SET Balance = Balance + {amount}");

        public static decimal GetVendingBalance()
        {
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand("SELECT Balance FROM VendingBalance", con))
            {
                con.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public static void ResetVendingBalance() => ExecuteScalar("UPDATE VendingBalance SET Balance = 0");

        private static void ExecuteScalar(string sql)
        {
            using (var con = new SqlConnection(connString))
            using (var cmd = new SqlCommand(sql, con))
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
