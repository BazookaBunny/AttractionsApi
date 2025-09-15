
public class UserModel
{
    public Guid Id { get; set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }

    private UserModel() { }

    public UserModel(string username, string passwordHash, UserRole role)
    {
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }
}