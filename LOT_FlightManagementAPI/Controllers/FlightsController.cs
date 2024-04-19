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
        private readonly FlightContext _context;

        public FlightsController(FlightContext context)
        {
            _context = context;
        }
        
        // Get all flights
        [HttpGet]
        public IEnumerable<Flight> Get()
        {
            return _context.Flights;
        }

        // Get a specific flight by its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> Get(ushort id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            return flight;
        }

        // Add a new flight
        [HttpPost]
        public async Task<ActionResult<Flight>> Post([FromBody] Flight flight)
        {
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new {id = flight.FlightId}, flight);
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
            if (FlightExists(id))
                return NotFound();
            
            // Mark flight as modified
            // (tells Entity Framework to update after async SaveChanges)
            _context.Entry(modifiedFlight).State = EntityState.Modified;

            // asynchronously save the changes
            await _context.SaveChangesAsync();

            return Ok();
        }
        private bool FlightExists(ushort id) => _context.Flights.Any(f => f.FlightId == id);

        // DELETE api/<FlightsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
