using StackExchange.Redis;
using System.Threading;
using System;

class Employee
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }


    public Employee() { }

    public Employee(string employeeId, string name, int age)
    {
        Id = employeeId;
        Name = name;
        Age = age;
    }

    public string GetDataFromDatabase()
    {
        // Simulate fetching data from the database
        // Replace this with your actual database fetching logic
        Thread.Sleep(200); // Simulating latency

        return "Employee data from database";
    }

}