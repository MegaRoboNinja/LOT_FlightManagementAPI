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
// because they share a one time setup.
// Hope it's not to long...
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

    /// <summary> Try te get a flight that is in fact in the database. </summary>
    [Test, Order(2)]
    public void GetSpecificTest()
    {
        Random random = new Random();
        ushort idToCheck = (ushort)(random.Next(1, dataSize));   
        var result = controller.Get(idToCheck);
        
        Assert.That(idToCheck, Is.EqualTo(result.Result.Value.FlightId));
    }

    /// <summary> Try to get a flight which is not in the database.</summary>
    [Test, Order(3)]
    public void GetSpecificInvalidDataTest()
    {
        ushort maxId = controller.Get().Max(f => f.FlightId);
        var result = controller.Get((ushort)(maxId+1));
        
        Assert.That(result.Result.Result, Is.TypeOf<NotFoundResult>());
        Assert.That(result.Result.Value, Is.Null);
    }
    
    /// <summary> Add a valid flight.</summary>
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

    /// <summary> Try to add a flight, whose id is already present in the database. </summary>
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
    
    /// <summary>Update a flight record in the database.</summary>
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

    /// <summary> Try to update a flight that doesn't exist in the DB </summary>
    [Test, Order(7)]
    public void UpdateNotExistingTest()
    {
        Flight modifiedFlight = new Flight(7,5,DateTime.Now,"Warsaw-Modlin", "Los Angeles LAX", "Boeing 737 Max 8");

        var result = controller.Put(modifiedFlight.FlightId, modifiedFlight);
        
        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    /// <summary> Try to change a flights id (shouldn't be allowed)</summary>
    [Test, Order(8)]
    public void UpdateID()
    {
        // Get an ID that actually exists in the DB
        ushort existingID = controller.Get().First().FlightId;
        // Create a modified flight with a different ID
        ushort differentID = (ushort)(existingID + 1);
        Flight modifiedFlight = new Flight(differentID,5,DateTime.Now,"Warsaw-Modlin", "Los Angeles LAX", "Boeing 737 Max 8");
        
        // Now try to replace the first one with the second
        var result = controller.Put(existingID, modifiedFlight);
        
        Assert.That(result.Result, Is.TypeOf<BadRequestResult>());
    }

    /// <summary> Delete a flight from the database </summary>
    [Test, Order(9)]
    public void DeleteTest()
    {
        // Get an ID that actually exists in the DB
        ushort existingID = controller.Get().First().FlightId;
        // Save the initial database size
        int initSize = controller.Get().Count();

        var result = controller.Delete(existingID);
        
        Assert.That(result, Is.TypeOf<OkResult>());
        // Make sure that there is one less record in the DB
        Assert.That(controller.Get().Count(), Is.EqualTo(initSize-1));
        // Make sure that the deleted record is no longer in the DB
        Assert.That(controller.FlightExists(existingID), Is.False);
    }
    
    /// <summary> Try to delete a flight of ID not present in the database </summary>
    [Test, Order(10)]
    public void DeleteNotExisingTest()
    {
        // Get an ID that doesn't exists in the DB
        ushort notExistingID = (ushort)(1 + controller.Get().Max(f => f.FlightId) );
        // Save the initial database size
        int initSize = controller.Get().Count();

        var result = controller.Delete(notExistingID);
        
        Assert.That(result, Is.TypeOf<NotFoundResult>());
        // Make sure that there is equal to the record in the DB
        Assert.That(controller.Get().Count(), Is.EqualTo(initSize));
    }
}