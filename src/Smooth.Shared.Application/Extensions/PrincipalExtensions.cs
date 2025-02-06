using IdentityModel;
using System.Diagnostics;
using System.Security.Claims;

namespace Smooth.Shared.Application.Extensions;

public static class PrincipalExtensions
{
    /// <summary>
    /// Gets the display name of the principal.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns>The display name if found; otherwise, an empty string.</returns>
    [DebuggerStepThrough]
    public static string GetDisplayName(this ClaimsPrincipal principal)
    {
        if (principal == null) return string.Empty;

        var name = principal.FindFirst(JwtClaimTypes.Name)?.Value;
        if (!string.IsNullOrWhiteSpace(name)) return name ?? string.Empty;

        var sub = principal.FindFirst(JwtClaimTypes.Subject)?.Value;
        if (!string.IsNullOrWhiteSpace(name)) return sub ?? string.Empty;

        return string.Empty;
    }


    /// <summary>
    /// Determines whether the principal is authenticated.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns><c>true</c> if the principal is authenticated; otherwise, <c>false</c>.</returns>
    [DebuggerStepThrough]
    public static bool IsAuthenticated(this ClaimsPrincipal principal)
    {
        return principal != null && principal.Identity != null && principal.Identity.IsAuthenticated;
    }


    /// <summary>
    /// Gets the user ID of the principal.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns>The user ID if found; otherwise, an empty string.</returns>
    //[DebuggerStepThrough]
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.FindFirst(c => c.Type == "sub")?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            return new Guid(userId);
        }

        return Guid.Empty;
    }
}
