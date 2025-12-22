var builder = WebApplication.CreateBuilder(args);

#region AWS Configuration  
builder.Configuration.AddSystemsManager(config =>
{
    config.Path = "/smarthub";
    config.Optional = true;
});
var jwtSecret = builder.Configuration["jwt-secret"];
if (!string.IsNullOrWhiteSpace(jwtSecret))
{
    builder.Configuration["JwtSettings:Secret"] = jwtSecret;
}
#endregion AWS Configuration  

// Add services to the container.  

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
