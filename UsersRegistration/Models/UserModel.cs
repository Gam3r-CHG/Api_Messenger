namespace UsersRegistration.Models;

public class UserModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}