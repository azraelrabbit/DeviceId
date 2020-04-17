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
        private  Process process;

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

            //process = processs;

              process = new Process();

                var pi = new ProcessStartInfo();


                pi.RedirectStandardInput = true;
                pi.RedirectStandardOutput = true;

                pi.FileName = "bash";

                process.StartInfo = pi;

                process.Start();
                process.StandardInput.AutoFlush = true;
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

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                    process.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~LinuxDeviceIdComponent()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }



    public enum LinuxDeviceType
    {
        MACAddress,
        ProcessorId,
        MotherboardSerialNumber,
        SystemUUID
    }


}
