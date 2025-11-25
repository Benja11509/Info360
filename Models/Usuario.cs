using Newtonsoft.Json;
namespace Info360.Models;
public class Usuario
{

    [JsonProperty]
    public int id { get; private set; }

    [JsonProperty]
    public string nombreUsuario { get; set; }
    [JsonProperty]
    public string? nombre { get;  set; }
    [JsonProperty]
    public string contraseña { get; set; }
    [JsonProperty]
    public string? apellido { get; set; }
    [JsonProperty]
    public DateTime? fechaNacimiento { get; set; }
    [JsonProperty]
    public string tipoUsuario { get;  set; }
    [JsonProperty]
    public string? telefono { get;  set; }
    [JsonProperty]
    public int? nivelApoyo { get;  set; }
    [JsonProperty]
    public DateTime? fechaIngreso { get;  set; }
    [JsonProperty]
    public int? puntos { get;  set; }
    [JsonProperty]
    public string mail { get;  set; }
    [JsonProperty]
    public string? fotoPerfil { get;  set; }
[JsonProperty]
    public string? descripcion { get;  set;}

[JsonProperty]
    public int? PreguntaActual { get;  set;}
    public Usuario(string nombreUsuario, string contraseña, string mail, string tipoUsuario)
    {
        this.nombreUsuario = nombreUsuario;
        this.contraseña = contraseña;
        this.mail = mail;
        this.tipoUsuario = tipoUsuario;

    }
    public Usuario()
    {

    }
    public void ActualizarDatosOpcionales(string pnombre, string papellido, DateTime? pfechaNacimiento, string ptelefono, string pfotoPerfil, int? pnivelApoyo, string? pdescripcion )
    {
nombre = pnombre;
apellido = papellido;
fechaNacimiento = pfechaNacimiento;
telefono = ptelefono;
fotoPerfil = pfotoPerfil;
nivelApoyo = pnivelApoyo;
descripcion = pdescripcion;


    }




}