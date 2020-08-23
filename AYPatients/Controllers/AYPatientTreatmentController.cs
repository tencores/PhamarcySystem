using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AYPatients.Models;
using Microsoft.AspNetCore.Http;

namespace AYPatients.Controllers
{
    public class AYPatientTreatmentController : Controller
    {
        private readonly PatientsContext _context;

        public AYPatientTreatmentController(PatientsContext context)
        {
            _context = context;
        }

     
        public static string sPDId { get; set; }
        public static string sPName { get; set; }
        public static string sPDName { get; set; }

        // GET: AYPatientTreatment
        public async Task<IActionResult> Index(string PatientDiagnosisId, string PatientName, string PatientDiagnosisName)
        {
            if (!string.IsNullOrEmpty(PatientDiagnosisId))
            {
                Response.Cookies.Append("PatientDiagnosisId", PatientDiagnosisId);
            }

            else if (Request.Query["PatientDiagnosisId"].Any())
            {
                Response.Cookies.Append("PatientDiagnosisId", Request.Query["PatientDiagnosisId"].ToString());
                PatientDiagnosisId = Request.Query["PatientDiagnosisId"].ToString();
            }

            else if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Request.Cookies["PatientDiagnosisId"].ToString();
            }

            else if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }

            else
            {
                TempData["message"] = "Please select a patient ";

                return RedirectToAction("index", "AYPatientDiagnosis");
            }

            sPName = PatientName;
            sPDId = PatientDiagnosisId;
            sPDName = PatientDiagnosisName;

            ViewData["patientName"] = sPName;
            ViewData["diagnosis"] = sPDName;

            

            var patientsContext = _context.PatientTreatment.Include(p => p.PatientDiagnosis).Include(p => p.Treatment)
                                          .Where(p=>p.PatientDiagnosisId==Convert.ToInt32(PatientDiagnosisId))
                                          .OrderByDescending(p=>p.DatePrescribed);
            return View(await patientsContext.ToListAsync());
        }

        // GET: AYPatientTreatment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["patientName"] = sPName;
            ViewData["diagnosis"] = sPDName;
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

            ViewData["patientName"] = sPName;
            ViewData["diagnosis"] = sPDName;


            return View(patientTreatment);
        }

        // GET: AYPatientTreatment/Create
        public IActionResult Create()
        {
            ViewData["patientName"] = sPName;
            ViewData["diagnosis"] = sPDName;
            //ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId");
            ViewData["TreatmentId"] = new SelectList(_context.Treatment.Where(p=>p.DiagnosisId== Convert.ToInt32(sPDId)), "TreatmentId", "Name");
            return View();
        }

        // POST: AYPatientTreatment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            patientTreatment.PatientDiagnosisId = Convert.ToInt32(sPDId);

            if (ModelState.IsValid)
            {
                _context.Add(patientTreatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
           
            ViewData["TreatmentId"] = new SelectList(_context.Treatment.Where(t=>t.DiagnosisId== Convert.ToInt32(sPDId)), "TreatmentId", "Name", patientTreatment.TreatmentId);
            

            return View(patientTreatment);
        }

        // GET: AYPatientTreatment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["patientName"] = sPName;
            ViewData["diagnosis"] = sPDName;
            if (id == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatment.FindAsync(id);
            if (patientTreatment == null)
            {
                return NotFound();
            }
            //ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "Name", patientTreatment.TreatmentId);
            ViewData["DatePrescribed"] = patientTreatment.DatePrescribed.ToString("dd MMMM yyyy HH:mm");

            return View(patientTreatment);
        }

        // POST: AYPatientTreatment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            ViewData["patientName"] = sPName;
            ViewData["diagnosis"] = sPDName;
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
           //ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "Name", patientTreatment.TreatmentId);
            //ViewData["DatePrescribed"] = patientTreatment.DatePrescribed.ToString("dd MMMM yyyy HH:mm");
            return View(patientTreatment);
        }

        // GET: AYPatientTreatment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["patientName"] = sPName;
            ViewData["diagnosis"] = sPDName;
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

        // POST: AYPatientTreatment/Delete/5
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
