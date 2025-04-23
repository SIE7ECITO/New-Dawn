using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewDawn.Models;

namespace NewDawn.Controllers
{
    [Authorize]
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
            // Obtener el ID del usuario autenticado desde los claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                // Si no hay usuario autenticado, redirigir al login
                return RedirectToAction("Login", "Usuarios");
            }

            // Convertir el ID del usuario a entero
            if (!int.TryParse(userIdClaim, out int userId))
            {
                ModelState.AddModelError("", "Error al obtener el ID del usuario autenticado.");
                return RedirectToAction("Login", "Usuarios");
            }

            // Obtener el rol del usuario autenticado desde los claims
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(userRole))
            {
                ModelState.AddModelError("", "Error al obtener el rol del usuario autenticado.");
                return RedirectToAction("Login", "Usuarios");
            }

            // Consultar las reservas
            IEnumerable<Reserva> reservas;
            if (userRole.ToLower() == "admin" || userRole.ToLower() == "empleado") // Mostrar todas las reservas si el usuario es administrador
            {
                reservas = await _context.Reservas
                    .Include(r => r.IdusuarioNavigation)    // Incluimos el usuario relacionado
                    .Include(r => r.IdpaqueteNavigation)    // Incluimos el paquete relacionado
                    .Include(r => r.HabitacionReservas)    // Incluimos las habitaciones relacionadas
                        .ThenInclude(hr => hr.IdhabitacionNavigation) // Habitaciones reservadas
                    .Include(r => r.ReservaServicios)      // Incluimos los servicios relacionados
                        .ThenInclude(rs => rs.IdservicioNavigation)  // Servicios asociados
                    .ToListAsync();

                return View(reservas); // Renderiza la vista para admin
            }
            else // Mostrar solo las reservas del usuario autenticado
            {
                reservas = await _context.Reservas
                    .Where(r => r.Idusuario == userId) // Filtrar por el ID del usuario autenticado
                    .Include(r => r.IdpaqueteNavigation)    // Incluimos el paquete relacionado
                    .Include(r => r.HabitacionReservas)    // Incluimos las habitaciones relacionadas
                        .ThenInclude(hr => hr.IdhabitacionNavigation) // Habitaciones reservadas
                    .Include(r => r.ReservaServicios)      // Incluimos los servicios relacionados
                        .ThenInclude(rs => rs.IdservicioNavigation)  // Servicios asociados
                    .ToListAsync();

                return View("IndexUsuario", reservas); // Renderiza una vista diferente para usuarios normales
            }
        }
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
                .Where(h => h.EstadoHabitacion && h.Idhabitacion != 0) // Excluir habitación con ID 0
                .ToListAsync();

            ViewBag.PaquetesDisponibles = await _context.Paquetes
                .Where(p => p.EstadoPaquete && p.Idpaquete != 0) // Excluir paquetes con ID 0 si aplica
                .ToListAsync();

            ViewBag.ServiciosDisponibles = await _context.Servicios
                .Where(s => s.EstadoServicio)
                .ToListAsync();
        }
        [HttpPost]
        public async Task<JsonResult> FiltrarHabitaciones(DateTime fechaInicio, DateTime fechaFin)
        {
            // Obtener habitaciones ocupadas en el rango de fechas
            var habitacionesOcupadas = await _context.HabitacionReservas
                .Include(hr => hr.IdreservaNavigation)
                .Where(hr =>
                    hr.IdreservaNavigation.EstadoReserva && // Solo reservas activas
                    hr.IdreservaNavigation.FechaFin.HasValue &&
                    hr.IdreservaNavigation.FechaComienzo.HasValue &&
                    (
                        hr.IdreservaNavigation.FechaFin.Value > DateOnly.FromDateTime(fechaInicio) &&
                        hr.IdreservaNavigation.FechaComienzo.Value < DateOnly.FromDateTime(fechaFin)
                    )
                )
                .Select(hr => hr.Idhabitacion)
                .ToListAsync();

            // Filtrar habitaciones disponibles excluyendo ID 0
            var habitacionesDisponibles = await _context.Habitacions
                .Where(h =>
                    h.EstadoHabitacion && // Solo habitaciones activas
                    h.Idhabitacion != 0 && // Excluir habitación con ID 0
                    !habitacionesOcupadas.Contains(h.Idhabitacion) // Excluir habitaciones ocupadas
                )
                .ToListAsync();

            return Json(habitacionesDisponibles);
        }


        // GET: Create
        public async Task<IActionResult> Create()
        {
            // Cargar datos para la vista
            await CargarDatosReservaAsync();

            // Inicializar el modelo de reserva con valores predeterminados
            var reserva = new Reserva
            {
                FechaComienzo = DateOnly.FromDateTime(DateTime.Today),
                FechaFin = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                EstadoReserva = true
            };

            // Obtener el rol del usuario autenticado
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole?.ToLower() == "admin" || userRole?.ToLower() == "empleado")
            {
                return View(reserva); 
            }
            else
            {
                return View("CreateUsuario", reserva); 
            }
        }



        //POST create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    Reserva reserva,
    List<int>? habitacionesSeleccionadas,
    int? idPaquete,
    List<int>? serviciosSeleccionados)
        {
            // Validar si las fechas son válidas
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

            // Obtener el usuario autenticado
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioIdClaim))
            {
                ModelState.AddModelError("", "No se encontró el usuario autenticado.");
                await CargarDatosReservaAsync();
                return View(reserva);
            }

            var usuario = await _context.Usuarios.FindAsync(int.Parse(usuarioIdClaim)); // Buscar usuario por ID
            if (usuario == null)
            {
                ModelState.AddModelError("", "No se encontró el usuario en la base de datos.");
                await CargarDatosReservaAsync();
                return View(reserva);
            }

            // Asignar valores relacionados con el usuario
            reserva.FechaReserva = DateOnly.FromDateTime(DateTime.Today);
            reserva.Idpaquete = idPaquete ?? 0;
            reserva.EstadoReserva = true;
            reserva.Idusuario = usuario.Idusuario;
            reserva.IdusuarioNavigation = usuario;

            // Calcular cantidad de días
            int cantidadDias = (reserva.FechaFin.Value.ToDateTime(TimeOnly.MinValue)
                                - reserva.FechaComienzo.Value.ToDateTime(TimeOnly.MinValue)).Days;

            if (cantidadDias <= 0)
            {
                ModelState.AddModelError("", "Las fechas no son válidas.");
                await CargarDatosReservaAsync();
                return View(reserva);
            }

            // Inicializar variables
            decimal total = 0;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Crear el pago inicial
                var pago = new Pago
                {
                    CantidadPago = 0, // Se actualizará más adelante
                    CantidadAbono = 0,
                    FechaPago = DateOnly.FromDateTime(DateTime.Now),
                    EstadoPago = true,
                    Idusuario = usuario.Idusuario
                };
                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                // Asignar el pago a la reserva
                reserva.Idpago = pago.Idpago;
                reserva.Idhabitacion = 0; // Inicial
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();

                // Procesar habitaciones seleccionadas
                if (reserva.Idpaquete == 0 && habitacionesSeleccionadas != null && habitacionesSeleccionadas.Any())
                {
                    foreach (var idHabitacion in habitacionesSeleccionadas)
                    {
                        var habitacion = await _context.Habitacions.FindAsync(idHabitacion);
                        if (habitacion == null || !habitacion.EstadoHabitacion)
                        {
                            ModelState.AddModelError("", $"Habitación {idHabitacion} no disponible.");
                            continue;
                        }

                        // Verificar solapamiento de fechas
                        var conflictos = await _context.HabitacionReservas
                            .Where(hr => hr.Idhabitacion == idHabitacion)
                            .Include(hr => hr.IdreservaNavigation)
                            .Where(hr =>
                                hr.IdreservaNavigation != null &&
                                hr.IdreservaNavigation.EstadoReserva &&
                                hr.IdreservaNavigation.FechaFin > reserva.FechaComienzo &&
                                hr.IdreservaNavigation.FechaComienzo < reserva.FechaFin)
                            .ToListAsync();

                        if (conflictos.Any())
                        {
                            ModelState.AddModelError("", $"La habitación {idHabitacion} ya está reservada en las fechas seleccionadas.");
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

                // Procesar paquete seleccionado
                if (reserva.Idpaquete != 0)
                {
                    var paquete = await _context.Paquetes
                        .Include(p => p.PaqueteHabitacions)
                        .FirstOrDefaultAsync(p => p.Idpaquete == reserva.Idpaquete);

                    foreach (var hp in paquete.PaqueteHabitacions)
                    {
                        var conflictos = await _context.HabitacionReservas
                            .Where(hr => hr.Idhabitacion == hp.Idhabitacion)
                            .Include(hr => hr.IdreservaNavigation)
                            .Where(hr =>
                                hr.IdreservaNavigation != null &&
                                hr.IdreservaNavigation.EstadoReserva &&
                                hr.IdreservaNavigation.FechaFin > reserva.FechaComienzo &&
                                hr.IdreservaNavigation.FechaComienzo < reserva.FechaFin)
                            .ToListAsync();

                        if (conflictos.Any())
                        {
                            ModelState.AddModelError("", $"La habitación del paquete con ID {hp.Idhabitacion} ya está reservada en las fechas seleccionadas.");
                        }
                    }
                }

                // Procesar servicios seleccionados
                if (serviciosSeleccionados != null)
                {
                    foreach (var idServicio in serviciosSeleccionados)
                    {
                        var servicio = await _context.Servicios.FindAsync(idServicio);
                        if (servicio == null || !servicio.EstadoServicio)
                        {
                            ModelState.AddModelError("", $"Servicio {idServicio} no disponible.");
                            continue;
                        }

                        total += servicio.ValorServicio;
                        if (!_context.ReservaServicios.Local.Any(rs => rs.Idreserva == reserva.Idreserva && rs.Idservicio == idServicio))
                        {
                            _context.ReservaServicios.Add(new ReservaServicio
                            {
                                Idservicio = idServicio,
                                Idreserva = reserva.Idreserva
                            });
                        }
                    }
                }

                // Validar estado del modelo
                if (!ModelState.IsValid)
                {
                    await transaction.RollbackAsync();
                    await CargarDatosReservaAsync();
                    return View(reserva);
                }

                // Actualizar valores totales
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
                ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                await CargarDatosReservaAsync();
                return View(reserva);
            }
        }


        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.HabitacionReservas)
                .Include(r => r.ReservaServicios)
                .FirstOrDefaultAsync(r => r.Idreserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            // Cargar datos para habitaciones, paquetes y servicios
            ViewBag.Habitaciones = await _context.Habitacions
                .Where(h => h.EstadoHabitacion && h.Idhabitacion != 0) // Excluir habitación con ID 0
                .ToListAsync();

            ViewBag.PaquetesDisponibles = await _context.Paquetes
                .Where(p => p.EstadoPaquete && p.Idpaquete != 0) // Excluir paquetes inválidos
                .ToListAsync();

            ViewBag.ServiciosDisponibles = await _context.Servicios
                .Where(s => s.EstadoServicio)
                .ToListAsync();

            return View(reserva);
        }


        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Reserva reserva,
            List<int>? habitacionesSeleccionadas,
            int? idPaquete,
            List<int>? serviciosSeleccionados)
        {
            if (id != reserva.Idreserva)
            {
                return NotFound();
            }

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

            var existingReserva = await _context.Reservas
                .Include(r => r.HabitacionReservas)
                .Include(r => r.ReservaServicios)
                .FirstOrDefaultAsync(r => r.Idreserva == id);

            if (existingReserva == null)
            {
                return NotFound();
            }

            existingReserva.FechaComienzo = reserva.FechaComienzo;
            existingReserva.FechaFin = reserva.FechaFin;
            existingReserva.Idpaquete = idPaquete ?? 0;

            // Update habitaciones and servicios logic similar to Create method

            decimal total = 0;
            int cantidadDias = (reserva.FechaFin.Value.ToDateTime(TimeOnly.MinValue) - reserva.FechaComienzo.Value.ToDateTime(TimeOnly.MinValue)).Days;

            
            // Habitaciones logic
            _context.HabitacionReservas.RemoveRange(existingReserva.HabitacionReservas); // Limpia las habitaciones existentes
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
                    existingReserva.HabitacionReservas.Add(new HabitacionReserva
                    {
                        Idhabitacion = idHabitacion,
                        Idreserva = existingReserva.Idreserva 
                    });
                }
            }

            // Paquete and servicios logic
            if (reserva.Idpaquete != 0)
            {
                var paquete = await _context.Paquetes
                    .Include(p => p.ServicioPaquetes)
                    .FirstOrDefaultAsync(p => p.Idpaquete == reserva.Idpaquete);

                if (paquete == null)
                {
                    ModelState.AddModelError("", "Paquete no encontrado");
                    await CargarDatosReservaAsync();
                    return View(reserva);
                }

                total += paquete.Precio * cantidadDias;

                foreach (var sp in paquete.ServicioPaquetes)
                {
                    if (!_context.ReservaServicios.Local.Any(rs => rs.Idreserva == reserva.Idreserva && rs.Idservicio == sp.Idservicio))
                    {
                        existingReserva.ReservaServicios.Add(new ReservaServicio
                        {
                            Idservicio = sp.Idservicio,
                            Idreserva = existingReserva.Idreserva
                        });
                    }
                }
            }

            /// Servicios logic
            var reservaServiciosCopia = existingReserva.ReservaServicios.ToList(); // Crear una copia para evitar modificar la colección durante la iteración
            _context.ReservaServicios.RemoveRange(reservaServiciosCopia); // Limpia las asociaciones de servicios existentes

            if (serviciosSeleccionados != null && serviciosSeleccionados.Any())
            {
                foreach (var idServicio in serviciosSeleccionados)
                {
                    var servicio = await _context.Servicios.FindAsync(idServicio);
                    if (servicio == null || !servicio.EstadoServicio)
                    {
                        ModelState.AddModelError("", $"Servicio {idServicio} no disponible.");
                        continue;
                    }

                    // Agregar el costo del servicio al total
                    total += servicio.ValorServicio;

                    // Agregar el servicio a la reserva
                    existingReserva.ReservaServicios.Add(new ReservaServicio
                    {
                        Idservicio = idServicio,
                        Idreserva = existingReserva.Idreserva // Asegúrate de asignar la clave foránea correctamente
                    });
                }
            }

            if (!ModelState.IsValid)
            {
                await CargarDatosReservaAsync();
                return View(reserva);
            }

            existingReserva.ValorTotal = (double)total;
            _context.Update(existingReserva);

            var pago = await _context.Pagos.FindAsync(existingReserva.Idpago);
            if (pago != null)
            {
                pago.CantidadPago = total;
                _context.Update(pago);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Delete
        public async Task<IActionResult>Delete(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.HabitacionReservas)
                .Include(r => r.ReservaServicios)
                .Include(r => r.IdusuarioNavigation)
                .FirstOrDefaultAsync(r => r.Idreserva == id);

            if (reserva == null)
            {
                ViewData["ErrorMessage"] = "No se encontró la reserva.";
                return View("Error"); // Devuelve una vista de error personalizada.
            }

            return View(reserva);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.HabitacionReservas)
                .Include(r => r.ReservaServicios)
                .FirstOrDefaultAsync(r => r.Idreserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            _context.HabitacionReservas.RemoveRange(reserva.HabitacionReservas);
            _context.ReservaServicios.RemoveRange(reserva.ReservaServicios);
            _context.Reservas.Remove(reserva);

            var pago = await _context.Pagos.FindAsync(reserva.Idpago);
            if (pago != null)
            {
                _context.Pagos.Remove(pago);
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
