using System;
using System.Diagnostics;
using DeviceId.Components;

namespace DeviceId
{
    /// <summary>
    /// Extension methods for <see cref="DeviceIdBuilder"/>.
    /// </summary>
    public static class DeviceIdBuilderExtensions
    {
        private static PlatformID Platform { get; } = Environment.OSVersion.Platform;


        public static Process Process { get; }


        static DeviceIdBuilderExtensions()
        {
            if (Platform == PlatformID.Unix || Platform == PlatformID.MacOSX)
            {
                Process = new Process();

                var pi = new ProcessStartInfo();


                pi.RedirectStandardInput = true;
                pi.RedirectStandardOutput = true;

                pi.FileName = "bash";

                Process.StartInfo = pi;

                Process.Start();
                Process.StandardInput.AutoFlush = true;
            }
            
        }


        /// <summary>
        /// Use the specified formatter.
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to use the formatter.</param>
        /// <param name="formatter">The <see cref="IDeviceIdFormatter"/> to use.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder UseFormatter(this DeviceIdBuilder builder, IDeviceIdFormatter formatter)
        {
            builder.Formatter = formatter;
            return builder;
        }

        /// <summary>
        /// Adds the specified component to the device identifier.
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
        /// <param name="component">The <see cref="IDeviceIdComponent"/> to add.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder AddComponent(this DeviceIdBuilder builder, IDeviceIdComponent component)
        {
            builder.Components.Add(component);
            return builder;
        }

        /// <summary>
        /// Adds the current user name to the device identifier. 
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder AddUserName(this DeviceIdBuilder builder)
        {
             
                return builder.AddComponent(new DeviceIdComponent("UserName", Environment.UserName));
            
           
        }

        /// <summary>
        /// Adds the machine name to the device identifier.
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder AddMachineName(this DeviceIdBuilder builder)
        {
            return builder.AddComponent(new DeviceIdComponent("MachineName", Environment.MachineName));
        }

        /// <summary>
        /// Adds the operating system version to the device identifier.
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder AddOSVersion(this DeviceIdBuilder builder)
        {
            return builder.AddComponent(new DeviceIdComponent("OSVersion", Environment.OSVersion.ToString()));
        }

        /// <summary>
        /// Adds the MAC address to the device identifier.
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder AddMacAddress(this DeviceIdBuilder builder)
        {
            if (Platform == PlatformID.Unix||Platform==PlatformID.MacOSX)
            {
                return builder.AddComponent(new LinuxDeviceIdComponent("MACAddress", LinuxDeviceType.MACAddress,Process));
            }
            else
            {
                return builder.AddComponent(new WmiDeviceIdComponent("MACAddress", "Win32_NetworkAdapterConfiguration",
                    "MACAddress"));
            }
        }

        /// <summary>
        /// Adds the processor ID to the device identifier.
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder AddProcessorId(this DeviceIdBuilder builder)
        { if (Platform == PlatformID.Unix||Platform==PlatformID.MacOSX)
            {
                return builder.AddComponent(new LinuxDeviceIdComponent("ProcessorId", LinuxDeviceType.ProcessorId,Process));
            }
            else
            {
                return builder.AddComponent(new WmiDeviceIdComponent("ProcessorId", "Win32_Processor", "ProcessorId"));
            }
        }

        /// <summary>
        /// Adds the motherboard serial number to the device identifier.
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder AddMotherboardSerialNumber(this DeviceIdBuilder builder)
        {
            if (Platform == PlatformID.Unix||Platform==PlatformID.MacOSX)
            {
                return builder.AddComponent(new LinuxDeviceIdComponent("MotherboardSerialNumber", LinuxDeviceType.ProcessorId,Process));
            }
            else
            {
                return builder.AddComponent(new WmiDeviceIdComponent("MotherboardSerialNumber", "Win32_BaseBoard",
                    "SerialNumber"));
            }
        }

        /// <summary>
        /// Adds the system UUID to the device identifier.
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder AddSystemUUID(this DeviceIdBuilder builder)
        {
            if (Platform == PlatformID.Unix||Platform==PlatformID.MacOSX)
            {
                return builder.AddComponent(new LinuxDeviceIdComponent("SystemUUID", LinuxDeviceType.SystemUUID,Process));
            }
            else
            {
                return builder.AddComponent(new WmiDeviceIdComponent("SystemUUID", "Win32_ComputerSystemProduct",
                    "UUID"));
            }
        }

        /// <summary>
        /// Adds the system drive's serial number to the device identifier.
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder AddSystemDriveSerialNumber(this DeviceIdBuilder builder)
        {
            return builder.AddComponent(new SystemDriveSerialNumberDeviceIdComponent());
        }

        /// <summary>
        /// Adds a file-based token to the device identifier.
        /// </summary>
        /// <param name="builder">The <see cref="DeviceIdBuilder"/> to add the component to.</param>
        /// <param name="path">The path of the token.</param>
        /// <returns>The <see cref="DeviceIdBuilder"/> instance.</returns>
        public static DeviceIdBuilder AddFileToken(this DeviceIdBuilder builder, string path)
        {
            var name = String.Concat("FileToken", path.GetHashCode());
            return builder.AddComponent(new FileTokenDeviceIdComponent(name, path));
        }
    }
}
