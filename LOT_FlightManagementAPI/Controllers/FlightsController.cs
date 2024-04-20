using LOT_FlightManagementAPI.Data;
using LOT_FlightManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        
        ///<summary> Get all flights </summary>
        [HttpGet]
        public IEnumerable<Flight> Get()
        {
            return _database.Flights;
        }

        ///<summary> Get a specific flight by its ID </summary>
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

        ///<summary> Add a new flight. I realise that providing the id is redundant
        /// but it somehow looks better to me this way.</summary>
        [HttpPost]
        public async Task<ActionResult<Flight>> Post([FromBody] Flight flight)
        {
            // Make sure that the id is unique
            if (_database.Flights.Any(other => other.FlightId == flight.FlightId))
                return BadRequest("Flight with the same ID already exists");
            
            _database.Flights.Add(flight);
            await _database.SaveChangesAsync();

            return flight;
        }

        ///<summary> Update/modify a flight</summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(ushort id, [FromBody] Flight modifiedFlight)
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

        /// <summary>Delete a flight from the database</summary>
        [HttpDelete("{id}")]
        public ActionResult Delete(ushort id)
        {
            if (!FlightExists(id))
                return NotFound();
            var flight = _database.Flights.SingleOrDefault( f => f.FlightId == id);
            _database.Flights.Remove(flight);
            _database.SaveChanges();
            return Ok();
        }
        
        /// <summary>Check if the database has a record of a flight of a given ID.
        /// It has to be marked as NonAction because the Swagger/OpenAPI confuse it for
        /// a Http endpoint.</summary>
        [NonAction]
        public bool FlightExists(ushort id) => _database.Flights.Any(f => f.FlightId == id);
    }
}
