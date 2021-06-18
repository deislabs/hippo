using System;
using System.Reflection;
using Hippo.Models;
using Hippo.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Hippo.Tests.ApiControllers
{
  public class TestStartup
  {
    private readonly MockTokenIssuer TokenIssuer;
    public TestStartup(MockTokenIssuer tokenIssuer)
    {
      TokenIssuer = tokenIssuer;
    }
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddIdentity<Account, IdentityRole>(cfg =>
      {
          cfg.User.RequireUniqueEmail = true;
      }).AddEntityFrameworkStores<DataContext>();
      services.AddControllers().AddApplicationPart(Assembly.Load("Hippo")).AddControllersAsServices();
      services.AddAuthentication().AddJwtBearer(
        cfg =>
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = TokenIssuer.Issuer,
          ValidAudience = TokenIssuer.Audience,
          IssuerSigningKey = TokenIssuer.SecurityKey
        }
      );


      services.AddAuthorization(options =>
      {
        options.AddPolicy("RequireAdministratorRole",
                  policy => policy.RequireRole("Administrator"));
      });


      services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
      services.AddDbContext<DataContext>(
              options =>
              options.UseInMemoryDatabase("Hippo")
          );


      services.AddScoped<ICurrentUser, ActionContextCurrentUser>();
      services.AddScoped<IUnitOfWork, DbUnitOfWork>();

      services.AddTransient<DataSeeder>();

      services.AddRouting(options => options.LowercaseUrls = true);
      services.AddMvc();
    }

    public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
    {

      app.UseRouting();


      app.UseAuthentication();
      app.UseAuthorization();


      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
      });

    }
  }
}
