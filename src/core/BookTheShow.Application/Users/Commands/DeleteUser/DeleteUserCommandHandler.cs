using BookTheShow.Application.Common.Interfaces;
using BookTheShow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookTheShow.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
            return Result.Failure("User not found");

        // Soft delete
        user.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}