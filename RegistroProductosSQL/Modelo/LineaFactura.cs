namespace RegistroProductosSQL.Modelo
{
    public class LineaFactura
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Valor de la línea (cantidad x precio) y su ITBIS
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public decimal Itbis => Math.Round(Subtotal * Factura.ItbisPorcentaje / 100m, 2);
    }
}
