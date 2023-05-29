using FarmCentral.Data;
using FarmCentral.Models;
using FarmCentral.Models.ModelsDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FarmCentral.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FarmCentralDbContext _context;

        public HomeController(ILogger<HomeController> logger, FarmCentralDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //Login view
        public IActionResult Index()
        {
            return View();
        }

        //Populate DB for testing purposes
        public async Task<IActionResult> PopulateDB()
        {
            //Linq queries adapted from
            //https://www.tutorialsteacher.com/linq/what-is-linq
            //accessed 29 May 2023
            //Find existing test data
            var query = from e in _context.Employees
                        where e.EmployeeEmail == "ted@gmail.com"
                        select e;
            var test = await query.ToListAsync();
            if (test.Count != 0)
            {
                //Don't populate twice
                return RedirectToAction(nameof(Index));
            }
            try
            {
                List<Employee> employees = new List<Employee>()
                {
                    new Employee() { EmployeeEmail = "ted@gmail.com", EmployeeName = "Ted", EmployeePassword = FarmCentralDbContext.Hash("ted")},
                    new Employee() { EmployeeEmail = "joe@gmail.com", EmployeeName = "Joe", EmployeePassword = FarmCentralDbContext.Hash("joe")},
                    new Employee() { EmployeeEmail = "max@gmail.com", EmployeeName = "Max", EmployeePassword = FarmCentralDbContext.Hash("max")},
                    new Employee() { EmployeeEmail = "billy@gmail.com", EmployeeName = "Billy", EmployeePassword = FarmCentralDbContext.Hash("billy")},
                    new Employee() { EmployeeEmail = "steve@gmail.com", EmployeeName = "Steve", EmployeePassword = FarmCentralDbContext.Hash("steve")},
                };

                List<Farmer> farmers = new List<Farmer>()
                {
                    new Farmer() { FarmerEmail = "bob@gmail.com", FarmerName = "Bob", FarmerPassword = FarmCentralDbContext.Hash("bob") },
                    new Farmer() { FarmerEmail = "mary@gmail.com", FarmerName = "Mary", FarmerPassword = FarmCentralDbContext.Hash("mary")},
                    new Farmer() { FarmerEmail = "jane@gmail.com", FarmerName = "Jane", FarmerPassword = FarmCentralDbContext.Hash("jane")},
                    new Farmer() { FarmerEmail = "alex@gmail.com", FarmerName = "Alex", FarmerPassword = FarmCentralDbContext.Hash("alex")},
                    new Farmer() { FarmerEmail = "tom@gmail.com", FarmerName = "Tom", FarmerPassword = FarmCentralDbContext.Hash("tom")}
                };

                foreach (var farmer in farmers)
                {
                    farmer.FarmerProducts.Add(new Product() { ProductType = "Dairy", ProductDescription = "Milk", ProductPrice = 34.99M, ProductListDate = new DateTime(2023, 05, 01), Farmer = farmer });
                    farmer.FarmerProducts.Add(new Product() { ProductType = "Dairy", ProductDescription = "Cheese", ProductPrice = 74.99M, ProductListDate = new DateTime(2023, 05, 11), Farmer = farmer });
                    farmer.FarmerProducts.Add(new Product() { ProductType = "Meat", ProductDescription = "Lamb Chops", ProductPrice = 114.99M, ProductListDate = new DateTime(2023, 03, 15), Farmer = farmer });
                    farmer.FarmerProducts.Add(new Product() { ProductType = "Meat", ProductDescription = "Pork Sausage", ProductPrice = 84.99M, ProductListDate = new DateTime(2023, 04, 29), Farmer = farmer });
                    farmer.FarmerProducts.Add(new Product() { ProductType = "Fruit", ProductDescription = "Apple", ProductPrice = 7.99M, ProductListDate = new DateTime(2023, 01, 01), Farmer = farmer });
                    farmer.FarmerProducts.Add(new Product() { ProductType = "Vegetables", ProductDescription = "Corn", ProductPrice = 19.99M, ProductListDate = new DateTime(2023, 02, 24), Farmer = farmer });
                }

                _context.Employees.AddRange(employees);
                _context.Farmers.AddRange(farmers);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Login(SigninViewModel signin)
        {
            //Generate hash for submitted password
            string hash = FarmCentralDbContext.Hash(signin.Password);

            //Check employees first because there are most likely less
            var employee = _context.Employees.FirstOrDefault(m => m.EmployeeEmail.Equals(signin.Email));
            if (employee != null && employee.EmployeeEmail.Equals(signin.Email) && employee.EmployeePassword.Equals(hash))
            {
                //Login and set logged in employee
                EmployeesController.LoggedInEmployee = employee;
                //Confirm farmer is null
                FarmersController.LoggedInFarmer = null;
                //Redirect if no error
                return RedirectToAction("Index", "Employees");
            }

            var farmer = _context.Farmers.FirstOrDefault(m => m.FarmerEmail.Equals(signin.Email));
            if (farmer != null && farmer.FarmerEmail.Equals(signin.Email) && farmer.FarmerPassword.Equals(hash))
            {
                //Login and set logged in farmer
                FarmersController.LoggedInFarmer = farmer;
                //Confirm employee is null
                EmployeesController.LoggedInEmployee = null;
                //Redirect if no error
                return RedirectToAction("Index", "Farmers");
            }

            //Login failed, show error
            ViewBag.LoginError = "Incorrect email or password.";
            return View("~/Views/Home/Index.cshtml");
        }

        //Log out and return to sign in page
        public IActionResult LogOut()
        {
            EmployeesController.LoggedInEmployee = null;
            FarmersController.LoggedInFarmer = null;
            ProductsController.CurrentFarmer = null;
            return RedirectToAction("Index", "Home");
        }

        //Register view
        public IActionResult Register()
        {
            return View();
        }

        //Register view
        public IActionResult Edit()
        {
            if (EmployeesController.LoggedInEmployee != null)
            {
                //Pass route values adapted from
                //https://stackoverflow.com/questions/1257482/redirecttoaction-with-parameter
                //User answered
                //https://stackoverflow.com/users/104252/kurt-schindler
                //Accessed 27 May 2023
                return RedirectToAction("Edit", "Employees", new { id = EmployeesController.LoggedInEmployee.EmployeeId });
            }
            else if (FarmersController.LoggedInFarmer != null)
            {
                return RedirectToAction("Edit", "Farmers", new { id = FarmersController.LoggedInFarmer.FarmerId });
            }
            //Issue with logged in user, return to login
            ViewBag.LoginError = "Account error, you have been logged out.";
            return View("~/Views/Home/Index.cshtml");
        }

        //Register view
        public IActionResult Delete()
        {
            if (EmployeesController.LoggedInEmployee != null)
            {
                return RedirectToAction("Delete", "Employees", new { id = EmployeesController.LoggedInEmployee.EmployeeId });
            }
            else if (FarmersController.LoggedInFarmer != null)
            {
                return RedirectToAction("Delete", "Farmers", new { id = FarmersController.LoggedInFarmer.FarmerId });
            }
            //Issue with logged in user, return to login
            ViewBag.LoginError = "Account error, you have been logged out.";
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertEmployee(EmployeeViewModel employeeVM)
        {
            //Check for duplicate emails
            var employees = _context.Employees.ToList();
            foreach (var item in employees)
            {
                if (employeeVM.EmployeeEmail.Equals(item.EmployeeEmail))
                {
                    ViewBag.RegisterError = "Email is in use.";
                    return View("~/Views/Home/Register.cshtml");
                }
            }
            //Check for matching passwords
            if (employeeVM.EmployeePassword != employeeVM.EmployeePasswordConfirmation)
            {
                ViewBag.RegisterError = "Passwords don't match.";
                return View("~/Views/Home/Register.cshtml");
            }
            //Create employee from view model
            Employee employee = new()
            {
                EmployeeEmail = employeeVM.EmployeeEmail,
                EmployeeName = employeeVM.EmployeeName,
                EmployeePassword = FarmCentralDbContext.Hash(employeeVM.EmployeePassword)
            };
            //Insert employee into database
            try
            {
                _context.Employees.Add(employee);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                string exception = "" + e.InnerException;
                if (exception.Contains("duplicate"))
                {
                    ViewBag.RegisterError = "Employee already exists.";
                }
                else
                {
                    ViewBag.RegisterError = exception;
                }
                return View("~/Views/Home/Register.cshtml");
            }
            //Return to login page
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}