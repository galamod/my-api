using Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string[] allowedOrigins =
{
    "https://galabot.netlify.app",
    "https://galasoft.netlify.app",
    "https://galaweb.netlify.app"
};

static string ConvertPostgresUrlToConnectionString(string url)
{
    if (string.IsNullOrEmpty(url)) return string.Empty;

    var uri = new Uri(url);
    var host = uri.Host;
    var database = uri.AbsolutePath.TrimStart('/');
    var userInfo = uri.UserInfo.Split(':');
    var username = userInfo[0];
    var password = userInfo.Length > 1 ? userInfo[1] : "";

    return $"Host={host};Port=5432;Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
}


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var dbUrl = builder.Configuration.GetConnectionString("DefaultConnection")
          ?? Environment.GetEnvironmentVariable("DATABASE_URL");

var connectionString = ConvertPostgresUrlToConnectionString(dbUrl);

Console.WriteLine($"Generated connection string: {connectionString}");

builder.Services.AddDbContext<LicenseDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();

// ✅ Получить все ключи
app.MapGet("/api/licenses", async (LicenseDbContext db) =>
    await db.LicenseKeys.ToListAsync());

// ✅ Проверка ключа
app.MapGet("/api/licenses/check/{key}", async (string key, LicenseDbContext db) =>
{
    var license = await db.LicenseKeys.FirstOrDefaultAsync(l => l.Key == key);
    if (license == null)
        return Results.NotFound("Ключ не найден.");

    if (license.IsUnlimited || license.ExpirationDate > DateTime.UtcNow)
        return Results.Ok(license);

    return Results.BadRequest("Ключ истёк.");
});

// ✅ Добавление ключа
app.MapPost("/api/licenses", async (LicenseKey license, LicenseDbContext db) =>
{
    db.LicenseKeys.Add(license);
    await db.SaveChangesAsync();
    return Results.Created($"/api/licenses/{license.Id}", license);
});

// ✅ Удаление ключа
app.MapDelete("/api/licenses/{id}", async (int id, LicenseDbContext db) =>
{
    var license = await db.LicenseKeys.FindAsync(id);
    if (license == null)
        return Results.NotFound();

    db.LicenseKeys.Remove(license);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


// Автоматическая миграция базы
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LicenseDbContext>();
    db.Database.Migrate();
}

app.Run();

