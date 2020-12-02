////This controller is used to perform CRUD operations on the MedicationType table in the database
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MBPatients.Models;
using Microsoft.AspNetCore.Http;

namespace MBPatients.Controllers
{
    public class MBMedicationTypeController : Controller
    {
        private readonly PatientsContext _context;
        //The constructor of this controller that initializes the _context variable to the context variable in the PatientsContext class
        public MBMedicationTypeController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MBMedicationType
        //Index action fetches the rows in the MedicationType table and returns them as a list in the corresponding Index view using the 
        //ToListAsync extension method.
        public async Task<IActionResult> Index()
        {
            
            //Ordering by name
            return View(await _context.MedicationType.OrderBy(m=>m.Name).ToListAsync());
        }

        // GET: MBMedicationType/Details/5
        //This action fetches the details of the id selected by the user and uses the FirstOrDefault extension method and the lambda expression
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationType = await _context.MedicationType
                .FirstOrDefaultAsync(m => m.MedicationTypeId == id);
            if (medicationType == null)
            {
                return NotFound();
            }

            return View(medicationType);
        }

        // GET: MBMedicationType/Create
        //This action method returns the create view used for inserting a new record in the database.
        public IActionResult Create()
        {
            return View();
        }

        // POST: MBMedicationType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //This action method validates the model first and then uses the add method to add all the values passed by the user to the medicationType
        // object and then uses the SaveChangesAsync method to save the changes to the database.
        public async Task<IActionResult> Create([Bind("MedicationTypeId,Name")] MedicationType medicationType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicationType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicationType);
        }

        // GET: MBMedicationType/Edit/5
        //This action is used to display the edit view with all the fields of the id selected by the user.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationType = await _context.MedicationType.FindAsync(id);
            if (medicationType == null)
            {
                return NotFound();
            }
            return View(medicationType);
        }

        // POST: MBMedicationType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //This action firstly checks if the model passed is valid or not and then uses the update method to update the values.But, the values
        //are not safed untill saveChangesAsync method is called that would reflect the changes in the database.
        public async Task<IActionResult> Edit(int id, [Bind("MedicationTypeId,Name")] MedicationType medicationType)
        {
            if (id != medicationType.MedicationTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicationType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationTypeExists(medicationType.MedicationTypeId))
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
            return View(medicationType);
        }

        // GET: MBMedicationType/Delete/5
        //This method is used to display the delete view for the id selected by the user with all the fields.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationType = await _context.MedicationType
                .FirstOrDefaultAsync(m => m.MedicationTypeId == id);
            if (medicationType == null)
            {
                return NotFound();
            }

            return View(medicationType);
        }

        // POST: MBMedicationType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //this method finds the record to the id selected using the FindAsync method and then uses the Remove method to delete the medicationType object
        //returned for the particular id .Then the changes are saved using the SaveChangesAsync method.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicationType = await _context.MedicationType.FindAsync(id);
            _context.MedicationType.Remove(medicationType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationTypeExists(int id)
        {
            return _context.MedicationType.Any(e => e.MedicationTypeId == id);
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
