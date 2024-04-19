using LOT_FlightManagementAPI.Data;
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
        
        // GET: api/<FlightsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<FlightsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FlightsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
