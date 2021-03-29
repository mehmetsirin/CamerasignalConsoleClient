using Microsoft.AspNet.SignalR.Client;

using Newtonsoft.Json;

using SIGNALRCONSOLECLIENT._Console.Model;

using System;
using System.Collections.Generic;
using System.IO;

namespace SIGNALRCONSOLECLIENT._Console
{
    internal class Program
    {

        static SettingsDto settingsDto = new SettingsDto();
        static HubConnection hubConnection = null;
        static IHubProxy myHub = null;
        public static string IMEI = "10";
        private static void Main(string[] args)
        {
            //Set connection

            //string url = "http://localhost:53306//signalr";
            var url = "http://ats2.rota.net.tr/signalr";

            hubConnection = new HubConnection(url, "key=1");

            //To enable client-side logging, set the TraceLevel and TraceWriter properties on the connection object.
            //Logging client events to a text file under SIGNALRCONSOLECLIENT._Console\bin\Debug
            var writer = new StreamWriter("clientLog.txt");
            writer.AutoFlush = true;
            hubConnection.TraceLevel = TraceLevels.All;
            hubConnection.TraceWriter = writer;

            //Make proxy to hub based on hub name on server
            myHub = hubConnection.CreateHubProxy("RealDataNew");

            //Start connection
            hubConnection.Start().ContinueWith(task =>
             {
                 if (task.IsFaulted)

                     Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                 else
                     Console.WriteLine("Connected:" + hubConnection.ConnectionId);
             }).Wait();

            Console.WriteLine(hubConnection.ConnectionId);

            myHub.On<string>("updateSettings", param =>
            {
                Console.WriteLine(param);
                var data = JsonConvert.DeserializeObject<SettingsDto>(param);
                settingsDto.ContexID = data.ContexID;
                settingsDto.IMEI = data.IMEI;
                settingsDto.Period = data.Period;
                settingsDto.Scale = data.Scale;
                settingsDto.ServerIPFTP = data.ServerIPFTP;
                settingsDto.ServerPortFTP = data.ServerPortFTP;
                settingsDto.ServerUrl = data.ServerUrl;
                settingsDto.VehicleBrand = data.VehicleBrand;
                settingsDto.VehicleModel = data.VehicleModel;
                settingsDto.VehicleSetupState = data.VehicleSetupState;
                settingsDto.VehicleYear = data.VehicleYear;
                settingsDto.WindScreenAngle = data.WindScreenAngle;
                settingsDto.ZeroCoor_x = data.ZeroCoor_x;
                settingsDto.ZeroCoor_y = data.ZeroCoor_y;
                settingsDto.ZeroCoor_z = data.ZeroCoor_z;
                GetDeviceSettingsCall(param);
            });

            myHub.On<string>("callDeviceSettings", param =>
            {
                Console.WriteLine(param);
                GetDeviceSettingsCall(param);
            });
                myHub.On<string>("callDeleteFileByName", param =>
            {
                Console.WriteLine("FİLE  DELETE");
                GetDeviceSettingsCall(param);
            });
            myHub.On<string>("getAllFileCallList", param =>
            {
                Console.WriteLine(param);
              GetSendFileList(param);
            });


            //myHub.Invoke<string>("DoSomething", "I'm doing something!!!").Wait();

            #region Handle Connection Lifetime Events
            //Raised when the client detects a slow or frequently dropping connection.
            hubConnection.ConnectionSlow += () => Console.WriteLine("Connection slow");
            //Raised when the connection has disconnected.
            hubConnection.Closed += () => Console.WriteLine("Connection stopped");
            //Raised when the underlying transport has reconnected.
            hubConnection.Reconnected += () => Console.WriteLine("Reconnected");
            //Raised when the underlying transport begins reconnecting.
            hubConnection.Reconnecting += () => Console.WriteLine("Reconnecting");
            //Raised when the connection state changes. Provides the old state and the new state.
            //ConnectionState Enumeration: Connecting, Connected, Reconnecting, Disconnected
            hubConnection.StateChanged += (change) => Console.WriteLine("ConnectionState changed from: " + change.OldState + " to " + change.NewState);
            //Raised when any data is received on the connection. Provides the received data.
            hubConnection.Received += (data) => Console.WriteLine("Connection received" + data);
            #endregion Handle Connection Lifetime Events

            //To handle errors that SignalR raises, you can add a handler for the Error event on the connection object.
            hubConnection.Error += ex => Console.WriteLine("SignalR error: {0}", ex.Message);

            Console.Read();
            hubConnection.Stop();
        }
        public static void GetDeviceSettingsCall(string ContextId)
        {
            settingsDto.ContexID = ContextId;
            settingsDto.IMEI = IMEI;
            settingsDto.Period = -1;
            settingsDto.Scale = -1;
            settingsDto.ServerIPFTP = "80";
            settingsDto.ServerPortFTP = 10;
            settingsDto.ServerUrl = "178.10.200.116:80";
            settingsDto.VehicleBrand = "MMV";
            settingsDto.VehicleModel = "Model2019";
            settingsDto.VehicleSetupState = 1;
            settingsDto.VehicleYear = "1999";
            settingsDto.WindScreenAngle = 2;
            settingsDto.ZeroCoor_x = 1;
            settingsDto.ZeroCoor_y = 1;
            settingsDto.ZeroCoor_z = 1;
            var data = JsonConvert.SerializeObject(settingsDto);
            myHub.Invoke<string>("GetDeviceSettings", data).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error calling send: {0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine(task.Result);
                }
            });

        }
        public  static  void GetSendFileList(string ContextID)
        {

             List<FileRequestDto> fileRequestDtos = new List<FileRequestDto>();

            fileRequestDtos.Add(new FileRequestDto() { DeviceImei=IMEI, Path= "/sdcard/ROTA/Video/15.03.2021_13-13-34.mp4*/sdcard/ROTA/Video/15.03.2021_13-18-34.mp4*/sdcard/ROTA/Video/15.03.2021_13-08-32.mp4*/sdcard/ROTA/System/12032021SystemLog*/sdcard/ROTA/System/DeviceSetup.txt*/sdcard/ROTA/System/15032021SystemLog*/sdcard/ROTA/Location/12032021_Loc_Log.txt*/sdcard/ROTA/Location/15032021_Loc_Log.txt", UserContext=ContextID});
          
            var data = JsonConvert.SerializeObject(fileRequestDtos[0]);
            myHub.Invoke<string>("GetAllFileList", data).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error calling send: {0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine(task.Result);
                }
            });
        }
    }
}
