using System.Runtime.InteropServices;

namespace AR.Drone.Data.Navigation.Native.Options
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct navdata_gyros_offsets_t
    {
        public ushort tag;
        public ushort size;
        public fixed float offset_g [3]; // <Ctype "float32_t * NB_GYROS">
    }
}