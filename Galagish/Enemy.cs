using System;
using System.Diagnostics;
using Cacti;


namespace Galagish
{
    class Enemy
    {
        public enum Type
        {
            Wanderer,
            Teleporter,
            Bomber
        }

        static Game GameObject;

        Type      mType;
        bool      mAlive;
        float     mXPos;
        float     mYPos;
        float     mSpeed;
        Stopwatch mUpdateTimer;
        Stopwatch mFireTimer;

        public Enemy(Game game, Type type, int x, int y, float speed)
        {
            GameObject = game;

            mAlive = true;
            mType = type;
            mXPos = x;
            mYPos = y;
            mSpeed = speed;

            mUpdateTimer = new Stopwatch();
            mUpdateTimer.Start();
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

        public int GetPointAmount()
        {
            return 100 + ((int)mType * 100);
        }

        public void Update()
        {
            if (!mAlive)
                return;

            switch (mType)
            {
                case Type.Wanderer:
                    {
                        // move
                        int dx = Utility.Rand() % 3 - 1;
                        int dy = Utility.Rand() % 3 - 1;
                        mXPos += dx * mSpeed;
                        mYPos += dy * mSpeed;

                        // wall collision
                        if (mXPos < 0)
                            mXPos = 0;
                        else if (mXPos > Console.WindowWidth - 1)
                            mXPos = Console.WindowWidth - 1;
                        if (mYPos < 0)
                            mYPos = 0;
                        else if (mYPos > Console.WindowHeight - 1)
                            mYPos = Console.WindowHeight - 1;

                        // fire
                        if (mFireTimer.ElapsedMilliseconds > 4000)
                        {
                            if (Utility.Rand() % 100 == 0)
                            {
                                GameObject.FireBullet(Bullet.Type.Enemy, (int)mXPos, (int)mYPos + 1);
                                mFireTimer.Reset();
                                mFireTimer.Start();
                            }
                        }
                        break;
                    }
                case Type.Teleporter:
                    {
                        if (mUpdateTimer.ElapsedMilliseconds > mSpeed)
                        {
                            // move
                            mXPos = Utility.Rand() % Console.WindowWidth;
                            mYPos = Utility.Rand() % (Console.WindowHeight - (Console.WindowHeight / 4));
                            mUpdateTimer.Reset();
                            mUpdateTimer.Start();

                            // fire
                            GameObject.FireBullet(Bullet.Type.Enemy, (int)mXPos, (int)mYPos + 1);
                        }
                        break;
                    }
                case Type.Bomber:
                    {
                        // move
                        mXPos += mSpeed;

                        // wall collision
                        if (mXPos > Console.WindowWidth - 1)
                        {
                            mXPos = 0;
                            mYPos = Utility.Rand() % (Console.WindowHeight - (Console.WindowHeight / 4));
                        }

                        // fire
                        if (mFireTimer.ElapsedMilliseconds > 2000)
                        {
                            if (Utility.Rand() % 10 == 0)
                            {
                                GameObject.FireBullet(Bullet.Type.Enemy, (int)mXPos, (int)mYPos + 1);
                                mFireTimer.Reset();
                                mFireTimer.Start();
                            }
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public void Draw()
        {
            if (!mAlive)
                return;

            // remember last used foreground color
            ConsoleColor prev = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition((int)mXPos, (int)mYPos);

            switch (mType)
            {
                case Type.Wanderer:
                    Console.Write('\u263B');
                    break;
                case Type.Teleporter:
                    Console.Write('v');
                    break;
                case Type.Bomber:
                    Console.Write('~');
                    break;
                default:
                    break;
            }

            Console.ForegroundColor = prev;
        }
    }
}
