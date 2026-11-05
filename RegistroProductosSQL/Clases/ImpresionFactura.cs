using System;
using System.Globalization;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RegistroProductosSQL.Modelo;

namespace RegistroProductosSQL.Clases
{
    public static class ImpresionFactura
    {
        private static readonly CultureInfo Cultura = CultureInfo.InvariantCulture;

        // Carpeta donde se guardan los PDF: Documentos/Facturas
        public static string Carpeta =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Facturas");

        // El nombre del archivo se basa en el número: Factura-00001.pdf
        public static string RutaPdf(Factura f) =>
            Path.Combine(Carpeta, $"Factura-{f.NumeroTexto}.pdf");

        public static string GuardarPdf(Factura f)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            Directory.CreateDirectory(Carpeta);
            string ruta = RutaPdf(f);

            Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(40);
                    pagina.DefaultTextStyle(x => x.FontSize(10));

                    pagina.Header().Element(c => Encabezado(c, f));
                    pagina.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Spacing(15);
                        col.Item().Element(c => Detalle(c, f));
                        col.Item().AlignRight().Element(c => Totales(c, f));
                    });
                    pagina.Footer().AlignCenter().Text(t =>
                    {
                        t.Span("Gracias por su compra  -  Página ");
                        t.CurrentPageNumber();
                        t.Span(" de ");
                        t.TotalPages();
                    });
                });
            }).GeneratePdf(ruta);

            return ruta;
        }

        private static void Encabezado(IContainer contenedor, Factura f)
        {
            contenedor.Column(col =>
            {
                col.Item().Row(fila =>
                {
                    fila.RelativeItem().Text(Factura.Titulo).FontSize(26).Bold();
                    fila.ConstantItem(150).AlignRight().Text($"No. {f.NumeroTexto}").FontSize(20).Bold();
                });

                col.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Medium).Padding(8).Column(datos =>
                {
                    datos.Spacing(3);
                    datos.Item().Row(fila =>
                    {
                        fila.RelativeItem().Text(t => Campo(t, "Fecha: ", f.Fecha.ToString("dd/MM/yyyy")));
                        fila.RelativeItem().Text(t => Campo(t, "Hora: ", f.Fecha.ToString("HH:mm")));
                        fila.RelativeItem().Text(t => Campo(t, "Tipo: ", f.TipoTexto));
                    });
                    datos.Item().Text(t => Campo(t, "Cajero: ", f.Cajero));
                    datos.Item().Row(fila =>
                    {
                        fila.RelativeItem(2).Text(t => Campo(t, "Cliente: ", f.ClienteNombre));
                        fila.RelativeItem(1).Text(t => Campo(t, "Cédula: ", f.ClienteCedula));
                    });
                });
            });
        }

        private static void Campo(TextDescriptor texto, string etiqueta, string valor)
        {
            texto.Span(etiqueta).Bold();
            texto.Span(valor);
        }

        private static void Detalle(IContainer contenedor, Factura f)
        {
            contenedor.Table(tabla =>
            {
                tabla.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(40);   // Id
                    c.RelativeColumn();     // Producto
                    c.ConstantColumn(40);   // Cantidad
                    c.ConstantColumn(75);   // Precio
                    c.ConstantColumn(80);   // Valor
                    c.ConstantColumn(70);   // ITBIS
                });

                tabla.Header(h =>
                {
                    h.Cell().Element(Cabecera).Text("Id");
                    h.Cell().Element(Cabecera).Text("Producto");
                    h.Cell().Element(Cabecera).AlignRight().Text("Cant.");
                    h.Cell().Element(Cabecera).AlignRight().Text("Precio");
                    h.Cell().Element(Cabecera).AlignRight().Text("Valor");
                    h.Cell().Element(Cabecera).AlignRight().Text("ITBIS");
                });

                foreach (LineaFactura l in f.Lineas)
                {
                    tabla.Cell().Element(Celda).Text(l.IdProducto.ToString());
                    tabla.Cell().Element(Celda).Text(l.Nombre);
                    tabla.Cell().Element(Celda).AlignRight().Text(l.Cantidad.ToString());
                    tabla.Cell().Element(Celda).AlignRight().Text(Dinero(l.PrecioUnitario));
                    tabla.Cell().Element(Celda).AlignRight().Text(Dinero(l.Subtotal));
                    tabla.Cell().Element(Celda).AlignRight().Text(Dinero(l.Itbis));
                }
            });
        }

        private static IContainer Cabecera(IContainer c) =>
            c.Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Grey.Darken1)
             .Padding(5).DefaultTextStyle(x => x.Bold());

        private static IContainer Celda(IContainer c) =>
            c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);

        private static void Totales(IContainer contenedor, Factura f)
        {
            contenedor.Width(260).Column(col =>
            {
                col.Spacing(3);
                col.Item().Element(c => FilaTotal(c, "Total bruto", f.TotalBruto));
                col.Item().Element(c => FilaTotal(c, $"ITBIS ({Factura.ItbisPorcentaje:0.##}%)", f.Itbis));
                col.Item().BorderTop(1).PaddingTop(4).Element(c => FilaTotal(c, "TOTAL A PAGAR", f.Total, true));

                if (f.EsCredito)
                {
                    col.Item().PaddingTop(8).Text("Factura a crédito: registrada en cuentas por cobrar.").Italic();
                }
                else
                {
                    col.Item().PaddingTop(8).Element(c => FilaTotal(c, "Monto pagado", f.MontoPagado));
                    col.Item().Element(c => FilaTotal(c, "Devuelta", f.Devuelta));
                }
            });
        }

        private static void FilaTotal(IContainer contenedor, string etiqueta, decimal valor, bool resaltar = false)
        {
            contenedor.Row(fila =>
            {
                var etiquetaTexto = fila.RelativeItem().Text(etiqueta);
                var valorTexto = fila.ConstantItem(100).AlignRight().Text(Dinero(valor));
                if (resaltar)
                {
                    etiquetaTexto.Bold().FontSize(12);
                    valorTexto.Bold().FontSize(12);
                }
            });
        }

        private static string Dinero(decimal valor) => valor.ToString("N2", Cultura);
    }
}
