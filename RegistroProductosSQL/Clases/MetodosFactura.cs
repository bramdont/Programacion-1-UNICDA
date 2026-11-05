using System;
using System.Data;
using System.Data.SqlClient;
using RegistroProductosSQL.Modelo;

namespace RegistroProductosSQL.Clases
{
    public class MetodosFactura
    {
        // Guarda cliente, cabecera, líneas (descuenta el stock) y, si es a crédito, la cuenta
        // por cobrar, todo en una sola transacción: o se guarda todo o no se guarda nada.
        // Asigna factura.Numero (consecutivo de 5 dígitos) y lo devuelve.
        public int GuardarFactura(Factura factura)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        int idCliente;
                        using (SqlCommand cmd = new SqlCommand("sp_GuardarCliente", con, tx))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@cedula", factura.ClienteCedula);
                            cmd.Parameters.AddWithValue("@nombre", factura.ClienteNombre);
                            idCliente = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        int idFactura;
                        using (SqlCommand cmd = new SqlCommand("sp_InsertarFactura", con, tx))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@fecha", factura.Fecha);
                            cmd.Parameters.AddWithValue("@id_usuario", Variables.IdUsuario);
                            cmd.Parameters.AddWithValue("@cliente_nombre", factura.ClienteNombre);
                            cmd.Parameters.AddWithValue("@cliente_cedula", factura.ClienteCedula);
                            cmd.Parameters.AddWithValue("@subtotal", factura.TotalBruto);
                            cmd.Parameters.AddWithValue("@iva", factura.Itbis); // monto del ITBIS
                            cmd.Parameters.AddWithValue("@total", factura.Total);
                            cmd.Parameters.AddWithValue("@tipo", factura.Tipo);
                            cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                            cmd.Parameters.AddWithValue("@monto_pagado", factura.MontoPagado);

                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                dr.Read();
                                idFactura = Convert.ToInt32(dr["id_factura"]);
                                factura.Numero = Convert.ToInt32(dr["numero"]);
                            }
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
                                cmd.Parameters.AddWithValue("@iva", linea.Itbis);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        if (factura.EsCredito)
                        {
                            using (SqlCommand cmd = new SqlCommand("sp_InsertarCuentaPorCobrar", con, tx))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@id_factura", idFactura);
                                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                                cmd.Parameters.AddWithValue("@fecha", factura.Fecha);
                                cmd.Parameters.AddWithValue("@monto", factura.Total);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                        return factura.Numero;
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
