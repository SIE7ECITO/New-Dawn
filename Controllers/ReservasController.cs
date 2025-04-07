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
            var newDawnContext = _context.Reservas.Include(r => r.IdhabitacionNavigation).Include(r => r.IdpagoNavigation).Include(r => r.IdpaqueteNavigation).Include(r => r.IdusuarioNavigation);
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
        public IActionResult Create()
        {
            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "Idusuario", "Nombre");
            ViewData["Paquetes"] = new SelectList(_context.Paquetes, "Idpaquete", "NombrePaquete");
            ViewData["Habitaciones"] = new MultiSelectList(_context.Habitacions, "Idhabitacion", "TipoHabitacion");
            ViewData["Servicios"] = new MultiSelectList(_context.Servicios, "Idservicio", "NombreServicio");

            ViewBag.HabitacionesData = _context.Habitacions
                .Select(h => new { h.Idhabitacion, h.PrecioNoche })
                .ToList();

            ViewBag.ServiciosData = _context.Servicios
                .Select(s => new { s.Idservicio, s.ValorServicio })
                .ToList();

            ViewBag.PaquetesData = _context.Paquetes
                .Select(p => new { p.Idpaquete, p.Precio })
                .ToList();

            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Idreserva,Idusuario,Idpaquete,Idpago,FechaReserva,FechaComienzo,FechaFin,EstadoReserva")] Reserva reserva,
                int[] selectedHabitaciones,
                int[] selectedServicios)
        {
            if (reserva.FechaFin < reserva.FechaComienzo)
            {
                ModelState.AddModelError("", "La fecha de fin no puede ser menor a la fecha de comienzo.");
            }

            if (ModelState.IsValid)
            {
                decimal valorTotal = 0m;

                // 1. Habitaciones
                foreach (var habitacionId in selectedHabitaciones)
                {
                    _context.HabitacionReservas.Add(new HabitacionReserva
                    {
                        Idreserva = reserva.Idreserva,
                        Idhabitacion = habitacionId
                    });

                    var habitacion = await _context.Habitacions.FindAsync(habitacionId);
                    if (habitacion != null)
                    {
                        var fechaInicio = reserva.FechaComienzo.ToDateTime(TimeOnly.MinValue);
                        var fechaFin = reserva.FechaFin.ToDateTime(TimeOnly.MinValue);
                        int dias = (fechaFin - fechaInicio).Days;
                        valorTotal += (decimal)habitacion.PrecioNoche * dias;
                    }
                }

                // 2. Servicios
                foreach (var servicioId in selectedServicios)
                {
                    _context.ReservaServicios.Add(new ReservaServicio
                    {
                        Idreserva = reserva.Idreserva,
                        Idservicio = servicioId
                    });

                    var servicio = await _context.Servicios.FindAsync(servicioId);
                    if (servicio != null)
                        valorTotal += servicio.ValorServicio;
                }

                // 3. Paquete (opcional)
                if (reserva.Idpaquete != null)
                {
                    var paquete = await _context.Paquetes.FindAsync(reserva.Idpaquete);
                    if (paquete != null)
                        valorTotal += paquete.Precio;
                }

                reserva.ValorTotal = (double)valorTotal;
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "Idusuario", "Nombre", reserva.Idusuario);
            ViewData["Paquetes"] = new SelectList(_context.Paquetes, "Idpaquete", "NombrePaquete", reserva.Idpaquete);
            ViewData["Habitaciones"] = new MultiSelectList(_context.Habitacions, "Idhabitacion", "TipoHabitacion", selectedHabitaciones);
            ViewData["Servicios"] = new MultiSelectList(_context.Servicios, "Idservicio", "NombreServicio", selectedServicios);
            return View(reserva);
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
            ViewData["Idhabitacion"] = new SelectList(_context.Habitacions, "Idhabitacion", "Idhabitacion", reserva.Idhabitacion);
            ViewData["Idpago"] = new SelectList(_context.Pagos, "Idpago", "Idpago", reserva.Idpago);
            ViewData["Idpaquete"] = new SelectList(_context.Paquetes, "Idpaquete", "Idpaquete", reserva.Idpaquete);
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario", reserva.Idusuario);
            ViewBag.HabitacionesPrecio = _context.Habitacions.ToDictionary(h => h.Idhabitacion.ToString(), h => h.PrecioNoche);
            ViewBag.ServiciosPrecio = _context.Servicios.ToDictionary(s => s.Idservicio.ToString(), s => s.ValorServicio);
            ViewBag.PaquetesPrecio = _context.Paquetes.ToDictionary(p => p.Idpaquete.ToString(), p => p.Precio);

            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idreserva,Idusuario,Idhabitacion,Idpaquete,Idpago,FechaReserva,FechaComienzo,FechaFin,ValorTotal,EstadoReserva")] Reserva reserva)
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
    }
}
