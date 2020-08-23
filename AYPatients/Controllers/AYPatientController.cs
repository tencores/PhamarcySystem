using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AYPatients.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AYPatients.Controllers
{
    public class AYPatientController : Controller
    {
        private readonly PatientsContext _context;

        public AYPatientController(PatientsContext context)
        {
            _context = context;
        }

        // GET: AYPatient
        public async Task<IActionResult> Index()
        {
            var patientsContext = _context.Patient
                                   .Include(p => p.ProvinceCodeNavigation).OrderBy(a => a.LastName + a.FirstName);  //can use orderby in this format                                ;
            return View(await patientsContext.ToListAsync());
        }

        // GET: AYPatient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .Include(p => p.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: AYPatient/Create
        public IActionResult Create()
        {
            //ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: AYPatient/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,FirstName,LastName,Address,City,ProvinceCode,PostalCode,Ohip,DateOfBirth,Deceased,DateOfDeath,HomePhone,Gender")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                //patient.PatientId = 1022;
                try
                {
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Patient {patient.FirstName + patient.LastName}'s information created succesfully ";
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)//04 LinqRequest ppt p6
                {
                    ModelState.AddModelError("", $"Exception happens when creating a patient files:{ ex.GetBaseException().Message}");
                }
                
            }
            //ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // GET: AYPatient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(a=>a.Name), "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // POST: AYPatient/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,FirstName,LastName,Address,City,ProvinceCode,PostalCode,Ohip,DateOfBirth,Deceased,DateOfDeath,HomePhone,Gender")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"patient {patient.FirstName+patient.LastName}'s information updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException dce)
                {
                    if (!PatientExists(patient.PatientId))
                    {
                        ModelState.AddModelError("", $"Exception happens when updating the informtion : {dce.GetBaseException().Message}.");
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Exception happens when updating the information :{dce.GetBaseException().Message}.");
                    }
                }
                
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(a => a.Name), "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // GET: AYPatient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .Include(p => p.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: AYPatient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {

                var patient = await _context.Patient.FindAsync(id);
                _context.Patient.Remove(patient);
                await _context.SaveChangesAsync();
                TempData["message"] = $"patient {patient.FirstName+patient.LastName}'s information deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Exception happens when deleting {ex.GetBaseException().Message} ";
            }
            return RedirectToAction("Delete");
        }

        private bool PatientExists(int id)
        {
            return _context.Patient.Any(e => e.PatientId == id);
        }
    }
}
