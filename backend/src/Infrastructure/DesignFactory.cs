using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AutoMarket.Infrastructure;

public sealed class DesignFactory : IDesignTimeDbContextFactory<MarketplaceDb>
{
    public MarketplaceDb CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__SupabasePostgres")
            ?? "Host=localhost;Database=automarket_design_only;Username=unused";
        return new MarketplaceDb(new DbContextOptionsBuilder<MarketplaceDb>().UseNpgsql(connection,
            n => n.MigrationsHistoryTable("__EFMigrationsHistory", "automarket")).Options);
    }
}
