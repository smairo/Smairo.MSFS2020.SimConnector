using System.Runtime.InteropServices;

namespace Smairo.MSFS2020.Model.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PlaneVariables
    {
        public double Altitude;
        public double Longitude;
        public double Latitude;
        public double FuelTotal;
        public double AirspeedTrue;
        public double TotalWeight;
    }
}