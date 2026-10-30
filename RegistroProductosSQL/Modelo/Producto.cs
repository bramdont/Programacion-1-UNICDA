using System.Data;

namespace RegistroProductosSQL.Modelo
{
    public class Producto
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Marca { get; set; } = string.Empty;

        public override string ToString() => $"{Codigo} - {Nombre} ({Marca}) - ${Precio:N2} - Stock: {Stock}";

        // Los SP devuelven columnas con alias Código, Nombre, Precio, Stock, Marca
        public static List<Producto> Desde(DataTable dt)
        {
            var lista = new List<Producto>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new Producto
                {
                    Codigo = Convert.ToInt32(row["Código"]),
                    Nombre = row["Nombre"].ToString() ?? string.Empty,
                    Precio = Convert.ToDecimal(row["Precio"]),
                    Stock = Convert.ToInt32(row["Stock"]),
                    Marca = row["Marca"].ToString() ?? string.Empty
                });
            }
            return lista;
        }
    }
}
