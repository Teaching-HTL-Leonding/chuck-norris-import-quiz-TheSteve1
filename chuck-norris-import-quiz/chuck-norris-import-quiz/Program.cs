using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

var factory = new NorrisContextFactory();
class ChuckNorrisJoke
{
	public int Id { get; set; }
	[MaxLength(40)]
	public int ChuckNorrisId { get; set; }
	[MaxLength(1024)]
	public string Url { get; set; }
	public string Joke { get; set; }
}
class NorrisContext : DbContext
{
	public DbSet<ChuckNorrisJoke> chuckNorrisJokes { get; set; }

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
	public NorrisContext(DbContextOptions<NorrisContext> options)
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
	: base(options)
	{ }
}
class NorrisContextFactory : IDesignTimeDbContextFactory<NorrisContext>
{
	public NorrisContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

		var optionsBuilder = new DbContextOptionsBuilder<NorrisContext>();
		optionsBuilder
			// Uncomment the following line if you want to print generated
			// SQL statements on the console.
			//.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
			.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

		return new NorrisContext(optionsBuilder.Options);
	}
}

