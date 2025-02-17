using Agrivision.Backend.Api;
using Agrivision.Backend.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDependencies(builder.Configuration);


builder.Services.AddDbContext<ApplicationUserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDbConnectionString")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Agrivision API"));
}

app.UseHttpsRedirection();

app.UseCors("AllowAny");

app.UseAuthorization();

app.MapControllers();

app.Run();