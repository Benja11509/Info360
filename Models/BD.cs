using Microsoft.Data.SqlClient;
using Dapper;



public static class BD
{

    private static string _connectionString = @"Server=localhost;
DataBase=Tandem;Integrated Security=True;TrustServerCertificate=True;";


    
     public static Usuario TraerUNUsuario (string NombreUSU, string Contraseña)
     {
     Usuario user = null;
         using (SqlConnection connection = new SqlConnection(_connectionString))
         {
             string query = "SELECT * FROM Integrantes where nombreUsuario = @pNombreusuarios AND contraseña = @pContraseña ";
            user = connection.QueryFirstOrDefault<Usuario>(query, new {pNombreusuarios = NombreUSU, pContraseña = Contraseña});
        }
       
        return user;
     }


    public static List<Usuario> TraerListaUsuarios()
    {
        List<Usuario> ListUsuarios = new List<Usuario>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM Usuarios";
            ListUsuarios = connection.Query<Usuario>(query).ToList();
        }
        return ListUsuarios;
    }
    public static List<Actividades> TraerListaActividades()
    {
        List<Actividades> ListAct = new List<Actividades>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM Actividades";
            ListAct = connection.Query<Actividades>(query).ToList();
        }
        return ListAct;
    }


  public static void VerificarUsuario (string NombreUSU, string Contraseña)
     {
       Usuario user = null;
         using (SqlConnection connection = new SqlConnection(_connectionString))
         {
             string query = "SELECT * FROM Integrantes where nombreUsuario = @pNombreusuarios AND contraseña = @pContraseña ";
             user = connection.QueryFirstOrDefault<Usuario>(query, new {pNombreusuarios = NombreUSU, pContraseña = Contraseña});
        }
       if (user == null){

       }
     }

    public static bool CrearUsuario (string NombreUSU, string Contraseña){
        Usuario user = null;
        bool sePudo = false;
        public void Agregar Jugador (Jugador jug){
             string query "INSERT INTO Jugadores (IdEquipo, Nombre, Fechalacimiento) VALUES (@pIdEquipe, OpNonbre, OpFechaNacimiento)";
             using(SqlConnection connection new SqlConnection(_connectionString)){
                connection.Execute(query, new pIdEquipo jug. IdEquipo, Nombre jug. Nombre, pFechaNacimiento jug. FechaNacimiento 1);
             }
        }
    }








}
