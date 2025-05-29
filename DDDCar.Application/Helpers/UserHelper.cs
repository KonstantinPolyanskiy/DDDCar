using System.Security.Claims;
using DDDCar.Core.DomainObjects;

namespace DDDCar.Application.Helpers;

public static class UserHelper
{
    public static User CreateUserByClaims(ClaimsPrincipal userClaims)
    {
        var roles = userClaims.FindAll(ClaimTypes.Role)       
            .Concat(userClaims.FindAll("role"))    
            .SelectMany(c => c.Value.Split(',')) 
            .Select(v => Enum.TryParse<Role>(v, ignoreCase: true, out var r) ? r : (Role?)null)
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .Distinct()
            .ToList();

        var user = new User();
        user.Create(
            userClaims.FindFirstValue(ClaimTypes.GivenName)!,
            userClaims.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
            userClaims.FindFirstValue(ClaimTypes.NameIdentifier)!,
            userClaims.FindFirstValue(ClaimTypes.Email)!,
            roles
        );
        
        return user;
    }
}