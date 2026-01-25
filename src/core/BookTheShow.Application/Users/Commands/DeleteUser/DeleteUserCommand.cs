namespace BookTheShow.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;
