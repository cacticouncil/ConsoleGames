using System;
using System.Collections.Generic;
using System.Threading;
using Cacti;


namespace Minesweeper
{
    class Program
    {
        enum CellState
        {
            Covered,
            Uncovered,
            Flagged,
        }

        struct Point
        {
            public int x;
            public int y;
        }

        struct Size
        {
            public int w;
            public int h;
        }

        struct Cell
        {
            public char      symbol;
            public CellState state;
        }

        static Random   PRNG;
        static Cell[,]  Minefield;
        static Point    FieldStart;
        static Size     FieldSize;
        static Point    Cursor;
        static Point    CursorLast;
        static int      NumMines;
        static int      NumFlags;
        static int      NumFound;
        static DateTime GameStart;
        static TimeSpan GameElapsed;
        static bool     GameOver;
        static bool     GodMode;
        static bool     KeyDown;

        static void Main(string[] args)
        {
            Utility.EOLWrap(false);
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            bool replay = true;

            while (replay)
            {
                Menu();
                InitGame();
                RunGame();
                replay = EndGame();
            }

            //Console.SetCursorPosition(1, Console.WindowHeight - 1);
            //Console.ResetColor();
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.Write("Game Over!");
            //Console.ReadLine();
        }

        static void Menu()
        {
            string input;

            Console.Clear();

            Utility.WriteCentered("Welcome to Minesweeper!", -2);

            Utility.WriteCentered("Minefield Width?  ", 0);
            input = Console.ReadLine();
            FieldSize.w = Convert.ToInt32(input);

            Utility.WriteCentered("Minefield Height? ", 1);
            input = Console.ReadLine();
            FieldSize.h = Convert.ToInt32(input);

            Utility.WriteCentered("Number of mines?  ", 2);
            input = Console.ReadLine();
            NumMines = Convert.ToInt32(input);
        }

        static void InitGame()
        {
            PRNG = new Random();
            Utility.SetupWindow("Minesweeper", FieldSize.w + 2, FieldSize.h + 4);
            Console.Clear();

            //FieldSize.w = Console.WindowWidth - 2;
            //FieldSize.h = Console.WindowHeight - 4;
            FieldStart.x = 1;
            FieldStart.y = 2;

            //NumMines = 20;
            NumFlags = NumMines;
            NumFound = 0;

            // initialize minefield
            Minefield = new Cell[FieldSize.h, FieldSize.w];
            for (int row = 0; row < FieldSize.h; row++)
            {
                for (int col = 0; col < FieldSize.w; col++)
                {
                    Minefield[row, col].symbol = ' ';
                    Minefield[row, col].state = CellState.Covered;
                }
            }

            // drop mines
            Point loc;
            for (int i = 0; i < NumMines; i++)
            {
                while (true)
                {
                    loc.x = PRNG.Next(0, FieldSize.w);
                    loc.y = PRNG.Next(0, FieldSize.h);

                    if (Minefield[loc.y, loc.x].symbol != '*')
                        break;
                }

                PlaceMine(loc);
            }

            // init cursor
            Cursor.x = Console.WindowWidth / 2;
            Cursor.y = Console.WindowHeight / 2;
            CursorLast = Cursor;

            GameOver = false;
            GodMode = false;
            KeyDown = false;
            GameStart = DateTime.Now;
        }

        static void RunGame()
        {
            DrawMinefield();
            Draw();

            while (!GameOver)
            {
                Update();
                Draw();

                Thread.Sleep(50);
            }

            // eat any remaining input in input buffer
            while (Console.KeyAvailable)
                Console.ReadKey(true);
        }

        static bool EndGame()
        {
            ConsoleColor bc = Console.BackgroundColor;
            ConsoleColor fc = Console.ForegroundColor;

            // indicate win/lose
            if (NumFound == NumMines)
                Console.BackgroundColor = ConsoleColor.Green;
            else
                Console.BackgroundColor = ConsoleColor.Red;
            DrawMinefield();

            Console.BackgroundColor = bc;
            Console.ForegroundColor = fc;

            // replay prompt
            Console.SetCursorPosition(1, Console.WindowHeight - 1);
            Console.Write("Play again?       \b\b\b\b\b\b");
            string answer = Console.ReadLine();
            if (String.Compare(answer, "y", true) == 0   ||
                String.Compare(answer, "yes", true) == 0 ||
                String.Compare(answer, "yep", true) == 0 )
                return true;
            else
                return false;
        }

        static void Update()
        {
            GameElapsed = DateTime.Now - GameStart;
            Console.SetCursorPosition(Cursor.x, Cursor.y);
            CursorLast = Cursor;
            Point cellIdx = ScreenToField(Cursor);

            // read user input
            //ConsoleKeyInfo key = Console.ReadKey(true);

            if (Utility.GetKeyState(ConsoleKey.Q) ||
                Utility.GetKeyState(ConsoleKey.Escape))
            {
                GameOver = true;
                return;
            }
            else if (Utility.GetKeyState(ConsoleKey.G))
            {
                GodMode = !GodMode;
                DrawMinefield();
            }
            else if (Utility.GetKeyState(ConsoleKey.RightArrow) ||
                     Utility.GetKeyState(ConsoleKey.LeftArrow)  ||
                     Utility.GetKeyState(ConsoleKey.UpArrow)    ||
                     Utility.GetKeyState(ConsoleKey.DownArrow)  )
            {
                if (Utility.GetKeyState(ConsoleKey.RightArrow))
                {
                    Cursor.x++;
                }
                else if (Utility.GetKeyState(ConsoleKey.LeftArrow))
                {
                    Cursor.x--;
                }
                if (Utility.GetKeyState(ConsoleKey.UpArrow))
                {
                    Cursor.y--;
                }
                else if (Utility.GetKeyState(ConsoleKey.DownArrow))
                {
                    Cursor.y++;
                }
            }
            else if (Utility.GetKeyState(ConsoleKey.Spacebar))
            {
                if (Minefield[cellIdx.y, cellIdx.x].state == CellState.Covered)
                    UncoverCell(cellIdx);
            }
            else if (Utility.GetKeyState(ConsoleKey.F) && !KeyDown)
            {
                KeyDown = true;

                if (Minefield[cellIdx.y, cellIdx.x].state == CellState.Flagged)
                {
                    Minefield[cellIdx.y, cellIdx.x].state = CellState.Covered;
                    NumFlags++;
                    if (Minefield[cellIdx.y, cellIdx.x].symbol == '*')
                        NumFound--;
                }
                else if (Minefield[cellIdx.y, cellIdx.x].state == CellState.Covered)
                {
                    if (NumFlags > 0)
                    {
                        Minefield[cellIdx.y, cellIdx.x].state = CellState.Flagged;
                        NumFlags--;
                        if (Minefield[cellIdx.y, cellIdx.x].symbol == '*')
                        {
                            NumFound++;
                            if (NumFlags == 0 && NumFound == NumMines)
                                GameOver = true;
                        }
                    }
                }
            }
            else if (!Utility.GetKeyState(ConsoleKey.F))
            {
                KeyDown = false;
            }

            // don't allow cursor outside of minefield
            if (Cursor.x < FieldStart.x)
                Cursor.x = FieldStart.x;
            else if (Cursor.x > FieldStart.x + FieldSize.w - 1)
                Cursor.x = FieldStart.x + FieldSize.w - 1;
            if (Cursor.y < FieldStart.y)
                Cursor.y = FieldStart.y;
            else if (Cursor.y > FieldStart.y + FieldSize.h - 1)
                Cursor.y = FieldStart.y + FieldSize.h - 1;
        }

        static void Draw()
        {
            Point idxlast = ScreenToField(CursorLast);
            DrawCell(idxlast, CursorLast);

            Point idx = ScreenToField(Cursor);
            Console.BackgroundColor = ConsoleColor.Magenta;
            DrawCell(idx, Cursor);
            Console.BackgroundColor = ConsoleColor.Black;

            Console.SetCursorPosition(1, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Mines:     \b\b\b\b" + NumFlags);

            string msg = "Time: " + GameElapsed.ToString("mm\\:ss");
            Console.SetCursorPosition(Console.WindowWidth - msg.Length - FieldStart.x, 0);
            Console.Write(msg);
        }

        static void DrawCell(Point idx, Point pos)
        {
            Console.SetCursorPosition(pos.x, pos.y);
            Console.ForegroundColor = ConsoleColor.White;

            if (GodMode)
            {
                Console.Write(Minefield[idx.y, idx.x].symbol);
                return;
            }

            switch (Minefield[idx.y, idx.x].state)
            {
                case CellState.Covered:
                    Console.Write('#');
                    break;
                case CellState.Uncovered:
                    switch (Minefield[idx.y, idx.x].symbol)
                    {
                        case '1': Console.ForegroundColor = ConsoleColor.Blue;  break;
                        case '2': Console.ForegroundColor = ConsoleColor.Green; break;
                        case '3': Console.ForegroundColor = ConsoleColor.Red; break;
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8': Console.ForegroundColor = ConsoleColor.DarkRed; break;
                        default:  break;
                    }
                    Console.Write(Minefield[idx.y, idx.x].symbol);
                    break;
                case CellState.Flagged:
                    Console.Write('F');
                    break;
                default:
                    Console.Write('?');
                    break;
            }
        }

        static void DrawMinefield()
        {
            Point cellLoc;

            for (int row = 0; row < FieldSize.h; row++)
            {
                for (int col = 0; col < FieldSize.w; col++)
                {
                    cellLoc.x = col;
                    cellLoc.y = row;
                    DrawCell(cellLoc, FieldToScreen(cellLoc));
                }
            }
        }

        static void UncoverCell(Point cellLoc)
        {
            // quick out, user uncovered a mine!
            if (Minefield[cellLoc.y, cellLoc.x].symbol == '*')
            {
                Minefield[cellLoc.y, cellLoc.x].state = CellState.Uncovered;
                GameOver = true;
                return;
            }

            // start list with the starting cell location
            List<Point> indices = new List<Point>();
            indices.Add(cellLoc);

            // for all cells in the list
            while (indices.Count > 0)
            {
                // dequeue first cell
                Point loc = indices[0];
                indices.RemoveAt(0);

                // ignore cell, unless it is a covered non-mine
                if (Minefield[loc.y, loc.x].state == CellState.Uncovered ||
                    Minefield[loc.y, loc.x].state == CellState.Flagged ||
                    Minefield[loc.y, loc.x].symbol == '*')
                    continue;

                // uncover the cell
                Minefield[loc.y, loc.x].state = CellState.Uncovered;
                DrawCell(loc, FieldToScreen(loc));

                // don't uncover neighbors unless cell is empty
                if (Minefield[loc.y, loc.x].symbol != ' ')
                    continue;

                // loop through all neighboring cell locations
                for (int row = loc.y - 1; row <= loc.y + 1; row++)
                {
                    for (int col = loc.x - 1; col <= loc.x + 1; col++)
                    {
                        // ignore starting cell location
                        if (row == loc.y && col == loc.x)
                            continue;

                        // ignore cell locations outside of the field
                        if (row < 0 || row >= FieldSize.h ||
                            col < 0 || col >= FieldSize.w)
                            continue;

                        // ignore if cell is already uncovered
                        if (Minefield[row, col].state == CellState.Uncovered)
                            continue;

                        // ignore if cell location is already in list
                        bool alreadyInList = false;
                        for (int i = 0; i < indices.Count; i++)
                        {
                            if (col == indices[i].x && row == indices[i].y)
                            {
                                alreadyInList = true;
                                break;
                            }
                        }
                        if (alreadyInList)
                            continue;

                        Point neighbor;
                        neighbor.x = col;
                        neighbor.y = row;
                        indices.Add(neighbor);
                    }
                }
            }
        }

        static void PlaceMine(Point idx)
        {
            // place mine
            Minefield[idx.y, idx.x].symbol = '*';

            // affect surrounding cells
            for (int row = idx.y - 1; row <= idx.y + 1; row++)
            {
                for (int col = idx.x - 1; col <= idx.x + 1; col++)
                {
                    // ignore cells out of bounds of minefield
                    if (row < 0 || row >= FieldSize.h ||
                        col < 0 || col >= FieldSize.w )
                        continue;

                    // ignore cells with mines
                    if (Minefield[row, col].symbol == '*')
                        continue;

                    // ignore starting cell - unecessary because we already dropped mine there
                    //if (row == idx.y && col == idx.x)
                    //    continue;

                    // increment value of cell
                    if (Minefield[row, col].symbol == ' ')
                        Minefield[row, col].symbol = '0';
                    Minefield[row, col].symbol++;
                }
            }
        }

        static Point ScreenToField(Point screenPos)
        {
            Point fieldPos;

            fieldPos.x = screenPos.x - FieldStart.x;
            fieldPos.y = screenPos.y - FieldStart.y;

            return fieldPos;
        }

        static Point FieldToScreen(Point fieldPos)
        {
            Point screenPos;

            screenPos.x = fieldPos.x + FieldStart.x;
            screenPos.y = fieldPos.y + FieldStart.y;

            return screenPos;
        }
    }
}
