using System;
using Cacti;


namespace CaveMapAdventure
{
    class Enemy
    {
        enum State
        {
            Idle,
            Tracking,
        }

        const int VisionRange = 15;
        static Game mTheGame;

        int mX;
        int mY;
        State mState;

        public Enemy(Game game, int x, int y)
        {
            mTheGame = game;
            mX = x;
            mY = y;
            mState = State.Idle;
        }

        public int GetX()
        {
            return mX;
        }

        public int GetY()
        {
            return mY;
        }

        public void Update(Player player)
        {
            int newx = mX;
            int newy = mY;

            // move
            switch (mState)
            {
                case State.Idle:
                    {
                        if (Utility.Rand() % 2 == 0)
                            newx += Utility.Rand() % 3 - 1;
                        else
                            newy += Utility.Rand() % 3 - 1;
                    }
                    break;
                case State.Tracking:
                    {
                        if (Utility.Rand() % 4 == 0)
                        {
                            if (player.GetX() > mX)
                                newx += 1;
                            else if (player.GetX() < mX)
                                newx -= 1;

                            if (player.GetY() > mY)
                                newy += 1;
                            else if (player.GetY() < mY)
                                newy -= 1;
                        }
                        else
                        {
                            if (Utility.Rand() % 2 == 0)
                                newx += Utility.Rand() % 3 - 1;
                            else
                                newy += Utility.Rand() % 3 - 1;
                        }
                    }
                    break;
                default:
                    break;
            }

            // map collision
            Map map = mTheGame.GetMap();
            if (map.IsOpenSpace(newx, mY))
                mX = newx;
            if (map.IsOpenSpace(mX, newy))
                mY = newy;

            int dx = player.GetX() - mX;
            int dy = player.GetY() - mY;
            int dist = (int)(Math.Sqrt(dx * dx + dy * dy));
            if (dist <= VisionRange)
                mState = State.Tracking;
            else
                mState = State.Idle;
        }

        public void Clear()
        {
            ConsoleColor tmp = Console.ForegroundColor;

            Map map = mTheGame.GetMap();
            Console.SetCursorPosition(mX + map.GetX(), mY + map.GetY());
            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write(' ');

            Console.ForegroundColor = tmp;
        }

        public void Draw()
        {
            ConsoleColor tmp = Console.ForegroundColor;

            Map map = mTheGame.GetMap();
            Console.SetCursorPosition(mX + map.GetX(), mY + map.GetY());
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write('#');

            Console.ForegroundColor = tmp;
        }
    }
}