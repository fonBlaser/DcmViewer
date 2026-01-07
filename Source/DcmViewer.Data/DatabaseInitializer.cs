using Microsoft.EntityFrameworkCore;

namespace DcmViewer.Data;

public static class DatabaseInitializer
{
    public static void Initialize(string filePath)
    {
        string connectionString = $"Data Source={filePath};Mode=ReadWriteCreate;Cache=Shared";
        using DcmDbContext db = new DcmDbContext(connectionString);

        db.Database.Migrate();

        db.Database.OpenConnection();
        db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
        db.Database.ExecuteSqlRaw("PRAGMA busy_timeout = 5000;");
        db.Database.CloseConnection();
    }
}