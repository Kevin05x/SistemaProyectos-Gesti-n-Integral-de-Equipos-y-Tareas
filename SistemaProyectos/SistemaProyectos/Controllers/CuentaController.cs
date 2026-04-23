using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaProyectos.Data;
using SistemaProyectos.Models;
using SistemaProyectos.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaProyectos.Controllers
{

    [AllowAnonymous]
    public class CuentaController : Controller
    {
        private readonly AppDbContext _context;

        public CuentaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = _context.Usuario
                .Include(u => u.DetalleEquipos)
                    .ThenInclude(de => de.Equipo)
                .FirstOrDefault(u => u.NombreUsuario == model.NombreUsuario);

            if (usuario == null)
            {
                ViewData["Mensaje"] = "Usuario o contraseña incorrectos";
                return View(model);
            }

            // VALIDACIÓN DE LA CONTRASEÑA CON HASH
            var passwordHasher = new PasswordHasher<Usuario>();
            var result = passwordHasher.VerifyHashedPassword(usuario, usuario.Contrasena, model.Contrasena);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewData["Mensaje"] = "Usuario o contraseña incorrectos";
                return View(model);
            }

            else
            {
                // Crear claims
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol),
            };

                foreach (var detalle in usuario.DetalleEquipos)
                {
                    claims.Add(new Claim("EquipoID", detalle.EquipoID.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                await HttpContext.SignInAsync("CookieAuth",
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = false
                    });

            }

            if (usuario.Rol== "Administrador")
            {
                return RedirectToAction("Index", "Home");
            }

            else if (usuario.Rol == "Empleado")
            {
                return RedirectToAction("HomeEmpleado", "Home");
            }
            else
            {
                await HttpContext.SignOutAsync("CookieAuth");
                ViewData["Mensaje"] = "El rol del usuario no es válido.";
                return View(model);
            }
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Cuenta");
        }

    }
}
