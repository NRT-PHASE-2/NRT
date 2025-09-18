namespace NRTVending
{
    public class Admin
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }

        public Admin(int id, string username, string password, double balance)
        {
            Id = id;
            Username = username ?? "";
            Password = password ?? "";
            Balance = balance;
        }
    }
}
