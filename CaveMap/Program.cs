using System;
using Cacti;


namespace CaveMapAdventure
{
    class Program
    {
        static void Main(string[] args)
        {
            Utility.EOLWrap(false);
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            Game game = new Game();
            //game.Menu();
            game.Init();
            game.Run();
            game.Shutdown();

            //Console.SetCursorPosition(0, Console.WindowHeight - 1);
            //Console.Write("Press ENTER to continue...");
            //Console.ReadLine();
        }
    }
}
