using System.Reflection;
using System.Security.Claims;
using IdentityServer;
using IdentityServer.Data;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var services = builder.Services;

services.AddDbContext<AppDbContext>(config =>
{
    //config.UseInMemoryDatabase("Memory");
    config.UseSqlServer(connectionString);

});

// register Identity services
services.AddIdentity<IdentityUser, IdentityRole>(config =>
    {
        config.Password.RequiredLength = 4;
        config.Password.RequireDigit = false;
        config.Password.RequireLowercase = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
        //config.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "IdentityServer.Cookie";
    config.LoginPath = "/Auth/Login";
    config.LogoutPath = "/Auth/Logout";
});

services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    //.AddInMemoryApiResources(Configurations.GetApi())
    //.AddInMemoryClients(Configurations.GetClients())
    //.AddInMemoryApiScopes(Configurations.GetApiScope())
    //.AddInMemoryIdentityResources(Configurations.GetIdentityResource())
    //.AddTestUsers(TestUsers.Users)
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
            sql => sql.MigrationsAssembly(migrationsAssembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
            sql => sql.MigrationsAssembly(migrationsAssembly));
    })
    .AddDeveloperSigningCredential();

services.AddControllersWithViews();

var app = builder.Build();


app.UseIdentityServer();

app.UseRouting();
app.UseEndpoints(config =>
{
    config.MapDefaultControllerRoute();
});

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    if (!userManager.Users.Any())
    {
        var user = new IdentityUser("letter");
        userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
        userManager.AddClaimAsync(user, new Claim("letter.boss", "big.cookies")).GetAwaiter().GetResult();
        userManager.AddClaimAsync(user, new Claim("letter.api.boss", "big.api.cookies")).GetAwaiter().GetResult();
    }

    // Db Migration
    scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

    var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    context.Database.Migrate();
    if (!context.Clients.Any())
    {
        foreach (var client in Configurations.GetClients())
        {
            context.Clients.Add(client.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.IdentityResources.Any())
    {
        foreach (var resource in Configurations.GetIdentityResource())
        {
            context.IdentityResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.ApiResources.Any())
    {
        foreach (var resource in Configurations.GetApi())
        {
            context.ApiResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.ApiScopes.Any())
    {
        foreach (var resource in Configurations.GetApiScope())
        {
            context.ApiScopes.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }
}

app.Run();
