using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestApiCase.Api.middleware;
using RestApiCase.Application.Tasks.DTOS.ResponseDTO;
using RestApiCase.Application.Tasks.Services;
using RestApiCase.Application.Tasks.Validators;
using RestApiCase.Application.User.Service;
using RestApiCase.Domain.Logging.Interfaces;
using RestApiCase.Domain.Tasks.Interfaces;
using RestApiCase.Domain.User.Entities;
using RestApiCase.Domain.User.Interface;
using RestApiCase.Infrastructure.Data;
using RestApiCase.Infrastructure.Logging;
using RestApiCase.Infrastructure.Repositories;
using System.Security.Cryptography;
using System.Text;

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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
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
builder.Services.AddHealthChecks()
     .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddScoped<ITaskService<TaskResponse>, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRequestLogFactory, RequestLogFactory>();
builder.Services.AddScoped<IRequestLogRepository, RequestLogRepository>();
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
app.UseMiddleware<LogginMiddleware>();
app.UseMiddleware<LoadRolesMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/healthcheck");
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Migra��es pendentes
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
        dbContext.SaveChanges();
    }

    // Seeding de Users (se n�o existirem)
    if (!dbContext.Users.Any())
    {
        await dbContext.Database.BeginTransactionAsync();
        try
        {
            var user1 = new User("user1", "test@gmail.com", BCrypt.Net.BCrypt.HashPassword("user1"));
            var user2 = new User("user2", "test2@gmail.com", BCrypt.Net.BCrypt.HashPassword("user2"));
            var superUser = new User("SuperUser", "super@gmail.com", BCrypt.Net.BCrypt.HashPassword("superUser"));

            dbContext.Users.AddRange(user1, user2, superUser);
            dbContext.SaveChanges();

            var userRole = RestApiCase.Domain.User.Enums.UserRoles.USER;
            var superRole = RestApiCase.Domain.User.Enums.UserRoles.SUPER_USER;
            var user1Role = new Role(userRole, user1.Id);
            var user2Role = new Role(userRole, user2.Id);
            var super1Role = new Role(superRole, user1.Id);
            user1.AddRole(user1Role);
            user2.AddRole(user2Role);
            superUser.AddRole(super1Role);
            dbContext.SaveChanges();

            await dbContext.Database.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbContext.Database.RollbackTransactionAsync();
        }
    }
}

app.Run();