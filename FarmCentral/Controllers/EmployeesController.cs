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
using System.Security.Policy;

namespace FarmCentral.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly FarmCentralDbContext _context;
        //Currently logged in employee
        public static Employee? LoggedInEmployee;

        public EmployeesController(FarmCentralDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            //Reset selected farmer
            ProductsController.CurrentFarmer = null;
            //Only show page if logged in
            if (LoggedInEmployee == null) return RedirectToAction("Index", "Home");
            //Set employee name
            ViewBag.EmployeeName = LoggedInEmployee.EmployeeName;
            //Send list of farmers to view
            return _context.Farmers != null ?
                          View(await _context.Farmers.ToListAsync()) :
                          Problem("Entity set 'FarmCentralDbContext.Farmers'  is null.");
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || id == null || _context.Employees == null)
            {
                return NotFound();
            }
            EmployeeViewModel employeeModel = new() { EmployeeEmail = employee.EmployeeEmail, EmployeeName = employee.EmployeeName, EmployeePassword = employee.EmployeePassword, EmployeePasswordConfirmation = employee.EmployeePassword };
            return View(employeeModel);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EmployeeEmail,EmployeeName,EmployeePassword,EmployeePasswordConfirmation")] EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid || LoggedInEmployee == null) return View(employeeVM);
            if (employeeVM.EmployeePassword != null && !employeeVM.EmployeePassword.Equals(employeeVM.EmployeePasswordConfirmation))
            {
                ViewBag.EditError = "Passwords don't match.";
                return View(employeeVM);
            }
            else if (employeeVM.EmployeePassword != null && employeeVM.EmployeePassword.Length < 6)
            {
                ViewBag.EditError = "Passwords must be at least 6 characters.";
                return View(employeeVM);
            }
            try
            {
                //Linq queries adapted from
                //https://www.tutorialsteacher.com/linq/what-is-linq
                //accessed 29 May 2023
                //Get employee async
                var query = from s in _context.Employees
                            where s.EmployeeId == id
                            select s;
                var employee = await query.FirstOrDefaultAsync();

                //Update employee and _db
                if (employee != null)
                {
                    string hash = "";
                    if (employeeVM.EmployeePassword != null)
                    {
                        hash = FarmCentralDbContext.Hash(employeeVM.EmployeePassword);
                    }

                    if (employeeVM.EmployeeName.Equals(employee.EmployeeName) && string.IsNullOrWhiteSpace(employeeVM.EmployeePassword) || hash.Equals(employee.EmployeePassword))
                    {
                        ViewBag.EditError = "No data was changed.";
                        return View(employeeVM);
                    }

                    employee.EmployeeName = employeeVM.EmployeeName;
                    //Only update password if changed
                    if (!string.IsNullOrWhiteSpace(employeeVM.EmployeePassword))
                    {
                        employee.EmployeePassword = hash;
                    }
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    //Update employee in system
                    ViewBag.EmployeeName = employeeVM.EmployeeName;
                    LoggedInEmployee = employee;
                }
            }
            catch (Exception)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'FarmCentralDbContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            //Ensure logout and return to login page after employee account deletion
            return RedirectToAction("LogOut", "Home");
        }

        private bool EmployeeExists(Guid id)
        {
            return (_context.Employees?.Any(e => e.EmployeeId == id)).GetValueOrDefault();
        }
    }
}
