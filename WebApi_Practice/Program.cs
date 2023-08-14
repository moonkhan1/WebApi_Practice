using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Security.Claims;
using System.Text;
using WebApi_Practice;
using WebApi_Practice.Data;
using WebApi_Practice.Middlewares;
using WebApi_Practice.Models;
using WebApi_Practice.Repository;
using WebApi_Practice.UnitOfWork;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddFluentValidation();
builder.Services.AddTransient<IValidator<StudentModel>, StudentValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IUser, User>();

builder.Services.AddSingleton<ISingletonOperation, Operation>();
builder.Services.AddTransient<ITransientOperation, Operation>();
builder.Services.AddScoped<IScopedOperation, Operation>();
builder.Services.AddDbContext<StudentDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IStudentRepository, StudentRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient(typeof(IRepository<,>),typeof(EfRepository<,>));

//Rule (Authentication)

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = "example.com",
                       ValidAudience = "example.com",
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]))
                   };
               });

//POLICY (Authorization)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));

    // SuperUser adinda bir policydir hansiki hem Admin hem de User olsun (Hem hemde)
    options.AddPolicy("SuperUser", policy => policy.RequireClaim(ClaimTypes.Role, "Admin")
                                                    .RequireClaim(ClaimTypes.Role, "User"));

    // OneOfTheRoles adinda bir policydir hansiki ya Admin ya de User olsun
    options.AddPolicy("OneOfTheRoles", policy => policy.RequireAssertion(context =>
    context.User.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value is "Admin" or "User")));
});

builder.Services.AddControllers(
    options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<JwtMiddleware>();

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SERILOG Configuration
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
               Path.Combine("D:\\LogFiles", "Application", "diagnostic.txt"),
               rollingInterval: RollingInterval.Day,
               retainedFileCountLimit: 20,
               rollOnFileSizeLimit: true,
               shared: true)
            .CreateLogger();

try
{
Log.Information("Starting web host");
app.Run();

}
catch(Exception exc)
{
    Log.Fatal(exc, "Error occured unexpectedly");

}
finally
{
Log.CloseAndFlush();

}

