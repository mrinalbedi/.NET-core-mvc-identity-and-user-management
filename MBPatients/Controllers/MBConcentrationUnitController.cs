//This controller is used to perform CRUD operations on the ConcentrationUnit table in the database
using MBPatients.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MBPatients.Controllers
{
    public class MBConcentrationUnitController : Controller
    {
        private readonly PatientsContext _context;

        //The constructor of this controller that initializes the _context variable to the context variable in the PatientsContext class
        public MBConcentrationUnitController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MBConcentrationUnit
        //Index action fetches the rows in the ConcentrationUnit table and returns them as a list in the corresponding Index view using the 
        //ToListAsync extension method.
        public async Task<IActionResult> Index()
        {
            return View(await _context.ConcentrationUnit.ToListAsync());
        }

        // GET: MBConcentrationUnit/Details/5
        //This action fetches the details of the id selected by the user and uses the FirstOrDefault extension method and the lambda expression.
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concentrationUnit = await _context.ConcentrationUnit
                .FirstOrDefaultAsync(m => m.ConcentrationCode == id);
            if (concentrationUnit == null)
            {
                return NotFound();
            }

            return View(concentrationUnit);
        }

        // GET: MBConcentrationUnit/Create
        //This action method returns the create view used for inserting a new record in the database.
        public IActionResult Create()
        {
            return View();
        }

        // POST: MBConcentrationUnit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        //This action method validates the model first and then uses the add method to add all the values passed by the user to the concentrationUnit
        // object and then uses the SaveChangesAsync method to save the changes to the database
        public async Task<IActionResult> Create([Bind("ConcentrationCode")] ConcentrationUnit concentrationUnit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(concentrationUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(concentrationUnit);
        }

        // GET: MBConcentrationUnit/Edit/5

        //This action is used to display the edit view with all the fields of the id selected by the user.
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concentrationUnit = await _context.ConcentrationUnit.FindAsync(id);
            if (concentrationUnit == null)
            {
                return NotFound();
            }
            return View(concentrationUnit);
        }

        // POST: MBConcentrationUnit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        //This action firstly checks if the model passed is valid or not and then uses the update method to update the values.But, the values
        //are not safed untill saveChangesAsync method is called that would reflect the changes in the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ConcentrationCode")] ConcentrationUnit concentrationUnit)
        {
            if (id != concentrationUnit.ConcentrationCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(concentrationUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConcentrationUnitExists(concentrationUnit.ConcentrationCode))
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
            return View(concentrationUnit);
        }

        // GET: MBConcentrationUnit/Delete/5
        //This method is used to display the delete view for the id selected by the user with all the fields.
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concentrationUnit = await _context.ConcentrationUnit
                .FirstOrDefaultAsync(m => m.ConcentrationCode == id);
            if (concentrationUnit == null)
            {
                return NotFound();
            }

            return View(concentrationUnit);
        }

        // POST: MBConcentrationUnit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        //this method finds the record to the id selected using the FindAsync method and then uses the Remove method to delete the concentrationUnit object
        //returned for the particular id .Then the changes are saved using the SaveChangesAsync method.
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var concentrationUnit = await _context.ConcentrationUnit.FindAsync(id);
            _context.ConcentrationUnit.Remove(concentrationUnit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConcentrationUnitExists(string id)
        {
            return _context.ConcentrationUnit.Any(e => e.ConcentrationCode == id);
        }
    }
}
