using System;
using System.Collections.Generic;
using System.Threading;
using Cacti;


namespace Galagish
{
    class Game
    {
        static int DefNumEnemies = 20;

        bool         mGameOver;
        int          mScore;
        Player       mPlayer;
        List<Enemy>  mEnemies;
        List<Bullet> mBullets;

        public Game()
        {
            // empty
        }

        //public void Menu()
        //{
        //    // TODO: make a menu screen
        //    // ask the user for some options
        //    // read in their input and use those values when initializing the game
        //    // remember to clear the screen
        //}

        public void Init()
        {
            mGameOver = false;
            mScore = 0;

            // init player
            mPlayer = new Player(this);
            mPlayer.SetPosition(Console.WindowWidth / 2, Console.WindowHeight - 1);

            // init enemies
            mEnemies = new List<Enemy>();
            for (int i = 0; i < DefNumEnemies; i++)
            {
                Enemy.Type type = (Enemy.Type)(Utility.Rand() % 3);
                int x = Utility.Rand() % Console.WindowWidth;
                int y = Utility.Rand() % (Console.WindowHeight - (Console.WindowHeight/4));
                float speed;
                switch (type)
                {
                    case Enemy.Type.Wanderer:
                        speed = (Utility.Rand() % 5 + 1) / 10.0f;
                        break;
                    case Enemy.Type.Teleporter:
                        speed = Utility.Rand() % 4000 + 1000;
                        break;
                    case Enemy.Type.Bomber:
                        speed = (Utility.Rand() % 10) / 10.0f;
                        break;
                    default:
                        speed = 1f;
                        break;
                }
                Enemy e = new Enemy(this, type, x, y, speed);
                mEnemies.Add(e);
            }

            // init bullets
            mBullets = new List<Bullet>();
        }

        public void Run()
        {
            while (!mGameOver)
            {
                // catch input
                if (Utility.GetKeyState(ConsoleKey.Q) ||
                    Utility.GetKeyState(ConsoleKey.Escape))
                    mGameOver = true;

                // update
                Update();

                // draw/render
                Draw();

                // timestep (1000/20 = 50 FPS) 
                Thread.Sleep(20);
            }

            // drain any key presses left in the input buffer
            while (Console.KeyAvailable)
                Console.ReadKey(true);
        }

        public void End()
        {
            bool won = (mPlayer.IsAlive() && (mEnemies.Count <= 0));

            if (won)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Clear();
                Utility.WriteCentered("You win!!");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Clear();
                Utility.WriteCentered("You lose :(");
            }
        }

        private void Update()
        {
            // bullets
            for (int i = 0; i < mBullets.Count; i++)
            {
                mBullets[i].Update();

                // player bullet collision
                if (mBullets[i].GetBulletType() == Bullet.Type.Player)
                {
                    for (int j = 0; j < mEnemies.Count; j++)
                    {
                        if (mBullets[i].GetX() == mEnemies[j].GetX() &&
                            mBullets[i].GetY() == mEnemies[j].GetY())
                        {
                            mBullets[i].Kill();
                            mEnemies[j].Kill();
                            mScore += mEnemies[j].GetPointAmount();
                            break;
                        }
                    }
                }
                // enemy bullet collision
                else if (mBullets[i].GetBulletType() == Bullet.Type.Enemy)
                {
                    if (mBullets[i].GetX() == mPlayer.GetX() &&
                        mBullets[i].GetY() == mPlayer.GetY())
                    {
                        mBullets[i].Kill();
                        mPlayer.Kill();
                    }
                }

                // remove bullet from list if dead
                if (!mBullets[i].IsAlive())
                {
                    mBullets.RemoveAt(i);
                    i--;
                }
            }

            // enemies
            for (int i = 0; i < mEnemies.Count; i++)
            {
                mEnemies[i].Update();

                // remove enemies from list if dead
                if (!mEnemies[i].IsAlive())
                {
                    mEnemies.RemoveAt(i);
                    i--;
                }
            }

            // player
            mPlayer.Update();

            // win conditions
            if (!mPlayer.IsAlive() || mEnemies.Count <= 0)
                mGameOver = true;
        }

        private void Draw()
        {
            Utility.LockConsole(true);
            Console.Clear();

            // enemies
            foreach (Enemy e in mEnemies)
                e.Draw();

            // bullets
            for (int i = 0; i < mBullets.Count; i++)
                mBullets[i].Draw();

            // player
            mPlayer.Draw();

            // HUD
            Console.SetCursorPosition(1, 0);
            Console.Write("Score:          \b\b\b\b\b\b\b\b\b" + mScore);

            Utility.LockConsole(false);
        }

        public void FireBullet(Bullet.Type type, int x, int y)
        {
            Bullet b = new Bullet(type, x, y);
            mBullets.Add(b);
        }
    }
}
