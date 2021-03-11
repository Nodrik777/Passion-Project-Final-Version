using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_231.Models.ViewModels
{
    //The View Model required to update a player
    public class UpdatePet
    {
        //Information about the player
        public PetDto pet { get; set; }
        //Needed for a dropdownlist which presents the player with a choice of teams to play for
        public IEnumerable<DepartmentDto> alldepartments { get; set; }
    }
}