using System.Collections.Generic;
using Microsoft.FlightSimulator.SimConnect;
using Smairo.MSFS2020.Model.Enumeration;
namespace Smairo.MSFS2020.Model
{
    public class SimvarRequest
    {
        public Definition Definition { get; set; }
        public Request Request { get; set; }
        public (string Name, string Unit) NameUnitTuple { get; set; }
        public SIMCONNECT_DATATYPE DataType { get; set; } = SIMCONNECT_DATATYPE.FLOAT64;
    };
}