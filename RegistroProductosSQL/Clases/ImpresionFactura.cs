using System;
using System.Globalization;
using System.IO;
using System.Text;
using RegistroProductosSQL.Modelo;

namespace RegistroProductosSQL.Clases
{
    public static class ImpresionFactura
    {
        private const int Ancho = 64;
        private const int LineasPorPagina = 60;
        private static readonly CultureInfo Cultura = CultureInfo.InvariantCulture;

        public static void Guardar(Factura factura, string ruta)
        {
            if (Path.GetExtension(ruta).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                File.WriteAllText(ruta, ComoTexto(factura), new UTF8Encoding(true));
            }
            else
            {
                File.WriteAllBytes(ruta, ComoPdf(factura));
            }
        }

        public static string ComoTexto(Factura f)
        {
            string linea = new string('-', Ancho);
            string doble = new string('=', Ancho);
            var sb = new StringBuilder();

            sb.AppendLine(doble);
            sb.AppendLine(Centrar($"FACTURA No. {f.Numero:D6}"));
            sb.AppendLine(doble);
            sb.AppendLine($"Fecha    : {f.Fecha:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Vendedor : {f.Vendedor}");
            sb.AppendLine($"Cliente  : {f.ClienteNombre}");
            sb.AppendLine($"Cédula   : {f.ClienteCedula}");
            sb.AppendLine(linea);
            sb.AppendLine($"{"Cant",-5} {"Descripción",-30} {"P. Unit.",12} {"Subtotal",13}");
            sb.AppendLine(linea);

            foreach (LineaFactura l in f.Lineas)
            {
                string desc = l.Nombre.Length > 30 ? l.Nombre.Substring(0, 30) : l.Nombre;
                sb.AppendLine($"{l.Cantidad,-5} {desc,-30} {Dinero(l.PrecioUnitario),12} {Dinero(l.Subtotal),13}");
            }

            sb.AppendLine(linea);
            sb.AppendLine(Total("Subtotal", f.Subtotal));
            sb.AppendLine(Total($"ITBIS {Factura.ItbisPorcentaje:0.##}%", f.Itbis));
            sb.AppendLine(Total("TOTAL", f.Total));
            sb.AppendLine(doble);
            sb.AppendLine(Centrar("Gracias por su compra"));

            return sb.ToString();
        }

        // PDF mínimo: el mismo texto de la factura en fuente monoespaciada (Courier)
        public static byte[] ComoPdf(Factura f)
        {
            string[] lineas = ComoTexto(f).Replace("\r", "").TrimEnd('\n').Split('\n');
            int paginas = (lineas.Length + LineasPorPagina - 1) / LineasPorPagina;
            Encoding latin1 = Encoding.Latin1;

            using var ms = new MemoryStream();
            var offsets = new List<long>();

            void Escribir(string s)
            {
                byte[] b = latin1.GetBytes(s);
                ms.Write(b, 0, b.Length);
            }

            void Objeto(string contenido)
            {
                offsets.Add(ms.Position);
                Escribir($"{offsets.Count} 0 obj\n{contenido}\nendobj\n");
            }

            Escribir("%PDF-1.4\n");
            Objeto("<< /Type /Catalog /Pages 2 0 R >>");

            var kids = new StringBuilder();
            for (int i = 0; i < paginas; i++) kids.Append($"{4 + 2 * i} 0 R ");
            Objeto($"<< /Type /Pages /Kids [ {kids}] /Count {paginas} >>");
            Objeto("<< /Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding >>");

            for (int i = 0; i < paginas; i++)
            {
                var contenido = new StringBuilder("BT /F1 10 Tf 12 TL 50 800 Td\n");
                foreach (string l in lineas.Skip(i * LineasPorPagina).Take(LineasPorPagina))
                {
                    contenido.Append('(').Append(Escapar(l)).Append(") Tj T*\n");
                }
                contenido.Append("ET");

                int largo = latin1.GetByteCount(contenido.ToString());
                Objeto($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 3 0 R >> >> /Contents {5 + 2 * i} 0 R >>");
                Objeto($"<< /Length {largo} >>\nstream\n{contenido}\nendstream");
            }

            long inicioXref = ms.Position;
            Escribir($"xref\n0 {offsets.Count + 1}\n0000000000 65535 f \n");
            foreach (long o in offsets) Escribir($"{o:D10} 00000 n \n");
            Escribir($"trailer\n<< /Size {offsets.Count + 1} /Root 1 0 R >>\nstartxref\n{inicioXref}\n%%EOF\n");

            return ms.ToArray();
        }

        private static string Escapar(string s) =>
            s.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");

        private static string Dinero(decimal valor) => valor.ToString("N2", Cultura);

        private static string Centrar(string s) => s.PadLeft((Ancho + s.Length) / 2);

        private static string Total(string etiqueta, decimal valor) =>
            $"{etiqueta,50} {Dinero(valor),13}";
    }
}
