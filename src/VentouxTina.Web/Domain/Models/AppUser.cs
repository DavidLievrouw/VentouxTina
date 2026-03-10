namespace VentouxTina.Web.Domain.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    /// <summary>BCrypt hash of the password (includes the salt).</summary>
    public string PasswordHash { get; set; } = string.Empty;
}
