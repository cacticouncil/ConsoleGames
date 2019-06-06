using System;


namespace Galagish
{
    class Bullet
    {
        public enum Type
        {
            Player,
            Enemy
        }

        const float Speed = 1f;

        Type  mType;
        float mXPos;
        float mYPos;
        bool  mAlive;

        public Bullet(Type type, int x, int y)
        {
            mType = type;
            mAlive = true;
            mXPos = x;
            mYPos = y;
        }

        public bool IsAlive()
        {
            return mAlive;
        }

        public void Kill()
        {
            mAlive = false;
        }

        public Bullet.Type GetBulletType()
        {
            return mType;
        }

        public int GetX()
        {
            return (int)mXPos;
        }

        public int GetY()
        {
            return (int)mYPos;
        }

        public void Update()
        {
            if (!mAlive)
                return;

            // modify position
            switch (mType)
            {
                case Type.Player:
                    mYPos -= Speed;
                    break;
                case Type.Enemy:
                    mYPos += Speed;
                    break;
                default:
                    break;
            }

            // wall collision
            switch (mType)
            {
                case Type.Player:
                    if (mYPos < 0)
                        mAlive = false;
                    break;
                case Type.Enemy:
                    if (mYPos > Console.WindowHeight - 1)
                        mAlive = false;
                    break;
                default:
                    break;
            }
        }

        public void Draw()
        {
            if (!mAlive)
                return;

            Console.SetCursorPosition((int)mXPos, (int)mYPos);

            switch (mType)
            {
                case Type.Player:
                    Console.Write('.');
                    break;
                case Type.Enemy:
                    Console.Write('!');
                    break;
                default:
                    break;
            }

        }
    }
}
