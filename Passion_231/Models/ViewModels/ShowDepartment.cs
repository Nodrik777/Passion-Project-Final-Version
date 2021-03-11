using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_231.Models.ViewModels
{
    public class ShowDepartment
    {
        //Information about the team
        public DepartmentDto department { get; set; }

        //Information about all players on that team
        public IEnumerable<PetDto> departmentpets { get; set; }

        //Information about all sponsors for that team
        public IEnumerable<EmployeeDto> departmentemployees { get; set; }
    }
}