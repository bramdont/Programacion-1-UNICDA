using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace RegistroProductosSQL
{
    // Reemplazo simple de MessageBox (Avalonia no incluye uno)
    public static class Mensajes
    {
        public static Task Mostrar(Window owner, string texto, string titulo) =>
            Dialogo(owner, texto, titulo, false);

        public static Task<bool> Confirmar(Window owner, string texto, string titulo) =>
            Dialogo(owner, texto, titulo, true);

        private static async Task<bool> Dialogo(Window owner, string texto, string titulo, bool confirmar)
        {
            var dialogo = new Window
            {
                Title = titulo,
                Width = 380,
                SizeToContent = SizeToContent.Height,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var botones = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 8
            };

            var aceptar = new Button { Content = confirmar ? "Sí" : "Aceptar", MinWidth = 80 };
            aceptar.Click += (_, _) => dialogo.Close(true);
            botones.Children.Add(aceptar);

            if (confirmar)
            {
                var cancelar = new Button { Content = "No", MinWidth = 80 };
                cancelar.Click += (_, _) => dialogo.Close(false);
                botones.Children.Add(cancelar);
            }

            dialogo.Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 20,
                Children =
                {
                    new TextBlock { Text = texto, TextWrapping = TextWrapping.Wrap },
                    botones
                }
            };

            return await dialogo.ShowDialog<bool>(owner);
        }
    }
}
