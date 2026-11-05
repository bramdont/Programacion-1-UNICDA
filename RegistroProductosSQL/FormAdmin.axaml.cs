using Avalonia.Controls;
using Avalonia.Interactivity;
using RegistroProductosSQL.Clases;

namespace RegistroProductosSQL
{
    public partial class FormAdmin : Window
    {
        public FormAdmin()
        {
            InitializeComponent();
        }

        private void FormAdmin_Opened(object? sender, EventArgs e)
        {
            lblUsuario.Text = $"Usuario: {Variables.NombreUsuario} ({Variables.Estado})";
        }

        private async void btnProductos_Click(object? sender, RoutedEventArgs e)
        {
            await new FormProductos().ShowDialog(this);
        }

        private async void btnCajeros_Click(object? sender, RoutedEventArgs e)
        {
            await new FormCajeros().ShowDialog(this);
        }

        private async void btnReportes_Click(object? sender, RoutedEventArgs e)
        {
            await new FormReportes().ShowDialog(this);
        }

        private void btnCerrarSesion_Click(object? sender, RoutedEventArgs e)
        {
            Navegacion.CerrarSesion(this);
        }
    }
}
