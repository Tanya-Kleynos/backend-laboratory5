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
    public class DiagnosesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiagnosesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Diagnoses
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

            ViewBag.Patient = patient;
            var diagnoses = await _context.Diagnoses
                .Include(w => w.Patient)
                .Where(x => x.PatientId == patientId)
                .ToListAsync();

            return this.View(diagnoses);
        }

        // GET: Diagnoses/Details/5
        public async Task<IActionResult> Details(Int32? patientId, Int32? diagnosisId)
        {
            if (patientId == null || diagnosisId == null)
            {
                return NotFound();
            }

            var diagnosis = await _context.Diagnoses
                .Include(d => d.Patient)
                .SingleOrDefaultAsync(m => m.PatientId == patientId && m.DiagnosisId == diagnosisId);
            if (diagnosis == null)
            {
                return NotFound();
            }

            return View(diagnosis);
        }

        // GET: Diagnoses/Create
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

            this.ViewBag.Patient = patient;
            return this.View(new DiagnosisCreateModel());
        }

        // POST: Diagnoses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Int32? patientId, DiagnosisCreateModel model)
        {
            if (patientId == null)
            {
                return this.NotFound();
            }

            var patient = await this._context.Patients
                .Include(d => d.Diagnoses)
                .SingleOrDefaultAsync(x => x.Id == patientId);

            if (patient == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                var diagnosisId = patient.Diagnoses.Any() ? patient.Diagnoses.Max(d => d.DiagnosisId) + 1 : 1;
                var diagnosis = new Diagnosis
                {
                    DiagnosisId = diagnosisId,
                    PatientId = patient.Id,
                    Type = model.Type,
                    Complications = model.Complications,
                    Details = model.Details
                };

                _context.Add(diagnosis);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { patientId = patient.Id });
            }
            this.ViewBag.Patient = patient;
            return View(model);
        }

        // GET: Diagnoses/Edit/5
        public async Task<IActionResult> Edit(Int32? patientId, Int32? diagnosisId)
        {
            if (patientId == null || diagnosisId == null)
            {
                return NotFound();
            }

            var diagnosis = await _context.Diagnoses.SingleOrDefaultAsync(m => m.PatientId == patientId && m.DiagnosisId == diagnosisId);
            if (diagnosis == null)
            {
                return NotFound();
            }

            var model = new DiagnosisEditModel
            {
                Type = diagnosis.Type,
                Complications = diagnosis.Complications,
                Details = diagnosis.Details
            };

            this.ViewBag.PatientId = diagnosis.PatientId;
            this.ViewBag.DiagnosisId = diagnosis.DiagnosisId;
            return View(model);
        }

        // POST: Diagnoses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? patientId, Int32? diagnosisId, DiagnosisEditModel model)
        {
            if (patientId == null || diagnosisId == null)
            {
                return NotFound();
            }

            var diagnosis = await _context.Diagnoses.SingleOrDefaultAsync(m => m.PatientId == patientId && m.DiagnosisId == diagnosisId);

            if (diagnosis == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                diagnosis.Type = model.Type;
                diagnosis.Complications = model.Complications;
                diagnosis.Details = model.Details;

                await this._context.SaveChangesAsync();
                return this.RedirectToAction("Index", new { patientId = diagnosis.PatientId });
            }
            return View(model);
        }

        // GET: Diagnoses/Delete/5
        public async Task<IActionResult> Delete(Int32? patientId, Int32? diagnosisId)
        {
            if (patientId == null || diagnosisId == null)
            {
                return NotFound();
            }

            var diagnosis = await _context.Diagnoses
                .Include(d => d.Patient)
                .SingleOrDefaultAsync(m => m.PatientId == patientId && m.DiagnosisId == diagnosisId);
            if (diagnosis == null)
            {
                return NotFound();
            }

            return View(diagnosis);
        }

        // POST: Diagnoses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int32 patientId, Int32 diagnosisId)
        {
            var diagnosis = await _context.Diagnoses.SingleOrDefaultAsync(m => m.PatientId == patientId && m.DiagnosisId == diagnosisId);
            _context.Diagnoses.Remove(diagnosis);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { patientId = diagnosis.PatientId });
        }
    }
}
