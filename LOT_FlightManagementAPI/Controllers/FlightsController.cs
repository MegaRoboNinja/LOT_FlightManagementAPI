using LOT_FlightManagementAPI.Data;
using LOT_FlightManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LOT_FlightManagementAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly FlightContext _database;

        public FlightsController(FlightContext database)
        {
            _database = database;
        }
        
        // Get all flights
        [HttpGet]
        public IEnumerable<Flight> Get()
        {
            return _database.Flights;
        }

        // Get a specific flight by its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> Get(ushort id)
        {
            var flight = await _database.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            return flight; // Ok(flight) maybe
        }

        // Add a new flight
        [HttpPost]
        public async Task<ActionResult<Flight>> Post([FromBody] Flight flight)
        {
            _database.Flights.Add(flight);
            await _database.SaveChangesAsync();

            return flight;
        }

        // Update/modify a flight
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(ushort id, [FromBody] Flight modifiedFlight)
        {
            // Modification cannot change the flights id
            // (could lead to two flights having the same id)
            if (id != modifiedFlight.FlightId)
                return BadRequest();

            // Check if the flight to be modified even exists
            if (!FlightExists(id))
                return NotFound();
            
            // Modify the flight (something I found on stack)
            _database.Entry(await _database.Flights.SingleOrDefaultAsync
                (x => x.FlightId == id)).CurrentValues.SetValues(modifiedFlight);

            // save the changes
            await _database.SaveChangesAsync();

            return Ok();
        }
        private bool FlightExists(ushort id) => _database.Flights.Any(f => f.FlightId == id);

        // Delete a flight from the database
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var flight = _database.Flights.SingleOrDefault( f => f.FlightId == id);
            _database.Flights.Remove(flight);
            _database.SaveChanges();
        }
    }
}
