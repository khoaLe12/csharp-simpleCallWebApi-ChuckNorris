using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace ChuckNorrisAPI;

class JokeContext : DbContext
{
    public JokeContext(DbContextOptions<JokeContext> options)
        : base(options)
    { }

    public DbSet<Fact> Facts { get; set; }
}

class JokeContextFactory : IDesignTimeDbContextFactory<JokeContext>
{
    public JokeContext CreateDbContext(string[]? args = null)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var optionsBuilder = new DbContextOptionsBuilder<JokeContext>();
        optionsBuilder
            // Uncomment the following line if you want to print generated
            // SQL statements on the console.
            //.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            .UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

        return new JokeContext(optionsBuilder.Options);
    }
}
