using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var factory = new NorrisContextFactory();
using var context = factory.CreateDbContext(args);

if (args[0].Equals("clean"))
{
	await context.Database.ExecuteSqlRawAsync("DELETE FROM NorrisJokes");
	Environment.Exit(0);
}
int numberOfJokes = 5;
numberOfJokes = int.Parse(args[0]);
if (numberOfJokes < 1 || numberOfJokes > 10)
{
	Console.WriteLine("The maximum number of jokes to be imported is 10.");
	Environment.Exit(1);
}

var httpClient = new HttpClient();


Console.WriteLine(numberOfJokes);

using var transaction = await context.Database.BeginTransactionAsync();
int countretries = 0;
for (int i = 0; i < numberOfJokes; i++)
{
	bool newOrExpicit = false;
	do
	{
		HttpResponseMessage resp = await httpClient.GetAsync("https://api.chucknorris.io/jokes/random");
		resp.EnsureSuccessStatusCode();
		string body = await resp.Content.ReadAsStringAsync();
		var norrisJoke = JsonSerializer.Deserialize<NorrisJoke>(body);
		if (norrisJoke.Categories.Contains("expicit") || norrisJoke.Equals(null))
		{
			newOrExpicit = true;
			continue;
		}
		var chuckNorrisJoke = new ChuckNorrisJoke()
		{ ChuckNorrisId = norrisJoke.Id, Url = norrisJoke.Url, Joke = norrisJoke.Value };
		Console.WriteLine(norrisJoke.Value);
		if (!context.NorrisJokes.Any(njoke => njoke.ChuckNorrisId.Equals(chuckNorrisJoke.Id)))
		{
			context.NorrisJokes.Add(chuckNorrisJoke);
			newOrExpicit = true;
		}
		else
		{
			countretries++;
			continue;
		}
		if (countretries.Equals(10))
		{
			Console.WriteLine("All Juck Norris Jokes are imported");
			Environment.Exit(0);
		}
	} while (!newOrExpicit);
}
await transaction.CommitAsync();
await context.SaveChangesAsync();


class ChuckNorrisJoke
{
	public int Id { get; set; }

	[MaxLength(40)]
	public string ChuckNorrisId { get; set; }

	[MaxLength(1024)]
	public string Url { get; set; }

	public string Joke { get; set; }
}

class NorrisContext : DbContext
{
	public DbSet<ChuckNorrisJoke> NorrisJokes { get; set; }

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

