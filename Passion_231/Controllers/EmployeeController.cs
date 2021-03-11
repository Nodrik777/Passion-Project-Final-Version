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
    public class EmployeeController : Controller
    {
        
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static EmployeeController()
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



        // GET: Employee/List
        public ActionResult List()
        {
            string url = "employeedata/getemployees";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<EmployeeDto> SelectedEmployees = response.Content.ReadAsAsync<IEnumerable<EmployeeDto>>().Result;
                return View(SelectedEmployees);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            UpdateEmployee ViewModel = new UpdateEmployee();

            string url = "employeedata/findemployee/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Employee data transfer object
                EmployeeDto SelectedEmployee = response.Content.ReadAsAsync<EmployeeDto>().Result;
                ViewModel.employee = SelectedEmployee;

                //find departments that are employeeed by this employee
                url = "employeedata/getdepartmentsforemployee/" + id;
                response = client.GetAsync(url).Result;

                //Put data into Employee data transfer object
                IEnumerable<DepartmentDto> SelectedDepartments = response.Content.ReadAsAsync<IEnumerable<DepartmentDto>>().Result;
                ViewModel.employeeddepartments = SelectedDepartments;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");

            }

        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Employee EmployeeInfo)
        {
            Debug.WriteLine(EmployeeInfo.EmployeeFirstName);
            string url = "employeedata/addemployee";
            Debug.WriteLine(jss.Serialize(EmployeeInfo));
            HttpContent content = new StringContent(jss.Serialize(EmployeeInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Employeeid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Employeeid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateEmployee ViewModel = new UpdateEmployee();

            string url = "employeedata/findemployee/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Employee data transfer object
                EmployeeDto SelectedEmployee = response.Content.ReadAsAsync<EmployeeDto>().Result;
                ViewModel.employee = SelectedEmployee;

                //find departments that are employeeed by this employee
                url = "employeedata/getdepartmentsforemployee/" + id;
                response = client.GetAsync(url).Result;

                //Put data into Employee data transfer object
                IEnumerable<DepartmentDto> SelectedDepartments = response.Content.ReadAsAsync<IEnumerable<DepartmentDto>>().Result;
                ViewModel.employeeddepartments = SelectedDepartments;

                //find departments that are not employeeed by this employee
                url = "employeedata/getdepartmentsnotemployeeed/" + id;
                response = client.GetAsync(url).Result;

                //put data into data transfer object
                IEnumerable<DepartmentDto> UnemployeeedDepartments = response.Content.ReadAsAsync<IEnumerable<DepartmentDto>>().Result;
                ViewModel.alldepartments = UnemployeeedDepartments;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");

            }
        }

        // GET: Employee/Unemployee/departmentid/employeeid
        [HttpGet]
        [Route("Employee/Unemployee/{departmentid}/{employeeid}")]
        public ActionResult Unemployee(int departmentid, int employeeid)
        {
            string url = "employeedata/unemployee/" + departmentid + "/" + employeeid;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = employeeid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: employee/employee
        // First employee is the noun (the employee themselves)
        // second employee is the verb (the act of employeeing)
        // The employee(1) employees(2) a department
        [HttpPost]
        [Route("Employee/employee/{departmentid}/{employeeid}")]
        public ActionResult Employee(int departmentid, int employeeid)
        {
            string url = "employeedata/employee/" + departmentid + "/" + employeeid;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = employeeid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Employee EmployeeInfo)
        {
            Debug.WriteLine(EmployeeInfo.EmployeeFirstName);
            string url = "employeedata/updateemployee/" + id;
            Debug.WriteLine(jss.Serialize(EmployeeInfo));
            HttpContent content = new StringContent(jss.Serialize(EmployeeInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("update employee request succeeded");
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                Debug.WriteLine("update employee request failed");
                Debug.WriteLine(response.StatusCode.ToString());
                return RedirectToAction("Error");
            }
        }

        // GET: Employee/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "employeedata/findemployee/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Employee data transfer object
                EmployeeDto SelectedEmployee = response.Content.ReadAsAsync<EmployeeDto>().Result;
                return View(SelectedEmployee);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "employeedata/deleteemployee/" + id;
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
