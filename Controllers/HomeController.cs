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
    // 1. Obtener el JSON del usuario de la sesión
    string? usuarioJson = HttpContext.Session.GetString("Usuario");
    if (string.IsNullOrEmpty(usuarioJson))
    {
        // Si no hay usuario en sesión, redirigir al login
        return RedirectToAction("Index", "Account"); 
    }
    
    // 2. Convertir el JSON a objeto Usuario para obtener nombreUsuario/contraseña
    Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);
    
    // 3. Traer el usuario completo de la BD (se necesita el Id)
    Usuario usuarioCompleto = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);

    List<Actividades> actividadesPendientes = new List<Actividades>();

    if (usuarioCompleto != null)
    {
        // 4. Usar el ID del usuario para traer SOLO sus actividades pendientes
        actividadesPendientes = BD.TraerActividadesPendientes(usuarioCompleto.id); 
    }
    
    // 5. Pasar la lista REAL de actividades pendientes a la vista
    ViewBag.ActividadesPendientes = actividadesPendientes; 



    ViewBag.TareasPendientes = BD.TraerActividadesPendientes(usuarioCompleto.id);
    
    return View("Home");
}
     public IActionResult Estadisticas()
    {
        return View("Actividades");
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
 




public IActionResult JuegoOrdenarPictogramas(int? index, bool? continuar)
{
    int indiceActual;
    const int ID_ACTIVIDAD_PICTOGRAMAS = 6; 
    if (continuar == false)
    {
   indiceActual = index ?? 0; 
    }
    else 
    {
        int correctas = HttpContext.Session.GetInt32("JuegoCorrectas") ?? 0;
indiceActual = correctas;
    }
  
    
    List<int> idsPreguntas;
    
   
    if (indiceActual == 0) 
    {
       
        HttpContext.Session.SetString("JuegoTiempoInicio", DateTime.Now.ToString("o"));
        HttpContext.Session.SetInt32("JuegoCorrectas", 0);
        
        idsPreguntas = BD.TraerIdsPreguntasActividad(ID_ACTIVIDAD_PICTOGRAMAS); 
        
        if (idsPreguntas.Count == 0) return RedirectToAction("Home");
        
        HttpContext.Session.SetString("JuegoIds", System.Text.Json.JsonSerializer.Serialize(idsPreguntas));
        HttpContext.Session.SetInt32("JuegoTotalPreguntas", idsPreguntas.Count);
        HttpContext.Session.SetInt32("JuegoIndiceActual", 0);
    }
    else
    {
        
        string? idsJson = HttpContext.Session.GetString("JuegoIds");
        if (string.IsNullOrEmpty(idsJson)) return RedirectToAction("Home"); 
        
        idsPreguntas = System.Text.Json.JsonSerializer.Deserialize<List<int>>(idsJson)!;
        HttpContext.Session.SetInt32("JuegoIndiceActual", indiceActual);
    }
    
 
    if (indiceActual >= idsPreguntas.Count)
    {
        return RedirectToAction("FinDeJuego");
    }

   
    int idPreguntaActual = idsPreguntas[indiceActual];
    PreguntaPictograma pregunta = BD.TraerPregunta(idPreguntaActual);

   
    if (pregunta == null)
    {
        return RedirectToAction("JuegoOrdenarPictogramas", new { index = indiceActual + 1 });
    }


    HttpContext.Session.SetInt32("JuegoIdPreguntaActual", idPreguntaActual);


    ViewBag.Pregunta = pregunta;
    ViewBag.IndiceActual = indiceActual; 
    
    
    
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
public IActionResult VerificarRespuesta(string opcion)
{
    int idPreguntaActual = HttpContext.Session.GetInt32("JuegoIdPreguntaActual") ?? 0;
    int indiceActual = HttpContext.Session.GetInt32("JuegoIndiceActual") ?? 0;

    if (idPreguntaActual == 0)
    {
        return RedirectToAction("Home");
    }
     
    bool esCorrecta = BD.VerificarRespuestaBD(idPreguntaActual, opcion);
    
    if (esCorrecta) 
    {
        int correctas = HttpContext.Session.GetInt32("JuegoCorrectas") ?? 0;
        HttpContext.Session.SetInt32("JuegoCorrectas", correctas + 1);
        
         idPreguntaActual = correctas + 1; 

    HttpContext.Session.SetInt32("JuegoIdPreguntaActual", idPreguntaActual);
        
        return RedirectToAction("JuegoOrdenarPictogramas", new { index = idPreguntaActual });
    }
    else 
    {
        TempData["MensajeError"] = "¡Incorrecto! Intenta de nuevo.";
        
        return RedirectToAction("JuegoOrdenarPictogramas", new { index = idPreguntaActual });
    }
}
    



public IActionResult FinDeJuego()
{
    DateTime tiempoFin = DateTime.Now;
    string tiempoInicioStr = HttpContext.Session.GetString("JuegoTiempoInicio");
    TimeSpan duracion = TimeSpan.Zero;
    
    if (!string.IsNullOrEmpty(tiempoInicioStr) && DateTime.TryParse(tiempoInicioStr, out DateTime tiempoInicio))
    {
        duracion = tiempoFin - tiempoInicio;
    }

    int correctas = HttpContext.Session.GetInt32("JuegoCorrectas") ?? 0;
    int totalPreguntas = HttpContext.Session.GetInt32("JuegoTotalPreguntas") ?? 10;
    int puntosGanados = correctas * 10;
    
    const int ID_ACTIVIDAD_PICTOGRAMAS = 6; 
    
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
        int puntosActuales = usuarioCompleto.puntos ?? 0;
        puntajeTotalFinal = puntosActuales + puntosGanados;
        BD.ActualizarPuntosUsuario(usuarioCompleto.id, puntajeTotalFinal);
        
      
        int progresoCompleto = 100;
        BD.ActualizarProgresoActividad(usuarioCompleto.id, ID_ACTIVIDAD_PICTOGRAMAS, progresoCompleto);
    }

    // Limpieza de Sesión
    HttpContext.Session.Remove("JuegoTiempoInicio");
    HttpContext.Session.Remove("JuegoCorrectas");
    HttpContext.Session.Remove("JuegoTotalPreguntas");
    HttpContext.Session.Remove("JuegoIds");
    HttpContext.Session.Remove("JuegoIndiceActual");
    HttpContext.Session.Remove("JuegoIdPreguntaActual");

   
    ViewBag.PuntosGanados = puntosGanados;
    ViewBag.PuntajeTotal = puntajeTotalFinal;
    ViewBag.Correctas = correctas;
    ViewBag.TotalPreguntas = totalPreguntas;
    ViewBag.Duracion = duracion.ToString(@"mm\:ss");

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
           
            usuarioActualizado.ActualizarDatosOpcionales(nombre, apellido, fechaNacimiento, telefono, fotoPerfil, nivelApoyo, descripcion);
            BD.ActualizarUsuario(usuarioActualizado);
        }

        return RedirectToAction("Perfil");
    }
}