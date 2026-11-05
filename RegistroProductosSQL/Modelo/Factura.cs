namespace RegistroProductosSQL.Modelo
{
    public class Factura
    {
        // Tasa del ITBIS (%). Para cambiarla se modifica solo esta constante.
        // En la BD el campo "iva" guarda el MONTO del ITBIS, no la tasa.
        public const decimal ItbisPorcentaje = 18m;

        public const string Titulo = "FACTURA";
        public const string TipoContado = "Contado";
        public const string TipoCredito = "Credito";

        public int Numero { get; set; }
        public string NumeroTexto => Numero.ToString("D5"); // 00001, 00002, ...
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = TipoContado;
        public string Cajero { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteCedula { get; set; } = string.Empty;
        public decimal MontoPagado { get; set; }
        public decimal Devuelta { get; set; }
        public List<LineaFactura> Lineas { get; set; } = new List<LineaFactura>();

        public bool EsCredito => Tipo == TipoCredito;
        public string TipoTexto => EsCredito ? "Crédito" : "Contado";

        // El ITBIS total es la suma del ITBIS de cada línea, así los totales siempre cuadran
        public decimal TotalBruto => Lineas.Sum(l => l.Subtotal);
        public decimal Itbis => Lineas.Sum(l => l.Itbis);
        public decimal Total => TotalBruto + Itbis;
    }
}
