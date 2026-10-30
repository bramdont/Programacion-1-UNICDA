using System;
using System.Data.SqlClient;

namespace RegistroProductosSQL.Clases
{
    public class Conexion
    {
        // Server = servidor de Azure SQL; Initial Catalog = base de datos donde se ejecutó el script SQL
        private static string cadenaConexion = @"Server=tcp:programacion1.database.windows.net,1433;Initial Catalog=programacion1;Persist Security Info=False;User ID=user;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection ObtenerConexion()
        {
            SqlConnection con = new SqlConnection(cadenaConexion);
            return con;
        }
    }
}
