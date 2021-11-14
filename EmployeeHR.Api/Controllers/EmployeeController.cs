using AutoMapper;
using EmployeeHR.Api.Models;
using EmployeeHR.Dto;
using EmployeeHR.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeLogic _employeeLogic;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeLogic employeeLogic, IMapper mapper)
        {
            this._employeeLogic = employeeLogic;
            this._mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync()
        {
            var employees = await this._employeeLogic.GetAsync();

            if (employees?.Count() == 0)
            {
                return NotFound();
            }

            return Ok(employees);
        }

        // GET api/<EmployeeController>/5
        [HttpGet("{id:int}", Name = nameof(GetByIdAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var employee = await this._employeeLogic.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CreateEmployeeRequest createEmployeeRequest)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest();
            }


            var employeeToAdd = this._mapper.Map<Employee>(createEmployeeRequest);

            var employeeAdded = await this._employeeLogic.AddAsync(employeeToAdd);

            var result = CreatedAtRoute(nameof(GetByIdAsync), new { employeeAdded.Id }, employeeAdded);
            return result;
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] Employee employee)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var employeeUpdated = await this._employeeLogic.UpdateAsync(id, employee);

                return Ok(employeeUpdated);
            }
            catch (CustomException ex)
            {
                return StatusCode((int)ex.StatusCode, new { Error = ex.Message, Result = employee });
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, new { Error = ex.Message, Result = employee });
                // throw;
            }
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id, [FromBody] Employee employee)
        {
            try
            {
                int affectedRecords = await this._employeeLogic.DeleteAsync(id, employee);

                return Ok(affectedRecords);
            }
            catch (CustomException ex)
            {
                return StatusCode((int)ex.StatusCode, new { Error = ex.Message, Result = employee });
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, new { Error = ex.Message, Result = employee });
                // throw;
            }
        }
    }
}