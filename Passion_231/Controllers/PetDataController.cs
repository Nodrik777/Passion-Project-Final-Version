using System;
using System.IO;
using System.Web;
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
    public class PetDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [ResponseType(typeof(IEnumerable<PetDto>))]
        [Route("api/petdata/getpets")]
        public IHttpActionResult GetPets()
        {
            List<Pet> Pets = db.Pets.ToList();
            List<PetDto> PetDtos = new List<PetDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Pet in Pets)
            {
                PetDto NewPet = new PetDto
                {
                    PetID = Pet.PetID,
                    PetName = Pet.PetName,
                    PetType = Pet.PetType,
                    PetAge = Pet.PetAge,
                    PetWeight = Pet.PetWeight,
                    PetHasPic = Pet.PetHasPic,
                    PicExtension = Pet.PicExtension,
                    DepartmentID = Pet.DepartmentID
                };
                PetDtos.Add(NewPet);
            }

            return Ok(PetDtos);
        }

        /// <summary>
        /// Gets a list or pets in the database alongside a status code (200 OK). Skips the first {startindex} records and takes {perpage} records.
        /// </summary>
        /// <returns>A list of pets including their ID, bio, first name, last name, and departmentid.</returns>
        /// <param name="StartIndex">The number of records to skip through</param>
        /// <param name="PerPage">The number of records for each page</param>
        /// <example>
        /// GET: api/PetData/GetPets/20/5
        /// Retrieves the first 5 pets after skipping 20 (fifth page)
        /// 
        /// GET: api/PetData/GetPets/15/3
        /// Retrieves the first 3 pets after skipping 15 (sixth page)
        /// 
        /// </example>
        [ResponseType(typeof(IEnumerable<PetDto>))]
        [Route("api/petdata/getpetspage/{StartIndex}/{PerPage}")]
        public IHttpActionResult GetPetsPage(int StartIndex, int PerPage)
        {
            List<Pet> Pets = db.Pets.OrderBy(p => p.PetID).Skip(StartIndex).Take(PerPage).ToList();
            List<PetDto> PetDtos = new List<PetDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Pet in Pets)
            {
                PetDto NewPet = new PetDto
                {
                    PetID = Pet.PetID,
                    PetName = Pet.PetName,
                    PetType = Pet.PetType,
                    PetAge = Pet.PetAge,
                    PetWeight = Pet.PetWeight,
                    PetHasPic = Pet.PetHasPic,
                    PicExtension = Pet.PicExtension,
                    DepartmentID = Pet.DepartmentID
                };
                PetDtos.Add(NewPet);
            }

            return Ok(PetDtos);
        }


        /// <summary>
        /// Finds a particular pet in the database with a 200 status code. If the pet is not found, return 404.
        /// </summary>
        /// <param name="id">The pet id</param>
        /// <returns>Information about the pet, including pet id, bio, first and last name, and departmentid</returns>
        // <example>
        // GET: api/PetData/FindPet/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(PetDto))]
        public IHttpActionResult FindPet(int id)
        {
            //Find the data
            Pet Pet = db.Pets.Find(id);
            //if not found, return 404 status code.
            if (Pet == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            PetDto PetDto = new PetDto
            {
                PetID = Pet.PetID,
                PetName = Pet.PetName,
                PetType = Pet.PetType,
                PetAge = Pet.PetAge,
                PetWeight = Pet.PetWeight,
                PetHasPic = Pet.PetHasPic,
                PicExtension = Pet.PicExtension,
                DepartmentID = Pet.DepartmentID
            };


            //pass along data as 200 status code OK response
            return Ok(PetDto);
        }

        /// <summary>
        /// Finds a particular Department in the database given a pet id with a 200 status code. If the Department is not found, return 404.
        /// </summary>
        /// <param name="id">The pet id</param>
        /// <returns>Information about the Department, including Department id, bio, first and last name, and departmentid</returns>
        // <example>
        // GET: api/DepartmentData/FindDepartmentForPet/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(DepartmentDto))]
        public IHttpActionResult FindDepartmentForPet(int id)
        {
            //Finds the first department which has any pets
            //that match the input petid
            Department Department = db.Departments
                .Where(t => t.Pets.Any(p => p.PetID == id))
                .FirstOrDefault();
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
        /// Updates a pet in the database given information about the pet.
        /// </summary>
        /// <param name="id">The pet id</param>
        /// <param name="pet">A pet object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/PetData/UpdatePet/5
        /// FORM DATA: Pet JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdatePet(int id, [FromBody] Pet pet)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pet.PetID)
            {
                return BadRequest();
            }


            db.Entry(pet).State = EntityState.Modified;
            // Picture update is handled by another method
            db.Entry(pet).Property(p => p.PetHasPic).IsModified = false;
            db.Entry(pet).Property(p => p.PicExtension).IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(id))
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
        /// Receives pet picture data, uploads it to the webserver and updates the pet's HasPic option
        /// </summary>
        /// <param name="id">the pet id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F petpic=@file.jpg "https://localhost:xx/api/petdata/updatepetpic/2"
        /// POST: api/PetData/UpdatePetPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UpdatePetPic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var PetPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (PetPic.ContentLength > 0)
                    {
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(PetPic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/Pets/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Pets/"), fn);

                                //save the file
                                PetPic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the pet haspic and picextension fields in the database
                                Pet SelectedPet = db.Pets.Find(id);
                                SelectedPet.PetHasPic = haspic;
                                SelectedPet.PicExtension = extension;
                                db.Entry(SelectedPet).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Pet Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                            }
                        }
                    }

                }
            }

            return Ok();
        }


        /// <summary>
        /// Adds a pet to the database.
        /// </summary>
        /// <param name="pet">A pet object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/PetData/AddPet
        ///  FORM DATA: Pet JSON Object
        /// </example>
        [ResponseType(typeof(Pet))]
        [HttpPost]
        public IHttpActionResult AddPet([FromBody] Pet pet)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Pets.Add(pet);
            db.SaveChanges();

            return Ok(pet.PetID);
        }

        /// <summary>
        /// Deletes a pet in the database
        /// </summary>
        /// <param name="id">The id of the pet to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/PetData/DeletePet/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeletePet(int id)
        {
            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return NotFound();
            }
            if (pet.PetHasPic && pet.PicExtension != "")
            {
                //also delete image from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Pets/" + id + "." + pet.PicExtension);
                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists... preparing to delete!");
                    System.IO.File.Delete(path);
                }
            }

            db.Pets.Remove(pet);
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
        /// Finds a pet in the system. Internal use only.
        /// </summary>
        /// <param name="id">The pet id</param>
        /// <returns>TRUE if the pet exists, false otherwise.</returns>
        private bool PetExists(int id)
        {
            return db.Pets.Count(e => e.PetID == id) > 0;
        }
    }

}
