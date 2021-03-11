using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_231.Models.ViewModels
{
    public class UpdateEmployee
    {
        //base information about the employee
        public EmployeeDto employee { get; set; }

        public IEnumerable<DepartmentDto> employeeddepartments { get; set; }

        public IEnumerable<DepartmentDto> alldepartments { get; set; }
    }
}