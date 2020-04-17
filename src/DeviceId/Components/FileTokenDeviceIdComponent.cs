using System;
using System.IO;
using System.Text;

namespace DeviceId.Components
{
    /// <summary>
    /// An implementation of <see cref="IDeviceIdComponent"/> that retrieves its value from a file.
    /// </summary>
    /// <remarks>
    /// If the file exists, the contents of that file will be used as the component value.
    /// If the file does not exist, a new file will be created and populated with a new GUID,
    /// which will be used as the component value.
    /// </remarks>
    public class FileTokenDeviceIdComponent : IDeviceIdComponent
    {
        /// <summary>
        /// The name of the component.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The path where the token will be stored.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTokenDeviceIdComponent"/> class.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="path">The path where the component will be stored.</param>
        public FileTokenDeviceIdComponent(string name, string path)
        {
            _name = name;
            _path = path;
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
            string value = Guid.NewGuid().ToString().ToUpper();

            if (File.Exists(_path))
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(_path);
                    value = Encoding.ASCII.GetString(bytes);
                }
                catch { }
            }
            else
            {
                try
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(value);
                    File.WriteAllBytes(_path, bytes);
                }
                catch { }
            }

            return value;
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
        // ~FileTokenDeviceIdComponent()
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
