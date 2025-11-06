using Newtonsoft.Json;

public class Actividades
{
[JsonProperty]
    public int Id { get; private set; }
    
    [JsonProperty]
    public string nombre { get; private set; }
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
    [JsonProperty]
    public List<Actividades> ListActividadesPendientes { get; private set; } = new List<Actividades>();


public Actividades ()
{

}










}