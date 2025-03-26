using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDawn.Models;

namespace NewDawn.Controllers
{
    public class ComodidadesController : Controller
    {
        private readonly NewDawnContext _context;

        public ComodidadesController(NewDawnContext context)
        {
            _context = context;
        }

        // GET: Comodidades
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comodidades.ToListAsync());
        }

        // GET: Comodidades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var comodidade = await _context.Comodidades.FirstOrDefaultAsync(m => m.IdComodidades == id);
            if (comodidade == null)
                return NotFound();

            return View(comodidade);
        }

        // GET: Comodidades/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comodidades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreComodidades,DescripcionComodidad,EstadoComodidad")] Comodidade comodidade)
        {
            if (!ModelState.IsValid)
                return View(comodidade);

            _context.Add(comodidade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ComodidadeExists(int id)
        {
            return _context.Comodidades.Any(e => e.IdComodidades == id);
        }


        // GET: Comodidades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var comodidade = await _context.Comodidades.FindAsync(id);
            if (comodidade == null)
                return NotFound();

            return View(comodidade);
        }

        // POST: Comodidades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NombreComodidades,DescripcionComodidad,EstadoComodidad")] Comodidade comodidade)
        {
            if (!ModelState.IsValid)
                return View(comodidade);

            try
            {
                comodidade.IdComodidades = id; // Asegura que el ID no se pierda
                _context.Update(comodidade);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComodidadeExists(id))
                    return NotFound();

                ModelState.AddModelError("", "Error al actualizar la base de datos. Intenta de nuevo.");
                return View(comodidade);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Comodidades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var comodidade = await _context.Comodidades.FirstOrDefaultAsync(m => m.IdComodidades == id);
            if (comodidade == null)
                return NotFound();

            return View(comodidade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comodidade = await _context.Comodidades.FindAsync(id);
            if (comodidade == null)
            {
                return NotFound();
            }

            try
            {
                _context.Comodidades.Remove(comodidade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "No se puede eliminar la comodidad porque está asociada a una habitación. Primero desvincúlela de las habitaciones.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

    }
}
