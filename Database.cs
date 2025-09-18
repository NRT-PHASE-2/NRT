using DesclutionRecords;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace NRTVending
{
    public class Database
    {
        private readonly string _connectionString;

        public Database()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["NRTDb"].ConnectionString;
        }

        // Admin-only: Register a student
        public bool RegisterUser(string studentId, string name, string password, double balance)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Ensure unique StudentId
                using (var check = new SqlCommand("SELECT COUNT(*) FROM Users WHERE StudentId=@sid", conn))
                {
                    check.Parameters.AddWithValue("@sid", studentId);
                    int existing = Convert.ToInt32(check.ExecuteScalar());
                    if (existing > 0) return false;
                }

                using (var cmd = new SqlCommand("INSERT INTO Users (StudentId, Name, Password, Balance) VALUES (@sid,@n,@p,@b)", conn))
                {
                    cmd.Parameters.AddWithValue("@sid", studentId);
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@p", password);
                    cmd.Parameters.AddWithValue("@b", balance);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        // Student login
        public User TryLogin(string studentId, string password)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT Id, StudentId, Name, Password, Balance FROM Users WHERE StudentId=@sid AND Password=@p", conn))
                {
                    cmd.Parameters.AddWithValue("@sid", studentId);
                    cmd.Parameters.AddWithValue("@p", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User(
                                Convert.ToInt32(reader["Id"]),
                                reader["StudentId"].ToString(),
                                reader["Name"].ToString(),
                                reader["Password"].ToString(),
                                Convert.ToDouble(reader["Balance"])
                            );
                        }
                    }
                }
            }
            return null;
        }

        // Admin login
        public Admin TryAdminLogin(string username, string password)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT Id, Username, Password, Balance FROM Admins WHERE Username=@u AND Password=@p", conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Admin(
                                Convert.ToInt32(reader["Id"]),
                                reader["Username"].ToString(),
                                reader["Password"].ToString(),
                                Convert.ToDouble(reader["Balance"])
                            );
                        }
                    }
                }
            }
            return null;
        }

        // Load items
        public List<Item> LoadItems()
        {
            var list = new List<Item>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT Id, Name, Type, Price, Stock, IsActive FROM Items", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Item(
                            Convert.ToInt32(reader["Id"]),
                            reader["Name"].ToString(),
                            reader["Type"].ToString(),
                            Convert.ToDouble(reader["Price"]),
                            Convert.ToInt32(reader["Stock"])
                        )
                        {
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        };
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        // Purchase item
        public bool BuyItem(User user, Item item)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Check stock
                        using (var stockCmd = new SqlCommand("SELECT Stock FROM Items WHERE Id=@id", conn, tran))
                        {
                            stockCmd.Parameters.AddWithValue("@id", item.Id);
                            int stock = Convert.ToInt32(stockCmd.ExecuteScalar());
                            if (stock <= 0) { tran.Rollback(); return false; }
                        }

                        // Check balance
                        using (var balCmd = new SqlCommand("SELECT Balance FROM Users WHERE Id=@uid", conn, tran))
                        {
                            balCmd.Parameters.AddWithValue("@uid", user.Id);
                            double balance = Convert.ToDouble(balCmd.ExecuteScalar());
                            if (balance < item.Price) { tran.Rollback(); return false; }
                        }

                        // Update stock
                        using (var updStock = new SqlCommand("UPDATE Items SET Stock = Stock - 1 WHERE Id=@id", conn, tran))
                        {
                            updStock.Parameters.AddWithValue("@id", item.Id);
                            updStock.ExecuteNonQuery();
                        }

                        // Deduct user balance
                        using (var updBal = new SqlCommand("UPDATE Users SET Balance = Balance - @price WHERE Id=@uid", conn, tran))
                        {
                            updBal.Parameters.AddWithValue("@price", item.Price);
                            updBal.Parameters.AddWithValue("@uid", user.Id);
                            updBal.ExecuteNonQuery();
                        }

                        // Add to machine balance
                        using (var updMach = new SqlCommand("UPDATE Machine SET Balance = Balance + @price WHERE Id=1", conn, tran))
                        {
                            updMach.Parameters.AddWithValue("@price", item.Price);
                            updMach.ExecuteNonQuery();
                        }

                        // Log purchase
                        using (var ins = new SqlCommand("INSERT INTO Purchases (UserId, ItemId, Price) VALUES (@uid,@iid,@price)", conn, tran))
                        {
                            ins.Parameters.AddWithValue("@uid", user.Id);
                            ins.Parameters.AddWithValue("@iid", item.Id);
                            ins.Parameters.AddWithValue("@price", item.Price);
                            ins.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        // Admin adds item
        public bool AddItem(string name, string type, double price, int stock)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "INSERT INTO Items (Name, Type, Price, Stock, IsActive) VALUES (@n,@t,@p,@s,1)", conn))
                {
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@t", type);
                    cmd.Parameters.AddWithValue("@p", price);
                    cmd.Parameters.AddWithValue("@s", stock);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Admin deletes item
        public bool DeleteItem(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM Items WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Update stock
        public bool UpdateStock(int id, int newStock)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("UPDATE Items SET Stock=@s WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@s", newStock);
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Admin withdraws money
        public bool WithdrawMoney(int adminId, double amount)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        double machineBal;
                        using (var cmd = new SqlCommand("SELECT Balance FROM Machine WHERE Id=1", conn, tran))
                        {
                            machineBal = Convert.ToDouble(cmd.ExecuteScalar());
                        }

                        if (machineBal < amount) { tran.Rollback(); return false; }

                        // Deduct from machine
                        using (var updMach = new SqlCommand("UPDATE Machine SET Balance = Balance - @amt WHERE Id=1", conn, tran))
                        {
                            updMach.Parameters.AddWithValue("@amt", amount);
                            updMach.ExecuteNonQuery();
                        }

                        // Add to admin balance
                        using (var updAdm = new SqlCommand("UPDATE Admins SET Balance = Balance + @amt WHERE Id=@aid", conn, tran))
                        {
                            updAdm.Parameters.AddWithValue("@amt", amount);
                            updAdm.Parameters.AddWithValue("@aid", adminId);
                            updAdm.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        // Get machine balance
        public double GetMachineBalance()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT Balance FROM Machine WHERE Id=1", conn))
                {
                    return Convert.ToDouble(cmd.ExecuteScalar());
                }
            }
        }
    }
}
