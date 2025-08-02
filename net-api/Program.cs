using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace net_api;

public class Program
{
    public static void Main(string[] args)
    {
        var relativePath = "./db/app.db";
        var absolutePath = Path.GetFullPath(relativePath);
        var dbDirectory = Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
        {
            Directory.CreateDirectory(dbDirectory);
        }
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<AppDbContext>(option=> option.UseSqlite($"Data Source={relativePath}"));
        
        
        var app = builder.Build();

        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var dbFilePath = new SqliteConnectionStringBuilder(connectionString).DataSource;
        
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!File.Exists(dbFilePath))
            {
                dbContext.Database.EnsureCreated(); 
            }
            else
            {
                dbContext.Database.Migrate(); 
            }
        }

        
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
    }
}