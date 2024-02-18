using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock(),
        };

        private readonly Random random = new Random();

        public Block NextBlock {  get; private set; }

        public BlockQueue()
        {
            NextBlock = GetRandomBlock();
        }

        private Block GetRandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock;

            NextBlock = GetRandomBlock();

            return block;
        }
    }
}
