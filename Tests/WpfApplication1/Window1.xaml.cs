using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        double pitch_to_pix(double pitch)
        {
            return (pitch / 35.0 * img.Height / 2);
        }

        private void slider_roll_ValueChanged1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void slider_roll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            roll.Angle = e.NewValue;
        }

        private void slider_pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double _pitch = e.NewValue;
            //_pitch = 4 * _pitch;

            double px = (0 * Math.Cos(roll.Angle)) - (_pitch * Math.Sin(roll.Angle));
            double py = (_pitch * Math.Cos(roll.Angle)) + (0 * Math.Sin(roll.Angle));

            double newX = 0.0d;
            double newY = 0.0d;

            newX = _pitch + (Math.Cos(roll.Angle * (Math.PI / 180)));
            newY = _pitch + (Math.Sin(roll.Angle * (Math.PI / 180)));

            Debug.WriteLine("X: " + newX + ", Y: " + newY);

            pitch.X = newX;
            pitch.Y = newY;
        }
    }
}
