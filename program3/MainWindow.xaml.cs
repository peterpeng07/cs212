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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FernNamespace
{
    /*
     * CS 212 Program 3, Oct 2020
     * this class draws a fractal fern prototype, a simple fractal fern, and a doodle shape
     * with slidebars and randomness
     */
    class Fern
    {
        private static double DELTATHETA = 0.1;
        private static double SEGLENGTH = 3;
        Random random = new Random();

        public Fern(double size, double depth, double turnbias, Canvas canvas)
        {
            canvas.Children.Clear();                                // delete old canvas contents

            sun(canvas);                                            //  draw the sun
            simple_fern((int)(canvas.Width / 2), (int)(canvas.Height - 30), size, depth, turnbias, canvas);     // draw the simple fractal fern
            protoype((int)(canvas.Width / 6), (int)(canvas.Height-30), (int)size, canvas);                      // draw the fern prototype
            doodle((int)(canvas.Width / 5 * 4), (int)(canvas.Height / 2), 250, 5, canvas);                      // draw the circles doodole shape
        }

        /*
         * this function draws the sun on the top left corner using an image.
         * the size of the sun is randomized.
         */
        private void sun(Canvas canvas)
        {
            int size = random.Next(100, 200);
            // Create Image Element
            Image myImage = new Image();
            myImage.Width = size;

            // Create source
            BitmapImage myBitmapImage = new BitmapImage();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(@"https://townsquare.media/site/725/files/2020/01/sun.jpg?w=980&q=75", UriKind.RelativeOrAbsolute);

            // To save significant application memory, set the DecodePixelWidth or
            // DecodePixelHeight of the BitmapImage value of the image source to the desired
            // height or width of the rendered image. If you don't do this, the application will
            // cache the image as though it were rendered as its normal size rather then just
            // the size that is displayed.
            // Note: In order to preserve aspect ratio, set DecodePixelWidth
            // or DecodePixelHeight but not both.
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            //set image source
            myImage.Source = myBitmapImage;
            // add image to canvas
            canvas.Children.Add(myImage);

        }

        /*
         * this function draws a simple fern recursively
         * size controls the overall size; redux controls how much smaller children clusters are compared to parents,
         * turnbias controls how likely it'll turn towards left (the sun)
         */
        private void simple_fern(int x, int y, double size, double redux, double turnbias, Canvas canvas)
        {
            // store the coordinates of the middle stem
            List<int> final = new List<int>();
            // draw three branches
            for (int i = 0; i < 3; i++)
            {
                double theta = i * Math.PI / 4;
                // make stem shorter than the other two branches
                if (i==1)
                    final = fern_leaf(x, y, size/1.75, turnbias, theta, i, canvas);
                else
                    fern_leaf(x, y, size, turnbias, theta, i, canvas);  

            }
            // recursively draw at the end of the middle stem
            if (size > 10)
                simple_fern(final[0], final[1], size / redux, redux, turnbias, canvas);
        }

        /*
         * this function draws the branches and leaves on branches
         * the direction of growth is randomized
         * returns the coordinates of the end point for recursion
         */
        private List<int> fern_leaf(int x1, int y1, double size, double turnbias, double direction, int index, Canvas canvas)
        {
            int x2 = x1, y2 = y1;
            int x3, y3;
            int factor = 2;
            Random random = new Random();

            for (int i = 0; i < size; i++)
            {
                // draws the branches
                direction += (random.NextDouble() > turnbias) ? -1 * DELTATHETA : DELTATHETA;
                x1 = x2; y1 = y2;
                x2 = x1 - (int)(SEGLENGTH * Math.Sin(direction));
                y2 = y1 - (int)(SEGLENGTH * Math.Cos(direction));
                line(x1, y1, x2, y2, 1 + size / 80, canvas);

                // draws the leaves
                if (i % 5 == 0 & index != 1 & i != 0)
                {
                    // top
                    x3 = x2 - (int)(size / factor * Math.Cos(direction - 0.785));
                    y3 = y2 - (int)(size / factor * Math.Sin(direction + 0.785));
                    line(x2, y2, x3, y3, 1 + size / 80, canvas);

                    // bottom
                    x3 = x2 + (int)(size / factor * Math.Cos(direction + 0.785));
                    y3 = y2 + (int)(size / factor * Math.Sin(direction - 0.785));
                    line(x2, y2, x3, y3, 1 + size / 80, canvas);

                    factor += 1;
                }

            }

            // stores coordinates of the endpoint
            List<int> location = new List<int>(2);
            location.Add(x2);
            location.Add(y2);

            return location;
        }

        /*
         * this function recursively draws the prototype of the fractal fern without randomness
         */
        private void protoype(int x, int y, int size, Canvas canvas)
        {
            int x2 = x, x1 = x;
            int y2 = y, y1 = y;

            if (size > 10)
            {
                // draws the branches
                x2 = x1 + size;
                y2 = y1 - size;
                line(x1, y1, x2, y2, 2, canvas);
                x2 = x1 - size;
                line(x1, y1, x2, y2, 2, canvas);
                line(x1, y1, x1, y2, 2, canvas);

                size = (int)(size * 0.8);

                // draw the leaves on branches
                leaves_left(x1, y1, size, canvas);
                leaves_right(x1, y1, size, canvas);

                // recursively draw on the endpoint
                protoype(x1, y2, size, canvas);
            }
        }

        /*
         * this function draws the leaves on the left side of the branch of the prototype
         */
        private void leaves_left(int x, int y, int size, Canvas canvas)
        {
            int x1, y1, x2, y2;

            x1 = x - (int)size / 3;
            y1 = y - (int)size / 3;
            x2 = x1 - (int)size / 2;
            y2 = y1 - (int)size / 2;

            line(x1, y1, x1, y2, 2, canvas);
            line(x1, y1, x2, y1, 2, canvas);

            size = (int)(size * 0.75);
            if (size > 10)
                leaves_left(x1, y1, size, canvas);
        }

        /*
         * this function draws the leaves on the right side of the branch of the prototype
         */
        private void leaves_right(int x, int y, int size, Canvas canvas)
        {
            int x3, y3, x4, y4;

            x3 = x + (int)size / 3;
            y3 = y - (int)size / 3;
            x4 = x3 + (int)size / 2;
            y4 = y3 - (int)size / 2;

            line(x3, y3, x3, y4, 2, canvas);
            line(x3, y3, x4, y3, 2, canvas);
          
            size = (int)(size * 0.75);
            if (size > 10)
                leaves_right(x3, y3, size, canvas);
        }


        /*
         * this function doodles recursively circles with random colors 
         */
        private void doodle(int x, int y, int width, int depth, Canvas canvas)
        {
            // generate random color
            Byte[] color = new Byte[3];
            random.NextBytes(color);

            // draw circle in the center
            circle(x, y, width / 4, color[0], color[1], color[2], canvas);

            // calculate coordinates of the four corners
            int x1 = x - width / 4, x2 = x + width / 4;
            int y1 = y - width / 4, y2 = y + width / 4;

            // recursively draw circles in the corners
            if (depth > 0)
            {
                doodle(x1, y1, width * 5 / 10, depth - 1, canvas);
                doodle(x2, y2, width * 5 / 10, depth - 1, canvas);
                doodle(x1, y2, width * 5 / 10, depth - 1, canvas);
                doodle(x2, y1, width * 5 / 10, depth - 1, canvas);
            }
        }

        /*
         * draw a line segment (x1,y1) to (x2,y2) with given color, thickness on canvas
         */
        private void line(int x1, int y1, int x2, int y2, double thickness, Canvas canvas)
        {
            Line myLine = new Line();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 60, 145, 50);
            myLine.X1 = x1;
            myLine.Y1 = y1;
            myLine.X2 = x2;
            myLine.Y2 = y2;
            myLine.Stroke = mySolidColorBrush;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.StrokeThickness = thickness;
            canvas.Children.Add(myLine);
        }

        /*
         * draw a red circle centered at (x,y), radius radius, onto canvas
         */
        private void circle(int x, int y, double radius, byte r, byte g, byte b, Canvas canvas)
        {
            Ellipse myEllipse = new Ellipse();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, r, g, b);
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
            myEllipse.VerticalAlignment = VerticalAlignment.Center;
            myEllipse.Width = 2 * radius;
            myEllipse.Height = 2 * radius;
            myEllipse.SetCenter(x, y);
            canvas.Children.Add(myEllipse);
        }

    }
}

/*
 * this class is needed to enable us to set the center for an ellipse (not built in?!)
 */
public static class EllipseX
{
    public static void SetCenter(this Ellipse ellipse, double X, double Y)
    {
        Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
        Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
    }
}

