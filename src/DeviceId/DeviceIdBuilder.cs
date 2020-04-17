using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using DeviceId.Encoders;
using DeviceId.Formatters;
using DeviceId.Internal;

namespace DeviceId
{
    /// <summary>
    /// Provides a fluent interface for constructing unique device identifiers.
    /// </summary>
    public class DeviceIdBuilder:IDisposable
    {


        /// <summary>
        /// Gets or sets the formatter to use.
        /// </summary>
        public IDeviceIdFormatter Formatter { get; set; }

        /// <summary>
        /// A set containing the components that will make up the device identifier.
        /// </summary>
        public ISet<IDeviceIdComponent> Components { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceIdBuilder"/> class.
        /// </summary>
        public DeviceIdBuilder()
        {
            Formatter = new HashDeviceIdFormatter(() => SHA256.Create(), new Base64UrlByteArrayEncoder());
            Components = new HashSet<IDeviceIdComponent>(new DeviceIdComponentEqualityComparer());
        }

        /// <summary>
        /// Returns a string representation of the device identifier.
        /// </summary>
        /// <returns>A string representation of the device identifier.</returns>
        public override string ToString()
        {
            if (Formatter == null)
            {
                throw new InvalidOperationException($"The {nameof(Formatter)} property must not be null in order for {nameof(ToString)} to be called.");
            }

            return Formatter.GetDeviceId(Components);
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
                    Formatter=null;

                    if (Components != null)
                    {
                        foreach(var component in Components)
                        {
                            component.Dispose();
                        }
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DeviceIdBuilder()
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
