//using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Configuration;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Redistest
{
    class Employee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public Employee(string id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }
    }

    class Program
    {
        

        static async Task Main(string[] args)
        {
            //var configuration = ConfigurationOptions.Parse("localhost:6379");
            //var redisConnection = ConnectionMultiplexer.Connect(configuration);
            //var redisCache = redisConnection.GetDatabase();

            //Console.WriteLine("Fetching data with caching:");

            //var cachedData = GetDataWithCaching(redisCache);
            //Console.WriteLine($"Result: {cachedData}");

            //Console.WriteLine("Fetching data without caching:");
            //var uncachedData = GetDataFromDatabase();

            //Console.WriteLine($"Result: {uncachedData}");
            //redisConnection.Close(); //It is important to close the connection
        }

       


    }
}