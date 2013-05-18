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
using System.Windows.Threading;

namespace SwingingAtwoodMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int loopCounter;
        DispatcherTimer timer;
        Ellipse ellipse = null;
        int x, y, i;
        double k1, k2, k3, k4, xPos, yPos;

        //constants
        Color blue = Color.FromRgb(0, 0, 255);
        Color black = Color.FromRgb(0, 0, 0);
        Color red = Color.FromRgb(255, 0, 0);
        const double g = 9.8;
        const double leftPegX = 180;
        const double leftPegY = 105;
        const double rightPegX = 355;
        const double rightPegY = 105;

        //initial conditions
        double M; // Mass of large bob
        double m; // Mass of small bob
        double theta; //Initial angle of small bob from vertical (see diagram in #region 4 equations)
        double r;
        double z;
        double L;
        double l;
        double omega;
        double v;
        double h;

        //drawn objects
        Ellipse largeBob = null;
        Ellipse smallBob = null;
        Line largeLine = null;
        Line smallLine = null;
        HashSet<Point> pointsSet = null;
        Polyline trace = null;
        PointCollection tracePoints = null;

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            double omega2, v2, theta2, r2, deltaR;

            //solve equations to give r2 and theta2 (these are the updated values from the new time-step)
            v2 = SolveEquation1(h, r, omega, M, m, theta, v);
            omega2 = SolveEquation2(h, v, omega, M, m, r, theta, omega);
            r2 = SolveEquation3(h, v, r);
            theta2 = SolveEquation4(h, omega, theta);

            //remove old pictures before drawing new ones
            PaintCanvas.Children.Remove(largeBob);
            PaintCanvas.Children.Remove(largeLine);
            PaintCanvas.Children.Remove(smallBob);
            PaintCanvas.Children.Remove(smallLine);
            PaintCanvas.Children.Remove(trace);

            //using r2 get l and the position of the large bob
            deltaR = r2 - r;
            l -= deltaR;
            largeLine = CreateLine(leftPegX, leftPegY, leftPegX, leftPegY + l, black);
            largeBob = CreateEllipse(10 * M / m, 10 * M / m, blue);

            //using theta2 and r2 get the position of the small bob
            xPos = r2 * Math.Sin(theta2) + rightPegX;
            yPos = r2 * Math.Cos(theta2) + rightPegY;
            smallLine = CreateLine(rightPegX, rightPegY, xPos, yPos, black);
            smallBob = CreateEllipse(10, 10, blue);

            //set the old values to the new values
            r = r2;
            theta = theta2;
            omega = omega2;
            v = v2;

            //draw the pictures
            PaintCanvas.Children.Add(largeLine);
            PaintCanvas.Children.Add(largeBob);
            Canvas.SetLeft(largeBob, leftPegX - 5 * M / m);
            Canvas.SetTop(largeBob, leftPegY + l - 5 * M / m);
            Canvas.SetZIndex(largeBob, 1);

            PaintCanvas.Children.Add(smallBob);
            PaintCanvas.Children.Add(smallLine);
            Canvas.SetLeft(smallBob, xPos - 5);
            Canvas.SetTop(smallBob, yPos - 5);
            Canvas.SetZIndex(smallBob, 1);

            Point p = new Point(xPos, yPos);
            if (pointsSet.Add(p))
                tracePoints.Add(p);

            SolidColorBrush redBrush = new SolidColorBrush(red);
            trace.Stroke = redBrush;
            trace.StrokeThickness = 1;
            trace.Points = tracePoints;

            PaintCanvas.Children.Add(trace);
        }

        //create an ellipse
        public Ellipse CreateEllipse(double height, double width, Color colour)
        {
            SolidColorBrush fillBrush = new SolidColorBrush(colour);

            return new Ellipse()
            {
                Height = height,
                Width = width,
                Fill = fillBrush
            };
        }

        //create a line
        public Line CreateLine(double x1, double y1, double x2, double y2, Color colour)
        {
            SolidColorBrush brush = new SolidColorBrush(colour);

            return new Line()
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                Stroke = brush,
                StrokeThickness = 2
            };
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //set initial conditions            
            M = 4; // Mass of large bob
            m = 1; // Mass of small bob
            theta = Math.PI / 2; //Initial angle of small bob from vertical (see diagram in #region 4 equations)
            omega = 0;
            v = 0;
            r = 125;
            z = 185;
            L = 445;
            h = 0.1;
            l = L - r - z;

            if (largeLine != null)
                PaintCanvas.Children.Remove(largeLine);
            if (largeBob != null)
                PaintCanvas.Children.Remove(largeBob);
            if (smallLine != null)
                PaintCanvas.Children.Remove(smallLine);
            if (smallBob != null)
                PaintCanvas.Children.Remove(smallBob);
            if (pointsSet != null)
            {
                PaintCanvas.Children.Remove(trace);
                pointsSet.Clear();
                tracePoints.Clear();
            }

            //draw the initial postion of the large bob
            largeLine = CreateLine(leftPegX, leftPegY, leftPegX, leftPegY + l, black);
            largeBob = CreateEllipse(10 * M / m, 10 * M / m, blue);
            PaintCanvas.Children.Add(largeLine);
            PaintCanvas.Children.Add(largeBob);
            Canvas.SetLeft(largeBob, leftPegX - 5 * M / m);
            Canvas.SetTop(largeBob, leftPegY + l - 5 * M / m);
            Canvas.SetZIndex(largeBob, 1);

            //draw the initial position of the small bob
            xPos = r * Math.Sin(theta) + rightPegX;
            yPos = r * Math.Cos(theta) + rightPegY;
            smallLine = CreateLine(rightPegX, rightPegY, xPos, yPos, black);
            smallBob = CreateEllipse(10, 10, blue);
            PaintCanvas.Children.Add(smallBob);
            PaintCanvas.Children.Add(smallLine);
            Canvas.SetLeft(smallBob, xPos - 5);
            Canvas.SetTop(smallBob, yPos - 5);
            Canvas.SetZIndex(smallBob, 1);

            //begin filling the set to draw the trace line
            trace = new Polyline();
            pointsSet = new HashSet<Point>();
            tracePoints = new PointCollection();
            Point p = new Point(xPos, yPos);
            pointsSet.Add(p);
            tracePoints.Add(p);

            timer.Start();

            button2.IsEnabled = true;
            button1.IsEnabled = false;
        }

        #region 4 equations

        /*
         * The four equations of motion are:
         * 
         * 1)   v'[t] = (r[t] * omega[t]^2 - M/m g + g Cos[theta]) / (M/m + 1)
         * 
         * 2)   omega'[t] = (-2 * v[t] * omega[t] - g Sin[theta]) / r
         * 
         * 3)   r'[t] = v[t]
         * 
         * 4)   theta'[t] = omega[t]
         * 
         *      ________z_________
         *    |*                 |*\
         *    |                  |  \
         *    |                  |   \
         *    | l = L - r - z    |    \ r
         *    |                  |theta\
         *    |                  |______\
         *    |                          \
         *    O M                         o m
         * 
         * Need to find r and theta.
         * Solve 1 to find v[t]
         * Solve 2 to find omega[t]
         * With these two values it is simple to solve equations 3 and 4
         * 
         * 4th order runge-kutta method:
         * y2 = y1 + (1 / 6) * (k1 + 2*k2 + 2*k3 + k4) * h
         * k1 = f(x, y1)
         * k2 = f(x + h/2, y1 + (k1 * h) / 2)
         * k3 = f(x + h/2, y1 + (k2 * h) / 2)
         * k4 = f(x + h, y1 + k3 * h)
         * 
         */

        public double SolveEquation1(double h, double r, double omega, double M, double m, double theta, double y1)
        {
            //h is the step-size
            //y1 is the result from the previous step
            double y2; // final result
            k1 = (r * omega * omega - (M / m) * g + g * Math.Cos(theta)) / (M / m + 1);
            return y2 = y1 + k1 * h;
        }

        public double SolveEquation2(double h, double v, double omega, double M, double m, double r, double theta, double y1)
        {
            double y2;
            k1 = (-2 * v * omega - g * Math.Sin(theta)) / r;
            k2 = (-2 * v * (omega + (k1 * h) / 2) - g * Math.Sin(theta)) / r;
            k3 = (-2 * v * (omega + (k2 * h) / 2) - g * Math.Sin(theta)) / r;
            k4 = (-2 * v * (omega + (k3 * h)) - g * Math.Sin(theta)) / r;
            return y2 = y1 + (k1 + 2 * k2 + 2 * k3 + k4) * (h / 6);
        }

        public double SolveEquation3(double h, double v, double y1)
        {
            double y2;
            k1 = v;
            return y2 = y1 + k1 * h;
        }

        public double SolveEquation4(double h, double omega, double y1)
        {
            double y2;
            k1 = omega;
            return y2 = y1 + k1 * h;
        }
        #endregion

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            button1.IsEnabled = true;
            button2.IsEnabled = false;
        }
    }
}
