// External stuff:
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
// My stuff:
using LOT_FlightManagementAPI.Controllers;
using LOT_FlightManagementAPI.Data;
using LOT_FlightManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LOT_FlightManagement.UnitTests;

public class Tests
{
    private FlightsController controller;
    private int dataSize;
    
    [OneTimeSetUp]
    public void Setup()
    {
        var data = new List<Flight>
        {
            new Flight(1,1, DateTime.Now,"Warsaw-Modlin", "New York, JFK", "Boeing 787-8"),
            new Flight(2, 2, DateTime.Now,"Rzeszow-Jasionka", "Warsaw-Modlin", "Embraer 170" ),
            new Flight(3, 3, DateTime.Now,"London-Luton", "Warsaw-Modlin", "Airbus A321neo" )
        };
        dataSize = data.Count();

        var options = new DbContextOptionsBuilder<FlightContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") 
            .Options;

        // Use a clean instance of FlightContext for each test
        var context = new FlightContext(options);

        // Add test data to the in-memory database
        context.Flights.AddRange(data);
        context.SaveChanges();

        controller = new FlightsController(context);
    }

    [Test, Order(1)]
    public void GetAllTest()
    {
        var result = controller.Get();
        
        // Check if the result got all the records
        Assert.That(dataSize, Is.EqualTo(result.Count()));
        
        // Now check individual records
        int expectedFlightId = 1;
        foreach (var flight in result)
            Assert.That(expectedFlightId++, Is.EqualTo(flight.FlightId));
    }

    [Test, Order(2)]
    public void GetSpecificTest()
    {
        Random random = new Random();
        ushort idToCheck = (ushort)(random.Next(1, dataSize));   
        var result = controller.Get(idToCheck);
        
        Assert.That(idToCheck, Is.EqualTo(result.Result.Value.FlightId));
    }

    [Test, Order(3)]
    public void AddFlightTest()
    {
        Flight newFlight = new Flight(4,4,DateTime.Now,"Warsaw-Modlin", "Rome-Fiumcino", "Boeing 737 Max 8");
        int initialCount = controller.Get().Count();
        
        var result = controller.Post(newFlight);

        var createdFlight = result.Result.Value;
        Assert.That(createdFlight, Is.Not.Null);
        Assert.That(newFlight.FlightId, Is.EqualTo(createdFlight.FlightId));
        Assert.That(controller.Get().Count(), Is.EqualTo(initialCount + 1));
    }
}