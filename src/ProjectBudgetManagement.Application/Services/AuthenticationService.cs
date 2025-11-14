using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ProjectBudgetManagement.Application.Services;

/// <summary>
/// Service for JWT token validation and user identity extraction.
/// </summary>
public class AuthenticationService
{
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    /// <summary>
    /// Initializes a new instance of the AuthenticationService class.
    /// </summary>
    /// <param name="jwtSecret">The JWT secret key.</param>
    /// <param name="jwtIssuer">The JWT issuer.</param>
    /// <param name="jwtAudience">The JWT audience.</param>
    public AuthenticationService(string jwtSecret, string jwtIssuer, string jwtAudience)
    {
        if (string.IsNullOrWhiteSpace(jwtSecret))
        {
            throw new ArgumentException("JWT secret cannot be null or empty.", nameof(jwtSecret));
        }

        if (string.IsNullOrWhiteSpace(jwtIssuer))
        {
            throw new ArgumentException("JWT issuer cannot be null or empty.", nameof(jwtIssuer));
        }

        if (string.IsNullOrWhiteSpace(jwtAudience))
        {
            throw new ArgumentException("JWT audience cannot be null or empty.", nameof(jwtAudience));
        }

        _jwtSecret = jwtSecret;
        _jwtIssuer = jwtIssuer;
        _jwtAudience = jwtAudience;
    }

    /// <summary>
    /// Validates a JWT token and returns the claims principal.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The claims principal if valid.</returns>
    /// <exception cref="SecurityTokenException">Thrown when token is invalid.</exception>
    public ClaimsPrincipal ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _jwtIssuer,
            ValidateAudience = true,
            ValidAudience = _jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        return principal;
    }

    /// <summary>
    /// Extracts the user ID from a claims principal.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The user ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user ID claim is not found.</exception>
    public Guid GetUserIdFromPrincipal(ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
                         ?? principal.FindFirst("sub")
                         ?? principal.FindFirst("userId");

        if (userIdClaim == null)
        {
            throw new InvalidOperationException("User ID claim not found in token.");
        }

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new InvalidOperationException($"Invalid user ID format: {userIdClaim.Value}");
        }

        return userId;
    }

    /// <summary>
    /// Extracts the user ID from a JWT token string.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>The user ID.</returns>
    public Guid GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        return GetUserIdFromPrincipal(principal);
    }

    /// <summary>
    /// Generates a JWT token for a user (for testing purposes).
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="userName">The user name.</param>
    /// <param name="expirationMinutes">Token expiration in minutes.</param>
    /// <returns>The generated JWT token.</returns>
    public string GenerateToken(Guid userId, string userName, int expirationMinutes = 60)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim("sub", userId.ToString()),
            new Claim("userId", userId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
