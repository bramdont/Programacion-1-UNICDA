using System.Data;

namespace RegistroProductosSQL.Modelo
{
    public class Cajero
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Turno { get; set; } = string.Empty;

        public static List<Cajero> Desde(DataTable dt)
        {
            var lista = new List<Cajero>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new Cajero
                {
                    Id = Convert.ToInt32(row["id_usuario"]),
                    Cedula = row["cedula"].ToString() ?? string.Empty,
                    Nombre = row["nombres"] + " " + row["apellidos"],
                    Usuario = row["usuario"].ToString() ?? string.Empty,
                    Turno = row["turno"].ToString() ?? string.Empty
                });
            }
            return lista;
        }
    }
}
