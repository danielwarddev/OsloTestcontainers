using Microsoft.EntityFrameworkCore;
using OsloTestcontainers.Api;
using OsloTestcontainers.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddDbContext<BookstoreContext>(options => options.UseNpgsql("Host=localhost;Database=bookstore;Username=postgres;Password=postgres"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }