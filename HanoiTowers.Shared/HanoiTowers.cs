using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanoiTowers
{
    public class HanoiTowers
    {
        public event EventHandler BlockMoved;

        public void Move(string source, string destination, string temp, int amount)
        {
            if (amount > 0)
            {
                //Move (amount - 1) blocks from source to temp
                Move(source, temp, destination, amount - 1);

                //Move 1 block from source to destination
                var args = new BlockMovedEventArgs(source, destination);
                BlockMoved(this, args);

                //Move (amount - 1) blocks from temp to destination
                Move(temp, destination, source, amount - 1);
            }
        }
    }
}
