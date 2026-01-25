namespace BookTheShow.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (emailExists)
            return Result.Failure<UserDto>("A user with this email already exists");

        // Parse role
        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
            return Result.Failure<UserDto>("Invalid user role");

        // Hash password (In production, use proper hashing like BCrypt)
        var passwordHash = HashPassword(request.Password);

        // Create user entity
        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            passwordHash,
            request.DateOfBirth,
            userRole
        );

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var userDto = MapToDto(user);

        return Result.Success(userDto);
    }

    private static string HashPassword(string password)
    {
        // TODO: Replace with BCrypt.Net-Next or ASP.NET Core Identity
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.GetFullName(),
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            Age = DateTime.UtcNow.Year - user.DateOfBirth.Year,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            LastLoginDate = user.LastLoginDate,
            CreatedAt = user.CreatedAt
        };
    }
}
