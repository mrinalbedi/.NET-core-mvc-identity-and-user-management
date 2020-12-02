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
    public class MBPatientDiagnosisController : Controller
    {
        private readonly PatientsContext _context;

        public MBPatientDiagnosisController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MBPatientDiagnosis
        public async Task<IActionResult> Index(int PatientId)
        {
            var patientsContext = _context.PatientDiagnosis.Include(p => p.Diagnosis).Include(p => p.Patient)
                .Include(p=>p.PatientTreatment).
                Where(x=>x.PatientId==PatientId).
                OrderBy(p=>p.Patient.LastName).
                ThenBy(p=>p.Patient.FirstName).
                ThenByDescending(p=>p.PatientDiagnosisId);
            return View(await patientsContext.ToListAsync());
        }

        // GET: MBPatientDiagnosis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnosis
                .Include(p => p.Diagnosis)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientDiagnosisId == id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }

            return View(patientDiagnosis);
        }

        // GET: MBPatientDiagnosis/Create
        public IActionResult Create()
        {
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name");
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName");
            return View();
        }

        // POST: MBPatientDiagnosis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientDiagnosisId,PatientId,DiagnosisId,Comments")] PatientDiagnosis patientDiagnosis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientDiagnosis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // GET: MBPatientDiagnosis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnosis.FindAsync(id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // POST: MBPatientDiagnosis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientDiagnosisId,PatientId,DiagnosisId,Comments")] PatientDiagnosis patientDiagnosis)
        {
            if (id != patientDiagnosis.PatientDiagnosisId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientDiagnosis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientDiagnosisExists(patientDiagnosis.PatientDiagnosisId))
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
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // GET: MBPatientDiagnosis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnosis
                .Include(p => p.Diagnosis)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientDiagnosisId == id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }

            return View(patientDiagnosis);
        }

        // POST: MBPatientDiagnosis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientDiagnosis = await _context.PatientDiagnosis.FindAsync(id);
            _context.PatientDiagnosis.Remove(patientDiagnosis);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientDiagnosisExists(int id)
        {
            return _context.PatientDiagnosis.Any(e => e.PatientDiagnosisId == id);
        }
    }
}
