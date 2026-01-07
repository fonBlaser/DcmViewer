using DcmViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace DcmViewer.Data;

public class DcmDbContext : DbContext
{
    private readonly string _connectionString;

    public DbSet<LogEntry> Logs => Set<LogEntry>();

    public DcmDbContext(string filePath)
    {
        _connectionString = $"Data Source={filePath};Mode=ReadWriteCreate;Cache=Shared";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<LogEntry>(e =>
        {
            e.ToTable("LogEntries");

            e.HasKey(x => x.Id);

            e.Property(x => x.Timestamp)
                .IsRequired();

            e.Property(x => x.Service)
                .IsRequired()
                .HasMaxLength(32);

            e.Property(x => x.Module)
                .IsRequired()
                .HasMaxLength(32);

            e.Property(x => x.Severity)
                .IsRequired(); ;

            e.HasIndex(x => x.Timestamp);
            e.HasIndex(x => x.Service);
            e.HasIndex(x => x.Module);
            e.HasIndex(x => x.Severity);
        });
    }
}