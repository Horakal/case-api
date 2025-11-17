using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestApiCase.Application.Tasks.Services;
using RestApiCase.Application.User.Service;
using RestApiCase.Infrastructure.Repositories;
using RestApiCase.Domain.Tasks.Interfaces;
using RestApiCase.Domain.User.Entities;
using RestApiCase.Domain.User.Interface;
using RestApiCase.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;
using RestApiCase.Application.Tasks.DTOS.ResponseDTO;
using RestApiCase.Application.Tasks.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
{
    options.SuppressMapClientErrors = true;
});
builder.Services.AddEndpointsApiExplorer();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TaskDbContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskValidator>();
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOption =>
{
    var key = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(key))
    {
        throw new InvalidOperationException("JWT key is not configured.");
    }
    var keyBytes = Encoding.UTF8.GetBytes(key);
    jwtOption.SaveToken = true;
    jwtOption.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
    };
});
builder.Services.AddScoped<ITaskService<TaskResponse>, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var errorResponse = new
        {
            StatusCode = context.Response.StatusCode,
            Message = $"Ocorreu um erro. Tente novamente mais tarde. {exception?.Error?.Message}",
        };
        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
    if (!dbContext.Users.Any())
    {
        dbContext.Users.AddRange(
            new User { UserName = "user1", Password = "test", Role = "USER" },
            new User { UserName = "superuser", Password = "superUser", Role = "SUPER_USER" }
        );
        dbContext.SaveChanges();
    }
}

app.Run();