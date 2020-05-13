using System.Runtime.InteropServices;

namespace AR.Drone.Data.Navigation.Native.Math
{
    [StructLayout(LayoutKind.Sequential)]
    public struct matrix33_t
    {
        public float m11;
        public float m12;
        public float m13;
        public float m21;
        public float m22;
        public float m23;
        public float m31;
        public float m32;
        public float m33;
    }
}