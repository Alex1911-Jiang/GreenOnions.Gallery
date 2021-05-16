namespace GreenOnions.Gallery.Models
{
    public class ResetPasswordModel
    {
        public string Account { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string Verify { get; set; }
    }
}
