using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDawn.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewDawn.Controllers
{
    [Authorize(Roles = "admin,empleado")]
    public class HabitacionesController : Controller
    {
        private readonly NewDawnContext _context;

        public HabitacionesController(NewDawnContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? capacidad, decimal? precioMin, decimal? precioMax, List<int>? comodidadesSeleccionadas)
        {
            var habitacionesQuery = _context.Habitacions
                .Include(h => h.HabitacionComodidades)
                .ThenInclude(hc => hc.IdComodidadesNavigation)
                .AsQueryable();

            if (capacidad.HasValue)
            {
                habitacionesQuery = habitacionesQuery.Where(h => h.Capacidad >= capacidad.Value);
            }

            if (precioMin.HasValue)
            {
                habitacionesQuery = habitacionesQuery.Where(h => h.PrecioNoche >= precioMin.Value);
            }

            if (precioMax.HasValue)
            {
                habitacionesQuery = habitacionesQuery.Where(h => h.PrecioNoche <= precioMax.Value);
            }

            if (comodidadesSeleccionadas != null && comodidadesSeleccionadas.Any())
            {
                habitacionesQuery = habitacionesQuery
                    .Where(h => h.HabitacionComodidades
                        .Any(hc => comodidadesSeleccionadas.Contains(hc.IdComodidades)));
            }

            // Para el filtro de comodidades en la vista
            ViewData["Comodidades"] = await _context.Comodidades.ToListAsync();

            return View(await habitacionesQuery.ToListAsync());
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
                .Include(h => h.PaqueteHabitacions)
                .FirstOrDefaultAsync(h => h.Idhabitacion == id);

            if (habitacionExistente == null)
                return NotFound();

            // Actualizar los datos de la habitación
            habitacionExistente.TipoHabitacion = habitacion.TipoHabitacion;
            habitacionExistente.EstadoHabitacion = habitacion.EstadoHabitacion;
            habitacionExistente.PrecioNoche = habitacion.PrecioNoche;
            habitacionExistente.Capacidad = habitacion.Capacidad;

            bool estabaEnPaquete = habitacionExistente.EnPaquete;
            habitacionExistente.EnPaquete = habitacion.EnPaquete;

            // Actualizar las comodidades
            if (ComodidadesSeleccionadas?.Any() == true)
            {
                // Eliminar solo las comodidades que ya no están seleccionadas
                var comodidadesAEliminar = habitacionExistente.HabitacionComodidades
                    .Where(hc => !ComodidadesSeleccionadas.Contains(hc.IdComodidades))
                    .ToList();

                _context.HabitacionComodidades.RemoveRange(comodidadesAEliminar);

                // Agregar nuevas comodidades seleccionadas
                var comodidadesExistentes = habitacionExistente.HabitacionComodidades
                    .Select(hc => hc.IdComodidades)
                    .ToList();

                var comodidadesAAgregar = ComodidadesSeleccionadas
                    .Where(idComodidad => !comodidadesExistentes.Contains(idComodidad))
                    .Select(idComodidad => new HabitacionComodidade
                    {
                        IdHabitacion = habitacion.Idhabitacion,
                        IdComodidades = idComodidad
                    });

                _context.HabitacionComodidades.AddRange(comodidadesAAgregar);
            }
            else
            {
                // Si no hay comodidades seleccionadas, eliminar todas
                _context.HabitacionComodidades.RemoveRange(habitacionExistente.HabitacionComodidades);
            }

            // Manejo de paquetes
            if (!habitacion.EnPaquete && estabaEnPaquete)
            {
                var paquetesRelacionados = habitacionExistente.PaqueteHabitacions
                    .Select(ph => ph.Idpaquete)
                    .Distinct()
                    .ToList();

                _context.PaqueteHabitacions.RemoveRange(habitacionExistente.PaqueteHabitacions);

                foreach (var idPaquete in paquetesRelacionados)
                {
                    var otrasHabitaciones = await _context.PaqueteHabitacions
                        .Where(ph => ph.Idpaquete == idPaquete && ph.Idhabitacion != habitacion.Idhabitacion)
                        .ToListAsync();

                    if (!otrasHabitaciones.Any())
                    {
                        var servicios = await _context.ServicioPaquetes
                            .Where(sp => sp.Idpaquete == idPaquete)
                            .ToListAsync();
                        _context.ServicioPaquetes.RemoveRange(servicios);

                        var paquete = await _context.Paquetes.FindAsync(idPaquete);
                        if (paquete != null)
                        {
                            _context.Paquetes.Remove(paquete);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
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