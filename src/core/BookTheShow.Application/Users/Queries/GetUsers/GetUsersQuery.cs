using BookTheShow.Application.Common.Models;
using BookTheShow.Application.Users.DTOs;
using MediatR;

namespace BookTheShow.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<Result<List<UserDto>>>
{
    public bool? IsActive { get; init; }
    public string? Role { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}