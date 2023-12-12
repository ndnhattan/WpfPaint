using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DemoPaint.Features.LoadSaveImage;
using MyLib;
using Fluent;
using EllipseLib;
using RentangleLib;
using LineLib;

namespace DemoPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        Brush color = Brushes.Black;
        double size = 1;
        DoubleCollection dashArray = null;
        private IShape _selectedShape;
        bool isInFillMode = false;
        bool textMode = false;  

        public MainWindow()
        {
            InitializeComponent();
        }

        ShapeFactory _factory;
        Image img;
        private string _selectedButtonName;
        private double zoomFactor = 1.2;

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scale = e.Delta > 0 ? 1.1 : 1 / 1.1;

            // Tính toán vị trí của con trỏ chuột trong không gian Canvas
            var mousePosition = e.GetPosition(drawingCanvas);

            // Nếu đang ở trạng thái zoom in (tức tỷ lệ zoom khác 1)
            if (Math.Abs(canvasScaleTransform.ScaleX - 1) > double.Epsilon)
            {
                // Di chuyển con trỏ chuột về tâm Canvas
                canvasScaleTransform.CenterX = mousePosition.X;
                canvasScaleTransform.CenterY = mousePosition.Y;
            }

            // Thay đổi tỷ lệ của ScaleTransform
            canvasScaleTransform.ScaleX *= scale;
            canvasScaleTransform.ScaleY *= scale;

            // Giới hạn tỷ lệ zoom 
            const double minScale = 1.0;
            const double maxScale = 5.0;

            if (canvasScaleTransform.ScaleX < minScale)
                canvasScaleTransform.ScaleX = minScale;
            if (canvasScaleTransform.ScaleY < minScale)
                canvasScaleTransform.ScaleY = minScale;

            if (canvasScaleTransform.ScaleX > maxScale)
                canvasScaleTransform.ScaleX = maxScale;
            if (canvasScaleTransform.ScaleY > maxScale)
                canvasScaleTransform.ScaleY = maxScale;

            // Ẩn StackPanel khi tỉ lệ zoom khác 1
            //actionsStackPanel.Visibility = Math.Abs(canvasScaleTransform.ScaleX - 1) < double.Epsilon ? Visibility.Visible : Visibility.Collapsed;

            // Nếu đang ở trạng thái zoom in (tức tỷ lệ zoom khác 1)
            if (Math.Abs(canvasScaleTransform.ScaleX - 1) > double.Epsilon)
            {
                // Đảm bảo rằng giữ nguyên vị trí con trỏ chuột sau khi thay đổi tỷ lệ
                canvasScaleTransform.CenterX = mousePosition.X;
                canvasScaleTransform.CenterY = mousePosition.Y;
            }

            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var abilities = new List<IShape>();

            // Do tim cac kha nang
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var fis = (new DirectoryInfo(folder)).GetFiles("*.dll");

            foreach (var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass & (!type.IsAbstract))
                    {
                        if (typeof(IShape).IsAssignableFrom(type))
                        {
                            var shape = Activator.CreateInstance(type) as IShape;
                            abilities.Add(shape!);
                        }
                    }
                }
            }

            _factory = new ShapeFactory();
            foreach (var ability in abilities)
            {
                _factory.Prototypes.Add(
                    ability.Name, ability
                );

                Fluent.Button button = new Fluent.Button()
                {
                    Tag = ability.Name,
                    Header = ability.Name,
                };
                //Them anh
                Image image = new Image();
                if (ability.Name == "Line")
                {
                    image.Source = new BitmapImage(new Uri("assets/line.png", UriKind.Relative));
                }
                else if(ability.Name == "Ellipse")
                {
                    image.Source = new BitmapImage(new Uri("assets/ellipse.png", UriKind.Relative));
                }
                else if (ability.Name == "Rectangle")
                {
                    image.Source = new BitmapImage(new Uri("assets/rectangle.png", UriKind.Relative));
                }
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
                button.LargeIcon = image;

                button.Click += (sender, args) =>
                {
                    var control = (System.Windows.Controls.Button)sender;
                    _choice = (string)control.Tag;
                };
                RibbonShape.Items.Add(button);
            };

            if (abilities.Count > 0)
            {
                _choice = abilities[0].Name;
                _selectedButtonName = _choice;
            }
            //btnUndo.IsEnabled = false;
            //btnRedo.IsEnabled = false;
        }

        bool isDrawing = false;
        Point _start;
        Point _end;
        string _choice; // Line
        List<IShape> _shapes = new List<IShape>();
        List<IShape> _newShapes=new List<IShape>();

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (textMode)
            {
                btnAddText.Content = "Add Text";
                Cursor = Cursors.Arrow;
                textMode = false;

                string text = textInput.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    TextBlock newText = new TextBlock();
                    newText.Text = text;
                    newText.FontSize = 20;
                    newText.Foreground = Brushes.Black; // Change color as needed

                    Point position = Mouse.GetPosition(drawingCanvas);
                    Canvas.SetLeft(newText, position.X);
                    Canvas.SetTop(newText, position.Y);

                    drawingCanvas.Children.Add(newText);
                }
            }
            else if (isInFillMode)
            {
                _selectedShape = SelectShapeAtPoint(e.GetPosition(drawingCanvas));

                // Kiểm tra xem có đối tượng được chọn hay không
                if (_selectedShape != null)
                {
                    // Thay đổi màu của Fill khi chọn
                    Color selectedColor = GetSelectedFillColor();
                    _selectedShape.Fill = new SolidColorBrush(selectedColor);

                    drawingCanvas.Children.Clear();

                    if (img != null)
                    {
                        drawingCanvas.Children.Add(img);
                    }

                    foreach (var shape in _shapes)
                    {
                        drawingCanvas.Children.Add(shape.Draw());
                    }
                }
            }
            else
            {
                isDrawing = true;
                _start = e.GetPosition(drawingCanvas);
            }
        }

        // Hàm để chọn một đối tượng tại một điểm xác định
        private IShape SelectShapeAtPoint(Point point)
        {
            foreach (var shape in _shapes)
            {
                if (shape is EllipseShape ellipse && IsPointInsideEllipse(point, ellipse))
                {
                    return ellipse;
                }
                else if (shape is RectangleShape rectangle && IsPointInsideRectangle(point, rectangle))
                {
                    return rectangle;
                }
                else if (shape is LineShape line && IsPointInsideLine(point, line))
                {
                    return line;
                }
            }

            return null; // Không có đối tượng nào được chọn
        }

        private bool IsPointInsideEllipse(Point point, EllipseShape ellipse)
        {
            if (ellipse.Points[0].X < ellipse.Points[1].X)
            {
                if (ellipse.Points[0].Y >= ellipse.Points[1].Y)
                {
                    if (point.X >= ellipse.Points[0].X && point.X <= ellipse.Points[1].X && point.Y >= ellipse.Points[1].Y && point.Y <= ellipse.Points[0].Y)
                    {
                        return true;
                    }
                }
                else
                {
                    if (point.X >= ellipse.Points[0].X && point.X <= ellipse.Points[1].X && point.Y >= ellipse.Points[0].Y && point.Y <= ellipse.Points[1].Y)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (ellipse.Points[0].Y >= ellipse.Points[1].Y)
                {
                    if (point.X >= ellipse.Points[1].X && point.X <= ellipse.Points[0].X && point.Y >= ellipse.Points[1].Y && point.Y <= ellipse.Points[0].Y)
                    {
                        return true;
                    }
                }
                else
                {
                    if (point.X >= ellipse.Points[1].X && point.X <= ellipse.Points[0].X && point.Y >= ellipse.Points[0].Y && point.Y <= ellipse.Points[1].Y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsPointInsideRectangle(Point point, RectangleShape rectangle)
        {
            if (rectangle.Points[0].X < rectangle.Points[1].X)
            {
                if (rectangle.Points[0].Y >= rectangle.Points[1].Y)
                {
                    if (point.X >= rectangle.Points[0].X && point.X <= rectangle.Points[1].X && point.Y >= rectangle.Points[1].Y && point.Y <= rectangle.Points[0].Y)
                    {
                        return true;
                    }
                }
                else
                {
                    if (point.X >= rectangle.Points[0].X && point.X <= rectangle.Points[1].X && point.Y >= rectangle.Points[0].Y && point.Y <= rectangle.Points[1].Y)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (rectangle.Points[0].Y >= rectangle.Points[1].Y)
                {
                    if (point.X >= rectangle.Points[1].X && point.X <= rectangle.Points[0].X && point.Y >= rectangle.Points[1].Y && point.Y <= rectangle.Points[0].Y)
                    {
                        return true;
                    }
                }
                else
                {
                    if (point.X >= rectangle.Points[1].X && point.X <= rectangle.Points[0].X && point.Y >= rectangle.Points[0].Y && point.Y <= rectangle.Points[1].Y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsPointInsideLine(Point point, LineShape line)
        {
            if (line.Points.Count >= 2)
            {
                double minX = Math.Min(line.Points[0].X, line.Points[1].X);
                double maxX = Math.Max(line.Points[0].X, line.Points[1].X);
                double minY = Math.Min(line.Points[0].Y, line.Points[1].Y);
                double maxY = Math.Max(line.Points[0].Y, line.Points[1].Y);

                if (point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY)
                {
                    return true;
                }
            }
            return false;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                _end = e.GetPosition(drawingCanvas);

                Title = $"{_start.X}, {_start.Y} => {_end.X}, {_end.Y}";

                IShape preview = _factory.Create(_choice);
                preview.Points.Add(_start);
                preview.Points.Add(_end);
                preview.Color = color;
                preview.Size = size;
                preview.DashArray = dashArray;

                drawingCanvas.Children.Clear();
                if (img != null)
                {
                    drawingCanvas.Children.Add(img);
                }

                foreach (var shape in _shapes)
                {
                    drawingCanvas.Children.Add(shape.Draw());
                }

                drawingCanvas.Children.Add(preview.Draw());
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing)
            {
                IShape shape = _factory.Create(_choice);
                shape.Points.Add(_start);
                shape.Points.Add(_end);
                shape.Color = color;
                shape.Size = size;
                shape.DashArray = dashArray;

                _shapes.Add(shape);
                _newShapes.Add(shape);
                isDrawing = false;
            }
        }

       
        private void btnSavePNG_Click(object sender, RoutedEventArgs e)
        {
            LoadSavePNG.SaveCanvasToPng(drawingCanvas); // Replace 'canvasToSave' with your Canvas name
        }

        private void btnLoadPNG_Click(object sender, RoutedEventArgs e)
        {
            LoadSavePNG.LoadPngToCanvas(drawingCanvas, ref img);
        }

        private void SaveShapesCustomBinary(string filePath)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
                {
                    // Lưu số lượng đối tượng
                    writer.Write(_shapes.Count);

                    foreach (var shape in _shapes)
                    {
                        // Lưu tên đối tượng
                        writer.Write(shape.Name);

                        // Lưu màu sắc
                        Color color = ((SolidColorBrush)shape.Color).Color;
                        writer.Write(color.A);
                        writer.Write(color.R);
                        writer.Write(color.G);
                        writer.Write(color.B);

                        // Lưu kích thước
                        writer.Write(shape.Size);

                        // Lưu số lượng DashArray (nếu có)
                        writer.Write(shape.DashArray != null ? shape.DashArray.Count : 0);

                        // Nếu DashArray không phải là null, lưu từng giá trị của nó
                        if (shape.DashArray != null)
                        {
                            foreach (double dashValue in shape.DashArray)
                            {
                                writer.Write(dashValue);
                            }
                        }

                        // Lưu số lượng điểm
                        writer.Write(shape.Points.Count);

                        foreach (var point in shape.Points)
                        {
                            // Lưu từng điểm
                            writer.Write(point.X);
                            writer.Write(point.Y);
                        }

                        // Lưu thuộc tính Fill
                        if (shape.Fill is SolidColorBrush fillBrush)
                        {
                            Color fillColor = fillBrush.Color;
                            writer.Write(fillColor.A);
                            writer.Write(fillColor.R);
                            writer.Write(fillColor.G);
                            writer.Write(fillColor.B);
                        }
                        else
                        {
                            // Nếu không có màu Fill, lưu giá trị mặc định
                            writer.Write(Colors.Transparent.A);
                            writer.Write(Colors.Transparent.R);
                            writer.Write(Colors.Transparent.G);
                            writer.Write(Colors.Transparent.B);
                        }
                    }

                    MessageBox.Show("Shapes saved successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving shapes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadShapesCustomBinary(string filePath)
        {
            try
            {
                _shapes.Clear();

                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    // Đọc số lượng đối tượng
                    int shapesCount = reader.ReadInt32();

                    for (int i = 0; i < shapesCount; i++)
                    {
                        // Đọc tên đối tượng
                        string shapeName = reader.ReadString();

                        // Tạo đối tượng hình dựa trên tên
                        var shape = _factory.Create(shapeName);

                        // Đọc màu sắc
                        byte a = reader.ReadByte();
                        byte r = reader.ReadByte();
                        byte g = reader.ReadByte();
                        byte b = reader.ReadByte();
                        Color color = Color.FromArgb(a, r, g, b);
                        shape.Color = new SolidColorBrush(color);

                        // Đọc kích thước
                        shape.Size = reader.ReadDouble();

                        // Đọc DashArray
                        int dashArrayCount = reader.ReadInt32();
                        if (dashArrayCount > 0)
                        {
                            DoubleCollection dashArray = new DoubleCollection();
                            for (int j = 0; j < dashArrayCount; j++)
                            {
                                dashArray.Add(reader.ReadDouble());
                            }
                            shape.DashArray = dashArray;
                        }
                        else
                        {
                            // Đặt DashArray là null nếu dashArrayCount là 0
                            shape.DashArray = null;
                        }

                        // Đọc số lượng điểm
                        int pointsCount = reader.ReadInt32();

                        for (int j = 0; j < pointsCount; j++)
                        {
                            // Đọc từng điểm
                            double x = reader.ReadDouble();
                            double y = reader.ReadDouble();
                            shape.Points.Add(new Point(x, y));
                        }

                        // Đọc thuộc tính Fill
                        byte fillA = reader.ReadByte();
                        byte fillR = reader.ReadByte();
                        byte fillG = reader.ReadByte();
                        byte fillB = reader.ReadByte();
                        Color fillColor = Color.FromArgb(fillA, fillR, fillG, fillB);
                        SolidColorBrush fillBrush = new SolidColorBrush(fillColor);
                        shape.Fill = fillBrush;

                        // Thêm hình vào danh sách
                        _shapes.Add(shape);
                    }
                }

                // Vẽ lại Canvas
                drawingCanvas.Children.Clear();

                foreach (var shape in _shapes)
                {
                    drawingCanvas.Children.Add(shape.Draw());
                }

                MessageBox.Show("Shapes loaded successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading shapes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveBin_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "shapes"; // Tên mặc định cho file
            dlg.DefaultExt = ".bin"; // Định dạng mặc định
            dlg.Filter = "Binary Files (.bin)|*.bin"; // Bộ lọc file

            // Hiển thị hộp thoại Save File Dialog
            bool? result = dlg.ShowDialog();

            // Kiểm tra xem người dùng đã chọn một tập tin chưa
            if (result == true)
            {
                // Lấy đường dẫn tập tin được chọn
                string filePath = dlg.FileName;

                // Gọi hàm lưu
                SaveShapesCustomBinary(filePath);
            }
        }

        private void btnLoadBin_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".bin"; // Định dạng mặc định
            dlg.Filter = "Binary Files (.bin)|*.bin"; // Bộ lọc file

            // Hiển thị hộp thoại Open File Dialog
            bool? result = dlg.ShowDialog();

            // Kiểm tra xem người dùng đã chọn một tập tin chưa
            if (result == true)
            {
                // Lấy đường dẫn tập tin được chọn
                string filePath = dlg.FileName;

                // Gọi hàm load
                LoadShapesCustomBinary(filePath);
            }
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (colorComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                Color selectedColor = (selectedItem.Background as SolidColorBrush)?.Color ?? Colors.Black;
                /*if (colorPreview != null)
                {

                colorPreview.Fill = new SolidColorBrush(selectedColor);
                }*/

                color = new SolidColorBrush(selectedColor);
            }
        }

        private void brushesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)brushesListBox.SelectedItem;
            if (selectedItem != null && selectedItem.Content != null)
            {
                string selectedDash = selectedItem.Content.ToString();
                DoubleCollection dashArray = new DoubleCollection();
                switch (selectedDash)
                {
                    case "Line":
                        // Solid line: no dashes, just a single space
                        dashArray = null;
                        break;
                    case "Dashed":
                        // Dashed pattern: 4 units of dash, 2 units of gap
                        dashArray.Add(4);
                        dashArray.Add(2);
                        break;
                    case "Dotted":
                        // Dotted pattern: 1 unit of dash, 1 unit of gap
                        dashArray.Add(1);
                        dashArray.Add(1);
                        break;
                    // Add more cases or customize patterns as needed
                    default:
                        break;
                }

                // Apply the created dash pattern to the StrokeDashArray property
                this.dashArray = dashArray;
            }

        }

        private void sizesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sizesListBox.SelectedItem != null)
            {
                ComboBoxItem item = ((ComboBoxItem)sizesListBox.SelectedItem);
                if (item != null && item.Content != null)
                {
                    string selectedSize = ((ComboBoxItem)sizesListBox.SelectedItem).Content.ToString();

                size = double.Parse(new string(selectedSize.Where(char.IsDigit).ToArray()));
                }
            }
        }

        private void BackstageTabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to quit?", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void SavePNG_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoadSavePNG.SaveCanvasToPng(drawingCanvas);
        }

        private void LoadPNG_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoadSavePNG.LoadPngToCanvas(drawingCanvas, ref img);
        }

        private void LoadBinary_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".bin"; // Định dạng mặc định
            dlg.Filter = "Binary Files (.bin)|*.bin"; // Bộ lọc file

            // Hiển thị hộp thoại Open File Dialog
            bool? result = dlg.ShowDialog();

            // Kiểm tra xem người dùng đã chọn một tập tin chưa
            if (result == true)
            {
                // Lấy đường dẫn tập tin được chọn
                string filePath = dlg.FileName;

                // Gọi hàm load
                LoadShapesCustomBinary(filePath);
            }
        }

        private void SaveBinary_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "shapes"; // Tên mặc định cho file
            dlg.DefaultExt = ".bin"; // Định dạng mặc định
            dlg.Filter = "Binary Files (.bin)|*.bin"; // Bộ lọc file

            // Hiển thị hộp thoại Save File Dialog
            bool? result = dlg.ShowDialog();

            // Kiểm tra xem người dùng đã chọn một tập tin chưa
            if (result == true)
            {
                // Lấy đường dẫn tập tin được chọn
                string filePath = dlg.FileName;

                // Gọi hàm lưu
                SaveShapesCustomBinary(filePath);
            }
        }
        private void Canvas_LayoutUpdated(object sender, EventArgs e)
        {
            if(_shapes.Count > 0 && _shapes.Count < _newShapes.Count)
            {
                btnUndo.IsEnabled = true;
                btnRedo.IsEnabled = true;
            }
            if(_shapes.Count == _newShapes.Count && _shapes.Count!=0)
            {
                btnUndo.IsEnabled = true;
                btnRedo.IsEnabled = false;
            }
            if (_shapes.Count == 0)
            {
                btnUndo.IsEnabled = false;
                btnRedo.IsEnabled = true;
            }
            if (_shapes.Count == 0 && _newShapes.Count == 0)
            {
                btnUndo.IsEnabled = false;
                btnRedo.IsEnabled = false;
            }
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Children.Clear();
            _shapes.RemoveAt(_shapes.Count-1);
            for (int i = 0; i < _shapes.Count; i++)
            {
                drawingCanvas.Children.Add(_shapes[i].Draw());
            }
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Children.Clear();
            _shapes.Add(_newShapes[_shapes.Count]);
            for (int i = 0; i < _shapes.Count; i++)
            {
                drawingCanvas.Children.Add(_shapes[i].Draw());
            }
        }

        private void btnFill_Click(object sender, RoutedEventArgs e)
        {
            if (isInFillMode)
            {
                btnFill.Content = "Fill";
                Cursor = Cursors.Arrow;
                isInFillMode = false;
            }
            else
            {
                btnFill.Content = "Cancel";
                Cursor = Cursors.Pen;
                isInFillMode = true;
            }
        }

        private void ColorFillComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private Color GetSelectedFillColor()
        {
            if (colorFillComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                return (selectedItem.Background as SolidColorBrush)?.Color ?? Colors.White;
            }

            return Colors.White; // Màu mặc định nếu không chọn được
        }

        private void AddText_Click(object sender, RoutedEventArgs e)
        {
            if (textMode)
            {
                btnAddText.Content = "Add Text";
                Cursor = Cursors.Arrow;
                textMode = false;
            }
            else
            {
                btnAddText.Content = "Cancel";
                Cursor = Cursors.Pen;
                textMode = true;
            }
        }
    }
}