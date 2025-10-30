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

        string view = "Index";
        ViewBag.Usuario = UsuarioLogin;
        if (ViewBag.Usuario == null)
        {
            view = "Login";
            ViewBag.MensajeLogin = "Usuario o contraseña incorrectos";
        }
        else
        {

            HttpContext.Session.SetString("ID", UsuarioLogin.Id.ToString());
        }

        return View(view);
    }




    [HttpPost]
    public IActionResult Registrarse2(string UserName, string Contraseña, string contraseña1, string Email, string tipoUser)
    {
string HaciaDondeVa = "Index";
        //no se porque no me toma la clase usuario cuando declaro un objeto de ese tipo

        DateTime UltimoInicio = DateTime.Now;
if(Contraseña == contraseña1)
{
 ViewBag.MensajeContraseña = "Las contraseñas no coinciden";
 HaciaDondeVa = "Registrarse"; 

}
else
{
 Usuario user = new Usuario (UserName, Contraseña, Email, tipoUser);

        ViewBag.SePudo = BD.CrearUsuario(User); //recibe el objeto usuario desde el formulario y lo desgloza dentro de crear usuario.

        if (ViewBag.SePudo)
        {
            HttpContext.Session.SetString("ID", UsuarioRegistrar.Id.ToString());
        }
        else
        {
            ViewBag.Mensaje = "Ya tienes un usuario existente en esta plataforma";

        }   
}
       
        
        return View(HaciaDondeVa);
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