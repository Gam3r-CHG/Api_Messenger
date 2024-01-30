namespace UsersRegistration.Models;

public class ValidatorResult
{
    public bool Valid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}