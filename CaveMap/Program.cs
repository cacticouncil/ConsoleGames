using System;


namespace CaveMapAdventure
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();

            game.Init();
            game.Run();
            game.Shutdown();

            //Console.SetCursorPosition(0, Console.WindowHeight - 1);
            //Console.Write("Press ENTER to continue...");
            //Console.ReadLine();
        }
    }
}
