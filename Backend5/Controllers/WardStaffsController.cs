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
    public class WardStaffsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WardStaffsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WardStaffs
        public async Task<IActionResult> Index(Int32? wardId)
        {
            if (wardId == null)
            {
                return this.NotFound();
            }

            var ward = await this._context.Wards
                .SingleOrDefaultAsync(x => x.Id == wardId);

            if (ward == null)
            {
                return this.NotFound();
            }

            this.ViewBag.Ward = ward;
            var wardStaffs = await this._context.WardStaffs
                .Include(w => w.Ward)
                .Where(x => x.WardId == wardId)
                .ToListAsync();

            return this.View(wardStaffs);
        }

        // GET: WardStaffs/Details/5
        public async Task<IActionResult> Details(Int32? wardId, Int32? wardStaffId)
        {
            if (wardId == null || wardStaffId == null)
            {
                return NotFound();
            }

            var wardStaff = await _context.WardStaffs
                .Include(w => w.Ward)
                .SingleOrDefaultAsync(m => m.WardStaffId == wardStaffId && m.WardId == wardId);
            if (wardStaff == null)
            {
                return NotFound();
            }

            return View(wardStaff);
        }

        // GET: WardStaffs/Create
        public async Task<IActionResult> Create(Int32? wardId)
        {
            if (wardId == null)
            {
                return this.NotFound();
            }

            var ward = await this._context.Wards
                .SingleOrDefaultAsync(x => x.Id == wardId);

            if (ward == null)
            {
                return this.NotFound();
            }

            this.ViewBag.Ward = ward;
            return this.View(new WardStaffCreateModel());
        }

        // POST: WardStaffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Int32? wardId, WardStaffCreateModel model)
        {
            if (wardId == null)
            {
                return this.NotFound();
            }

            var ward = await this._context.Wards
                .Include(w => w.WardStaffs)
                .SingleOrDefaultAsync(x => x.Id == wardId);

            if (ward == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                var wardStaffId = ward.WardStaffs.Any() ? ward.WardStaffs.Max(x => x.WardStaffId) + 1 : 1;
                var wardStaff = new WardStaff 
                {
                    WardStaffId = wardStaffId,
                    WardId = ward.Id,
                    Name = model.Name,
                    Position = model.Position
                };

                _context.Add(wardStaff);
                await _context.SaveChangesAsync();
                return this.RedirectToAction("Index", new { wardId = ward.Id });
            }
            
            this.ViewBag.Ward = ward;
            return View(model);           
        }

        // GET: WardStaffs/Edit/5
        public async Task<IActionResult> Edit(Int32? wardId, Int32? wardStaffId)
        {
            if (wardId == null || wardStaffId == null)
            {
                return NotFound();
            }

            var wardStaff = await _context.WardStaffs.SingleOrDefaultAsync(m => m.WardStaffId == wardStaffId && m.WardId == wardId);

            if (wardStaff == null)
            {
                return NotFound();
            }

            var model = new WardStaffEditModel
            {
                Name = wardStaff.Name,
                Position = wardStaff.Position
            };

            this.ViewBag.WardId = wardId;
            this.ViewBag.WardStaffId = wardStaffId;
            return View(model);
        }

        // POST: WardStaffs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? wardId, Int32? wardStaffId, WardStaffEditModel model)
        {
            if (wardId == null || wardStaffId == null)
            {
                return NotFound();
            }

            var wardStaff = await this._context.WardStaffs.SingleOrDefaultAsync(m => m.WardStaffId == wardStaffId && m.WardId == wardId);

            if (wardStaff == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                wardStaff.Name = model.Name;
                wardStaff.Position = model.Position;

                await this._context.SaveChangesAsync();
                return this.RedirectToAction("Index", new { wardId = wardStaff.WardId });
            }

            return View(model);
        }

        // GET: WardStaffs/Delete/5
        public async Task<IActionResult> Delete(Int32? wardId, Int32? wardStaffId)
        {
            if (wardId == null || wardStaffId == null)
            {
                return NotFound();
            }

            var wardStaff = await _context.WardStaffs
                .Include(w => w.Ward)
                .SingleOrDefaultAsync(m => m.WardStaffId == wardStaffId && m.WardId == wardId);
            if (wardStaff == null)
            {
                return NotFound();
            }

            return View(wardStaff);
        }

        // POST: WardStaffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Int32 wardId, Int32 wardStaffId)
        {
            var wardStaff = await _context.WardStaffs.SingleOrDefaultAsync(m => m.WardStaffId == wardStaffId && m.WardId == wardId);
            _context.WardStaffs.Remove(wardStaff);
            await _context.SaveChangesAsync();
            return this.RedirectToAction("Index", new { wardId = wardStaff.WardId });
        }

    }
}
