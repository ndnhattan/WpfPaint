    
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using MyLib;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Windows.Media.Media3D;

namespace EllipseLib
{

    public class EllipseShape() : IShape
    {
        public override Brush Color { get; set; }
        public override double Size { get; set; }
        public override DoubleCollection DashArray { get; set; }
        public override SolidColorBrush Fill { get; set; } = Brushes.Transparent;

        public UIElement element { get; set; }

        public override string Name => "Ellipse";

        public override IShape Clone()
        {
            return new EllipseShape();
        }

        public override UIElement Draw()
        {
            // TODO: can dam bao Diem 0 < Diem 1
            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);

            element = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = Color,
                StrokeThickness = Size,
                StrokeDashArray = DashArray,
                Fill = Fill,
                RenderTransform = new RotateTransform()
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

        
    }
}
