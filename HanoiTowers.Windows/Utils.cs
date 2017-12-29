using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanoiTowers
{
    class Utils
    {
        private static Random _rand = new Random();

        public static int GetRandom(int minValue, int maxValue)
        {
            return _rand.Next(minValue, maxValue);
        }

        public static int GetRandom(int maxValue)
        {
            return GetRandom(0, maxValue);
        }

        public static int GetRandom()
        {
            return GetRandom(0, int.MaxValue);
        }

        public static bool IsNumber(string num)
        {
            int n;
            return int.TryParse(num, out n);
        }
    }
}
