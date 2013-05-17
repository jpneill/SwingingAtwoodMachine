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
        int x, y;
        Color blue = Color.FromRgb(0, 0, 255);
        Color black = Color.FromRgb(0, 0, 0);

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(36);
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            PaintCanvas.Children.Remove(ellipse);

            if (--loopCounter == 0)
                timer.Stop();

            ellipse = CreateEllipse(20, 20, blue);
            PaintCanvas.Children.Add(ellipse);
            x++;
            y++;

            Canvas.SetLeft(ellipse, x - 1);
            Canvas.SetTop(ellipse, y - 1);
        }

        //create an ellipse
        public Ellipse CreateEllipse(int height, int width, Color colour)
        {
            SolidColorBrush fillBrush = new SolidColorBrush(colour);

            return new Ellipse()
            {
                Height = height,
                Width = width,
                Fill = fillBrush
            };
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            loopCounter = 50;
            x = y = 20;
            timer.Start();
        }
    }
}
