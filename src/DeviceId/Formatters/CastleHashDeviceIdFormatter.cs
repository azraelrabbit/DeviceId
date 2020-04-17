using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DeviceId.Formatters
{
    /// <summary>
    /// An implementation of <see cref="IDeviceIdFormatter"/> that combines the components into a hash.
    /// </summary>
    public class CastleHashDeviceIdFormatter : IDeviceIdFormatter
    {

        private string _salt;
        /// <summary>
        /// The <see cref="IByteArrayEncoder"/> to use to encode the resulting hash.
        /// </summary>
        private readonly IByteArrayEncoder _byteArrayEncoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashDeviceIdFormatter"/> class.
        /// </summary>

        /// <param name="byteArrayEncoder">The <see cref="IByteArrayEncoder"/> to use to encode the resulting hash.</param>
        public CastleHashDeviceIdFormatter(string salt, IByteArrayEncoder byteArrayEncoder)
        {
            _salt = salt ?? throw new ArgumentNullException(nameof(salt));
            _byteArrayEncoder = byteArrayEncoder ?? throw new ArgumentNullException(nameof(byteArrayEncoder));
        }

        /// <summary>
        /// Returns the device identifier string created by combining the specified <see cref="IDeviceIdComponent"/> instances.
        /// </summary>
        /// <param name="components">A sequence containing the <see cref="IDeviceIdComponent"/> instances to combine into the device identifier string.</param>
        /// <returns>The device identifier string.</returns>
        public string GetDeviceId(IEnumerable<IDeviceIdComponent> components)
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            var value = String.Join(",", components.OrderBy(x => x.Name).Select(x => x.GetValue()));

            var hash = crypto.Security.ComputeHash(value, _salt);
            var hashbytes = Encoding.UTF8.GetBytes(hash);
            return _byteArrayEncoder.Encode(hashbytes);

        }
    }
}
