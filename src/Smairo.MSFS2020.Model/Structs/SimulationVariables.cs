using System.Runtime.InteropServices;

namespace Smairo.MSFS2020.Model.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SimulationVariables
    {
        public double UNLIMITED_FUEL;
        public double REALISM_CRASH_DETECTION;
        public double SIMULATION_RATE;
        //public double REALISM;
    }
}