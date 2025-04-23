using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDawn.Models;

namespace NewDawn.Controllers
{
    [Authorize(Roles = "admin,empleado")]
    public class ServiciosController : Controller
    {
        private readonly NewDawnContext _context;

        public ServiciosController(NewDawnContext context)
        {
            _context = context;
        }

        // GET: Servicios
        public async Task<IActionResult> Index()
        {
            var servicios = await _context.Servicios.AsNoTracking().ToListAsync();
            return View(servicios);
        }

        // GET: Servicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var servicio = await _context.Servicios.AsNoTracking().FirstOrDefaultAsync(m => m.Idservicio == id);
            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        // GET: Servicios/Create
        public IActionResult Create() => View();

        // POST: Servicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idservicio,NombreServicio,DescripcionServicio,ValorServicio,FechaCreacion")] Servicio servicio)
        {
            if (!ModelState.IsValid)
                return View(servicio);

            try
            {
                servicio.EstadoServicio = true; 

                _context.Add(servicio);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Servicio creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el servicio: {ex.Message}");
                return View(servicio);
            }
        }

        // GET: Servicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        // POST: Servicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idservicio,NombreServicio,DescripcionServicio,ValorServicio,FechaCreacion,EstadoServicio")] Servicio servicio)
        {
            if (id != servicio.Idservicio)
                return NotFound();

            if (!ModelState.IsValid)
                return View(servicio);

            try
            {
                _context.Update(servicio);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Servicio actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicioExists(servicio.Idservicio))
                    return NotFound();

                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el servicio: {ex.Message}");
                return View(servicio);
            }
        }

        // GET: Servicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var servicio = await _context.Servicios.AsNoTracking().FirstOrDefaultAsync(m => m.Idservicio == id);
            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        // POST: Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var servicio = await _context.Servicios.FindAsync(id);
                if (servicio == null)
                    return NotFound();
                else { 
                    _context.Servicios.Remove(servicio);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Servicio eliminado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el servicio: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ServicioExists(int id) => _context.Servicios.Any(e => e.Idservicio == id);
    }
}
