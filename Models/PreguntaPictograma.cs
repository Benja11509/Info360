using Newtonsoft.Json;
public class PreguntaPictograma
    {
      [JsonProperty]
        public int IdPregunta { get; set; }
    [JsonProperty]
        public string ImagenPictograma { get; set; }
           [JsonProperty]
        public string Opcion1 { get; set; }
        [JsonProperty]
        public string Opcion2 { get; set; }
        [JsonProperty]
        public string Opcion3 { get; set; }
       [JsonProperty]
        public string Opcion4 { get; set; }
        [JsonProperty]
        public string RespuestaCorrecta { get; set; }
          [JsonProperty]
        public  List<PreguntaPictograma> _ListaPreguntas { get; private set; } = new List<PreguntaPictograma>();
            [JsonProperty]
        public  List<int> _ListaPreguntasHechas { get; private set; } = new List<int>();
        [JsonProperty]
        public  int _IndiceActual { get; private set; } = 0;

    [JsonProperty]
        public Dictionary<string, DateTime> HorasPorDia = new dictionary<string, DateTime>();


        public  PreguntaPictograma()
        {
            Random r = new Random();
            _IndiceActual = r.Next(_ListaPreguntas.Count);
        }

        public  PreguntaPictograma AvanzarSiguientePregunta()
        {
            _ListaPreguntas = BD.TraerPreguntas();
            Random r = new Random();
            _IndiceActual = r.Next(_ListaPreguntas.Count);
            if (_ListaPreguntas != null && !_ListaPreguntasHechas.Contains(_IndiceActual))
            {
                _ListaPreguntasHechas.Add(_IndiceActual);
                return _ListaPreguntas[_IndiceActual];
            } else if(_ListaPreguntasHechas.Contains(_IndiceActual)) AvanzarSiguientePregunta();
            return null; // Se terminaron las preguntas
        }
    }