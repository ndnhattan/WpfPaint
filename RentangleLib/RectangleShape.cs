
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using MyLib;

namespace RentangleLib
{
    public class RectangleShape() : IShape
    {
        public override Brush Color { get; set; }
        public override double Size { get; set; }
        public override DoubleCollection DashArray { get; set; }
        public override SolidColorBrush Fill { get; set; } = Brushes.Transparent;

        public override IShape Clone()
        {
            return new RectangleShape();
        }

        public override UIElement Draw()
        {
            // TODO: can dam bao Diem 0 < Diem 1
            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);

            var element = new System.Windows.Shapes.Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = Color,
                StrokeThickness = Size,
                StrokeDashArray = DashArray,
                Fill = Fill
            };
            if (Points[0].X <= Points[1].X && Points[0].Y <= Points[1].Y)
            {
                Canvas.SetLeft(element, Points[0].X);
                Canvas.SetTop(element, Points[0].Y);
            }
            else if (Points[0].X >= Points[1].X && Points[0].Y <= Points[1].Y)
            {
                Canvas.SetLeft(element, Points[1].X);
                Canvas.SetTop(element, Points[0].Y);
            }
            else if (Points[0].X <= Points[1].X && Points[0].Y >= Points[1].Y)
            {
                Canvas.SetLeft(element, Points[0].X);
                Canvas.SetTop(element, Points[1].Y);
            }
            else if (Points[0].X >= Points[1].X && Points[0].Y >= Points[1].Y)
            {
                Canvas.SetLeft(element, Points[1].X);
                Canvas.SetTop(element, Points[1].Y);
            }

            return element;
        }

        public override string Name => "Rectangle";
    }

}
