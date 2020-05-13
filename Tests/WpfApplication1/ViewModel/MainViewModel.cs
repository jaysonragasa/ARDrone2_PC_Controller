using AR.Drone.Avionics;
using AR.Drone.Avionics.Objectives;
using AR.Drone.Avionics.Objectives.IntentObtainers;
using AR.Drone.Client;
using AR.Drone.Client.Command;
using AR.Drone.Client.Configuration;
using AR.Drone.Data;
using AR.Drone.Data.Navigation;
using AR.Drone.Media;
using AR.Drone.Video;
using AR.Drone.WinApp;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NativeWifi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WpfApplication1.Model;

namespace WpfApplication1.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        bool ConnectedMessageSent = false;

        DispatcherTimer tmrStatusUpdate;
        DispatcherTimer tmrVideoUpdate;

        private DroneClient _droneClient;
        private NavigationData _navigationData;
        private NavigationPacket _navigationPacket;

        private Settings _configuration;

        private Autopilot _autopilot;

        // video stuff
        private VideoPacketDecoderWorker _videoPacketDecoderWorker;
        private VideoFrame _frame;
        private PacketRecorder _packetRecorderWorker;
        private uint _frameNumber;
        private Bitmap _frameBitmap;

        // Wifi
        WlanClient wlan;
        Wlan.WlanAssociationAttributes drone_network;

        private HudDetails _Details = new HudDetails();
        public HudDetails Details
        {
            get { return _Details; }
            set
            {
                if (_Details != value)
                {
                    _Details = value;
                    base.RaisePropertyChanged("Details");
                }
            }
        }

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                Inititialize();
                InitializeAutopilot();
                InitWiFi();
                //InitConfig();
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();

            _droneClient.Stop();
            _droneClient.Dispose();
            _videoPacketDecoderWorker.Dispose();
            _autopilot.Stop();
        }

        void Inititialize()
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
            _videoPacketDecoderWorker.Start();

            _droneClient = new DroneClient("192.168.1.1");
            _droneClient.NavigationPacketAcquired += OnNavigationPacketAcquired;
            _droneClient.VideoPacketAcquired += OnVideoPacketAcquired;
            _droneClient.NavigationDataAcquired += data => _navigationData = data;

            // commands
            Command_Connect = new RelayCommand(CommandConnect);
            Command_Disconnect = new RelayCommand(CommandDisconnect);
            Command_ShowDrownLayer = new RelayCommand<bool>(CommandShowDrownLayer);
            Command_UpdateDroneSettings = new RelayCommand(CommandUpdateDroneSettings);
        }

        // Make sure '_autopilot' variable is initialized with an object
        private void InitializeAutopilot()
        {
            if (_autopilot != null) return;

            _autopilot = new Autopilot(_droneClient);
            _autopilot.OnOutOfObjectives += Autopilot_OnOutOfObjectives;
        }

        void InitWiFi()
        {
            wlan = new WlanClient();
            //foreach (WlanClient.WlanInterface wlanIface in wlan.Interfaces)
            {
                try
                {
                    drone_network = wlan.Interfaces[0].CurrentConnection.wlanAssociationAttributes;
                    Wlan.Dot11Ssid _ssid = drone_network.dot11Ssid;

                    string SSID = GetStringForSSID(_ssid);
                    this.Details.DroneWiFiName = SSID;
                }
                catch { }
            }
        }

        void InitConfig()
        {
            Task<Settings> configurationTask = _droneClient.GetConfigurationTask();
            configurationTask.ContinueWith(delegate(Task<Settings> task)
            {
                if (task.Exception != null)
                {
                    Trace.TraceWarning("Get configuration task is faulted with exception: {0}", task.Exception.InnerException.Message);
                    return;
                }

                _configuration = task.Result;
                GetConfig();
            });
            configurationTask.Start();
        }

        #region commands
        public ICommand Command_Connect { get; internal set; }
        public ICommand Command_Disconnect { get; internal set; }
        public ICommand Command_ShowDrownLayer { get; internal set; }
        public ICommand Command_UpdateDroneSettings { get; internal set; }
        #endregion

        #region command methods
        void CommandConnect()
        {
            Debug("Connecting ...");

            _droneClient.Start();
            tmrVideoUpdate.Start();
            tmrStatusUpdate.Start();

            InitConfig();
        }

        void CommandDisconnect()
        {
            _droneClient.Stop();
            tmrVideoUpdate.Stop();
            tmrStatusUpdate.Stop();

            ConnectedMessageSent = false;

            Debug("Disconnected");
        }

        void CommandShowDrownLayer(bool value)
        {
            Debug(value.ToString());
        }

        void CommandUpdateDroneSettings()
        {
            SetConfig();
        }
        #endregion

        #region // autopilot events
        // Event that occurs when no objectives are waiting in the autopilot queue
        private void Autopilot_OnOutOfObjectives()
        {
            _autopilot.Active = false;
        }
        #endregion

        #region // timers event
        void tmrStatusUpdate_Tick(object sender, EventArgs e)
        {
            List<string> alert_msgs = new List<string>();

            #region 1
            if (_droneClient.IsConnected)
            {
                if (ConnectedMessageSent == false)
                {
                    Debug("Connected");
                    ConnectedMessageSent = true;
                }
            }
            #endregion

            #region // wifi signal strength
            if (wlan != null)
            {
                double signal_strength = 0.0d;

                try
                {
                    drone_network = wlan.Interfaces[0].CurrentConnection.wlanAssociationAttributes;
                    Wlan.Dot11Ssid _ssid = drone_network.dot11Ssid;
                    string SSID = GetStringForSSID(_ssid);
                    signal_strength = drone_network.wlanSignalQuality;

                    this.Details.DroneWiFiName = SSID;
                    this.Details.WiFiSignalStrength = signal_strength;

                    string alert = "WARNING! WiFi signal is below 25%. Navigate back now!";

                    if (this.Details.WiFiSignalStrength < 25)
                    {
                        if (!alert_msgs.Contains(alert))
                        {
                            alert_msgs.Add(alert);
                        }
                    }
                    else
                    {
                        alert_msgs.Remove(alert);
                    }
                }
                catch { }
            }
            #endregion

            #region 2
            if (_navigationData != null)
            {
                // battery
                this.Details.BatteryLevel = (int)this._navigationData.Battery.Percentage;
                string alert = "WARNING! Battery is below 25%";

                if (this._navigationData.Battery.Percentage < 25)
                {
                    if (!alert_msgs.Contains(alert))
                    {
                        alert_msgs.Add(alert);
                    }
                }
                else
                {
                    alert_msgs.Remove(alert);
                }

                //this.attitudeControl.SetAttitudeIndicatorParameters(_navigationData.Pitch, -_navigationData.Roll);
                this.Details.Pitch = _navigationData.Pitch;
                this.Details.Roll = -_navigationData.Roll;

                if (_navigationData.Yaw > 0)
                {
                    //this.headingControl.SetHeadingIndicatorParameters(Convert.ToInt32(_navigationData.Yaw));

                    //rotate_y.Angle = -_navigationData.Yaw;
                    this.Details.Yaw = _navigationData.Yaw;
                    //this.Details.RotateY = -_navigationData.Yaw;
                }
                else
                {
                    float y = (360 + _navigationData.Yaw);

                    //this.headingControl.SetHeadingIndicatorParameters(y);

                    this.Details.Yaw = y;
                    //this.Details.RotateY = -y;
                }

                this.Details.RotateX = -_navigationData.Pitch;
                this.Details.RotateZ = _navigationData.Roll;

                this.Details.Altitude = _navigationData.Altitude;

                UpdateAttitudeIndicator((int)_navigationData.Pitch, (int)_navigationData.Roll);

                //this.altimeterControl.SetAlimeterParameters((int)_navigationData.Altitude);

                Console.WriteLine("Pitch: {0}, Roll: {1}, Yaw: {2}", this.Details.Pitch, this.Details.Roll, this.Details.Yaw);
            }
            #endregion

            if (alert_msgs.Count > 0)
            {
                string alerts = string.Empty;
                alerts = string.Join("\r\n", alert_msgs.ToArray());
                ShowAlertStatus(alerts, true);
            }
            else
            {
                ShowAlertStatus(string.Empty, false);
            }
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
            this.Details.VideoFrame = VideoHelper.ConvertToBitmapImage(ref _frameBitmap);
        }
        #endregion

        #region keyboard event
        public void KeyBoardKeyPressed(object sender, TextCompositionEventArgs e)
        {
            string chr = e.Text;

            switch (chr)
            {
                case "a": // left
                    DroneControl("a");
                    break;
                case "d": // right
                    DroneControl("d");
                    break;
                case "w": // forward
                    DroneControl("w");
                    break;
                case "s": // backward
                    DroneControl("s");
                    break;
                case "j": // yaw left
                    DroneControl("j");
                    break;
                case "l": // yaw right
                    DroneControl("l");
                    break;
                case "i": // gaz up
                    DroneControl("i");
                    break;
                case "k": // gaz down
                    DroneControl("k");
                    break;
                case "x": // hover
                    DroneControl("x");
                    break;
                case "v": // start engine
                    Debug("Starting engine");
                    DroneControl("v");
                    break;
                case "b": // land
                    Debug("Landing");
                    DroneControl("b");
                    break;
                case "n": // emegency landing
                    Debug("Emergency landing");
                    DroneControl("n");
                    break;
                case "m": // reset
                    Debug("Resetting drone");
                    DroneControl("m");
                    break;
                case "f": // flat trim
                    Debug("Calibrate ground");
                    _droneClient.FlatTrim();
                    break;

            };

            //Debug(chr);
        }
        #endregion

        #region // droneclient events
        private void OnNavigationPacketAcquired(NavigationPacket packet)
        {
            if (_packetRecorderWorker != null && _packetRecorderWorker.IsAlive)
                _packetRecorderWorker.EnqueuePacket(packet);

            _navigationPacket = packet;
        }

        private void OnVideoPacketDecoded(VideoFrame frame)
        {
            _frame = frame;
        }

        void OnVideoPacketAcquired(VideoPacket packet)
        {
            if (_packetRecorderWorker != null && _packetRecorderWorker.IsAlive)
                _packetRecorderWorker.EnqueuePacket(packet);
            if (_videoPacketDecoderWorker.IsAlive)
                _videoPacketDecoderWorker.EnqueuePacket(packet);
        }
        #endregion

        #region methods
        void GetConfig()
        {
            //Debug("Retreiving drone settings ...");

            if (_configuration != null)
            {
                this.Details.OutdoorFlight = _configuration.Control.Outdoor;
                this.Details.OutdoorHull = _configuration.Control.FlightWithoutShell;

                if (this.Details.OutdoorFlight == false)
                {
                    this.Details.AltitudeLimit = _configuration.Control.AltitudeMax;
                    this.Details.VerticalSpeedMax = (int)_configuration.Control.IndoorControlVzMax;
                    this.Details.RotationSpeedMax = Math.Round((_configuration.Control.IndoorControlYaw), 2);
                    this.Details.TiltAngleMax = Math.Round((_configuration.Control.IndoorEulerAngleMax), 2);
                }
                else
                {
                    this.Details.AltitudeLimit = _configuration.Control.AltitudeMax;
                    this.Details.VerticalSpeedMax = (int)_configuration.Control.OutdoorControlVzMax;
                    this.Details.RotationSpeedMax = Math.Round((_configuration.Control.OutdoorControlYaw), 2);
                    this.Details.TiltAngleMax = Math.Round((_configuration.Control.OutdoorEulerAngleMax), 2);
                }
            }

            //Debug("Drone settings retrieved!");
        }

        void SetConfig()
        {
            var sendConfigTask = new Task(() =>
            {
                Debug("Updating drone settings ...");

                if (_configuration == null) _configuration = new Settings();
                Settings configuration = _configuration;

                if (string.IsNullOrEmpty(configuration.Custom.SessionId) ||
                    configuration.Custom.SessionId == "00000000")
                {
                    // set new session, application and profile
                    _droneClient.AckControlAndWaitForConfirmation(); // wait for the control confirmation

                    configuration.Custom.SessionId = Settings.NewId();
                    _droneClient.Send(configuration);

                    _droneClient.AckControlAndWaitForConfirmation();

                    configuration.Custom.ProfileId = Settings.NewId();
                    _droneClient.Send(configuration);

                    _droneClient.AckControlAndWaitForConfirmation();

                    configuration.Custom.ApplicationId = Settings.NewId();
                    _droneClient.Send(configuration);

                    _droneClient.AckControlAndWaitForConfirmation();
                }

                configuration.Control.Outdoor = this.Details.OutdoorFlight;
                configuration.Control.FlightWithoutShell = this.Details.OutdoorHull;

                configuration.Control.AltitudeMax = (int)this.Details.AltitudeLimit;

                //if (this.Details.OutdoorFlight == false)
                {
                    //configuration.Control.ControlVzMax = 1f; // (float)this.Details.VerticalSpeedMax;
                    configuration.Control.ControlYaw = 0.10f; // (float)this.Details.RotationSpeedMax;
                    //configuration.Control.EulerAngleMax = 0.1f; // (float)this.Details.TiltAngleMax;
                }
                //else
                //{
                //    _configuration.Control.OutdoorControlVzMax = (float)this.Details.VerticalSpeedMax;
                //    _configuration.Control.OutdoorControlYaw = (float)this.Details.RotationSpeedMax;
                //    _configuration.Control.OutdoorEulerAngleMax = (float)this.Details.TiltAngleMax;
                //}

                this._droneClient.Send(configuration);

                Debug("New drone settings sent!");
            });
            sendConfigTask.Start();
        }

        string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        void DroneControl(object sender)
        {
            int expiration = Properties.Settings.Default.AutoPilotExpiration;
            bool useQ = Properties.Settings.Default.UseAutoPilot;

            _autopilot.Active = useQ;

            float _lrfb = Properties.Settings.Default.DefaultLRFBSpeed;
            float _yaw = Properties.Settings.Default.DefaultYawSpeed;
            float _gaz = Properties.Settings.Default.DefaultGazSpeed;

            #region // engine
            if (sender == "v")
            {
                _droneClient.FlatTrim();
                _autopilot.ClearObjectives();

                _droneClient.Takeoff();
            };
            if (sender == "n")
            {
                _autopilot.ClearObjectives();
                _droneClient.Emergency();
            };
            if (sender == "m")
            {
                _droneClient.ResetEmergency();
            };
            if (sender == "b")
            {
                _autopilot.ClearObjectives();
                _droneClient.Land();
            };
            #endregion

            _autopilot.Active = true;

            // navigation
            #region left right up down and hover
            //if (sender == btnLeft || sender == btnRight)
            //{
            //    if (useQ)
            //    {
            //        _autopilot.EnqueueObjective(Objective.Create(expiration, new SetRoll(
            //            (sender == btnLeft ? -_lrfb : _lrfb)
            //        )));
            //    }
            //    else
            //    {
            //        _droneClient.Progress(FlightMode.Progressive,
            //            roll: (sender == btnLeft ? -_lrfb : _lrfb)
            //        );
            //    }
            //}

            if (sender == "a")
            {
                if (useQ)
                {
                    _autopilot.EnqueueObjective(Objective.Create(expiration, new SetRoll(-_lrfb)));
                }
                else
                {
                    _droneClient.Progress(FlightMode.Progressive, roll: -_lrfb);
                }
            };
            if (sender == "d")
            {
                if (useQ)
                {
                    _autopilot.EnqueueObjective(Objective.Create(expiration, new SetRoll(_lrfb)));
                }
                else
                {
                    _droneClient.Progress(FlightMode.Progressive, roll: _lrfb);
                }
            };
            if (sender == "w")
            {
                if (useQ)
                {
                    _autopilot.EnqueueObjective(Objective.Create(expiration, new SetPitch(-_lrfb)));
                }
                else
                {
                    _droneClient.Progress(FlightMode.Progressive, pitch: -_lrfb);
                }
            };
            if (sender == "s")
            {
                if (useQ)
                {
                    _autopilot.EnqueueObjective(Objective.Create(expiration, new SetPitch(_lrfb)));
                }
                else
                {
                    _droneClient.Progress(FlightMode.Progressive, pitch: _lrfb);
                }
            };

            if (sender == "x")
            {
                if (useQ)
                {
                    _autopilot.EnqueueObjective(
                    Objective.Create(expiration,
                        new VelocityX(0.0f),
                        new VelocityY(0.0f),
                        new Altitude(1.0f)
                    ));
                }
                else
                {
                    _droneClient.Hover();
                }
            }
            #endregion

            #region yaw left and right
            if (sender == "j")
            {
                if (useQ)
                {
                    _autopilot.EnqueueObjective(Objective.Create(expiration, new SetYaw(-_yaw)));
                }
                else
                {
                    _droneClient.Progress(FlightMode.Progressive, yaw: -_yaw);
                }
            };
            if (sender == "l")
            {
                if (useQ)
                {
                    _autopilot.EnqueueObjective(Objective.Create(expiration, new SetYaw(_yaw)));
                }
                else
                {
                    _droneClient.Progress(FlightMode.Progressive, yaw: _yaw);
                }
            };
            #endregion

            #region gaz up and down
            if (sender == "i")
            {
                if (useQ)
                {
                    _autopilot.EnqueueObjective(Objective.Create(expiration, new SetGaz(_gaz)));
                }
                else
                {
                    _droneClient.Progress(FlightMode.Progressive, gaz: _gaz);
                }
                //_droneClient.Hover();
            };
            if (sender == "k")
            {
                if (useQ)
                {
                    _autopilot.EnqueueObjective(Objective.Create(expiration, new SetGaz(-_gaz)));
                }
                else
                {
                    _droneClient.Progress(FlightMode.Progressive, gaz: -_gaz);
                }
            };
            #endregion
        }

        void UpdateAttitudeIndicator(int pitch, int roll)
        {
            int _roll = roll;
            double _pitch = pitch;
            _pitch = 4 * _pitch;

            double newX = 0.0d;
            double newY = 0.0d;

            //Debug("pitch: " + pitch + ", roll: " + _roll);

            newX = _pitch * (Math.Cos((-_roll + 90) * (Math.PI / 180)));
            newY = _pitch * (Math.Sin((-_roll + 90) * (Math.PI / 180)));

            this.Details.PitchX = (float)newX;
            this.Details.PitchY = (float)newY;
            //Pitch2.X = newX;
            //Pitch2.Y = newY;

            this.Details.RollAngle = -roll;
        }

        void ShowAlertStatus(string text, bool show = true)
        {
            if (show)
            {
                //GridAlertStatus.Visibility = System.Windows.Visibility.Visible;
                //lblAlert.Text = text;
                this.Details.AlertStatus = text;
                this.Details.ShowAlertStatus = true;
            }
            else
            {
                //GridAlertStatus.Visibility = System.Windows.Visibility.Collapsed;
                this.Details.ShowAlertStatus = false;
            }

        }

        string lastTxt = string.Empty;
        void Debug(string txt)
        {
            string ntxt = DateTime.Now.ToString("hh:mm:ss \\> ") + txt;

            if (ntxt != lastTxt)
            {
                lastTxt = ntxt;
                System.Diagnostics.Debug.WriteLine(ntxt);
                ////lstDebug.Items.Insert(0, ntxt);

                try
                {
                    this.Details.Log.Insert(0, ntxt);
                }
                catch { }
            }
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
        #endregion
    }
}