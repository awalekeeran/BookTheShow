namespace BookTheShow.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<UserDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsQueryable();

        // Apply filters
        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        if (!string.IsNullOrEmpty(request.Role))
        {
            if (Enum.TryParse<UserRole>(request.Role, true, out var role))
                query = query.Where(u => u.Role == role);
        }

        // Pagination
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        return Result.Success(users);
    }
}
