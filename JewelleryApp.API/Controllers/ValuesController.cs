using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JewelleryApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JewelleryApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]             //Attribute based routing
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;

        }
        // GET api/values
        //Create Record
        [HttpGet]
        //Synchronous Code is when a request comes in, it will be blocked until one code is sent through
        //Asynchronous Code helps with scalability, and multithreads and multiusers. Threads won't be blocked and can handle other requests.
        public async Task<IActionResult> GetValues()      //Instead of ActionResult with IEnumerable we use IActionResult because it allows us to return HTTP responses, instead of just strings.
        //Task represents that many tasks can be run
        {
            var values = await _context.Values.ToListAsync();

            return Ok(values);
        }

        // GET api/values/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await _context.Values.FirstOrDefaultAsync(x => x.id == id);
            return Ok(value);

        }

        // POST api/values
        //Edit Record
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        //Delete Record
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}