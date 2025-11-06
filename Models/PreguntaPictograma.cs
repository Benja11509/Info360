public class PreguntaPictograma
    {
        // Propiedades de la Base de Datos
  
        public int IdPregunta { get; set; }

        public string ImagenPictograma { get; set; }
       
        public string Opcion1 { get; set; }
    
        public string Opcion2 { get; set; }
    
        public string Opcion3 { get; set; }
   
        public string Opcion4 { get; set; }
    
        public int RespuestaCorrecta { get; set; }

        // --- MANEJO DE ESTADO (Igual que en Objeto.cs) ---

        // Lista estática para guardar las preguntas del juego actual
      
        public static List<PreguntaPictograma> _ListaPreguntas { get; private set; } = new List<PreguntaPictograma>();
        public static List<int> _ListaPreguntasHechas { get; private set; } = new List<int>();
        
        // Variable estática para guardar el índice de la pregunta actual
    
        public static int _IndiceActual { get; private set; } = 0;

        // Método para cargar la lista de preguntas (lo llamará el Controller)
        public static void CargarPreguntas(List<PreguntaPictograma> preguntas)
        {
            _ListaPreguntas = preguntas;
            Random r = new Random();
            _IndiceActual = r.Next(_ListaPreguntas.Count);
        }

        // Método para obtener la pregunta actual
        public static PreguntaPictograma ObtenerPreguntaActual()
        {
            if (_ListaPreguntas != null && !_ListaPreguntasHechas.Contains(_IndiceActual))
            {
                _ListaPreguntasHechas.Add(_IndiceActual);
                return _ListaPreguntas[_IndiceActual];
            }
            return null; // No hay más preguntas
        }

        // Método para avanzar a la siguiente pregunta
        public static PreguntaPictograma AvanzarSiguientePregunta()
        {
            Random r = new Random();
            _IndiceActual = r.Next(_ListaPreguntas.Count);
            if (_ListaPreguntas != null && !_ListaPreguntasHechas.Contains(_IndiceActual))
            {
                return _ListaPreguntas[_IndiceActual];
            } else if(_ListaPreguntasHechas.Contains(_IndiceActual)) AvanzarSiguientePregunta();
            return null; // Se terminaron las preguntas
        }
    }