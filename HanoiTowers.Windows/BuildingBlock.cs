using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace HanoiTowers
{
    class BuildingBlock
    {
        public double Width { get; private set; }
        public double Height { get; private set; }
        public Position Position { get; private set; }
        public Image Image { get; private set; }

        public BuildingBlock(double width, double height, 
            Position position, Image image)
        {
            Width = width;
            Height = height;
            Position = position;
            Image = image;
        }

        public void Move(Position position)
        {
            Position.Top = position.Top - Height;
            Position.Center = position.Center;
        }
    }
}
