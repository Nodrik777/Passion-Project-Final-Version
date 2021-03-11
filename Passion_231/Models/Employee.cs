using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Passion_231.Models
{
    public class Employee
    {

        [Key]
        public int EmployeeID { get; set; }

        public string EmployeeFirstName { get; set; }

        public string EmployeeLastName { get; set; }

        public DateTime HireDate { get; set; }

        public string EmployeeBio { get; set; }





        public ICollection<Department> Departments { get; set; }
    }

    public class EmployeeDto
    {
        public int EmployeeID { get; set; }

        [DisplayName("First Name")]
        public string EmployeeFirstName { get; set; }

        [DisplayName("Last Name")]
        public string EmployeeLastName { get; set; }

        [DisplayName("Hire Date")]
        public DateTime HireDate { get; set; }

        [DisplayName("Employee Bio")]
        public string EmployeeBio { get; set; }
    }
}
