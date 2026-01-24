using System.Collections.Generic;
using BookTheShow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookTheShow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
