using BookTheShow.Application.Common.Models;
using BookTheShow.Application.Users.DTOs;
using MediatR;

namespace BookTheShow.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;