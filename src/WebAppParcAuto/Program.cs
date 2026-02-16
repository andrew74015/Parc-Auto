using Common.Constants;
using Common.Extensions;
using Database.DataModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Services.Declarations;
using Services.Implementations;
using System.Security.Claims;

namespace WebAppParcAuto
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services
                .AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultDatabaseConnection"),
                    sqlServerOptions =>
                    {
                        sqlServerOptions.MigrationsAssembly("Database");
                        sqlServerOptions.CommandTimeout(120);
                        sqlServerOptions.MigrationsHistoryTable("Migrations");
                    });
                })
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;                    
                })
                .AddCookie(options =>
                {                   
                    options.LoginPath = "/Utilizator/Conectare";
                    options.AccessDeniedPath = "/Utilizator/AccesRestrictionat";
                })
                .AddGoogle(options =>
                {
                    options.Scope.Clear();
                    options.Scope.Add("email");
                    options.Scope.Add("profile");

                    options.ClientId = builder.Configuration["OAuth-Google:ClientId"]
                        ?? throw new InvalidProgramException("invalid client id");

                    options.ClientSecret = builder.Configuration["OAuth-Google:ClientSecret"]
                        ?? throw new InvalidProgramException("invalid client secret");

                    options.Events.OnCreatingTicket = async (context) => {

                        var user = context.Principal;

                        if (user == null || !user.GetIsAuthenticated())
                            return;

                        if (user.Identity is not ClaimsIdentity claimsIdentity)
                            throw new NullReferenceException(nameof(user.Identity));

                        var oldClaims = new List<Claim>();

                        oldClaims.AddRange(user.Claims);

                        foreach (var claim in oldClaims)
                            claimsIdentity.RemoveClaim(claim);

                        var newClaims = new List<Claim>
                        {
                            new(CustomClaimTypes.AuthScheme, SchemeAutentificare.Google),
                            new(CustomClaimTypes.IdentityId, oldClaims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                            new(CustomClaimTypes.Name, oldClaims.First(c => c.Type == ClaimTypes.Name).Value),
                            new(CustomClaimTypes.Email, oldClaims.First(c => c.Type == ClaimTypes.Email).Value.ToLower())
                        };

                        var lookupAngajat = new Angajat
                        {
                            Email = newClaims.FirstOrDefault(c => c.Type == CustomClaimTypes.Email)?.Value ?? string.Empty,
                        };

                        var appDbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                        var angajat = await appDbContext.Angajati.FirstOrDefaultAsync(a => a.Email == lookupAngajat.Email);

                        if (angajat != null)
                            newClaims.Add(new Claim(ClaimTypes.Role, "Angajat"));

                        if (Utilizatori.AdministratoriImpliciti.Contains(lookupAngajat.Email.ToLower()))
                            newClaims.Add(new Claim(ClaimTypes.Role, "Administrator"));

                        claimsIdentity.AddClaims(newClaims);

                        user.LogTo(logger, LogLevel.Information, $"OnSigningIn() ({SchemeAutentificare.Google})");

                        await Task.Yield();
                    };

                    options.Events.OnRemoteFailure = async (context) =>
                    {
                        context.Response.Redirect("/home/index");
                        context.HandleResponse();

                        await Task.CompletedTask;
                    };
                });

            builder.Services
                .AddSingleton<IDatabaseInitializer, DatabaseInitializer>();

            var app = builder.Build();

            app.Services.GetRequiredService<IDatabaseInitializer>().Initialize();

            // Configure the HTTP request pipeline.

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
