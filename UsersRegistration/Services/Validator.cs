using System.Text.RegularExpressions;
using UsersRegistration.Models;

namespace UsersRegistration.Services;

public class Validator
{
    public ValidatorResult ValidateLoginModel(LoginModel loginModel)
    {
        List<string> errors = new List<string>();

        var validateEmailResult = ValidateEmail(loginModel.Email);
        if (!validateEmailResult.Valid)
        {
            errors.AddRange(validateEmailResult.Errors);
        }

        var validatePasswordResult = ValidatePassword(loginModel.Password);
        if (!validatePasswordResult.Valid)
        {
            errors.AddRange(validatePasswordResult.Errors);
        }

        return errors.Count > 0
            ? new ValidatorResult {Valid = false, Errors = errors}
            : new ValidatorResult {Valid = true};
    }

    private ValidatorResult ValidateEmail(string email)
    {
        if (IsValidEmail(email))
        {
            return new ValidatorResult {Valid = true};
        }

        return new ValidatorResult {Valid = false, Errors = ["Email is not valid format"]};
    }

    private bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            return false;
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }

    private ValidatorResult ValidatePassword(string password)
    {
        List<string> errors = new List<string>();

        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        var hasMiniMaxChars = new Regex(@"^.{4,16}$");
        var hasLowerChar = new Regex(@"[a-z]+");
        var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password should not be empty");
        }

        if (!hasLowerChar.IsMatch(password))
        {
            errors.Add("Password should contain At least one lower case letter");
        }

        if (!hasUpperChar.IsMatch(password))
        {
            errors.Add("Password should contain At least one upper case letter");
        }

        if (!hasMiniMaxChars.IsMatch(password))
        {
            errors.Add("Password should not be less than 4 or greater than 16 characters");
        }

        if (!hasNumber.IsMatch(password))
        {
            errors.Add("Password should contain At least one numeric value");
        }

        if (!hasSymbols.IsMatch(password))
        {
            errors.Add("Password should contain At least one special case characters");
        }

        return errors.Count > 0
            ? new ValidatorResult {Valid = false, Errors = errors}
            : new ValidatorResult {Valid = true};
    }
}