namespace data_access.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set;}
        public bool IsActive { get; set;}
    }
}