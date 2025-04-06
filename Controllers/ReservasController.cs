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
            ViewData["Idhabitacion"] = new SelectList(_context.Habitacions, "Idhabitacion", "Idhabitacion");
            ViewData["Idpago"] = new SelectList(_context.Pagos, "Idpago", "Idpago");
            ViewData["Idpaquete"] = new SelectList(_context.Paquetes, "Idpaquete", "Idpaquete");
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario");
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idreserva,Idusuario,Idhabitacion,Idpaquete,Idpago,FechaReserva,FechaComienzo,FechaFin,ValorTotal,EstadoReserva")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idhabitacion"] = new SelectList(_context.Habitacions, "Idhabitacion", "Idhabitacion", reserva.Idhabitacion);
            ViewData["Idpago"] = new SelectList(_context.Pagos, "Idpago", "Idpago", reserva.Idpago);
            ViewData["Idpaquete"] = new SelectList(_context.Paquetes, "Idpaquete", "Idpaquete", reserva.Idpaquete);
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario", reserva.Idusuario);
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
