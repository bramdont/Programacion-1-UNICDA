using System.Data;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using RegistroProductosSQL.Clases;
using RegistroProductosSQL.Modelo;

namespace RegistroProductosSQL
{
    public partial class FormReportes : Window
    {
        private MetodosReporte metodos = new MetodosReporte();

        public FormReportes()
        {
            InitializeComponent();
        }

        private void FormReportes_Opened(object? sender, EventArgs e)
        {
            // Los reportes son solo para el administrador
            if (Variables.Estado != Variables.Administrador)
            {
                Close();
                return;
            }

            cbTipo.SelectedIndex = 0;
            cbReporte.SelectedIndex = 0;
        }

        // Muestra solo los filtros que usa cada reporte
        private void cbReporte_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            int reporte = cbReporte.SelectedIndex;
            bool usaFechas = reporte is 0 or 1 or 3 or 4;

            panelDesde.IsVisible = usaFechas;
            panelHasta.IsVisible = usaFechas && reporte != 0; // el cuadre es de un solo día
            panelTipo.IsVisible = reporte == 3;
            lblDesde.Text = reporte == 0 ? "Fecha:" : "Desde:";

            DateTime hoy = DateTime.Today;
            dpDesde.SelectedDate = reporte == 0 ? hoy : new DateTime(hoy.Year, hoy.Month, 1);
            dpHasta.SelectedDate = hoy;
        }

        private async void btnGenerar_Click(object? sender, RoutedEventArgs e)
        {
            DateTime desde = dpDesde.SelectedDate?.Date ?? DateTime.Today;
            DateTime hasta = dpHasta.SelectedDate?.Date ?? DateTime.Today;
            if (cbReporte.SelectedIndex == 0) hasta = desde;

            if (desde > hasta)
            {
                await Mensajes.Mostrar(this, "La fecha inicial no puede ser mayor que la final.", "Atención");
                return;
            }

            try
            {
                DataTable tabla;
                switch (cbReporte.SelectedIndex)
                {
                    case 0:
                        tabla = metodos.CuadreDelDia(desde, hasta);
                        break;
                    case 1:
                        tabla = metodos.VentasPorCajero(desde, hasta);
                        break;
                    case 2:
                        tabla = metodos.Inventario();
                        break;
                    case 3:
                        string? tipo = cbTipo.SelectedIndex switch
                        {
                            1 => Factura.TipoContado,
                            2 => Factura.TipoCredito,
                            _ => null // todas
                        };
                        tabla = metodos.Facturas(desde, hasta, tipo);
                        break;
                    case 4:
                        tabla = metodos.TopClientes(desde, hasta);
                        break;
                    case 5:
                        tabla = metodos.CuentasPorCobrar();
                        break;
                    default:
                        await Mensajes.Mostrar(this, "Seleccione un reporte.", "Atención");
                        return;
                }

                MostrarTabla(tabla);
                lblResumen.Text = $"Registros encontrados: {tabla.Rows.Count}";
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"Error al generar el reporte: {ex.Message}", "Error");
            }
        }

        // Cada reporte tiene columnas distintas: se crean en código a partir del DataTable
        private void MostrarTabla(DataTable tabla)
        {
            dgvReporte.ItemsSource = null;
            dgvReporte.Columns.Clear();

            for (int i = 0; i < tabla.Columns.Count; i++)
            {
                bool esDinero = tabla.Columns[i].DataType == typeof(decimal);
                dgvReporte.Columns.Add(new DataGridTextColumn
                {
                    Header = tabla.Columns[i].ColumnName,
                    Binding = new Binding($"[{i}]") { Mode = BindingMode.OneWay, StringFormat = esDinero ? "N2" : "" },
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });
            }

            dgvReporte.ItemsSource = tabla.Rows.Cast<DataRow>().Select(fila => fila.ItemArray).ToList();
        }

        private void btnCerrar_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
