using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDawn.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewDawn.Controllers
{
    public class HabitacionesController : Controller
    {
        private readonly NewDawnContext _context;

        public HabitacionesController(NewDawnContext context)
        {
            _context = context;
        }

        // GET: Habitaciones
        public async Task<IActionResult> Index()
        {
            var habitaciones = await _context.Habitacions
                .Include(h => h.HabitacionComodidades)
                .ThenInclude(hc => hc.IdComodidadesNavigation)
                .ToListAsync();
            return View(habitaciones);
        }

        // GET: Habitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var habitacion = await _context.Habitacions
                .Include(h => h.HabitacionComodidades)
                .ThenInclude(hc => hc.IdComodidadesNavigation)
                .FirstOrDefaultAsync(m => m.Idhabitacion == id);

            if (habitacion == null)
                return NotFound();

            return View(habitacion);
        }

        // GET: Habitaciones/Create
        public IActionResult Create()
        {
            ViewBag.Comodidades = _context.Comodidades.ToList();
            return View();
        }

        // POST: Habitaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Habitacion habitacion, List<int> ComodidadesSeleccionadas)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Comodidades = _context.Comodidades.ToList();
                return View(habitacion);
            }

            _context.Add(habitacion);
            await _context.SaveChangesAsync();

            if (ComodidadesSeleccionadas?.Any() == true)
            {
                var comodidades = ComodidadesSeleccionadas.Select(id => new HabitacionComodidade
                {
                    IdHabitacion = habitacion.Idhabitacion,
                    IdComodidades = id
                });
                _context.HabitacionComodidades.AddRange(comodidades);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Habitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var habitacion = await _context.Habitacions
                .Include(h => h.HabitacionComodidades)
                .FirstOrDefaultAsync(h => h.Idhabitacion == id);

            if (habitacion == null)
                return NotFound();

            ViewBag.Comodidades = _context.Comodidades.ToList();
            ViewBag.ComodidadesSeleccionadas = habitacion.HabitacionComodidades.Select(hc => hc.IdComodidades).ToList();

            return View(habitacion);
        }

        // POST: Habitaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Habitacion habitacion, List<int> ComodidadesSeleccionadas)
        {
            if (id != habitacion.Idhabitacion)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Comodidades = _context.Comodidades.ToList();
                ViewBag.ComodidadesSeleccionadas = ComodidadesSeleccionadas;
                return View(habitacion);
            }

            var habitacionExistente = await _context.Habitacions
                .Include(h => h.HabitacionComodidades)
                .FirstOrDefaultAsync(h => h.Idhabitacion == id);

            if (habitacionExistente == null)
                return NotFound();

            habitacionExistente.TipoHabitacion = habitacion.TipoHabitacion;
            habitacionExistente.EstadoHabitacion = habitacion.EstadoHabitacion;
            habitacionExistente.PrecioNoche = habitacion.PrecioNoche;
            habitacionExistente.EnPaquete = habitacion.EnPaquete;
            habitacionExistente.Capacidad = habitacion.Capacidad;

            _context.HabitacionComodidades.RemoveRange(habitacionExistente.HabitacionComodidades);
            await _context.SaveChangesAsync();

            if (ComodidadesSeleccionadas?.Any() == true)
            {
                var comodidades = ComodidadesSeleccionadas.Select(idComodidad => new HabitacionComodidade
                {
                    IdHabitacion = habitacion.Idhabitacion,
                    IdComodidades = idComodidad
                });
                _context.HabitacionComodidades.AddRange(comodidades);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Habitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(Index));

            var habitacion = await _context.Habitacions
                .Include(h => h.HabitacionComodidades)
                    .ThenInclude(hc => hc.IdComodidadesNavigation) // Incluye la relación con Comodidades
                .Include(h => h.PaqueteHabitacions) // Si hay paquetes asociados
                .FirstOrDefaultAsync(m => m.Idhabitacion == id);

            if (habitacion == null)
                return RedirectToAction(nameof(Index));

            if (habitacion.PaqueteHabitacions.Any())
            {
                ViewBag.ErrorMessage = "No se puede eliminar la habitación porque está asociada a un paquete. Primero elimínela del paquete.";
            }

            return View(habitacion);
        }


        // POST: Habitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var habitacion = await _context.Habitacions
                .Include(h => h.HabitacionComodidades)
                .Include(h => h.PaqueteHabitacions)
                .FirstOrDefaultAsync(h => h.Idhabitacion == id);

            if (habitacion == null)
                return RedirectToAction(nameof(Index));

            if (habitacion.PaqueteHabitacions.Any())
            {
                TempData["ErrorMessage"] = "No se puede eliminar la habitación porque está asociada a un paquete. Primero elimínela del paquete.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.HabitacionComodidades.RemoveRange(habitacion.HabitacionComodidades);
            _context.Habitacions.Remove(habitacion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}