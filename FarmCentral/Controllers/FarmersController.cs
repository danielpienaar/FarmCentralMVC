using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FarmCentral.Data;
using FarmCentral.Models.ModelsDB;
using FarmCentral.Models;

namespace FarmCentral.Controllers
{
    public class FarmersController : Controller
    {
        private readonly FarmCentralDbContext _context;
        //Currently logged in dupFarmer
        public static Farmer? LoggedInFarmer;

        public FarmersController(FarmCentralDbContext context)
        {
            _context = context;
        }

        // GET: Farmers
        public async Task<IActionResult> Index()
        {
            //Only show page if logged in
            if (LoggedInFarmer == null) return RedirectToAction("Index", "Home");

            if (LoggedInFarmer != null)
            {
                ViewBag.FarmerName = LoggedInFarmer.FarmerName;
                //Get and display products for logged in farmer
                var query = from p in _context.Products
                            where p.Farmer.FarmerId == LoggedInFarmer.FarmerId
                            select p;
                var products = await query.ToListAsync();
                return View(products);
            }
            return _context.Products != null ?
                        View(await _context.Products.ToListAsync()) :
                        Problem("Entity set 'FarmCentralDbContext.Products'  is null.");
        }

        // GET: Farmers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Farmers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FarmerEmail,FarmerName,FarmerPassword,FarmerPasswordConfirmation")] FarmerViewModel farmerVM)
        {
            //Check for duplicate emails
            var dupFarmer = await _context.Farmers.FirstOrDefaultAsync(m => m.FarmerEmail.Equals(farmerVM.FarmerEmail));
            if (dupFarmer != null)
            {
                ViewBag.FarmerError = "Email is in use.";
                return View(farmerVM);
            }
            //Check for matching passwords
            if (farmerVM.FarmerPassword != farmerVM.FarmerPasswordConfirmation)
            {
                ViewBag.FarmerError = "Passwords don't match.";
                return View(farmerVM);
            }
            else if (farmerVM.FarmerPassword.Length < 6)
            {
                ViewBag.FarmerError = "Passwords must be at least 6 characters.";
                return View(farmerVM);
            }
            //Create farmer from view model
            Farmer farmer = new()
            {
                FarmerEmail = farmerVM.FarmerEmail,
                FarmerName = farmerVM.FarmerName,
                FarmerPassword = FarmCentralDbContext.Hash(farmerVM.FarmerPassword)
            };
            //Insert farmer into database
            try
            {
                _context.Farmers.Add(farmer);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                string exception = "" + e.InnerException;
                if (exception.Contains("duplicate"))
                {
                    ViewBag.FarmerError = "Farmer already exists.";
                }
                else
                {
                    ViewBag.FarmerError = exception;
                }
                return View(farmerVM);
            }
            //Return to employee home page
            return RedirectToAction("Index", "Employees");
        }

        // GET: Farmers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Farmers == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
            {
                return NotFound();
            }
            if (LoggedInFarmer != null)
            {
                ViewBag.EditMessage = "Edit Account";
            }
            else
            {
                ViewBag.EditMessage = "Edit Farmer";
            }
            FarmerViewModel farmerModel = new() { FarmerEmail = farmer.FarmerEmail, FarmerName = farmer.FarmerName };
            return View(farmerModel);
        }

        // POST: Farmers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("FarmerEmail,FarmerName,FarmerPassword,FarmerPasswordConfirmation")] FarmerViewModel farmerVM)
        {
            if (LoggedInFarmer != null)
            {
                ViewBag.EditMessage = "Edit Account";
            }
            else
            {
                ViewBag.EditMessage = "Edit Farmer";
            }
            if (!ModelState.IsValid) return View(farmerVM);
            if (farmerVM.FarmerPassword != null && !farmerVM.FarmerPassword.Equals(farmerVM.FarmerPasswordConfirmation))
            {
                ViewBag.EditError = "Passwords don't match.";
                return View(farmerVM);
            }
            else if (farmerVM.FarmerPassword != null && farmerVM.FarmerPassword.Length < 6)
            {
                ViewBag.EditError = "Passwords must be at least 6 characters.";
                return View(farmerVM);
            }
            try
            {
                //Linq queries adapted from
                //https://www.tutorialsteacher.com/linq/what-is-linq
                //accessed 29 May 2023
                //Get farmer async
                var query = from f in _context.Farmers
                            where f.FarmerId == id
                            select f;
                var farmer = await query.FirstOrDefaultAsync();

                //Update employee and _db
                if (farmer != null)
                {
                    string hash = "";
                    if (farmerVM.FarmerPassword != null)
                    {
                        hash = FarmCentralDbContext.Hash(farmerVM.FarmerPassword);
                    }

                    if (farmerVM.FarmerName.Equals(farmer.FarmerName) && string.IsNullOrWhiteSpace(farmerVM.FarmerPassword) || hash.Equals(farmer.FarmerPassword))
                    {
                        ViewBag.EditError = "No data was changed.";
                        return View(farmerVM);
                    }

                    farmer.FarmerName = farmerVM.FarmerName;
                    //Only update password if changed
                    if (!string.IsNullOrWhiteSpace(farmerVM.FarmerPassword))
                    {
                        farmer.FarmerPassword = hash;
                    }
                    _context.Update(farmer);
                    await _context.SaveChangesAsync();
                    //Update farmer in system if logged in
                    if (LoggedInFarmer != null)
                    {
                        ViewBag.FarmerName = farmerVM.FarmerName;
                        LoggedInFarmer = farmer;
                    }
                }
            }
            catch (Exception)
            {
                if (!FarmerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (LoggedInFarmer != null)
            {
                //Return to farmer home page if logged in as farmer
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //Return to employee home page if employee is logged in
                return RedirectToAction("Index", "Employees");
            }

        }

        // GET: Farmers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Farmers == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(m => m.FarmerId == id);
            if (farmer == null)
            {
                return NotFound();
            }
            if (LoggedInFarmer != null)
            {
                ViewBag.DeleteMessage = "Are you sure you want to delete your account?";
            }
            else
            {
                ViewBag.DeleteMessage = "Are you sure you want to delete this farmer?";
            }
            return View(farmer);
        }

        // POST: Farmers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Farmers == null)
            {
                return Problem("Entity set 'FarmCentralDbContext.Farmers'  is null.");
            }
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer != null)
            {
                _context.Farmers.Remove(farmer);
            }

            await _context.SaveChangesAsync();
            if (LoggedInFarmer != null)
            {
                //Ensure logout and return to login page after farmer account deletion
                return RedirectToAction("LogOut", "Home");
            }
            else
            {
                //Return to employee home page if employee is logged in
                return RedirectToAction("Index", "Employees");
            }
        }

        //Select a farmer and view the product list for the farmer id
        public IActionResult Select(Guid? id)
        {
            ProductsController.CurrentFarmer = _context.Farmers.Find(id);
            return RedirectToAction("Index", "Products");
        }

        private bool FarmerExists(Guid id)
        {
            return (_context.Farmers?.Any(e => e.FarmerId == id)).GetValueOrDefault();
        }
    }
}
