﻿using System.Runtime.InteropServices;

namespace Smairo.MSFS2020.Model.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SimulationVariables
    {
        public double SimulationRate;
        public double RealismPercentage;
        public bool RealismCrashDetection;
        public bool UnlimitedFuelFlag;
        public int CrashFlag;
    }
}