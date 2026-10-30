using System;
using System.Drawing;

namespace JuegoSnake
{
    public class Comida
    {
        public Point Posicion { get; set; }
        public ConsoleColor Color { get; set; }
        public Ventana VentanaC { get; set; }

        public Comida(ConsoleColor color, Ventana ventana)
        {
            Color = color;
            VentanaC = ventana;
        }

        public void Dibujar()
        {
            Console.ForegroundColor = Color;
            Console.SetCursorPosition(Posicion.X, Posicion.Y);
            Console.Write("█");
        }

        public bool GenerarComida(Snake snake)
        {
            int longitudSerpiente = snake.Cuerpo.Count + 1;

            if (VentanaC.Area - longitudSerpiente <= 0)
            {
                return false;
            }

            Random random = new Random();
            int x = random.Next(VentanaC.LimiteSuperior.X + 1, VentanaC.LimiteInferior.X);
            int y = random.Next(VentanaC.LimiteSuperior.Y + 1, VentanaC.LimiteInferior.Y);

            Point nuevaPosicion = new Point(x, y);

            foreach (Point parte in snake.Cuerpo)
            {
                if (nuevaPosicion == parte || nuevaPosicion == snake.Cabeza)
                {
                    return GenerarComida(snake);
                }
            }

            Posicion = nuevaPosicion;
            Dibujar();
            return true;
        }
    }
}