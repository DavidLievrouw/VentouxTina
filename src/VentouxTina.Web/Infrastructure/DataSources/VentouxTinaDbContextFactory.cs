using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace VentouxTina.Web.Infrastructure.DataSources;

public class VentouxTinaDbContextFactory : IDesignTimeDbContextFactory<VentouxTinaDbContext>
{
    public VentouxTinaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VentouxTinaDbContext>();
        // MariaDB 11.4 — matches docker-compose image
        optionsBuilder.UseMySql(
            "Server=localhost;Port=3307;Database=ventouxtina;User=ventouxtina;Password=ventouxtina_dev_pw;",
            new MariaDbServerVersion(new Version(11, 4, 0)),
            mySql => mySql.SchemaBehavior(MySqlSchemaBehavior.Ignore)
        );

        return new VentouxTinaDbContext(optionsBuilder.Options);
    }
}
