using Microsoft.Extensions.Configuration;
using ps_redis_demo;
using StackExchange.Redis;
using System.Text.Json;

namespace Redistest
{

    class Program
    {
        private static RedisConnection? _redisConnection;

        static async Task Main(string[] args)
        {
            // Initialize
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            var configuration = builder.Build();
            _redisConnection = await RedisConnection.InitializeAsync(connectionString: configuration["CacheConnection"].ToString());

            try
            {
                Task thread1 = Task.Run(() => RunRedisCommandsAsync("Thread 1"));
                Console.ReadKey();
            }
            finally
            {
                _redisConnection.Dispose();
            }
        }

        private static async Task RunRedisCommandsAsync(string prefix)
        {
            // Put the object in the cache
            var azureAppDev = new Course(1, "Introduction to Application Development on Microsoft Azure", "Matthew Kruczek", "A wonderful course about how to build applications in Azure");
            var result = await _redisConnection.BasicRetryAsync(async (db) => await db.StringSetAsync("azureAppDev", JsonSerializer.Serialize(azureAppDev)));
            Console.WriteLine($"{Environment.NewLine}{prefix}: Cache response from storing serialized Course object: {result}");

            // Get the object from the cache
            var getMessageResult = await _redisConnection.BasicRetryAsync(async (db) => await db.StringGetAsync("azureAppDev"));
            var azureAppDevFromCache = JsonSerializer.Deserialize<Course>(getMessageResult.ToString());
            Console.WriteLine($"{prefix}: Cached Course Object:{Environment.NewLine}");
            Console.WriteLine($"{prefix}: Course.Id : {azureAppDevFromCache.Id}");
            Console.WriteLine($"{prefix}: Course.Name   : {azureAppDevFromCache.Name}");
            Console.WriteLine($"{prefix}: Course.Author   : {azureAppDevFromCache.Author}");
            Console.WriteLine($"{prefix}: Course.Description  : {azureAppDevFromCache.Description}{Environment.NewLine}");
        }
    }
}