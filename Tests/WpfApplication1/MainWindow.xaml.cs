using AR.Drone.Client;
using AR.Drone.Data;
using AR.Drone.Data.Navigation;
using AR.Drone.Media;
using AR.Drone.Video;
using AR.Drone.WinApp;
using AviationInstruments;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer tmrStatusUpdate;
        DispatcherTimer tmrVideoUpdate;

        private DroneClient _droneClient;
        private NavigationData _navigationData;
        private NavigationPacket _navigationPacket;

        // video stuff
        private VideoPacketDecoderWorker _videoPacketDecoderWorker;
        private VideoFrame _frame;
        private PacketRecorder _packetRecorderWorker;
        private uint _frameNumber;
        private Bitmap _frameBitmap;

        // hud
        private InstrumentsManager instrumentsManager;
        //private HudInterface hudInterface;

        // attitude params
        double PitchAngle = 0; // Phi
        double RollAngle = 0; // Theta

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.btnActivate.Click += btnActivate_Click;
            this.btnDeActivate.Click += btnDeActivate_Click;
        }

        void btnDeActivate_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.Stop();
        }

        void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            _droneClient.Start();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _droneClient.Dispose();
            _videoPacketDecoderWorker.Dispose();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tmrStatusUpdate = new DispatcherTimer();
            tmrStatusUpdate.Interval = TimeSpan.FromMilliseconds(100);
            tmrStatusUpdate.IsEnabled = true;
            tmrStatusUpdate.Tick += tmrStatusUpdate_Tick;

            tmrVideoUpdate = new DispatcherTimer();
            tmrVideoUpdate.Interval = TimeSpan.FromMilliseconds(20);
            tmrVideoUpdate.IsEnabled = true;
            tmrVideoUpdate.Tick += tmrVideoUpdate_Tick;

            _videoPacketDecoderWorker = new VideoPacketDecoderWorker(AR.Drone.Video.PixelFormat.BGR24, true, OnVideoPacketDecoded);
            //_videoPacketDecoderWorker.Start();

            _droneClient = new DroneClient("192.168.1.1");
            _droneClient.NavigationPacketAcquired += OnNavigationPacketAcquired;
            _droneClient.VideoPacketAcquired += _droneClient_VideoPacketAcquired;
            _droneClient.NavigationDataAcquired += data => _navigationData = data;

            //InitializeAviationControls();
            w = AttitudeMeter.ActualWidth;
            h = AttitudeMeter.ActualHeight;

            SetAttitudeIndicatorParameters(pitch.Value, roll.Value);
        }

        private void InitializeAviationControls()
        {
            instrumentsManager = new InstrumentsManager(ref _navigationData);
            instrumentsManager.addInstrument(this.attitudeControl);
            instrumentsManager.addInstrument(this.altimeterControl);
            instrumentsManager.addInstrument(this.headingControl);
            instrumentsManager.startManage();
        }

        private void OnNavigationPacketAcquired(NavigationPacket packet)
        {
            if (_packetRecorderWorker != null && _packetRecorderWorker.IsAlive)
                _packetRecorderWorker.EnqueuePacket(packet);

            _navigationPacket = packet;
        }

        void tmrVideoUpdate_Tick(object sender, EventArgs e)
        {
            if (_frame == null || _frameNumber == _frame.Number)
                return;
            _frameNumber = _frame.Number;

            if (_frameBitmap == null)
                _frameBitmap = VideoHelper.CreateBitmap(ref _frame);
            else
                VideoHelper.UpdateBitmap(ref _frameBitmap, ref _frame);

            //pbVideo.Image = _frameBitmap;
            imgVideo.Source = VideoHelper.ConvertToBitmapImage(ref _frameBitmap);
        }

        private void OnVideoPacketDecoded(VideoFrame frame)
        {
            _frame = frame;
        }

        void _droneClient_VideoPacketAcquired(VideoPacket packet)
        {
            if (_packetRecorderWorker != null && _packetRecorderWorker.IsAlive)
                _packetRecorderWorker.EnqueuePacket(packet);
            if (_videoPacketDecoderWorker.IsAlive)
                _videoPacketDecoderWorker.EnqueuePacket(packet);
        }

        void tmrStatusUpdate_Tick(object sender, EventArgs e)
        {
            lblIsConnected.Text = "Connected: " + _droneClient.IsActive.ToString();

            if (_navigationData != null)
            {
                txBattery.Text = "Battery: " + this._navigationData.Battery.Percentage;

                //Debug.WriteLine("Pitch: {0}, Roll: {1}", _navigationData.Pitch, _navigationData.Roll);

                this.attitudeControl.SetAttitudeIndicatorParameters(_navigationData.Pitch, -_navigationData.Roll);
                
                SetAttitudeIndicatorParameters(_navigationData.Pitch, -_navigationData.Roll);

                if (_navigationData.Yaw > 0)
                {
                    this.headingControl.SetHeadingIndicatorParameters(Convert.ToInt32(_navigationData.Yaw));
                }
                else
                {
                    this.headingControl.SetHeadingIndicatorParameters(360 + Convert.ToInt32(_navigationData.Yaw));
                }
            }
        }

        double w = 0;
        double h = 0;
        public void SetAttitudeIndicatorParameters(double aircraftPitchAngle, double aircraftRollAngle)
        {
            System.Drawing.Point ptImg = new System.Drawing.Point(25, -210);
            System.Drawing.Point ptRot = new System.Drawing.Point((int)AttitudeMeter.ActualWidth / 2, (int)AttitudeMeter.ActualHeight / 2);

            PitchAngle = aircraftPitchAngle;
            RollAngle = aircraftRollAngle;

            float scale = (float)AttitudeMeter.ActualWidth / (float)frame.ActualWidth;

            double beta = 0;
            double d = 0;
            float deltaXRot = 0;
            float deltaYRot = 0;
            float deltaXTrs = 0;
            float deltaYTrs = 0;
            double alphaRot = aircraftRollAngle * Math.PI / 180;
            double alphaTrs = 0;
            double deltaPx = (int)(4 * PitchAngle);

            // Rotation

            if (ptImg != ptRot)
            {
                // Internals coeffs
                if (ptRot.X != 0)
                {
                    beta = Math.Atan((double)ptRot.Y / (double)ptRot.X);
                }

                d = Math.Sqrt((ptRot.X * ptRot.X) + (ptRot.Y * ptRot.Y));

                // Computed offset
                deltaXRot = (float)(d * (Math.Cos(alphaRot - beta) - Math.Cos(alphaRot) * Math.Cos(alphaRot + beta) - Math.Sin(alphaRot) * Math.Sin(alphaRot + beta)));
                deltaYRot = (float)(d * (Math.Sin(beta - alphaRot) + Math.Sin(alphaRot) * Math.Cos(alphaRot + beta) - Math.Cos(alphaRot) * Math.Sin(alphaRot + beta)));
            }

            // Translation

            // Computed offset
            deltaXTrs = (float)(deltaPx * (Math.Sin(alphaTrs)));
            deltaYTrs = (float)(-deltaPx * (-Math.Cos(alphaTrs)));

            float floatX = (ptImg.X + deltaXRot + deltaXTrs) * scale;
            float floatY = (ptImg.Y + deltaYRot + deltaYTrs) * scale;

            TransformGroup tg = new TransformGroup() { };
            tg.Children.Add(new RotateTransform()
            {
                CenterX = 0.5,
                CenterY = 0.5,
                Angle = aircraftRollAngle
            });
            //tg.Children.Add(new ScaleTransform()
            //{
            //    CenterX = 0.5,
            //    CenterY = 0.5,
            //    ScaleX = 2,
            //    ScaleY = 2
            //});
            tg.Children.Add(new TranslateTransform()
            {
                X = 0,
                Y = PitchAngle
            });
            
            AttitudeMeter.RenderTransform = tg;
            //Canvas.SetLeft(AttitudeMeter, floatX);
            //Canvas.SetTop(AttitudeMeter, floatY);
            //AttitudeMeter.Width = (double)AttitudeMeter.Width * scale;
            //AttitudeMeter.Height = (double)AttitudeMeter.Height * scale;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = (Slider)sender;
            if (s != null)
            {
                SetAttitudeIndicatorParameters(pitch.Value, -roll.Value);
                this.attitudeControl.SetAttitudeIndicatorParameters(pitch.Value, -roll.Value);
            }
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = (Slider)sender;
            if (s != null)
            {
                SetAttitudeIndicatorParameters(pitch.Value, -roll.Value);
                this.attitudeControl.SetAttitudeIndicatorParameters(pitch.Value, -roll.Value);
            }
        }
    }
}
