using System.Runtime.InteropServices;

namespace AR.Drone.Data.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential)]
    public struct navdata_games_t
    {
        public ushort tag;
        public ushort size;
        public uint double_tap_counter;
        public uint finish_line_counter;
    }
}