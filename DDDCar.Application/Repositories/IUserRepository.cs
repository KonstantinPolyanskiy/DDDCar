using DDDCar.Core.DtoObjects;
using Microsoft.AspNetCore.Identity;

namespace DDDCar.Application.Repositories;

public interface IUserRepository
{
    Task<IdentityResult> AddAsync(UserDto user);
    Task<IdentityResult> UpdateAsync(UserDto user);
    
    Task<IdentityResult> DeleteByLoginAsync(string login);
    Task<IdentityResult> DeleteByIdAsync(Guid id);
    
    Task<IdentityResult> FindByIdAsync(Guid id);
    Task<IdentityResult> FindByLoginAsync(string login);
}