namespace RegistroProductosSQL.Clases
{
    public static class Variables
    {
        // Valores posibles de Usuario.estado
        public const string Administrador = "Administrador";
        public const string Cajero = "Cajero";
        public const string Invitado = "Invitado";

        public static int IdUsuario { get; set; }
        public static string NombreUsuario { get; set; } = string.Empty;
        public static string Estado { get; set; } = string.Empty; // Administrador, Cajero o Invitado
        public static string Turno { get; set; } = string.Empty;  // solo cajeros: Mañana, Tarde o Noche

        public static void CerrarSesion()
        {
            IdUsuario = 0;
            NombreUsuario = string.Empty;
            Estado = string.Empty;
            Turno = string.Empty;
        }
    }
}
