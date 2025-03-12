using System.Text.RegularExpressions;

public class Validator
{
    // Password validation: Min 10 chars, at least one lowercase, uppercase, digit, and special character
    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{10,}$";
        return Regex.IsMatch(password, pattern);
    }

    // Email validation: Checks for a standard email format
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }
}
