namespace RegistroProductosSQL.Clases
{
    // Turnos de 8 horas. Para cambiar el horario se modifican estas constantes.
    public static class Turnos
    {
        public const string Manana = "Mañana";
        public const string Tarde = "Tarde";
        public const string Noche = "Noche";

        public const int InicioManana = 6;   // 06:00 a 13:59
        public const int InicioTarde = 14;   // 14:00 a 21:59
        public const int InicioNoche = 22;   // 22:00 a 05:59

        public static readonly string[] Lista = { Manana, Tarde, Noche };

        public static string Horario(string turno) => turno switch
        {
            Manana => $"{InicioManana:00}:00 - {InicioTarde:00}:00",
            Tarde => $"{InicioTarde:00}:00 - {InicioNoche:00}:00",
            Noche => $"{InicioNoche:00}:00 - {InicioManana:00}:00",
            _ => "-"
        };

        public static bool EnTurno(string turno, DateTime ahora)
        {
            int hora = ahora.Hour;
            return turno switch
            {
                Manana => hora >= InicioManana && hora < InicioTarde,
                Tarde => hora >= InicioTarde && hora < InicioNoche,
                Noche => hora >= InicioNoche || hora < InicioManana,
                _ => false
            };
        }
    }
}
