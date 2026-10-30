using Avalonia.Controls;
using Avalonia.Interactivity;
using RegistroProductosSQL.Clases;
using RegistroProductosSQL.Modelo;

namespace RegistroProductosSQL
{
    public partial class FormProductos : Window
    {
        private MetodosProducto metodos = new MetodosProducto();

        public FormProductos()
        {
            InitializeComponent();
        }

        private void FormProductos_Opened(object? sender, EventArgs e)
        {
            lblUsuarioLogueado.Text = $"Usuario: {Variables.NombreUsuario} ({Variables.Estado})";
            cbCriterio.SelectedIndex = 0;

            ValidarPermisos();
            CargarProductos();
        }

        private void ValidarPermisos()
        {
            // Si el estado es Invitado, se inhabilitan los botones de modificar datos
            if (Variables.Estado.Equals("Invitado", StringComparison.OrdinalIgnoreCase))
            {
                btnNuevo.IsEnabled = false;
                btnInsertar.IsEnabled = false;
                btnActualizar.IsEnabled = false;
                btnEliminar.IsEnabled = false;
            }
        }

        private void MostrarProductos(List<Producto> productos)
        {
            dgvProductos.ItemsSource = productos;
            lblTotalRegistros.Text = $"Registros encontrados: {productos.Count}";
        }

        private async void CargarProductos()
        {
            try
            {
                MostrarProductos(Producto.Desde(metodos.ListarProductos()));
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"Error al cargar productos: {ex.Message}", "Error");
            }
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            txtMarca.Clear();
            txtNombre.Focus();
        }

        private void btnNuevo_Click(object? sender, RoutedEventArgs e)
        {
            LimpiarCampos();
        }

        private async void btnInsertar_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtPrecio.Text) ||
                string.IsNullOrEmpty(txtStock.Text) || string.IsNullOrEmpty(txtMarca.Text))
            {
                await Mensajes.Mostrar(this, "Complete todos los campos obligatorios.", "Atención");
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || !int.TryParse(txtStock.Text, out int stock))
            {
                await Mensajes.Mostrar(this, "Precio o Stock ingresados no son válidos.", "Error de Formato");
                return;
            }

            if (metodos.InsertarProducto(txtNombre.Text.Trim(), precio, stock, txtMarca.Text.Trim()))
            {
                await Mensajes.Mostrar(this, "Los datos se guardaron correctamente.", "Éxito");
                LimpiarCampos();
                CargarProductos();
            }
        }

        private async void btnActualizar_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCodigo.Text))
            {
                await Mensajes.Mostrar(this, "Seleccione un producto para actualizar.", "Atención");
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || !int.TryParse(txtStock.Text, out int stock))
            {
                await Mensajes.Mostrar(this, "Precio o Stock ingresados no son válidos.", "Error de Formato");
                return;
            }

            int code = Convert.ToInt32(txtCodigo.Text);

            if (metodos.ActualizarProducto(code, txtNombre.Text?.Trim() ?? "", precio, stock, txtMarca.Text?.Trim() ?? ""))
            {
                await Mensajes.Mostrar(this, "Los datos se actualizaron correctamente.", "Éxito");
                LimpiarCampos();
                CargarProductos();
            }
        }

        private async void btnEliminar_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCodigo.Text))
            {
                await Mensajes.Mostrar(this, "Seleccione un producto para eliminar.", "Atención");
                return;
            }

            if (await Mensajes.Confirmar(this, "¿Desea eliminar el registro seleccionado?", "Confirmar Eliminación"))
            {
                int code = Convert.ToInt32(txtCodigo.Text);
                if (metodos.EliminarProducto(code))
                {
                    await Mensajes.Mostrar(this, "Registro eliminado correctamente.", "Éxito");
                    LimpiarCampos();
                    CargarProductos();
                }
            }
        }

        private async void btnBuscar_Click(object? sender, RoutedEventArgs e)
        {
            string criterio = (cbCriterio.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Nombre";
            string valor = txtBuscar.Text?.Trim() ?? string.Empty;

            try
            {
                MostrarProductos(Producto.Desde(metodos.BuscarProducto(criterio, valor)));
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"Error al buscar productos: {ex.Message}", "Error");
            }
        }

        private void dgvProductos_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (dgvProductos.SelectedItem is Producto p)
            {
                txtCodigo.Text = p.Codigo.ToString();
                txtNombre.Text = p.Nombre;
                txtPrecio.Text = p.Precio.ToString();
                txtStock.Text = p.Stock.ToString();
                txtMarca.Text = p.Marca;
            }
        }

        private void btnLimpiar_Click(object? sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            CargarProductos();
        }

        private async void btnFacturar_Click(object? sender, RoutedEventArgs e)
        {
            await new FormFactura().ShowDialog(this);
            //CargarProductos(); // las ventas descuentan stock
        }

        private void btnSalir_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
