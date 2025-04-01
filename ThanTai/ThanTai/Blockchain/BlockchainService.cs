using System.Collections.Generic;

namespace ThanTai.Blockchain
{
    public class BlockchainService
    {
        public List<Block> Chain { get; set; }

        public BlockchainService()
        {
            Chain = new List<Block>();
            AddGenesisBlock();
        }

        private void AddGenesisBlock()
        {
            Chain.Add(new Block(0, "0", "Genesis Block"));
        }

        public Block GetLatestBlock()
        {
            return Chain[^1]; // Lấy block cuối cùng
        }

        public void AddBlock(Block newBlock)
        {
            newBlock.PreviousHash = GetLatestBlock().Hash;
            newBlock.Hash = newBlock.CalculateHash();
            Chain.Add(newBlock);
        }
    }
}
