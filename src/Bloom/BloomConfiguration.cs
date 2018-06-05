using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Bloom.Configuration
{
    public class BloomConfiguration
    {
        internal const long DefaultBloomFilterSize = 1 * (long)BloomFilterSizeMultipliers.MegaBytes;
        internal static readonly List<Type> DefaultHashAlgorithms = new List<Type>(){typeof(MD5CryptoServiceProvider),
                                                        typeof(SHA1CryptoServiceProvider),
                                                        typeof(SHA256CryptoServiceProvider)};

        private long? _bloomFilterSize;
        private List<Type> _hashAlgorithms;

        /// <summary>
        /// Gets or sets the size of the Bloom filter. Default is 1MB.
        /// </summary>
        /// <value> Default Bloom filter size.</value>
        public long BloomFilterSize
        {
            get { return _bloomFilterSize ?? DefaultBloomFilterSize; }
            set { _bloomFilterSize = value; }
        }

        public List<Type> HashAlgorithms
        {
            get { return _hashAlgorithms ?? DefaultHashAlgorithms; }
            set { _hashAlgorithms = value; }
        }
    }
}