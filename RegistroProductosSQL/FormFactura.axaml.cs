using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using RegistroProductosSQL.Clases;
using RegistroProductosSQL.Modelo;

namespace RegistroProductosSQL
{
    public partial class FormFactura : Window
    {
        private MetodosProducto metodosProducto = new MetodosProducto();
        private MetodosFactura metodosFactura = new MetodosFactura();
        private ObservableCollection<LineaFactura> lineas = new ObservableCollection<LineaFactura>();
        private List<Producto> productos = new List<Producto>();
        private Factura? facturaGuardada;
        private string? rutaPdf;

        public FormFactura()
        {
            InitializeComponent();
            dgvLineas.ItemsSource = lineas;
            lineas.CollectionChanged += (_, _) => ActualizarTotales();
            KeyDown += FormFactura_KeyDown;
        }

        private async void FormFactura_Opened(object? sender, EventArgs e)
        {
            // Solo los cajeros facturan
            if (Variables.Estado != Variables.Cajero)
            {
                await Mensajes.Mostrar(this, "Solo los cajeros pueden facturar.", "Acceso denegado");
                Close();
                return;
            }

            lblCajero.Text = $"Cajero: {Variables.NombreUsuario} - Turno {Variables.Turno} ({Turnos.Horario(Variables.Turno)})";
            cbTipo.SelectedIndex = 0;
            CargarProductos();
        }

        private async void CargarProductos()
        {
            try
            {
                productos = Producto.Desde(metodosProducto.ListarProductos());
                // En la lista solo se muestran artículos con existencias
                cbProducto.ItemsSource = productos.Where(p => p.Stock > 0).ToList();
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"Error al cargar productos: {ex.Message}", "Error");
            }
        }

        private bool EsCredito => cbTipo.SelectedIndex == 1;

        private Factura ConstruirFactura()
        {
            var factura = new Factura
            {
                Fecha = DateTime.Now,
                Tipo = EsCredito ? Factura.TipoCredito : Factura.TipoContado,
                Cajero = Variables.NombreUsuario,
                ClienteNombre = txtCliente.Text?.Trim() ?? string.Empty,
                ClienteCedula = txtCedula.Text?.Trim() ?? string.Empty,
                Lineas = lineas.ToList()
            };

            if (!factura.EsCredito && decimal.TryParse(txtPagado.Text, out decimal pagado))
            {
                factura.MontoPagado = Math.Round(pagado, 2);
                factura.Devuelta = Math.Max(0, factura.MontoPagado - factura.Total);
            }
            return factura;
        }

        private void ActualizarTotales()
        {
            Factura f = ConstruirFactura();
            lblBruto.Text = $"Total bruto: {f.TotalBruto:N2}";
            lblItbis.Text = $"ITBIS ({Factura.ItbisPorcentaje:0.##}%): {f.Itbis:N2}";
            lblTotal.Text = $"TOTAL A PAGAR: {f.Total:N2}";

            if (f.EsCredito)
            {
                lblDevuelta.Text = "A crédito: queda en cuentas por cobrar";
            }
            else if (f.MontoPagado < f.Total)
            {
                lblDevuelta.Text = $"Falta: {f.Total - f.MontoPagado:N2}";
            }
            else
            {
                lblDevuelta.Text = $"Devuelta: {f.Devuelta:N2}";
            }
        }

        private void cbTipo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (lblDevuelta == null) return; // aún se está creando la ventana
            txtPagado.IsEnabled = !EsCredito;
            if (EsCredito) txtPagado.Clear();
            ActualizarTotales();
        }

        private void txtPagado_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (lblDevuelta == null) return;
            ActualizarTotales();
        }

        private void BloquearEdicion(bool bloquear)
        {
            panelEdicion.IsEnabled = !bloquear;
            panelPago.IsEnabled = !bloquear;
            btnGuardar.IsEnabled = !bloquear;
        }

        private async void btnAgregar_Click(object? sender, RoutedEventArgs e)
        {
            Producto? producto;
            string codigoTexto = txtCodigo.Text?.Trim() ?? string.Empty;

            if (codigoTexto != string.Empty)
            {
                if (!int.TryParse(codigoTexto, out int codigo))
                {
                    await Mensajes.Mostrar(this, "El código del producto debe ser un número.", "Error de Formato");
                    return;
                }

                producto = productos.FirstOrDefault(p => p.Codigo == codigo);
                if (producto == null)
                {
                    await Mensajes.Mostrar(this, $"No existe ningún producto con el código {codigo}.", "Producto inexistente");
                    return;
                }
            }
            else if (cbProducto.SelectedItem is Producto seleccionado)
            {
                producto = seleccionado;
            }
            else
            {
                await Mensajes.Mostrar(this, "Seleccione un artículo o escriba su código.", "Atención");
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                await Mensajes.Mostrar(this, "Ingrese una cantidad válida (un número entero mayor que cero).", "Error de Formato");
                return;
            }

            int indice = lineas.ToList().FindIndex(l => l.IdProducto == producto.Codigo);
            int yaEnFactura = indice >= 0 ? lineas[indice].Cantidad : 0;

            if (yaEnFactura + cantidad > producto.Stock)
            {
                await Mensajes.Mostrar(this, $"Stock insuficiente de {producto.Nombre}. Disponible: {producto.Stock}, ya en la factura: {yaEnFactura}.", "Atención");
                return;
            }

            var linea = new LineaFactura
            {
                IdProducto = producto.Codigo,
                Nombre = producto.Nombre,
                Cantidad = yaEnFactura + cantidad,
                PrecioUnitario = producto.Precio
            };

            if (indice >= 0) lineas[indice] = linea;
            else lineas.Add(linea);

            txtCodigo.Clear();
            txtCantidad.Text = "1";
        }

        private async void btnQuitar_Click(object? sender, RoutedEventArgs e)
        {
            if (dgvLineas.SelectedItem is LineaFactura linea)
            {
                lineas.Remove(linea);
            }
            else
            {
                await Mensajes.Mostrar(this, "Seleccione una línea para quitar.", "Atención");
            }
        }

        private async void btnGuardar_Click(object? sender, RoutedEventArgs e)
        {
            // El turno se vuelve a comprobar al guardar: pudo terminar mientras se facturaba
            if (!Turnos.EnTurno(Variables.Turno, DateTime.Now))
            {
                await Mensajes.Mostrar(this, $"Está fuera de su turno ({Variables.Turno}: {Turnos.Horario(Variables.Turno)}). No puede facturar.", "Fuera de turno");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCliente.Text) || string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                await Mensajes.Mostrar(this, "Ingrese el nombre y la cédula del cliente.", "Atención");
                return;
            }

            if (lineas.Count == 0)
            {
                await Mensajes.Mostrar(this, "Agregue al menos un artículo a la factura.", "Atención");
                return;
            }

            if (!EsCredito)
            {
                if (!decimal.TryParse(txtPagado.Text, out decimal pagado) || pagado < 0)
                {
                    await Mensajes.Mostrar(this, "Ingrese un monto pagado válido.", "Error de Formato");
                    return;
                }

                if (Math.Round(pagado, 2) < ConstruirFactura().Total)
                {
                    await Mensajes.Mostrar(this, "El monto pagado no cubre el total a pagar.", "Pago insuficiente");
                    return;
                }
            }

            Factura factura = ConstruirFactura();

            try
            {
                metodosFactura.GuardarFactura(factura); // asigna factura.Numero
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"No se pudo guardar la factura: {ex.Message}", "Error");
                CargarProductos(); // el stock pudo cambiar
                return;
            }

            facturaGuardada = factura;
            BloquearEdicion(true);
            lblNumero.Text = $"Factura No. {factura.NumeroTexto}";

            string resumen = factura.EsCredito
                ? $"Factura {factura.NumeroTexto} a crédito por {factura.Total:N2}.\nRegistrada en cuentas por cobrar de {factura.ClienteNombre}."
                : $"Factura {factura.NumeroTexto} al contado.\nA pagar: {factura.Total:N2}   Pagado: {factura.MontoPagado:N2}   Devuelta: {factura.Devuelta:N2}";

            try
            {
                rutaPdf = ImpresionFactura.GuardarPdf(factura);
                await Mensajes.Mostrar(this, $"{resumen}\n\nPDF guardado en:\n{rutaPdf}", "Factura guardada");
                AbrirPdf();
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"{resumen}\n\nLa factura se guardó, pero no se pudo crear el PDF: {ex.Message}", "Factura guardada");
            }
        }

        private async void FormFactura_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                e.Handled = true;
                await ReabrirPdf();
            }
        }

        private async void btnImprimir_Click(object? sender, RoutedEventArgs e)
        {
            await ReabrirPdf();
        }

        private async Task ReabrirPdf()
        {
            if (facturaGuardada == null)
            {
                await Mensajes.Mostrar(this, "Guarde la factura para generar su PDF.", "Atención");
                return;
            }

            try
            {
                // Si el archivo se borró, se vuelve a crear
                if (rutaPdf == null || !File.Exists(rutaPdf)) rutaPdf = ImpresionFactura.GuardarPdf(facturaGuardada);
                AbrirPdf();
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"No se pudo abrir el PDF: {ex.Message}", "Error");
            }
        }

        // Abre el PDF con el visor predeterminado del sistema
        private void AbrirPdf()
        {
            if (rutaPdf == null) return;
            Process.Start(new ProcessStartInfo(rutaPdf) { UseShellExecute = true });
        }

        private void btnNueva_Click(object? sender, RoutedEventArgs e)
        {
            lineas.Clear();
            txtCliente.Clear();
            txtCedula.Clear();
            txtCodigo.Clear();
            txtPagado.Clear();
            txtCantidad.Text = "1";
            cbTipo.SelectedIndex = 0;
            facturaGuardada = null;
            rutaPdf = null;
            lblNumero.Text = "Factura nueva (el número se asigna al guardar)";
            BloquearEdicion(false);
            CargarProductos(); // el stock cambió con la venta anterior
        }

        private void btnCerrar_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
