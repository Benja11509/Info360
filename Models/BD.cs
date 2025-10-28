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
       bool esta = false;
       if(user != null) esta = true;
       return esta;
     }

    public static bool CrearUsuario (string NombreUSU, string Contraseña){
        bool sePudo = false;
        bool esta = VerificarUsuario(NombreUSU, Contraseña);
        if(esta == false){
            Usuario user = null;
            bool sePudo = false;
                //aca tengo que hacer el insert con el usuario, ver que hago con los que permiten nulos
             string query = "INSERT INTO Usuarios (Nombre, Fechalacimiento) VALUES ( OpNonbre, OpFechaNacimiento)";
             using(SqlConnection connection new SqlConnection(_connectionString)){
                connection.Execute(query, new pIdEquipo jug. IdEquipo, Nombre jug. Nombre, pFechaNacimiento jug. FechaNacimiento 1);
             }
        }
        
        return sePudo;
    }








}
