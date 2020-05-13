using AR.Drone.Data.Navigation;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApplication1.Model
{
    public class HudDetails : ViewModelBase
    {
        private ObservableCollection<string> _Log = new ObservableCollection<string>();
        public ObservableCollection<string> Log
        {
            get { return _Log; }
            set
            {
                if (_Log != value)
                {
                    _Log = value;
                    base.RaisePropertyChanged("Log");
                }
            }
        }

        private int _BatteryLevel = 0;
        public int BatteryLevel
        {
            get { return _BatteryLevel; }
            set
            {
                if (_BatteryLevel != value)
                {
                    _BatteryLevel = value;
                    base.RaisePropertyChanged("BatteryLevel");
                }
            }
        }

        private string _DroneWiFiName = "disconnected";
        public string DroneWiFiName
        {
            get { return _DroneWiFiName; }
            set
            {
                if (_DroneWiFiName != value)
                {
                    _DroneWiFiName = value;
                    base.RaisePropertyChanged("DroneWiFiName");
                }
            }
        }

        private double _WiFiSignalStrength = 0;
        public double WiFiSignalStrength
        {
            get { return _WiFiSignalStrength; }
            set
            {
                if (_WiFiSignalStrength != value)
                {
                    _WiFiSignalStrength = value;
                    base.RaisePropertyChanged("WiFiSignalStrength");
                }
            }
        }

        private ImageSource _VideoFrame;
        public ImageSource VideoFrame
        {
            get { return _VideoFrame; }
            set
            {
                if (_VideoFrame != value)
                {
                    _VideoFrame = value;
                    base.RaisePropertyChanged("VideoFrame");
                }
            }
        }

        private float _Pitch = 0.0f;
        public float Pitch
        {
            get { return _Pitch; }
            set
            {
                if (_Pitch != value)
                {
                    _Pitch = value;
                    base.RaisePropertyChanged("Pitch");
                }
            }
        }

        private float _PitchX = 0.0f;
        public float PitchX
        {
            get { return _PitchX; }
            set
            {
                if (_PitchX != value)
                {
                    _PitchX = value;
                    base.RaisePropertyChanged("PitchX");
                }
            }
        }

        private float _PitchY = 0.0f;
        public float PitchY
        {
            get { return _PitchY; }
            set
            {
                if (_PitchY != value)
                {
                    _PitchY = value;
                    base.RaisePropertyChanged("PitchY");
                }
            }
        }

        private float _Yaw = 0.0f;
        public float Yaw
        {
            get { return _Yaw; }
            set
            {
                if (_Yaw != value)
                {
                    _Yaw = value;
                    base.RaisePropertyChanged("Yaw");
                }
            }
        }

        private float _Roll = 0.0f;
        public float Roll
        {
            get { return _Roll; }
            set
            {
                if (_Roll != value)
                {
                    _Roll = value;
                    base.RaisePropertyChanged("Roll");
                }
            }
        }

        private float _RollAngle = 0.0f;
        public float RollAngle
        {
            get { return _RollAngle; }
            set
            {
                if (_RollAngle != value)
                {
                    _RollAngle = value;
                    base.RaisePropertyChanged("RollAngle");
                }
            }
        }

        private float _Altitude = 0.0f;
        public float Altitude
        {
            get { return _Altitude; }
            set
            {
                if (_Altitude != value)
                {
                    _Altitude = value;
                    base.RaisePropertyChanged("Altitude");
                }
            }
        }

        private float _RotateX = 0;
        public float RotateX
        {
            get { return _RotateX; }
            set
            {
                if (_RotateX != value)
                {
                    _RotateX = value;
                    base.RaisePropertyChanged("RotateX");
                }
            }
        }

        private float _RotateY = 0;
        public float RotateY
        {
            get { return _RotateY; }
            set
            {
                if (_RotateY != value)
                {
                    _RotateY = value;
                    base.RaisePropertyChanged("RotateY");
                }
            }
        }

        private float _RotateZ = 0;
        public float RotateZ
        {
            get { return _RotateZ; }
            set
            {
                if (_RotateZ != value)
                {
                    _RotateZ = value;
                    base.RaisePropertyChanged("RotateZ");
                }
            }
        }

        private NavigationData _NaviData = new NavigationData();
        public NavigationData NaviData
        {
            get { return _NaviData; }
            set
            {
                if (_NaviData != value)
                {
                    _NaviData = value;
                    base.RaisePropertyChanged("NaviData");
                }
            }
        }

        private bool _ShowAlertStatus = false;
        public bool ShowAlertStatus
        {
            get { return _ShowAlertStatus; }
            set
            {
                if (_ShowAlertStatus != value)
                {
                    _ShowAlertStatus = value;
                    base.RaisePropertyChanged("ShowAlertStatus");
                }
            }
        }

        private string _AlertStatus = string.Empty;
        public string AlertStatus
        {
            get { return _AlertStatus; }
            set
            {
                if (_AlertStatus != value)
                {
                    _AlertStatus = value;
                    base.RaisePropertyChanged("AlertStatus");
                }
            }
        }

        private bool _ShowDroneLayer = Properties.Settings.Default.ShowDroneImage;
        public bool ShowDroneLayer
        {
            get { return _ShowDroneLayer; }
            set
            {
                if (_ShowDroneLayer != value)
                {
                    Properties.Settings.Default.ShowDroneImage = value;
                    _ShowDroneLayer = value;
                    base.RaisePropertyChanged("ShowDroneLayer");
                }
            }
        }

        #region Settings
        private bool _OutdoorFlight = false;
        /// <summary>
        /// Description :
        /// This settings tells the control loop that the AR.Drone is flying outside.
        /// Setting the indoor/outdoor flight will load the corresponding indoor/outdoor_control_yaw, indoor/outdoor_euler_
        /// angle_max and indoor/outdoor_control_vz_max.
        /// Note : This settings enables the wind estimator of the AR.Drone 2.0 , and thus should always
        /// </summary>
        public bool OutdoorFlight
        {
            get { return _OutdoorFlight; }
            set
            {
                if (_OutdoorFlight != value)
                {
                    _OutdoorFlight = value;
                    base.RaisePropertyChanged("OutdoorFlight");
                }
            }
        }

        private bool _OutdoorHull = false;
        /// <summary>
        /// Description :
        /// This settings tells the control loop that the AR.Drone is currently using the outdoor hull. Deactivate it when flying
        /// with the indoor hull
        /// Note : This settings corresponds to the outdoor hull setting of AR.FreeFlight.
        /// Note : This setting is not linked with the CONTROL:outdoor setting. They have different effects on the control loop.
        /// </summary>
        public bool OutdoorHull
        {
            get { return _OutdoorHull; }
            set
            {
                if (_OutdoorHull != value)
                {
                    _OutdoorHull = value;
                    base.RaisePropertyChanged("OutdoorHull");
                }
            }
        }

        private int _AltitudeLimit = 0;
        /// <summary>
        /// Description :
        /// Maximum drone altitude in millimeters.
        /// On AR.Drone 1.0 : Give an integer value between 500 and 5000 to prevent the drone from flying above this limit,
        /// or set it to 10000 to let the drone fly as high as desired. On AR.Drone 2.0 : Any value will be set as a maximum
        /// altitude, as the pressure sensor allow altitude
        /// </summary>
        public int AltitudeLimit
        {
            get { return _AltitudeLimit; }
            set
            {
                if (_AltitudeLimit != value)
                {
                    _AltitudeLimit = value;
                    base.RaisePropertyChanged("AltitudeLimit");
                }
            }
        }

        private int _VerticalSpeedMax = 0;
        public int VerticalSpeedMax
        {
            get { return _VerticalSpeedMax; }
            set
            {
                if (_VerticalSpeedMax != value)
                {
                    _VerticalSpeedMax = value;
                    base.RaisePropertyChanged("VerticalSpeedMax");
                }
            }
        }

        private double _RotationSpeedMax = 0.0f;
        public double RotationSpeedMax
        {
            get { return _RotationSpeedMax; }
            set
            {
                if (_RotationSpeedMax != value)
                {
                    _RotationSpeedMax = value;
                    base.RaisePropertyChanged("RotationSpeedMax");
                }
            }
        }

        private double _TiltAngleMax = 0.0f;
        public double TiltAngleMax
        {
            get { return _TiltAngleMax; }
            set
            {
                if (_TiltAngleMax != value)
                {
                    _TiltAngleMax = value;
                    base.RaisePropertyChanged("TiltAngleMax");
                }
            }
        }
        #endregion
    }
}
