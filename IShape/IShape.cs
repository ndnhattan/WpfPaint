
using System.Windows;
using System.Windows.Media;

namespace MyLib
{
    public abstract class IShape
    {
        public abstract Brush Color { get; set; }
        public abstract double Size { get; set; }
        public abstract DoubleCollection DashArray { get; set; }

        public abstract string Name { get; }
        public List<Point> Points { get; set; } = new List<Point>();
        public abstract SolidColorBrush Fill { get; set; }

        public abstract UIElement Draw();
        public abstract IShape Clone();
    }
}
