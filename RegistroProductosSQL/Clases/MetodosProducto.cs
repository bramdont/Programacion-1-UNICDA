using System;
using System.Data;
using System.Data.SqlClient;

namespace RegistroProductosSQL.Clases
{
    public class MetodosProducto
    {
        public DataTable ListarProductos()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarProductos", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public bool InsertarProducto(string nombre, decimal precio, int stock, string marca)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertarProducto", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@precio", precio);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@marca", marca);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ActualizarProducto(int idProducto, string nombre, decimal precio, int stock, string marca)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarProducto", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_producto", idProducto);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@precio", precio);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@marca", marca);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool EliminarProducto(int idProducto)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("sp_EliminarProducto", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_producto", idProducto);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public DataTable BuscarProducto(string criterio, string valor)
        {
            DataTable dt = new DataTable();
            string nombreSp = criterio switch
            {
                "Código" => "sp_BuscarProductoPorCodigo",
                "Nombre" => "sp_BuscarProductoPorNombre",
                "Precio" => "sp_BuscarProductoPorPrecio",
                "Marca" => "sp_BuscarProductoPorMarca",
                _ => "sp_ListarProductos"
            };

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(nombreSp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (criterio == "Código" && int.TryParse(valor, out int code))
                    {
                        cmd.Parameters.AddWithValue("@id_producto", code);
                    }
                    else if (criterio == "Nombre")
                    {
                        cmd.Parameters.AddWithValue("@nombre", valor);
                    }
                    else if (criterio == "Precio" && decimal.TryParse(valor, out decimal price))
                    {
                        cmd.Parameters.AddWithValue("@precio", price);
                    }
                    else if (criterio == "Marca")
                    {
                        cmd.Parameters.AddWithValue("@marca", valor);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public bool ValidarUsuario(string usuario, string clave)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand("sp_IniciarSesion", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@clave", clave);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            Variables.IdUsuario = Convert.ToInt32(dr["id_usuario"]);
                            Variables.NombreUsuario = dr["nombres"].ToString() + " " + dr["apellidos"].ToString();
                            Variables.Estado = dr["estado"].ToString() ?? "Invitado";
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
