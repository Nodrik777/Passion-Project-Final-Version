using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Passion_231.Models;
using System.Diagnostics;

namespace Passion_231.Controllers
{
    public class DepartmentDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [ResponseType(typeof(IEnumerable<DepartmentDto>))]
        public IHttpActionResult GetDepartments()
        {
            List<Department> Departments = db.Departments.ToList();
            List<DepartmentDto> DepartmentDtos = new List<DepartmentDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Department in Departments)
            {
                DepartmentDto NewDepartment = new DepartmentDto
                {
                    DepartmentID = Department.DepartmentID,
                    DepartmentName = Department.DepartmentName,
                    DepartmentClass = Department.DepartmentClass
                };
                DepartmentDtos.Add(NewDepartment);
            }

            return Ok(DepartmentDtos);
        }


        /// <summary>
        /// Gets a list of pets in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input departmentid</param>
        /// <returns>A list of pets associated with the department</returns>
        /// <example>
        /// GET: api/DepartmentData/GetPetsForDepartment
        /// </example>
        [ResponseType(typeof(IEnumerable<PetDto>))]
        public IHttpActionResult GetPetsForDepartment(int id)
        {
            List<Pet> Pets = db.Pets.Where(p => p.DepartmentID == id)
                .ToList();
            List<PetDto> PetDtos = new List<PetDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Pet in Pets)
            {
                PetDto NewPet = new PetDto
                {
                    PetID = Pet.PetID,
                    PetName = Pet.PetName,
                    PetType = Pet.PetType,
                    PetWeight = Pet.PetWeight,
                    PetAge = Pet.PetAge,
                    DepartmentID = Pet.DepartmentID
                };
                PetDtos.Add(NewPet);
            }

            return Ok(PetDtos);
        }

        /// <summary>
        /// Gets a list or Employees in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input departmentid</param>
        /// <returns>A list of Employees including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/EmployeeData/GetEmployeesForDepartment
        /// </example>
        [ResponseType(typeof(IEnumerable<EmployeeDto>))]
        public IHttpActionResult GetEmployeesForDepartment(int id)
        {
            List<Employee> Employees = db.Employees
                .Where(s => s.Departments.Any(t => t.DepartmentID == id))
                .ToList();
            List<EmployeeDto> EmployeeDtos = new List<EmployeeDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Employee in Employees)
            {
                EmployeeDto NewEmployee = new EmployeeDto
                {
                    EmployeeID = Employee.EmployeeID,
                    EmployeeFirstName = Employee.EmployeeFirstName,
                    EmployeeLastName = Employee.EmployeeLastName,
                    HireDate = Employee.HireDate,
                    EmployeeBio = Employee.EmployeeBio
                };
                EmployeeDtos.Add(NewEmployee);
            }

            return Ok(EmployeeDtos);
        }

        /// <summary>
        /// Finds a particular Department in the database with a 200 status code. If the Department is not found, return 404.
        /// </summary>
        /// <param name="id">The Department id</param>
        /// <returns>Information about the Department, including Department id, bio, first and last name, and departmentid</returns>
        // <example>
        // GET: api/DepartmentData/FindDepartment/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(DepartmentDto))]
        public IHttpActionResult FindDepartment(int id)
        {
            //Find the data
            Department Department = db.Departments.Find(id);
            //if not found, return 404 status code.
            if (Department == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            DepartmentDto DepartmentDto = new DepartmentDto
            {
                DepartmentID = Department.DepartmentID,
                DepartmentName = Department.DepartmentName,
                DepartmentClass = Department.DepartmentClass
            };


            //pass along data as 200 status code OK response
            return Ok(DepartmentDto);
        }

        /// <summary>
        /// Updates a Department in the database given information about the Department.
        /// </summary>
        /// <param name="id">The Department id</param>
        /// <param name="Department">A Department object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/DepartmentData/UpdateDepartment/5
        /// FORM DATA: Department JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateDepartment(int id, [FromBody] Department Department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Department.DepartmentID)
            {
                return BadRequest();
            }

            db.Entry(Department).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Adds a Department to the database.
        /// </summary>
        /// <param name="Department">A Department object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/DepartmentData/AddDepartment
        ///  FORM DATA: Department JSON Object
        /// </example>
        [ResponseType(typeof(Department))]
        [HttpPost]
        public IHttpActionResult AddDepartment([FromBody] Department Department)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Departments.Add(Department);
            db.SaveChanges();

            return Ok(Department.DepartmentID);
        }

        /// <summary>
        /// Deletes a Department in the database
        /// </summary>
        /// <param name="id">The id of the Department to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/DepartmentData/DeleteDepartment/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteDepartment(int id)
        {
            Department Department = db.Departments.Find(id);
            if (Department == null)
            {
                return NotFound();
            }

            db.Departments.Remove(Department);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finds a Department in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Department id</param>
        /// <returns>TRUE if the Department exists, false otherwise.</returns>
        private bool DepartmentExists(int id)
        {
            return db.Departments.Count(e => e.DepartmentID == id) > 0;
        }
    }
}
