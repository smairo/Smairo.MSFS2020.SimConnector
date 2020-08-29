using Microsoft.FlightSimulator.SimConnect;
using Smairo.MSFS2020.Model;
using Smairo.MSFS2020.Model.Enumeration;
using Smairo.MSFS2020.Model.Structs;
using Smairo.MSFS2020.SimConnector.Interfaces;
using Smairo.MSFS2020.SimConnector.Writers;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Smairo.MSFS2020.SimConnector
{
    public class SimvarsViewModel : IBaseSimConnectWrapper
    {
        // User-defined win32 event
        public const int WmUserSimconnect = 0x0402;

        // Writer to use
        private readonly IWriter _dataWriter = new JsonFileWriter();

        public bool Connected { get; private set; }

        private IntPtr _hWnd = new IntPtr(0);
        private SimConnect _simConnection;
        private readonly DispatcherTimer _pullDataTimer = new DispatcherTimer();
        private readonly SimVarMapper _mapper;
        public SimvarsViewModel()
        {
            _pullDataTimer.Interval = new TimeSpan(0, 0, 0, 5, 0);
            _pullDataTimer.Tick += OnTickPullData;
            _mapper = new SimVarMapper();
        }

        private void OnTickPullData(object sender, EventArgs e)
        {
            try
            {
                _simConnection?.RequestDataOnSimObjectType(
                    Request.PLANE_ALTITUDE,
                    Definition.PlaneVariables,
                    0,
                    SIMCONNECT_SIMOBJECT_TYPE.USER);

                _simConnection?.RequestDataOnSimObjectType(
                    Request.REALISM_CRASH_DETECTION,
                    Definition.SimulationVariables,
                    0,
                    SIMCONNECT_SIMOBJECT_TYPE.USER);

                _simConnection?.RequestDataOnSimObjectType(
                    Request.TITLE,
                    Definition.PlaneMetadatas,
                    0,
                    SIMCONNECT_SIMOBJECT_TYPE.USER);
            }
            catch (COMException ex)
            {
                Console.WriteLine("Connection to KH failed: " + ex.Message);
                Connected = false;
            }
        }

        public void Connect()
        {
            if (Connected) return;
            try
            {
                // The constructor is similar to SimConnect_Open in the native API
                _simConnection = new SimConnect(
                    $"Simconnect{Guid.NewGuid()}",
                    _hWnd,
                    WmUserSimconnect,
                    null,
                    0);

                Connected = true;

                _simConnection.OnRecvOpen += OnRecvOpen;
                _simConnection.OnRecvQuit += OnRecvQuit;
                _simConnection.OnRecvException += OnRecvException;
                _simConnection.OnRecvSimobjectDataBytype += OnRecvSimobjectDataByType;
                _simConnection.OnRecvSimobjectData += OnRecvSimobjectData;
                _simConnection.OnRecvEvent += OnRecvEvent;

                // Setup crash
                _simConnection.SubscribeToSystemEvent(
                    Event.PlaneCrashed,
                    "Crashed");
                _simConnection.SubscribeToSystemEvent(
                    Event.PositionChanged,
                    "PositionChanged");
                _simConnection.SubscribeToSystemEvent(
                    Event.SimStart,
                    "SimStart");

                //_simConnection.SubscribeToSystemEvent(
                //    Event.FlightLoaded,
                //    "FlightLoaded");
                
                var definition = Definition.PlaneMetadatas;
                foreach (var value in _mapper.GetRequestsForStruct<PlaneMetadatas>())
                {
                    _simConnection.AddToDataDefinition(
                        definition,
                        value.NameUnitTuple.Name,
                        value.NameUnitTuple.Unit,
                        value.DataType,
                        0.0f,
                        SimConnect.SIMCONNECT_UNUSED);
                }
                _simConnection.RegisterDataDefineStruct<PlaneMetadatas>(definition);

                definition = Definition.PlaneVariables;
                foreach (var value in _mapper.GetRequestsForStruct<PlaneVariables>())
                {
                    _simConnection.AddToDataDefinition(
                        definition,
                        value.NameUnitTuple.Name,
                        value.NameUnitTuple.Unit,
                        value.DataType,
                        0.0f,
                        SimConnect.SIMCONNECT_UNUSED);
                }
                _simConnection.RegisterDataDefineStruct<PlaneVariables>(definition);

                definition = Definition.SimulationVariables;
                foreach (var value in _mapper.GetRequestsForStruct<SimulationVariables>())
                {
                    _simConnection.AddToDataDefinition(
                            definition,
                            value.NameUnitTuple.Name,
                            value.NameUnitTuple.Unit,
                            value.DataType,
                            0.0f,
                            SimConnect.SIMCONNECT_UNUSED);
                }
                _simConnection.RegisterDataDefineStruct<SimulationVariables>(definition);
            }
            catch (COMException ex)
            {
                Console.WriteLine("Connection to KH failed: " + ex.Message);
                Connected = false;
            }
        }

        private void OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            var val = data.dwData.FirstOrDefault();
            if (val is null)
                return;

            PlaneMetadatas? planeMetadata = null;
            PlaneVariables? planeVariables = null;
            SimulationVariables? simVariables = null;
            switch ((Definition) data.dwDefineID)
            {
                case Definition.PlaneMetadatas:
                    planeMetadata = (PlaneMetadatas)val;
                    break;
                case Definition.PlaneVariables:
                    planeVariables = (PlaneVariables)val;
                    break;
                case Definition.SimulationVariables:
                    simVariables = (SimulationVariables)val;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _dataWriter.WriteToStoreAsync(planeMetadata, planeVariables, simVariables)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private void OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data)
        {
            switch (data.uEventID)
            {
                case 0:
                    Console.WriteLine("Plane Crashed!");
                    break;
                case 1:
                    Console.WriteLine("Position changed!");
                    break;
                case 2:
                    Console.WriteLine("Sim start/stop...");
                    break;
            }
            
            //_dataWriter.WriteToStoreAsync(planeCrashed: true)
            //    .ConfigureAwait(false)
            //    .GetAwaiter()
            //    .GetResult();
        }

        #region Client reciever events
        private void OnRecvSimobjectDataByType(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            var val = data.dwData.FirstOrDefault();
            if (val is null)
                return;

            PlaneMetadatas? planeMetadata = null;
            PlaneVariables? planeVariables = null;
            SimulationVariables? simVariables = null;
            switch ((Definition) data.dwDefineID)
            {
                case Definition.PlaneMetadatas:
                    planeMetadata = (PlaneMetadatas)val;
                    break;
                case Definition.PlaneVariables:
                    planeVariables = (PlaneVariables)val;
                    break;
                case Definition.SimulationVariables:
                    simVariables = (SimulationVariables)val;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _dataWriter.WriteToStoreAsync(planeMetadata, planeVariables, simVariables)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private void OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
            Console.WriteLine("SimConnect_OnRecvException: " + eException.ToString());
            //Connected = false;
        }

        private void OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Connected = false;
            _pullDataTimer.Stop();
        }

        private void OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            _pullDataTimer.Start();
        }
        #endregion

        public int GetUserSimConnectWinEvent()
        {
            return WmUserSimconnect;
        }

        public void ReceiveSimConnectMessage()
        {
            _simConnection?.ReceiveMessage();
        }

        public void SetWindowHandle(IntPtr hWnd)
        {
            _hWnd = hWnd;
        }

        public void Disconnect()
        {
        }
    }
}