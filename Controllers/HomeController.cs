using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Info360.Models;

namespace Info360.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

      public IActionResult Index()
   {
      return RedirectToAction("Index", "Account");
   }
      public IActionResult Home(){
         return View("Index");
      }

      public IActionResult Perfil()
    {
        string? usuarioJson = HttpContext.Session.GetString("Usuario");
        
        if (string.IsNullOrEmpty(usuarioJson))
        {
            return RedirectToAction("Index", "Account"); // Si no hay sesión, va al Login
        }

        Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);

        Usuario? usuarioCompleto = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);

        if (usuarioCompleto == null)
        {
            return RedirectToAction("CerrarSesion", "Account"); // Si el usuario no existe, cierra sesión
        }

        ViewBag.usuario = usuarioCompleto;
        ViewBag.Vinculos = BD.ListaVinculos(usuarioCompleto);

        return View("Perfil");
    }
}
