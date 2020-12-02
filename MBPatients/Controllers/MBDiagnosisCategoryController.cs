////This controller is used to perform CRUD operations on the DiagnosisCategory table in the database
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
    public class MBDiagnosisCategoryController : Controller
    {
        private readonly PatientsContext _context;


        //The constructor of this controller that initializes the _context variable to the context variable in the PatientsContext class
        public MBDiagnosisCategoryController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MBDiagnosisCategory
        //Index action fetches the rows in the DiagnosisCateogry table and returns them as a list in the corresponding Index view using the 
        //ToListAsync extension method.
        public async Task<IActionResult> Index()
        {
            return View(await _context.DiagnosisCategory.ToListAsync());
        }

        //This action fetches the details of the id selected by the user and uses the FirstOrDefault extension method and the lambda expression.
        // GET: MBDiagnosisCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosisCategory = await _context.DiagnosisCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diagnosisCategory == null)
            {
                return NotFound();
            }

            return View(diagnosisCategory);
        }
        //This action method returns the create view used for inserting a new record in the database.
        // GET: MBDiagnosisCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MBDiagnosisCategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //This action method validates the model first and then uses the add method to add all the values passed by the user to the diagnosisCategory
        // object and then uses the SaveChangesAsync method to save the changes to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] DiagnosisCategory diagnosisCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diagnosisCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diagnosisCategory);
        }

        // GET: MBDiagnosisCategory/Edit/5
        //This action is used to display the edit view with all the fields of the id selected by the user.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosisCategory = await _context.DiagnosisCategory.FindAsync(id);
            if (diagnosisCategory == null)
            {
                return NotFound();
            }
            return View(diagnosisCategory);
        }

        // POST: MBDiagnosisCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //This action firstly checks if the model passed is valid or not and then uses the update method to update the values.But, the values
        //are not safed untill saveChangesAsync method is called that would reflect the changes in the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] DiagnosisCategory diagnosisCategory)
        {
            if (id != diagnosisCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diagnosisCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiagnosisCategoryExists(diagnosisCategory.Id))
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
            return View(diagnosisCategory);
        }

        // GET: MBDiagnosisCategory/Delete/5
        //This method is used to display the delete view for the id selected by the user with all the fields.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnosisCategory = await _context.DiagnosisCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diagnosisCategory == null)
            {
                return NotFound();
            }

            return View(diagnosisCategory);
        }

        // POST: MBDiagnosisCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //this method finds the record to the id selected using the FindAsync method and then uses the Remove method to delete the diagnosisCategory object
        //returned for the particular id .Then the changes are saved using the SaveChangesAsync method.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diagnosisCategory = await _context.DiagnosisCategory.FindAsync(id);
            _context.DiagnosisCategory.Remove(diagnosisCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiagnosisCategoryExists(int id)
        {
            return _context.DiagnosisCategory.Any(e => e.Id == id);
        }
    }
}
