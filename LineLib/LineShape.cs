using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using MyLib;
using System.Xml.Linq;

namespace LineLib
{
    public class LineShape() : IShape
    {
        public override IShape Clone()
        {
            return new LineShape();
        }
        public override SolidColorBrush Fill { get; set; } = Brushes.Transparent;
        public UIElement element {  get; set; }  

        public override UIElement Draw()
        {
            element= new Line()
            {
                X1 = Points[0].X,
                Y1 = Points[0].Y,
                X2 = Points[1].X,
                Y2 = Points[1].Y,
                Stroke = Color,
                StrokeThickness = Size,
                StrokeDashArray = DashArray,
                Fill = Fill,
                RenderTransform = new RotateTransform()
            };
            return element;
        }

        public override string Name => "Line";

        public override Brush Color { get; set; }
        public override double Size { get; set; }
        public override DoubleCollection DashArray { get; set; }
    }

}
