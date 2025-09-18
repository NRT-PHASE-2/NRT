namespace NRTVending
{
    public class User
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }

        public User(int id, string studentId, string name, string password, double balance)
        {
            Id = id;
            StudentId = studentId ?? "";
            Name = name ?? "";
            Password = password ?? "";
            Balance = balance;
        }
    }
}
