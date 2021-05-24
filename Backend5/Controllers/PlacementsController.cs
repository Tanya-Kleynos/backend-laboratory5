using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend5.Data;
using Backend5.Models;
using Backend5.Models.ViewModels;

namespace Backend5.Controllers
{
    public class PlacementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlacementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Placements
        public async Task<IActionResult> Index(Int32? patientId)
        {
            if (patientId == null)
            {
                return this.NotFound();
            }

            var patient = await this._context.Patients
                .SingleOrDefaultAsync(x => x.Id == patientId);

            if (patient == null)
            {
                return this.NotFound();
            }

            var items = await this._context.Placements
                .Include(h => h.Ward)
                .Include(h => h.Patient)
                .Where(x => x.PatientId == patient.Id)
                .ToListAsync();
            this.ViewBag.Patient = patient;
            return this.View(items);
        }

        // GET: Placements/Details/5
        public async Task<IActionResult> Details(Int32? id)
        {   
            if (id == null)
            {
                return NotFound();
            }

            var placement = await _context.Placements
                .Include(p => p.Patient)
                .Include(p => p.Ward)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (placement == null)
            {
                return NotFound();
            }

            return View(placement);
        }

        // GET: Placements/Create
        public async Task<IActionResult> Create(Int32? patientId)
        {
            if (patientId == null)
            {
                return this.NotFound();
            }

            var patient = await this._context.Patients
                .SingleOrDefaultAsync(x => x.Id == patientId);

            if (patient == null)
            {
                return this.NotFound();
            }

            ViewBag.Patient = patient;
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name");
            return View(new PlacementCreateModel());
        }

        // POST: Placements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Int32? patientId, PlacementCreateModel model)
        {
            if (patientId == null)
            {
                return this.NotFound();
            }

            var patient = await this._context.Patients
                .SingleOrDefaultAsync(x => x.Id == patientId);

            if (patient == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                var placement = new Placement
                {
                    WardId = model.WardId,
                    PatientId = patient.Id,
                    Bed = model.Bed
                };

                _context.Add(placement);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { patientId = patient.Id });
            }
            ViewBag.Patient = patient;
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name", model.WardId);
            return View(model);
        }

        // GET: Placements/Edit/5
        public async Task<IActionResult> Edit(Int32? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placement = await _context.Placements.SingleOrDefaultAsync(m => m.Id == id);
            if (placement == null)
            {
                return NotFound();
            }

            var model = new PlacementEditModel
            {
                Bed = placement.Bed
            };

            ViewBag.PatientId = placement.PatientId;
            
            return View(model);
        }

        // POST: Placements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? id, PlacementEditModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placement = await _context.Placements.SingleOrDefaultAsync(m => m.Id == id);
            if (placement == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                placement.Bed = model.Bed;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { patientId = placement.PatientId });
            }
            return View(model);
        }

        // GET: Placements/Delete/5
        public async Task<IActionResult> Delete(Int32? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placement = await _context.Placements
                .Include(x => x.Patient)
                .Include(x => x.Ward)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (placement == null)
            {
                return NotFound();
            }

            return View(placement);
        }

        // POST: Placements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int32 id)
        {
            var placement = await _context.Placements.Include(x => x.Patient).SingleOrDefaultAsync(m => m.Id == id);
            _context.Placements.Remove(placement);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { patientId = placement.Patient.Id });
        }

    }
}
