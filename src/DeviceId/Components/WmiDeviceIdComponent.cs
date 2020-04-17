using System;
using System.Collections.Generic;
using System.Management;

namespace DeviceId.Components
{
    /// <summary>
    /// An implementation of <see cref="IDeviceIdComponent"/> that retrieves data from a WMI class.
    /// </summary>
    public class WmiDeviceIdComponent : IDeviceIdComponent
    {
        /// <summary>
        /// The name of the component.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The WMI class name.
        /// </summary>
        private readonly string _wmiClass;

        /// <summary>
        /// The WMI property name.
        /// </summary>
        private readonly string _wmiProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiDeviceIdComponent"/> class.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="wmiClass">The WMI class name.</param>
        /// <param name="wmiProperty">The WMI property name.</param>
        public WmiDeviceIdComponent(string name, string wmiClass, string wmiProperty)
        {
            _name = name;
            _wmiClass = wmiClass;
            _wmiProperty = wmiProperty;
        }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Gets the component value.
        /// </summary>
        /// <returns>The component value.</returns>
        public string GetValue()
        {
            List<string> values = new List<string>();

            using (ManagementClass mc = new ManagementClass(_wmiClass))
            {
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    try
                    {
                        string value = mo[_wmiProperty] as string;
                        if (value != null)
                        {
                            values.Add(value);
                        }
                    }
                    finally
                    {
                        mo.Dispose();
                    }
                }
            }

            values.Sort();

            return String.Join(",", values);
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
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~WmiDeviceIdComponent()
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
