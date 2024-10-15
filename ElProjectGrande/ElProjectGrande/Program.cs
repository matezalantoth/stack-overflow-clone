using System.Net;
using System.Text;
using DotNetEnv;
using ElProjectGrande.Data;
using ElProjectGrande.Exceptions;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Services.AnswerServices.Factory;
using ElProjectGrande.Services.AnswerServices.Repository;
using ElProjectGrande.Services.AuthenticationServices.AuthenticationSeeder;
using ElProjectGrande.Services.AuthenticationServices.TokenService;
using ElProjectGrande.Services.QuestionServices.Factory;
using ElProjectGrande.Services.QuestionServices.Repository;
using ElProjectGrande.Services.TagServices.Factory;
using ElProjectGrande.Services.TagServices.Repository;
using ElProjectGrande.Services.UserServices.Factory;
using ElProjectGrande.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


Env.Load(".env");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", innerBuilder =>
    {
        innerBuilder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});


builder.Services.AddDbContext<ApiDbContext>(options =>
{

    options.UseMySQL(GetConnString());
});

builder.Services.AddIdentityCore<User>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApiDbContext>();

builder.Services.AddSingleton<IUserFactory, UserFactory>();
builder.Services.AddSingleton<IQuestionFactory, QuestionFactory>();
builder.Services.AddSingleton<IAnswerFactory, AnswerFactory>();
builder.Services.AddSingleton<ITagFactory, TagFactory>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<AuthenticationSeeder>();

AddJwt();

var app = builder.Build();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        await context.Response.WriteAsJsonAsync(exceptionHandlerPathFeature?.Error.Message ?? "Something went wrong.");
        if (exceptionHandlerPathFeature?.Error is UnauthorizedAccessException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsJsonAsync("You have been banned or muted");
        }

        if (exceptionHandlerPathFeature?.Error is NotFoundException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(exceptionHandlerPathFeature.Error.Message);
        }
    });
});



using var scope = app.Services.CreateScope();
var authenticationSeeder = scope.ServiceProvider.GetRequiredService<AuthenticationSeeder>();
authenticationSeeder.SeedRoles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

void AddJwt()
{
    var issuingKey = "";
    if (builder.Environment.IsEnvironment("Testing"))
    {
        issuingKey = "xR3!m9QpT4hK8jLa!Vw6D%Zc5N2fUrT3D";
    }
    else
    {
        issuingKey = Environment.GetEnvironmentVariable("ISSUING_KEY") ??
            throw new Exception("ISSUING_KEY not found");
    }
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "Grande",
                ValidAudience = "Grande",
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(issuingKey)
                )
            };
        });
}

string GetConnString()
{
    var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__MySql");
    if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
        connectionString = $"Server={Environment.GetEnvironmentVariable("LOCAL_SERVER_NAME")};" +
                           $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                           $"User={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
                           $"Password={Environment.GetEnvironmentVariable("DB_USER_PASSWORD")};" +
                           $"Port={Environment.GetEnvironmentVariable("DB_PORT")};";

    if (connectionString == null) throw new Exception("Could not find connection string");
    return connectionString;
}

void AddSwaggerGen()
{
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                []
            }
        });
    });
}

app.Run();
public partial class Program
{
}