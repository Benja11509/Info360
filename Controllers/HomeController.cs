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
        
        string? usuarioJson = HttpContext.Session.GetString("Usuario");
        if (string.IsNullOrEmpty(usuarioJson))
        {
            
            return RedirectToAction("Index", "Account"); 
        }
        
        // INICIO: LÓGICA AÑADIDA PARA EL TIEMPO EN PANTALLA (INICIO)
        if (HttpContext.Session.GetString("TiempoInicioActividad") == null) 
        {
            HttpContext.Session.SetString("TiempoInicioActividad", DateTime.UtcNow.ToString("o"));
        }
        // FIN: LÓGICA AÑADIDA

        Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);
        
        Usuario usuarioCompleto = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);
ViewBag.usuario = usuarioCompleto;
        List<Actividades> actividadesPendientes = new List<Actividades>();

        if (usuarioCompleto != null)
        {
            actividadesPendientes = BD.TraerActividadesPendientes(usuarioCompleto.id); 
        
        // ====================================================================
        // CORRECCIÓN CS0029: Asignamos a DateTime y luego convertimos a segundos.
        // ====================================================================
        DateTime tiempoAcumuladoBD = BD.TraerTiempoEnPantallaTotal(usuarioCompleto.id);
        
        // Restamos el valor mínimo de SQL (1/1/1900) para obtener la DURACIÓN REAL (TimeSpan)
        TimeSpan duracionAcumulada = tiempoAcumuladoBD.Subtract(new DateTime(1900, 1, 1));
        
        // Ahora sí, convertimos la duración (TimeSpan) a segundos enteros (int)
        int totalSegundosAcumulado = (int)duracionAcumulada.TotalSeconds;

        ViewBag.TiempoEnPantallaTotal = totalSegundosAcumulado;
        }
        
        ViewBag.ActividadesPendientes = actividadesPendientes; 
        ViewBag.TareasPendientes = BD.TraerActividadesPendientes(usuarioCompleto.id);
        
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
 ViewBag.BarraProgreso = indiceActual *10;

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
        BD.ActualizarProgresoActividad(usuarioCompleto.id, ID_ACTIVIDAD_PICTOGRAMAS, correctas);
        BD.RegistrarTiempoDiario(usuarioCompleto.id, duracion);

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
    


    public IActionResult Perfil(bool verMas = false) //La varible ver mas representa que 
    {
        string? usuarioJson = HttpContext.Session.GetString("Usuario"); // Crear el usuario session
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
[HttpPost]
    public JsonResult RegistrarTiempoSesion(long tiempoFinMs)
    {
        string? usuarioJson = HttpContext.Session.GetString("Usuario");
        string? tiempoInicioStr = HttpContext.Session.GetString("TiempoInicioActividad");
        
        if (string.IsNullOrEmpty(usuarioJson) || string.IsNullOrEmpty(tiempoInicioStr))
        {
            return Json(new { success = false });
        }

        // Convertimos milisegundos a ticks para crear el DateTime
        DateTime tiempoFin = new DateTime(tiempoFinMs * 10000, DateTimeKind.Utc); 
        
        if (DateTime.TryParse(tiempoInicioStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime tiempoInicio))
        {
            TimeSpan duracion = tiempoFin - tiempoInicio;

            if (duracion.TotalSeconds > 1) 
            {
                Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);
                Usuario usuarioCompleto = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);

                if (usuarioCompleto != null)
                {
                    // 1. REGISTRA EL TIEMPO DIARIO (para el gráfico de barras)
                    BD.RegistrarTiempoDiario(usuarioCompleto.id, duracion);

                    // 2. LÓGICA CLAVE: ACTUALIZA EL TIEMPO TOTAL ACUMULADO (campo maestro)
                    // **NOTA: Debes implementar este método en BD.cs y el SP en SQL**
                    BD.ActualizarTiempoEnPantallaTotal(usuarioCompleto.id, duracion);
                }
            }

            HttpContext.Session.Remove("TiempoInicioActividad");
            return Json(new { success = true });
        }
        
        return Json(new { success = false });
    }

public IActionResult IrAPreviewEstadisticas()
{

     return View("PreviewEstadisticas");
}

public IActionResult PreviewEstadisticas(int idPerteneciente)
{

     return RedirectToAction("Estadisticas", new {idPerteneciente = idPerteneciente});
}
public IActionResult Estadisticas(int IdPerteneciente)
{
    string? usuarioJson = HttpContext.Session.GetString("Usuario");
    if (string.IsNullOrEmpty(usuarioJson))
    {
        return RedirectToAction("Index", "Account");
    }
    Usuario userDeSesion = Objeto.StringToObject<Usuario>(usuarioJson);
    
    Usuario usuarioActualizado = BD.TraerUNUsuario(userDeSesion.nombreUsuario, userDeSesion.contraseña);

 List<Usuario> ListaVinculos = BD.ListaVinculos(usuarioActualizado);

if(IdPerteneciente >0)
{
  IdPerteneciente = ListaVinculos[0].id;
 }
    // 1. DATOS DIARIOS (Lista de todos los días que usó la app y cuánto tiempo hizo en cada día)
    List<TiempoDiario> tiemposDiarios = BD.TraerTiemposDiarios(IdPerteneciente);

    
    ViewBag.TiempoDiario = tiemposDiarios;

    // 2. CÁLCULO DEL PROMEDIO DIARIO (NUEVA VARIABLE)
    if (tiemposDiarios != null && tiemposDiarios.Any())
    {
        // Calculamos el total de segundos de todos los días.
        double totalSegundos = tiemposDiarios.Sum(t => (double)t.Tiempo);
        int totalDias = tiemposDiarios.Count;
        
        // Calculamos el promedio y lo convertimos a TimeSpan para fácil uso en la vista.
        double promedioSegundos = totalSegundos / totalDias;
        TimeSpan tiempoPromedio = TimeSpan.FromSeconds(promedioSegundos);
        
        // Pasamos el promedio
        ViewBag.TiempoPromedioDiario = tiempoPromedio;
        
        // Opcional: También pasamos el conteo total de días
        ViewBag.TotalDiasRegistrados = totalDias;
    }
    else
    {
        // Si no hay datos, inicializamos el promedio y conteo a valores seguros.
        ViewBag.TiempoPromedioDiario = TimeSpan.Zero;
        ViewBag.TotalDiasRegistrados = 0;
    }

    // 3. TIEMPO TOTAL ACUMULADO (Métrica maestra)
    DateTime tiempoAcumuladoBD = BD.TraerTiempoEnPantallaTotal(IdPerteneciente);
    TimeSpan duracionAcumulada = tiempoAcumuladoBD.Subtract(new DateTime(1900, 1, 1));
    
    // Pasamos el TimeSpan directamente (es más limpio que pasar los segundos y re-convertir).
    ViewBag.TiempoTotalAppAcumulado = duracionAcumulada; 
    
    // Otros ViewBags
    ViewBag.progreso = BD.TraerProgresoActividad(usuarioActualizado.id, 6);
    
    // Eliminamos el ViewBag.TiempoEnPantallaTotal (int) ya que usamos TiempoTotalAppAcumulado (TimeSpan).

    return View("Estadisticas");
}
        
    
    




    
}