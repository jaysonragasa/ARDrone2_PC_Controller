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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AttitudeIndicator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void slider_roll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            update(slider_pitch.Value, e.NewValue);
        }

        private void slider_pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.WriteLine(e.NewValue);
            update(e.NewValue, slider_roll.Value);
        }

        void update(double pitch, double roll)
        {
            double _pitch = pitch;
            _pitch = 4 * _pitch;

            double newX = 0.0d;
            double newY = 0.0d;

            newX = _pitch * (Math.Cos((-roll + 90) * (Math.PI / 180)));
            newY = _pitch * (Math.Sin((-roll + 90) * (Math.PI / 180)));

            Pitch.X = newX;
            Pitch.Y = newY;

            Debug.WriteLine(newX + ", " + newY);

            //Pitch2.X = newX;
            //Pitch2.Y = newY;

            Roll.Angle = -roll;
        }
    }
}
