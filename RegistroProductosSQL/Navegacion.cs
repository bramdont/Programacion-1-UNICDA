using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using RegistroProductosSQL.Clases;

namespace RegistroProductosSQL
{
    // Cambio entre ventanas principales (login, menú de administrador, menú de cajero)
    public static class Navegacion
    {
        // Avalonia no permite ShowDialog con un dueño oculto: la ventana nueva pasa
        // a ser la principal y luego se cierra la actual
        public static void IrA(Window actual, Window nueva)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = nueva;
            }
            nueva.Show();
            actual.Close();
        }

        public static void CerrarSesion(Window actual)
        {
            Variables.CerrarSesion();
            IrA(actual, new FormLogin());
        }
    }
}
