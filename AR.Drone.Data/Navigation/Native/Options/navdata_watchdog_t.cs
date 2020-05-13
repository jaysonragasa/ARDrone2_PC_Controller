using System.Runtime.InteropServices;

namespace AR.Drone.Data.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential)]
    public struct navdata_watchdog_t
    {
        public ushort tag;
        public ushort size;
        public int watchdog;
    }
}