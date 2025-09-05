using System;
using System.Data.SqlClient;
using System.IO;

namespace NRT_Vending_Machine.Data
{
    public class DBConnection
    {
        private readonly string connectionString;

        public DBConnection()
        {
            // Get the application's base directory
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Construct the path to the MDF file
            string mdfPath = Path.Combine(baseDirectory, "MDF", "VendingDB.mdf");
            
            // Ensure the MDF directory exists
            Directory.CreateDirectory(Path.Combine(baseDirectory, "MDF"));
            
            // Construct the connection string with the full path
            connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={mdfPath};";
        }

        public void TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Database connection successful.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Database connection failed: " + ex.Message);
            }
        }
    }
}