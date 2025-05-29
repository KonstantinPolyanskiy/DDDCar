using DDDCar.Core.EntityObjects;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DDDCar.Infrastructure.Contexts;

public class ApplicationDbContext : IdentityDbContext<UserEntity>
{
    public DbSet<CarEntity> Cars { get; set; }
    public DbSet<PhotoEntity> Photos { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
    { }
}