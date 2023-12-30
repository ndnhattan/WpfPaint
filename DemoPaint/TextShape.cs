using MyLib;
using RentangleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace DemoPaint
{
    internal class TextShape : IShape
    {
        public override Brush Color { get; set; }
        public override double Size { get; set; }
        public override DoubleCollection DashArray { get; set; }
        public override SolidColorBrush Fill { get; set; } = Brushes.Transparent;

        public double Top { get; set; } 
        public double Left { get; set; }
        public string Text { get; set; }    

        public override IShape Clone()
        {
            return new TextShape();
        }

        public override UIElement Draw()
        {
            var element = new TextBlock();
            element.FontSize = 20 * Size;
            element.Text = Text;
            element.Foreground = Color;

            return element;
        }

        public override string Name => "Text";
    }

}
