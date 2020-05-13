using System.Runtime.InteropServices;

namespace AR.Drone.Data.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential)]
    public struct velocities_t
    {
        public float x;
        public float y;
        public float z;
    }
}