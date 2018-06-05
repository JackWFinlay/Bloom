using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Bloom.Configuration;

namespace Bloom
{
    public class BloomFilter : IBloomFilter
    {
        private const int _numBytesInInt64 = 8;
        public readonly long Size;
        public readonly bool[] Filter;

        public readonly List<Type> HashAlgorithms;

        public BloomFilter()
        {
            Size = BloomConfiguration.DefaultBloomFilterSize;
            HashAlgorithms = BloomConfiguration.DefaultHashAlgorithms;
            Filter = new bool[Size];
        }

        public BloomFilter(BloomConfiguration configuration)
        {
            Size = configuration.BloomFilterSize;
            HashAlgorithms = configuration.HashAlgorithms;
            Filter = new bool[Size];
        }

        public void Add(byte[] item)
        {
            List<long> listOfIndexes = GetIndexesOfHashResults(item);
            foreach (long index in listOfIndexes)
            {
                Filter[index] = true;
            }
        }

        public bool Contains(byte[] item)
        {
            List<long> listOfIndexes = GetIndexesOfHashResults(item);
            bool containsAll = true;

            foreach (long index in listOfIndexes)
            {
                if (!Filter[index])
                {
                    containsAll = false;
                    break;
                }
            }

            return containsAll;
        }

        private List<long> GetIndexesOfHashResults(byte[] item)
        {
            List<long> listOfIndexes = new List<long>();

            foreach (Type algorithmType in HashAlgorithms)
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

        private long GetLongFromByteArray(byte[] byteArray)
        {
            long result = 0;
            bool lastBytes = false;

            for (int i = 0; i < byteArray.Length; i += _numBytesInInt64)
            {
                int numberOfBytesToTake = _numBytesInInt64;
                byte[] eightBytes = new byte[_numBytesInInt64];


                if ((i + _numBytesInInt64) > byteArray.Length)
                {
                    numberOfBytesToTake = _numBytesInInt64 - Math.Abs(i - byteArray.Length) - 1;
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
            return byteArrayAsCompactedLong % Size;
        }
    }
}
