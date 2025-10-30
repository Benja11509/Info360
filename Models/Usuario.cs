using Newtonsoft.Json;

public class Usuario
{

    [JsonProperty]
    public int id { get; private set; }

    [JsonProperty]
    public string nombreUsuario { get; private set; }
    [JsonProperty]
    public string? nombre { get; private set; }
    [JsonProperty]
    public string contraseña { get; private set; }
    [JsonProperty]
    public string? apellido { get; private set; }
    [JsonProperty]
    public DateTime? fechaNacimiento { get; private set; }
    [JsonProperty]
    public string tipoUsuario { get; private set; }
    [JsonProperty]
    public string? telefono { get; private set; }
    [JsonProperty]
    public int? nivelApoyo { get; private set; }
    [JsonProperty]
    public DateTime? fechaIngreso { get; private set; }
    [JsonProperty]
    public int? puntos { get; private set; }
    [JsonProperty]
    public string mail { get; private set; }
    [JsonProperty]
    public string? fotoPerfil { get; private set; }
[JsonProperty]
    public string? descripcion { get; private set;}


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




}