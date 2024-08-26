using ElProjectGrande.Data;
using ElProjectGrande.Services;

var builder = WebApplication.CreateBuilder(args);


DotNetEnv.Env.Load(".env");
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiDbContext>();
builder.Services.AddSingleton<IUserFactory, UserFactory>();
builder.Services.AddSingleton<IUserVerifier, UserVerifier>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();