using LOT_FlightManagementAPI.Data;
using LOT_FlightManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<Flight>> Get(UInt16 id)
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

        // PUT api/<FlightsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FlightsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
