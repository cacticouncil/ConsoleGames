using System;
using System.Diagnostics;
using Cacti;


namespace CaveMapAdventure
{
    class Player
    {
        const int PlayerSpeed = 1;
        static Game mTheGame;

        int mX;
        int mY;
        bool mIsAlive;

        public Player(Game game, int x, int y)
        {
            mTheGame = game;
            mX = x;
            mY = y;
            mIsAlive = true;
        }

        public bool IsAlive()
        {
            return mIsAlive;
        }

        public int GetX()
        {
            return mX;
        }

        public int GetY()
        {
            return mY;
        }

        public void Kill()
        {
            mIsAlive = false;
        }

        public void Update()
        {
            Map map = mTheGame.GetMap();

            int newx = mX;
            int newy = mY;

            if (Utility.GetKeyState(ConsoleKey.LeftArrow))
                newx -= PlayerSpeed;
            else if (Utility.GetKeyState(ConsoleKey.RightArrow))
                newx += PlayerSpeed;

            if (Utility.GetKeyState(ConsoleKey.UpArrow))
                newy -= PlayerSpeed;
            else if (Utility.GetKeyState(ConsoleKey.DownArrow))
                newy += PlayerSpeed;

            // wall collision
            if (map.IsOpenSpace(newx, mY))
                mX = newx;
            if (map.IsOpenSpace(mX, newy))
                mY = newy;
        }

        public void Clear()
        {
            ConsoleColor prevFC = Console.ForegroundColor;

            Map map = mTheGame.GetMap();

            Console.SetCursorPosition(map.GetX() + mX, map.GetY() + mY);
            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write(' ');

            Console.ForegroundColor = prevFC;
        }

        public void Draw()
        {
            ConsoleColor prevFC = Console.ForegroundColor;

            Map map = mTheGame.GetMap();

            Console.SetCursorPosition(map.GetX() + mX, map.GetY() + mY);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write('@');

            Console.ForegroundColor = prevFC;
        }
    }
}