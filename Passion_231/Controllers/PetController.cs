using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Passion_231.Models;
using Passion_231.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace Passion_231.Controllers
{
    public class PetController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static PetController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            //change this to match your own local port number
            client.BaseAddress = new Uri("https://localhost:44399/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));


            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

        }



        // GET: Pet/List?{PageNum}
        // If the page number is not included, set it to 0
        public ActionResult List(int PageNum = 0)
        {
            // Grab all pets
            string url = "petdata/getpets";
            // Send off an HTTP request
            // GET : /api/petdata/getpets
            // Retrieve response
            HttpResponseMessage response = client.GetAsync(url).Result;
            // If the response is a success, proceed
            if (response.IsSuccessStatusCode)
            {
                // Fetch the response content into IEnumerable<PetDto>
                IEnumerable<PetDto> SelectedPets = response.Content.ReadAsAsync<IEnumerable<PetDto>>().Result;

                // -- Start of Pagination Algorithm --

                // Find the total number of pets
                int PetCount = SelectedPets.Count();
                // Number of pets to display per page
                int PerPage = 4;
                // Determines the maximum number of pages (rounded up), assuming a page 0 start.
                int MaxPage = (int)Math.Ceiling((decimal)PetCount / PerPage) - 1;

                // Lower boundary for Max Page
                if (MaxPage < 0) MaxPage = 0;
                // Lower boundary for Page Number
                if (PageNum < 0) PageNum = 0;
                // Upper Bound for Page Number
                if (PageNum > MaxPage) PageNum = MaxPage;

                // The Record Index of our Page Start
                int StartIndex = PerPage * PageNum;

                //Helps us generate the HTML which shows "Page 1 of ..." on the list view
                ViewData["PageNum"] = PageNum;
                ViewData["PageSummary"] = " " + (PageNum + 1) + " of " + (MaxPage + 1) + " ";

                // -- End of Pagination Algorithm --


                // Send back another request to get pets, this time according to our paginated logic rules
                // GET api/petdata/getpetspage/{startindex}/{perpage}
                url = "petdata/getpetspage/" + StartIndex + "/" + PerPage;
                response = client.GetAsync(url).Result;

                // Retrieve the response of the HTTP Request
                IEnumerable<PetDto> SelectedPetsPage = response.Content.ReadAsAsync<IEnumerable<PetDto>>().Result;

                //Return the paginated of pets instead of the entire list
                return View(SelectedPetsPage);

            }
            else
            {
                // If we reach here something went wrong with our list algorithm
                return RedirectToAction("Error");
            }


        }

        // GET: Pet/Details/5
        public ActionResult Details(int id)
        {
            ShowPet ViewModel = new ShowPet();
            string url = "petdata/findpet/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into pet data transfer object
                PetDto SelectedPet = response.Content.ReadAsAsync<PetDto>().Result;
                ViewModel.Pet = SelectedPet;


                url = "petdata/finddepartmentforpet/" + id;
                response = client.GetAsync(url).Result;
                DepartmentDto SelectedDepartment = response.Content.ReadAsAsync<DepartmentDto>().Result;
                ViewModel.Department = SelectedDepartment;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Pet/Create
        public ActionResult Create()
        {
            UpdatePet ViewModel = new UpdatePet();
            //get information about departments this pet COULD play for.
            string url = "departmentdata/getdepartments";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<DepartmentDto> PotentialDepartments = response.Content.ReadAsAsync<IEnumerable<DepartmentDto>>().Result;
            ViewModel.alldepartments = PotentialDepartments;

            return View(ViewModel);
        }

        // POST: Pet/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Pet PetInfo)
        {
            Debug.WriteLine(PetInfo.PetName);
            string url = "petdata/addpet";
            Debug.WriteLine(jss.Serialize(PetInfo));
            HttpContent content = new StringContent(jss.Serialize(PetInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int petid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = petid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Pet/Edit/5
        public ActionResult Edit(int id)
        {
            UpdatePet ViewModel = new UpdatePet();

            string url = "petdata/findpet/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into pet data transfer object
                PetDto SelectedPet = response.Content.ReadAsAsync<PetDto>().Result;
                ViewModel.pet = SelectedPet;

                //get information about departments this pet COULD play for.
                url = "departmentdata/getdepartments";
                response = client.GetAsync(url).Result;
                IEnumerable<DepartmentDto> PotentialDepartments = response.Content.ReadAsAsync<IEnumerable<DepartmentDto>>().Result;
                ViewModel.alldepartments = PotentialDepartments;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Pet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Pet PetInfo, HttpPostedFileBase PetPic)
        {
            //Debug.WriteLine(PetInfo.PetFirstName);
            string url = "petdata/updatepet/" + id;
            Debug.WriteLine(jss.Serialize(PetInfo));
            HttpContent content = new StringContent(jss.Serialize(PetInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                //only attempt to send pet picture data if we have it
                if (PetPic != null)
                {
                    Debug.WriteLine("Calling Update Image method.");
                    //Send over image data for pet
                    url = "petdata/updatepetpic/" + id;
                    //Debug.WriteLine("Received pet picture "+PetPic.FileName);

                    MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                    HttpContent imagecontent = new StreamContent(PetPic.InputStream);
                    requestcontent.Add(imagecontent, "PetPic", PetPic.FileName);
                    response = client.PostAsync(url, requestcontent).Result;
                }

                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Pet/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "petdata/findpet/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into pet data transfer object
                PetDto SelectedPet = response.Content.ReadAsAsync<PetDto>().Result;
                return View(SelectedPet);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Pet/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "petdata/deletepet/" + id;
            //post body is empty
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
