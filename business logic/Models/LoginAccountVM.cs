namespace business_logic.Models
{
    public class LoginAccountVM
    {
        public bool Result { get; set; }
        public string? Message { get; set; }
        public int? UserId { get; set; }
        public string? Name { get; set; }
    }
}