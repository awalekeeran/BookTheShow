using BookTheShow.Application.Common.Models;
using MediatR;

namespace BookTheShow.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;