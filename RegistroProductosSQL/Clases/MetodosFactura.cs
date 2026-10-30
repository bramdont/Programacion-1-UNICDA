using System;
using System.Data;
using System.Data.SqlClient;
using RegistroProductosSQL.Modelo;

namespace RegistroProductosSQL.Clases
{
    public class MetodosFactura
    {
        // Guarda la cabecera y las líneas en una sola transacción; devuelve el número de factura
        public int GuardarFactura(Factura factura)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        int idFactura;
                        using (SqlCommand cmd = new SqlCommand("sp_InsertarFactura", con, tx))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@fecha", factura.Fecha);
                            cmd.Parameters.AddWithValue("@id_usuario", Variables.IdUsuario);
                            cmd.Parameters.AddWithValue("@cliente_nombre", factura.ClienteNombre);
                            cmd.Parameters.AddWithValue("@cliente_cedula", factura.ClienteCedula);
                            cmd.Parameters.AddWithValue("@subtotal", factura.Subtotal);
                            cmd.Parameters.AddWithValue("@iva", factura.Itbis);
                            cmd.Parameters.AddWithValue("@total", factura.Total);
                            idFactura = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        foreach (LineaFactura linea in factura.Lineas)
                        {
                            using (SqlCommand cmd = new SqlCommand("sp_InsertarDetalleFactura", con, tx))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@id_factura", idFactura);
                                cmd.Parameters.AddWithValue("@id_producto", linea.IdProducto);
                                cmd.Parameters.AddWithValue("@cantidad", linea.Cantidad);
                                cmd.Parameters.AddWithValue("@precio", linea.PrecioUnitario);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                        return idFactura;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
