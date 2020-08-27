using Newtonsoft.Json;
using Smairo.MSFS2020.Model.Structs;
namespace Smairo.MSFS2020.Model
{
    public class SimVarCollection
    {
        public SimVarCollection(
            PlaneMetadatas metadata,
            PlaneVariables planeVariables,
            SimulationVariables simVariables)
        {
            SimulationRate = simVariables.SimulationRate;
            RealismCrashDetection = simVariables.RealismCrashDetection;
            RealismPercentage = simVariables.RealismPercentage;
            UnlimitedFuelFlag = simVariables.UnlimitedFuelFlag;
            CrashFlag = simVariables.CrashFlag;
            Altitude = planeVariables.Altitude;
            Latitude = planeVariables.Latitude;
            Longitude = planeVariables.Longitude;
            AirspeedTrue = planeVariables.AirspeedTrue;
            FuelTotal = planeVariables.FuelTotal;
            TotalWeight = planeVariables.TotalWeight;
            AtcAirline = metadata.AtcAirline;
            AtcFlightNumber = metadata.AtcFlightNumber;
            AtcId = metadata.AtcId;
            AtcType = metadata.AtcType;
            AtcModel = metadata.AtcModel;
            Title = metadata.Title;
        }

        public double SimulationRate { get; set; }
        public double RealismPercentage { get; set; }
        public bool RealismCrashDetection { get; set; }
        public bool UnlimitedFuelFlag { get; set; }
        public int CrashFlag { get; set; }
        public double Altitude { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double AirspeedTrue { get; set; }
        public double FuelTotal { get; set; }
        public double TotalWeight { get; set; }
        public string AtcType { get; set; }
        public string AtcModel { get; set; }
        public string AtcAirline { get; set; }
        public string AtcId { get; set; }
        public string AtcFlightNumber { get; set; }
        public string Title { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                new JsonSerializerSettings {});
        }
    }
}
