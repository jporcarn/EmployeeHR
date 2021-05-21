using EmployeeHR.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeLogic _employeeLogic;

        public EmployeeController(IEmployeeLogic employeeLogic)
        {
            this._employeeLogic = employeeLogic;
        }

        // GET: api/<EmployeeController>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var employees = await this._employeeLogic.GetAsync();

            if (employees?.Count == 0)
            {
                return NoContent();
            }

            return Ok(employees);
        }

        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public string GetByIdAsync(int id)
        {
            return "value";
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public void PostAsync([FromBody] string value)
        {
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        public void PutAsync(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public void DeleteAsync(int id)
        {
        }
    }
}