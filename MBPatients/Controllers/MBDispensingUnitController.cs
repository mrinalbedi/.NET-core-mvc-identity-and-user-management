////This controller is used to perform CRUD operations on the dispensingUnit table in the database
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MBPatients.Models;

namespace MBPatients.Controllers
{
    public class MBDispensingUnitController : Controller
    {
        private readonly PatientsContext _context;

        //The constructor of this controller that initializes the _context variable to the context variable in the PatientsContext class
        public MBDispensingUnitController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MBDispensingUnit
        //Index action fetches the rows in the DispensingUnit table and returns them as a list in the corresponding Index view using the 
        //ToListAsync extension method.
        public async Task<IActionResult> Index()
        {
            return View(await _context.DispensingUnit.ToListAsync());
        }

        // GET: MBDispensingUnit/Details/5
        //This action fetches the details of the id selected by the user and uses the FirstOrDefault extension method and the lambda expression.
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispensingUnit = await _context.DispensingUnit
                .FirstOrDefaultAsync(m => m.DispensingCode == id);
            if (dispensingUnit == null)
            {
                return NotFound();
            }

            return View(dispensingUnit);
        }

        // GET: MBDispensingUnit/Create
        //This action method returns the create view used for inserting a new record in the database.
        public IActionResult Create()
        {
            return View();
        }

        // POST: MBDispensingUnit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //This action method validates the model first and then uses the add method to add all the values passed by the user to the dispensingUnit
        // object and then uses the SaveChangesAsync method to save the changes to the database.
        public async Task<IActionResult> Create([Bind("DispensingCode")] DispensingUnit dispensingUnit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dispensingUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dispensingUnit);
        }

        // GET: MBDispensingUnit/Edit/5
        //This action is used to display the edit view with all the fields of the id selected by the user.
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispensingUnit = await _context.DispensingUnit.FindAsync(id);
            if (dispensingUnit == null)
            {
                return NotFound();
            }
            return View(dispensingUnit);
        }

        // POST: MBDispensingUnit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //This action firstly checks if the model passed is valid or not and then uses the update method to update the values.But, the values
        //are not safed untill saveChangesAsync method is called that would reflect the changes in the database.
        public async Task<IActionResult> Edit(string id, [Bind("DispensingCode")] DispensingUnit dispensingUnit)
        {
            if (id != dispensingUnit.DispensingCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dispensingUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DispensingUnitExists(dispensingUnit.DispensingCode))
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
            return View(dispensingUnit);
        }

        // GET: MBDispensingUnit/Delete/5
        //This method is used to display the delete view for the id selected by the user with all the fields.
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispensingUnit = await _context.DispensingUnit
                .FirstOrDefaultAsync(m => m.DispensingCode == id);
            if (dispensingUnit == null)
            {
                return NotFound();
            }

            return View(dispensingUnit);
        }

        // POST: MBDispensingUnit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //this method finds the record to the id selected using the FindAsync method and then uses the Remove method to delete the dispensingUnit object
        //returned for the particular id .Then the changes are saved using the SaveChangesAsync method.
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var dispensingUnit = await _context.DispensingUnit.FindAsync(id);
            _context.DispensingUnit.Remove(dispensingUnit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DispensingUnitExists(string id)
        {
            return _context.DispensingUnit.Any(e => e.DispensingCode == id);
        }
    }
}
