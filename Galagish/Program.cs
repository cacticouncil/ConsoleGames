using System;
using System.Text;
using Cacti;


namespace Galagish
{
    class Program
    {
        static Game mGame;

        static void Main(string[] args)
        {
            Utility.SetupWindow("Galagish", 80, 32);
            Utility.EOLWrap(false);
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            mGame = new Game();
            //mGame.Menu();
            mGame.Init();
            mGame.Run();
            mGame.End();
        }
    }
}
