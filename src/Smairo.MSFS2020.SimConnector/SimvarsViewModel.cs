﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.FlightSimulator.SimConnect;
using Smairo.MSFS2020.Model;
using Smairo.MSFS2020.Model.Enumeration;
using Smairo.MSFS2020.Model.Structs;
using Smairo.MSFS2020.SimConnector.Interfaces;

namespace Smairo.MSFS2020.SimConnector
{
    public class SimvarsViewModel : IBaseSimConnectWrapper
    {
        // User-defined win32 event
        public const int WmUserSimconnect = 0x0402;
        public bool Connected { get; private set; }
        private bool _collectionSet;

        private IntPtr _hWnd = new IntPtr(0);
        private SimConnect _simConnection;
        private readonly DispatcherTimer _pullDataTimer = new DispatcherTimer();

        public SimvarsViewModel()
        {
            _pullDataTimer.Interval = new TimeSpan(0, 0, 0, 5, 0);
            _pullDataTimer.Tick += OnTickPullData;
        }

        private void OnTickPullData(object sender, EventArgs e)
        {
            //if(_collectionSet) return;
            // TODO EX

            _simConnection?.RequestDataOnSimObjectType(
               Request.PlaneAltitude,
               Definition.PlaneVariable,
               0,
               SIMCONNECT_SIMOBJECT_TYPE.USER);

            _simConnection?.RequestDataOnSimObjectType(
                Request.RealismCrashDetection,
                Definition.SimulationVariable,
                0,
                SIMCONNECT_SIMOBJECT_TYPE.USER);

            _simConnection?.RequestDataOnSimObjectType(
                Request.Title,
                Definition.PlaneMetadata,
                0,
                SIMCONNECT_SIMOBJECT_TYPE.USER);

            //var requests = _requestedData;
            //for (var i = 0; i < requests.Count; i++)
            //{
            //    var simvarRequest = requests[i];
            //    //_simConnection?.RequestDataOnSimObject(
            //    //    simvarRequest.Request,
            //    //    simvarRequest.Definition,
            //    //    (uint) i,
            //    //    SIMCONNECT_PERIOD.SECOND,
            //    //    SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT,
            //    //    0,
            //    //    0,
            //    //    0
            //    //);
            //    _simConnection?.RequestDataOnSimObjectType(
            //        simvarRequest.Request,
            //        simvarRequest.Definition,
            //       0,
            //       SIMCONNECT_SIMOBJECT_TYPE.USER);
            //}
            //foreach (var simvarRequest in _requestedData)
            //{
            //    _simConnection?.RequestDataOnSimObject(
            //        simvarRequest.Request,
            //        simvarRequest.Definition,
            //        0,
            //        SIMCONNECT_PERIOD.SECOND,
            //        SIMCONNECT_DATA_REQUEST_FLAG.CHANGED,
            //        0,
            //        0,
            //        0
            //    );
            //}

            _collectionSet = true;
        }

        public void Connect()
        {
            if (Connected) return;
            try
            {
                // The constructor is similar to SimConnect_Open in the native API
                _simConnection = new SimConnect(
                    "Simconnect - Simvar test",
                    _hWnd,
                    WmUserSimconnect,
                    null,
                    0);

                Connected = true;

                _simConnection.OnRecvOpen += OnRecvOpen;
                _simConnection.OnRecvQuit += OnRecvQuit;
                _simConnection.OnRecvException += OnRecvException;
                _simConnection.OnRecvSimobjectDataBytype += OnRecvSimobjectDataBytype;
                _simConnection.OnRecvSimobjectData += OnRecvSimobjectData;

                _simConnection.OnRecvClientData += OnRecvClientData;

                var definition = Definition.PlaneMetadata;
                foreach (var value in GetMetadataRequests())
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

                definition = Definition.PlaneVariable;
                foreach (var value in GetPlaneRequests())
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

                definition = Definition.SimulationVariable;
                foreach (var value in GetSimulationRequests())
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


                //foreach (var value in _requestedData)
                //{
                //    _simConnection.AddToDataDefinition(
                //        value.Definition,
                //        value.NameUnitTuple.Name,
                //        value.NameUnitTuple.Unit,
                //        value.DataType,
                //        0.0f,
                //        SimConnect.SIMCONNECT_UNUSED);

                //    _simConnection.RegisterDataDefineStruct<double>(value.Definition);
                //}
            }
            catch (COMException ex)
            {
                Console.WriteLine("Connection to KH failed: " + ex.Message);
                Connected = false;
            }
        }

        #region Client reciever events
        private void OnRecvClientData(SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data)
        {
        }

        private void OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            var val = data.dwData.FirstOrDefault();
            if (val is null)
            {
                return;
            }

            Console.WriteLine($"Val is: {data.dwRequestID} - {val}");
        }

        private void OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            var val = data.dwData.FirstOrDefault();
            if (val is null)
            {
                return;
            }

            switch ((Definition) data.dwDefineID)
            {
                case Definition.PlaneMetadata:
                    var metadata = (PlaneMetadatas) val;
                    Console.WriteLine($"Plane is: {metadata.Title} - {metadata.AtcModel}");
                    break;
                case Definition.PlaneVariable:
                    var planeVariables = (PlaneVariables) val;
                    Console.WriteLine($"Plane is at: '{planeVariables.Latitude} {planeVariables.Longitude}' and flying in {planeVariables.Altitude} ft");
                    break;
                case Definition.SimulationVariable:
                    var simulationVariables = (SimulationVariables) val;
                    Console.WriteLine($"Can crash: {simulationVariables.RealismCrashDetection}. Crashing flag: {simulationVariables.CrashFlag}.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Console.WriteLine($"Val is: {data.dwRequestID} - {val}");
        }

        private void OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Connected = false;
        }

        private void OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Connected = false;
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

        private static List<SimvarRequest> GetSimulationRequests()
        {
            return new List<SimvarRequest>
            {
                new SimvarRequest
                {
                    Definition = Definition.SimulationVariable,
                    NameUnitTuple = ( "UNLIMITED FUEL", "bool"),
                    Request = Request.UnlimitedFuelFlag
                },
                new SimvarRequest
                {
                    Definition = Definition.SimulationVariable,
                    NameUnitTuple = ( "REALISM", "number"),
                    Request = Request.RealismPercentage
                },
                new SimvarRequest
                {
                    Definition = Definition.SimulationVariable,
                    NameUnitTuple = ( "REALISM CRASH DETECTION", "bool"),
                    Request = Request.RealismCrashDetection
                },
                new SimvarRequest
                {
                    Definition = Definition.SimulationVariable,
                    NameUnitTuple = ( "SIMULATION RATE", "number"),
                    Request = Request.SimulationRate
                },
                new SimvarRequest
                {
                    Definition = Definition.SimulationVariable,
                    NameUnitTuple = ( "CRASH FLAG", "enum"),
                    Request = Request.CrashFlag
                }
            };
        }

        private static List<SimvarRequest> GetPlaneRequests()
        {
            return new List<SimvarRequest>
            {
                new SimvarRequest
                {
                    Definition = Definition.PlaneVariable,
                    NameUnitTuple = ( "PLANE ALTITUDE", "feet"),
                    Request = Request.PlaneAltitude
                },
                new SimvarRequest
                {
                    Definition = Definition.PlaneVariable,
                    NameUnitTuple = ( "PLANE LONGITUDE", "degree"),
                    Request = Request.PlaneLongitude
                },
                new SimvarRequest
                {
                    Definition = Definition.PlaneVariable,
                    NameUnitTuple = ( "PLANE LATITUDE", "degree"),
                    Request = Request.PlaneLatitude
                },
                new SimvarRequest
                {
                    Definition = Definition.PlaneVariable,
                    NameUnitTuple = ( "FUEL TOTAL QUANTITY", "gallons"),
                    Request = Request.FuelTotalQuantity
                },
                new SimvarRequest
                {
                    Definition = Definition.PlaneVariable,
                    NameUnitTuple = ( "AIRSPEED TRUE", "knots"),
                    Request = Request.AirSpeedTrue
                },
                new SimvarRequest
                {
                    Definition = Definition.PlaneVariable,
                    NameUnitTuple = ( "TOTAL WEIGHT", "pounds"),
                    Request = Request.TotalWeight
                },
            };
        }

        private static List<SimvarRequest> GetMetadataRequests()
        {
            return new List<SimvarRequest>
            {
                new SimvarRequest
                {
                    Definition = Definition.PlaneMetadata,
                    NameUnitTuple = ( "ATC TYPE", ""),
                    Request = Request.AtcType,
                    DataType = SIMCONNECT_DATATYPE.STRING256
                },
                new SimvarRequest
                {
                    Definition = Definition.PlaneMetadata,
                    NameUnitTuple = ( "ATC MODEL", ""),
                    Request = Request.AtcModel,
                    DataType = SIMCONNECT_DATATYPE.STRING256
                },
                new SimvarRequest
                {
                    Definition = Definition.PlaneMetadata,
                    NameUnitTuple = ( "ATC AIRLINE", ""),
                    Request = Request.AtcAirline,
                    DataType = SIMCONNECT_DATATYPE.STRING256
                },
                new SimvarRequest
                {
                    Definition = Definition.PlaneMetadata,
                    NameUnitTuple = ( "ATC ID", ""),
                    Request = Request.AtcId,
                    DataType = SIMCONNECT_DATATYPE.STRING256
                },
                new SimvarRequest
                {
                    Definition = Definition.PlaneMetadata,
                    NameUnitTuple = ( "ATC FLIGHT NUMBER", ""),
                    Request = Request.AtcFlightNumber,
                    DataType = SIMCONNECT_DATATYPE.STRING256
                },
                new SimvarRequest
                {
                    Definition = Definition.PlaneMetadata,
                    NameUnitTuple = ( "TITLE", ""),
                    Request = Request.Title,
                    DataType = SIMCONNECT_DATATYPE.STRING256
                }
            };
        }
    }
}