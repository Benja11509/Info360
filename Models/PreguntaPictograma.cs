public class PreguntaPictograma
    {
  
        public int IdPregunta { get; set; }

        public string ImagenPictograma { get; set; }
       
        public string Opcion1 { get; set; }
    
        public string Opcion2 { get; set; }
    
        public string Opcion3 { get; set; }
   
        public string Opcion4 { get; set; }
    
        public string RespuestaCorrecta { get; set; }
      
        public  List<PreguntaPictograma> _ListaPreguntas { get; private set; } = new List<PreguntaPictograma>();
        public  List<int> _ListaPreguntasHechas { get; private set; } = new List<int>();
    
        public  int _IndiceActual { get; private set; } = 0;

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