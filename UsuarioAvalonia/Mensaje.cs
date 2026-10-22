using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;

namespace UsuarioAvalonia
{
    /// <summary>
    /// Ventana de aviso sencilla. Avalonia no trae un MessageBox como Windows Forms,
    /// así que esta clase cumple esa función.
    /// </summary>
    public static class Mensaje
    {
        public static Task Mostrar(Window owner, string texto, string titulo)
        {
            var btnAceptar = new Button
            {
                Content = "Aceptar",
                Width = 90,
                HorizontalAlignment = HorizontalAlignment.Right,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            var ventana = new Window
            {
                Title = titulo,
                Width = 340,
                SizeToContent = SizeToContent.Height,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Spacing = 16,
                    Children =
                    {
                        new TextBlock { Text = texto, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                        btnAceptar
                    }
                }
            };

            btnAceptar.Click += (_, _) => ventana.Close();

            return ventana.ShowDialog(owner);
        }

        /// <summary>
        /// Ventana de confirmación con botones Sí / No (equivalente a MessageBox con YesNo).
        /// </summary>
        /// <returns>true si el usuario presiona Sí</returns>
        public static async Task<bool> Confirmar(Window owner, string texto, string titulo)
        {
            bool respuesta = false;

            var btnSi = new Button { Content = "Sí", Width = 90, HorizontalContentAlignment = HorizontalAlignment.Center };
            var btnNo = new Button { Content = "No", Width = 90, HorizontalContentAlignment = HorizontalAlignment.Center };

            var ventana = new Window
            {
                Title = titulo,
                Width = 340,
                SizeToContent = SizeToContent.Height,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Spacing = 16,
                    Children =
                    {
                        new TextBlock { Text = texto, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Spacing = 8,
                            Children = { btnSi, btnNo }
                        }
                    }
                }
            };

            btnSi.Click += (_, _) => { respuesta = true; ventana.Close(); };
            btnNo.Click += (_, _) => ventana.Close();

            await ventana.ShowDialog(owner);
            return respuesta;
        }
    }
}
