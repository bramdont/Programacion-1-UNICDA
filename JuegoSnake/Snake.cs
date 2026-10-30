using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace JuegoSnake
{
    public enum Direccion
    {
        Derecha,
        Izquierda,
        Arriba,
        Abajo
    }

    public class Snake
    {
        public bool Vivo { get; set; }
        public ConsoleColor ColorCabeza { get; set; }
        public ConsoleColor ColorCuerpo { get; set; }
        public Ventana VentanaC { get; set; }
        public List<Point> Cuerpo { get; set; }
        public Point Cabeza { get; set; }
        public Point PosicionInicial { get; set; }
        public Direccion DireccionActual { get; set; }
        public Comida ComidaC { get; set; }

        private bool Comiendo;
        public int Puntaje { get; set; }
        public int PuntajeMaximo { get; set; }

        public Snake(Point posicionInicial, ConsoleColor colorCabeza, ConsoleColor colorCuerpo, 
                     Ventana ventana, Comida comida)
        {
            PosicionInicial = posicionInicial;
            Cabeza = posicionInicial;
            ColorCabeza = colorCabeza;
            ColorCuerpo = colorCuerpo;
            VentanaC = ventana;
            ComidaC = comida;
            Cuerpo = new List<Point>();
            DireccionActual = Direccion.Derecha;
            Vivo = true;
            Puntaje = 0;
            PuntajeMaximo = 0;
        }

        public void IniciarCuerpo(int numPartes)
        {
            int x = Cabeza.X - 1;
            Console.ForegroundColor = ColorCuerpo;

            for (int i = 0; i < numPartes; i++)
            {
                Point parte = new Point(x, Cabeza.Y);
                Console.SetCursorPosition(parte.X, parte.Y);
                Console.Write("▓");
                Cuerpo.Add(parte);
                x--;
            }
        }

        public void Init()
        {
            Console.Clear();
            VentanaC.DibujarMarco();
            Cabeza = PosicionInicial;
            Cuerpo.Clear();
            IniciarCuerpo(2);
            Vivo = true;
            DireccionActual = Direccion.Derecha;
            ComidaC.GenerarComida(this);
        }

        public void Mover()
        {
            Teclado();
            Point posAnteriorCabeza = Cabeza;

            MoverCabeza();
            MoverCuerpo(posAnteriorCabeza);
            ColisionesComida();

            if (ColisionesCuerpo())
            {
                Muerte();
                VentanaC.GameOver("G A M E   O V E R");
            }
        }

        private void MoverCabeza()
        {
            Console.SetCursorPosition(Cabeza.X, Cabeza.Y);
            Console.Write(" ");

            Point nuevaCabeza = Cabeza;

            switch (DireccionActual)
            {
                case Direccion.Derecha: nuevaCabeza.X++; break;
                case Direccion.Izquierda: nuevaCabeza.X--; break;
                case Direccion.Abajo: nuevaCabeza.Y++; break;
                case Direccion.Arriba: nuevaCabeza.Y--; break;
            }

            Cabeza = nuevaCabeza;
            ColisionesMarco();

            Console.ForegroundColor = ColorCabeza;
            Console.SetCursorPosition(Cabeza.X, Cabeza.Y);
            Console.Write("█");
        }

        private void MoverCuerpo(Point posAnteriorCabeza)
        {
            Console.ForegroundColor = ColorCuerpo;
            Console.SetCursorPosition(posAnteriorCabeza.X, posAnteriorCabeza.Y);
            Console.Write("▓");
            Cuerpo.Insert(0, posAnteriorCabeza);

            if (Comiendo)
            {
                Comiendo = false;
                return;
            }

            Point ultimaParte = Cuerpo[Cuerpo.Count - 1];
            Console.SetCursorPosition(ultimaParte.X, ultimaParte.Y);
            Console.Write(" ");
            Cuerpo.RemoveAt(Cuerpo.Count - 1);
        }

        private void Teclado()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo tecla = Console.ReadKey(true);

                if (tecla.Key == ConsoleKey.RightArrow && DireccionActual != Direccion.Izquierda)
                    DireccionActual = Direccion.Derecha;
                else if (tecla.Key == ConsoleKey.LeftArrow && DireccionActual != Direccion.Derecha)
                    DireccionActual = Direccion.Izquierda;
                else if (tecla.Key == ConsoleKey.UpArrow && DireccionActual != Direccion.Abajo)
                    DireccionActual = Direccion.Arriba;
                else if (tecla.Key == ConsoleKey.DownArrow && DireccionActual != Direccion.Arriba)
                    DireccionActual = Direccion.Abajo;
            }
        }

        private void ColisionesMarco()
        {
            if (Cabeza.X <= VentanaC.LimiteSuperior.X)
                Cabeza = new Point(VentanaC.LimiteInferior.X - 1, Cabeza.Y);
            else if (Cabeza.X >= VentanaC.LimiteInferior.X)
                Cabeza = new Point(VentanaC.LimiteSuperior.X + 1, Cabeza.Y);

            if (Cabeza.Y <= VentanaC.LimiteSuperior.Y)
                Cabeza = new Point(Cabeza.X, VentanaC.LimiteInferior.Y - 1);
            else if (Cabeza.Y >= VentanaC.LimiteInferior.Y)
                Cabeza = new Point(Cabeza.X, VentanaC.LimiteSuperior.Y + 1);
        }

        private void ColisionesComida()
        {
            if (Cabeza == ComidaC.Posicion)
            {
                Comiendo = true;
                Puntaje++;

                if (Puntaje > PuntajeMaximo)
                    PuntajeMaximo = Puntaje;

                if (!ComidaC.GenerarComida(this))
                {
                    Vivo = false;
                    VentanaC.GameOver("J U E G O   C O M P L E T A D O");
                }
            }
        }

        private bool ColisionesCuerpo()
        {
            foreach (Point parte in Cuerpo)
            {
                if (Cabeza == parte)
                {
                    Vivo = false;
                    return true;
                }
            }
            return false;
        }

        public void Muerte()
        {
            Console.ForegroundColor = ColorCuerpo;
            foreach (Point parte in Cuerpo)
            {
                if (parte == Cabeza) continue;

                Console.SetCursorPosition(parte.X, parte.Y);
                Console.Write("░");
                Thread.Sleep(120);
            }
        }

        public void Informacion(int dist1, int dist2)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(VentanaC.LimiteSuperior.X + dist1, VentanaC.LimiteSuperior.Y - 1);
            Console.Write($"Puntaje: {Puntaje}  ");

            Console.SetCursorPosition(VentanaC.LimiteSuperior.X + dist2, VentanaC.LimiteSuperior.Y - 1);
            Console.Write($"Puntaje Máximo: {PuntajeMaximo}  ");
        }
    }
}