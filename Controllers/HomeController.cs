using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Info360.Models;
using Microsoft.AspNetCore.Http; 
using System; 
using System.Text.Json;        // Para guardar/leer objetos en la Session

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

    public IActionResult Home()
    {
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
  public IActionResult VerMasPerfil(bool Toco)
    {
    

        if(Toco)
        {
        ViewBag.TocoMasPefil = true;
        }
     
           return RedirectToAction("Perfil");
    }
    public IActionResult Perfil()
    {
        string? usuarioJson = HttpContext.Session.GetString("Usuario");
        if (string.IsNullOrEmpty(usuarioJson))
        {
            return RedirectToAction("Index", "Account");
        }
        Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);
        Usuario? usuarioCompleto = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);

        if (usuarioCompleto == null)
        {
            return RedirectToAction("CerrarSesion", "Account");
        }

        ViewBag.usuario = usuarioCompleto;
        ViewBag.Vinculos = BD.ListaVinculos(usuarioCompleto);

        return View("Perfil");
    }

    
    public IActionResult Configuracion()
    {
        string? usuarioJson = HttpContext.Session.GetString("Usuario");
        if (string.IsNullOrEmpty(usuarioJson))
        {
            return RedirectToAction("Index", "Account");
        }
        Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);
        Usuario? usuarioCompleto = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);

        if (usuarioCompleto == null)
        {
            return RedirectToAction("CerrarSesion", "Account");
        }

        ViewBag.usuario = usuarioCompleto; 
        return View("Configuracion"); 
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
           
            
            if (!string.IsNullOrEmpty(nombre))
            {
                usuarioActualizado.nombre = nombre;
            }
            
            if (!string.IsNullOrEmpty(apellido))
            {
                usuarioActualizado.apellido = apellido;
            }

            if (fechaNacimiento != null)
            {
                usuarioActualizado.fechaNacimiento = fechaNacimiento;
            }
            
            if (!string.IsNullOrEmpty(telefono))
            {
                usuarioActualizado.telefono = telefono;
            }

            if (!string.IsNullOrEmpty(fotoPerfil))
            {
                usuarioActualizado.fotoPerfil = fotoPerfil;
            }

            if (nivelApoyo != null)
            {
                usuarioActualizado.nivelApoyo = nivelApoyo;
            }
            
          
            usuarioActualizado.descripcion = descripcion;

      
            
            BD.ActualizarUsuario(usuarioActualizado);
        }

        return RedirectToAction("Perfil");
    }

    [HttpGet]
    public IActionResult JuegoPictogramas()
    {
        // 1. Cargar la lista de preguntas desde la BD
        List<PreguntaPictograma> preguntas = BD.ObtenerPreguntasPictogramas();
        
        // 2. Guardar la lista completa en la Sesión
        string jsonPreguntas = JsonSerializer.Serialize(preguntas);
        HttpContext.Session.SetString("ListaPreguntas", jsonPreguntas);

        // 3. Guardar el índice actual (empezamos en 0)
        HttpContext.Session.SetInt32("PreguntaActualIndex", 0);

        // 4. Obtener la primera pregunta
        PreguntaPictograma primeraPregunta = preguntas[0];

        // 5. ***** CAMBIO AQUÍ: Pasamos los datos al ViewBag *****
        ViewBag.IdPregunta = primeraPregunta.IdPregunta;
        ViewBag.ImagenPictograma = primeraPregunta.ImagenPictograma;
        ViewBag.Opcion1 = primeraPregunta.Opcion1;
        ViewBag.Opcion2 = primeraPregunta.Opcion2;
        ViewBag.Opcion3 = primeraPregunta.Opcion3;
        ViewBag.Opcion4 = primeraPregunta.Opcion4;

        // 6. ***** CAMBIO AQUÍ: Devolvemos la Vista sin modelo *****
        return View();
    }

    [HttpPost]
    public IActionResult VerificarRespuesta(int idPregunta, int opcionSeleccionada)
    {
        // 1. Recuperar la pregunta actual (que está guardada en la lista estática)
        // Usamos el idPregunta solo para asegurarnos de que estamos verificando la correcta
        PreguntaPictograma pregunta = PreguntaPictograma._ListaPreguntas.FirstOrDefault(p => p.IdPregunta == idPregunta);

        if (pregunta == null)
        {
            return Json(new { correcta = false, error = "Pregunta no encontrada" });
        }

        // 2. Verificar si la respuesta es correcta
        bool esCorrecta = (pregunta.RespuestaCorrecta == opcionSeleccionada);

        // 3. Si es incorrecta, buscar el texto de la respuesta correcta para mostrarlo
        string respuestaCorrectaTexto = "";
        if (!esCorrecta)
        {
            switch (pregunta.RespuestaCorrecta)
            {
                case 1: respuestaCorrectaTexto = pregunta.Opcion1; break;
                case 2: respuestaCorrectaTexto = pregunta.Opcion2; break;
                case 3: respuestaCorrectaTexto = pregunta.Opcion3; break;
                case 4: respuestaCorrectaTexto = pregunta.Opcion4; break;
            }
        }

        // 4. Devolver un JSON al JavaScript con el resultado
        return Json(new { 
            correcta = esCorrecta, 
            respuestaCorrectaTexto = respuestaCorrectaTexto 
        });
    }

    [HttpGet]
    public IActionResult SiguientePregunta()
    {
        // 1. Pedirle al modelo que avance el índice y nos dé la siguiente pregunta
        PreguntaPictograma siguientePregunta = PreguntaPictograma.AvanzarSiguientePregunta();

        // 2. Verificar si se terminó el juego
        if (siguientePregunta == null)
        {
            // Fin del juego
            return Json(new { finJuego = true });
        }

        // 3. Si no terminó, devolver la siguiente pregunta como JSON
        return Json(new { 
            finJuego = false,
            idPregunta = siguientePregunta.IdPregunta,
            imagenPictograma = siguientePregunta.ImagenPictograma,
            opcion1 = siguientePregunta.Opcion1,
            opcion2 = siguientePregunta.Opcion2,
            opcion3 = siguientePregunta.Opcion3,
            opcion4 = siguientePregunta.Opcion4
        });
    }
}