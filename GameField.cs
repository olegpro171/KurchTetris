using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Tetris
{
    public class GameField
    {
        private readonly int[,] GameGrid;
        public int Rows { get; }
        public int Columns { get; }

        public int this[int row, int col]
        {
            get => GameGrid[row, col];
            set => GameGrid[row, col] = value;
        }

        public GameField(int rows, int cols)
        {
            Rows = rows; Columns = cols;
            GameGrid = new int[Rows, Columns];
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    GameGrid[row, col] = 0;
                }
            }
        }

        public bool IsInGrid(int row, int col)
        {
            return row >= 0 && row < Rows && col >= 0 && col < Columns;
        }

        public bool IsEmpty(int row, int col)
        {
            return IsInGrid(row, col) && GameGrid[row, col] == 0;
        }

        public bool IsEmpty(Position pos)
        {
            return IsEmpty(pos.Row, pos.Col);
        }

        public bool IsRowFull(int row)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (GameGrid[row, col] == 0)
                { return false; }
            }
            return true;
        }

        public bool IsRowEmpty(int row)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (GameGrid[row, col] != 0)
                { return false; }
            }
            return true;
        }

        private void ClearRow(int row)
        {
            for (int col = 0; col < Columns; col++)
            { GameGrid[row, col] = 0; }
        }

        private void MoveRowDown(int row, int n)
        {
            for (int col = 0; col < Columns; col++)
            {
                GameGrid[row + n, col] = GameGrid[row, col];
                GameGrid[row, col] = 0;
            }
        }

        public int ClearFullRows()
        {
            int rowsCleared = 0;

            for (int row = Rows - 1; row >= 0; row--)
            {
                if (IsRowFull(row))
                {
                    ClearRow(row);
                    rowsCleared++;
                }
                else if (rowsCleared > 0)
                {
                    MoveRowDown(row, rowsCleared);
                }
            }

            return rowsCleared;
        }
    }
}
