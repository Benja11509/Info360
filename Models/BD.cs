using Microsoft.Data.SqlClient;
using Dapper;



public static class BD
{

    private static string _connectionString = @"Server=localhost;
DataBase=Tandem;Integrated Security=True;TrustServerCertificate=True;";



    public static Usuario TraerUNUsuario(string NombreUSU, string Contraseña)
    {
        Usuario user = null;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM Integrantes where nombreUsuario = @pNombreusuarios AND contraseña = @pContraseña ";
            user = connection.QueryFirstOrDefault<Usuario>(query, new { pNombreusuarios = NombreUSU, pContraseña = Contraseña });
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
        bool esta = VerificarUsuario(Usu.nombre, Usu.contraseña);
        if (esta == false)
        {
            Usuario user = null;
            sePudo = false;
            string query = "INSERT INTO Usuarios (NombreUSU, Contraseña, nombre, apellido, fechaNacimiento, tipoUsuario, telefono, nivelApoyo, fechaIngreso, puntos, mail, fotoPerfil) VALUES ( @pNombreUSU, @pContraseña, @pnombre, @papellido, @pfechaNacimiento, @ptipoUsuario, @ptelefono, @pnivelApoyo, @pfechaIngreso, @ppuntos, @pmail, @pfotoPerfil)";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(query, new { @pNombreUSU = Usu.nombreUsuario, @pContraseña = Usu.contraseña, @pnombre = Usu.nombre, @papellido = Usu.apellido, @pfechaNacimiento = Usu.fechaNacimiento, @ptipoUsuario = Usu.tipoUsuario, @ptelefono = Usu.telefono, @pnivelApoyo = Usu.nivelApoyo, @pfechaIngreso = Usu.fechaIngreso, @ppuntos = Usu.puntos, @pmail = Usu.mail, @pfotoPerfil = Usu.fotoPerfil });
            }
            esta = VerificarUsuario(Usu.nombre, Usu.contraseña);
            if (esta) sePudo = true;
        }
        //aca tira muchos errores porque no me deja declarar objetos de tipo usuario :(
        return sePudo;
    }

    public static bool EliminarUsuario(string nombre)
    {
        bool sePudo = false;
        string query = "DELETE FROM Usuarios WHERE nombreUsuario = @nombre";
        int registrosAfectados = 0;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            registrosAfectados = connection.Execute(query, new { nombre });
        }
        if (registrosAfectados > 0) sePudo = true;

        return sePudo;
    }

}
