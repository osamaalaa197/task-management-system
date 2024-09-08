using System.Security.Claims;

namespace TaskManagement.Extensions;

public static class UserExtensions
{
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    public static bool HasRole(this ClaimsPrincipal user, string role) =>
        user.FindFirst(ClaimTypes.Role)!.Value == role;
}
