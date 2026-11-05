using Avalonia.Controls;
using Avalonia.Interactivity;
using RegistroProductosSQL.Clases;

namespace RegistroProductosSQL
{
    public partial class FormCajero : Window
    {
        public FormCajero()
        {
            InitializeComponent();
        }

        private void FormCajero_Opened(object? sender, EventArgs e)
        {
            lblUsuario.Text = $"Cajero: {Variables.NombreUsuario}";
            MostrarTurno();
        }

        private void MostrarTurno()
        {
            string estado = Turnos.EnTurno(Variables.Turno, DateTime.Now) ? "dentro de turno" : "fuera de turno";
            lblTurno.Text = $"Turno {Variables.Turno} ({Turnos.Horario(Variables.Turno)}) - {estado}";
        }

        private async void btnFacturar_Click(object? sender, RoutedEventArgs e)
        {
            MostrarTurno();

            // El cajero solo puede facturar dentro de su turno
            if (!Turnos.EnTurno(Variables.Turno, DateTime.Now))
            {
                await Mensajes.Mostrar(this,
                    $"Está fuera de su turno. Su turno es {Variables.Turno} ({Turnos.Horario(Variables.Turno)}).",
                    "Fuera de turno");
                return;
            }

            await new FormFactura().ShowDialog(this);
        }

        private void btnCerrarSesion_Click(object? sender, RoutedEventArgs e)
        {
            Navegacion.CerrarSesion(this);
        }
    }
}
