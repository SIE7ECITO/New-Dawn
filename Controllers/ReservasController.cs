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
    public class ReservasController : Controller
    {
        private readonly NewDawnContext _context;

        public ReservasController(NewDawnContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var newDawnContext = _context.Reservas
                .Include(r => r.IdhabitacionNavigation)
                .Include(r => r.IdpagoNavigation)
                .Include(r => r.IdpaqueteNavigation)
                .Include(r => r.IdusuarioNavigation);
            return View(await newDawnContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.IdhabitacionNavigation)
                .Include(r => r.IdpagoNavigation)
                .Include(r => r.IdpaqueteNavigation)
                .Include(r => r.IdusuarioNavigation)
                .FirstOrDefaultAsync(m => m.Idreserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }


        // GET: Reservas/Create
        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["Idhabitacion"] = new SelectList(_context.Habitacions, "Idhabitacion", "TipoHabitacion");
            ViewData["Servicios"] = new SelectList(_context.Servicios, "Idservicio", "NombreServicio");
            ViewData["Idpaquete"] = new SelectList(_context.Paquetes, "Idpaquete", "NombrePaquete");
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "NombreUsuario");

            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Idreserva,Idusuario,Idpago,FechaReserva,FechaComienzo,FechaFin,EstadoReserva")] Reserva reserva,
            int[] habitacionesSeleccionadas,
            int[] selectedServices,
            int? paqueteSeleccionado)
        {
            if (ModelState.IsValid)
            {
                // Validar disponibilidad de habitaciones
                foreach (var habitacionId in habitacionesSeleccionadas)
                {
                    if (!await IsRoomAvailable(habitacionId, reserva.FechaComienzo, reserva.FechaFin))
                    {
                        ModelState.AddModelError("Idhabitacion", "Una o más habitaciones ya están reservadas en las fechas seleccionadas.");

                        // Recargar los datos en caso de error
                        ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "NombreUsuario", reserva.Idusuario);
                        ViewData["Idhabitacion"] = new SelectList(_context.Habitacions, "Idhabitacion", "TipoHabitacion", habitacionesSeleccionadas);
                        ViewData["Idpaquete"] = new SelectList(_context.Paquetes, "Idpaquete", "NombrePaquete", paqueteSeleccionado);
                        ViewData["Servicios"] = new SelectList(_context.Servicios, "Idservicio", "NombreServicio");

                        return View(reserva);
                    }
                }

                // Guardar la reserva
                _context.Add(reserva);
                await _context.SaveChangesAsync();

                // Asociar habitaciones seleccionadas con la reserva
                foreach (var habitacionId in habitacionesSeleccionadas)
                {
                    var habitacionReserva = new HabitacionReserva
                    {
                        Idreserva = reserva.Idreserva,
                        Idhabitacion = habitacionId
                    };
                    _context.HabitacionReservas.Add(habitacionReserva);
                }

                // Asociar servicios seleccionados con la reserva
                if (selectedServices != null && selectedServices.Any())
                {
                    foreach (var serviceId in selectedServices)
                    {
                        var reservaServicio = new ReservaServicio
                        {
                            Idreserva = reserva.Idreserva,
                            Idservicio = serviceId
                        };
                        _context.ReservaServicios.Add(reservaServicio);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recargar los datos en caso de error
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "NombreUsuario", reserva.Idusuario);
            ViewData["Idhabitacion"] = new SelectList(_context.Habitacions, "Idhabitacion", "TipoHabitacion", habitacionesSeleccionadas);
            ViewData["Idpaquete"] = new SelectList(_context.Paquetes, "Idpaquete", "NombrePaquete", paqueteSeleccionado);
            ViewData["Servicios"] = new SelectList(_context.Servicios, "Idservicio", "NombreServicio");

            return View(reserva);
        }

        // ✅ Método corregido para verificar disponibilidad de habitaciones
        private async Task<bool> IsRoomAvailable(int habitacionId, DateOnly fechaComienzo, DateOnly fechaFin)
        {
            DateTime fechaComienzoDateTime = fechaComienzo.ToDateTime(TimeOnly.MinValue);
            DateTime fechaFinDateTime = fechaFin.ToDateTime(TimeOnly.MinValue);

            // Verifica en HabitacionReserva si la habitación ya está reservada en ese rango de fechas
            var existeReserva = await _context.HabitacionReservas
      .Where(hr => hr.Idhabitacion == habitacionId &&
                   hr.IdreservaNavigation.FechaComienzo.ToDateTime(TimeOnly.MinValue) < fechaFinDateTime &&
                   hr.IdreservaNavigation.FechaFin.ToDateTime(TimeOnly.MinValue) > fechaComienzoDateTime &&
                   hr.IdreservaNavigation.EstadoReserva)
      .AnyAsync();


            return !existeReserva;
        }


        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            // Cargar las opciones de habitación, pago, paquete y usuario
            ViewData["Idhabitacion"] = new SelectList(_context.Habitacions, "Idhabitacion", "Idhabitacion", reserva.Idhabitacion);
            ViewData["Idpago"] = new SelectList(_context.Pagos, "Idpago", "Idpago", reserva.Idpago);
            ViewData["Idpaquete"] = new SelectList(_context.Paquetes, "Idpaquete", "Idpaquete", reserva.Idpaquete);
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario", reserva.Idusuario);

            // Si la reserva tiene paquete, también cargar los servicios ya seleccionados
            if (reserva.Idpaquete != null)
            {
                var selectedServices = _context.ReservaServicios
                    .Where(rs => rs.Idreserva == reserva.Idreserva)
                    .Select(rs => rs.Idservicio)
                    .ToArray();
                ViewBag.SelectedServices = selectedServices;
            }

            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Idreserva,Idusuario,Idhabitacion,Idpaquete,Idpago,FechaReserva,FechaComienzo,FechaFin,EstadoReserva")] Reserva reserva,
            int[]? selectedServices)
        {
            if (id != reserva.Idreserva)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Validar disponibilidad de la habitación excluyendo la reserva actual
                if (!await IsRoomAvailable(reserva.Idhabitacion, reserva.FechaComienzo, reserva.FechaFin, reserva.Idreserva))
                {
                    ModelState.AddModelError("Idhabitacion", "La habitación ya está reservada en las fechas seleccionadas.");
                    ViewData["Idhabitacion"] = new SelectList(_context.Habitacions, "Idhabitacion", "Idhabitacion", reserva.Idhabitacion);
                    ViewData["Idpago"] = new SelectList(_context.Pagos, "Idpago", "Idpago", reserva.Idpago);
                    ViewData["Idpaquete"] = new SelectList(_context.Paquetes, "Idpaquete", "Idpaquete", reserva.Idpaquete);
                    ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario", reserva.Idusuario);
                    return View(reserva);
                }

                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();

                    // Si se selecciona paquete, actualizar los servicios asociados
                    if (reserva.Idpaquete != null)
                    {
                        // Eliminar los servicios previos asociados a esta reserva
                        var prevServices = _context.ReservaServicios.Where(rs => rs.Idreserva == reserva.Idreserva);
                        _context.ReservaServicios.RemoveRange(prevServices);
                        await _context.SaveChangesAsync();

                        if (selectedServices != null && selectedServices.Any())
                        {
                            foreach (var serviceId in selectedServices)
                            {
                                ReservaServicio rs = new ReservaServicio
                                {
                                    Idreserva = reserva.Idreserva,
                                    Idservicio = serviceId
                                };
                                _context.ReservaServicios.Add(rs);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
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

            ViewData["Idhabitacion"] = new SelectList(_context.Habitacions, "Idhabitacion", "Idhabitacion", reserva.Idhabitacion);
            ViewData["Idpago"] = new SelectList(_context.Pagos, "Idpago", "Idpago", reserva.Idpago);
            ViewData["Idpaquete"] = new SelectList(_context.Paquetes, "Idpaquete", "Idpaquete", reserva.Idpaquete);
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario", reserva.Idusuario);
            return View(reserva);
        }


        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.IdhabitacionNavigation)
                .Include(r => r.IdpagoNavigation)
                .Include(r => r.IdpaqueteNavigation)
                .Include(r => r.IdusuarioNavigation)
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
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Idreserva == id);
        }

        /// <summary>
        /// Valida si la habitación se encuentra disponible en el rango de fechas especificado.
        /// Se revisa tanto en la tabla principal de Reservas como en la relación HabitacionReserva.
        /// Se puede excluir una reserva en particular (por ejemplo, al editar).
        /// </summary>
        private async Task<bool> IsRoomAvailable(int idHabitacion, DateOnly fechaInicio, DateOnly fechaFin, int? reservaId = null)
        {
            // Verificar en la tabla principal de Reservas
            var queryReservas = _context.Reservas
                .Where(r => r.Idhabitacion == idHabitacion &&
                            r.FechaFin >= fechaInicio &&
                            r.FechaComienzo <= fechaFin);
            if (reservaId.HasValue)
            {
                queryReservas = queryReservas.Where(r => r.Idreserva != reservaId.Value);
            }
            bool reservaExistente = await queryReservas.AnyAsync();

            // Verificar en la relación HabitacionReserva
            var queryHabitacionReserva = _context.HabitacionReservas
                .Include(hr => hr.IdreservaNavigation)
                .Where(hr => hr.Idhabitacion == idHabitacion &&
                             hr.IdreservaNavigation.FechaFin >= fechaInicio &&
                             hr.IdreservaNavigation.FechaComienzo <= fechaFin);
            if (reservaId.HasValue)
            {
                queryHabitacionReserva = queryHabitacionReserva.Where(hr => hr.Idreserva != reservaId.Value);
            }
            bool habitacionReservaExistente = await queryHabitacionReserva.AnyAsync();

            return !(reservaExistente || habitacionReservaExistente);
        }
    }
}
