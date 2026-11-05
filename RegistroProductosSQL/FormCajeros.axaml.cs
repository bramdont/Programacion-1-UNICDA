using Avalonia.Controls;
using Avalonia.Interactivity;
using RegistroProductosSQL.Clases;
using RegistroProductosSQL.Modelo;

namespace RegistroProductosSQL
{
    public partial class FormCajeros : Window
    {
        private MetodosUsuario metodos = new MetodosUsuario();

        public FormCajeros()
        {
            InitializeComponent();
        }

        private void FormCajeros_Opened(object? sender, EventArgs e)
        {
            // Solo el administrador crea cajeros
            if (Variables.Estado != Variables.Administrador)
            {
                Close();
                return;
            }

            cbTurno.ItemsSource = Turnos.Lista.Select(t => $"{t} ({Turnos.Horario(t)})").ToList();
            CargarCajeros();
        }

        private async void CargarCajeros()
        {
            try
            {
                List<Cajero> cajeros = Cajero.Desde(metodos.ListarCajeros());
                dgvCajeros.ItemsSource = cajeros;
                lblTotalRegistros.Text = $"Cajeros: {cajeros.Count}";
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"Error al cargar cajeros: {ex.Message}", "Error");
            }
        }

        private void LimpiarCampos()
        {
            txtCedula.Clear();
            txtNombres.Clear();
            txtApellidos.Clear();
            txtTelefono.Clear();
            txtEmail.Clear();
            txtUsuario.Clear();
            txtClave.Clear();
            cbTurno.SelectedIndex = -1;
            txtCedula.Focus();
        }

        private async void btnCrear_Click(object? sender, RoutedEventArgs e)
        {
            string cedula = txtCedula.Text?.Trim() ?? string.Empty;
            string nombres = txtNombres.Text?.Trim() ?? string.Empty;
            string apellidos = txtApellidos.Text?.Trim() ?? string.Empty;
            string usuario = txtUsuario.Text?.Trim() ?? string.Empty;
            string clave = txtClave.Text?.Trim() ?? string.Empty;

            if (cedula == "" || nombres == "" || apellidos == "" || usuario == "" || clave == "")
            {
                await Mensajes.Mostrar(this, "Complete cédula, nombres, apellidos, usuario y contraseña.", "Atención");
                return;
            }

            if (cbTurno.SelectedIndex < 0)
            {
                await Mensajes.Mostrar(this, "Seleccione el turno del cajero.", "Atención");
                return;
            }

            string turno = Turnos.Lista[cbTurno.SelectedIndex];

            try
            {
                if (metodos.CrearCajero(cedula, nombres, apellidos, txtTelefono.Text?.Trim() ?? "",
                                        txtEmail.Text?.Trim() ?? "", usuario, clave, turno))
                {
                    await Mensajes.Mostrar(this, $"Cajero creado: {usuario} (turno {turno}).", "Éxito");
                    LimpiarCampos();
                    CargarCajeros();
                }
            }
            catch (Exception ex)
            {
                // Por ejemplo: usuario o cédula repetidos
                await Mensajes.Mostrar(this, $"No se pudo crear el cajero: {ex.Message}", "Error");
            }
        }

        private void btnLimpiar_Click(object? sender, RoutedEventArgs e)
        {
            LimpiarCampos();
        }

        private void btnCerrar_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
