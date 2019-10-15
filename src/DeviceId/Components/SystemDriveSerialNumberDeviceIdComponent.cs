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

        private static Process process;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemDriveSerialNumberDeviceIdComponent"/> class.
        /// </summary>
        public SystemDriveSerialNumberDeviceIdComponent()
        {
            PlatForm = Environment.OSVersion.Platform;

            if (PlatForm == PlatformID.Unix || PlatForm == PlatformID.MacOSX)
            {
                process = DeviceIdBuilderExtensions.Process;
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

                var diskline = process.StandardOutput.ReadLine()?.Replace("  ","").Replace("-","");

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
    }
}
