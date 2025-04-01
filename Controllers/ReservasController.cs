using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewDawn.Models;
using NewDawn.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NewDawn.Controllers
{
    public class ReservasController : Controller
    {
        private readonly NewDawnContext _context;

        public ReservasController(NewDawnContext context)
        {
            _context = context;
        }
        // GET: Reservas/Index
        public async Task<IActionResult> Index()
        {
            var reservas = await _context.Reservas
                .Include(r => r.IdusuarioNavigation)  // Incluye el usuario
                .Include(r => r.HabitacionReservas)
                    .ThenInclude(hr => hr.IdhabitacionNavigation)  // Incluye las habitaciones
                .Include(r => r.HuespedReservas)
                    .ThenInclude(hr => hr.IdhuespedNavigation)  // Incluye los huéspedes
                .Include(r => r.ReservaServicios)
                    .ThenInclude(rs => rs.IdservicioNavigation)  // Incluye los servicios
                .Include(r => r.IdpaqueteNavigation)  // Incluye el paquete
                .ToListAsync();

            return View(reservas);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            // Convertir DateOnly a DateTime (solo la parte de la fecha)
            var today = DateTime.Now.Date;  // Solo la parte de la fecha (DateTime)

            // Seleccionar habitaciones que no están reservadas en las fechas seleccionadas
            ViewData["Habitaciones"] = _context.Habitacions
                .Where(h => !h.HabitacionReservas
                    .Any(hr => hr.IdreservaNavigation.FechaComienzo.ToDateTime(new TimeOnly(0, 0)) <= today &&
                               hr.IdreservaNavigation.FechaFin.ToDateTime(new TimeOnly(0, 0)) >= today))
                .ToList();

            // Seleccionar paquetes que no tienen habitaciones reservadas en las fechas seleccionadas
            ViewData["Paquetes"] = _context.Paquetes
                .Where(p => !p.PaqueteHabitacions
                    .Any(ph => ph.IdhabitacionNavigation.HabitacionReservas
                        .Any(hr => hr.IdreservaNavigation.FechaComienzo.ToDateTime(new TimeOnly(0, 0)) <= today &&
                                   hr.IdreservaNavigation.FechaFin.ToDateTime(new TimeOnly(0, 0)) >= today)))
                .ToList();

            // Seleccionar servicios disponibles
            ViewData["Servicios"] = _context.Servicios.ToList();

            return View();
        }






        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva, int[] selectedHabitaciones, int? selectedPaquete, int[] selectedServicios, int? selectedHuesped)
        {
            if (ModelState.IsValid)
            {
                // Asignar fecha de reserva como fecha actual
                reserva.FechaReserva = DateOnly.FromDateTime(DateTime.Now);
                reserva.EstadoReserva = true; // Estado activo por defecto

                // Verificar si el huésped existe, si no, redirigir a la página de registro de huésped
                if (selectedHuesped == null)
                {
                    // Si el huésped no está seleccionado, redirigir al registro de huespedes
                    return RedirectToAction("Create", "Huespedes");
                }

                // Validación de habitaciones disponibles
                foreach (var habitacionId in selectedHabitaciones)
                {
                    if (await IsRoomAvailable(habitacionId, reserva.FechaComienzo.ToDateTime(TimeOnly.MinValue), reserva.FechaFin.ToDateTime(TimeOnly.MinValue)))
                    {
                        ModelState.AddModelError("Habitacion", "Una o más habitaciones ya están reservadas en las fechas seleccionadas.");
                        ViewData["Habitaciones"] = _context.Habitacions.ToList();
                        ViewData["Paquetes"] = _context.Paquetes.ToList();
                        ViewData["Servicios"] = _context.Servicios.ToList();
                        return View(reserva);
                    }
                }

                // Asignar habitaciones seleccionadas
                foreach (var habitacionId in selectedHabitaciones)
                {
                    _context.HabitacionReservas.Add(new HabitacionReserva
                    {
                        Idhabitacion = habitacionId,
                        Idreserva = reserva.Idreserva
                    });
                }

                // Asignar paquete si es seleccionado
                if (selectedPaquete.HasValue)
                {
                    reserva.Idpaquete = selectedPaquete;
                }

                // Asignar servicios seleccionados
                foreach (var servicioId in selectedServicios)
                {
                    _context.ReservaServicios.Add(new ReservaServicio
                    {
                        Idreserva = reserva.Idreserva,
                        Idservicio = servicioId
                    });
                }

                // Asignar huésped
                if (selectedHuesped.HasValue)
                {
                    _context.HuespedReservas.Add(new HuespedReserva
                    {
                        Idreserva = reserva.Idreserva,
                        Idhuesped = selectedHuesped.Value
                    });
                }

                _context.Add(reserva);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["Habitaciones"] = _context.Habitacions.ToList();
            ViewData["Paquetes"] = _context.Paquetes.ToList();
            ViewData["Servicios"] = _context.Servicios.ToList();
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var reserva = await _context.Reservas.Include(r => r.HabitacionReservas)
                                                  .Include(r => r.ReservaServicios)
                                                  .Include(r => r.HuespedReservas)
                                                  .FirstOrDefaultAsync(r => r.Idreserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            ViewData["Habitaciones"] = _context.Habitacions.ToList();
            ViewData["Paquetes"] = _context.Paquetes.ToList();
            ViewData["Servicios"] = _context.Servicios.ToList();
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reserva reserva, int[] selectedHabitaciones, int? selectedPaquete, int[] selectedServicios, int? selectedHuesped)
        {
            if (id != reserva.Idreserva)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Idreserva))
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

            return View(reserva);
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.IdusuarioNavigation)  // Incluye el usuario
                .Include(r => r.HabitacionReservas)
                    .ThenInclude(hr => hr.IdhabitacionNavigation)  // Incluye las habitaciones
                .Include(r => r.HuespedReservas)
                    .ThenInclude(hr => hr.IdhuespedNavigation)  // Incluye los huéspedes
                .Include(r => r.ReservaServicios)
                    .ThenInclude(rs => rs.IdservicioNavigation)  // Incluye los servicios
                .Include(r => r.IdpaqueteNavigation)  // Incluye el paquete
                .FirstOrDefaultAsync(r => r.Idreserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }


        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(m => m.Idreserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Idreserva == id);
        }

        private async Task<bool> IsRoomAvailable(int habitacionId, DateTime fechaComienzo, DateTime fechaFin)
        {
            return !await _context.HabitacionReservas
                .Include(hr => hr.IdreservaNavigation)
                .AnyAsync(hr =>
                    hr.Idhabitacion == habitacionId &&
                    hr.IdreservaNavigation.FechaComienzo.ToDateTime(new TimeOnly(0, 0)) < fechaFin.Date &&
                    hr.IdreservaNavigation.FechaFin.ToDateTime(new TimeOnly(0, 0)) > fechaComienzo.Date &&
                    hr.IdreservaNavigation.EstadoReserva);
        }

    }
}
