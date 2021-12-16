using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using server;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication("OAuth")
    .AddJwtBearer("OAuth", options => 
    {
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = (context) =>
            {
                if(context.Request.Query.ContainsKey("access_token"))
                {
                    context.Token = context.Request.Query["access_token"];
                }
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = Constants.Issuer,
            ValidAudience = Constants.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Constants.Secret))
        };
    });

//services.AddAuthentication("CookieAuth")
//    .AddCookie("CookieAuth", config =>
//    {
//        config.Cookie.Name = "letter.cookie";
//        config.LoginPath = "/Home/Authenticate/";
//    });

services.AddControllersWithViews();

var app = builder.Build();

app.UseRouting();

// who you are?
app.UseAuthentication();

// are you allowed?
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
