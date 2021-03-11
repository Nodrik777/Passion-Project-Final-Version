using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Passion_231.Models;
using Passion_231.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;


namespace Passion_231.Controllers
{
    public class DepartmentController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static DepartmentController()
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



        // GET: Department/List
        public ActionResult List()
        {
            string url = "departmentdata/getdepartments";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<DepartmentDto> SelectedDepartments = response.Content.ReadAsAsync<IEnumerable<DepartmentDto>>().Result;
                return View(SelectedDepartments);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Department/Details/5
        public ActionResult Details(int id)
        {
            ShowDepartment ViewModel = new ShowDepartment();
            string url = "departmentdata/finddepartment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Department data transfer object
                DepartmentDto SelectedDepartment = response.Content.ReadAsAsync<DepartmentDto>().Result;
                ViewModel.department = SelectedDepartment;

                //We don't need to throw any errors if this is null
                //A department not having any pets is not an issue.
                url = "departmentdata/getpetsfordepartment/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                IEnumerable<PetDto> SelectedPets = response.Content.ReadAsAsync<IEnumerable<PetDto>>().Result;
                ViewModel.departmentpets = SelectedPets;


                url = "departmentdata/getemployeesfordepartment/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                //Put data into Department data transfer object
                IEnumerable<EmployeeDto> SelectedEmployees = response.Content.ReadAsAsync<IEnumerable<EmployeeDto>>().Result;
                ViewModel.departmentemployees = SelectedEmployees;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Department DepartmentInfo)
        {
            Debug.WriteLine(DepartmentInfo.DepartmentName);
            string url = "Departmentdata/addDepartment";
            Debug.WriteLine(jss.Serialize(DepartmentInfo));
            HttpContent content = new StringContent(jss.Serialize(DepartmentInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Departmentid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Departmentid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Department/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "departmentdata/finddepartment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Department data transfer object
                DepartmentDto SelectedDepartment = response.Content.ReadAsAsync<DepartmentDto>().Result;
                return View(SelectedDepartment);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Department DepartmentInfo)
        {
            Debug.WriteLine(DepartmentInfo.DepartmentName);
            string url = "departmentdata/updatedepartment/" + id;
            Debug.WriteLine(jss.Serialize(DepartmentInfo));
            HttpContent content = new StringContent(jss.Serialize(DepartmentInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Department/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "departmentdata/finddepartment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Department data transfer object
                DepartmentDto SelectedDepartment = response.Content.ReadAsAsync<DepartmentDto>().Result;
                return View(SelectedDepartment);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "departmentdata/deletedepartment/" + id;
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
