using System;
using Avalonia;

namespace UsuarioAvalonia
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        /// <summary>
        /// Configura la aplicación Avalonia (equivalente a Application.Run de Windows Forms).
        /// </summary>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
