using Microsoft.AspNetCore.Identity;

// A dummy user class to satisfy the PasswordHasher
public class ApplicationUser {}

public class Program
{
    public static void Main()
    {
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var password = "password";

        // Hash the password
        var hashedPassword = passwordHasher.HashPassword(new ApplicationUser(), password);

        System.Console.WriteLine($"Password: {password}");
        System.Console.WriteLine($"Hashed Password: {hashedPassword}");

        // Verify the password
        var result = passwordHasher.VerifyHashedPassword(new ApplicationUser(), hashedPassword, password);
        System.Console.WriteLine($"Verification result: {result}");

        // Hash it again to show it's different
        var hashedPassword2 = passwordHasher.HashPassword(new ApplicationUser(), password);
        System.Console.WriteLine($"Hashed Password (2nd time): {hashedPassword2}");
        System.Console.WriteLine($"Are the two hashes the same? {hashedPassword == hashedPassword2}");
    }
}
