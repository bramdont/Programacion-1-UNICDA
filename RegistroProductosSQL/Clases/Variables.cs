namespace RegistroProductosSQL.Clases
{
    public static class Variables
    {
        public static int IdUsuario { get; set; }
        public static string NombreUsuario { get; set; } = string.Empty;
        public static string Estado { get; set; } = string.Empty; // "Administrador" o "Invitado"
    }
}
