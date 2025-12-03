using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Info360.Models;
using Microsoft.AspNetCore.Http;

namespace Info360.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        string? usuarioJson = HttpContext.Session.GetString("Usuario");
        Usuario user; 

        if (string.IsNullOrEmpty(usuarioJson))
        {
            user = new Usuario(); 
        }
        else
        {
            user = Objeto.StringToObject<Usuario>(usuarioJson);
        }

        return View("Login");
    }

    [HttpPost]
    public IActionResult LogIn2(string userName, string contraseña)
    {
        Usuario? UsuarioLogin = BD.TraerUNUsuario(userName, contraseña);
        string view = "Login"; 
        
        if (UsuarioLogin == null)
        {
            ViewBag.MensajeLogin = "Usuario o contraseña incorrectos";
        }
        else
        {
            string usuarioJson = Objeto.ObjectToString(UsuarioLogin);
            HttpContext.Session.SetString("Usuario", usuarioJson);
            ViewBag.tipoUsuario = UsuarioLogin.tipoUsuario;
            return RedirectToAction("Home", "Home"); 
        }

        return View(view);
    }

    [HttpPost]
    public IActionResult Registrarse2(string userName, string contraseña, string contraseña1, string email, string tipoUser)
    {
        string HaciaDondeVa = "Index"; 
        
        if(contraseña != contraseña1)
        {
            ViewBag.MensajeContraseña = "Las contraseñas no coinciden";
            HaciaDondeVa = "Registrarse"; 
        }
        else
        {
            Usuario user = new Usuario(userName, contraseña, email, tipoUser);
            ViewBag.SePudo = BD.CrearUsuario(user); 

            if (ViewBag.SePudo)
            {
                string usuarioJson = Objeto.ObjectToString(user);
                HttpContext.Session.SetString("Usuario", usuarioJson);
                 ViewBag.tipoUsuario = user.tipoUsuario;
                return RedirectToAction("Home", "Home"); 
            }
            else
            {
                ViewBag.Mensaje = "Ya tienes un usuario existente en esta plataforma";
                HaciaDondeVa = "Registrarse"; 
            }   
        }
        
        return View(HaciaDondeVa);
    }

    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return View("Login");
    }

    public IActionResult Registrarse1()
    {
        return View("Registrarse");
    }

    public IActionResult LogIn1()
    {
        return View("Login");
    }
}