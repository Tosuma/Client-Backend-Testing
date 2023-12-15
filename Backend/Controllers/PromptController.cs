using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromptController : ControllerBase
    {
        // GET: api/<PromptController>  
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Console.WriteLine("API call: [get] api/prompt");
            return new string[] { "value1", "value2" };
        }

        // GET api/<PromptController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PromptController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PromptController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PromptController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
