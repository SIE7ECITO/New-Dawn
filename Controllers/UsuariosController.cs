using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using NewDawn.Models;

namespace NewDawn.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly NewDawnContext _context;

        public UsuariosController(NewDawnContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index(string searchCc)
        {
            var usuarios = from u in _context.Usuarios.Include(u => u.IdrolNavigation)
                           select u;

            if (!string.IsNullOrEmpty(searchCc))
            {
                usuarios = usuarios.Where(u => u.Ccusuario.ToString().Contains(searchCc));
            }

            ViewData["CurrentFilter"] = searchCc;

            return View(await usuarios.ToListAsync());
        }



        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idusuario,Ccusuario,NombreUsuario,Apellido,NumeroTelUsuario,Correo,ContraseñaUsuario,Idrol")] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Error: " + modelError.ErrorMessage);
                }
                ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol", usuario.Idrol);
                return View(usuario);
            }

            // 🔹 Buscar el rol en la base de datos y asignarlo a IdrolNavigation
            usuario.IdrolNavigation = await _context.Rols.FindAsync(usuario.Idrol);
            if (usuario.IdrolNavigation == null)
            {
                ModelState.AddModelError("Idrol", "El rol seleccionado no es válido.");
                ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol", usuario.Idrol);
                return View(usuario);
            }

            // 🔹 Verificar que la contraseña no esté vacía (pero sin encriptarla)
            if (string.IsNullOrWhiteSpace(usuario.ContraseñaUsuario))
            {
                ModelState.AddModelError("ContraseñaUsuario", "La contraseña es obligatoria.");
                ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol", usuario.Idrol);
                return View(usuario);
            }

            usuario.EstadoUsuario = true;
            // 🔹 Guardar usuario en la base de datos sin encriptar la contraseña
            _context.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdrolNavigation) // Incluir la relación con el rol
                .FirstOrDefaultAsync(m => m.Idusuario == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }





        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", usuario.Idrol);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idusuario,Ccusuario,NombreUsuario,Apellido,NumeroTelUsuario,Correo,ContraseñaUsuario,EstadoUsuario,Idrol")] Usuario usuario)
        {
            if (id != usuario.Idusuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Idusuario))
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
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", usuario.Idrol);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idusuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Idusuario == id);
        }


        // GET: Usuarios/Register
        public IActionResult Register()
        {
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol");
            return View();
        }

        // POST: Usuarios/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Ccusuario,NombreUsuario,Apellido,NumeroTelUsuario,Correo,ContraseñaUsuario,EstadoUsuario,Idrol")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.ContraseñaUsuario = BCrypt.Net.BCrypt.HashPassword(usuario.ContraseñaUsuario);
                usuario.EstadoUsuario = true;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol", usuario.Idrol);
            return View(usuario);
        }
        // GET: Usuarios/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Usuarios/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contraseña)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contraseña))
            {
                ViewData["ErrorMessage"] = "Debe completar todos los campos.";
                return View();
            }

            var usuario = await _context.Usuarios.Include(u => u.IdrolNavigation)
                                                 .FirstOrDefaultAsync(u => u.Correo == correo);

            // Verificar que el usuario existe y que la contraseña coincide
            if (usuario == null || usuario.ContraseñaUsuario != contraseña)
            {
                ViewData["ErrorMessage"] = "Correo o contraseña incorrectos.";
                return View();
            }

            // Crear Claims para el usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Idusuario.ToString()), // ID del Usuario
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),                  // Nombre del Usuario
                new Claim(ClaimTypes.Email, usuario.Correo),                        // Correo del Usuario
                new Claim(ClaimTypes.Role, usuario.IdrolNavigation.NombreRol)       // Rol del usuario
             };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Autenticar al usuario
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirigir al home o dashboard
            return RedirectToAction("Index", "Home");
        }
        // GET: Usuarios/Logout
        // POST: Usuarios/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
    // Cerrar la sesión del usuario
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    // Redirigir al login
    return RedirectToAction("Login", "Usuarios");
    }

        // GET: Usuarios/RegisterUser
        public IActionResult RegisterUser()
        {
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol");
            return View();
        }

        // POST: Usuarios/RegisterUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser([Bind("Ccusuario,NombreUsuario,Apellido,Correo,ContraseñaUsuario,Idrol")] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol", usuario.Idrol);
                return View(usuario);
            }

            // Validar si el correo ya está registrado
            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == usuario.Correo);
            if (usuarioExistente != null)
            {
                ModelState.AddModelError("Correo", "El correo ya está registrado.");
                ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol", usuario.Idrol);
                return View(usuario);
            }

            // Validar y encriptar la contraseña
            if (string.IsNullOrWhiteSpace(usuario.ContraseñaUsuario))
            {
                ModelState.AddModelError("ContraseñaUsuario", "La contraseña es obligatoria.");
                ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol", usuario.Idrol);
                return View(usuario);
            }
            usuario.ContraseñaUsuario = BCrypt.Net.BCrypt.HashPassword(usuario.ContraseñaUsuario);

            // Estado por defecto: Activo
            usuario.EstadoUsuario = true;

            try
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Usuarios");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");
                ModelState.AddModelError("", "Ocurrió un error al registrar el usuario.");
            }

            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "NombreRol", usuario.Idrol);
            return View(usuario);
        }



        // GET: Mostrar la vista de recuperación
        public IActionResult Recuperar()
        {
            return View();
        }

        // POST: Procesar recuperación de contraseña
        [HttpPost]
        public async Task<IActionResult> Recuperar(string Correo)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == Correo);
            if (usuario == null)
            {
                ViewData["Error"] = "El correo no está registrado.";
                return View();
            }

            // 🔹 Simular el código de recuperación
            Random rnd = new();
            int codigoRecuperacion = rnd.Next(100000, 999999); // Genera un código de 6 dígitos

            // 🔹 Guardar el código en la sesión
            HttpContext.Session.SetInt32("CodigoRecuperacion", codigoRecuperacion);
            HttpContext.Session.SetString("CorreoRecuperacion", Correo);

            // 🔹 Simular "envío" mostrando el código en una alerta
            TempData["Codigo"] = codigoRecuperacion;

            return RedirectToAction("VerificarCodigo");
        }

        // GET: Vista para ingresar el código
        public IActionResult VerificarCodigo()
        {
            if (!TempData.ContainsKey("Codigo"))
            {
                return RedirectToAction("Recuperar");
            }

            ViewData["Codigo"] = TempData["Codigo"]; // Mostrar el código en la alerta
            return View();
        }

        // POST: Verificar código y restablecer contraseña
        [HttpPost]
        public async Task<IActionResult> VerificarCodigo(int CodigoIngresado)
        {
            int? codigoGuardado = HttpContext.Session.GetInt32("CodigoRecuperacion");
            string correo = HttpContext.Session.GetString("CorreoRecuperacion");

            if (codigoGuardado == null || codigoGuardado != CodigoIngresado)
            {
                ViewData["Error"] = "Código incorrecto.";
                return View();
            }

            return RedirectToAction("Restablecer");
        }

        // GET: Vista para restablecer contraseña
        public IActionResult Restablecer()
        {
            return View();
        }

        // POST: Guardar nueva contraseña
        [HttpPost]
        public async Task<IActionResult> Restablecer(string NuevaContraseña)
        {
            string correo = HttpContext.Session.GetString("CorreoRecuperacion");

            if (string.IsNullOrEmpty(correo))
            {
                return RedirectToAction("Recuperar");
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
            {
                return RedirectToAction("Recuperar");
            }

            usuario.ContraseñaUsuario = NuevaContraseña;
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }


    }
}