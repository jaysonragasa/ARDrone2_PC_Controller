/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using AR.Drone.Data.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
//using ARDrone;
//using ARDrone.Control;
//using ARDrone.Control.Data;

namespace AviationInstruments
{
    public class InstrumentsManager
    {
        //private DroneControl droneControl;
        private NavigationData _navData;
        private List<InstrumentControl> instrumentList;

        readonly object stateLock = new object();
        bool shouldThreadBeTerminated = false;

        public InstrumentsManager(ref NavigationData navData)
        {
            this._navData = navData;
            instrumentList = new List<InstrumentControl>();
        }

        public void addInstrument(InstrumentControl instrumentControl)
        {
            this.instrumentList.Add(instrumentControl);
        }

        public void startManage()
        {
            lock (stateLock)
            {
                shouldThreadBeTerminated = false;
            }

            Thread workerThread = new Thread(this.manage);
            workerThread.Start();
        }

        public void stopManage()
        {
            lock (stateLock)
            {
                shouldThreadBeTerminated = true;
            }
        }

        private void manage()
        {
            bool localStop = false;
            lock (stateLock)
            {
                localStop = shouldThreadBeTerminated;
            }

            while (!localStop)
            {
                lock (stateLock)
                {
                    localStop = shouldThreadBeTerminated;
                }
                this.updateInstruments();
                Thread.Sleep(100);
            }
        }

        private void updateInstruments()
        {
            //DroneData droneData = null;
            //if (droneControl.IsConnected)
            //   droneData = droneControl.NavigationData;
            //else
            //    droneData = new DroneData();

            foreach (InstrumentControl instrumentControl in instrumentList)
            {
                try
                {
                    switch (instrumentControl.GetType().Name)
                    {
                        case "AttitudeIndicatorInstrumentControl":
                            updateInstrument((AttitudeIndicatorInstrumentControl)instrumentControl);
                            break;

                        case "AltimeterInstrumentControl":
                            updateInstrument((AltimeterInstrumentControl)instrumentControl);
                            break;

                        case "HeadingIndicatorInstrumentControl":
                            updateInstrument((HeadingIndicatorInstrumentControl)instrumentControl);
                            break;
                        case "VerticalSpeedIndicatorInstrumentControl":
                            updateInstrument((VerticalSpeedIndicatorInstrumentControl)instrumentControl);
                            break;
                    }
                }
                catch (InvalidOperationException)
                { }
                catch (Exception)
                {
                    this.stopManage();
                }
            }
        }

        private void updateInstrument(AttitudeIndicatorInstrumentControl control)
        {
            control.Invoke((MethodInvoker)delegate
            {
                if (_navData != null)
                {
                    control.SetAttitudeIndicatorParameters(_navData.Pitch, -_navData.Roll);
                }
            });
        }

        private void updateInstrument(AltimeterInstrumentControl control)
        {
            control.Invoke((MethodInvoker)delegate
            {
                if (_navData != null)
                {
                    control.SetAlimeterParameters((int)_navData.Altitude);
                }
            });
        }

        private void updateInstrument(HeadingIndicatorInstrumentControl control)
        {
            control.Invoke((MethodInvoker)delegate
            {
                if (_navData != null)
                {
                    // Psi range -180..0..180 but heading indicator require 0..360
                    //if (droneData.Psi > 0)
                    //{
                    //    control.SetHeadingIndicatorParameters(Convert.ToInt32(droneData.Psi));
                    //}
                    //else
                    //{
                    //    control.SetHeadingIndicatorParameters(360 + Convert.ToInt32(droneData.Psi));
                    //}
                }
            });
        }

        private void updateInstrument(VerticalSpeedIndicatorInstrumentControl control)
        {
            control.Invoke((MethodInvoker)delegate
            {
                if (_navData != null)
                {
                    //control.SetVerticalSpeedIndicatorParameters(_navData);
                }
            });
        }
    }
}
