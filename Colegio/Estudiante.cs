class Estudiante
{
    public const int CantidadNotas = 4;

    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public int[] Notas { get; set; }

    public Estudiante(string nombre, string apellido, int[] notas)
    {
        Nombre = nombre;
        Apellido = apellido;
        Notas = notas;
    }

    public double Promedio
    {
        get
        {
            int suma = 0;
            foreach (int nota in Notas)
            {
                suma += nota;
            }

            return (double)suma / Notas.Length;
        }
    }

    public string Literal
    {
        get
        {
            double promedio = Promedio;

            if (promedio >= 90)
            {
                return "A";
            }
            else if (promedio >= 80)
            {
                return "B";
            }
            else if (promedio >= 70)
            {
                return "C";
            }
            else
            {
                return "D";
            }
        }
    }
}
