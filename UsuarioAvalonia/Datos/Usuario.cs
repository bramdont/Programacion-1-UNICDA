using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using UsuarioAvalonia.Modelo;

namespace UsuarioAvalonia.Datos
{
    /// <summary>
    /// Clase encargada de la gestión de la información de la tabla Usuarios en SQL Server.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Inserta un usuario nuevo. El Id lo genera la base de datos y queda asignado en el modelo.
        /// </summary>
        /// <param name="modelo">Objeto UsuarioModel con los datos del usuario</param>
        public async Task Guardar(UsuarioModel modelo)
        {
            const string sql = @"INSERT INTO Usuarios (Nombre, Edad, Estatus)
                                 OUTPUT INSERTED.id
                                 VALUES (@Nombre, @Edad, @Estatus)";

            await using var conexion = await ConexionDb.AbrirConexion();
            await using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.AddRange(Parametros(modelo));

            modelo.Id = (int)(await comando.ExecuteScalarAsync())!;
        }

        /// <summary>
        /// Consulta los usuarios activos y retorna la lista.
        /// </summary>
        /// <returns>Lista de tipo UsuarioModel</returns>
        public Task<List<UsuarioModel>> Consultar()
        {
            const string sql = @"SELECT id, Nombre, Edad, Estatus
                                 FROM Usuarios
                                 WHERE Estatus = 1
                                 ORDER BY id";

            return ConexionDb.EjecutarConsulta(sql, lector => new UsuarioModel
            {
                Id = lector.GetInt32(0),
                Nombre = lector.IsDBNull(1) ? string.Empty : lector.GetString(1),
                Edad = lector.IsDBNull(2) ? 0 : lector.GetInt32(2),
                Estatus = !lector.IsDBNull(3) && lector.GetBoolean(3)
            });
        }

        /// <summary>
        /// Actualiza el nombre y la edad del usuario indicado por modelo.Id.
        /// </summary>
        /// <returns>true si el usuario existía y se actualizó</returns>
        public async Task<bool> Actualizar(UsuarioModel modelo)
        {
            const string sql = @"UPDATE Usuarios
                                 SET Nombre = @Nombre, Edad = @Edad
                                 WHERE id = @Id";

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = modelo.Id },
                new SqlParameter("@Nombre", SqlDbType.NVarChar, 50) { Value = modelo.Nombre },
                new SqlParameter("@Edad", SqlDbType.Int) { Value = modelo.Edad }
            };

            return await ConexionDb.EjecutarComando(sql, parametros.ToArray()) > 0;
        }

        /// <summary>
        /// Eliminación lógica: desactiva al usuario (Estatus = 0), el registro se conserva.
        /// </summary>
        /// <returns>true si el usuario existía y se desactivó</returns>
        public async Task<bool> Eliminar(int id)
        {
            const string sql = "UPDATE Usuarios SET Estatus = 0 WHERE id = @Id";

            var parametro = new SqlParameter("@Id", SqlDbType.Int) { Value = id };

            return await ConexionDb.EjecutarComando(sql, parametro) > 0;
        }

        private static SqlParameter[] Parametros(UsuarioModel modelo)
        {
            return new[]
            {
                new SqlParameter("@Nombre", SqlDbType.NVarChar, 50) { Value = modelo.Nombre },
                new SqlParameter("@Edad", SqlDbType.Int) { Value = modelo.Edad },
                new SqlParameter("@Estatus", SqlDbType.Bit) { Value = modelo.Estatus }
            };
        }
    }
}
