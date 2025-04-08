using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewDawn.Models;

namespace NewDawn.Controllers
{
    public class HuespedesController : Controller
    {
        private readonly NewDawnContext _context;

        public HuespedesController(NewDawnContext context)
        {
            _context = context;
        }

        // GET: Huespedes
        public async Task<IActionResult> Index(string searchCc)
        {
            var huespedes = from h in _context.Huespeds
                            select h;

            if (!string.IsNullOrEmpty(searchCc))
            {
                huespedes = huespedes.Where(h => h.Cchuesped.ToString().Contains(searchCc));
            }

            return View(await huespedes.ToListAsync());
        }


        // GET: Huespedes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var huesped = await _context.Huespeds
                .FirstOrDefaultAsync(m => m.Idhuesped == id);
            if (huesped == null)
            {
                return NotFound();
            }

            return View(huesped);
        }

        // GET: Huespedes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Huespedes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idhuesped,Cchuesped,NombreHuesped,Correo")] Huesped huesped)
        {
            if (ModelState.IsValid)
            {
                _context.Add(huesped);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(huesped);
        }

        // GET: Huespedes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var huesped = await _context.Huespeds.FindAsync(id);
            if (huesped == null)
            {
                return NotFound();
            }
            return View(huesped);
        }

        // POST: Huespedes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idhuesped,Cchuesped,NombreHuesped,Correo")] Huesped huesped)
        {
            if (id != huesped.Idhuesped)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(huesped);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HuespedExists(huesped.Idhuesped))
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
            return View(huesped);
        }

        // GET: Huespedes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var huesped = await _context.Huespeds
                .FirstOrDefaultAsync(m => m.Idhuesped == id);
            if (huesped == null)
            {
                return NotFound();
            }

            return View(huesped);
        }

        // POST: Huespedes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var huesped = await _context.Huespeds.FindAsync(id);
            if (huesped != null)
            {
                _context.Huespeds.Remove(huesped);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HuespedExists(int id)
        {
            return _context.Huespeds.Any(e => e.Idhuesped == id);
        }
    }
}
