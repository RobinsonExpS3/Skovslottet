using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Slottet.API.Auth;

public class DevAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DevAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // X-Dev-Role header tillader test-scripts at impersonate andre roller
        // uden omkompilering. Default = Admin når headeren mangler.
        // Tilladte værdier: Admin, Employee, Storskaerm.
        var roleHeader = Request.Headers["X-Dev-Role"].FirstOrDefault();
        var role = roleHeader switch
        {
            "Employee"   => "Employee",
            "Storskaerm" => "Storskaerm",
            _            => "Admin"
        };

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "dev-user"),
            new Claim("StaffID", "00000000-0000-0000-0000-000000000001"),
            new Claim(ClaimTypes.Name, "Dev User"),
            new Claim(ClaimTypes.Role, role),
            new Claim("StaffName", "Dev User")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}