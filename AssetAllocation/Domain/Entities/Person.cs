namespace AssetAllocation.Api;

public class Person : Entity<Guid>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int RegistrationNumber { get; set; }
    public string Department { get; set; }
    public string Title { get; set; }
    public string Email { get; set; }
    public AuthenticatorType AuthenticatorType { get; set; }
    public byte[] PasswordSalt { get; set; }
    public byte[] PasswordHash { get; set; }
    public bool Status { get; set; }

    public ICollection<CarAllocation> CarAllocations { get; set; } = null!;
    public ICollection<PhoneAllocation> PhoneAllocations { get; set; } = null!;
    public ICollection<PersonOperationClaim> PersonOperationClaims { get; set; } = null!;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = null!;
    public ICollection<OtpAuthenticator> OtpAuthenticators { get; set; } = null!;
    public ICollection<EmailAuthenticator> EmailAuthenticators { get; set; } = null!;

    public Person() : this(string.Empty, string.Empty, DateTime.MinValue, 0, string.Empty, string.Empty, string.Empty, [], [])
    {
        
    }
    public Person(string name, string surname, DateTime dateOfBirth, int registrationNumber, string department, string title, string email, byte[] passwordSalt, byte[] passwordHash)
    {
        Id = Guid.NewGuid();
        Name = name;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        RegistrationNumber = registrationNumber;
        Department = department;
        Title = title;
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
    }

    public Person(
        Guid id, string name, string surname, DateTime dateOfBirth,
        int registrationNumber, string department, string title,
        string email, byte[] passwordSalt, byte[] passwordHash) : base(id)
    {
        Name = name;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        RegistrationNumber = registrationNumber;
        Department = department;
        Title = title;
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
    }
}
