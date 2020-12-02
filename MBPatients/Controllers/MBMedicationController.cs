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
    public class MBMedicationController : Controller
    {
        private readonly PatientsContext _context;

        public MBMedicationController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MBMedication
        public async Task<IActionResult> Index(string  MedicationTypeId)
           {
            if (!string.IsNullOrEmpty(MedicationTypeId))
            {
                Response.Cookies.Append("MedicationTypeId", MedicationTypeId);
                HttpContext.Session.SetString("MedicationTypeId", MedicationTypeId);
            }
            else if (Request.Query["MedicationTypeId"].Any())
            {
                Response.Cookies.Append("MedicationTypeId", Request.Query["MedicationTypeId"].ToString());
                HttpContext.Session.SetString("MedicationTypeId", Request.Query["MedicationTypeId"].ToString());
                MedicationTypeId = Request.Query["MedicationTypeId"].ToString();
            }
            else if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }
            else if (HttpContext.Session.GetString("MedicationTypeId") != null)
            {
                MedicationTypeId = HttpContext.Session.GetString("MedicationTypeId");
            }
            else
            {
                TempData["message"] = "Please select any medication type";
                return RedirectToAction("Index", "MedicationType");
            }



            var MedicationType = _context.MedicationType.Where(m => m.MedicationTypeId == Int32.Parse(MedicationTypeId)).FirstOrDefault();
            ViewData["MedicationTypeId"] = MedicationTypeId;
            ViewData["MedicationTypeName"] = MedicationType.Name;



            var medicationContext = _context.Medication.Include(m => m.MedicationType).Include(m => m.ConcentrationCodeNavigation)
                                    .Include(m => m.DispensingCodeNavigation)
                .Where(m => m.MedicationTypeId == Int32.Parse(MedicationTypeId))
                .OrderBy(m => m.Name)
                .ThenBy(m => m.Concentration);
            return View(await medicationContext.ToListAsync());
        }

        // GET: MBMedication/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            ViewData["medicationName"] = medication.Name;
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        // GET: MBMedication/Create
        public IActionResult Create()
        {
            string MedicationTypeCode = string.Empty;
            if(Request.Cookies["MedicationTypeId"]!=null)
            {
                MedicationTypeCode = Request.Cookies["MedicationTypeId"].ToString();
            }
            else if(HttpContext.Session.GetString("MedicationTypeId")!=null)
            {
                MedicationTypeCode= HttpContext.Session.GetString("MedicationTypeId");
            }
            var MedicationTypeName = _context.MedicationType.Where(x => x.MedicationTypeId == Convert.ToInt32(MedicationTypeCode)).FirstOrDefault();
            ViewData["MedicationtypeCode"] = MedicationTypeCode;
            ViewData["MedicationTypeName"] = MedicationTypeName.Name;

            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(m=>m.ConcentrationCode), "ConcentrationCode", "ConcentrationCode");
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(m=>m.DispensingCode), "DispensingCode", "DispensingCode");
            //ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name");
            return View();
        }

        // POST: MBMedication/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            string MedicationTypeCode = string.Empty;
            if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeCode = Request.Cookies["MedicationTypeId"].ToString();
            }
            else if (HttpContext.Session.GetString("MedicationTypeId") != null)
            {
                MedicationTypeCode = HttpContext.Session.GetString("MedicationTypeId");
            }
            var isDuplicate = _context.Medication.Where(m => m.Name == medication.Name && m.Concentration == medication.Concentration
            && m.ConcentrationCode == medication.ConcentrationCode);
            if (isDuplicate.Any())
            {
                ModelState.AddModelError("", "There is already a record with the entered name in the system: " + medication.Name);
                ModelState.AddModelError("", "There is already a record with the entered concentration in the system: " + medication.Concentration);
                ModelState.AddModelError("", "There is already a record with the entered concentration code in the system  : " + medication.ConcentrationCode);

            }
            if (ModelState.IsValid)
            {
                medication.MedicationTypeId = Int32.Parse(MedicationTypeCode);
                _context.Add(medication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit, "DispensingCode", "DispensingCode", medication.DispensingCode);
            return View(medication);
        }

        // GET: MBMedication/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication.FindAsync(id);
            ViewData["medicationName"] = medication.Name;
            if (medication == null)
            {
                return NotFound();
            }
            ViewData["ConcentrationCode"] = medication.ConcentrationCode;
            //ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit, "DispensingCode", "DispensingCode", medication.DispensingCode);
            //ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }

        // POST: MBMedication/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            if (id != medication.Din)
            {
                return NotFound();
            }
            string MedicationTypeCode = string.Empty;
            if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeCode = Request.Cookies["MedicationTypeId"].ToString();
            }
            else if (HttpContext.Session.GetString("MedicationTypeId") != null)
            {
                MedicationTypeCode = HttpContext.Session.GetString("MedicationTypeId");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    medication.MedicationTypeId = Int32.Parse(MedicationTypeCode);
                    _context.Update(medication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.Din))
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
            ViewData["ConcentrationCode"] = medication.ConcentrationCode;
            //ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit, "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit, "DispensingCode", "DispensingCode", medication.DispensingCode);
            //ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }

        // GET: MBMedication/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            ViewData["medicationName"] = medication.Name;
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        // POST: MBMedication/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var medication = await _context.Medication.FindAsync(id);
            _context.Medication.Remove(medication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationExists(string id)
        {
            return _context.Medication.Any(e => e.Din == id);
        }
    }
}
