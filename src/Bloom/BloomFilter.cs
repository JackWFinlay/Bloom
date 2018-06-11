using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Bloom.Configuration;

namespace Bloom
{
    public class BloomFilter : IBloomFilter
    {
        private const int NumBytesInInt64 = 8;
        private readonly long _size;
        private readonly bool[] _filter;

        public long Size => _size;

        public bool[] Filter => _filter;

        public List<Type> HashAlgorithms => _hashAlgorithms;

        private readonly List<Type> _hashAlgorithms;

        public BloomFilter()
        {
            _size = BloomConfiguration.DefaultBloomFilterSize;
            _hashAlgorithms = BloomConfiguration.DefaultHashAlgorithms;
            _filter = new bool[_size];
        }

        public BloomFilter(BloomConfiguration configuration)
        {
            _size = configuration.BloomFilterSize;
            _hashAlgorithms = configuration.HashAlgorithms;
            _filter = new bool[_size];
        }

        public virtual void Add(byte[] item)
        {
            IEnumerable<long> listOfIndexes = GetIndexesOfHashResults(item);
            foreach (long index in listOfIndexes)
            {
                _filter[index] = true;
            }
        }

        public virtual bool Contains(byte[] item)
        {
            IEnumerable<long> listOfIndexes = GetIndexesOfHashResults(item);
            bool containsAll = true;

            foreach (long index in listOfIndexes)
            {
                if (!_filter[index])
                {
                    containsAll = false;
                    break;
                }
            }

            return containsAll;
        }

        private IEnumerable<long> GetIndexesOfHashResults(byte[] item)
        {
            List<long> listOfIndexes = new List<long>();

            foreach (Type algorithmType in _hashAlgorithms)
            {
                HashAlgorithm hashAlgorithm = (HashAlgorithm)Activator.CreateInstance(algorithmType);

                byte[] hash = hashAlgorithm.ComputeHash(item);

                long index = ReduceByteArrayToIndexWithinBoundsOfFilter(hash);

                listOfIndexes.Add(index);
            }

            return listOfIndexes;
        }

        private long ReduceByteArrayToIndexWithinBoundsOfFilter(byte[] byteArray)
        {
            long byteArrayAsCompactedLong = GetLongFromByteArray(byteArray);
            long indexWithinBounds = Math.Abs(ReduceLongToBoundsOfFilter(byteArrayAsCompactedLong));

            return indexWithinBounds;
        }

        private static long GetLongFromByteArray(byte[] byteArray)
        {
            long result = 0;
            bool lastBytes = false;

            for (int i = 0; i < byteArray.Length; i += NumBytesInInt64)
            {
                int numberOfBytesToTake = NumBytesInInt64;
                byte[] eightBytes = new byte[NumBytesInInt64];


                if ((i + NumBytesInInt64) > byteArray.Length)
                {
                    numberOfBytesToTake = NumBytesInInt64 - Math.Abs(i - byteArray.Length) - 1;
                    lastBytes = true;
                }

                for (int j = 0; j < numberOfBytesToTake; j++)
                {
                    eightBytes[j] = byteArray[i + j];
                }

                result += BitConverter.ToInt64(eightBytes, 0);

                if (lastBytes)
                {
                    break;
                }
            }

            return result;
        }

        private long ReduceLongToBoundsOfFilter(long byteArrayAsCompactedLong)
        {
            return byteArrayAsCompactedLong % _size;
        }
    }
}
