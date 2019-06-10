using System;
using System.Threading;
using Cacti;


namespace Bounce
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup console
            Utility.SetupWindow("Bounce", 80, 32);
            Utility.EOLWrap(false);
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            // initialize gameplay variables
            int x = Console.WindowWidth / 2;
            int y = Console.WindowHeight / 2;
            int vx = 1;
            int vy = 1;

            // game loop
            while (true)
            {
                // clear old position of ball
                Console.SetCursorPosition(x, y);
                Console.Write(' ');

                // move the ball (physics)
                x += vx;
                y += vy;

                // wall collision (x-axis)
                if (x < 0)
                {
                    x = 0;
                    vx = -vx;
                }
                else if (x >= Console.WindowWidth)
                {
                    x = Console.WindowWidth - 1;
                    vx = -vx;
                }

                // wall collision (y-axis)
                if (y < 0)
                {
                    y = 0;
                    vy = -vy;
                }
                else if (y >= Console.WindowHeight)
                {
                    y = Console.WindowHeight - 1;
                    vy = -vy;
                }

                // draw/render
                Console.SetCursorPosition(x, y);
                Console.Write('O');

                // loop exit
                if (Utility.GetKeyState(ConsoleKey.Q))
                    break;

                // timestep (1000/20 = 50 FPS) 
                Thread.Sleep(20);
            }
        }
    }
}
