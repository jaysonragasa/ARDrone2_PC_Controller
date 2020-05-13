using Microsoft.Practices.ServiceLocation;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WpfApplication1.ViewModel;

namespace WpfApplication1
{
    public partial class HudWindow : Window
    {
        DispatcherTimer tmr;

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public HudWindow()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {

            }
            else
            {
                tmr = new DispatcherTimer();
                tmr.Tick += tmr_Tick;
                tmr.Interval = TimeSpan.FromMilliseconds(20);
                tmr.Start();

                TextCompositionManager.AddTextInputHandler(this, new TextCompositionEventHandler(Main.KeyBoardKeyPressed));

                Sensitivity_DirectionSlider.Value = Properties.Settings.Default.DefaultLRFBSpeed;
                Sensitivity_YawSlider.Value = Properties.Settings.Default.DefaultYawSpeed;
                Sensitivity_GazSlider.Value = Properties.Settings.Default.DefaultGazSpeed;
            }
        }

        void tmr_Tick(object sender, System.EventArgs e)
        {
            this.attitudeControl.SetAttitudeIndicatorParameters(this.Main.Details.Pitch, this.Main.Details.Roll);
            this.headingControl.SetHeadingIndicatorParameters((int)this.Main.Details.Yaw);
            this.altimeterControl.SetAlimeterParameters((int)this.Main.Details.Altitude);
        }

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);

            this.attitudeControl.Show();
            this.attitudeControl.SetAttitudeIndicatorParameters(2, 2);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Main.Cleanup();
            Properties.Settings.Default.Save();

            base.OnClosing(e);
        }

        private void Sensitivity_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = (Slider)sender;

            float v = (float)Math.Round(s.Value, 2);

            if (sender == Sensitivity_DirectionSlider)
            {
                Properties.Settings.Default.DefaultLRFBSpeed = v;
                txtSenDVal.Text = v.ToString();
            }
            else if (sender == Sensitivity_YawSlider)
            {
                Properties.Settings.Default.DefaultYawSpeed = v;
                txtSenYawVal.Text = v.ToString();
            }
            else if (sender == Sensitivity_GazSlider)
            {
                Properties.Settings.Default.DefaultGazSpeed = v;
                txtSenGazVal.Text = v.ToString();
            }

            Properties.Settings.Default.Save();
        }
    }

    /// <summary>
    /// Interaction logic for HudWindow.xaml
    /// </summary>
    public partial class HudWindow2 : Window //, INotifyPropertyChanged
    {
        //bool ConnectedMessageSent = false;

        //DispatcherTimer tmrStatusUpdate;
        //DispatcherTimer tmrVideoUpdate;

        //private DroneClient _droneClient;
        //private NavigationData _navigationData;
        //private NavigationPacket _navigationPacket;

        //private Autopilot _autopilot;

        //// video stuff
        //private VideoPacketDecoderWorker _videoPacketDecoderWorker;
        //private VideoFrame _frame;
        //private PacketRecorder _packetRecorderWorker;
        //private uint _frameNumber;
        //private Bitmap _frameBitmap;

        //// hud
        //private InstrumentsManager instrumentsManager;
        ////private HudInterface hudInterface;

        //// input method
        //private ARDrone.Input.InputManager inputManager;
        //KeyboardHook keyboard;

        //private HudStatus _HudStatus = new HudStatus();
        //public HudStatus HudStatus
        //{
        //    get { return _HudStatus; }
        //    set
        //    {
        //        if (_HudStatus != value)
        //        {
        //            _HudStatus = value;
        //            //RaisePropertyChanged("HudStatus");
        //        }
        //    }
        //}

        //// Wifi
        //WlanClient wlan;
        //Wlan.WlanAssociationAttributes drone_network;

        //public HudWindow2()
        //{
        //    InitializeComponent();
        //}

        //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        //{
        //    base.OnClosing(e);


        //    return;

        //    _droneClient.Stop();
        //    _droneClient.Dispose();
        //    _videoPacketDecoderWorker.Dispose();
        //    _autopilot.Stop();
        //}

        //protected override void OnInitialized(EventArgs e)
        //{
        //    base.OnInitialized(e);

        //    return;

        //    tmrStatusUpdate = new DispatcherTimer();
        //    tmrStatusUpdate.Interval = TimeSpan.FromMilliseconds(100);
        //    tmrStatusUpdate.IsEnabled = true;
        //    tmrStatusUpdate.Tick += tmrStatusUpdate_Tick;

        //    tmrVideoUpdate = new DispatcherTimer();
        //    tmrVideoUpdate.Interval = TimeSpan.FromMilliseconds(20);
        //    tmrVideoUpdate.IsEnabled = true;
        //    tmrVideoUpdate.Tick += tmrVideoUpdate_Tick;

        //    _videoPacketDecoderWorker = new VideoPacketDecoderWorker(AR.Drone.Video.PixelFormat.BGR24, true, OnVideoPacketDecoded);
        //    _videoPacketDecoderWorker.Start();

        //    _droneClient = new DroneClient("192.168.1.1");
        //    _droneClient.NavigationPacketAcquired += OnNavigationPacketAcquired;
        //    _droneClient.VideoPacketAcquired += OnVideoPacketAcquired;
        //    _droneClient.NavigationDataAcquired += data => _navigationData = data;
        //    //_droneClient.Start();

        //    wlan = new WlanClient();
        //    //foreach (WlanClient.WlanInterface wlanIface in wlan.Interfaces)
        //    {
        //        drone_network = wlan.Interfaces[0].CurrentConnection.wlanAssociationAttributes;
        //        Wlan.Dot11Ssid _ssid = drone_network.dot11Ssid;

        //        string SSID = GetStringForSSID(_ssid);
        //        _HudStatus.DroneWiFiName = SSID;
        //    }

        //    this.DataContext = this.HudStatus;

        //    //this.AttitudeIndicator.Clip = new EllipseGeometry()
        //    //{
        //    //    Center = new System.Windows.Point(this.AttitudeIndicator.ActualWidth / 2, this.AttitudeIndicator.ActualHeight / 2),
        //    //    RadiusX = 200, RadiusY = 200
        //    //};

        //    InitializeControlEvents();
        //    InitializeAutopilot();
        //    //InitializeInputMethod();
        //    //keyboard = new KeyboardHook();
        //    //keyboard.KeyBoardKeyPressed += keyboard_KeyBoardKeyPressed;

        //    TextCompositionManager.AddTextInputHandler(this, new TextCompositionEventHandler(KeyBoardKeyPressed));
        //}

        //void InitializeControlEvents()
        //{
        //    btnConnect.Click += (a, b) =>
        //    {
        //        Debug("Connecting ...");

        //        _droneClient.Start();
        //        tmrVideoUpdate.Start();
        //        tmrStatusUpdate.Start();
        //    };
        //    btnDisconnect.Click += (a, b) =>
        //    {
        //        _droneClient.Stop();
        //        tmrStatusUpdate.Stop();
        //        tmrVideoUpdate.Start();

        //        ConnectedMessageSent = false;

        //        Debug("Disconnected");
        //    };
        //    btnFlatTrim.Click += (a, b) =>
        //    {
        //        _droneClient.FlatTrim();
        //        Debug("Sent FlatTrim command");
        //    };
        //    btnChnageCamera.Click += (a, b) =>
        //    {
                
        //    };

        //    // engine
        //    btnEngineStart.Click += (a, b) =>
        //    {
        //        DroneControl(btnEngineStart);
        //    };
        //    btnEngineEmergency.Click += (a, b) =>
        //    {
        //        DroneControl(btnEngineEmergency);
        //    };
        //    btnEngineEmergencyReset.Click += (a, b) =>
        //    {
        //        _droneClient.ResetEmergency();
        //    };
        //    btnEngineLand.Click += (a, b) =>
        //    {
        //        DroneControl(btnEngineLand);
        //    };

        //    // navigation
        //    btnLeft.Click += (a, b) =>
        //    {
        //        DroneControl(btnLeft);
        //    };
        //    btnRight.Click += (a, b) =>
        //    {
        //        DroneControl(btnRight);
        //    };
        //    btnForward.Click += (a, b) =>
        //    {
        //        DroneControl(btnForward);
        //    };
        //    btnBackward.Click += (a, b) =>
        //    {
        //        DroneControl(btnBackward);
        //    };
        //    btnYawLeft.Click += (a, b) =>
        //    {
        //        DroneControl(btnYawLeft);
        //    };
        //    btnYawRight.Click += (a, b) =>
        //    {
        //        DroneControl(btnYawRight);
        //    };
        //    btnUp.Click += (a, b) =>
        //    {
        //        DroneControl(btnUp);
        //    };
        //    btnDown.Click += (a, b) =>
        //    {
        //        DroneControl(btnDown);
        //    };
        //    btnHover.Click += (a, b) =>
        //    {
        //        DroneControl(btnHover);
        //    };
        //}

        //void InitializeInputMethod()
        //{
        //    inputManager = new ARDrone.Input.InputManager(Utility.GetWindowHandle(this));
        //    inputManager.SwitchInputMode(ARDrone.Input.InputManager.InputMode.ControlInput);

        //    inputManager.NewInputState += inputManager_NewInputState;
        //    inputManager.NewInputDevice += inputManager_NewInputDevice;
        //    inputManager.InputDeviceLost += inputManager_InputDeviceLost;

        //    inputManager.SetFlags(false, false, false, false);
        //}

        //// Make sure '_autopilot' variable is initialized with an object
        //private void InitializeAutopilot()
        //{
        //    if (_autopilot != null) return;

        //    _autopilot = new Autopilot(_droneClient);
        //    _autopilot.OnOutOfObjectives += Autopilot_OnOutOfObjectives;
        //}

        //// Event that occurs when no objectives are waiting in the autopilot queue
        //private void Autopilot_OnOutOfObjectives()
        //{
        //    _autopilot.Active = false;
        //}

        //void KeyBoardKeyPressed(object sender, TextCompositionEventArgs e)
        //{
        //    string chr = e.Text;

        //    switch (chr)
        //    {
        //        case "a": // left
        //            DroneControl(btnLeft);
        //            break;
        //        case "d": // right
        //            DroneControl(btnRight);
        //            break;
        //        case "w": // forward
        //            DroneControl(btnForward);
        //            break;
        //        case "s": // backward
        //            DroneControl(btnBackward);
        //            break;
        //        case "j": // yaw left
        //            DroneControl(btnYawLeft);
        //            break;
        //        case "l": // yaw right
        //            DroneControl(btnYawRight);
        //            break;
        //        case "i": // gaz up
        //            DroneControl(btnUp);
        //            break;
        //        case "k": // gaz down
        //            DroneControl(btnDown);
        //            break;
        //        case "x": // hover
        //            DroneControl(btnHover);
        //            break;
        //        case "v": // start engine
        //            DroneControl(btnEngineStart);
        //            break;
        //        case "b": // land
        //            DroneControl(btnEngineLand);
        //            break;
        //        case "n": // emegency landing
        //            DroneControl(btnEngineEmergency);
        //            break;
        //        case "f": // flat trim
        //            _droneClient.FlatTrim();
        //            break;

        //    };

        //    Debug(chr);
        //}

        //#region // inputManager events
        //void inputManager_InputDeviceLost(object sender, ARDrone.Input.Utils.InputDeviceLostEventArgs e)
        //{
            
        //}

        //void inputManager_NewInputDevice(object sender, ARDrone.Input.Utils.NewInputDeviceEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("new input: " + e.DeviceName);
        //}

        //void inputManager_NewInputState(object sender, ARDrone.Input.Utils.NewInputStateEventArgs e)
        //{
            
        //}
        //#endregion

        //#region // timers event
        //void tmrStatusUpdate_Tick(object sender, EventArgs e)
        //{
        //    if (_droneClient.IsConnected)
        //    {
        //        if (ConnectedMessageSent == false)
        //        {
        //            Debug("Connected");
        //            ConnectedMessageSent = true;
        //        }

        //    }

        //    // wifi signal strength
        //    if (wlan != null)
        //    {
        //        double signal_strength = 0.0d;

        //        try
        //        {
        //            drone_network = wlan.Interfaces[0].CurrentConnection.wlanAssociationAttributes;
        //            _HudStatus.WiFiSignalStrength = drone_network.wlanSignalQuality;

        //            signal_strength = drone_network.wlanSignalQuality;
        //        }
        //        catch { }

        //        if (_HudStatus.WiFiSignalStrength < 25)
        //        {
        //            ShowAlertStatus("WARNING! WiFi signal is below 25%. Navigate back now!");
        //        }
        //        else
        //        {
        //            ShowAlertStatus(string.Empty, false);
        //        }
        //    }

        //    if (_navigationData != null)
        //    {
        //        // battery
        //        this.HudStatus.BatteryLevel = (int)this._navigationData.Battery.Percentage;
        //        if (this._navigationData.Battery.Percentage < 25)
        //        {
        //            ShowAlertStatus("WARNING! Battery is below 25%");
        //        }
        //        else
        //        {
        //            ShowAlertStatus(string.Empty, false);
        //        }

        //        this.attitudeControl.SetAttitudeIndicatorParameters(_navigationData.Pitch, -_navigationData.Roll);

        //        if (_navigationData.Yaw > 0)
        //        {
        //            this.headingControl.SetHeadingIndicatorParameters(Convert.ToInt32(_navigationData.Yaw));

        //            rotate_y.Angle = -_navigationData.Yaw;
        //        }
        //        else
        //        {
        //            this.headingControl.SetHeadingIndicatorParameters(360 + Convert.ToInt32(_navigationData.Yaw));

        //            rotate_y.Angle = -(360 + _navigationData.Yaw);
        //        }

        //        rotate_x.Angle = -_navigationData.Pitch;
        //        rotate_z.Angle = _navigationData.Roll;

        //        UpdateAttitudeIndicator((int)_navigationData.Pitch, (int)_navigationData.Roll);

        //        this.altimeterControl.SetAlimeterParameters((int)_navigationData.Altitude);

        //        Console.WriteLine("Pitch: {0}, Roll: {1}, Yaw: {2}", _navigationData.Pitch, _navigationData.Roll, _navigationData.Yaw);
        //    }
        //}

        //void tmrVideoUpdate_Tick(object sender, EventArgs e)
        //{
        //    if (_frame == null || _frameNumber == _frame.Number)
        //        return;
        //    _frameNumber = _frame.Number;

        //    if (_frameBitmap == null)
        //        _frameBitmap = VideoHelper.CreateBitmap(ref _frame);
        //    else
        //        VideoHelper.UpdateBitmap(ref _frameBitmap, ref _frame);

        //    //pbVideo.Image = _frameBitmap;
        //    imgVideo.Source = VideoHelper.ConvertToBitmapImage(ref _frameBitmap);
        //}
        //#endregion

        //#region // droneclient events
        //private void OnNavigationPacketAcquired(NavigationPacket packet)
        //{
        //    if (_packetRecorderWorker != null && _packetRecorderWorker.IsAlive)
        //        _packetRecorderWorker.EnqueuePacket(packet);

        //    _navigationPacket = packet;
        //}

        //private void OnVideoPacketDecoded(VideoFrame frame)
        //{
        //    _frame = frame;
        //}

        //void OnVideoPacketAcquired(VideoPacket packet)
        //{
        //    if (_packetRecorderWorker != null && _packetRecorderWorker.IsAlive)
        //        _packetRecorderWorker.EnqueuePacket(packet);
        //    if (_videoPacketDecoderWorker.IsAlive)
        //        _videoPacketDecoderWorker.EnqueuePacket(packet);
        //}
        //#endregion

        //private void slider_roll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    UpdateAttitudeIndicator((int)slider_pitch.Value, (int)e.NewValue);
        //}

        //private void slider_pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    UpdateAttitudeIndicator((int)e.NewValue, (int)slider_roll.Value);
        //}

        //#region methods
        //void BindAutoPilot()
        //{
        //    _autopilot.BindToClient();
        //    _autopilot.Start();
        //}

        //void UnbindAutoPilot()
        //{
        //    _autopilot.Stop();
        //    _autopilot.UnbindFromClient();
        //}

        //void DroneControl(object sender)
        //{
        //    int expiration = Properties.Settings.Default.AutoPilotExpiration;
        //    bool useQ = Properties.Settings.Default.UseAutoPilot;

        //    _autopilot.Active = useQ;

        //    float _lrfb = Properties.Settings.Default.DefaultLRFBSpeed;
        //    float _yaw = Properties.Settings.Default.DefaultYawSpeed;
        //    float _gaz = Properties.Settings.Default.DefaultGazSpeed;

        //    #region // engine
        //    if (sender == btnEngineStart)
        //    {
        //        _droneClient.FlatTrim();
        //        _autopilot.ClearObjectives();

        //        _droneClient.Takeoff();
        //    };
        //    if (sender == btnEngineEmergency)
        //    {
        //        _autopilot.ClearObjectives();
        //        _droneClient.Emergency();
        //    };
        //    if (sender == btnEngineEmergencyReset)
        //    {
        //        _droneClient.ResetEmergency();
        //    };
        //    if (sender == btnEngineLand)
        //    {
        //        _autopilot.ClearObjectives();
        //        _droneClient.Land();
        //    };
        //    #endregion

        //    _autopilot.Active = true;

        //    // navigation
        //    #region left right up down and hover
        //    //if (sender == btnLeft || sender == btnRight)
        //    //{
        //    //    if (useQ)
        //    //    {
        //    //        _autopilot.EnqueueObjective(Objective.Create(expiration, new SetRoll(
        //    //            (sender == btnLeft ? -_lrfb : _lrfb)
        //    //        )));
        //    //    }
        //    //    else
        //    //    {
        //    //        _droneClient.Progress(FlightMode.Progressive,
        //    //            roll: (sender == btnLeft ? -_lrfb : _lrfb)
        //    //        );
        //    //    }
        //    //}

        //    if (sender == btnLeft)
        //    {
        //        if (useQ)
        //        {
        //            _autopilot.EnqueueObjective(Objective.Create(expiration, new SetRoll(-_lrfb)));
        //        }
        //        else
        //        {
        //            _droneClient.Progress(FlightMode.Progressive, roll: -_lrfb);
        //        }
        //    };
        //    if (sender == btnRight)
        //    {
        //        if (useQ)
        //        {
        //            _autopilot.EnqueueObjective(Objective.Create(expiration, new SetRoll(_lrfb)));
        //        }
        //        else
        //        {
        //            _droneClient.Progress(FlightMode.Progressive, roll: _lrfb);
        //        }
        //    };
        //    if (sender == btnForward)
        //    {
        //        if (useQ)
        //        {
        //            _autopilot.EnqueueObjective(Objective.Create(expiration, new SetPitch(-_lrfb)));
        //        }
        //        else
        //        {
        //            _droneClient.Progress(FlightMode.Progressive, pitch: -_lrfb);
        //        }
        //    };
        //    if (sender == btnBackward)
        //    {
        //        if (useQ)
        //        {
        //            _autopilot.EnqueueObjective(Objective.Create(expiration, new SetPitch(_lrfb)));
        //        }
        //        else
        //        {
        //            _droneClient.Progress(FlightMode.Progressive, pitch: _lrfb);
        //        }
        //    };

        //    if (sender == btnHover)
        //    {
        //        if (useQ)
        //        {
        //            _autopilot.EnqueueObjective(
        //            Objective.Create(expiration,
        //                new VelocityX(0.0f),
        //                new VelocityY(0.0f),
        //                new Altitude(1.0f)
        //            ));
        //        }
        //        else
        //        {
        //            _droneClient.Hover();
        //        }
        //    }
        //    #endregion

        //    #region yaw left and right
        //    if (sender == btnYawLeft)
        //    {
        //        if (useQ)
        //        {
        //            _autopilot.EnqueueObjective(Objective.Create(expiration, new SetYaw(-_yaw)));
        //        }
        //        else
        //        {
        //            _droneClient.Progress(FlightMode.Progressive, yaw: -_yaw);
        //        }
        //    };
        //    if (sender == btnYawRight)
        //    {
        //        if (useQ)
        //        {
        //            _autopilot.EnqueueObjective(Objective.Create(expiration, new SetYaw(_yaw)));
        //        }
        //        else
        //        {
        //            _droneClient.Progress(FlightMode.Progressive, yaw: _yaw);
        //        }
        //    };
        //    #endregion

        //    #region gaz up and down
        //    if (sender == btnUp)
        //    {
        //        if (useQ)
        //        {
        //            _autopilot.EnqueueObjective(Objective.Create(expiration, new SetGaz(_gaz)));
        //        }
        //        else
        //        {
        //            _droneClient.Progress(FlightMode.Progressive, gaz: _gaz);
        //        }
        //        //_droneClient.Hover();
        //    };
        //    if (sender == btnDown)
        //    {
        //        if (useQ)
        //        {
        //            _autopilot.EnqueueObjective(Objective.Create(expiration, new SetGaz(-_gaz)));
        //        }
        //        else
        //        {
        //            _droneClient.Progress(FlightMode.Progressive, gaz: -_gaz);
        //        }
        //    };
        //    #endregion
        //}

        //string GetStringForSSID(Wlan.Dot11Ssid ssid)
        //{
        //    return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        //}

        //void UpdateAttitudeIndicator(int pitch, int roll)
        //{
        //    int _roll = roll;
        //    double _pitch = pitch;
        //    _pitch = 4 * _pitch;

        //    double newX = 0.0d;
        //    double newY = 0.0d;

        //    //Debug("pitch: " + pitch + ", roll: " + _roll);

        //    newX = _pitch * (Math.Cos((-_roll + 90) * (Math.PI / 180)));
        //    newY = _pitch * (Math.Sin((-_roll + 90) * (Math.PI / 180)));

        //    Pitch.X = newX;
        //    Pitch.Y = newY;
        //    //Pitch2.X = newX;
        //    //Pitch2.Y = newY;

        //    Roll.Angle = -roll;
        //}

        //string lastTxt = string.Empty;
        //void Debug(string txt)
        //{
        //    string ntxt = DateTime.Now.ToString("MM/dd/yyyy mm:hh:ss ") + txt;

        //    if (ntxt != lastTxt)
        //    {
        //        lastTxt = ntxt;
        //        lstDebug.Items.Insert(0, ntxt);
        //    }
        //}

        //void ShowAlertStatus(string text, bool show = true)
        //{
        //    if (show)
        //    {
        //        GridAlertStatus.Visibility = System.Windows.Visibility.Visible;
        //        lblAlert.Text = text;
        //    }
        //    else
        //    {
        //        GridAlertStatus.Visibility = System.Windows.Visibility.Collapsed;
        //    }

        //}
        //#endregion
    }

    public class HudStatus : INotifyPropertyChanged
    {
        private int _BatteryLevel = 0;
        public int BatteryLevel
        {
            get { return _BatteryLevel; }
            set
            {
                if (_BatteryLevel != value)
                {
                    _BatteryLevel = value;
                    RaisePropertyChanged("BatteryLevel");
                }
            }
        }

        private string _DroneWiFiName = string.Empty;
        public string DroneWiFiName
        {
            get { return _DroneWiFiName; }
            set
            {
                if (_DroneWiFiName != value)
                {
                    _DroneWiFiName = value;
                    RaisePropertyChanged("DroneWiFiName");
                }
            }
        }

        private double _WiFiSignalStrength = 0.0d;
        public double WiFiSignalStrength
        {
            get { return _WiFiSignalStrength; }
            set
            {
                if (_WiFiSignalStrength != value)
                {
                    _WiFiSignalStrength = value;
                    RaisePropertyChanged("WiFiSignalStrength");
                }
            }
        }

        #region // INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
