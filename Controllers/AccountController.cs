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
        return View("Index");
    }
    [HttpPost]
    public IActionResult LogIn2(string UserName, string Contraseña)
    {
        Usuario UsuarioLogin = BD.TraerUNUsuario(UserName, Contraseña);

        string DONDE = "Index";
        ViewBag.Usuario = UsuarioLogin;
        if (ViewBag.Usuario == null)
        {
            DONDE = "Login";
            ViewBag.MensajeLogin = "Usuario o contraseña incorrectos";
        }
        else
        {

            HttpContext.Session.SetString("ID", UsuarioLogin.Id.ToString());
        }

        return View(DONDE);
    }




    [HttpPost]
    public IActionResult Registrarse2(string UserName, string Email, string Contraseña)
    {


        DateTime UltimoInicio = DateTime.Now;

        Usuario UsuarioRegistrar = new Usuario(UserName, Email); //hay que ver bien el tema de que recibe el contructor, que pone en 0 y todo eso 


        ViewBag.SePudo = BD.CrearUnUsuario(UserName,Email,Contraseña);

        if (ViewBag.SePudo)
        {
            HttpContext.Session.SetString("ID", UsuarioRegistrar.Id.ToString());
        }
        else
        {
            ViewBag.Mensaje = "Ya tienes un usuario existente en esta plataforma";

        }
        
        return View("Index");
    }

    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return View("Index");
    }


    public IActionResult Registrarse1()
    {

        return View("Registrar");
    }
    public IActionResult LogIn1()
    {

        return View("Index");
    }

}