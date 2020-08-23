/*
 *the controller for Dispensing Unit 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AYPatients.Models;

namespace AYPatients.Controllers
{
    public class AYDispensingUnitController : Controller
    {
        private readonly PatientsContext _context;

        public AYDispensingUnitController(PatientsContext context)
        {
            _context = context;
        }

        // GET: AYDispensingUnit
        public async Task<IActionResult> Index() // the action to get to index
        {
            return View(await _context.DispensingUnit.ToListAsync());
        }

        // GET: AYDispensingUnit/Details/5
        public async Task<IActionResult> Details(string id) // the action to show the details pm key is dispensingCode
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

        // GET: AYDispensingUnit/Create
        public IActionResult Create() //the action to create a new Dispensing Unit in the database
        {
            return View();
        }

        // POST: AYDispensingUnit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: AYDispensingUnit/Edit/5
        public async Task<IActionResult> Edit(string id) // the action to edit a current dispensing unit
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

        // POST: AYDispensingUnit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: AYDispensingUnit/Delete/5
        public async Task<IActionResult> Delete(string id) //action to delete the DispensingUnit in the database
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

        // POST: AYDispensingUnit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
