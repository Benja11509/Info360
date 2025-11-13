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
 
// --- ACCIÓN 'JuegoOrdenarPictogramas' CORREGIDA ---
    public IActionResult JuegoOrdenarPictogramas(int? id)
    {
         int idParaCargar; 
        if(id >= 0)
        {
            idParaCargar = id.Value ;
        }
        else
        {
// Si no viene un ID, empezamos por la pregunta 1
         idParaCargar = 1; 

        }
        
        PreguntaPictograma pregunta = BD.TraerPregunta(idParaCargar);
        
        if (pregunta == null)
        {
            // Si no hay más preguntas, volvemos a la página de Actividades
            return RedirectToAction("Actividades");
        }

        ViewBag.Pregunta = pregunta;

        // Creamos la lista de opciones y las desordenamos
        List<string> opciones = new List<string>
        {
            pregunta.RespuestaCorrecta,
            pregunta.Opcion1,
            pregunta.Opcion2,
            pregunta.Opcion3, 
            pregunta.Opcion4
        };
        ViewBag.Opciones = opciones.OrderBy(x => Guid.NewGuid()).ToList();
        
        // Pasamos el mensaje de error (si existe) a la vista
        if (TempData["MensajeError"] != null)
        {
            ViewBag.Mensaje = TempData["MensajeError"].ToString();
        }

        return View("JuegoOrdenarPictogramas");
    }

    // --- ACCIÓN 'VerificarRespuesta' CORREGIDA ---
    [HttpPost]
    public IActionResult VerificarRespuesta(string opcion, int idPregunta)
    {
        bool esCorrecta = BD.VerificarRespuestaBD(idPregunta, opcion);

        if (esCorrecta)
        {
            // (Lógica simple, asumimos que la próxima pregunta es la siguiente ID)
            int proximaPreguntaId = idPregunta + 1; 
            
            // (Aquí deberías agregar lógica para sumar puntos al usuario)
            
            return RedirectToAction("JuegoOrdenarPictogramas", new { id = proximaPreguntaId });
        }
        else
        {
            // Usamos TempData para que el mensaje sobreviva a la redirección
            TempData["MensajeError"] = "¡Incorrecto! Intenta de nuevo.";
            
            // Redirigimos de vuelta a la MISMA pregunta
            return RedirectToAction("JuegoOrdenarPictogramas", new { id = idPregunta });
        }
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