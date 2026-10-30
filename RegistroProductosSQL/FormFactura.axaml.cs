using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using RegistroProductosSQL.Clases;
using RegistroProductosSQL.Modelo;

namespace RegistroProductosSQL
{
    public partial class FormFactura : Window
    {
        private MetodosProducto metodosProducto = new MetodosProducto();
        private MetodosFactura metodosFactura = new MetodosFactura();
        private ObservableCollection<LineaFactura> lineas = new ObservableCollection<LineaFactura>();
        private Factura? facturaGuardada;

        public FormFactura()
        {
            InitializeComponent();
            dgvLineas.ItemsSource = lineas;
            lineas.CollectionChanged += (_, _) => ActualizarTotales();
            KeyDown += FormFactura_KeyDown;
        }

        private void FormFactura_Opened(object? sender, EventArgs e)
        {
            CargarProductos();
        }

        private async void CargarProductos()
        {
            try
            {
                // Solo se pueden vender artículos con existencias
                cbProducto.ItemsSource = Producto.Desde(metodosProducto.ListarProductos())
                    .Where(p => p.Stock > 0)
                    .ToList();
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"Error al cargar productos: {ex.Message}", "Error");
            }
        }

        private Factura ConstruirFactura() => new Factura
        {
            Fecha = DateTime.Now,
            Vendedor = Variables.NombreUsuario,
            ClienteNombre = txtCliente.Text?.Trim() ?? string.Empty,
            ClienteCedula = txtCedula.Text?.Trim() ?? string.Empty,
            Lineas = lineas.ToList()
        };

        private void ActualizarTotales()
        {
            Factura f = ConstruirFactura();
            lblSubtotal.Text = $"Subtotal: {f.Subtotal:N2}";
            lblItbis.Text = $"ITBIS ({Factura.ItbisPorcentaje:0.##}%): {f.Itbis:N2}";
            lblTotal.Text = $"TOTAL: {f.Total:N2}";
        }

        private void BloquearEdicion(bool bloquear)
        {
            panelEdicion.IsEnabled = !bloquear;
            btnGuardar.IsEnabled = !bloquear;
        }

        private async void btnAgregar_Click(object? sender, RoutedEventArgs e)
        {
            if (cbProducto.SelectedItem is not Producto producto)
            {
                await Mensajes.Mostrar(this, "Seleccione un artículo.", "Atención");
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                await Mensajes.Mostrar(this, "Ingrese una cantidad válida.", "Error de Formato");
                return;
            }

            int indice = lineas.ToList().FindIndex(l => l.IdProducto == producto.Codigo);
            int yaEnFactura = indice >= 0 ? lineas[indice].Cantidad : 0;

            if (yaEnFactura + cantidad > producto.Stock)
            {
                await Mensajes.Mostrar(this, $"Stock insuficiente. Disponible: {producto.Stock}, ya en la factura: {yaEnFactura}.", "Atención");
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
            if (string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                await Mensajes.Mostrar(this, "Ingrese el nombre del cliente.", "Atención");
                return;
            }

            if (lineas.Count == 0)
            {
                await Mensajes.Mostrar(this, "Agregue al menos un artículo a la factura.", "Atención");
                return;
            }

            Factura factura = ConstruirFactura();

            try
            {
                factura.Numero = metodosFactura.GuardarFactura(factura);
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"No se pudo guardar la factura: {ex.Message}", "Error");
                return;
            }

            facturaGuardada = factura;
            BloquearEdicion(true);
            lblNumero.Text = $"Factura No. {factura.Numero:D6}";
            await Mensajes.Mostrar(this, "La factura se guardó correctamente. Presione F5 para imprimirla.", "Éxito");
        }

        private async void FormFactura_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                e.Handled = true;
                await Imprimir();
            }
        }

        private async void btnImprimir_Click(object? sender, RoutedEventArgs e)
        {
            await Imprimir();
        }

        private async Task Imprimir()
        {
            if (facturaGuardada == null)
            {
                await Mensajes.Mostrar(this, "Guarde la factura antes de imprimirla.", "Atención");
                return;
            }

            var archivo = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Imprimir factura",
                SuggestedFileName = $"Factura-{facturaGuardada.Numero:D6}",
                DefaultExtension = "pdf",
                ShowOverwritePrompt = true,
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Documento PDF") { Patterns = new[] { "*.pdf" } },
                    new FilePickerFileType("Archivo de texto") { Patterns = new[] { "*.txt" } }
                }
            });

            string? ruta = archivo?.TryGetLocalPath();
            if (ruta == null) return; // el usuario canceló

            string extension = Path.GetExtension(ruta).ToLowerInvariant();
            if (extension != ".pdf" && extension != ".txt") ruta += ".pdf";

            try
            {
                ImpresionFactura.Guardar(facturaGuardada, ruta);
                await Mensajes.Mostrar(this, $"Factura guardada en:\n{ruta}", "Factura impresa");
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"No se pudo escribir el archivo: {ex.Message}", "Error");
            }
        }

        private void btnNueva_Click(object? sender, RoutedEventArgs e)
        {
            lineas.Clear();
            txtCliente.Clear();
            txtCedula.Clear();
            txtCantidad.Text = "1";
            facturaGuardada = null;
            lblNumero.Text = "Factura nueva (sin guardar)";
            BloquearEdicion(false);
            CargarProductos(); // el stock cambió con la venta anterior
        }

        private void btnCerrar_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
