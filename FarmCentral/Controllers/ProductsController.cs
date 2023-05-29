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
using System.Globalization;

namespace FarmCentral.Controllers
{
    public class ProductsController : Controller
    {
        private readonly FarmCentralDbContext _context;
        public static Farmer? CurrentFarmer;

        public ProductsController(FarmCentralDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string? category, DateTime? startDate, DateTime? endDate)
        {
            //Only show page if logged in
            if (EmployeesController.LoggedInEmployee == null) return RedirectToAction("Index", "Home");
            if (CurrentFarmer != null)
            {
                ViewBag.FarmerName = CurrentFarmer.FarmerName;
                //Get and display products for selected farmer
                var query = from p in _context.Products
                            where p.Farmer.FarmerId == CurrentFarmer.FarmerId
                            select p;
                var products = await query.ToListAsync();
                //Send distinct product type list to view
                var distinctProductTypes = products.Select(p => p.ProductType).Distinct().ToList();
                ViewBag.ProductTypes = distinctProductTypes;
                //Filter by category or date range
                if (category != null)
                {
                    var categoryProducts = products.Where(p => p.ProductType.Equals(category)).ToList();
                    return View(categoryProducts);
                }
                else if (startDate != null && endDate != null)
                {
                    var timeProducts = products.Where(p => p.ProductListDate >= startDate && p.ProductListDate <= endDate).ToList();
                    return View(timeProducts);
                }
                return View(products);
            }
            return _context.Products != null ?
                        View(await _context.Products.ToListAsync()) :
                        Problem("Entity set 'FarmCentralDbContext.Products'  is null.");
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductType,ProductDescription,ProductPrice,ProductListDate")] ProductViewModel productVM)
        {
            //Insert product into database
            try
            {
                //Get farmer async
                var farmer = await _context.Farmers.FindAsync(FarmersController.LoggedInFarmer.FarmerId);
                //Create product from view model
                Product product = new()
                {
                    ProductType = productVM.ProductType,
                    ProductDescription = productVM.ProductDescription,
                    ProductPrice = Convert.ToDecimal(productVM.ProductPrice),
                    ProductListDate = productVM.ProductListDate,
                    Farmer = farmer
                };
                _context.Products.Add(product);
                _context.SaveChanges();
            }
            catch (FormatException)
            {
                ViewBag.ProductError = "Invalid price. If including decimals, please use commas \",\".";
                return View(productVM);
            }
            catch (NullReferenceException)
            {
                ViewBag.ProductError = "Couldn't find current farmer.";
                return View(productVM);
            }
            catch (Exception e)
            {
                string exception = "" + e.InnerException;
                if (exception.Contains("duplicate"))
                {
                    ViewBag.ProductError = "Product already exists.";
                }
                else
                {
                    ViewBag.ProductError = exception;
                }
                return View(productVM);
            }
            //Return to farmer home page
            return RedirectToAction("Index", "Farmers");
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ProductViewModel productModel = new() { ProductType = product.ProductType, ProductDescription = product.ProductDescription, ProductPrice = ""+product.ProductPrice, ProductListDate = product.ProductListDate };
            return View(productModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProductType,ProductDescription,ProductPrice,ProductListDate")] ProductViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products.FindAsync(id);
                    if (product != null)
                    {
                        product.ProductType = productVM.ProductType;
                        product.ProductDescription = productVM.ProductDescription;
                        product.ProductPrice = Convert.ToDecimal(productVM.ProductPrice);
                        product.ProductListDate = productVM.ProductListDate;
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (FormatException)
                {
                    ViewBag.EditError = "Invalid price. If including decimals, please use commas \",\".";
                    return View(productVM);
                }
                return RedirectToAction("Index", "Farmers");
            }
            return View(productVM);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'FarmCentralDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
