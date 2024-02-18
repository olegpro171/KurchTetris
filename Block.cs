using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public abstract class Block
    {
        protected abstract Position[][] LocalFields { get; }
        protected abstract Position InitialOffset { get; }
        public abstract int Id { get; }

        private int rotation;
        private Position offset;
        
        public int Rotation
        {
            get { return rotation; }
            set
            {
                if (value >= 0 && value < LocalFields.Length)
                {
                    rotation = value;
                }
            }
        }

        public Block()
        {
            offset = InitialOffset.Copy();
        }

        public IEnumerable<Position> GetTilesPositions()
        {
            foreach (Position pos in LocalFields[rotation])
            {
                yield return pos.Copy(offset.Row, offset.Col);
            }
        }

        public void Rotate()
        {
            rotation = (rotation + 1) % LocalFields.Length;
        }

        public void Move(int offsetRows, int offsetCols)
        {
            offset.Row += offsetRows;
            offset.Col += offsetCols;
        }

        public void Reset()
        {
            offset = InitialOffset.Copy();
            rotation = 0;
        }

        public void SetOffset(Position newOffset)
        {
            this.offset = newOffset.Copy();
        }
        public Position GetOffset()
        {
            return offset;
        }
    }
}
