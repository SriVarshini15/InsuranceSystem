using System.Security.Claims;

namespace VehicleInsuranceSystem.Extensions;

public static class AuthorizationExtensions
{
    public static bool CanAccessUser(this ClaimsPrincipal user, int userId)
    {
        if (user.IsInRole("Admin") || user.IsInRole("Officer"))
        {
            return true;
        }

        var claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var currentUserId) && currentUserId == userId;
    }
}
