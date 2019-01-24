using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Numerics;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Rect area= new Rect(new Point(-2.4, -1.5), new Point(0.8, 1.5));
        private Rectangle selection = new Rectangle() { Stroke = Brushes.Black, StrokeThickness = 1, Visibility = Visibility.Collapsed };
        private bool mousedown = false;
        private Point mousedownpos;

        public MainWindow()
        {
            InitializeComponent();
            image1.Source = drawSet(area);
            Canvas1.Children.Add(selection);

        }
      
       
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            area = new Rect(new Point(-2.4, -1.5), new Point(0.8, 1.5));

            image1.Source = drawSet(area);

        }


        WriteableBitmap drawSet(Rect area)
        {
            int PixelHeight = (int) image1.Height;
            int PixelWidth = (int) image1.Width;
            WriteableBitmap wbmap = new WriteableBitmap(PixelWidth, PixelHeight, 96, 96, PixelFormats.Bgra32, null);
            
            int BytesPerPixel = wbmap.Format.BitsPerPixel / 8;
            byte[] pixels = new byte[PixelHeight * PixelWidth * BytesPerPixel];
            
            int s = PixelWidth * BytesPerPixel;

            double xscale = (area.Right - area.Left) / PixelWidth;
            double yscale = (area.Top - area.Bottom) / PixelHeight;
            for (int i = 0; i < pixels.Length; i += BytesPerPixel)
            {
                int py = i / s;
                int px = i % s / BytesPerPixel;
                double x = area.Left + px * xscale;
                double y = area.Top - py * yscale;
                Complex c = new Complex(x,y);
                int count = mandelbrot(c);
                Color C = colorMap(count);
                pixels[i] = C.B;
                pixels[i+1] = C.G;
                pixels[i + 2] = C.R;
                pixels[i + 3] = C.A;
                
            }
            wbmap.WritePixels(new Int32Rect(0, 0, PixelWidth, PixelHeight), pixels, s, 0);
            return wbmap;

        }

        Color colorMap(int count)
        {
            Color C = new Color();
            C.B =(byte) ( count/ 100 * 25);
            count = count % 100;
            C.G = (byte)(count / 10 * 25);
            C.R = (byte)(count % 10 * 25);
            C.A = 255;
            return C;
        }
        Int32 mandelbrot(Complex c)
        {
            Int32 count = 0;
            Complex z = Complex.Zero;

            while (count < 1000 && z.Magnitude < 4)
            {
                z = z * z + c;
                count++;
            }
            return count;
        }

        private void Canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mousedown = true;
            mousedownpos = e.GetPosition(Canvas1);
            Canvas.SetLeft(selection, mousedownpos.X);
            Canvas.SetTop(selection, mousedownpos.Y);
            selection.Width = 0;
            selection.Height = 0;
            selection.Visibility = Visibility.Visible;
        }

    
     
      

        private void Canvas1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mousedown = false;
            selection.Visibility = Visibility.Collapsed;

            double xscale = (area.Right-area.Left) / image1.Width;
            double yscale = (area.Top-area.Bottom)/ image1.Height;
            Point TopLeft= new Point(area.Left + Canvas.GetLeft(selection) * xscale, area.Top - Canvas.GetTop(selection) * yscale);
            Point BottomRight = TopLeft+new Vector(selection.Width*xscale,-selection.Height*yscale);  
            area=new Rect(TopLeft,BottomRight);
            image1.Source = drawSet(area);
        }

        private void Canvas1_MouseMove(object sender, MouseEventArgs e)
        {
    
            if (mousedown)
            {
                Point mousepos = e.GetPosition(Canvas1);
                
                Vector diff =  mousepos-mousedownpos;
                Point TopLeft = mousedownpos;
                if (diff.X < 0)
                {
                    TopLeft.X = mousepos.X;
                    diff.X = -diff.X;
                }
                if (diff.Y < 0)
                {
                    TopLeft.Y = mousepos.Y;
                    diff.Y = -diff.Y;
                }
                selection.Width = diff.X;
                selection.Height = diff.Y;
                Canvas.SetLeft(selection, TopLeft.X);
                Canvas.SetTop(selection, TopLeft.Y);
            }


        }








    }

}
