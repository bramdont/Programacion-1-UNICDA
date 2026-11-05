using System;
using System.Data;
using System.Data.SqlClient;

namespace RegistroProductosSQL.Clases
{
    public class MetodosUsuario
    {
        public bool CrearCajero(string cedula, string nombres, string apellidos, string telefono,
                                string email, string usuario, string clave, string turno)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("sp_CrearCajero", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cedula", cedula);
                    cmd.Parameters.AddWithValue("@nombres", nombres);
                    cmd.Parameters.AddWithValue("@apellidos", apellidos);
                    cmd.Parameters.AddWithValue("@telefono", telefono);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.Parameters.AddWithValue("@turno", turno);
                    cmd.Parameters.AddWithValue("@fecha_ingreso", DateTime.Now);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public DataTable ListarCajeros()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarCajeros", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }
    }
}
