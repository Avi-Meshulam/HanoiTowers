using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanoiTowers
{
    class Program
    {
        private static int _stepsCounter = 0;

        static void Main(string[] args)
        {
            var hanoiTowers = new HanoiTowers();
            hanoiTowers.BlockMoved += HanoiTowers_BlockMoved;

            while (true)
            {
                Utils.ColorConsoleWrite("Please enter number of blocks in source tower: ");
                int blocksCount = Utils.GetInteger();
                Console.WriteLine();

                _stepsCounter = 0;
                hanoiTowers.Move("source", "dest", "temp", blocksCount);

                Console.WriteLine($"\nFinished in {_stepsCounter} steps");

                if (!Utils.AskYesNo("Another one?"))
                    break;

                Console.WriteLine();
            }
            
            hanoiTowers.BlockMoved -= HanoiTowers_BlockMoved;

#if DEBUG
            Console.WriteLine("\nPress any key to continue . . .");
            Console.ReadKey(true);
#endif
        }

        private static void HanoiTowers_BlockMoved(object sender, EventArgs e)
        {
            var args = (BlockMovedEventArgs)e;
            Console.WriteLine("{0,-6} -> {1}", args.SourceTower, args.DestinationTower);
            _stepsCounter++;
        }
    }
}
