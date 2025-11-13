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
 
public IActionResult JuegoOrdenarPictogramas(int? id)
    {
        int idParaCargar = id ?? 1; 

        // ---- 1. INICIO DEL TRACKING ----
        // Si es la primera pregunta (id=1), reiniciamos los contadores del juego
        if (idParaCargar == 1)
        {
            // Guardamos el momento exacto en que empezó
            // Usamos "o" (Round-trip) para un formato de fecha preciso
            HttpContext.Session.SetString("JuegoTiempoInicio", DateTime.Now.ToString("o"));
            
            // Reiniciamos el contador de respuestas correctas
            HttpContext.Session.SetInt32("JuegoCorrectas", 0);
            
            // Contamos y guardamos el total de preguntas (usando la nueva función de BD.cs)
            int totalPreguntas = BD.GetTotalPreguntas(); // Necesitaremos crear esta función
            HttpContext.Session.SetInt32("JuegoTotalPreguntas", totalPreguntas);
        }
        // ---- FIN DEL TRACKING ----

        PreguntaPictograma pregunta = BD.TraerPregunta(idParaCargar);
        
        if (pregunta == null)
        {
            // Si no hay más preguntas (ej. completaste la 10 y pide la 11), vamos al final
            return RedirectToAction("FinDeJuego");
        }
    
        ViewBag.Pregunta = pregunta;

        List<string> opciones = new List<string>
        {
            pregunta.Opcion1,
            pregunta.Opcion2,
            pregunta.Opcion3, 
            pregunta.Opcion4
        };
        ViewBag.Opciones = opciones.OrderBy(x => Guid.NewGuid()).ToList();
        
        if (TempData["MensajeError"] != null)
        {
            ViewBag.Mensaje = TempData["MensajeError"].ToString();
        }

        return View("JuegoOrdenarPictogramas");
    }

    [HttpPost]
    public IActionResult VerificarRespuesta(string opcion, int idPregunta)
    {
        bool esCorrecta = BD.VerificarRespuestaBD(idPregunta, opcion);

        if (esCorrecta)
        {
            // ---- 2. SUMAR RESPUESTA CORRECTA ----
            // Traemos el contador de la Sesión, le sumamos 1 y lo volvemos a guardar
            int correctas = HttpContext.Session.GetInt32("JuegoCorrectas") ?? 0;
            HttpContext.Session.SetInt32("JuegoCorrectas", correctas + 1);
            
            int proximaPreguntaId = idPregunta + 1; 
            return RedirectToAction("JuegoOrdenarPictogramas", new { id = proximaPreguntaId });
        }
        else
        {
            TempData["MensajeError"] = "¡Incorrecto! Intenta de nuevo.";
            return RedirectToAction("JuegoOrdenarPictogramas", new { id = idPregunta });
        }
    }

    
    public IActionResult FinDeJuego()
    {
        // ---- 3. CÁLCULO DE RESULTADOS ----

        // --- A. Calcular Duración ---
        DateTime tiempoFin = DateTime.Now;
        string tiempoInicioStr = HttpContext.Session.GetString("JuegoTiempoInicio");
        TimeSpan duracion = TimeSpan.Zero; // 0 por defecto
        
        // Solo calculamos si tenemos un tiempo de inicio guardado
        if (!string.IsNullOrEmpty(tiempoInicioStr) && DateTime.TryParse(tiempoInicioStr, out DateTime tiempoInicio))
        {
            duracion = tiempoFin - tiempoInicio;
        }

        // --- B. Obtener Respuestas ---
        int correctas = HttpContext.Session.GetInt32("JuegoCorrectas") ?? 0;
        int totalPreguntas = HttpContext.Session.GetInt32("JuegoTotalPreguntas") ?? 10; // 10 por si falla

        // --- C. Asignar Puntos ---
        int puntosGanados = correctas * 10; // 10 puntos por respuesta correcta
        
        string? usuarioJson = HttpContext.Session.GetString("Usuario");
        if (string.IsNullOrEmpty(usuarioJson))
        {
            return RedirectToAction("Index", "Account");
        }
        Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);
        Usuario usuarioCompleto = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);

        int puntajeTotalFinal = 0;
        if (usuarioCompleto != null)
        {
            // MANEJO DE INT? (PUNTOS NULOS)
            // 1. Obtenemos los puntos actuales, o 0 si es null.
            int puntosActuales = usuarioCompleto.puntos ?? 0;
            
            // 2. Sumamos los nuevos puntos.
            puntajeTotalFinal = puntosActuales + puntosGanados;
            
            // 3. Guardamos en la BD (Necesitaremos crear ActualizarPuntosUsuario)
            BD.ActualizarPuntosUsuario(usuarioCompleto.id, puntajeTotalFinal);
        }

        // --- D. Limpiar Sesión ---
        // Borramos los datos del juego para que no interfieran la próxima vez
        HttpContext.Session.Remove("JuegoTiempoInicio");
        HttpContext.Session.Remove("JuegoCorrectas");
        HttpContext.Session.Remove("JuegoTotalPreguntas");

        // --- E. Enviar Datos a la Vista ---
        ViewBag.PuntosGanados = puntosGanados;
        ViewBag.PuntajeTotal = puntajeTotalFinal;
        ViewBag.Correctas = correctas;
        ViewBag.TotalPreguntas = totalPreguntas;
        ViewBag.Duracion = duracion.ToString(@"mm\:ss"); // Formato "02:30" (minutos:segundos)

        return View("FinDeJuego"); 
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