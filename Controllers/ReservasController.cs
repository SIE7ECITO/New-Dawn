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
            var reservas = await _context.Reservas
                .Include(r => r.IdusuarioNavigation)    // Incluimos el usuario relacionado
                .Include(r => r.IdpaqueteNavigation)    // Incluimos el paquete relacionado
                .Include(r => r.HabitacionReservas)    // Incluimos las habitaciones relacionadas
                    .ThenInclude(hr => hr.IdhabitacionNavigation) // Habitaciones reservadas
                .Include(r => r.ReservaServicios)      // Incluimos los servicios relacionados
                    .ThenInclude(rs => rs.IdservicioNavigation)  // Servicios asociados
                .ToListAsync();

            return View(reservas);
        }
        // Acción para ver los detalles de una reserva
        // Acción para ver los detalles de una reserva
        // Acción para ver los detalles de una reserva
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            // Obtener la reserva con la información relacionada (habitaciones, paquete, servicios, etc.)
            var reserva = await _context.Reservas
                .Include(r => r.IdusuarioNavigation)  // Usuario asociado a la reserva
                .Include(r => r.HabitacionReservas)
                    .ThenInclude(hr => hr.IdhabitacionNavigation)  // Habitaciones reservadas
                .Include(r => r.ReservaServicios)
                    .ThenInclude(rs => rs.IdservicioNavigation)  // Servicios asociados
                .Include(r => r.IdpaqueteNavigation)  // Paquete asociado a la reserva
                .FirstOrDefaultAsync(r => r.Idreserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            // Aquí obtenemos la relación de Huesped desde HuespedReserva
            var huespedReserva = await _context.HuespedReservas
                .FirstOrDefaultAsync(hr => hr.Idreserva == reserva.Idreserva);

            // Si encontramos la relación, obtenemos el huesped usando el ID
            if (huespedReserva != null)
            {
                var huesped = await _context.Huespeds
                    .FirstOrDefaultAsync(h => h.Idhuesped == huespedReserva.Idhuesped);

                // Agregar la información del huesped al modelo
                reserva.HuespedReservas = (ICollection<HuespedReserva>?)huesped; // Agregar el detalle del huesped a la reserva
            }

            return View(reserva);
        }





        private async Task CargarDatosReservaAsync()
        {
            ViewBag.HabitacionesDisponibles = await _context.Habitacions
                .Where(h => h.EstadoHabitacion && !h.EnPaquete && h.Idhabitacion != 0)
                .ToListAsync();

            ViewBag.PaquetesDisponibles = await _context.Paquetes
                .Where(p => p.EstadoPaquete && p.Idpaquete != 0)
                .ToListAsync();

            ViewBag.ServiciosDisponibles = await _context.Servicios
                .Where(s => s.EstadoServicio)
                .ToListAsync();
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            // Cargar datos para vista
            await CargarDatosReservaAsync();

            // Inicializar reserva con valores por defecto
            var reserva = new Reserva
            {
                FechaComienzo = DateOnly.FromDateTime(DateTime.Today),
                FechaFin = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                EstadoReserva = true
            };

            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Reserva reserva,
            List<int>? habitacionesSeleccionadas,
            int? idPaquete,
            List<int>? serviciosSeleccionados)
        {
            if (reserva.FechaComienzo == null || reserva.FechaFin == null)
            {
                ModelState.AddModelError("", "Las fechas son requeridas");
            }
            else if (reserva.FechaFin.Value <= reserva.FechaComienzo.Value)
            {
                ModelState.AddModelError("", "La fecha de fin debe ser posterior a la fecha de inicio");
            }

            if (!ModelState.IsValid)
            {
                await CargarDatosReservaAsync();
                return View(reserva);
            }

            reserva.FechaReserva = DateOnly.FromDateTime(DateTime.Today);
            reserva.Idpaquete = idPaquete ?? 0;
            reserva.EstadoReserva = true;

            var usuario = await _context.Usuarios.FindAsync(1); // Temporal
            if (usuario == null)
            {
                ModelState.AddModelError("", "No se encontró el usuario");
                await CargarDatosReservaAsync();
                return View(reserva);
            }
            reserva.Idusuario = usuario.Idusuario;
            reserva.IdusuarioNavigation = usuario;

            int cantidadDias = 0;
            if (reserva.FechaComienzo != null && reserva.FechaFin != null)
            {
                var fechaInicio = reserva.FechaComienzo.Value.ToDateTime(TimeOnly.MinValue);
                var fechaFin = reserva.FechaFin.Value.ToDateTime(TimeOnly.MinValue);
                cantidadDias = (fechaFin - fechaInicio).Days;
            }
            else
            {
                ModelState.AddModelError("", "Las fechas no son válidas.");
                await CargarDatosReservaAsync();
                return View(reserva);
            }

            decimal total = 0;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pago = new Pago
                {
                    CantidadPago = 0, // Se actualiza luego
                    CantidadAbono = 0,
                    FechaPago = DateOnly.FromDateTime(DateTime.Now),
                    EstadoPago = true,
                    Idusuario = usuario.Idusuario
                };
                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                reserva.Idpago = pago.Idpago;
                reserva.Idhabitacion = 0; // Inicial
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();

                if (reserva.Idpaquete == 0)
                {
                    if (habitacionesSeleccionadas != null && habitacionesSeleccionadas.Any())
                    {
                        foreach (var idHabitacion in habitacionesSeleccionadas)
                        {
                            var habitacion = await _context.Habitacions.FindAsync(idHabitacion);
                            if (habitacion == null || !habitacion.EstadoHabitacion)
                            {
                                ModelState.AddModelError("", $"Habitación {idHabitacion} no disponible");
                                continue;
                            }

                            total += habitacion.PrecioNoche * cantidadDias;
                            _context.HabitacionReservas.Add(new HabitacionReserva
                            {
                                Idhabitacion = idHabitacion,
                                Idreserva = reserva.Idreserva
                            });
                        }

                        reserva.Idhabitacion = habitacionesSeleccionadas.FirstOrDefault();
                    }
                    else
                    {
                        _context.HabitacionReservas.Add(new HabitacionReserva
                        {
                            Idhabitacion = 0,
                            Idreserva = reserva.Idreserva
                        });
                        reserva.Idhabitacion = 0;
                    }
                }

                if (reserva.Idpaquete != 0)
                {
                    var paquete = await _context.Paquetes
                        .Include(p => p.ServicioPaquetes)
                        .FirstOrDefaultAsync(p => p.Idpaquete == reserva.Idpaquete);

                    if (paquete == null)
                    {
                        ModelState.AddModelError("", "Paquete no encontrado");
                        await transaction.RollbackAsync();
                        await CargarDatosReservaAsync();
                        return View(reserva);
                    }

                    total += paquete.Precio * cantidadDias;

                    foreach (var sp in paquete.ServicioPaquetes)
                    {
                        _context.ReservaServicios.Add(new ReservaServicio
                        {
                            Idservicio = sp.Idservicio,
                            Idreserva = reserva.Idreserva
                        });
                    }
                }

                if (serviciosSeleccionados != null)
                {
                    foreach (var idServicio in serviciosSeleccionados)
                    {
                        var servicio = await _context.Servicios.FindAsync(idServicio);
                        if (servicio == null || !servicio.EstadoServicio)
                        {
                            ModelState.AddModelError("", $"Servicio {idServicio} no disponible");
                            continue;
                        }

                        total += servicio.ValorServicio;
                        _context.ReservaServicios.Add(new ReservaServicio
                        {
                            Idservicio = servicio.Idservicio,
                            Idreserva = reserva.Idreserva
                        });
                    }
                }

                if (!ModelState.IsValid)
                {
                    await transaction.RollbackAsync();
                    await CargarDatosReservaAsync();
                    return View(reserva);
                }

                reserva.ValorTotal = (double)total;
                _context.Update(reserva);

                pago.CantidadPago = total;
                _context.Update(pago);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError("", $"Error al guardar: {mensajeError}");

                await CargarDatosReservaAsync();
                return View(reserva);
            }
        }


        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.HabitacionReservas)
                .Include(r => r.ReservaServicios)
                .Include(r => r.IdpaqueteNavigation)
                .Include(r => r.IdhabitacionNavigation)
                .Include(r => r.ReservaServicios)
                .ThenInclude(rs => rs.IdservicioNavigation)
                .Include(r => r.IdpagoNavigation)
                .FirstOrDefaultAsync(r => r.Idreserva == id);

            if (reserva == null) return NotFound();

            // Cargar todos los elementos seleccionables
            var habitacionesDisponibles = await _context.Habitacions
                .Where(h => h.EstadoHabitacion == true).ToListAsync();

            var serviciosDisponibles = await _context.Servicios
                .Where(s => s.EstadoServicio == true).ToListAsync();

            var paquetesDisponibles = await _context.Paquetes
                .Where(p => p.EstadoPaquete == true).ToListAsync();

            // Obtener los IDs seleccionados para preselección en la vista
            var habitacionesSeleccionadas = reserva.HabitacionReservas.Select(hr => hr.Idhabitacion).ToList();
            var serviciosSeleccionados = reserva.ReservaServicios.Select(rs => rs.Idservicio).ToList();

            ViewBag.Habitaciones = habitacionesDisponibles;
            ViewBag.HabitacionesSeleccionadas = habitacionesSeleccionadas;

            ViewBag.Servicios = serviciosDisponibles;
            ViewBag.ServiciosSeleccionados = serviciosSeleccionados;

            ViewBag.Paquetes = paquetesDisponibles;
            ViewBag.IdPaqueteSeleccionado = reserva.Idpaquete;

            // Puedes pasar también el pago asociado si lo vas a mostrar o editar
            ViewBag.Pago = reserva.IdpagoNavigation;


            return View(reserva);
        }



        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reserva reserva, List<int> habitacionesSeleccionadas, List<int> serviciosSeleccionados)
        {
            if (id != reserva.Idreserva) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var reservaExistente = await _context.Reservas
                        .Include(r => r.HabitacionReservas)
                        .Include(r => r.ReservaServicios)
                        .Include(r => r.IdpagoNavigation)
                        .FirstOrDefaultAsync(r => r.Idreserva == id);

                    if (reservaExistente == null) return NotFound();

                    // Actualizar datos principales
                    reservaExistente.FechaComienzo = reserva.FechaComienzo;
                    reservaExistente.FechaFin = reserva.FechaFin;
                    reservaExistente.EstadoReserva = reserva.EstadoReserva;
                    reservaExistente.FechaReserva = reserva.FechaReserva;
                    reservaExistente.Idusuario = reserva.Idusuario;

                    // Limpiar relaciones anteriores
                    _context.HabitacionReservas.RemoveRange(reservaExistente.HabitacionReservas);
                    _context.ReservaServicios.RemoveRange(reservaExistente.ReservaServicios);

                    // Habitaciones o paquete
                    if (habitacionesSeleccionadas != null && habitacionesSeleccionadas.Count > 0)
                    {
                        foreach (var idHab in habitacionesSeleccionadas)
                        {
                            reservaExistente.HabitacionReservas.Add(new HabitacionReserva
                            {
                                Idreserva = reservaExistente.Idreserva,
                                Idhabitacion = idHab
                            });
                        }

                        reservaExistente.Idhabitacion = 0;
                        reservaExistente.Idpaquete = 0;
                    }
                    else if (reserva.Idpaquete != 0)
                    {
                        reservaExistente.Idpaquete = reserva.Idpaquete;
                        reservaExistente.Idhabitacion = 0;

                        var serviciosPaquete = await _context.ServicioPaquetes
                            .Where(sp => sp.Idpaquete == reserva.Idpaquete)
                            .Select(sp => sp.Idservicio)
                            .ToListAsync();

                        serviciosSeleccionados.AddRange(serviciosPaquete);
                        serviciosSeleccionados = serviciosSeleccionados.Distinct().ToList();
                    }
                    else
                    {
                        reservaExistente.Idhabitacion = 0;
                        reservaExistente.Idpaquete = 0;
                    }

                    // Servicios
                    if (serviciosSeleccionados != null)
                    {
                        foreach (var idServ in serviciosSeleccionados.Distinct())
                        {
                            reservaExistente.ReservaServicios.Add(new ReservaServicio
                            {
                                Idreserva = reservaExistente.Idreserva,
                                Idservicio = idServ
                            });
                        }
                    }

                    // Recalcular total
                    decimal total = 0;
                    int dias = (reservaExistente.FechaFin.Value.ToDateTime(TimeOnly.MinValue) -
                                reservaExistente.FechaComienzo.Value.ToDateTime(TimeOnly.MinValue)).Days;

                    if (habitacionesSeleccionadas != null && habitacionesSeleccionadas.Count > 0)
                    {
                        var habitaciones = await _context.Habitacions
                            .Where(h => habitacionesSeleccionadas.Contains(h.Idhabitacion))
                            .ToListAsync();

                        total += habitaciones.Sum(h => h.PrecioNoche * (decimal)dias);
                    }

                    if (reservaExistente.Idpaquete != 0)
                    {
                        var paquete = await _context.Paquetes
                            .FirstOrDefaultAsync(p => p.Idpaquete == reservaExistente.Idpaquete);

                        if (paquete != null)
                            total += paquete.Precio * (decimal)dias;
                    }

                    if (serviciosSeleccionados != null && serviciosSeleccionados.Count > 0)
                    {
                        var servicios = await _context.Servicios
                            .Where(s => serviciosSeleccionados.Contains(s.Idservicio))
                            .ToListAsync();

                        total += servicios.Sum(s => s.ValorServicio);
                    }

                    // Guardar total
                    reservaExistente.ValorTotal = (double)total;

                    // Actualizar el pago
                    if (reservaExistente.IdpagoNavigation != null)
                    {
                        reservaExistente.IdpagoNavigation.CantidadPago = total;
                        _context.Pagos.Update(reservaExistente.IdpagoNavigation);
                    }

                    _context.Reservas.Update(reservaExistente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Idreserva)) return NotFound();
                    throw;
                }
            }

            // En caso de error, recargar combos
            ViewBag.Habitaciones = await _context.Habitacions.Where(h => h.EstadoHabitacion).ToListAsync();
            ViewBag.Servicios = await _context.Servicios.Where(s => s.EstadoServicio).ToListAsync();
            ViewBag.Paquetes = await _context.Paquetes.Where(p => p.EstadoPaquete).ToListAsync();

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
