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
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly NewDawnContext _context;

        public RolesController(NewDawnContext context)
        {
            _context = context;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rols.ToListAsync());
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.Rols
                .FirstOrDefaultAsync(m => m.Idrol == id);
            if (rol == null)
            {
                return NotFound();
            }

            return View(rol);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            var Permisos = _context.Permisos.ToList();
            ViewBag.Permisos = Permisos;

            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idrol,NombreRol")] Rol rol, int[] Permitidos)
        {
            var Permisos = await _context.Permisos.ToListAsync();
            ViewBag.Permisos = Permisos;

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error: El rol no pudo ser creado.");
                return View(rol);
            }
            rol.EstadoRol = true;
            // Agregar el rol a la base de datos
            _context.Add(rol);
            await _context.SaveChangesAsync();

            // Si hay permisos seleccionados, los guardamos
            if (Permitidos != null && Permitidos.Length > 0)
            {
                foreach (var idPermiso in Permitidos)
                {
                    var rolPermiso = new RolPermiso
                    {
                        Idrol = rol.Idrol,
                        Idpermisos = idPermiso
                    };
                    _context.Add(rolPermiso);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.Rols
                .Include(r => r.RolPermisos) // Cargar los permisos actuales del rol
                .FirstOrDefaultAsync(r => r.Idrol == id);

            if (rol == null)
            {
                return NotFound();
            }

            ViewBag.Permisos = await _context.Permisos.ToListAsync();
            ViewBag.Permitidos = rol.RolPermisos.Select(rp => rp.Idpermisos).ToList(); // Permisos actuales del rol

            return View(rol);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Rol rol, int[] Permitidos)
        {
            if (id != rol.Idrol)
            {
                return NotFound();
            }

            ViewBag.Permisos = await _context.Permisos.ToListAsync();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error: No se pudo actualizar el rol.");
                return View(rol);
            }

            try
            {
                // Actualizar el rol
                _context.Update(rol);
                await _context.SaveChangesAsync();

                // Actualizar permisos del rol
                var permisosActuales = _context.RolPermisos.Where(rp => rp.Idrol == rol.Idrol);
                _context.RolPermisos.RemoveRange(permisosActuales); // Eliminar permisos antiguos

                if (Permitidos != null && Permitidos.Length > 0)
                {
                    var nuevosPermisos = Permitidos.Select(idPermiso => new RolPermiso
                    {
                        Idrol = rol.Idrol,
                        Idpermisos = idPermiso
                    }).ToList();

                    _context.RolPermisos.AddRange(nuevosPermisos);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolExists(rol.Idrol))
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

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var rol = await _context.Rols.FirstOrDefaultAsync(m => m.Idrol == id);
            if (rol == null)
                return NotFound();

            return View(rol);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rol = await _context.Rols.FindAsync(id);
            if (rol == null)
                return NotFound();

            try
            {
                _context.Rols.Remove(rol);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el rol porque está asociado a uno o más usuarios. Primero desvincúlelo de los usuarios.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // Método auxiliar para verificar si el rol existe
        private bool RolExists(int id)
        {
            return _context.Rols.Any(e => e.Idrol == id);
        }

    }
}
