using System;
using System.Drawing;
using System.Threading;

namespace JuegoSnake
{
    public class Ventana
    {
        public string Titulo { get; set; }
        public int Ancho { get; set; }
        public int Alto { get; set; }
        public ConsoleColor ColorFondo { get; set; }
        public ConsoleColor ColorLetra { get; set; }
        public Point LimiteSuperior { get; set; }
        public Point LimiteInferior { get; set; }
        public int Area { get; set; }

        public Ventana(string titulo, int ancho, int alto, ConsoleColor colorFondo, 
                       ConsoleColor colorLetra, Point limiteSuperior, Point limiteInferior)
        {
            Titulo = titulo;
            Ancho = ancho;
            Alto = alto;
            ColorFondo = colorFondo;
            ColorLetra = colorLetra;
            LimiteSuperior = limiteSuperior;
            LimiteInferior = limiteInferior;
            Area = (LimiteInferior.X - LimiteSuperior.X - 1) * (LimiteInferior.Y - LimiteSuperior.Y - 1);
            Init();
        }

        private void Init()
        {
            try { Console.SetWindowSize(Ancho, Alto); } catch {}
            Console.Title = Titulo;
            Console.CursorVisible = false;
            Console.BackgroundColor = ColorFondo;
            Console.Clear();
        }

        public void DibujarMarco()
        {
            Console.ForegroundColor = ColorLetra;

            for (int i = LimiteSuperior.X; i <= LimiteInferior.X; i++)
            {
                Console.SetCursorPosition(i, LimiteSuperior.Y);
                Console.Write("═");
                Console.SetCursorPosition(i, LimiteInferior.Y);
                Console.Write("═");
            }

            for (int i = LimiteSuperior.Y; i <= LimiteInferior.Y; i++)
            {
                Console.SetCursorPosition(LimiteSuperior.X, i);
                Console.Write("║");
                Console.SetCursorPosition(LimiteInferior.X, i);
                Console.Write("║");
            }

            Console.SetCursorPosition(LimiteSuperior.X, LimiteSuperior.Y);
            Console.Write("╔");
            Console.SetCursorPosition(LimiteSuperior.X, LimiteInferior.Y);
            Console.Write("╚");
            Console.SetCursorPosition(LimiteInferior.X, LimiteSuperior.Y);
            Console.Write("╗");
            Console.SetCursorPosition(LimiteInferior.X, LimiteInferior.Y);
            Console.Write("╝");
        }

        public void Menu()
        {
            Console.ForegroundColor = ConsoleColor.White;
            int centroX = (LimiteSuperior.X + LimiteInferior.X) / 2;
            int centroY = (LimiteSuperior.Y + LimiteInferior.Y) / 2;

            Console.SetCursorPosition(centroX - 12, centroY - 4);
            Console.Write("J U E G O   S N A K E");

            Console.SetCursorPosition(centroX - 8, centroY - 2);
            Console.Write("ENTER - Jugar");

            Console.SetCursorPosition(centroX - 8, centroY - 1);
            Console.Write("ESCAPE - Salir");
        }

        public void Teclado(ref bool ejecucion, ref bool jugar, Snake snake)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo tecla = Console.ReadKey(true);
                if (tecla.Key == ConsoleKey.Enter)
                {
                    jugar = true;
                    Console.Clear();
                    DibujarMarco();
                    snake.Init();
                }
                else if (tecla.Key == ConsoleKey.Escape)
                {
                    ejecucion = false;
                }
            }
        }

        public void GameOver(string texto)
        {
            Console.Clear();
            DibujarMarco();

            int centroX = (LimiteSuperior.X + LimiteInferior.X) / 2;
            int centroY = (LimiteSuperior.Y + LimiteInferior.Y) / 2;

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(centroX - 10, centroY - 2);
            Console.Write(texto);

            Thread.Sleep(3000);
            Console.Clear();
            DibujarMarco();
        }
    }
}