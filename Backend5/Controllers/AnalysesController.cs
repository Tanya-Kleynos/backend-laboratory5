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
    public class AnalysesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalysesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Analyses
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


            var items = await this._context.Analyses
                .Include(h => h.Lab)
                .Include(h => h.Patient)
                .Where(x => x.PatientId == patient.Id)
                .ToListAsync();
            this.ViewBag.Patient = patient;
            return this.View(items);
        }

        // GET: Analyses/Details/5
        public async Task<IActionResult> Details(Int32? patientId, Int32? analysisId)
        {
            if (patientId == null || analysisId == null)
            {
                return NotFound();
            }

            var analysis = await _context.Analyses
                .Include(a => a.Lab)
                .Include(a => a.Patient)
                .SingleOrDefaultAsync(m => m.AnalysisId == analysisId && m.PatientId == patientId);
            if (analysis == null)
            {
                return NotFound();
            }

            return View(analysis);
        }

        // GET: Analyses/Create
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
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Name");
            return View(new AnalysisCreateModel());
        }

        // POST: Analyses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Int32? patientId, AnalysisCreateModel model)
        {
            if (patientId == null)
            {
                return this.NotFound();
            }

            var patient = await this._context.Patients
                .Include(a => a.Analyses)
                .SingleOrDefaultAsync(x => x.Id == patientId);

            if (patient == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                var analysisId = patient.Analyses.Any() ? patient.Analyses.Max(a => a.AnalysisId) + 1 : 1;
                var analysis = new Analysis
                {
                    PatientId = patient.Id,
                    AnalysisId = analysisId,
                    LabId = model.LabId,
                    Type = model.Type,
                    Date = model.Date,
                    Status = model.Status
                };

                _context.Add(analysis);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { patientId = patient.Id });
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Name", model.LabId);
            ViewBag.Patient = patient;
            return View(model);
        }

        // GET: Analyses/Edit/5
        public async Task<IActionResult> Edit(Int32? patientId, Int32? analysisId)
        {
            if (patientId == null || analysisId == null)
            {
                return NotFound();
            }

            var analysis = await _context.Analyses.SingleOrDefaultAsync(m => m.AnalysisId == analysisId && m.PatientId == patientId);
            if (analysis == null)
            {
                return NotFound();
            }

            var model = new AnalysisEditModel
            {
                Type = analysis.Type,
                Date = analysis.Date,
                Status = analysis.Status
            };

            ViewBag.PatientId = analysis.PatientId;
            ViewBag.AnalysisId = analysis.AnalysisId;
            return View(model);
        }

        // POST: Analyses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? patientId, Int32? analysisId, AnalysisEditModel model)
        {
            if (patientId == null || analysisId == null)
            {
                return NotFound();
            }

            var analysis = await _context.Analyses.SingleOrDefaultAsync(m => m.AnalysisId == analysisId && m.PatientId == patientId);
            if (analysis == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                analysis.Type = model.Type;
                analysis.Date = model.Date;
                analysis.Status = model.Status;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { patientId = analysis.PatientId });
            }
            return View(model);
        }

        // GET: Analyses/Delete/5
        public async Task<IActionResult> Delete(Int32? patientId, Int32? analysisId)
        {
            if (patientId == null || analysisId == null)
            {
                return NotFound();
            }

            var analysis = await _context.Analyses
                .Include(a => a.Lab)
                .Include(a => a.Patient)
                .SingleOrDefaultAsync(m => m.AnalysisId == analysisId && m.PatientId == patientId);
            if (analysis == null)
            {
                return NotFound();
            }

            return View(analysis);
        }

        // POST: Analyses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int32 patientId, Int32 analysisId)
        {
            var analysis = await _context.Analyses.Include(x => x.Patient).SingleOrDefaultAsync(m => m.AnalysisId == analysisId && m.PatientId == patientId);
            _context.Analyses.Remove(analysis);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { patientId = analysis.Patient.Id });
        }
    }
}
