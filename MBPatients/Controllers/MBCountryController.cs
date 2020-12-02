////This controller is used to perform CRUD operations on the Country table in the database
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MBPatients.Models;
using Microsoft.AspNetCore.Authorization;

namespace MBPatients.Controllers
{
    [Authorize(Roles ="members")]
    public class MBCountryController : Controller
    {
        private readonly PatientsContext _context;


        //The constructor of this controller that initializes the _context variable to the context variable in the PatientsContext class
        public MBCountryController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MBCountry
        //Index action fetches the rows in the Country table and returns them as a list in the corresponding Index view using the 
        //ToListAsync extension method.

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Country.ToListAsync());
        }

        // GET: MBCountry/Details/5
        //This action fetches the details of the id selected by the user and uses the FirstOrDefault extension method and the lambda expression.
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country
                .FirstOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: MBCountry/Create
        //This action method returns the create view used for inserting a new record in the database.
        [Authorize(Roles = "administrators")]
        [Authorize(Roles = "medicalstaff")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: MBCountry/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //This action method validates the model first and then uses the add method to add all the values passed by the user to the country
        // object and then uses the SaveChangesAsync method to save the changes to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CountryCode,Name,PostalPattern,PhonePattern,FederalSalesTax")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: MBCountry/Edit/5
        //This action is used to display the edit view with all the fields of the id selected by the user.
        [Authorize(Roles = "administrators")]
        [Authorize(Roles = "medicalstaff")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: MBCountry/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //This action firstly checks if the model passed is valid or not and then uses the update method to update the values.But, the values
        //are not safed untill saveChangesAsync method is called that would reflect the changes in the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CountryCode,Name,PostalPattern,PhonePattern,FederalSalesTax")] Country country)
        {
            if (id != country.CountryCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.CountryCode))
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
            return View(country);
        }

        // GET: MBCountry/Delete/5
        //This method is used to display the delete view for the id selected by the user with all the fields.
        [Authorize(Roles = "administrators")]
        [Authorize(Roles = "medicalstaff")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country
                .FirstOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: MBCountry/Delete/5
        //this method finds the record to the id selected using the FindAsync method and then uses the Remove method to delete the country object
        //returned for the particular id .Then the changes are saved using the SaveChangesAsync method.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var country = await _context.Country.FindAsync(id);
            _context.Country.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(string id)
        {
            return _context.Country.Any(e => e.CountryCode == id);
        }
    }
}
