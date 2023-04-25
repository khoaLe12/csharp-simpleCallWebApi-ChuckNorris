using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace ChuckNorrisAPI
{
    public class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main()
        {
            var factory = new JokeContextFactory();
            using var context = factory.CreateDbContext();

            int userInput;
            bool isContinue = true;

            while (isContinue)
            {
                Console.Write("Please input some value (1-10 to get jokes, end to finish, clear to reset database): ");
                string? input = Console.ReadLine();

                //FINISH
                if (input.Equals("finish"))
                {
                    break;
                }
                //CLEAR
                else if (input.Equals("clear"))
                {
                    context.Facts.RemoveRange(context.Facts);
                    await context.SaveChangesAsync();
                    Console.WriteLine("CLear all");
                }
                //1-10
                else if (int.TryParse(input, out userInput))
                {
                    if (userInput >= 1 && userInput <= 10)
                    {
                        Console.WriteLine("Loading");
                        using var transaction = await context.Database.BeginTransactionAsync();
                        try
                        {
                            for (int i = 1; i <= userInput; i++)
                            {
                                var fact = await getJoke();
                                if (fact != null)
                                {
                                    if (!await context.Facts.Where(f => f.ChuckNorrisId == fact.ChuckNorrisId).AnyAsync())
                                    {
                                        await context.Facts.AddAsync(fact);
                                        await context.SaveChangesAsync();
                                        Console.WriteLine($"Fact {fact.Id} Added succesfully.");
                                    }
                                    else
                                    {
                                        --i;
                                        Console.WriteLine($"New fact already exist in system, getting new joke!!!!");
                                    }
                                }
                                else if (fact == null)
                                {
                                    --i;
                                    Console.WriteLine($"Can not get joke from web, Load again");
                                }
                            }
                            transaction.Commit();
                        }
                        catch (SqlException ex)
                        {
                            Console.Error.WriteLine($"Something bad happened {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input: the number must be between 1 and 10.");
                    }
                }
                //NO INPUT 
                else if (input.IsNullOrEmpty())
                {
                    Console.WriteLine("Loading");
                    using var transaction = await context.Database.BeginTransactionAsync();
                    try
                    {
                        for (int i = 1; i <= 5; i++)
                        {
                            var fact = await getJoke();
                            if (fact != null)
                            {
                                if (!await context.Facts.Where(f => f.ChuckNorrisId == fact.ChuckNorrisId).AnyAsync())
                                {
                                    await context.Facts.AddAsync(fact);
                                    await context.SaveChangesAsync();
                                    Console.WriteLine($"Fact {fact.Id} Added succesfully.");
                                }
                                else
                                {
                                    --i;
                                    Console.WriteLine($"New fact already exist in system, getting new joke!!!!");
                                }
                            }
                            else if (fact == null)
                            {
                                --i;
                                Console.WriteLine($"Can not get joke from web, Load again");
                            }
                        }
                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        Console.Error.WriteLine($"Something bad happened {ex.Message}");
                    }
                }
                //INVALID CHARACTERS
                else
                {
                    Console.WriteLine("Invalid input: please enter a valid number.");
                }
            }
        }
        private static async Task<Fact?> getJoke()
        {
            try
            {
                string responseBody = await client.GetStringAsync("https://api.chucknorris.io/jokes/random");
                return JsonSerializer.Deserialize<Fact>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }
}