using System;
using System.Diagnostics;
using Cacti;


namespace Galagish
{
    class Player
    {
        const int FireRate = 80;

        static Game GameObject;

        bool      mAlive;
        float     mXPos;
        float     mYPos;
        Stopwatch mFireTimer;

        public Player(Game game)
        {
            GameObject = game;

            mAlive = true;
            mXPos = 0;
            mYPos = 0;
            mFireTimer = new Stopwatch();
            mFireTimer.Start();
        }

        public bool IsAlive()
        {
            return mAlive;
        }

        public void Kill()
        {
            mAlive = false;
        }

        public int GetX()
        {
            return (int)mXPos;
        }

        public int GetY()
        {
            return (int)mYPos;
        }

        public void SetPosition(int x, int y)
        {
            mXPos = x;
            mYPos = y;
        }

        public void Update()
        {
            if (!mAlive)
                return;

            // handle input
            if (Utility.GetKeyState(ConsoleKey.UpArrow))
            {
                if (mFireTimer.ElapsedMilliseconds > FireRate)
                {
                    GameObject.FireBullet(Bullet.Type.Player, (int)mXPos, (int)mYPos - 1);
                    mFireTimer.Reset();
                    mFireTimer.Start();
                }
            }
            if (Utility.GetKeyState(ConsoleKey.RightArrow))
                mXPos++;
            else if (Utility.GetKeyState(ConsoleKey.LeftArrow))
                mXPos--;

            // wall collision (X-axis only)
            if (mXPos < 0)
                mXPos = 0;
            else if (mXPos > Console.WindowWidth - 1)
                mXPos = Console.WindowWidth - 1;
        }

        public void Draw()
        {
            // remember last used foreground color
            ConsoleColor prev = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition((int)mXPos, (int)mYPos);
            Console.Write('^');

            Console.ForegroundColor = prev;
        }
    }
}
