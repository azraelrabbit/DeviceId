using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace DeviceId.Components
{
    /// <summary>
    /// An implementation of <see cref="IDeviceIdComponent"/> that uses the system drive's serial number.
    /// </summary>
    public class SystemDriveSerialNumberDeviceIdComponent : IDeviceIdComponent
    {
        private PlatformID PlatForm { get; }


        private string cmdLsDrive = "blkid | grep -oP 'UUID=\"\\K[^\"]+' | sha256sum | awk '{print $1}'"; //"ls -G";

        private Process process;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemDriveSerialNumberDeviceIdComponent"/> class.
        /// </summary>
        public SystemDriveSerialNumberDeviceIdComponent()
        {
            PlatForm = Environment.OSVersion.Platform;

            if (PlatForm == PlatformID.Unix || PlatForm == PlatformID.MacOSX)
            {
                process = new Process();

                var pi = new ProcessStartInfo();


                pi.RedirectStandardInput = true;
                pi.RedirectStandardOutput = true;

                pi.FileName = "bash";

                process.StartInfo = pi;

                process.Start();
                process.StandardInput.AutoFlush = true;
            }
        }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        public string Name => "SystemDriveSerialNumber";

        /// <summary>
        /// Gets the component value.
        /// </summary>
        /// <returns>The component value.</returns>
        public string GetValue()
        {
            if (PlatForm == PlatformID.Win32NT || PlatForm == PlatformID.Win32S ||
                PlatForm == PlatformID.Win32Windows || PlatForm == PlatformID.WinCE)
            {
                var systemLogicalDiskDeviceId = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 2);

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_LogicalDisk where DeviceId = '{systemLogicalDiskDeviceId}'"))
                {
                    foreach (ManagementObject disk in searcher.Get())
                    {
                        foreach (ManagementObject partition in disk.GetRelated("Win32_DiskPartition"))
                        {
                            foreach (ManagementObject drive in partition.GetRelated("Win32_DiskDrive"))
                            {
                                var serialNumber = drive["SerialNumber"] as string;
                                return serialNumber;
                            }
                        }
                    }
                }
            }
            else if (PlatForm == PlatformID.Unix)
            {
                //linux

                process.StandardInput.WriteLine(cmdLsDrive);


                var diskline = process.StandardOutput.ReadLine()?.Replace("  ", "").Replace("-", "");

                ////KiB Swap: 30285596 total, 30203564 free,    82032 used.  8403944 avail Mem
                //3ef2b806-efd7-4eef-aaa2-2584909365ff  9de3ff9f-ac50-4da7-8af2-13b712002e15  a97670dd-ed1a-4da3-8ccd-180630a6bfd9

                //var diskarr = diskline.Split(',').Select(p => string.IsNullOrEmpty(p.Trim()));
                return diskline;
            }
            else
            {
                //macos
                //process.StandardInput.WriteLine(cmdLsDrive);

                Console.WriteLine("Macosx not Implement");
                var diskline = "";// process.StandardOutput.ReadLine()?.Replace("  ","").Replace("-","");

                //3ef2b806-efd7-4eef-aaa2-2584909365ff  9de3ff9f-ac50-4da7-8af2-13b712002e15  a97670dd-ed1a-4da3-8ccd-180630a6bfd9
                //var diskarr = diskline.Split(',').Select(p => string.IsNullOrEmpty(p.Trim()));
                return diskline;
            }


            return null;
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
                    if (PlatForm == PlatformID.Unix || PlatForm == PlatformID.MacOSX)
                    {
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
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~SystemDriveSerialNumberDeviceIdComponent()
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
}
