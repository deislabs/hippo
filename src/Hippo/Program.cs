using System.Text;
using System.Text.Json.Serialization;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Schedulers;
using Hippo.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddMvc().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new
    JsonStringEnumConverter()));

// authentication/authorization
builder.Services.AddIdentity<Account, IdentityRole>(cfg =>
    {
        cfg.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<DataContext>();

builder.Services.AddAuthentication().AddCookie().AddJwtBearer(cfg =>
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new
    SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdministratorRole",
            policy => policy.RequireRole("Administrator"));
    });

// data context
switch (builder.Configuration.GetValue<string>("Database:Driver", "inmemory").ToLower())
{
    case "inmemory":
        builder.Services.AddDbContext<DataContext, InMemoryDataContext>();
        break;
    case "postgresql":
        builder.Services.AddDbContext<DataContext, PostgresDataContext>();
        break;
    case "sqlite":
        builder.Services.AddDbContext<DataContext, SqliteDataContext>();
        break;
}

builder.Services.AddScoped<ICurrentUser, ActionContextCurrentUser>();
builder.Services.AddScoped<IUnitOfWork, DbUnitOfWork>();
builder.Services.AddSingleton<ITaskQueue<ChannelReference>, TaskQueue<ChannelReference>>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

// job scheduler
builder.Services.AddHostedService<ChannelUpdateBackgroundService>();
switch (builder.Configuration.GetValue<string>("Scheduler:Driver", "wagi").ToLower())
{
    case "wagi":
        builder.Services.AddSingleton<JobScheduler, WagiLocalJobScheduler>();
        break;
}

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(builder.Configuration.GetValue<int>("Kestrel:Endpoints:Http:Port", 5000));
    options.ListenAnyIP(
    builder.Configuration.GetValue<int>("Kestrel:Endpoints:Https:Port", 5001),
        listenOptions =>
        {
            listenOptions.UseHttps();
        });
}
);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "hippo API",
        Version = "v1"
    });
    c.AddSecurityDefinition("http", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { {
    new OpenApiSecurityScheme {
    Reference = new OpenApiReference {
        Type = ReferenceType.SecurityScheme,
        Id = "http"
    }
    },
    Array.Empty<string>() } });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // run database migrations
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetService<DataContext>();
    dataContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "hippo v1"));
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=App}/{action=Index}/{id?}"
);

app.Run();
