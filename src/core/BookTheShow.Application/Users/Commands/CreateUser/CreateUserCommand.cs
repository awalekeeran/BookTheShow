namespace BookTheShow.Application.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<Result<UserDto>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string Role { get; init; } = "Customer";
}
