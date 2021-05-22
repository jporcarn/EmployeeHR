using EmployeeHR.Dto;
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
        [HttpGet("{id:int}", Name = nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var employee = await this._employeeLogic.GetByIdAsync(id);
            if (employee == null)
            {
                return NoContent();
            }

            return Ok(employee);
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Employee employeeToAdd)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest();
            }

            var employeeAdded = await this._employeeLogic.AddAsync(employeeToAdd);

            var result = CreatedAtRoute(nameof(GetByIdAsync), new { Id = employeeAdded.Id }, employeeAdded);
            return result;
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        public void PutAsync(int id, [FromBody] string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public void DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}