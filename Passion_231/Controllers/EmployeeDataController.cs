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
    public class EmployeeDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [ResponseType(typeof(IEnumerable<EmployeeDto>))]
        public IHttpActionResult GetEmployees()
        {
            List<Employee> Employees = db.Employees.ToList();
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
        /// Gets a list or Departments in the database associated with a particular employee. Returns a status code (200 OK)
        /// </summary>
        /// <param name="id">The input employee id</param>
        /// <returns>A list of Departments including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/DepartmentData/GetDepartmentsForEmployee
        /// </example>
        [ResponseType(typeof(IEnumerable<DepartmentDto>))]
        public IHttpActionResult GetDepartmentsForEmployee(int id)
        {
            //sql equivalent
            //select * from departments
            //inner join employeedepartments on employeedepartments.departmentid = departments.departmentid
            //inner join employees on employees.employeeid = employeedepartments.employeeid
            List<Department> Departments = db.Departments
                .Where(t => t.Employees.Any(s => s.EmployeeID == id))
                .ToList();
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
        /// Gets a list or Departments in the database NOT associated with a employee. These could be potentially employeeed departments.
        /// </summary>
        /// <param name="id">The input employee id</param>
        /// <returns>A list of Departments including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/DepartmentData/GetDepartmentsForEmployee
        /// </example>
        [ResponseType(typeof(IEnumerable<DepartmentDto>))]
        public IHttpActionResult GetDepartmentsNotEmployeeed(int id)
        {
            List<Department> Departments = db.Departments
                .Where(t => !t.Employees.Any(s => s.EmployeeID == id))
                .ToList();
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
        /// Finds a particular Employee in the database with a 200 status code. If the Employee is not found, return 404.
        /// </summary>
        /// <param name="id">The Employee id</param>
        /// <returns>Information about the Employee, including Employee id, bio, first and last name, and departmentid</returns>
        // <example>
        // GET: api/EmployeeData/FindEmployee/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(EmployeeDto))]
        public IHttpActionResult FindEmployee(int id)
        {
            //Find the data
            Employee Employee = db.Employees.Find(id);
            //if not found, return 404 status code.
            if (Employee == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            EmployeeDto EmployeeDto = new EmployeeDto
            {
                EmployeeID = Employee.EmployeeID,
                EmployeeFirstName = Employee.EmployeeFirstName,
                EmployeeLastName = Employee.EmployeeLastName,
                HireDate = Employee.HireDate,
                EmployeeBio = Employee.EmployeeBio
            };


            //pass along data as 200 status code OK response
            return Ok(EmployeeDto);
        }

        /// <summary>
        /// Updates a Employee in the database given information about the Employee.
        /// </summary>
        /// <param name="id">The Employee id</param>
        /// <param name="Employee">A Employee object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/EmployeeData/UpdateEmployee/5
        /// FORM DATA: Employee JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateEmployee(int id, [FromBody] Employee Employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Employee.EmployeeID)
            {
                return BadRequest();
            }

            db.Entry(Employee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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
        /// Adds a Employee to the database.
        /// </summary>
        /// <param name="Employee">A Employee object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/Employees/AddEmployee
        ///  FORM DATA: Employee JSON Object
        /// </example>
        [ResponseType(typeof(Employee))]
        [HttpPost]
        public IHttpActionResult AddEmployee([FromBody] Employee Employee)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Employees.Add(Employee);
            db.SaveChanges();

            return Ok(Employee.EmployeeID);
        }

        /// <summary>
        /// Deletes a Employee in the database
        /// </summary>
        /// <param name="id">The id of the Employee to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/Employees/DeleteEmployee/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteEmployee(int id)
        {
            Employee Employee = db.Employees.Find(id);
            if (Employee == null)
            {
                return NotFound();
            }

            db.Employees.Remove(Employee);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes a relationship between a particular department and a employee
        /// </summary>
        /// <param name="departmentid">The department id</param>
        /// <param name="employeeid">The Employee id</param>
        /// <returns>status code of 200 OK</returns>
        [HttpGet]
        [Route("api/employeedata/unemployee/{departmentid}/{employeeid}")]
        public IHttpActionResult Unemployee(int departmentid, int employeeid)
        {
            //First select the employee (also loading in department data)
            Employee SelectedEmployee = db.Employees
                .Include(s => s.Departments)
                .Where(s => s.EmployeeID == employeeid)
                .FirstOrDefault();

            //Then select the department
            Department SelectedDepartment = db.Departments.Find(departmentid);

            //Debug.WriteLine("Selected Employee is.. " + SelectedEmployee.EmployeeName);
            //Debug.WriteLine("Selected Department is.. " + SelectedDepartment.DepartmentName);

            if (SelectedEmployee == null || SelectedDepartment == null || !SelectedEmployee.Departments.Contains(SelectedDepartment))
            {

                return NotFound();
            }
            else
            {
                //Remove the employee from the department
                SelectedEmployee.Departments.Remove(SelectedDepartment);
                db.SaveChanges();
                return Ok();
            }
        }

        /// <summary>
        /// Adds a relationship between a particular department and a employee
        /// </summary>
        /// <param name="departmentid">The department id</param>
        /// <param name="employeeid">The Employee id</param>
        /// <returns>status code of 200 OK</returns>
        [HttpGet]
        [Route("api/employeedata/employee/{departmentid}/{employeeid}")]
        public IHttpActionResult Employee(int departmentid, int employeeid)
        {
            //First select the employee (also loading in department data)
            Employee SelectedEmployee = db.Employees
                .Include(s => s.Departments)
                .Where(s => s.EmployeeID == employeeid)
                .FirstOrDefault();

            //Then select the department
            Department SelectedDepartment = db.Departments.Find(departmentid);

            //Debug.WriteLine("Selected Employee is.. " + SelectedEmployee.EmployeeName);
            //Debug.WriteLine("Selected Department is.. " + SelectedDepartment.DepartmentName);

            if (SelectedEmployee == null || SelectedDepartment == null || SelectedEmployee.Departments.Contains(SelectedDepartment))
            {

                return NotFound();
            }
            else
            {
                //Remove the employee from the department
                SelectedEmployee.Departments.Add(SelectedDepartment);
                db.SaveChanges();
                return Ok();
            }
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
        /// Finds a Employee in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Employee id</param>
        /// <returns>TRUE if the Employee exists, false otherwise.</returns>
        private bool EmployeeExists(int id)
        {
            return db.Employees.Count(e => e.EmployeeID == id) > 0;
        }

        private bool EmployeeAssociated(int departmentid, int employeeid)
        {
            return true;
        }
    }
}
