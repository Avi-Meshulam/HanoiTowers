using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanoiTowers
{
    class Tower
    {
        private Stack<BuildingBlock> _buildingBlocks;

        public string Name { get; private set; }
        public Position Position { get; private set; }
        public int BlocksCount { get { return _buildingBlocks.Count; } }

        public Tower(string name, Position position)
        {
            Name = name;
            Position = position;
            _buildingBlocks = new Stack<BuildingBlock>();
        }

        public BuildingBlock MoveBlock(Tower targetTower)
        {
            BuildingBlock block = _buildingBlocks.Pop();
            block.Move(targetTower.Position);
            Position.Top += block.Height;
            targetTower.AddBlock(block);
            return block;
        }

        public void AddBlock(BuildingBlock block)
        {
            _buildingBlocks.Push(block);
            Position.Top -= block.Height;
        }
    }
}
