namespace RegistroProductosSQL.Modelo
{
    public class Factura
    {
        public const decimal ItbisPorcentaje = 18m;

        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Vendedor { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteCedula { get; set; } = string.Empty;
        public List<LineaFactura> Lineas { get; set; } = new List<LineaFactura>();

        public decimal Subtotal => Lineas.Sum(l => l.Subtotal);
        public decimal Itbis => Math.Round(Subtotal * ItbisPorcentaje / 100m, 2);
        public decimal Total => Subtotal + Itbis;
    }
}
