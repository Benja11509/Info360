using Microsoft.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Data;
public static class BD
{
    private static string _connectionString = @"Server=localhost;
DataBase=Tandem;Integrated Security=True;TrustServerCertificate=True;";

    public static Usuario TraerUNUsuario(string NombreUSU, string Contraseña)
    {
        Usuario user = null;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "TraerUNUsuario";
            user = connection.QueryFirstOrDefault<Usuario>(storedProcedure,
            new { pNombreusuarios = NombreUSU, pContraseña = Contraseña },
            commandType: CommandType.StoredProcedure);
        }
        return user;
    }



    public static List<Usuario> TraerListaUsuarios()
    {
        List<Usuario> ListUsuarios = new List<Usuario>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "TraerListaUsuarios";
            ListUsuarios = connection.Query<Usuario>(storedProcedure,
            commandType: CommandType.StoredProcedure).ToList();
        }
        return ListUsuarios;
    }
    public static List<Actividades> TraerListaActividades()
    {
        List<Actividades> ListAct = new List<Actividades>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "TraerListaActividades";
            ListAct = connection.Query<Actividades>(storedProcedure,
            commandType: CommandType.StoredProcedure).ToList();
        }
        return ListAct;
    }

    public static bool VerificarUsuario(string NombreUSU, string Contraseña)
    {
        Usuario user = TraerUNUsuario(NombreUSU, Contraseña);
        bool esta = false;
        if (user != null) esta = true;
        return esta;
    }

    public static bool CrearUsuario(Usuario Usu)
    {
        bool sePudo = false;
        bool esta = VerificarUsuario(Usu.nombreUsuario, Usu.contraseña);

        if (esta == false)
        {
            sePudo = false;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string storedProcedure = "CrearUsuario";
                connection.Execute(storedProcedure,
                new { pNombreUsu = Usu.nombreUsuario, pContraseña = Usu.contraseña, ptipoUsuario = Usu.tipoUsuario, pmail = Usu.mail },
                commandType: CommandType.StoredProcedure);
            }

            esta = VerificarUsuario(Usu.nombreUsuario, Usu.contraseña);
            if (esta) sePudo = true;
        }
        return sePudo;
    }

    public static void ActualizarUsuario(Usuario user)
    {
        string storedProcedure = "ActualizarUsuario";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(storedProcedure, new
            {
                @pNombre = user.nombre,
                @pApellido = user.apellido,
                @pFechaNacimiento = user.fechaNacimiento,
                @pTelefono = user.telefono,
                @pNivelApoyo = user.nivelApoyo,
                @pFotoPerfil = user.fotoPerfil,
                @pDescripcion = user.descripcion,
                @pId = user.id
            }, commandType: CommandType.StoredProcedure);
        }
    }

    public static bool EliminarUsuario(string nombre)
    {
        bool sePudo = false;
        int registrosAfectados = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "EliminarUsuario";
            registrosAfectados = connection.Execute(storedProcedure, new { nombre }, commandType: CommandType.StoredProcedure);
        }
        if (registrosAfectados > 0) sePudo = true;
        return sePudo;
    }

    public static List<Usuario> ListaVinculos(Usuario user)
    {
        List<Usuario> ListVinculos = new List<Usuario>();
        int idUser = user.id;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "ListaVinculos";
            ListVinculos = connection.Query<Usuario>(storedProcedure, new{ idUser },
            commandType: CommandType.StoredProcedure).ToList();
        }

        return ListVinculos;
    }

    public static List<Usuario> ListaUsuariosDisponibles(int idTutor)
    {
        List<Usuario> lista = new List<Usuario>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "ListaUsuariosDisponibles";
            lista = connection.Query<Usuario>(storedProcedure, new { idTutor },
            commandType: CommandType.StoredProcedure).ToList();
        }
        return lista;
    }


    public static void AgregarVinculoBD(int idTutor, int idPerteneciente, string parentezco)
    {

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "AgregarVinculoBD";
            int registrosAfectados = connection.Execute(storedProcedure, new { pIdTutor = idTutor, pIdPerteneciente = idPerteneciente, pParentezco = parentezco }, commandType: CommandType.StoredProcedure);
        }
    }


    public static void EliminarVinculoBD(int idTutor, int idPerteneciente)
    {
        string storedProcedure = "EliminarVinculoBD";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(storedProcedure, new { pIdTutor = idTutor, pIdPerteneciente = idPerteneciente },
            commandType: CommandType.StoredProcedure);
        }
    }
    public static List<PreguntaPictograma> TraerPreguntas()
    {
        List<PreguntaPictograma> ListaPreguntas = new List<PreguntaPictograma>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "TraerPreguntas";
            ListaPreguntas = connection.Query<PreguntaPictograma>(storedProcedure,
            commandType: CommandType.StoredProcedure).ToList();
        }
        return ListaPreguntas;
    }



    public static PreguntaPictograma TraerPregunta(int idPregunta)
    {
        PreguntaPictograma pregunta = null;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "TraerPregunta";
            pregunta = connection.QueryFirstOrDefault<PreguntaPictograma>(storedProcedure, new { IdPregunta = idPregunta }, commandType: CommandType.StoredProcedure);
        }
        return pregunta;
    }

    public static bool VerificarRespuestaBD(int idPregunta, string opcion)
    {
        string storedProcedure = "VerificarRespuestaBD";
        bool ADevolver = false;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            int resultado = connection.ExecuteScalar<int>(storedProcedure, new { pIdPregunta = idPregunta, pOpcion = opcion }, commandType: CommandType.StoredProcedure);
            if (resultado > 0)
            {
                ADevolver = true;
            }
            return (ADevolver);
        }
    }

    public static void ActualizarPuntosUsuario(int idUsuario, int puntosTotales)
    {
        string storedProcedure = "ActualizarPuntosUsuario";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(storedProcedure, new { @pPuntos = puntosTotales, @pId = idUsuario }, commandType: CommandType.StoredProcedure);
        }
    }


    public static int GetTotalPreguntas()
    {
        string storedProcedure = "GetTotalPreguntas";
        int cantList = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            cantList = connection.Query<int>(storedProcedure, commandType: CommandType.StoredProcedure).ToList().Count();
        }
        return cantList;
    }

    
public static List<Actividades> TraerActividadesPendientes(int idUsuario)
{
    List<Actividades> ListActPendientes = new List<Actividades>();
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string storedProcedure = "TraerActividadesPendientes"; 
        // Pasamos el Id del usuario como parámetro
        ListActPendientes = connection.Query<Actividades>(storedProcedure, 
            new { pIdUsuario = idUsuario }, 
            commandType: CommandType.StoredProcedure).ToList();
    }
    return ListActPendientes;
}



// 1. Método para obtener todos los IDs de las preguntas de una actividad (para navegación robusta)
public static List<int> TraerIdsPreguntasActividad(int idActividad)
{
    // Consulta directa para obtener los IDs de preguntas ordenadas por ID
    string query = "SELECT id FROM PreguntasPictogramas WHERE idActividad = @pIdActividad ORDER BY id ASC";
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        return connection.Query<int>(query, new { pIdActividad = idActividad }).ToList();
    }
}

// 2. Método para actualizar el progreso de la actividad para un usuario
public static void ActualizarProgresoActividad(int idUsuario, int idActividad, int progreso)
{
    string storedProcedure = "ActualizarProgresoActividad";
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        connection.Execute(storedProcedure, new { 
            pIdUsuario = idUsuario, 
            pIdActividad = idActividad, 
            pProgreso = progreso 
        }, commandType: CommandType.StoredProcedure);
    }
}
}
