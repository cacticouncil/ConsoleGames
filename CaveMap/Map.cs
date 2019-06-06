using System;
using System.Collections.Generic;
using System.Threading;
using Cacti;


namespace CaveMapAdventure
{
    class Map
    {
        struct CellPos
        {
            public int x;
            public int y;

            public CellPos(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        const char CellWall  = '█';  // '\u2588'
        const char CellSpace = ' ';
        const int PercentWalls = 40; // out of 100
        const int MapLoadPause = 20; // ms

        int mMapX;
        int mMapY;
        int mMapWidth;
        int mMapHeight;
        char[,] mCells;
        char[,] mCellsNextGen;

        public Map(int x, int y, int width, int height)
        {
            mMapX = x;
            mMapY = y;
            mMapWidth = width;
            mMapHeight = height;

            mCells = new char[mMapHeight, mMapWidth];
            mCellsNextGen = new char[mMapHeight, mMapWidth];

            GenerateNewMap();
        }

        public int GetX()
        {
            return mMapX;
        }

        public int GetY()
        {
            return mMapY;
        }

        public int GetWidth()
        {
            return mMapWidth;
        }

        public int GetHeight()
        {
            return mMapHeight;
        }

        public bool IsOpenSpace(int x, int y)
        {
            return (mCells[y, x] == CellSpace);
        }

        public bool IsCaveValid(int x, int y, int threshold)
        {
            List<CellPos> cells = new List<CellPos>();
            List<CellPos> visited = new List<CellPos>();
            int numMapTiles = mMapWidth * mMapHeight;

            // add starting tile
            CellPos currCell;
            currCell.x = x;
            currCell.y = y;
            cells.Add(currCell);

            // while there are tiles still in this queue
            while (cells.Count > 0)
            {
                // remove a tile
                currCell = cells[0];
                cells.RemoveAt(0);
                visited.Add(currCell);

                // this cavern is large enough because the number of tiles found
                // was larger than the threshold
                int percent = (int)((float)visited.Count / (float)numMapTiles * 100.0f);
                if (percent >= threshold)
                    return true;

                // consider only cardinal neighbors,
                // ignore diagonal neighbors because of problems like this:
                // X XXX X
                // XX P XX
                // X XXX X
                // if player P is spawned in the cave, you wouldn't want
                // that cave to be considered valid if there are open spaces
                // on the diagonals only
                CellPos[] neighbors = new CellPos[4];
                neighbors[0] = new CellPos(x - 1, y); // west
                neighbors[1] = new CellPos(x + 1, y); // east
                neighbors[2] = new CellPos(x, y - 1); // north
                neighbors[3] = new CellPos(x, y + 1); // south

                // queue up its neighbors
                for (int i = 0; i < 4; i++)
                {
                    // ignore starting tile
                    if (neighbors[i].x == currCell.x && neighbors[i].y == currCell.y)
                        continue;

                    // if out of bounds of map, count this as a wall
                    if (neighbors[i].x < 0 || neighbors[i].x >= mMapWidth ||
                        neighbors[i].y < 0 || neighbors[i].y >= mMapHeight)
                        continue;

                    // ignore walls
                    if (mCells[neighbors[i].y, neighbors[i].x] == CellWall)
                        continue;

                    // ignore if neighbor is already in queue
                    bool ignore = false;
                    for (int j = 0; j < cells.Count; j++)
                    {
                        if (cells[j].x == neighbors[i].x &&
                            cells[j].y == neighbors[i].y)
                        {
                            ignore = true;
                            break;
                        }
                    }
                    if (ignore)
                        continue;

                    // ignore if neighbor has already been visited
                    ignore = false;
                    for (int j = 0; j < visited.Count; j++)
                    {
                        if (visited[j].x == neighbors[i].x &&
                            visited[j].y == neighbors[i].y)
                        {
                            ignore = true;
                            break;
                        }
                    }
                    if (ignore)
                        continue;

                    // finally, add neighbor to queue
                    cells.Add(neighbors[i]);
                }
            }

            return false;
        }

        public void Draw()
        {
            for (int row = 0; row < mMapHeight; row++)
            {
                for (int col = 0; col < mMapWidth; col++)
                {
                    Console.SetCursorPosition(mMapX + col, mMapY + row);
                    Console.Write(mCells[row, col]);
                }
            }
        }

        private void GenerateNewMap()
        {
            RandomizeMap();
            Pause();

            // refine, but prevent large open areas
            for (int i = 0; i < 4; i++)
            {
                RefineMap(true);
                Pause();
            }

            // refine
            for (int i = 0; i < 3; i++)
            {
                RefineMap(false);
                Pause();
            }
        }

        private void RandomizeMap()
        {
            // fill horizontal borders with walls
            for (int i = 0; i < mMapWidth; i++)
            {
                mCells[0, i] = CellWall;
                mCells[mMapHeight-1, i] = CellWall;
            }

            // fill vertical borders with walls
            for (int i = 0; i < mMapHeight; i++)
            {
                mCells[i, 0] = CellWall;
                mCells[i, mMapWidth-1] = CellWall;
            }

            // fill every other tile randomly with some % of walls
            for (int row = 1; row < mMapHeight-1; row++)
            {
                for (int col = 1; col < mMapWidth-1; col++)
                {
                    if (Utility.Rand() % 100 + 1 <= PercentWalls)
                        mCells[row, col] = CellWall;
                    else
                        mCells[row, col] = CellSpace;
                }
            }

            // fill a strip of whitespace across the middle
            int middle = mMapHeight / 2;
            for (int i = 1; i < mMapWidth-1; i++)
            {
                mCells[middle-1, i] = CellSpace;
                mCells[middle,   i] = CellSpace;
                mCells[middle+1, i] = CellSpace;
            }

            Array.Copy(mCells, mCellsNextGen, mMapWidth * mMapHeight);
        }

        private void RefineMap(bool preventOpenAreas)
        {
            for (int row = 0; row < mMapHeight; row++)
            {
                for (int col = 0; col < mMapWidth; col++)
                {
                    int numWallsA1 = CountNeighborWalls(col, row, 1);
                    int numWallsA2 = CountNeighborWalls(col, row, 2);

                    if (mCells[row, col] == CellWall)
                    {
                        if (numWallsA1 >= 4)
                            mCellsNextGen[row, col] = CellWall;
                        else
                            mCellsNextGen[row, col] = CellSpace;
                    }
                    else
                    {
                        if (numWallsA1 >= 5)
                            mCellsNextGen[row, col] = CellWall;
                        else if (numWallsA2 <= 1 && preventOpenAreas)
                            mCellsNextGen[row, col] = CellWall;
                        else
                            mCellsNextGen[row, col] = CellSpace;
                    }
                }
            }

            Array.Copy(mCellsNextGen, mCells, mMapWidth * mMapHeight);
        }

        private int CountNeighborWalls(int cellx, int celly, int dist)
        {
            int count = 0;

            for (int y = celly - dist; y <= celly + dist; y++)
            {
                for (int x = cellx - dist; x <= cellx + dist; x++)
                {
                    // ignore the tile we are considering neighbors of
                    if (x == cellx && y == celly)
                        continue;

                    // if out of bounds, count it as a wall
                    if (x < 0 || x >= mMapWidth ||
                        y < 0 || y >= mMapHeight)
                    {
                        count++;
                        continue;
                    }

                    // count actual walls
                    if (mCells[y, x] != CellSpace)
                    {
                        count++;
                        continue;
                    }
                }
            }

            return count;
        }

        private void Pause()
        {
            Draw();
            //Console.SetCursorPosition(0, 0);
            //Console.ReadLine();
            Thread.Sleep(MapLoadPause);
        }
    }
}