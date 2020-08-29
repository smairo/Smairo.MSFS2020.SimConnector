using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Smairo.MSFS2020.Model.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct GpsVariables
    {
        public double GPS_IS_ACTIVE_FLIGHT_PLAN;
        public double GPS_IS_ARRIVED;
    }
}
