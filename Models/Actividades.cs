using Newtonsoft.Json;
namespace Info360.Models;
public class Actividades
{
[JsonProperty]
    public int Id { get;  set; }
    
    [JsonProperty]
    public string nombre { get; set; }
    [JsonProperty]
    public string categoria { get; private set; }
    [JsonProperty]
    public int  dificultad { get; private set; }
    [JsonProperty]
    public string descripcion { get; private set; }
    [JsonProperty]
    public string topico { get; private set; }
    [JsonProperty]
    public int recompensa { get; private set; }
    [JsonProperty]
    public int progreso { get; private set; }
    [JsonProperty]
    public bool favorito { get; private set; }
    [JsonProperty]
    public string imagen { get; private set; }
   
   


public Actividades ()
{

}










}