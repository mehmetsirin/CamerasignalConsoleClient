using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGNALRCONSOLECLIENT._Console.Model
{
    public class SettingsDto
    {
        public string IMEI { get; set; }
        public int Period { get; set; }
        public string ServerIPTCP { get; set; }
        public int ServerPortTCP { get; set; }
        public string ServerIPFTP { get; set; }
        public int ServerPortFTP { get; set; }
        public string ServerUrl { get; set; }
        public string VehicleBrand { get; set; }
        public string VehicleModel { get; set; }
        public int VehicleSetupState { get; set; }
        public double ZeroCoor_y { get; set; }
        public double ZeroCoor_x { get; set; }
        public string VehicleYear { get; set; }
        public double ZeroCoor_z { get; set; }
        public double Scale { get; set; }
        public double WindScreenAngle { get; set; }
        public string ContexID { get; set; }

    }
}
