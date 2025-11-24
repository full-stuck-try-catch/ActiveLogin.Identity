using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Qr;
using ActiveLogin.Authentication.BankId.QrCoder;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddControllers();

builder.Services
    .AddBankId(bankId =>
    {
        bankId.UseSimulatedEnvironment();
        bankId.UseQrCoderQrCodeGenerator();
    })
    .AddAspNetCore(options =>
    {
        options.BankIdLoginReturnUrlPath = "/auth/signin-bankid";
        options.PersonalIdentityNumberValidatorConfiguration.SupportedLength = ActiveLogin.Identity.Swedish.StrictMode.TenOrTwelveDigits;
    });

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = BankIdAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddBankIdAuth(options =>
    {
        options.BankIdSignPath = "/auth/sign";
        options.BankIdCancelPath = "/auth/cancel";
        options.BankIdStatusPath = "/auth/status";
        options.BankIdCompleteSignPath = "/auth/complete";
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddKeycloakAuthentication(builder.Configuration, options =>
{
    options.RequireHttpsMetadata = false;
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/profile"));

app.MapGet("/profile", [Authorize] (HttpContext context) =>
{
    var user = context.User;
    return Results.Ok(new
    {
        user.Identity?.Name,
        Claims = user.Claims.Select(c => new { c.Type, c.Value })
    });
});

app.MapPost("/auth/sign", [AllowAnonymous] (HttpContext context) =>
{
    var props = new AuthenticationProperties
    {
        RedirectUri = "/profile"
    };

    return Results.Challenge(props, [BankIdAuthenticationDefaults.AuthenticationScheme]);
});

app.MapBankIdClientSideApi();
app.MapControllers();

app.Run();
