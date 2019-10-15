using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace DeviceId.Components
{
    public class LinuxDeviceIdComponent : IDeviceIdComponent
    {
        private static Process process;

        public string Name { get; }


        private readonly LinuxDeviceType deviceType;


        public LinuxDeviceIdComponent(string name, LinuxDeviceType linuxdeviceType,Process processs)
        {

            Name = name;
            deviceType = linuxdeviceType;

            //process = new Process();

            //var pi = new ProcessStartInfo();


            //pi.RedirectStandardInput = true;
            //pi.RedirectStandardOutput = true;

            //pi.FileName = "bash";

            //process.StartInfo = pi;

            //process.Start();
            //process.StandardInput.AutoFlush = true;

            process = processs;
        }


        public string GetValue()
        {
            List<string> values = new List<string>();

            
            switch (deviceType)
            {
                case LinuxDeviceType.MACAddress:
                    values = GetMacaddress();
                    break;
                case LinuxDeviceType.ProcessorId:
                    values = GetProcessorId();
                    break;
                case LinuxDeviceType.SystemUUID:
                    values = GetSystemUUID();
                    break;
                case LinuxDeviceType.MotherboardSerialNumber:
                    values = GetMotherboardSN();
                    break;
                default:
                    values=new List<string>();
                    break;
                    
            }
 

            values.Sort();

            return String.Join(",", values);
        }

        //board_serial
        private string cmdBoardSnLine = "cat /sys/class/dmi/id/board_serial";
        private List<string> GetMotherboardSN()
        {
            process.StandardInput.WriteLine(cmdBoardSnLine);

            var uuidLine = process.StandardOutput.ReadLine()?.Replace("  ", "").Replace("-", "");

            return new List<string>(){uuidLine};
        }

        private string cmdSystemUUIDLine = "cat /sys/class/dmi/id/product_uuid";
        private List<string> GetSystemUUID()
        {
            process.StandardInput.WriteLine(cmdSystemUUIDLine);

            var uuidLine = process.StandardOutput.ReadLine()?.Replace("  ", "").Replace("-", "");

            return new List<string>(){uuidLine};
        }

        //dmidecode -t 4 | grep ID |sort -u |awk -F': ' '{print $2}'|xargs |sed 's/ //g'
        private string cmdCPUIDLine = "dmidecode -t 4 | grep ID |sort -u |awk -F': ' '{print $2}'|xargs |sed 's/ //g'";
        private List<string> GetProcessorId() 
        {
            process.StandardInput.WriteLine(cmdCPUIDLine);

            var uuidLine = process.StandardOutput.ReadLine()?.Replace("  ", "").Replace("-", "");

            return new List<string>(){uuidLine};
        }

    
        private List<string> GetMacaddress()
        {

            var ethlist = NetworkInterface.GetAllNetworkInterfaces();

            var valueList=new List<string>();

            foreach (var  eth in ethlist)
            {
                var addr = eth.GetPhysicalAddress()?.ToString();
                if (!string.IsNullOrEmpty(addr) && addr!="00000000" && addr!="000000000000")
                {
                    valueList.Add(addr);
                }
            }

            return valueList;
        }
    }



    public enum LinuxDeviceType
    {
        MACAddress,
        ProcessorId,
        MotherboardSerialNumber,
        SystemUUID
    }


}
