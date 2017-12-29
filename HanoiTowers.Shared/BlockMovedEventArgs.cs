using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanoiTowers
{
    public class BlockMovedEventArgs : EventArgs
    {
        public BlockMovedEventArgs(string sourceTower, string destinationTower)
        {
            SourceTower = sourceTower;
            DestinationTower = destinationTower;
        }

        public string SourceTower { get; private set; }
        public string DestinationTower { get; private set; }
    }
}
