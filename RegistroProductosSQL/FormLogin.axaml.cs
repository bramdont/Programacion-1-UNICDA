using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using RegistroProductosSQL.Clases;

namespace RegistroProductosSQL
{
    public partial class FormLogin : Window
    {
        private MetodosProducto metodos = new MetodosProducto();

        public FormLogin()
        {
            InitializeComponent();
        }

        private async void btnIngresar_Click(object? sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text?.Trim() ?? string.Empty;
            string clave = txtClave.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
            {
                await Mensajes.Mostrar(this, "Por favor, ingrese el usuario y la contraseña.", "Atención");
                return;
            }

            try
            {
                if (metodos.ValidarUsuario(usuario, clave))
                {
                    await Mensajes.Mostrar(this, $"Bienvenido al Sistema, {Variables.NombreUsuario}", "Acceso Concedido");

                    // Avalonia no permite ShowDialog con un dueño oculto: la ventana de
                    // productos pasa a ser la principal y luego se cierra el login
                    FormProductos mainForm = new FormProductos();
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.MainWindow = mainForm;
                    }
                    mainForm.Show();
                    this.Close();
                }
                else
                {
                    await Mensajes.Mostrar(this, "El usuario o la contraseña son incorrectos.", "Error de Autenticación");
                    txtClave.Clear();
                    txtUsuario.Focus();
                }
            }
            catch (Exception ex)
            {
                await Mensajes.Mostrar(this, $"Error al conectar con la base de datos: {ex.Message}", "Error de Conexión");
            }
        }

        private void btnSalir_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
