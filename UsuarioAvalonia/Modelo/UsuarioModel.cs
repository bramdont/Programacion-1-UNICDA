namespace UsuarioAvalonia.Modelo
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }

        /// <summary>
        /// true = activo (1 en la base de datos), false = desactivado (0).
        /// </summary>
        public bool Estatus { get; set; } = true;
    }
}
