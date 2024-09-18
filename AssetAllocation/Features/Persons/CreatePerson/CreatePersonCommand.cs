using AutoMapper;
using MediatR;

namespace AssetAllocation.Api;

public class CreatePersonCommand : IRequest<PersonCreatedResponse>
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int RegistrationNumber { get; set; }
    public string? Department { get; set; }
    public string? Title { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }

    public CreatePersonCommand(string name, string surname, DateTime dateOfBirth, int registrationNumber, string department, string title, string email, string password)
    {
        Name = name;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        RegistrationNumber = registrationNumber;
        Department = department;
        Title = title;
        Email = email;
        Password = password;
    }

    public string[] Roles => [ PersonOperationClaims.Admin, PersonOperationClaims.Write, PersonOperationClaims.Add ];

    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, PersonCreatedResponse>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly IMapper _mapper;

        public CreatePersonCommandHandler(AssetAllocationDbContext assetAllocationDbContext, IMapper mapper)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
            _mapper = mapper;
        }

        public async Task<PersonCreatedResponse> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            if (_assetAllocationDbContext.Persons.Any(p => p.RegistrationNumber == request.RegistrationNumber))
            {
                throw new Exception("Registration number already exists");
            }

            var person = _mapper.Map<Person>(request);

            HashingHelper.CreatePasswordHash(
                request.Password!,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
            );

            person.PasswordHash = passwordHash;
            person.PasswordSalt = passwordSalt;

            await _assetAllocationDbContext.AddAsync(person);
            await _assetAllocationDbContext.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<PersonCreatedResponse>(person);

            return response;
        }
    }
}