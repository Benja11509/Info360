using Newtonsoft.Json;
namespace Info360.Models;
public class TiempoDiario
{
[JsonProperty]
    public int id { get;  set; }
    
    [JsonProperty]
    public int idUsuario { get; set; }
    [JsonProperty]
    public int Tiempo { get; set; }
    [JsonProperty]
public DateTime Fecha { get;  set; }

public DateTime TiempoEnPantallaTotal { get; set; }

public TiempoDiario ()
{

}
public TimeSpan TiempoNeto 
    {
        get { return TimeSpan.FromSeconds(Tiempo); }
    }









}