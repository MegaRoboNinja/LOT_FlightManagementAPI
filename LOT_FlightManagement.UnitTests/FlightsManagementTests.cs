// External stuff:
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
// My stuff:
using LOT_FlightManagementAPI.Controllers;
using LOT_FlightManagementAPI.Data;
using LOT_FlightManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LOT_FlightManagement.UnitTests;

// I decided to keep all the tests in a single file
// because they share a one time setup
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

    // TESTS:
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

    /// <summary>
    /// Try te get a flight that is in fact in the database.
    /// </summary>
    [Test, Order(2)]
    public void GetSpecificTest()
    {
        Random random = new Random();
        ushort idToCheck = (ushort)(random.Next(1, dataSize));   
        var result = controller.Get(idToCheck);
        
        Assert.That(idToCheck, Is.EqualTo(result.Result.Value.FlightId));
    }

    /// <summary>
    /// Try to get a flight which is not in the database.
    /// </summary>
    [Test, Order(3)]
    public void GetSpecificInvalidDataTest()
    {
        ushort maxId = controller.Get().Max(f => f.FlightId);
        var result = controller.Get((ushort)(maxId+1));
        
        Assert.That(result.Result.Result, Is.TypeOf<NotFoundResult>());
        Assert.That(result.Result.Value, Is.Null);
    }
    
    /// <summary>
    /// Add a valid flight.
    /// </summary>
    [Test, Order(4)]
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

    /// <summary>
    /// Try to add a flight, whose id is already present in the database.
    /// </summary>
    [Test, Order(5)]
    public void AddFlightInvalidDataTest()
    {
        // Get a flight that's already in the database 
        Flight newFlight = controller.Get().First();
        int initialCount = controller.Get().Count();

        var result = controller.Post(newFlight);
        
        var createdFlight = result.Result.Value;
        // Check if the number of flights in the DB has increased (it shouldn't)
        Assert.That(controller.Get().Count(), Is.EqualTo(initialCount));
        // Check if the returned structure informs properly about data being invalid 
        Assert.That(result.Result.Result, Is.TypeOf<BadRequestObjectResult>());
        // Make sure that 
        Assert.That(result.Result.Value, Is.Null);
    }
    
    /// <summary>
    /// Update a flight record in the database.
    /// </summary>
    [Test, Order(6)]
    public void UpdateTest()
    {
        // Changed route and flightNumber++
        Flight modifiedFlight = new Flight(3,5,DateTime.Now,"Warsaw-Modlin", "London-City", "Boeing 737 Max 8");
        int initialCount = controller.Get().Count();
        
        var result = controller.Put(modifiedFlight.FlightId, modifiedFlight);

        var flightInDB = controller.Get(modifiedFlight.FlightId).Result.Value;
        
        Assert.That(flightInDB, Is.Not.Null);
        Assert.AreEqual(modifiedFlight, flightInDB);
        Assert.That(controller.Get().Count(), Is.EqualTo(initialCount));
    }
}