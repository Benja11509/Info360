using Newtonsoft.Json;

public class Usuario
{

    [JsonProperty]
    public int Id { get; private set; }
    
    [JsonProperty]
    public string nombreUsuario { get; private set; }
    [JsonProperty]
    public string nombre { get; private set; }
    [JsonProperty]
    public string apellido { get; private set; }
    [JsonProperty]
    public DateTime fechaNacimiento { get; private set; }
    [JsonProperty]
    public string tipoUsuario { get; private set; }
    [JsonProperty]
    public string telefono { get; private set; }
    [JsonProperty]
    public int NivelApoyo { get; private set; }
    [JsonProperty]
    public DateTime FechaIngreso { get; private set; }
    [JsonProperty]
    public int puntos { get; private set; }
    [JsonProperty]
    public string mail { get; private set; }
    [JsonProperty]
    public string fotoPerfil { get; private set; }



public Usuario()
{


}







}