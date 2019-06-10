using System;
using Cacti;


namespace Galagish
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup console
            Utility.SetupWindow("Galagish", 80, 32);
            Utility.EOLWrap(false);
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // setup and run game
            Game mGame = new Game();
            //mGame.Menu();
            mGame.Init();
            mGame.Run();
            mGame.End();

            // wait for any key before exiting
            Console.ReadKey();
        }
    }
}
