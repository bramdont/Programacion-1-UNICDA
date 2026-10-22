using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace UsuarioAvalonia.Datos
{
    /// <summary>
    /// Helper que administra la conexión con SQL Server. Lee la cadena de conexión de
    /// connectionstring.txt y centraliza la apertura/cierre de conexiones, de modo que
    /// las demás clases de la capa Datos solo se preocupan por el SQL que ejecutan.
    /// </summary>
    public static class ConexionDb
    {
        private const string ArchivoCadena = "connectionstring.txt";

        private static readonly Lazy<string> cadenaConexion = new Lazy<string>(LeerCadena);

        /// <summary>
        /// Lee la cadena de conexión del archivo que se copia junto al ejecutable.
        /// </summary>
        private static string LeerCadena()
        {
            string ruta = Path.Combine(AppContext.BaseDirectory, ArchivoCadena);

            if (!File.Exists(ruta))
            {
                throw new FileNotFoundException($"No se encontró el archivo {ArchivoCadena} en {AppContext.BaseDirectory}.", ruta);
            }

            string cadena = File.ReadAllText(ruta).Trim();

            if (string.IsNullOrWhiteSpace(cadena))
            {
                throw new InvalidOperationException($"El archivo {ArchivoCadena} está vacío.");
            }

            return cadena;
        }

        /// <summary>
        /// Crea y abre una conexión nueva. Quien la reciba debe liberarla con using.
        /// </summary>
        public static async Task<SqlConnection> AbrirConexion()
        {
            var conexion = new SqlConnection(cadenaConexion.Value);

            try
            {
                await conexion.OpenAsync();
                return conexion;
            }
            catch
            {
                await conexion.DisposeAsync();
                throw;
            }
        }

        /// <summary>
        /// Ejecuta una instrucción INSERT, UPDATE o DELETE.
        /// </summary>
        /// <returns>Número de filas afectadas</returns>
        public static async Task<int> EjecutarComando(string sql, params SqlParameter[] parametros)
        {
            await using var conexion = await AbrirConexion();
            await using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.AddRange(parametros);

            return await comando.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Ejecuta una consulta SELECT y convierte cada fila con la función recibida.
        /// </summary>
        public static async Task<List<T>> EjecutarConsulta<T>(string sql, Func<SqlDataReader, T> convertir, params SqlParameter[] parametros)
        {
            var resultado = new List<T>();

            await using var conexion = await AbrirConexion();
            await using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.AddRange(parametros);

            await using var lector = await comando.ExecuteReaderAsync();
            while (await lector.ReadAsync())
            {
                resultado.Add(convertir(lector));
            }

            return resultado;
        }
    }
}
