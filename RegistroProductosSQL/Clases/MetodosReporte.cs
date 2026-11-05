using System;
using System.Data;
using System.Data.SqlClient;

namespace RegistroProductosSQL.Clases
{
    public class MetodosReporte
    {
        public DataTable CuadreDelDia(DateTime desde, DateTime hasta) =>
            EjecutarReporte("sp_ReporteCuadre", desde, hasta);

        public DataTable VentasPorCajero(DateTime desde, DateTime hasta) =>
            EjecutarReporte("sp_ReporteVentasPorCajero", desde, hasta);

        public DataTable Inventario() =>
            EjecutarReporte("sp_ReporteInventario", null, null);

        // tipo: null = todas, "Contado" o "Credito"
        public DataTable Facturas(DateTime desde, DateTime hasta, string? tipo) =>
            EjecutarReporte("sp_ReporteFacturas", desde, hasta, tipo);

        public DataTable TopClientes(DateTime desde, DateTime hasta) =>
            EjecutarReporte("sp_ReporteTopClientes", desde, hasta);

        public DataTable CuentasPorCobrar() =>
            EjecutarReporte("sp_ReporteCuentasPorCobrar", null, null);

        private DataTable EjecutarReporte(string nombreSp, DateTime? desde, DateTime? hasta, string? tipo = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(nombreSp, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (desde != null && hasta != null)
                    {
                        cmd.Parameters.Add("@desde", SqlDbType.Date).Value = desde.Value.Date;
                        cmd.Parameters.Add("@hasta", SqlDbType.Date).Value = hasta.Value.Date;
                    }
                    if (nombreSp == "sp_ReporteFacturas")
                    {
                        cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = (object?)tipo ?? DBNull.Value;
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }
    }
}
