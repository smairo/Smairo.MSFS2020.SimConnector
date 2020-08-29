using System.Runtime.InteropServices;

namespace Smairo.MSFS2020.Model.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PlaneVariables
    {
        public double PLANE_ALTITUDE;
        public double PLANE_LONGITUDE;
        public double PLANE_LATITUDE;
        public double AIRSPEED_INDICATED;
        public double SIM_ON_GROUND;
    }
}