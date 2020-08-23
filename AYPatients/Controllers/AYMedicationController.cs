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
    public class AYMedicationController : Controller
    {
        private readonly PatientsContext _context;

        public AYMedicationController(PatientsContext context)
        {
            _context = context;
        }

        public static string medTypeId;
        public static string medTypeName;
         

        // GET: AYMedications
        public async Task<IActionResult> Index(string medicationTypeId)
        {
            
            if (!string.IsNullOrEmpty(medicationTypeId))
            {
               Response.Cookies.Append("medicationTypeId", medicationTypeId);
               HttpContext.Session.SetString("medicationTypeId", medicationTypeId);
            }

            else if (Request.Query["MedicationTypeId"].Any())
            {
                Response.Cookies.Append("medicationTypeId", Request.Query["MedicationTypeId"].ToString());
                HttpContext.Session.SetString("medicationTypeId", Request.Query["MedicationTypeId"].ToString());
                medicationTypeId = Request.Query["MedicationTypeId"].ToString();
            }

            else if (Request.Cookies["MedicationTypeId"] != null)
            {
                medicationTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }

            else if (HttpContext.Session.GetString("MedicationTypeId") != null)
            {
                medicationTypeId = HttpContext.Session.GetString("MedicationTypeId");
            }             

            else
            {
                TempData["message"] = "*Please select medication type";

                return RedirectToAction("index", "AYMedicationType");
            }

            

            var mediType = _context.MedicationType
                           .Where(m => m.MedicationTypeId.ToString() == medicationTypeId)
                           .FirstOrDefault();

            ViewData["MedTypeCode"] = medicationTypeId;  //not sure where to use
            ViewData["MedTypeName"] = mediType.Name;

            medTypeName= mediType.Name;
            medTypeId = medicationTypeId; //assign medicationTypeId to medTypeName

            var patientsContext = _context.Medication.Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .OrderBy(m => m.Name)
                .ThenBy(m => m.Concentration)
                .Where(m => m.MedicationTypeId.ToString() == medicationTypeId);
             
            return View(await patientsContext.ToListAsync());
        }
              
        // GET: AYMedications/Details/5
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
            if (medication == null)
            {
                return NotFound();
            }

            ViewData["MedTypeName"] = medTypeName;
            return View(medication);
        }

        // GET: AYMedications/Create
        public IActionResult Create()
        {
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(m => m.ConcentrationCode), "ConcentrationCode", "ConcentrationCode");
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderByDescending(m=>m.DispensingCode), "DispensingCode", "DispensingCode");
            //ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name");
            ViewData["MedTypeName"] = medTypeName;
            return View();
        }

        // POST: AYMedications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            
            
            if (ModelState.IsValid)
            {
                var existingMedication = _context.Medication
                                        .Where(m => m.Name == medication.Name
                                        && m.ConcentrationCode == medication.ConcentrationCode
                                        && m.Concentration == medication.Concentration).FirstOrDefault(); //firstordefualt means to return such an object
                if (existingMedication != null)
                {
                    TempData["message"] = $"medication {medication.Name} with  {medication.Concentration} {medication.ConcentrationCode} already exists";
                }

                else
                {
                    medication.MedicationTypeId = Convert.ToInt32(medTypeId);
                    _context.Add(medication);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                
            }
            ViewData["MedTypeName"] = medTypeName;
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(m => m.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(m=>m.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
           // ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }

        // GET: AYMedications/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication.FindAsync(id);
            if (medication == null)
            {
                return NotFound();
            }

            ViewData["MedTypeName"] = medTypeName;
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(m=>m.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(m=>m.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
           // ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }

        // POST: AYMedications/Edit/5
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

            if (ModelState.IsValid)
            {
                medication.MedicationTypeId = Convert.ToInt32(Request.Cookies["medicationTypeId"]);

                medication.ConcentrationCode = _context.Medication
                    .Where(m => m.MedicationTypeId.ToString() == medTypeId)
                    .FirstOrDefault().ConcentrationCode;

                try
                {                    
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

            ViewData["MedTypeName"] = medTypeName;
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(m=>m.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(m => m.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
           // ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }

        // GET: AYMedications/Delete/5
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
            if (medication == null)
            {
                return NotFound();
            }

            ViewData["MedTypeName"] = medTypeName;                
            return View(medication);

        }

        // POST: AYMedications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var medication = await _context.Medication.FindAsync(id);
            _context.Medication.Remove(medication);
            await _context.SaveChangesAsync();
            ViewData["MedTypeName"] = medTypeName;
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationExists(string id)
        {
            return _context.Medication.Any(e => e.Din == id);
        }

        

        
    }
}
