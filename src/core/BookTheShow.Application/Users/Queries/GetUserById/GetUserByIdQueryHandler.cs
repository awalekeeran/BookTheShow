using BookTheShow.Application.Common.Interfaces;
using BookTheShow.Application.Common.Models;
using BookTheShow.Application.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookTheShow.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(u => u.Id == request.Id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth,
                Age = DateTime.UtcNow.Year - u.DateOfBirth.Year,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                LastLoginDate = u.LastLoginDate,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return Result.Failure<UserDto>("User not found");

        return Result.Success(user);
    }
}