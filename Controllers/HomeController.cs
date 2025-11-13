using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Info360.Models;
using Microsoft.AspNetCore.Http; 
using System; 
using System.Text.Json;         

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
         return View("Index");
    }

    public IActionResult Home()
    {
        Actividades Act = new Actividades();
        ViewBag.ActividadesPendientes = Act.ListActividadesPendientes;
        return View("Home");
    }
    
    public IActionResult Actividades()
    {
        return View("Actividades");
    }

    public IActionResult Juegos()
    {
        return View("Juegos");
    }
    public IActionResult VerJuegos(){
        return View("Juegos");
    }
    public IActionResult JuegoPictogramas(){
        return View("previewJuego");
    }
    public IActionResult jugarOrdenarPictograma(){
        PreguntaPictograma preg = new PreguntaPictograma();
        ViewBag.Pregunta =  preg.AvanzarSiguientePregunta();
        ViewBag.PreguntaElegida = null;
        return View("JuegoOrdenarPictogramas");
    }
    public IActionResult resolverQueEs(int idPregunta, string opcion)
{
    PreguntaPictograma preg = new PreguntaPictograma();
    
    // Buscar la pregunta por su ID real, NO por índice
    var pregunta = preg._ListaPreguntas.FirstOrDefault(p => p.IdPregunta == idPregunta);
    if (pregunta == null)
    {
        return Content("La pregunta no existe.");
    }

    if (pregunta.RespuestaCorrecta == opcion)
    {
        ViewBag.sala = "Correcto";
    }
    else
    {
        ViewBag.Pregunta = pregunta;
        ViewBag.PreguntaElegida = null;
        ViewBag.intentos++;

        ViewBag.opcion1 = pregunta.Opcion1;
        ViewBag.opcion2 = pregunta.Opcion2;
        ViewBag.opcion3 = pregunta.Opcion3;
        ViewBag.opcion4 = pregunta.Opcion4;

        ViewBag.sala = "JuegoOrdenarPictogramas";
    }

    return View(ViewBag.sala);
}

 public IActionResult CerrarSesion(){
        return View("Index");
    }
     public IActionResult Nosotros(){
        return View("Nosotros");
    }
       public IActionResult Ayuda(){
        return View("Ayuda");
    }
    


    public IActionResult Perfil(bool verMas = false)
    {
        string? usuarioJson = HttpContext.Session.GetString("Usuario");
        if (string.IsNullOrEmpty(usuarioJson))
        {
            return RedirectToAction("Index", "Account");
        }
        Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);
        
      
        Usuario usuarioCompleto = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);

        if (usuarioCompleto == null)
        {
            return RedirectToAction("CerrarSesion", "Account");
        }

        ViewBag.usuario = usuarioCompleto;
        ViewBag.Vinculos = BD.ListaVinculos(usuarioCompleto);
        ViewBag.TocoMasPefil = verMas; 

        // --- CORRECCIÓN DE LÓGICA ---
        // Cambiado de "tutor" a "responsable" para que coincida con tu Vista
        if(usuarioCompleto.tipoUsuario == "responsable")
        {
            ViewBag.UsuariosDisponibles = BD.ListaUsuariosDisponibles(usuarioCompleto.id);
        }

        return View("Perfil");
    }

    
    public IActionResult VerMasPerfil()
    {
        return RedirectToAction("Perfil", new { verMas = true });
    }
    
    public IActionResult Configuracion()
    {
        string? usuarioJson = HttpContext.Session.GetString("Usuario");
        if (string.IsNullOrEmpty(usuarioJson))
        {
            return RedirectToAction("Index", "Account");
        }
        Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);
       
        Usuario usuarioCompleto = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);

        if (usuarioCompleto == null)
        {
            return RedirectToAction("CerrarSesion", "Account");
        }

        ViewBag.usuario = usuarioCompleto; 
        return View("Configuracion"); 
    }
    
    [HttpPost]
    public IActionResult AgregarVinculo(int idTutor, int idPerteneciente, string parentesco)
    {
        BD.AgregarVinculoBD(idTutor, idPerteneciente, parentesco);
        return RedirectToAction("Perfil");
    }

    [HttpPost]
    public IActionResult EliminarVinculo(int idTutor, int idPerteneciente)
    {
        BD.EliminarVinculoBD(idTutor, idPerteneciente);
        return RedirectToAction("Perfil");
    }
    
    [HttpPost]
    public IActionResult GuardarConfiguracion(string nombre, string apellido, DateTime? fechaNacimiento, string telefono, string fotoPerfil, int? nivelApoyo, string descripcion)
    {
        string? usuarioJson = HttpContext.Session.GetString("Usuario");
        if (string.IsNullOrEmpty(usuarioJson))
        {
            return RedirectToAction("Index", "Account");
        }
        Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);
        
  
        Usuario usuarioActualizado = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);

        if (usuarioActualizado != null)
        {
            // Llamamos al método en el Modelo para actualizar los datos
            usuarioActualizado.ActualizarDatosOpcionales(nombre, apellido, fechaNacimiento, telefono, fotoPerfil, nivelApoyo, descripcion);
            BD.ActualizarUsuario(usuarioActualizado);
        }

        return RedirectToAction("Perfil");
    }
}