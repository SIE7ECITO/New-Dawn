﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewDawn.Models;

namespace NewDawn.Controllers
{
    [Authorize(Roles = "admin,empleado")]
    public class PaquetesController : Controller
    {
        private readonly NewDawnContext _context;

        public PaquetesController(NewDawnContext context)
        {
            _context = context;
        }

        // GET: Paquetes
        public async Task<IActionResult> Index(string nombre, decimal? precioMin, decimal? precioMax, bool? estado)
        {
            var query = _context.Paquetes.AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(p => p.NombrePaquete.Contains(nombre));
            }

            if (precioMin.HasValue)
            {
                query = query.Where(p => p.Precio >= precioMin.Value);
            }

            if (precioMax.HasValue)
            {
                query = query.Where(p => p.Precio <= precioMax.Value);
            }

            if (estado.HasValue)
            {
                query = query.Where(p => p.EstadoPaquete == estado.Value);
            }

            var paquetes = await query.ToListAsync();
            return View(paquetes);
        }

        // GET: Paquetes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var paquete = await _context.Paquetes
                .Include(p => p.PaqueteHabitacions)
                    .ThenInclude(ph => ph.IdhabitacionNavigation)
                .Include(p => p.ServicioPaquetes)
                    .ThenInclude(sp => sp.IdservicioNavigation)
                .FirstOrDefaultAsync(m => m.Idpaquete == id);

            if (paquete == null)
                return NotFound();

            return View(paquete);
        }

        // GET: Paquetes/Create
        public IActionResult Create()
        {
            ViewBag.Habitaciones = _context.Habitacions.ToList();
            ViewBag.Servicios = _context.Servicios.ToList();
            return View();
        }

        // POST: Paquetes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Paquete paquete, List<int> HabitacionesSeleccionadas, List<int> ServiciosSeleccionados)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Habitaciones = _context.Habitacions.ToList();
                ViewBag.Servicios = _context.Servicios.ToList();
                return View(paquete);
            }
            paquete.EstadoPaquete = true;


            _context.Add(paquete);
            await _context.SaveChangesAsync();

            if (HabitacionesSeleccionadas?.Any() == true)
            {
                var paqueteHabitaciones = HabitacionesSeleccionadas.Select(id => new PaqueteHabitacion
                {
                    Idpaquete = paquete.Idpaquete,
                    Idhabitacion = id
                });
                _context.PaqueteHabitacions.AddRange(paqueteHabitaciones);

                // Actualizar EnPaquete a true para las habitaciones seleccionadas
                var habitaciones = _context.Habitacions.Where(h => HabitacionesSeleccionadas.Contains(h.Idhabitacion)).ToList();
                foreach (var habitacion in habitaciones)
                {
                    habitacion.EnPaquete = true;
                    _context.Habitacions.Update(habitacion);
                }
            }

            if (ServiciosSeleccionados?.Any() == true)
            {
                var paqueteServicios = ServiciosSeleccionados.Select(id => new ServicioPaquete
                {
                    Idpaquete = paquete.Idpaquete,
                    Idservicio = id
                });
                _context.ServicioPaquetes.AddRange(paqueteServicios);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Paquetes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var paquete = await _context.Paquetes
                .Include(p => p.PaqueteHabitacions)
                .Include(p => p.ServicioPaquetes)
                .FirstOrDefaultAsync(p => p.Idpaquete == id);

            if (paquete == null)
                return NotFound();

            ViewBag.Habitaciones = _context.Habitacions.ToList();
            ViewBag.Servicios = _context.Servicios.ToList();
            ViewBag.HabitacionesSeleccionadas = paquete.PaqueteHabitacions.Select(ph => ph.Idhabitacion).ToList();
            ViewBag.ServiciosSeleccionados = paquete.ServicioPaquetes.Select(sp => sp.Idservicio).ToList();

            return View(paquete);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Paquete paquete, List<int> HabitacionesSeleccionadas, List<int> ServiciosSeleccionados)
        {
            if (id != paquete.Idpaquete)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Habitaciones = _context.Habitacions.ToList();
                ViewBag.Servicios = _context.Servicios.ToList();
                ViewBag.HabitacionesSeleccionadas = HabitacionesSeleccionadas;
                ViewBag.ServiciosSeleccionados = ServiciosSeleccionados;
                return View(paquete);
            }

            var paqueteExistente = await _context.Paquetes
                .Include(p => p.PaqueteHabitacions)
                .Include(p => p.ServicioPaquetes)
                .FirstOrDefaultAsync(p => p.Idpaquete == id);

            if (paqueteExistente == null)
                return NotFound();

            // Actualizar datos básicos
            paqueteExistente.NombrePaquete = paquete.NombrePaquete;
            paqueteExistente.Descripcion = paquete.Descripcion;
            paqueteExistente.Precio = paquete.Precio;
            paqueteExistente.EstadoPaquete = paquete.EstadoPaquete;

            // Guardar IDs de habitaciones antes de eliminar relaciones
            var habitacionesAntes = paqueteExistente.PaqueteHabitacions.Select(ph => ph.Idhabitacion).ToList();

            // Eliminar relaciones previas
            _context.PaqueteHabitacions.RemoveRange(paqueteExistente.PaqueteHabitacions);
            _context.ServicioPaquetes.RemoveRange(paqueteExistente.ServicioPaquetes);
            await _context.SaveChangesAsync();

            // Agregar nuevas relaciones
            if (HabitacionesSeleccionadas?.Any() == true)
            {
                var paqueteHabitaciones = HabitacionesSeleccionadas.Select(idHab => new PaqueteHabitacion
                {
                    Idpaquete = paquete.Idpaquete,
                    Idhabitacion = idHab
                });
                _context.PaqueteHabitacions.AddRange(paqueteHabitaciones);

                // Marcar nuevas habitaciones como EnPaquete = true
                var habitacionesNuevas = _context.Habitacions.Where(h => HabitacionesSeleccionadas.Contains(h.Idhabitacion)).ToList();
                foreach (var hab in habitacionesNuevas)
                {
                    hab.EnPaquete = true;
                    _context.Habitacions.Update(hab);
                }
            }

            if (ServiciosSeleccionados?.Any() == true)
            {
                var paqueteServicios = ServiciosSeleccionados.Select(idServ => new ServicioPaquete
                {
                    Idpaquete = paquete.Idpaquete,
                    Idservicio = idServ
                });
                _context.ServicioPaquetes.AddRange(paqueteServicios);
            }

            // Verificar si las habitaciones removidas siguen en otros paquetes
            var habitacionesRemovidas = habitacionesAntes.Except(HabitacionesSeleccionadas ?? new List<int>()).ToList();
            foreach (var idHab in habitacionesRemovidas)
            {
                bool sigueEnPaquete = _context.PaqueteHabitacions.Any(ph => ph.Idhabitacion == idHab);
                if (!sigueEnPaquete)
                {
                    var habitacion = await _context.Habitacions.FindAsync(idHab);
                    if (habitacion != null)
                    {
                        habitacion.EnPaquete = false;
                        _context.Habitacions.Update(habitacion);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Paquetes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var paquete = await _context.Paquetes.FindAsync(id);
            if (paquete == null)
                return NotFound();

            return View(paquete);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var paquete = await _context.Paquetes
                .Include(p => p.PaqueteHabitacions)
                .Include(p => p.ServicioPaquetes)
                .FirstOrDefaultAsync(p => p.Idpaquete == id);

            if (paquete == null)
                return NotFound();

            // 🔴 Eliminar primero las relaciones antes de borrar el paquete
            if (paquete.PaqueteHabitacions.Any())
                _context.PaqueteHabitacions.RemoveRange(paquete.PaqueteHabitacions);

            if (paquete.ServicioPaquetes.Any())
                _context.ServicioPaquetes.RemoveRange(paquete.ServicioPaquetes);

            // 🔴 Ahora eliminamos el paquete
            _context.Paquetes.Remove(paquete);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirige automáticamente después de eliminar
        }

    }
}