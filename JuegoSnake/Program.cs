using System;
using System.Drawing;
using System.Threading;

namespace JuegoSnake
{
    internal class Program
    {
        static bool ejecucion = true;
        static bool jugar = false;

        static Ventana ventana;
        static Snake snake;
        static Comida comida;

        static void Main(string[] args)
        {
            Iniciar();
            Game();
        }

        static void Iniciar()
        {
            ventana = new Ventana("Snake", 65, 20, ConsoleColor.Black, ConsoleColor.White, 
                                  new Point(5, 3), new Point(59, 18));

            comida = new Comida(ConsoleColor.Green, ventana);

            snake = new Snake(new Point(8, 4), ConsoleColor.Red, ConsoleColor.White, ventana, comida);

            ventana.DibujarMarco();
        }

        static void Game()
        {
            while (ejecucion)
            {
                ventana.Menu();
                ventana.Teclado(ref ejecucion, ref jugar, snake);

                while (jugar)
                {
                    snake.Mover();
                    snake.Informacion(0, 34);

                    if (!snake.Vivo)
                    {
                        jugar = false;
                        snake.Puntaje = 0;
                    }

                    Thread.Sleep(100);
                }

                Thread.Sleep(100);
            }
        }
    }
}