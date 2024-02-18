using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Tetris
{
    public class GameState
    {
        public enum GameStates
        {
            Game,
            GameOver,
            GamePaused,
            MainMenu,
            About,
        }

        private Block currentBlock;

        private GameStates state;

        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();
                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);

                    if (!CheckIfBlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }

        public GameStates State
        {
            get
            {
                return state;
            }

            set
            {
                if (Enum.IsDefined(typeof(GameStates), value))
                {
                    state = value;
                }
            }
        }

        public GameField GameField { get; }
        public BlockQueue Queue { get; }
        public bool GameOver { get; private set; }

        public bool GamePaused { get; private set; }

        public int Score { get; private set; }

        public Block HeldBlock { get; private set; }
        public bool CanHold { get; private set; }

        public GameState()
        {
            GameField = new GameField(rows:22, cols:10);
            Queue = new BlockQueue();
            CurrentBlock = Queue.GetAndUpdate();
            CanHold = true;
        }

        private bool CheckIfBlockFits()
        {
            foreach (Position pos in CurrentBlock.GetTilesPositions())
            {
                if (!GameField.IsEmpty(pos))
                { return false; }
            }
            return true;
        }

        public void HoldBlock()
        {
            if (!CanHold) { return; }
            if (HeldBlock != null && HeldBlock.Id == CurrentBlock.Id) { return; }
            else if (HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = Queue.GetAndUpdate();
                CanHold = false;
            }
            else
            {
                Block temp = CurrentBlock;
                int tempRotation = currentBlock.Rotation;
                CurrentBlock = HeldBlock;
                HeldBlock = temp;
                CanHold = false;
            }
            
        }

        public void Rotate()
        {
            CurrentBlock.Rotate();

            if (!CheckIfBlockFits())
            {
                for (int i = 0; i < 3; i++)
                { CurrentBlock.Rotate(); }
            }

        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);
            if (!CheckIfBlockFits())
            { CurrentBlock.Move(0, 1); }
        }

        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);
            if (!CheckIfBlockFits())
            { CurrentBlock.Move(0, -1); }

        }

        private bool IsGameOver()
        {
            return (!GameField.IsRowEmpty(0) || !GameField.IsRowEmpty(1));
        }

        private void PlaceBlock()
        {
            foreach (Position pos in CurrentBlock.GetTilesPositions())
            {
                GameField[pos.Row, pos.Col] = CurrentBlock.Id;
            }


            int rowsCleared = GameField.ClearFullRows();

            switch (rowsCleared)
            {
                case 1:
                    Score += 40;
                    break;

                case 2:
                    Score += 100;
                    break;

                case 3:
                    Score += 300;
                    break;

                case 4:
                    Score += 1200;
                    break;
            }
            
            if (IsGameOver())
            {
                State = GameStates.GameOver;
            }
            else
            {
                CurrentBlock = Queue.GetAndUpdate();
                CanHold = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!CheckIfBlockFits())
            {
                CurrentBlock.Move(-1, 0); 
                PlaceBlock();
            }
        }

        private int TileDropDistance(Position pos)
        {
            int drop = 0;

            while (GameField.IsEmpty(pos.Row + drop + 1, pos.Col))
            { drop++; }

            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameField.Rows;

            foreach (Position pos in CurrentBlock.GetTilesPositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(pos));
            }

            return drop;
        }

        public void DropBlock()
        {
            int dropDistance = BlockDropDistance();
            CurrentBlock.Move(dropDistance, 0);
            PlaceBlock();
            Score += dropDistance + 1;
        }
    }
}
