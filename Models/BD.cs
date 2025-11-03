using Microsoft.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Linq;

public static class BD
{
    private static string _connectionString = @"Server=localhost;
DataBase=Tandem;Integrated Security=True;TrustServerCertificate=True;";

    public static Usuario TraerUNUsuario(string NombreUSU, string Contraseña)
    {
        Usuario user = null;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM Usuarios where nombreUsuario = @pNombreusuarios AND contraseña = @pContraseña ";
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
        bool esta = VerificarUsuario(Usu.nombreUsuario, Usu.contraseña);
        
        if (esta == false)
        {
            sePudo = false;
            string query = "INSERT INTO Usuarios (nombreUsuario, contraseña, tipoUsuario, mail) VALUES ( @pNombreUSU, @pContraseña, @ptipoUsuario, @pmail)";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(query, new { @pNombreUSU = Usu.nombreUsuario, @pContraseña = Usu.contraseña, @ptipoUsuario = Usu.tipoUsuario, @pmail = Usu.mail});
            }
            esta = VerificarUsuario(Usu.nombreUsuario, Usu.contraseña);
            if (esta) sePudo = true;
        }
        return sePudo;
    }

    public static void ActualizarUsuario(Usuario user)
    {
        string query = @"UPDATE Usuarios SET 
                            nombre = @pNombre, 
                            apellido = @pApellido, 
                            fechaNacimiento = @pFechaNacimiento, 
                            telefono = @pTelefono, 
                            nivelApoyo = @pNivelApoyo, 
                            fotoPerfil = @pFotoPerfil,
                            descripcion = @pDescripcion
                       WHERE id = @pId";
        
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new {
                @pNombre = user.nombre,
                @pApellido = user.apellido,
                @pFechaNacimiento = user.fechaNacimiento,
                @pTelefono = user.telefono,
                @pNivelApoyo = user.nivelApoyo,
                @pFotoPerfil = user.fotoPerfil,
                @pDescripcion = user.descripcion,
                @pId = user.id
            });
        }
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

    public static List<Usuario> ListaVinculos(Usuario user)
    {
        List<Usuario> ListVinculos = new List<Usuario>();
        if(user.tipoUsuario == "tutor")
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuarios WHERE tipoUsuario = 'perteneciente' AND id IN (SELECT idPerteneciente FROM Tutoria WHERE idTutor = @idUser)";
                ListVinculos = connection.Query<Usuario>(query, new { idUser = user.id }).ToList();
            }
        }
        else 
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuarios WHERE tipoUsuario = 'tutor' AND id IN (SELECT idTutor FROM Tutoria WHERE idPerteneciente = @idUser)";
                ListVinculos = connection.Query<Usuario>(query, new { idUser = user.id }).ToList();
            }
        }
        return ListVinculos;
    }

    public static List<Usuario> ListaUsuariosDisponibles(int idTutor)
    {
        List<Usuario> lista = new List<Usuario>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
         
            string query = "SELECT * FROM Usuarios WHERE tipoUsuario = 'perteneciente' AND Id NOT IN (SELECT idPerteneciente FROM Tutoria WHERE idTutor = @idTutor)";
            lista = connection.Query<Usuario>(query, new { idTutor }).ToList();
        }
        return lista;
    }

   
    public static void AgregarVinculoBD(int idTutor, int idPerteneciente, string parentesco)
    {
        string query = "INSERT INTO Tutoria(idTutor, idPerteneciente, parentesco) VALUES (@pIdTutor, @pIdPerteneciente, @pParentesco)";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new { pIdTutor = idTutor, pIdPerteneciente = idPerteneciente, pParentesco = parentesco });
        }
    }

  
    public static void EliminarVinculoBD(int idTutor, int idPerteneciente)
    {
        string query = "DELETE FROM Tutoria WHERE idTutor = @pIdTutor AND idPerteneciente = @pIdPerteneciente";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new { pIdTutor = idTutor, pIdPerteneciente = idPerteneciente });
        }
    }
     
}