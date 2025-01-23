using Microsoft.Extensions.Caching.StackExchangeRedis;
using Moq;
using redis_poc;
using Redistest;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

public class Tests
{
    private ConnectionMultiplexer _redisConnection;
    private IDatabase _redisCache;
    private RedisHelper _redisHelper;

    public Tests()
    {
        _redisHelper = new RedisHelper("localhost:6379");

    }


    #region String

    [Fact]
    public void SetString()
    {
        // Arrange
        Employee employee = new Employee("1", "John Doe", 30);

        var cacheKey = String.Format("employee:{0}", employee.Id);

        //_redisCache.StringSet(cacheKey, "Cached John Doe", TimeSpan.FromMinutes(10));
        _redisHelper.StringSet(cacheKey, "Cached John Doe");


        // Act
        var result = _redisHelper.StringGet(cacheKey);

        // Assert
        Assert.Equal("Cached John Doe", result);
    }

    [Fact]
    public void SetString_multipleEmployees()
    {
        // Arrange

        for (int i = 0; i < 10; i++)
        {
            Employee employee = new Employee(i.ToString(), String.Format("Employee_{0}", i), 30);

            var cacheKey = String.Format("employees:{0}", employee.Id);

            _redisHelper.StringSet(cacheKey, employee.Name);
        }

        var searchKey = String.Format("employees:{0}", "5");

        // Act
        var result = _redisHelper.StringGet(searchKey);

        // Assert
        Assert.Equal("Employee_5", result);
    }

    [Fact]
    public void SetString_object()
    {
        // Arrange
        Employee employee = new Employee("1", "John Doe", 30);

        var cacheKey = String.Format("employee:{0}", employee.Id);
        var serializedEmployee = JsonSerializer.Serialize(employee);

        _redisHelper.StringSet(cacheKey, serializedEmployee);

        // Act
        var result = _redisHelper.StringGet(cacheKey);

        // Assert
        Assert.Equal(serializedEmployee.ToString(), result.ToString());
    }

    #endregion

    #region Hash


    [Fact]
    public void HashSetCollection()
    {
        // Arrange
        
        
        //Create the employee instances
        Employee employeeJohn = new Employee("1", "John", 30);
        Employee employeeJack = new Employee("2", "Jack", 30);
        Employee employeeLucas = new Employee("3", "Lucas", 30);


        string cacheKey = "employees";

        //Sets the employees in the collection
        _redisHelper.SetCollection<Employee>(
            cacheKey,
            new List<(string, Employee)>
            {
                (employeeJohn.Name, employeeJohn),
                (employeeJack.Name, employeeJack),
                (employeeLucas.Name, employeeLucas)
            },
            TimeSpan.FromMinutes(5));

        // Act

        //Gets all the employees from Redis
        var allEmployees = _redisHelper.GetCollection<Employee>(cacheKey);

        //Gets only Jack from Redis
        var jackAndJohn = _redisHelper.GetItemsFromCollection<Employee>(cacheKey, "Jack");

        // Assert

        Assert.Equal(3, allEmployees.Count());
        Assert.Equal("Jack", jackAndJohn.FirstOrDefault().Name);

    }

    [Fact]
    public void HashSetandGet()
    {
        // Arrange


        //Create the employee instances
        Employee employeeJohn = new Employee("100", "John", 30);


        var cacheKey = String.Format("employee:{0}", employeeJohn.Id);

        //Sets the employees in the collection
        _redisHelper.HashSet<Employee>(
            cacheKey,
            employeeJohn);

        // Act

        //Gets all the employees from Redis
        var result = _redisHelper.HashGet<Employee>(cacheKey);

        // Assert

        Assert.Equal("John", result.Name);

    }


    #endregion

    #region SET


    [Fact]
    public void Set_multiple_FTEEmployees()
    {
        // Arrange

        for (int i = 0; i < 10; i++)
        {
            Employee employee = new Employee(i.ToString(), String.Format("Employee_{0}", i), 30);

            // unique key for the entire SET of strings
            var cacheKey = String.Format("fullTimeEmployees", employee.Id);

            _redisHelper.Set(cacheKey, employee.Name);
        }

        var searchKey = "fullTimeEmployees";

        // Act
        var result = _redisHelper.SetMembers(searchKey);

        // Assert
        Assert.Equal(10, result.Count());
    }


    [Fact]
    public void Set_UserNames()
    {
        // Arrange

        // unique key for the entire SET of strings
        var setKey = String.Format("usernames:unique");

        for (int i = 0; i < 10; i++)
        {
            var userName = String.Format("User_{0}", i.ToString());

            _redisHelper.Set(setKey, userName);
        }

        var memberKey = "User_2";

        // Act
        var result = _redisHelper.IsSetMember(setKey, memberKey);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Set_UserLikes()
    {
        // Arrange

        var userId = "45";

        // unique key for the entire SET of strings
        var setKey = String.Format("users:likes:{0}", userId);

        // create list of items that user likes
        for (int item = 0; item < 20; item ++)
        {
            var itemId = String.Format("Item_{0}", item.ToString());

            _redisHelper.Set(setKey, itemId);
        }


        userId = "46";

        // unique key for the entire SET of strings
        setKey = String.Format("users:likes:{0}", userId);

        // create list of items that user likes
        for (int item = 0; item < 10; item++)
        {
            var itemId = String.Format("Item_{0}", item.ToString());

            _redisHelper.Set(setKey, itemId);
        }



        // Act (Which items does user 45 like?)

        var user45Likes = _redisHelper.SetMembers(String.Format("users:likes:{0}", "45"));

        // Act (Which items does user 46 like?)

        var user46Likes = _redisHelper.SetMembers(String.Format("users:likes:{0}", "46"));

        // Act (Does user with id 45 like item 5?)
        var user45Likes5 = _redisHelper.IsSetMember(String.Format("users:likes:{0}", "45"), "Item_5");


        // Assert
        Assert.Equal(20, user45Likes.Count());
        Assert.Equal(10, user46Likes.Count());
        Assert.True(user45Likes5);
        
        //var memberKey = "User_2";

        //var result = _redisHelper.IsSetMember(setKey, memberKey);

        // Assert
        //Assert.True(result);
    }


    #endregion

    #region LOAD SIMULATION

    [Fact]
    public void LoadMultipleObjects()
    {
        // Arrange

        for (int i = 0; i < 100000; i++)
        {
            Employee employee = new Employee(i.ToString(), String.Format("Employee_{0}", i), 30);

            // unique key for each Hash entry
            var cacheKey = String.Format("employee:{0}", employee.Id);

            //Sets the employees in the collection
            _redisHelper.HashSet<Employee>(
                cacheKey,
                employee);
        }

        // Act

        var searchKey = String.Format("employee:{0}", "500");

        var result = _redisHelper.HashGet<Employee>(searchKey);

        // Assert

        Assert.Equal("Employee_500", result.Name);

    }


    [Fact]
    //create a test to return employees from database and measure the time elapsed
    public void LoadFromDatabase()
    {
        // Arrange
        Employee employee = new Employee("1", "John Doe", 30);

        // Act

        // track start and finish time of the call
        var startTime = DateTime.Now;

        for (int i = 0; i < 1000; i++)
        {
            var result = employee.GetDataFromDatabase();
        }

        var endTime = DateTime.Now;

        var elapsed = endTime - startTime;

        // Assert
        Console.WriteLine($"Elapsed time: {elapsed.TotalMilliseconds} ms");

    }

    [Fact]
    public void RetrieveMultipleObjects()
    {
        // Arrange

        // track start and finish time of the call
        var startTime = DateTime.Now;

        for (int i = 0; i < 100000; i++)
        {
            var searchKey = String.Format("employee:{0}", i.ToString());

            var result = _redisHelper.HashGet<Employee>(searchKey);
        }

        var endTime = DateTime.Now;

        var elapsed = endTime - startTime;

        // Assert
        Console.WriteLine($"Elapsed time: {elapsed.TotalMilliseconds} ms");


    }

    #endregion

    // add set (string) (simple and entire object serialized) --> OK
    // add collection (hash) --> OK
    // add hash to cache
    // set and setmembers
    // queries (see course examples and bees)
    //  # number of likes per employee
    // # number of employees per department
    // performance tests (database x cache)

}
