using System;
using System.Threading;
using Cacti;


namespace CaveMapAdventure
{
    class Game
    {
        const int WindowWidth = 80;
        const int WindowHeight = 32;
        const int MapX = 0;
        const int MapY = 2;
        const int MapWidth = 80;
        const int MapHeight = 30;

        Map mMap;
        Player mPlayer;
        Enemy[] mEnemies;
        int mNumEnemies;
        bool mGameOver;
        string mPlayerName;

        public void Init()
        {
            Utility.SetupWindow("CaveMap Adventure", WindowWidth, WindowHeight);
            Utility.EOLWrap(false);
            Console.Clear();

            // menu
            Utility.WriteCentered("Welcome to CaveMapAdventure!", -1);
            Utility.WriteCentered("Player Name? ", 1);
            mPlayerName = Console.ReadLine();
            mNumEnemies = 0;
            do
            {
                Console.SetCursorPosition(0, Console.WindowHeight / 2 + 2);
                for (int i = 0; i < Console.WindowWidth/4; i++)
                    Console.Write("    ");
                Utility.WriteCentered("How many enemies? ", 2);
                mNumEnemies = Utility.ReadInt();
            } while (!Utility.IsReadGood() || mNumEnemies <= 0);

            Load();
        }

        public void Run()
        {
            while (!mGameOver)
            {
                // catch input
                if (Utility.GetKeyState(ConsoleKey.Escape) || Utility.GetKeyState(ConsoleKey.Q))
                    break;
                if (Utility.GetKeyState(ConsoleKey.N))
                    Load();

                // update
                Update();

                // draw
                Draw();

                // timestep
                Thread.Sleep(40);
            }
        }

        public void Shutdown()
        {

        }

        public Map GetMap()
        {
            return mMap;
        }

        public int RandomRange(int min, int max)
        {
            return Utility.Rand() % (max - min + 1) + min;
        }

        private void Load()
        {
            Console.Clear();

            // make map
            mMap = new Map(MapX, MapY, MapWidth, MapHeight);
            mMap.Draw();

            // make player
            int x = 0;
            int y = 0;
            do
            {
                x = Utility.Rand() % mMap.GetWidth();
                y = Utility.Rand() % mMap.GetHeight();
            } while (!mMap.IsOpenSpace(x, y) && !mMap.IsCaveValid(x, y, 40));
            mPlayer = new Player(this, x, y);

            // make enemies
            mEnemies = new Enemy[mNumEnemies];
            for (int i = 0; i < mEnemies.Length; i++)
            {
                do
                {
                    x = Utility.Rand() % MapWidth;
                    y = Utility.Rand() % MapHeight;
                } while (!mMap.IsOpenSpace(x, y));

                mEnemies[i] = new Enemy(this, x, y);
            }

            mGameOver = false;
        }

        private void Update()
        {
            // player
            mPlayer.Clear();
            mPlayer.Update();

            // enemies
            for (int i = 0; i < mEnemies.Length; i++)
            {
                mEnemies[i].Clear();
                mEnemies[i].Update(mPlayer);
            }

            // player-enemy collision
            for (int i = 0; i < mEnemies.Length; i++)
            {
                if (mPlayer.GetX() == mEnemies[i].GetX() &&
                    mPlayer.GetY() == mEnemies[i].GetY())
                {
                    mGameOver = true;
                    break;
                }
            }
        }

        private void Draw()
        {
            //player
            mPlayer.Draw();

            // enemies
            for (int i = 0; i < mEnemies.Length; i++)
                mEnemies[i].Draw();

            // hud
            ConsoleColor prev = Console.ForegroundColor;
            Console.SetCursorPosition(1, 0);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(mPlayerName);
            string msg = "Enemies: " + mNumEnemies.ToString();
            Console.SetCursorPosition(Console.WindowWidth - msg.Length - 1, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(msg);
            Console.ForegroundColor = prev;
        }
    }
}