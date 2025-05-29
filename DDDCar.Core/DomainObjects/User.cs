using DDDCar.Core.DomainObjects.Results;

namespace DDDCar.Core.DomainObjects;

public class User
{
    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = string.Empty;
    
    public string Login { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    public IReadOnlyList<Role> Roles { get; private set; } = [];

    public void Create(string firstName, string lastName, string login, string email, IReadOnlyList<Role> roles)
    {
        Id = Guid.NewGuid();
        
        FirstName = firstName;
        LastName = lastName;
        Login = login;
        Email = email;
        Roles = roles;
    }
}