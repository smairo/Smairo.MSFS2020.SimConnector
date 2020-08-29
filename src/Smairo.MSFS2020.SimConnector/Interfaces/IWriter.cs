using System.Threading.Tasks;
using Smairo.MSFS2020.Model.Structs;
namespace Smairo.MSFS2020.SimConnector.Interfaces
{
    public interface IWriter
    {
        Task WriteToStoreAsync(
            PlaneMetadatas? metadata = null,
            PlaneVariables? planeVariables = null,
            SimulationVariables? simulationVariables = null,
            GpsVariables? gpsVariables = null,
            bool? planeCrashed = null,
            bool? planeLanded = null);
    }
}