using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MBPatients.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;

namespace MBPatients.Controllers
{
    public class MBPatientTreatmentController : Controller
    {
        private readonly PatientsContext _context;

        public MBPatientTreatmentController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MBPatientTreatment
        public async Task<IActionResult> Index(int PatientDiagnosisId,string FirstName,string LastName,string diagnosisName)
        {
            if (!string.IsNullOrEmpty(PatientDiagnosisId.ToString()))
            {
                Response.Cookies.Append("PatientDiagnosisId", PatientDiagnosisId.ToString());
            }
            else if (Request.Query["PatientDiagnosisId"].Any())
            {
                Response.Cookies.Append("PatientDiagnosisId", Request.Query["PatientDiagnosisId"].ToString());

                PatientDiagnosisId = Convert.ToInt32(Request.Query["PatientDiagnosisId"]);
            }
            else if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Convert.ToInt32(Request.Cookies["PatientDiagnosisId"]);
            }
            else if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = (int)HttpContext.Session.GetInt32("PatientDiagnosisId");
            }
            else
            {
                TempData["message"] = "Please select a patient's Diagnosis";
                return RedirectToAction("Index", "MBPatientDiagnosis");
            }
           
            ViewData["LastName"] = LastName;
            ViewData["FirstName"] = FirstName;
            ViewData["diagnosisName"] = diagnosisName;
            var patientsContext = _context.PatientTreatment.Include(p => p.PatientDiagnosis).Include(t=>t.Treatment)
                .Where(p=>p.PatientDiagnosisId==PatientDiagnosisId)
                .OrderByDescending(a=>a.DatePrescribed);
            return View(await patientsContext.ToListAsync());
        }

        // GET: MBPatientTreatment/Details/5
        public async Task<IActionResult> Details(int? id,string diagnosisName)
        {
            string PatientDiagnosisId = string.Empty;
            if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Request.Cookies["PatientDiagnosisId"].ToString();
            }
            else if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }
            var x = _context.PatientDiagnosis.Where(r => r.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();
            var y = _context.Patient.Where(r => r.PatientId == x.PatientId).FirstOrDefault();
            ViewData["LastName"] = y.LastName;
            ViewData["FirstName"] = y.FirstName;
            ViewData["diagnosisName"] = diagnosisName;
            if (id == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatment
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            return View(patientTreatment);
        }

        // GET: MBPatientTreatment/Create
        public IActionResult Create(string diagnosisName)
        {
            string PatientDiagnosisId = string.Empty;
            if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Request.Cookies["PatientDiagnosisId"].ToString();
            }
            else if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }
            var x=_context.PatientDiagnosis.Where(r => r.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();
            var y=_context.Patient.Where(r => r.PatientId == x.PatientId).FirstOrDefault();
            ViewData["LastName"] = y.LastName;
            ViewData["FirstName"] = y.FirstName;
            ViewData["diagnosisName"] = diagnosisName;

            var mb = _context.PatientDiagnosis.Where(a => a.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();

            ViewData["TreatmentId"] = new SelectList(_context.Treatment.Where(x=>x.DiagnosisId== mb.DiagnosisId), "TreatmentId", "Name");
            return View();
        }

        // POST: MBPatientTreatment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            var PatientDiagnosisId = Request.Cookies["PatientDiagnosisId"];
            patientTreatment.PatientDiagnosisId = Convert.ToInt32(PatientDiagnosisId);
            if (ModelState.IsValid)
            {
                _context.Add(patientTreatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "Name", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // GET: MBPatientTreatment/Edit/5
        public async Task<IActionResult> Edit(int? id,string diagnosisName)
        {
            string PatientDiagnosisId = string.Empty;
            if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Request.Cookies["PatientDiagnosisId"].ToString();
            }
            else if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }
            var x = _context.PatientDiagnosis.Where(r => r.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();
            var y = _context.Patient.Where(r => r.PatientId == x.PatientId).FirstOrDefault();
            ViewData["LastName"] = y.LastName;
            ViewData["FirstName"] = y.FirstName;
            ViewData["diagnosisName"] = diagnosisName;
            if (id == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatment.FindAsync(id);
    
            ViewBag.DateInFormat = patientTreatment.DatePrescribed.ToString("dd MMMM yyyy hh:mm");
            if (patientTreatment == null)
            {
                return NotFound();
            }
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "Name", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // POST: MBPatientTreatment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            
            if (id != patientTreatment.PatientTreatmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientTreatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientTreatmentExists(patientTreatment.PatientTreatmentId))
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
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "Name", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // GET: MBPatientTreatment/Delete/5
        public async Task<IActionResult> Delete(int? id,string diagnosisName)
        {
            string PatientDiagnosisId = string.Empty;
            if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Request.Cookies["PatientDiagnosisId"].ToString();
            }
            else if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }
            var x = _context.PatientDiagnosis.Where(r => r.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();
            var y = _context.Patient.Where(r => r.PatientId == x.PatientId).FirstOrDefault();
            ViewData["LastName"] = y.LastName;
            ViewData["FirstName"] = y.FirstName;
            ViewData["diagnosisName"] = diagnosisName;

            if (id == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatment
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            return View(patientTreatment);
        }

        // POST: MBPatientTreatment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientTreatment = await _context.PatientTreatment.FindAsync(id);
            _context.PatientTreatment.Remove(patientTreatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientTreatmentExists(int id)
        {
            return _context.PatientTreatment.Any(e => e.PatientTreatmentId == id);
        }
    }
}
