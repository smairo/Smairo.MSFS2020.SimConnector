using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Smairo.MSFS2020.Model;
using Smairo.MSFS2020.Model.Structs;
using Smairo.MSFS2020.SimConnector.Interfaces;

namespace Smairo.MSFS2020.SimConnector.Writers
{
    public class JsonFileWriter : IWriter
    {
        private PlaneMetadatas? _currentPlaneMetadata;
        private PlaneVariables? _currentPlaneVariables;
        private SimulationVariables? _currentSimVariables;

        public async Task WriteToStoreAsync(
            PlaneMetadatas? metadata = null,
            PlaneVariables? planeVariables = null,
            SimulationVariables? simulationVariables = null,
            bool? planeCrashed = null,
            bool? planeLanded = null)
        {
            if (planeCrashed.HasValue && planeCrashed.Value ||
                planeLanded.HasValue && planeLanded.Value)
                await EndFlightAsync(planeCrashed, planeLanded);

            if (metadata.HasValue)
                _currentPlaneMetadata = metadata.Value;

            if (planeVariables.HasValue)
                _currentPlaneVariables = planeVariables.Value;

            if (simulationVariables.HasValue)
                _currentSimVariables = simulationVariables.Value;

            if (_currentPlaneMetadata.HasValue
                && _currentPlaneVariables.HasValue
                && _currentSimVariables.HasValue)
            {
                var flightData = new SimVarCollection(
                    _currentPlaneMetadata.Value,
                    _currentPlaneVariables.Value,
                    _currentSimVariables.Value);

                await WriteToJsonFileAsync(flightData);
            }
        }

        private Task WriteToJsonFileAsync(SimVarCollection flightData)
        {
            var json = flightData.ToJson();
            //Console.Write(json + Environment.NewLine);
            File.WriteAllText("flight.json", json, Encoding.UTF8);

            _currentSimVariables = null;
            _currentPlaneVariables = null;
            _currentPlaneMetadata = null;
            return Task.CompletedTask;
        }

        private static Task EndFlightAsync(bool? planeCrashed, bool? planeLanded)
        {
            throw new NotImplementedException();
        }
    }
}